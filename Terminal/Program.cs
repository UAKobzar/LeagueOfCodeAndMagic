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

            //var file = await Descent.GetAsFile();
            //File.WriteAllText(@"D:\OwnProjects\LegendsOfCodeAndMagic\LegendsOfCodeAndMagic\Configuration.cs", file);

            var oneFile = OneFileCompiller.Compiller.Compile(@"D:\OwnProjects\LegendsOfCodeAndMagic\LegendsOfCodeAndMagic");
            File.WriteAllText(@"D:\OwnProjects\LegendsOfCodeAndMagic\OneFile.cs", oneFile);
        }
    }
}
