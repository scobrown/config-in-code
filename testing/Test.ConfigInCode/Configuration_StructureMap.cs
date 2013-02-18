using ConfigInCode;
using ConfigInCode.StructureMapPlugin;
using NUnit.Framework;
using StructureMap;

namespace Test.ConfigInCode
{
    [TestFixture]
    class Configuration_StructureMap
    {
        [Test]
        public void ShouldLoadFromContainer()
        {
            ObjectFactory.Initialize(x =>
                                     x.For<ITestConfiguration>().Use(context =>
                                         {
                                             var config = new Configuration<ITestConfiguration>();
                                             return config.GetConfiguration("Configuration", "UnusedParam");
                                         }));
            var result = ObjectFactory.GetInstance<ITestConfiguration>();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public void ShouldLoadFromDefaultContainerExtension()
        {
            ObjectFactory.Initialize(x =>
                                     x.WithDefaultConfiguration<ITestConfiguration>());
            var result = ObjectFactory.GetInstance<ITestConfiguration>();
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public void ShouldLoadFromContainerExtension()
        {
            ObjectFactory.Initialize(x =>
                                     x.WithConfiguration<ITestConfiguration>("Configuration", "UnusedParam"));
            var result = ObjectFactory.GetInstance<ITestConfiguration>();
            Assert.That(result, Is.Not.Null);
        }
    }

}