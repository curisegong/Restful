# Restful
基于NetCore+SqlSugar+Redis/MemeoryCache通用的数据库单表视图增删改查，目前支持Oracle、SQLServer、MySQL、Sqlite、PostgreSQL,可以自由构建其他数据库操作，前台只需要安装响应的参数组装JSON格式
详细请查看代码里面的注解


  /// <summary>
        /// 列名要查询的列
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// 表名和视图名
        /// </summary>
        public string TableViewName { get; set; }

        /// <summary>
        /// 参数已键值对存在用于Update和Insert的列名和键值对
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

查询范例入下
{"Columns":"orderid,factoryid","TableViewName":"view_factory_min","Where":[{"Id":"orderid","Value":"1","OP":"EQ"}],"GroupBy":"orderid,factoryid","Args":"min=qty","SortBy":"","PageSize":10,"PageNumber":1}
结果
对应SQL语句
SELECT orderid,factoryid ,MIN(qty) MIN_qty FROM view_factory_min WHERE 1=1  AND orderid = @orderid0  GROUP BY orderid,factoryid

{"Columns":"C1,C2,C3,C4,C5","TableViewName":"A","Where":[{"Id":"C1","Value":"1","OP":"EQ","DateType":null},{"Id":"C2","Value":"1","OP":"EQ","DateType":null}],"GroupBy":null,"SortBy":"C1,C2","Args":"SUM=C1,MAX=C2,MIN=C3,AVG=C4","PageSize":10,"PageNumber":1}
对应SQL语句
SELECT C1,C2,C3,C4,C5 ,SUM(C1) SUM_C1,MAX(C2) MAX_C2,MIN(C3) MIN_C3,AVG(C4) AVG_C4 FROM A WHERE 1=1  AND C1 = @C10  AND C2 = @C21  ORDER BY C1,C2
 
 {"Columns":"C1,C2,C3,C4,C5","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"OP":"Or"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Id":"C9","Value":"2","OP":"EQ"},{"Op":")"}],"GroupBy":null,"SortBy":"C1,C2","PageSize":10,"PageNumber":1}
对应SQL语句
SELECT C1,C2,C3,C4,C5  FROM A WHERE 1=1  AND C1 IN (@C10)  AND (   (  C9 >= @C91 Or C9 < @C92   )  AND C9 = @C93   )  ORDER BY C1,C2

 插入及更新传入参数
 {"Columns":"C1,C2,C3,C4,C5","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Op":"("},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"OP":"Or"},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Op":")"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Id":"C9","Value":"2","OP":"EQ"},{"Op":")"}],Paramters:[{"Id":"C10",Value:"12"}]}


{"Columns":"C1,C2,C3,C4,C5","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Op":"("},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"OP":"Or"},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Op":")"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Id":"C9","Value":"2","OP":"EQ"},{"Op":")"}],Paramters:[{"Id":"C10",Value:"12"}]}

{"Columns":"C1,C2,C3","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"OP":"Or"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"}]}


{"Columns":"C1,C2,C3","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"OP":"Or"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"}],Parameters:[{"Id":"C10",Value:"12"}]}


{"Columns":"C1,C2,C3","TableViewName":"A","Where":[{"Id":"C1","Value":"","OP":"IS"}],Parameters:[{"Id":"C10",Value:"12"},{"Id":"D1",Value:"2021-01-12"}]}

存在问题
Order By还没考虑全
目前只能基于内网使用，存在安全风险，需要继续添加权限控制
