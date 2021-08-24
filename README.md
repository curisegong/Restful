# Restful

有兴趣可以与作者一起继续完善的加QQ群：590691546

也可访问我的个人博客

https://share.22studio.cn/

设计这个的初衷是因为当时为了前段Vue项目报表查询，因为设计到非常多的字段，所以当时设计了一个通用的Vue查询组件，该组件会生成对应条件的JSON字符串，因为项目是用的Springboot，后台采用Mybatis去操作条件，如果条件继续增加或是修改将导致后台每次要同步去更新发版，所以考虑设计一个通用的前段表单设计，后台根据前端的JSON条件自己拼接生产SQL，目前Java后台采用的是拼SQL的形式，存在一定的注入风险，后续将会把Java的重新改造。
所以.Net版本采用了变量形式防止注入

有兴趣的可以查看Vue的通用查询表单 
地址 
https://www.npmjs.com/package/quick-query-form

基于NetCore+SqlSugar+Redis/MemeoryCache通用的数据库单表视图增删改查，目前支持Oracle、SQLServer、MySQL、Sqlite、PostgreSQL,可以自由构建其他数据库操作，前台只需要安装响应的参数组装JSON格式
详细请查看代码里面的注解


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

查询范例如下
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
 {"Columns":"C1,C2,C3,C4,C5","TableViewName":"A",Parameters:[{"Id":"C10",Value:"12"}]}
 对应SQL语句
 INSERT INTO A ( C10) VALUES(@C10)

{"Columns":"C1,C2,C3,C4,C5","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Op":"("},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"OP":"Or"},{"Op":"("},{"Id":"C9","Value":"2","OP":"GTE"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Op":")"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"},{"Id":"C9","Value":"2","OP":"EQ"},{"Op":")"}],Parameters:[{"Id":"C10",Value:"12"}]}
对应SQL语句
 UPDATE A SET C10=@C10 WHERE 1=1  AND C1 IN (@C11)  AND (   (   (  C9 >= @C92 Or  (  C9 >= @C93  AND C9 < @C94   )   )  AND C9 < @C95   )  AND C9 = @C96   ) 

删除操作
{"Columns":"C1,C2,C3","TableViewName":"A","Where":[{"Id":"C1","Value":"1558SD202108080001,J30057SD202108080006,J30057SD202108080005,J30057SD202108080002","OP":"IN"},{"Op":"("},{"Id":"C9","Value":"2","OP":"GT"},{"OP":"Or"},{"Id":"C9","Value":"3","OP":"LT"},{"Op":")"}]}
对应SQL语句
 DELETE FROM A WHERE 1=1  AND C1 IN (@C10)  AND (  C9 > @C91 Or C9 < @C92   ) 

 
也可访问我的个人博客

https://share.22studio.cn/

有兴趣可以与作者一起继续完善的加QQ群：590691546
![image](https://github.com/curisegong/Restful/blob/master/123123123123131.png)
