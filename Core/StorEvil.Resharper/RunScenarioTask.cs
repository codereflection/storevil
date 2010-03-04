using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;
using StorEvil.Core;

namespace StorEvil.Resharper
{
    [Serializable]
    public class RunScenarioTask : RemoteTask
    {
        
        private const string MagicDelimiter = "$*$*$";

        private IEnumerable<string> Body;
        private string Name;
        private bool IsOutline;
        private IEnumerable<string> FieldNames;
        private IEnumerable<IEnumerable<string>> Examples;

        public RunScenarioTask()
            : base("StorEvil")
        {
        }

        public RunScenarioTask(XmlElement element) : base(element)
        {
            var type = GetXmlAttribute(element, "ScenarioType");
            if (type == "Scenario")
                LoadScenarioXml(element);
            else
                LoadScenarioOutlineXml(element);
        }

        private void LoadScenarioOutlineXml(XmlElement element)
        {
            Body = GetXmlBody(element);
            Name = GetXmlAttribute(element, "Name");
            IsOutline = false;

          
        }

        private void LoadScenarioXml(XmlElement element)
        {
            Body = GetXmlBody(element);
            Name = GetXmlAttribute(element, "Name");
             FieldNames = SplitValues(GetXmlAttribute(element, "FieldNames"));
            var exampleLines = GetXmlAttribute(element, "Examples").Split(new[]{"|||"}, StringSplitOptions.None);
            var examples = new List<IEnumerable<String>>();

            foreach (var exampleLine in exampleLines)
                examples.Add(SplitValues(exampleLine));

            Examples = examples;
            IsOutline = true;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            if (IsOutline)
                SaveScenarioOutlineXml(element);
            else
                SaveScenarioXml(element);
        }

        private void SaveScenarioXml(XmlElement element)
        {
            
            SetXmlAttribute(element, "Name", Name);
            SetXmlAttribute(element, "ScenarioType", "Scenario");
            SetXmlBody(element, Body);
        }

        private void SaveScenarioOutlineXml(XmlElement element)
        {
            SetXmlAttribute(element, "ScenarioType", "ScenarioOutline");
            SetXmlAttribute(element, "Name", Name);
            SetXmlBody(element, Body);
            SetXmlAttribute(element, "FieldNames", JoinValues(FieldNames));
            
            var examplesJoined = new List<string>();
            foreach (var example in Examples)
                examplesJoined.Add(JoinValues(example));

            var exampleValue = string.Join("|||", examplesJoined.ToArray());
            SetXmlAttribute(element, "Examples", exampleValue);
        }

        private IEnumerable<string> GetXmlBody(XmlElement element)
        {
            var text = GetXmlAttribute(element, "Text");
            return SplitValues(text);
        }

        private IEnumerable<string> SplitValues(string text)
        {
            return text.Split(new[] {MagicDelimiter}, StringSplitOptions.None);
        }

        private void SetXmlBody(XmlElement element, IEnumerable<string> body)
        {
            SetXmlAttribute(element, "Text", JoinValues(body));
        }

        private string JoinValues(IEnumerable<string> values)
        {
            return string.Join("$*$*$", new List<string>(values).ToArray());
        }

        public RunScenarioTask(IScenario scenario) : base("StorEvil")
        {
            Name = scenario.Name;
            if (scenario is Scenario)
            {
                var s = scenario as Scenario;
                Body = s.Body;
            }
            else
            {
                var so = scenario as ScenarioOutline;
                Body = so.Scenario.Body;
                Examples = so.Examples;
                FieldNames = so.FieldNames;
            }
        }

        public IScenario GetScenario()
        {
            if (IsOutline)
            {
                return new ScenarioOutline(Name, new Scenario(Name, Body), FieldNames, Examples);
            }
            else
            {
                return new Scenario(Name, Body);
            }
        }
    }

   
}