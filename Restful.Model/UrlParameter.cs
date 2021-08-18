using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restful.Model
{
    public class UrlParameter
    {
        /// <summary>
        /// 列名要查询的列
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// 表名和视图名
        /// </summary>
        public string TableViewName { get; set; }

        /// <summary>
        /// 参数已键值对存在
        /// </summary>
        public List<Parameter> Parameters { get; set; }

        /// <summary>
        /// 条件已键值对存在
        /// </summary>
        public List<Parameter> Where { get; set; }

        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// 排序字段默认升序 字段前加-号表示降序
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// 聚合函数如sum max等等
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        public int PageNumber { get; set; }
    }
}
