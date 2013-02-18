using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Hosting;

namespace ConfigInCode
{
    public class ScriptLoader
    {
        private static readonly bool Loaded;
        static ScriptLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {

                String resourceName = "ConfigInCode.References." +

                   new AssemblyName(args.Name).Name + ".dll";

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;

                    Byte[] assemblyData = new Byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    return Assembly.Load(assemblyData);
                }

            };
            Loaded = true;
        }

        protected ScriptRuntime RunTime { get; set; }
        internal ScriptScope Scope { get; private set; }
        public string[] SearchPaths {get { return _engine.GetSearchPaths().ToArray(); }}

        private readonly ScriptEngine _engine;

        internal ScriptLoader(ScriptRuntime runtime, string languageName)
        {
            if (!Loaded) ;
 
            RunTime = runtime;
            RunTime.LoadAssembly(typeof(System.Environment).Assembly);
            RunTime.LoadAssembly(typeof(System.Dynamic.ExpandoObject).Assembly);
            _engine = RunTime.GetEngine(languageName);
            var paths = new List<string>(_engine.GetSearchPaths())
                            {
                                AppDomain.CurrentDomain.BaseDirectory,

                            };
            _engine.SetSearchPaths(paths);

            Scope = _engine.CreateScope();
        }
        public void LoadAssembly(Assembly assembly)
        {
            RunTime.LoadAssembly(assembly);
        }


        public void RunScriptFromFile(string fileName)
        {
            Scope = _engine.ExecuteFile(fileName, Scope);
        }
        public void RunScriptFromText(string script)
        {
           _engine.Execute(script, Scope);
        }
        internal dynamic RunScript(string script)
        {
            return _engine.Execute(script);
        }

        public dynamic CreateInstance(string className, ScriptScope scope, params object[] constructionParameters)
        {
            dynamic dynType;
            if (RunTime.Globals.ContainsVariable(className))
                dynType = RunTime.Globals.GetVariable(className);
            else
                dynType = scope.GetVariable(className);
            return _engine.Operations.CreateInstance(dynType, constructionParameters);
        }
        public T CreateInstance<T>(string className, params object[] constructionParameters) where T : class
        {
            return CreateInstance(className, Scope, constructionParameters) as T;
        }
    }
}
