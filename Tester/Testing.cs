using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        private StringContent httpContent;

        public async Task Test(string apisFile, List<Credentials> usersList)
        {
            string toWriteInTheFile = "";
            int i = 1;
            for (int t = 0; t < 2; t++)
            {


                foreach (var user in usersList)
                {
                    //httpContent = new StringContent(@"{ ""username"": """+username+""", ""password"": """+password+""" }", Encoding.UTF8, "application/json");
                    httpContent = new StringContent("{ \"username\": \"" + user.Username + "\", \"password\": \"" + user.Password + "\" }", Encoding.UTF8, "application/json");

                    string[] apisToTest = GestioneFile.getRows(apisFile);

                    foreach (string api in apisToTest)
                    {
                        string[] spec = api.Split(" ");
                        toWriteInTheFile += spec[0] + "          username: " + user.Username + "\n";
                        using (var httpClient = new HttpClient())
                        {
                            Task<string> res = getStringThatContainsTheToken(httpClient);
                            string token = getToken(res.Result);
                            if (t == 1)
                            {
                                toWriteInTheFile += "CON TOKEN\n";
                                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            }
                            else
                            {
                                toWriteInTheFile += "SENZA TOKEN\n";
                            }
                            string actually;

                            if (api.Split(" ")[2] == "Update")
                            {
                                actually = await update(httpClient, api.Split(" ")[1], api.Split(" ")[3]);
                                if (actually == "No Content")
                                {
                                    GestioneDB db = new GestioneDB();
                                    actually = db.DataReader(api.Split(" ")[3]);
                                    toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: UPDATE ---> \n" + creaateStringToWriteInTheFile(spec[3], actually);
                                    if (!areEquals(spec[3], actually))
                                        toWriteInTheFile += "     ESITO: K.O." + "\n******************************************************************************************************************\n";
                                    else
                                        toWriteInTheFile += "     ESITO: SUCCESSFULL" + "\n******************************************************************************************************************\n";
                                }
                                else if (actually == "Forbidden")
                                {
                                    toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: UPDATE ---> \nSPEC: 204 FORBIDDEN RESPONSE: 403 FORBIDDEN ESITO: K.O" + "\n******************************************************************************************************************\n";
                                }
                                else if (actually == "Unauthorized")
                                {
                                    toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: UPDATE ---> \nSPEC: 401 UNAUTHORIZED RESPONSE: 401 UNAUTHORIZED ESITO: SUCCESSFULL" + "\n******************************************************************************************************************\n";
                                }else if(actually == "Bad Request")
                                {
                                    toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: UPDATE ---> \nSPEC: 400 BAD REQUEST RESPONSE: 400 BAD REQUEST ESITO: SUCCESSFULL" + "\n******************************************************************************************************************\n";
                                }

                            }
                            else if (api.Split(" ")[2] == "Get")
                            {
                                actually = await get(httpClient, api.Split(" ")[1]);
                                if (actually == "Unauthorized")
                                {
                                    toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: GET ---> \nSPEC 401 UNAUTHORIZED RESPONSE: 401 UNAUTHORIZED ESITO: SUCCESSFULL" + "\n******************************************************************************************************************\n"; 
                                }else if (actually != string.Empty)
                                {
                                    if (!areEquals(spec[3], actually))
                                    {
                                        toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: GET ---> \n";
                                        toWriteInTheFile += creaateStringToWriteInTheFile(spec[2], actually) + "     ESITO: K.O." + "\n******************************************************************************************************************\n";
                                    }
                                    else
                                    {
                                        toWriteInTheFile += "API: " + api.Split(" ")[1] + "\nVERB: GET ---> \n";
                                        toWriteInTheFile += creaateStringToWriteInTheFile(spec[2], actually) + "     ESITO: SUCCESSFULL" + "\n******************************************************************************************************************\n";
                                    }
                                }
                                
                            }

                        }
                        i++;
                    }
                    await GestioneFile.setRows(toWriteInTheFile);
                }
            }
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

        //private async Task<string> getResource(HttpClient httpClient, string api)
        //{
        //    //var status = httpClient.GetStringAsync(api).GetAwaiter().GetResult();
        //    var res = await httpClient.GetAsync(api);
        //    string response = "";
        //    if(res.StatusCode == HttpStatusCode.OK) //200
        //    {
        //        response = httpClient.GetStringAsync(api).GetAwaiter().GetResult();
        //    }
        //    else if(res.StatusCode == HttpStatusCode.Unauthorized){ //401
        //        response = "Unauthorized";
        //    }else if(res.StatusCode == HttpStatusCode.Forbidden) //403
        //    {
        //        response = "Forbidden";
        //    }
            
        //    return response;
        //}

        private bool areEquals(string spec, string actually)
        {
            //string[] actuallyTrasform = new string[actually.Length];
            //actuallyTrasform[0] = actually[0].Replace(", ", ":").Replace("[", "").Replace("]", "");
            //actuallyTrasform[1] = actually[1].Replace(", ", ":").Replace("[", "").Replace("]", "");
            //bool areEquals = true;
            //for (int i=0; i<actually.Length; i++)
            //{
            //    if (actually[i].Replace(", ", ":").Replace("[", "").Replace("]", "") != spec[i + 1])
            //    {
            //        areEquals = false;
            //    }
            //}
            return spec == actually;
        }

        //private string[] get(string resource)
        //{
        //    string[] jsonSplit = new string[resource.Split(',').Length];
        //    JObject json = JObject.Parse(resource);
        //    int i = 0;
        //    foreach (var item in json)
        //    {
        //        jsonSplit[i] = item.ToString();
        //        i++;
        //    }
        //    return jsonSplit;//json.SelectToken("languageId").ToString();
        //}

        private async Task<string> get(HttpClient httpClient, string api)
        {
            string response = "";
            var res = await httpClient.GetAsync(api);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                response = httpClient.GetStringAsync(api).GetAwaiter().GetResult();
            }else if(res.StatusCode == HttpStatusCode.Unauthorized)
            {
                response = "Unauthorized";
            }
                
            return response;
        }

        private async Task<string> update(HttpClient httpClient, string api, string payload)
        {
            string response = "";
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var message = await httpClient.PostAsync(api, httpContent);
            if (message.StatusCode == HttpStatusCode.Forbidden)
            {
                response = "Forbidden";
            }
            else if (message.StatusCode == HttpStatusCode.NoContent)
            {
                response = "No Content";
            }
            else if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                response = "Unauthorized";
            }
            else if (message.StatusCode == HttpStatusCode.BadRequest)
                response = "Bad Request";
                return response;
        }

        private string creaateStringToWriteInTheFile(string spec, string actually)
        {
            string result = "SPECTED: " + spec + "\n" + "ACTUALLY: "+actually; 
            //string result = spec[0]+"    { ";
            //for (int i=1; i<spec.Length; i++)
            //{
            //    result += spec[i]+"  ";
            //}
            //result += " }  { ";
            //for (int i = 0; i < actually.Length; i++)
            //{
            //    result += actually[i]+"   ";
            //}
            //result += " }";
            return result;
        }
    }
}
