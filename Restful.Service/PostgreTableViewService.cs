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
    [IocAttribute(ServiceLifetime.Scoped, "PostgreSQL", typeof(TableViewService))]
    public class PostgreTableViewService : TableViewService
    {
        public PostgreTableViewService(IServiceProvider serviceProvider) : base(serviceProvider)
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


        }
    }
}
