using StructureMap;

namespace ConfigInCode.StructureMap
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
