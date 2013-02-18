using System.Reflection;
using ConfigInCode;
using NUnit.Framework;

namespace Test.ConfigInCode
{
    /// <summary>
    /// Some of these tests are testing serverconfig which is not kosher
    /// </summary>
    [TestFixture]
    class Configuration_Load
    {
        [Test]
        public void can_load_config_file()
        {
            var config = new Configuration<ITestConfiguration>();
            var c = config.GetConfiguration("Configuration", "UnusedParam");
            Assert.NotNull(c);
        }
        [Test]
        public void can_pass_in_constructor_value()
        {
            var config = new Configuration<ITestConfiguration>();
            var c = config.GetConfiguration("Configuration", "HelloWorld");
            Assert.AreEqual("HelloWorld", c.ConstructorValue);
        }
        [Test]
        public void can_get_script_from_stream()
        {
            var config = new Configuration<ITestConfiguration>();
            var c = config.GetConfiguration("Embedded",
                                            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                                                "Test.ConfigInCode.Embedded.rb"), "UnusedParam");
            Assert.NotNull(c);
        }
        [Test]
        public void can_respond_to_method_call()
        {
            var config = new Configuration<IRespond>();
            var c = config.GetConfiguration("RubyResponder");
            Assert.That(c.Request(), Is.EqualTo("Hello World"));
        }
    }
}