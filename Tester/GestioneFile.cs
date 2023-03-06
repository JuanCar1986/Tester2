using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public static class GestioneFile
    {
        public static string[] getRows(string apisFile)
        {
            string[] lines = System.IO.File.ReadAllLines(apisFile);
            return lines;
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
