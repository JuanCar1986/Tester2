// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;
using RSMS.Models.InputModels.Login;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tester;

class Program
{
    static async Task Main(string[] args)
    {
        string apisFile= args[0];
        List<Credentials> usersList = new List<Credentials>();
        Credentials u1 = new Credentials();
        u1.Username = "pomini";
        u1.Password = "c";
        Credentials u2 = new Credentials();
        u2.Username = "operator";
        u2.Password = "pomini";
        usersList.Add(u1);
        usersList.Add(u2);

        Testing t = new Testing();
        t.Url = "https://localhost:5001/api/Login";
        await t.Test(apisFile, usersList);
        //GestioneDB db = new GestioneDB();
        //db.DataReader();
    }
}


//var url = "https://localhost:5001/api/Login";
//StringContent httpContent = new StringContent(@"{ ""username"": ""operator"", ""password"": ""pomini"" }", Encoding.UTF8, "application/json");
//using (var httpClient = new HttpClient())
//{
//    var response = await httpClient.PostAsync(url, httpContent);
//    var res = await response.Content.ReadAsStringAsync();
//    var tipo = res.GetType().ToString();
//    JObject json = JObject.Parse(res);
//    var token = json.SelectToken("token");
//    var options = new JsonSerializerOptions();
//    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
//    string requestAddress = "https://localhost:5001/api/Usersettings";
//    var items = await httpClient.GetStringAsync(requestAddress);
//    Debug.Assert(items != null);
//    Console.WriteLine(items);
//}