using System.Collections.Generic;
using Newtonsoft.Json;

namespace Postbucket.BLL.Extensions
{
    public static class DictionaryExtensions
    {
        public static string Serialize(this Dictionary<string, string> kvPairs)
        {
            return JsonConvert.SerializeObject(kvPairs, Formatting.Indented);
        }
    }
}