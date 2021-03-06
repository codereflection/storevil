using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using StorEvil.Context;
using StorEvil.Core;
using StorEvil.Nunit;
using StorEvil.Parsing;
using StorEvil.Utility;

namespace StorEvil.NUnit
{

    [TestFixture]
    public class NUnitFixtureGeneratorTests : TestBase
    {
        [Test]
        public void Should_Create_One_Test_For_A_Single_Scenario()
        {
            var s = BuildScenario("test", new[] {"When I Do Something"});
            var story = new Story("test", "summary", new[] {s});

            Assembly a = BuildTestAssembly(story);
            var fixture = a.GetTypes()[0];
            fixture.GetTestMethods().Count().ShouldEqual(1);
        }

        [Test]
        public void Should_Compile_When_No_Scenarios()
        {
            var s = new Story("test", "summary", new Scenario[] {});
            Assembly a = BuildTestAssembly(s);
        }

        [Test]
        public void Should_Contain_A_Single_testFixture_for_a_Story()
        {
            var s = new Story("test", "summary", new Scenario[] {});
            Assembly a = BuildTestAssembly(s);
            a.GetTypes().Length.ShouldEqual(1);
        }

        [Test]
        public void Should_have_category_for_each_tag_on_story()
        {
            var scenario = BuildScenario("test", new[] { "When I Do Something" });
            var s = new Story("test", "summary", new Scenario[] { scenario }) { Tags = new[] { "foo", "bar" } };
            Assembly a = BuildTestAssembly(s);
            var testClass = a.GetTypes().First();
            var attributes = testClass
                .GetCustomAttributes(typeof (CategoryAttribute), true)
                .Cast<CategoryAttribute>()
                .Select(x=>x.Name);

            attributes.ElementsShouldEqual("foo", "bar");
            
        }

        [Test]
        public void Should_name_class_after_id()
        {
            var s = BuildScenario("test", new[] { "When I Do Something" });
            var story = new Story("C:\\Example\\Path\\To\\File.feature", "summary", new[] { s });

            Assembly a = BuildTestAssembly(story);
            var fixture = a.GetTypes()[0];
            fixture.Name.ShouldEqual("Example_Path_To_File_Specs");
        }

        private Assembly BuildTestAssembly(Story story)
        {
            return BuildTestAssembly(story, null);
        }
        private Assembly BuildTestAssembly(Story story, string[] extraNamespaces)
        {
            var generator = new NUnitFixtureGenerator(new ScenarioPreprocessor(), FakeMethodGenerator(extraNamespaces));
            var code = generator.GenerateFixture(story, GetContext());

            return CreateTestAssembly(code);
        }

        private static Assembly CreateTestAssembly(string code)
        {
            const string header = "using System;\r\nusing NUnit.Framework;";
            return TestHelper.CreateAssembly(header + "\r\n" + code);
        }

        private ITestMethodGenerator FakeMethodGenerator(string[] usings)
        {
            var gen = Fake<ITestMethodGenerator>();
            var testName = "Test" + Guid.NewGuid().ToString().Replace("-", "");
            string body = "\r\n[Test] public void " + testName + "() {}";
            gen.Stub(x => x.GetTestFromScenario(null, null))
                .IgnoreArguments()
                .Return(new NUnitTest(testName, body, new TestContextField[0], usings ?? new string[0]));

            
            return gen;
        }

        private StoryContext GetContext()
        {
            return new StoryContext(new[] {typeof (TestContext)});
        }

        protected static Scenario BuildScenario(string name, params string[] lines)
        {
            return new Scenario("test", lines.Select(line => new ScenarioLine { Text = line }).ToArray());
        }
    }
}