using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConfigInCode
{
    public interface IConfiguration<T> where T : class
    {
        string FileName { get; }
        bool ConfigExists { get; }
        string[] GemPaths { get; }
        T GetConfiguration(string implimentation, params object[] constructionParameters);
        T GetConfiguration(string implimentation, Func<T> defaultIfNone, params object[] constructionParameters);
        T GetConfiguration(string implimentation, Stream resourceStream, params object[] constructionParameters);
    }

    public class Configuration<T> : IConfiguration<T> where T : class
    {
        public string FileName { get; private set; }
        public bool ConfigExists { get { return File.Exists(FileName); } }
        public RubyScript RubyScript { get; private set; }
        public string[] GemPaths { get { return RubyScript.SearchPaths; } }

        public Configuration()
        {
            var rubyFile = typeof (T).Name + ".rb";
            var foldersToCheck = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration"),
                    AppDomain.CurrentDomain.BaseDirectory,
                    Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Configuration"),
                    AppDomain.CurrentDomain.RelativeSearchPath,
                };
            FileName = foldersToCheck.Select(x => Path.Combine(x, rubyFile))
                .First(x => File.Exists(x));
            RubyScript = new RubyScript();
            RubyScript.LoadAssembly(Assembly.GetAssembly(typeof(T)));
        }

        public T GetConfiguration(string implimentation, params object[] constructionParameters)
        {
            RubyScript.RunScriptFromFile(FileName);
            return RubyScript.CreateInstance<T>(implimentation, constructionParameters);
        }
        public T GetConfiguration(string implimentation, Func<T> defaultIfNone, params object[] constructionParameters)
        {
            T config = null;
            try
            {
                if (File.Exists(FileName))
                {
                    GetConfiguration(implimentation, constructionParameters);
                }
            }
            catch
            {
                config = defaultIfNone();
            }
            return config;
        }
        public T GetConfiguration(string implimentation, Stream resourceStream, params object[] constructionParameters)
        {
            var textStream = new StreamReader(resourceStream);
            RubyScript.RunScriptFromText(textStream.ReadToEnd());
            return RubyScript.CreateInstance<T>(implimentation, constructionParameters);
        }
    }
}
