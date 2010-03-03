using System.Linq;
using System.Text;
using StorEvil.Core;

namespace StorEvil.Nunit
{
    /// <summary>
    /// Generates NUnit fixtures
    /// </summary>
    public class NUnitFixtureGenerator :  IFixtureGenerator
    {
        private readonly IScenarioPreprocessor _preprocessor;

        public NUnitFixtureGenerator(IScenarioPreprocessor preprocessor, NUnitTestMethodGenerator methodGenerator)
        {
            _preprocessor = preprocessor;
            MethodGenerator = methodGenerator;
        }

        public NUnitTestMethodGenerator MethodGenerator { get; set; }

        const string FixtureFormat =
            @"

namespace {0} {{
    {1}
    

    [TestFixture]
    public class {2} {{
        [TestFixtureSetUp]
        public void WriteStoryToConsole() {{
            {4}
        }}
        
{3}
    }}
}}";
        
        private static string GetFixtureName(Story story)
        {
            var storyName = new StringBuilder();
            foreach (string word in story.Summary.Split(' '))
            {
                storyName.Append(word[0].ToString().ToUpper());
                if (word.Length > 1)
                    storyName.Append(word.Substring(1).ToLower());

                storyName.Append("_");
            }

            return storyName.ToString();
        }

        public string GenerateFixture(Story story, StoryContext context)
        {   
            var tests = new StringBuilder();

            var contextSet = new TestContextSet();
            foreach (var scenario in story.Scenarios)
            {
                foreach (var s in _preprocessor.Preprocess(scenario))
                {
                    NUnitTest test = MethodGenerator.GetTestFromScenario(s, context);
                    tests.Append("        " + test.Body);
                    tests.AppendLine();
                    contextSet.AddRange(test.ContextTypes);
                }
            }
            var namespaces = contextSet.GroupBy(x => x.Type.Namespace).Select(y => y.Key);

            var usingStatements = new[] {""}; // namespaces.Select(x => string.Format("using {0};", x));
            var usings = string.Join("\r\n", usingStatements.ToArray());

            var writeStoryToConsole = "Console.WriteLine(@\"" + story.Summary.Replace("\"", "\"\"") + "\r\n"  + " \");";
            var ns = "TestNamespace";
            return string.Format(FixtureFormat, ns, usings, GetFixtureName(story),  tests, writeStoryToConsole );

        }
    }
}