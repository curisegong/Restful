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
    [IocAttribute(ServiceLifetime.Scoped, "Oracle", typeof(TableViewService))]
    public class OracleTableViewService : TableViewService
    {
        public OracleTableViewService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        public override void CacheTableViewInfo(string ownerOrSchema)
        {
            CacheTableViewInfo(@"SELECT TABLE_NAME TABLEVIEWNAME,COLUMN_NAME COLUMNNAME,CASE WHEN DATA_TYPE='VARCHAR2' THEN 'VARCHAR' ELSE DATA_TYPE END  DATATYPE FROM ALL_TAB_COLS WHERE OWNER=@OWNER ORDER BY TABLE_NAME", ownerOrSchema);
        }
    }
}
