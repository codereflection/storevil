using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorEvil.Core;
using StorEvil.Parsing;

namespace StorEvil.InPlace
{
    public class HandlerCodeGenerator
    {
        private readonly IScenarioPreprocessor _scenarioPreprocessor;

        public HandlerCodeGenerator(IScenarioPreprocessor scenarioPreprocessor)
        {
            _scenarioPreprocessor = scenarioPreprocessor;
        }

        public string GetSourceCode(Story story, IEnumerable<Scenario> scenarios, IEnumerable<string> referencedAssemblies)
        {
            var codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("// " + story.Id);
            AppendStoryCode(codeBuilder, story, scenarios.ToArray());

            return string.Format(_sourceCodeTemplate, "", BuildContextFactorySetup(referencedAssemblies), codeBuilder.ToString());
        }

        private string BuildContextFactorySetup(IEnumerable<string> referencedAssemblies)
        {
            var adds =
                referencedAssemblies.Select(a => string.Format("AddAssembly(@\"{0}\");", a));
            return string.Join("\r\n            ", adds.ToArray());
        }

        private void AppendStoryCode(StringBuilder codeBuilder, Story story, Scenario[] scenarios)
        {
            var i = 0;
            foreach (var scenario in scenarios)
            {
                codeBuilder.AppendLine(@"
scenario = scenarios[" + i + @"];

using (StartScenario(story, scenario)) {

#line 1  """ + story.Id + @"""
#line hidden");
                foreach (var line in GetLines(scenario))
                {
                    codeBuilder.AppendFormat(@"
if (ShouldContinue) {{
#line {0} 
ExecuteLine(@""{1}"");
#line hidden
}}  
", line.LineNumber, line.Text.Replace("\"", "\"\""));
                }
                codeBuilder.AppendLine("}");
                codeBuilder.AppendLine(@"CollectScenarioResult();");

                i++;
            }
        }

        private IEnumerable<ScenarioLine> GetLines(IScenario scenario)
        {
            if (scenario is Scenario)
                return ((Scenario)scenario).Body;
            else
                return ((ScenarioOutline)scenario).Scenario.Body;
        }

        private string _sourceCodeTemplate =
            @"
namespace StorEvilTestAssembly {{
    using StorEvil.Context;
    using StorEvil.Core;
    using StorEvil.Interpreter;
    using StorEvil.InPlace;
    using System.Linq;
    using StorEvil.Parsing;
    using StorEvil.ResultListeners;
    {0}

    public class StorEvilDriver : StorEvil.InPlace.DriverBase  {{        
        public StorEvilDriver(IResultListener listener) : base(listener) {{
           
        }}

        public override void HandleStory(Story story) {{
                                    
            {1}
            var scenarios = GetScenarios(story);
            Scenario scenario;

            {2}

            return;
        }}
    }}
}}";

    }
}