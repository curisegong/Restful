using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Restful.Common.IoC
{
    /// <summary>
    /// 标注要运用DI的类 被此属性标注的类 要被注册到依赖注入容器中 并且可以指定类要映射的接口或者类
    /// 此属性只能运用于类，并且此属性不能继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IocAttribute : Attribute
    {
        //Targets用于指示 哪些接口或者类 要被 "被属性修饰了的类" 进行依赖注入
        public List<Type> TargetTypes = new List<Type>();
        public ServiceLifetime lifetime;
        public string workForDataBaseType;
        public IocAttribute(ServiceLifetime argLifetime, string _workForDataBaseType, params Type[] argTargets)
        {
            lifetime = argLifetime;
            workForDataBaseType = _workForDataBaseType;
            foreach (var argTarget in argTargets)
            {
                TargetTypes.Add(argTarget);
            }
        }

        public List<Type> GetTargetTypes()
        {
            return TargetTypes;
        }
        public ServiceLifetime Lifetime
        {
            get
            {
                return this.lifetime;
            }
        }

        public string WorkForDataBaseType
        {
            get
            {
                return this.workForDataBaseType;
            }
        }
    }
}
