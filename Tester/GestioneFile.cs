using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Tester
{
    public static class GestioneFile
    {
        public static string[] getRows(string apisFile)
        {
            string[] lines = System.IO.File.ReadAllLines(apisFile);
            
            return deleteTheComments(lines);
        }

        private static string[] deleteTheComments(string[] lines)
        {
            string pattern = "#";
            List<string> body = new List<string>();
            for(int i = 0; i<lines.Length; i++)
            {
                if(!Regex.IsMatch(lines[i], pattern))
                {
                    body.Add(lines[i]);
                }
            }
            return body.ToArray();
        }

        public static async Task setRows(string result)
        {
            string text = "";
            if (result != null)
            {
                text = result;

                await File.WriteAllTextAsync("resultsOfTests.txt", text);
                
            }
            
        }
    }
}
