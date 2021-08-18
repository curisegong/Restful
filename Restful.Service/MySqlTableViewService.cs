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
using System.Runtime.InteropServices;

namespace Restful.Service
{

    [IocAttribute(ServiceLifetime.Scoped, "MySql", typeof(TableViewService))]
    public class MySqlTableViewService : TableViewService
    {

        public MySqlTableViewService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        public override void CacheTableViewInfo(string ownerOrSchema)
        {
            CacheTableViewInfo(@"SELECT TABLE_NAME TABLEVIEWNAME,COLUMN_NAME COLUMNNAME,DATA_TYPE DATATYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@OWNER ORDER BY TABLE_NAME", ownerOrSchema);
        }

        protected override void GetConditionValue(Parameter item, List<SugarParameter> parameters, string value)
        {

            //GCHandle hander = GCHandle.Alloc(this);

            //var pin = GCHandle.ToIntPtr(hander);
            //Console.WriteLine(pin);

            switch ((DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), item.DateType.ToUpper()))
            {
                case DataTypeEnum.DATE:
                case DataTypeEnum.TIMESTAMP:
                case DataTypeEnum.DATETIME:
                    if (value.Length == 10)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("DATA_FORMAT('",value, "','%Y-%m-%d')")));
                    if (value.Length == 6)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("DATA_FORMAT('", value, "','%Y-%m')")));
                     else
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("DATA_FORMAT('", value, "','%Y-%m-%d %H:%i:%S')")));
                    break;
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
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("DATA_FORMAT('", item.Value, "','%Y-%m-%d')")));
                    if (item.Value.Length == 6)
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("DATA_FORMAT('", item.Value, "','%Y-%m')")));
                    else
                        parameters.Add(new SugarParameter(string.Concat(item.Id, parameters.Count), string.Concat("DATA_FORMAT('", item.Value, "','%Y-%m-%d %H:%i:%S')")));
                    break;
                default:
                    base.GetInsertUpdateValue(item, parameters);
                    break;
            }
        }

    }
}
