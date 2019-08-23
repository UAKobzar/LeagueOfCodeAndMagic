using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OneFileCompiller
{
    public class Compiller
    {
        public static string Compile(string inputDir)
        {
            List<string> usings = new List<string>();
            var lines = new List<string>();
            bool skipNextLine = false;
            foreach (var file in Directory.GetFiles(inputDir).Where(f=>f.EndsWith(".cs")))
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    if(skipNextLine)
                    {
                        skipNextLine = false;
                        continue;
                    }
                    if(line.Contains("using "))
                    {
                        usings.Add(line);
                    }
                    else if(!line.Contains("namespace"))
                    {
                        lines.Add(line);
                    }
                    else
                    {
                        skipNextLine = true;
                    }
                }

                lines.RemoveAt(lines.FindLastIndex(l => l.Contains("}")));
                
            }

            var result = new List<string>();
            result.AddRange(usings.Distinct());
            result.AddRange(lines);

            return string.Join("\n", result);
        }
    }
}
