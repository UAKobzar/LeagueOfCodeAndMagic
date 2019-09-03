using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CardParser
{
    public class Parser
    {
        public static async Task<string> Parse()
        {
            var result = await ParseWithoutTemplate();
            var template = File.ReadAllText("CardsList.cs");
            var cards = string.Join(",\n", result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(c => $"\"{c}\""));

            var file = template.Replace("{0}", cards);
            return file;
        }

        public static async Task<string> ParseWithoutTemplate()
        {
            using (HttpClient client = new HttpClient())
            {
                var request = await client.GetAsync("https://files.codingame.com/legends-of-code-and-magic/cardlist.txt");
                var result = await request.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
