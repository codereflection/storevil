using System.IO;
using System.Text;

namespace StorEvil.Nunit
{
    /// <summary>
    /// Writes all fixtures to a single file
    /// </summary>
    public class SingleFileTestFixtureWriter : ITestFixtureWriter
    {
        private readonly TestFixtureGenerationSettings _settings;

        public SingleFileTestFixtureWriter(TestFixtureGenerationSettings settings)
        {
            _settings = settings;

            _code.AppendLine("using NUnit.Framework;");
            _code.AppendLine("using System;");
            _code.AppendLine("using StorEvil;");
        }

        public string TargetFilePath { get; set; }
        readonly StringBuilder _code = new StringBuilder();

        public void WriteFixture(string storyId, string sourceCode)
        {
            _code.AppendLine();
            _code.Append(sourceCode);
        }

        public void Finished()
        {
            // write to target file
            File.WriteAllText(_settings.TargetFilePath, _code.ToString());
        }
    }

    public class TestFixtureGenerationSettings
    {
        public string TargetFilePath { get; set; }
    }
}