using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DataInitializer
{
    public interface ILoremIpsum
    {
        string Text { get; }
        IList<string> Words { get; }

        string GetName(int count);
    }

    public class LoremIpsum : ILoremIpsum
    {

        private readonly Random _random;

        public string Text
        {
            get
            {
                return "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
            }
        }

        private List<string> _words;
        public IList<string> Words
        {
            get
            {
                if (_words == null || !_words.Any())
                {
                    _words = Regex.Replace(Text, "^[a-z0-9A-Z]", string.Empty).Split(' ').ToList();
                }
                return _words;
            }
        }

        public LoremIpsum(Random random)
        {
            _random = random;
        }

        public string GetName(int count)
        {
            var names = new List<string>();
            for (int i = 0; i < count; ++i)
            {
                names.Add(Words[_random.Next(Words.Count)]);
            }
            return string.Join(" ", names.ToArray());
        }


    }
}
