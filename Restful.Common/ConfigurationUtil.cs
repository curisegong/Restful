using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using SqlSugar;

namespace Restful.Common
{
    /// <summary>
    /// 配置文件相关操作
    /// </summary>
    public class ConfigurationUtil
    {
        public static readonly IConfiguration Configuration;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DBConnectionString
        {
            get
            {
                string s = ConfigurationUtil.GetSection("DbType");
                switch (s)
                {
                    case "MySql":
                        return Configuration.GetConnectionString("MySqlConnection");
                    case "SqlServer":
                        return Configuration.GetConnectionString("DefaultConnection");
                    case "Sqlite":
                        return Configuration.GetConnectionString("SqliteConnection");
                    case "Oracle":
                        return Configuration.GetConnectionString("OracleConnection");
                    case "PostgreSQL":
                        return Configuration.GetConnectionString("PgSqlConnection");
                    default:
                        return Configuration.GetConnectionString("DefaultConnection");
                }

            }
        }

        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public static string RedisConnectionString { get => Configuration.GetConnectionString("RedisConnection"); }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DbType DbType
        {
            get
            {
                DbType dbType = DbType.SqlServer;
                string s = ConfigurationUtil.GetSection("DbType");
                switch (s)
                {
                    case "MySql":
                        dbType = DbType.MySql;
                        break;
                    case "SqlServer":
                        dbType = DbType.SqlServer;
                        break;
                    case "Sqlite":
                        dbType = DbType.Sqlite;
                        break;
                    case "Oracle":
                        dbType = DbType.Oracle;
                        break;
                    case "PostgreSQL":
                        dbType = DbType.PostgreSQL;
                        break;
                    default:
                        dbType = DbType.SqlServer;
                        break;
                }

                return dbType;
            }
        }

        static ConfigurationUtil()
        {
            string directory = Directory.GetCurrentDirectory(); //AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf(@"\bin")); //
            Configuration = new ConfigurationBuilder()//.Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true }).Build();
                .SetBasePath(directory)
                .AddJsonFile("appsettings.json", true)
                .Build();
        }


        /// <summary>
        /// 获取指定key对应的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSection(string key)
        {
            return Configuration.GetValue<string>(key);
        }

    }
}
