using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restful.Model
{
    public class TableViewInfo
    {
        public string ColumnName { get; set; }

        public string TableViewName { get; set; }

        public string DataType { get; set; }
    }


    public enum DataTypeEnum
    {
        NUMBER = 0,
        VARCHAR = 1,
        DATE = 2,
        DATETIME = 3,
        CLOB = 4,
        CHAR = 5,
        FLOAT = 6,
        DOUBLIE = 7,
        NVARCHAR=8,
        TEXT=9,
        TIMESTAMP=10,
        INT = 11
    }
}
