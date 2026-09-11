using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GithubReleaseDownloader
{
    public class SemanticOrder
    {
        private List<string> wordOrder = new List<string>();

        public void RegisterOrder(string name)
        {
            name = name.ToLower();
            if (!Regex.Match(name,@"^[a-z0-9]+$").Success)
            {
                throw new ArgumentException($"{name} is not a valid value. Only accepts A-Z, a-z and 0-9.");
            }

            wordOrder.Add(name);
        }

        public string[] GetSemanticOrder()
        {
            return wordOrder.ToArray();
        }
    }
}
