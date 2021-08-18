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


namespace Restful.Service
{

    [IocAttribute(ServiceLifetime.Scoped, "SqlServer", typeof(TableViewService))]
    public class SqlServerTableViewService : TableViewService
    {
        public SqlServerTableViewService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        public override void CacheTableViewInfo(string ownerOrSchema)
        {
            CacheTableViewInfo(@"SELECT UPPER(TABLE_NAME) TABLEVIEWNAME,UPPER(COLUMN_NAME) COLUMNNAME,UPPER(DATA_TYPE) DATATYPE
FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG=@OWNER ", ownerOrSchema);
        }

        protected override void GetConditionValue(Parameter item, List<SugarParameter> parameters, string value)
        {
            switch ((DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), item.DateType.ToUpper()))
            {
                case DataTypeEnum.DATE:
                case DataTypeEnum.TIMESTAMP:
                case DataTypeEnum.DATETIME:
                    if (value.Length == 10)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("CONVERT(DATETIME,'", value, "',101")));
                    if (value.Length == 6)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("CONVERT(DATETIME,'", value, "',102")));
                    else
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("CONVERT(DATETIME,'", value, "',101"))); break;
                default:
                    base.GetConditionValue(item, parameters, value);
                    break;
            }
        }

        protected override void GetInsertUpdateValue(Parameter item, List<SugarParameter> parameters)
        {
            switch ((DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), item.DateType.ToUpper()))
            {
                case DataTypeEnum.DATE:
                case DataTypeEnum.TIMESTAMP:
                case DataTypeEnum.DATETIME:
                    if (item.Value.Length == 10)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("CONVERT(DATETIME,'", item.Value, "',101")));
                    if (item.Value.Length == 6)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("CONVERT(DATETIME,'", item.Value, "',102")));
                    else
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("CONVERT(DATETIME,'", item.Value, "',101"))); break;
                default:
                    base.GetInsertUpdateValue(item, parameters);
                    break;
            }
        }
    }

}
