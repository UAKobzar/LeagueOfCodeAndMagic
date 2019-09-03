using CardParser;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessCode().Wait();
            Console.WriteLine("Ready");
            Console.Read();
        }

        static async Task ProcessCode()
        {
            //var file = await CardParser.Parser.Parse();
            //File.WriteAllText(@"D:\OwnProjects\LegendsOfCodeAndMagic\LegendsOfCodeAndMagic\CardsList.cs", file);

            //var values = await Descent.GetValues();

            //foreach (var item in values)
            //{
            //    Console.WriteLine(item.Key + ": " + item.Value);
            //}

            var oneFile = OneFileCompiller.Compiller.Compile(@"D:\OwnProjects\LegendsOfCodeAndMagic\LegendsOfCodeAndMagic");
            File.WriteAllText(@"D:\OwnProjects\LegendsOfCodeAndMagic\OneFile.cs", oneFile);
        }
    }
}
