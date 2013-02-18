using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace ConfigInCode.StructureMapPlugin
{
    public static class StructureMapExtensions
    {
        public static void WithConfiguration<T>(this IInitializationExpression init, string implimentation, params object[] constructorParams) where T : class
        {
            init.For<T>().Use(context =>
            {
                var config = new Configuration<T>();
                return config.GetConfiguration(implimentation, constructorParams);
            });

        }
        public static void WithDefaultConfiguration<T>(this IInitializationExpression init, params object[] constructorParams) where T : class
        {
            init.For<T>().Use(context =>
            {
                var config = new Configuration<T>();
                return config.GetDefaultConfiguration(constructorParams);
            });

        }
    }
}
