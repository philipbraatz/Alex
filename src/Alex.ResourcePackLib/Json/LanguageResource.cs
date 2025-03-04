﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Alex.ResourcePackLib.Json
{
    public class LanguageResource : Dictionary<string, string>
    {
        private static readonly Regex LangFileRegex = new Regex(@"^\s*(?'key'[\w\.]+)\s*=\s*(?'value'.+)\s*$",
                                                      RegexOptions.Compiled | RegexOptions.Multiline |
                                                      RegexOptions.IgnoreCase);

        [JsonProperty("language.name")]
        public string CultureName { get; set; }
        
        [JsonProperty("language.region")]
        public string CultureRegion { get; set; }

        [JsonProperty("language.code")]
        public string CultureCode { get; set; }

        public LanguageResource()
        {
            
        }
        
        public LanguageResource(IEnumerable<KeyValuePair<string, string>> values) : base(values, StringComparer.OrdinalIgnoreCase)
        {
            
        }
        
        public static LanguageResource ParseLangFile(string text)
        {
            var lines = LangFileRegex.Matches(text);

            var lang = new LanguageResource();

            foreach (Match match in lines)
            {
                var key = match.Groups["key"].Value;
                var value = match.Groups["value"].Value;
                
                if (key == "language.code")
                {
                    lang.CultureCode = value;
                }
                else if (key == "language.name")
                {
                    lang.CultureName = value;
                }
                else if (key == "language.region")
                {
                    lang.CultureRegion = value;
                }

                lang[key] = value;
            }

            return lang;
        }
    }
}
