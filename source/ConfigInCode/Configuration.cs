using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConfigInCode
{
    public interface IConfiguration<T> where T : class
    {
        /// <summary>
        /// The file name that was resolved for the type
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Does a file exist for the type
        /// </summary>
        bool ConfigExists { get; }
        /// <summary>
        /// Gem paths that will be searched to during Ruby gem resolution
        /// </summary>
        string[] GemPaths { get; }

        /// <summary>
        /// Gets an instance from a file matching T.ext using the type of T to find the dlr class
        /// </summary>
        /// <param name="constructionParameters">Parameters to be passed into the constructor</param>
        /// <returns>An instance of Type T</returns>
        T GetDefaultConfiguration(params object[] constructionParameters);
        /// <summary>
        /// Gets an instance from a file matching T.ext
        /// </summary>
        /// <param name="implimentation">The dlr class name to load</param>
        /// <param name="constructionParameters">Parameters to be passed into the constructor</param>
        /// <returns>An instance of Type T</returns>
        T GetConfiguration(string implimentation, params object[] constructionParameters);
        /// <summary>
        /// Gets an instance or returns the default if no implementation found
        /// </summary>
        /// <param name="implimentation">The dlr class name to load</param>
        /// <param name="defaultIfNone">Function to get a type if dlr implementation is not found</param>
        /// <param name="constructionParameters">Parameters to be passed into the constructor</param>
        /// <returns>An instance of Type T</returns>
        T GetConfiguration(string implimentation, Func<T> defaultIfNone, params object[] constructionParameters);
        /// <summary>
        /// Gets an instance from the supplied stream
        /// </summary>
        /// <param name="implimentation">The dlr class name to load</param>
        /// <param name="resourceStream">A stream representing the dlr text</param>
        /// <param name="constructionParameters">Parameters to be passed into the constructor</param>
        /// <returns>An instance of Type T</returns>
        T GetConfiguration(string implimentation, Stream resourceStream, params object[] constructionParameters);
    }

    public class Configuration<T> : IConfiguration<T> where T : class
    {
        public string FileName { get; private set; }
        public bool ConfigExists { get { return File.Exists(FileName); } }
        public ScriptLoader Scripting { get; private set; }
        public string[] GemPaths { get { return Scripting.SearchPaths; } }

        public Configuration()
        {
            var rubyFile = typeof(T).Name + ".rb";
            var pythonFile = typeof(T).Name + ".py";
            var foldersToCheck = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration"),
                    AppDomain.CurrentDomain.BaseDirectory,
                    Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Configuration"),
                    AppDomain.CurrentDomain.RelativeSearchPath,
                };
            FileName = foldersToCheck.SelectMany(x => new[]
                {
                    Path.Combine(x, rubyFile),
                    Path.Combine(x, pythonFile)
                }).First(File.Exists);
            if(Path.GetExtension(FileName) == ".rb")
                Scripting = new ScriptLoader(IronRuby.Ruby.CreateRuntime(), "ruby");
            else
                Scripting = new ScriptLoader(IronPython.Hosting.Python.CreateRuntime(), "python");
            Scripting.LoadAssembly(Assembly.GetAssembly(typeof(T)));
        }

        public T GetConfiguration(string implimentation, params object[] constructionParameters)
        {
            Scripting.RunScriptFromFile(FileName);
            return Scripting.CreateInstance<T>(implimentation, constructionParameters);
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
            Scripting.RunScriptFromText(textStream.ReadToEnd());
            return Scripting.CreateInstance<T>(implimentation, constructionParameters);
        }

        public T GetDefaultConfiguration(params object[] constructorParams)
        {
            return GetConfiguration(typeof (T).Name.Trim('I'), constructorParams);
        }
    }
}
