using System;

namespace Restful.Model
{
    public class Parameter
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 操作符，对于OperatorEnum
        /// </summary>
        public string OP { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DateType { get; set; }
    }


    public enum OperatorEnum
    {
        LT = 0, //小于
        GT = 1, //大于
        LTE = 2,//小于等于
        GTE = 3,//大于等于
        LIKE = 4,//模糊
        SW = 5,//已**开头
        EW = 6,//已**结尾
        IN = 7,//in
        EQ = 8,//等于
        NEQ = 9,//不等于
        IS = 10,//is null
        IE = 11,//is not nul
        LPT = 12,//左括号
        RPT = 13//右括号
    }
}
