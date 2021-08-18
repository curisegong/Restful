using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Restful.Common.Cache;
using Restful.Common.IoC;
using Restful.DataBase;
using Restful.Model;
using SqlSugar;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Restful.Common;

namespace Restful.Service
{
    [IocAttribute(ServiceLifetime.Scoped, "", typeof(TableViewService))]
    public class TableViewService
    {

        protected static Regex Regx { get; } = new Regex(@"(.+?)=(.+?),");

        protected static List<string> ConnectOperators { get; } = new List<string>() { "(", ")", "OR" };


        protected static List<string> ArgFunctions { get; } = new List<string>() { "MAX", "MIN", "SUM", "AVG", "COUNT" };
        private SqlSugarClient Db { get; } = TTStudioDbContext.Db;

        private readonly Restful.Common.Cache.ICacheService memoryCache;

        private readonly IServiceProvider _serviceProvider;

        public TableViewService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            memoryCache = GetCacheInstance();

        }

        public virtual void CacheTableViewInfo(string sql, string ownerOrSchema)
        {
            SetTableViewInfoCache(Db.SqlQueryable<TableViewInfo>(sql).AddParameters(new List<SugarParameter>() { new SugarParameter("@OWNER", ownerOrSchema) }).ToList());
        }

        /// <summary>
        /// default for oracle database 
        /// </summary>
        /// <param name="ownerOrSchema"></param>
        public virtual void CacheTableViewInfo(string ownerOrSchema)
        {
            CacheTableViewInfo(@"SELECT TABLE_NAME TABLEVIEWNAME,COLUMN_NAME COLUMNNAME,CASE WHEN DATA_TYPE='VARCHAR2' THEN 'VARCHAR' ELSE DATA_TYPE END  DATATYPE FROM ALL_TAB_COLS WHERE OWNER=@OWNER ORDER BY TABLE_NAME", ownerOrSchema);
        }


        protected virtual void SetTableViewInfoCache(List<TableViewInfo> list)
        {
            memoryCache.Add<List<TableViewInfo>>(CacheConstrant.TableViewInfoCache, list);
        }

        public List<TableViewInfo> GetTableViewInfoCache()
        {
            return memoryCache.Get<List<TableViewInfo>>(CacheConstrant.TableViewInfoCache);
        }

        public string GetQueryResult(UrlParameter urlParameter)
        {
            List<TableViewInfo> tableViewInfos = GetTableViewInfoCache();
            string checkResult = CheckUrlParameter(urlParameter, tableViewInfos);
            if (!string.IsNullOrWhiteSpace(checkResult)) return checkResult;


            List<SugarParameter> parameters = new List<SugarParameter>();
            string sql = GetQuerySql(urlParameter, parameters);
            QueryGrid queryGrid = new QueryGrid();
            int total = 0;
            queryGrid.Data = Db.SqlQueryable<object>(sql).AddParameters(parameters).ToPageList(urlParameter.PageNumber, urlParameter.PageSize, ref total);
            queryGrid.Total = total;
            queryGrid.ResultCode = 1;
            return JsonConvert.SerializeObject(queryGrid);
        }

        public string DoAdd(UrlParameter urlParameter)
        {
            List<TableViewInfo> tableViewInfos = GetTableViewInfoCache();
            string checkResult = CheckUrlParameter(urlParameter, tableViewInfos);
            if (!string.IsNullOrWhiteSpace(checkResult)) return checkResult;
            List<SugarParameter> parameters = new List<SugarParameter>();
            string sql = GetInsertSql(urlParameter, parameters);

            int total = Db.Ado.ExecuteCommand(sql, parameters);

            return JsonConvert.SerializeObject(new { ResultCode = 1, Total = total });
        }

        public string DoUpdate(UrlParameter urlParameter)
        {
            List<TableViewInfo> tableViewInfos = GetTableViewInfoCache();
            string checkResult = CheckUrlParameter(urlParameter, tableViewInfos);
            if (!string.IsNullOrWhiteSpace(checkResult)) return checkResult;
            checkResult = CheckUpdateDeleteParameter(urlParameter);
            if (!string.IsNullOrWhiteSpace(checkResult)) return checkResult;
            List<SugarParameter> parameters = new List<SugarParameter>();
            string sql = GetUpdateSql(urlParameter, parameters);


            int total = Db.Ado.ExecuteCommand(sql, parameters);

            return JsonConvert.SerializeObject(new { ResultCode = 1, Total = total });
        }

        public string DoDelete(UrlParameter urlParameter)
        {
            List<TableViewInfo> tableViewInfos = GetTableViewInfoCache();
            string checkResult = CheckUrlParameter(urlParameter, tableViewInfos);
            if (!string.IsNullOrWhiteSpace(checkResult)) return checkResult;
            checkResult = CheckUpdateDeleteParameter(urlParameter);
            if (!string.IsNullOrWhiteSpace(checkResult)) return checkResult;
            List<SugarParameter> parameters = new List<SugarParameter>();
            string sql = GetDeleteSql(urlParameter, parameters);
            int total = Db.Ado.ExecuteCommand(sql, parameters);

            return JsonConvert.SerializeObject(new { ResultCode = 1, Total = total });

        }

        /// <summary>
        /// check the parmaeter input were correct.prevent SQL injection
        /// </summary>
        /// <param name="urlParameter"></param>
        /// <param name="tableViewInfos"></param>
        /// <returns></returns>
        protected string CheckUrlParameter(UrlParameter urlParameter, List<TableViewInfo> tableViewInfos)
        {
            if (string.IsNullOrWhiteSpace(urlParameter.TableViewName))
                return string.Concat(urlParameter.TableViewName, " can not be empty");
            if (tableViewInfos.Where(m => m.TableViewName == urlParameter.TableViewName.Trim()).Count() == 0)
                return string.Concat("table or view name ", urlParameter.TableViewName, " is not exists");
            //get all columns of the table
            string[] columns = tableViewInfos.Where(m => m.TableViewName == urlParameter.TableViewName.Trim()).Select(m => m.ColumnName).ToArray();
            //verify the group by column is exists
            if (!string.IsNullOrWhiteSpace(urlParameter.Columns))
            {
                string[] inputColumns = urlParameter.Columns.Split(",");

                string[] intersectColumns = inputColumns.Except(columns).ToArray();

                if (intersectColumns.Count() > 0)
                    return string.Concat("table or view not contains columns ", string.Join(',', intersectColumns));

            }
            else
            {
                urlParameter.Columns = "*";
            }
            if (!string.IsNullOrWhiteSpace(urlParameter.GroupBy))
            {
                string[] groupBys = urlParameter.GroupBy.Split(",");

                string[] intersectColumns = groupBys.Except(columns).ToArray();

                if (intersectColumns.Length > 0)
                    return string.Concat("table or view not contains columns ", string.Join(',', intersectColumns));

            }

            if (!string.IsNullOrWhiteSpace(urlParameter.SortBy))
            {
                string[] sortBys = urlParameter.SortBy.Split(",");//default asc, - will be desc


                string[] intersectColumns = sortBys.Except(columns).ToArray();

                if (intersectColumns.Length > 0)
                    return string.Concat("table or view not contains columns ", string.Join(',', intersectColumns));
            }

            //update insert columns 
            if (urlParameter.Parameters != null && urlParameter.Parameters.Count > 0)
            {
                string[] whereColumn = urlParameter.Parameters.Where(m => !string.IsNullOrWhiteSpace(m.Id)).Select(m => m.Id).ToArray();

                string[] intersectColumns = whereColumn.Except(columns).ToArray();

                if (intersectColumns.Length > 0)
                    return string.Concat("table or view not contains columns ", string.Join(',', intersectColumns));
                urlParameter.Parameters.ForEach(
                    m =>
                    {
                        if (!string.IsNullOrWhiteSpace(m.Id))
                            m.DateType = tableViewInfos.Where(n => n.ColumnName == m.Id && n.TableViewName == urlParameter.TableViewName).First().DataType;
                    }
                    );
            }

            //condition columns 
            if (urlParameter.Where != null && urlParameter.Where.Count > 0)
            {
                string[] whereColumn = urlParameter.Where.Where(m => !string.IsNullOrWhiteSpace(m.Id)).Select(m => m.Id).ToArray();

                string[] intersectColumns = whereColumn.Except(columns).ToArray();

                if (intersectColumns.Length > 0)
                    return string.Concat("table or view not contains columns ", string.Join(',', intersectColumns));
                urlParameter.Where.ForEach(
                    m =>
                    {
                        if (!string.IsNullOrWhiteSpace(m.Id))
                            m.DateType = tableViewInfos.Where(n => n.ColumnName == m.Id && n.TableViewName == urlParameter.TableViewName).First().DataType;
                    }
                    );

                int count = urlParameter.Where.Where(m => string.IsNullOrWhiteSpace(m.Id) && !ConnectOperators.Contains(string.Concat(m.OP.ToUpper()))).Count();

                if (count > 0)
                    return string.Concat("opertator can only be left parenthesis,right parenthesis,or & and  ", string.Join(',', intersectColumns));
            }


            if (!string.IsNullOrWhiteSpace(urlParameter.Args))
            {
                List<string> keyColumns = new List<string>();
                List<string> valueColumns = new List<string>();
                foreach (Match item in Regx.Matches(string.Concat(urlParameter.Args, ",")))
                {
                    if (item.Groups.Count != 3) return "arg function format is not correct";
                    keyColumns.Add(item.Groups[1].Value.ToUpper());
                    valueColumns.Add(item.Groups[2].Value);
                }

                //foreach (string keyValue in urlParameter.Args.Split(","))
                //{
                //    keyColumns.Add(keyValue.Split("=")[0].ToUpper());
                //    valueColumns.Add(keyValue.Split("=")[1]);
                //}

                string[] intersectColumns = valueColumns.Except(columns).ToArray();

                if (intersectColumns.Count() > 0)
                    return string.Concat("table or view not contains columns ", string.Join(',', intersectColumns));

                intersectColumns = keyColumns.Except(ArgFunctions).ToArray();

                if (intersectColumns.Count() > 0)
                    return string.Concat("so far we only suppor arg function for  ", string.Join(',', ArgFunctions));
            }

            return null;
        }

        protected string CheckUpdateDeleteParameter(UrlParameter urlParameter)
        {
            if (urlParameter.Where == null || urlParameter.Where.Count == 0)
            {
                return "when do delete,update action. the size of parameter where should  greater than 0";
            }
            return null;
        }

        protected string GetQuerySql(UrlParameter urlParameter, List<SugarParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ").Append(urlParameter.Columns).Append(" ");
            if (!string.IsNullOrWhiteSpace(urlParameter.Args))
            {
                sb.Append(",");
                foreach (Match item in Regx.Matches(string.Concat(urlParameter.Args, ",")))
                {
                    sb.Append(item.Groups[1].Value.ToUpper()).Append("(").Append(item.Groups[2].Value).Append(") ").Append(item.Groups[1].Value.ToUpper()).Append("_").Append(item.Groups[2].Value).Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append(" FROM ").Append(urlParameter.TableViewName).Append(" WHERE 1=1 ");

            sb.Append(GetWhereSql(urlParameter, parameters));

            if (!string.IsNullOrEmpty(urlParameter.GroupBy))
            {
                sb.Append(" GROUP BY ").Append(urlParameter.GroupBy);
            }

            if (!string.IsNullOrEmpty(urlParameter.SortBy))
            {
                sb.Append(" ORDER BY ").Append(urlParameter.SortBy.Replace("-", " DESC "));
            }


            return sb.ToString();
        }

        protected string GetInsertSql(UrlParameter urlParameter, List<SugarParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder valueSB = new StringBuilder();

            sb.Append("INSERT INTO ").Append(urlParameter.TableViewName).Append(" ( ");

            if (urlParameter.Parameters != null)
            {

                for (int index = 0; index < urlParameter.Parameters.Count; index++)
                {
                    if (!string.IsNullOrEmpty(urlParameter.Parameters[index].Id))
                    {
                        if (index < urlParameter.Parameters.Count - 1)
                        {
                            sb.Append(urlParameter.Parameters[index].Id).Append(",");
                        }
                        else
                        {
                            sb.Append(urlParameter.Parameters[index].Id).Append(") VALUES(");
                        }
                    }
                }

                for (int index = 0; index < urlParameter.Parameters.Count; index++)
                {
                    if (!string.IsNullOrEmpty(urlParameter.Parameters[index].Id))
                    {
                        if (index < urlParameter.Parameters.Count - 1)
                        {
                            sb.Append(GetInsertUpdateColumn(urlParameter.Parameters[index])).Append(",");
                        }
                        else
                        {
                            sb.Append(GetInsertUpdateColumn(urlParameter.Parameters[index])).Append(")");
                        }
                        GetInsertUpdateValue(urlParameter.Parameters[index], parameters);
                    }
                }
            }





            return sb.ToString();
        }

        protected string GetUpdateSql(UrlParameter urlParameter, List<SugarParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(" UPDATE ").Append(urlParameter.TableViewName).Append(" SET ");

            if (urlParameter.Parameters != null)
            {

                for (int index = 0; index < urlParameter.Parameters.Count; index++)
                {
                    if (!string.IsNullOrEmpty(urlParameter.Parameters[index].Id))
                    {
                        if (index < urlParameter.Parameters.Count - 1)
                        {
                            sb.Append(urlParameter.Parameters[index].Id).Append("=");
                            sb.Append(GetInsertUpdateColumn(urlParameter.Parameters[index])).Append(",");
                        }
                        else
                        {
                            sb.Append(urlParameter.Parameters[index].Id).Append("=");
                            sb.Append(GetInsertUpdateColumn(urlParameter.Parameters[index]));
                        }
                        GetInsertUpdateValue(urlParameter.Parameters[index], parameters);
                    }
                }
            }
            sb.Append(" WHERE 1=1 ");
            sb.Append(GetWhereSql(urlParameter, parameters));
            return sb.ToString();
        }

        protected string GetDeleteSql(UrlParameter urlParameter, List<SugarParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder valueSB = new StringBuilder();

            sb.Append(" DELETE FROM ").Append(urlParameter.TableViewName).Append(" WHERE 1=1 ");
            sb.Append(GetWhereSql(urlParameter, parameters));
            return sb.ToString();
        }


        protected string GetWhereSql(UrlParameter urlParameter, List<SugarParameter> parameters)
        {
            if (urlParameter.Where != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < urlParameter.Where.Count; index++)
                {
                    if (!string.IsNullOrEmpty(urlParameter.Where[index].Id))
                    {
                        if (index == 0 || !ConnectOperators.Contains(urlParameter.Where[index - 1].OP.ToUpper()))
                            sb.Append(" AND ").Append(GetWhereParameter(urlParameter.Where[index], parameters)).Append(" ");
                        else if (urlParameter.Where[index - 1].OP.ToUpper() == ")")
                        {
                            sb.Append(" AND ").Append(GetWhereParameter(urlParameter.Where[index], parameters)).Append(" ");
                        }
                        else
                            sb.Append(" ").Append(GetWhereParameter(urlParameter.Where[index], parameters)).Append(" ");
                    }
                    else
                    {
                        if (index == 0 || !ConnectOperators.Contains(urlParameter.Where[index - 1].OP.ToUpper()))
                        {
                            if (urlParameter.Where[index].OP.ToUpper() == ")")
                                sb.Append("  ").Append(urlParameter.Where[index].OP).Append(" ");
                            else if (urlParameter.Where[index].OP.ToUpper() != "OR")
                                sb.Append(" AND ").Append(urlParameter.Where[index].OP.ToUpper()).Append(" ");
                            else
                                sb.Append(urlParameter.Where[index].OP);
                        }
                        else
                        {
                            if (ConnectOperators.Contains(urlParameter.Where[index - 1].OP.ToUpper()))
                                sb.Append("  ").Append(urlParameter.Where[index].OP.ToUpper()).Append(" ");
                        }

                    }

                }
                return sb.ToString();
            }
            return "";
        }
        /// <summary>
        /// 根据操作符拼接SQL及参数
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string GetWhereParameter(Parameter item, List<SugarParameter> parameters)
        {

            StringBuilder sb = new StringBuilder();
            switch ((OperatorEnum)Enum.Parse(typeof(OperatorEnum), item.OP.ToUpper()))
            {
                case OperatorEnum.EQ:

                    sb.Append(item.Id).Append(" = @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, item.Value);
                    break;
                case OperatorEnum.EW:

                    sb.Append(item.Id).Append(" LIKE @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, string.Concat("%", item.Value));
                    break;
                case OperatorEnum.SW:

                    sb.Append(item.Id).Append(" LIKE @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, string.Concat(item.Value, "%"));
                    break;
                case OperatorEnum.GT:

                    sb.Append(item.Id).Append(" > @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, item.Value);
                    break;
                case OperatorEnum.LIKE:

                    sb.Append(item.Id).Append(" LIKE @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, string.Concat("%", item.Value, "%"));
                    break;
                case OperatorEnum.NEQ:

                    sb.Append(item.Id).Append(" <> @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, item.Value);
                    break;
                case OperatorEnum.LT:

                    sb.Append(item.Id).Append(" < @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, item.Value);
                    break;
                case OperatorEnum.GTE:

                    sb.Append(item.Id).Append(" >= @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, item.Value);
                    break;
                case OperatorEnum.LTE:

                    sb.Append(item.Id).Append(" <= @").Append(string.Concat(item.Id, parameters.Count)).ToString();
                    GetConditionValue(item, parameters, item.Value);
                    break;
                case OperatorEnum.IE:
                    sb.Append(item.Id).Append(" IS NOT NULL ").ToString();
                    break;
                case OperatorEnum.IS:
                    sb.Append(item.Id).Append(" IS  NULL ").ToString();
                    break;
                case OperatorEnum.IN:

                    sb.Append(item.Id).Append(" IN (@").Append(string.Concat(item.Id, parameters.Count)).Append(")").ToString();
                    GetConditionInValue(item, parameters, item.Value);
                    break;

            }
            return sb.ToString();
        }


        /// <summary>
        /// 按数据库类型可以重新按字段类型重写
        /// 如预设类型缺失，可以自己按字符串比较
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        protected virtual void GetConditionValue(Parameter item, List<SugarParameter> parameters, string value)
        {
            switch ((DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), item.DateType.ToUpper()))
            {
                case DataTypeEnum.DATE:
                    parameters.Add(new SugarParameter(string.Concat("TO_DATE(@", string.Concat(item.Id, parameters.Count), ",'YYYY-MM-DD')"), value));
                    break;
                case DataTypeEnum.DATETIME:
                    parameters.Add(new SugarParameter(string.Concat("TO_DATE(@", string.Concat(item.Id, parameters.Count), ",'YYYY-MM-DD HH24:MI:SS')"), value));
                    break;
                case DataTypeEnum.VARCHAR:
                case DataTypeEnum.CLOB:
                case DataTypeEnum.CHAR:
                case DataTypeEnum.NVARCHAR:
                case DataTypeEnum.TEXT:
                    parameters.Add(new SugarParameter(string.Concat("@", string.Concat(item.Id, parameters.Count)), value));
                    break;
                case DataTypeEnum.NUMBER:
                case DataTypeEnum.INT:
                    parameters.Add(new SugarParameter(string.Concat("@", string.Concat(item.Id, parameters.Count)), Convert.ToInt32(value)));
                    break;
                case DataTypeEnum.FLOAT:
                    parameters.Add(new SugarParameter(string.Concat("@", string.Concat(item.Id, parameters.Count)), Convert.ToSingle(value)));
                    break;
                case DataTypeEnum.DOUBLIE:
                    parameters.Add(new SugarParameter(string.Concat("@", string.Concat(item.Id, parameters.Count)), Convert.ToDouble(value)));
                    break;
                default:
                    parameters.Add(new SugarParameter(string.Concat("@", string.Concat(item.Id, parameters.Count)), value));
                    break;
            }
        }

        /// <summary>
        /// 按数据库类型可以重新按字段类型重写
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        protected virtual void GetConditionInValue(Parameter item, List<SugarParameter> parameters, string value)
        {
            parameters.Add(new SugarParameter(string.Concat(":", string.Concat(item.Id, parameters.Count)), value.Split(",")));
        }


        protected virtual string GetInsertUpdateColumn(Parameter item)
        {
            StringBuilder sb = new StringBuilder();
            switch ((DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), item.DateType.ToUpper()))
            {
                case DataTypeEnum.DATE:
                case DataTypeEnum.DATETIME:
                    if (item.Value.Length == 10)
                        sb.Append(string.Concat("TO_DATE(@", item.Id, ",'YYYY-MM-DD')"));
                    else
                        sb.Append(string.Concat("TO_DATE(@", item.Id, ",'YYYY-MM-DD HH24:MI:SS')"));
                    break;
                case DataTypeEnum.VARCHAR:
                case DataTypeEnum.CLOB:
                case DataTypeEnum.CHAR:
                case DataTypeEnum.NVARCHAR:
                case DataTypeEnum.TEXT:
                    sb.Append(string.Concat("@", item.Id));
                    break;
                case DataTypeEnum.NUMBER:
                case DataTypeEnum.INT:
                    sb.Append(string.Concat("@", item.Id));

                    break;
                case DataTypeEnum.FLOAT:
                    sb.Append(string.Concat("@", item.Id));

                    break;
                case DataTypeEnum.DOUBLIE:
                    sb.Append(string.Concat("@", item.Id));
                    break;
                default:
                    sb.Append(string.Concat("@", item.Id));
                    break;
            }
            return sb.ToString();
        }

        protected virtual void GetInsertUpdateValue(Parameter item, List<SugarParameter> parameters)
        {
            switch ((DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), item.DateType.ToUpper()))
            {
                case DataTypeEnum.DATE:
                case DataTypeEnum.DATETIME:
                case DataTypeEnum.VARCHAR:
                case DataTypeEnum.CLOB:
                case DataTypeEnum.CHAR:
                case DataTypeEnum.NVARCHAR:
                case DataTypeEnum.TEXT:
                    parameters.Add(new SugarParameter(string.Concat("@", item.Id), item.Value));
                    break;
                case DataTypeEnum.NUMBER:
                case DataTypeEnum.INT:
                    parameters.Add(new SugarParameter(string.Concat("@", item.Id), Convert.ToInt32(item.Value)));
                    break;
                case DataTypeEnum.FLOAT:
                    parameters.Add(new SugarParameter(string.Concat("@", item.Id), Convert.ToSingle(item.Value)));
                    break;
                case DataTypeEnum.DOUBLIE:
                    parameters.Add(new SugarParameter(string.Concat("@", item.Id), Convert.ToDouble(item.Value)));
                    break;
                default:
                    parameters.Add(new SugarParameter(string.Concat("@", item.Id), item.Value));
                    break;
            }
        }

        public virtual Common.Cache.ICacheService GetCacheInstance()
        {
            string cacheType = ConfigurationUtil.GetSection("CacheType");
            switch (cacheType)
            {
                case "Redis":
                    return (RedisCacheHelper)_serviceProvider.GetService(typeof(RedisCacheHelper));
                default:
                    return (MemoryCacheManager)_serviceProvider.GetService(typeof(MemoryCacheManager));
            }
        }

        public virtual TableViewService GetInstance()
        {

            string dbType = ConfigurationUtil.GetSection("DbType");
            switch (dbType)
            {
                case "MySql":
                    return _serviceProvider.GetServices<TableViewService>().FirstOrDefault(m => m.GetType() == typeof(MySqlTableViewService));
                case "SqlServer":
                    return _serviceProvider.GetServices<TableViewService>().FirstOrDefault(m => m.GetType() == typeof(SqlServerTableViewService));
                case "Sqlite":
                    return _serviceProvider.GetServices<TableViewService>().FirstOrDefault(m => m.GetType() == typeof(SqlLiteTableViewService));
                case "Oracle":
                    return _serviceProvider.GetServices<TableViewService>().FirstOrDefault(m => m.GetType() == typeof(OracleTableViewService));
                case "PostgreSQL":
                    return _serviceProvider.GetServices<TableViewService>().FirstOrDefault(m => m.GetType() == typeof(PostgreTableViewService));
                default:
                    throw new NotImplementedException("no database select for config settting ,so far wo only support MySql SqlServer SqlLite,Oracle PostgreSQL");
            }
        }
    }
}
