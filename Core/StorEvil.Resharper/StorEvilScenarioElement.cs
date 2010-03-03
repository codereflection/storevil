using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    public class StorEvilScenarioElement : StorEvilUnitTestElement
    {
        private readonly UnitTestNamespace _namespace;
        private readonly IScenario _scenario;

        public StorEvilScenarioElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                       string title, IScenario scenario)
            : base(provider, parent, project, title)
        {
            _namespace = new UnitTestNamespace(project.Name);
            _scenario = scenario;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return _namespace;
        }

        public override bool Equals(object obj)
        {
            if (obj is StorEvilScenarioElement)
            {
                var testElement = (StorEvilUnitTestElement) obj;
                return testElement.GetNamespace().NamespaceName == _namespace.NamespaceName &&
                       testElement.GetTitle() == GetTitle();
            }

            return false;
        }

        public override IList<UnitTestTask> GetTaskSequence()
        {
            return new List<UnitTestTask>() { new UnitTestTask(this, new RunScenarioTask(_scenario))};
        }
    }
}