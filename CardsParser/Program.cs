using System;
using System.Net.Http;

namespace CardsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = client.GetAsync("https://files.codingame.com/legends-of-code-and-magic/cardlist.txt");
                request.Wait();
                var result = request.Result.Content.ReadAsStringAsync();
                result.Wait();
                var resultString = result.Result;
            }
        }
    }
}
