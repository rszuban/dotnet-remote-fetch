using Newtonsoft.Json.Linq;
using System;

namespace RemoteFetch.Classes
{
    class FetchValueParser
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public string ParseCustom(FetchValueParser fetchValueParser, string textToParse)
        {
            var returnValue = "";

            switch(fetchValueParser.Name)
            {
                case "json":
                    JObject json = JObject.Parse(System.Net.WebUtility.HtmlDecode(textToParse));
                    returnValue = json[fetchValueParser.Value].ToString();
                    break;

                default:
                    Console.WriteLine($"{fetchValueParser.Name} parser is currently not supported.");
                    break;
            }

            return returnValue;
        }
    }
}
