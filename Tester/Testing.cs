using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tester
{
    public class Testing
    {
        public string Url { get; set; }
        public string ResourceUrl { get; set; }  
        public StringContent httpContent = new StringContent(@"{ ""username"": ""operator"", ""password"": ""pomini"" }", Encoding.UTF8, "application/json");

        public async Task Test(string apisFile)
        {

            string toWriteInTheFile = "";
            string[] apisToTest = GestioneFile.getRows(apisFile);
            int i = 1;
            foreach (string api in apisToTest)
                            {
                                using (var httpClient = new HttpClient())
                                {

                                    Task<string> res = getStringThatContainsTheToken(httpClient);
                                    string token = getToken(res.Result);
                                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                    string result = getResource(httpClient, api.Split(" ")[0]);
                                    string[] actually = getFinalResource(result);
                                    string[] spec = api.Split(" ");
                                    toWriteInTheFile += "Test "+i+ "                                        SPEC                                        ACTUALLY\n" + creaateStringToWriteInTheFile(spec, actually);
                                    if (!areEquals(spec, actually))
                                        toWriteInTheFile += "     ESITO: K.O." + "\n******************************************************************************************************************\n";
                                    else
                                        toWriteInTheFile += "     ESITO: SUCCESSFULL" + "\n******************************************************************************************************************\n";
                }
                                i++;
                            }
            await GestioneFile.setRows(toWriteInTheFile);
            
                //return finalResponse.Result;
                //Debug.Assert(finalResponse.Result != null);
                //Console.WriteLine(finalResponse);
            
        }

        private async Task<string> getStringThatContainsTheToken(HttpClient httpClient)
        {
            var response = await httpClient.PostAsync(Url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }

        private string getToken(string res)
        {
            JObject json = JObject.Parse(res);
            return json.SelectToken("token").ToString();
        }

        private string getResource(HttpClient httpClient, string api)
        {
            return httpClient.GetStringAsync(api).GetAwaiter().GetResult();
        }

        private bool areEquals(string[] spec, string[] actually)
        {
            string[] actuallyTrasform = new string[actually.Length];
            actuallyTrasform[0] = actually[0].Replace(", ", ":").Replace("[", "").Replace("]", "");
            actuallyTrasform[1] = actually[1].Replace(", ", ":").Replace("[", "").Replace("]", "");
            bool areEquals = true;
            for (int i=0; i<actually.Length; i++)
            {
                if (actually[i].Replace(", ", ":").Replace("[", "").Replace("]", "") != spec[i + 1])
                {
                    areEquals = false;
                }
            }
            return areEquals;
        }

        private string[] getFinalResource(string resource)
        {
            string[] jsonSplit = new string[resource.Split(',').Length];
            JObject json = JObject.Parse(resource);
            int i = 0;
            foreach (var item in json)
            {
                jsonSplit[i] = item.ToString();
                i++;
            }
            return jsonSplit;//json.SelectToken("languageId").ToString();
        }

        private string creaateStringToWriteInTheFile(string[] spec, string[] actually)
        {
            string result = spec[0]+"    { ";
            for (int i=1; i<spec.Length; i++)
            {
                result += spec[i]+"  ";
            }
            result += " }  { ";
            for (int i = 0; i < actually.Length; i++)
            {
                result += actually[i]+"   ";
            }
            result += " }";
            return result;
        }
    }
}
