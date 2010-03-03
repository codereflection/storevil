using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Nunit;

namespace StorEvil.Context
{
    /// <summary>
    /// Matches the reflected name of a Property or Field on a context class
    /// to words in a scenario line.
    /// </summary>
    public class PropertyOrFieldNameMatcher : IMemberMatcher
    {
        private readonly List<WordFilter> _wordFilters = new List<WordFilter>();
        public MemberInfo MemberInfo { get; set; }
        public IEnumerable<NameMatch> GetMatches(string line)
        {
            return new[] { GetMatch(line) };  
        }

        private readonly ContextMemberNameSplitter _nameSplitter = new ContextMemberNameSplitter();
        private readonly ScenarioLineParser _scenarioLineParser = new ScenarioLineParser();

        public PropertyOrFieldNameMatcher(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            BuildNonMethodWordFilters();
        }

        private void BuildNonMethodWordFilters()
        {
            _wordFilters.AddRange(
                _nameSplitter
                    .SplitMemberName(MemberInfo.Name)
                    .Select(word => new TextMatchWordFilter(word))
                    .Cast<WordFilter>()
                );
        }

        /// <summary>
        /// Determine whether this member is a full or partial match 
        /// (or no match) for the set of words that is passed in 
        /// </summary>
        /// <param name="line">a set of words that is part of a scenario</param>
        /// <returns></returns>
        public NameMatch GetMatch(string line)
        {
            var words = _scenarioLineParser.ExtractWordsFromScenarioLine(line);

            // can't match if not enough words to fill us up
            if (words.Count < _wordFilters.Count)
                return null;

            var isMatch = WordFiltersMatch(words);

            return !isMatch ? null : BuildNameMatch(words);
        }

        private bool WordFiltersMatch(IList<string> words)
        {
            for (int i = 0; i < _wordFilters.Count; i++)
                if (!_wordFilters[i].IsMatch(words[i]))
                    return false;

            return true;
        }

        readonly Dictionary<string, object> _noParams = new Dictionary<string, object>();

        private NameMatch BuildNameMatch(IEnumerable<string> words)
        {
            var matchedText = JoinWords(words.Take(_wordFilters.Count()));

            // check for exact match
            if (words.Count() == _wordFilters.Count)
                return new ExactMatch(_noParams, matchedText);

            var remainingText = JoinWords(words.Skip(_wordFilters.Count()));

            return new PartialMatch(MemberInfo, _noParams, matchedText, remainingText);
        }

        private static string JoinWords(IEnumerable<string> words)
        {
            return string.Join(" ", words.ToArray());
        }
    }
}