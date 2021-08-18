using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Restful.Common;
using Restful.Common.Cache;
using Restful.Common.Filters;
using Restful.Common.IoC;
using Restful.Model;
using Restful.Service;

namespace Restful
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            //Configuration = configuration;
            Configuration = new ConfigurationBuilder()
       .SetBasePath(env.ContentRootPath)
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables().Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddProblemDetails();
            services.AddControllers(options =>
            {
                options.Filters.Add(new GlobalExceptionFilter());//全局异常处理
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restful", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddDistributedMemoryCache();
            //Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetServiceOrCreateInstance
            //services.AddMemoryCache();//启用内存缓存
            //services.AddScoped<MemoryCacheManager>(); //内存缓存认证注入 

            //初始化Redis 
            services.AddScoped<RedisCacheHelper>(); //Redis内存缓存认证注入 
            Redis.Initialization(); //MemoryCache和Redis缓存二选一
            //自定义注入,利用特性将程序集里面的实体类全部注入到IOC容器，不使用接口实现
            //目前将SQLServer、MySQL、Oracle、SQLLite、Postgre五中数据库，如果需要支持其他数据库，可以再Service里面自定义
            //实体类继承TableViewService，重写里面的部分方法
            services.AutoRegisterServicesFromAssembly("Restful.Service");
            //              services.AutoRegisterServicesFromAssembly("Restful.Service", x => x.FullName == "Restful.Service.TableViewService");
            //services.AddScoped<TableViewService>();
            //CacheWhenStartup(services);


        }

        public void CacheWhenStartup(TableViewService tableViewService)
        {
            tableViewService.GetInstance().CacheTableViewInfo("clothplan");
            // tableViewService.GetInstance().CacheTableViewInfo("clothplan");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            CacheWhenStartup(serviceProvider.GetRequiredService<TableViewService>());
            if (env.IsDevelopment())
            {
                app.UseProblemDetails();

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restful v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
