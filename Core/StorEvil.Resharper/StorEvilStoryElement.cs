using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;

namespace StorEvil.Resharper
{
    public class StorEvilStoryElement : StorEvilUnitTestElement
    {
        public ConfigSettings Config { get; set; }
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilStoryElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project, string title, ConfigSettings config)
            : base(provider, parent, project, title)
        {
            Config = config;
            _namespace = new UnitTestNamespace(project.Name + ".namespaceYo");
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilUnitTestElement)
            {
                var testElement = (StorEvilUnitTestElement)obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
        }

        public override IList<UnitTestTask> GetTaskSequence()
        {
            var tasks = new List<UnitTestTask>();
            foreach (var assemblyLocation in Config.AssemblyLocations)
                tasks.Add(new UnitTestTask(this, new AssemblyLoadTask(assemblyLocation)));

            return tasks;
        }
    }
}