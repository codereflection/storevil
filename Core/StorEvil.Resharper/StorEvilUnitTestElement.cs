using System;
using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Text;

namespace StorEvil.Resharper
{
    public abstract class StorEvilUnitTestElement : UnitTestElement
    {
        protected readonly IProject Project;
        private readonly string _title;

        protected StorEvilUnitTestElement(IUnitTestProvider provider, UnitTestElement parent, IProject project,
                                       string title) : base(provider, parent)
        {
            Project = project;
            _title = title;
        }

        public override IProject GetProject()
        {
            return Project;
        }

        public override string GetTitle()
        {
            return _title;
        }

        public override string GetTypeClrName()
        {
            return GetProject().Name + "." + _title;
        }

        public override IList<IProjectFile> GetProjectFiles()
        {
            return new List<IProjectFile>();
        }

        public override string GetKind()
        {
            return "StorEvil";
        }

        public override string ShortName
        {
            get { return _title; }
        }

        public override bool Matches(string filter, IdentifierMatcher matcher)
        {
            return true;
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();
            if (element != null && element.IsValid())
            {
                var locations = new List<UnitTestElementLocation>();
                foreach (IDeclaration declaration in element.GetDeclarations())
                {
                    IFile file = declaration.GetContainingFile();
                    if (file != null)
                        locations.Add(new UnitTestElementLocation(file.ProjectFile,
                                                                  declaration.GetDocumentRange().TextRange,
                                                                  declaration.GetDocumentRange().TextRange));
                }
                return new UnitTestElementDisposition(locations, this);
            }
            return UnitTestElementDisposition.InvalidDisposition;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return null;
        }

        protected ITypeElement GetDeclaredType()
        {
            return null;
        }

        public virtual IList<UnitTestTask> GetTaskSequence()
        {
            return new List<UnitTestTask>();
        }
    }
}