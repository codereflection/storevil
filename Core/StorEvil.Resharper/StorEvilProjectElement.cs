using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace StorEvil.Resharper
{
    public class StorEvilProjectElement : StorEvilUnitTestElement
    {
        private readonly UnitTestNamespace _namespace = new UnitTestNamespace("namespace.foo");

        public StorEvilProjectElement(StorEvilTestProvider provider, UnitTestElement parent, IProject project,
                                      string title)
            : base(provider, parent, project, title)
        {
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
    }
}