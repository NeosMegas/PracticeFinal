using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PracticeFinal.WebAPI.Models;
using System.Collections;
using System.Net;

namespace PracticeFinal.Web.Models
{
    public class SiteInfos : IEnumerable
    {
        public Dictionary<string, string> Infos { get; set; }

        public SiteInfos(IConfiguration appsettings, HttpContext httpContext)
        {
            Infos = new Dictionary<string, string>();
            for (int i = 1; i < 14; i++)
            {
                KeyValuePair<string, string> kvp = GetSiteInfo(i, appsettings, httpContext);
                if (!Infos.ContainsKey(kvp.Key))
                {
                    if (kvp.Key == "splashText")
                        if (Infos["useRandomSplash"] == "1")
                        {
                            Infos.Add(kvp.Key, Utils.GetRandomSplash());
                            continue;
                        }
                    Infos.Add(kvp.Key, kvp.Value);
                }
            }
        }

        private static KeyValuePair<string, string> GetSiteInfo(int id, IConfiguration appsettings, HttpContext httpContext)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            var response = httpClient.GetAsync($"siteinfo/{id}");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                KeyValuePair<string, string> siteInfo = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(result);
                return siteInfo!;
            }
            return KeyValuePair.Create("", "");
        }

        public IEnumerator GetEnumerator() => Infos.GetEnumerator();

        public string this[string name]
        {
            get {
                //if (name == "splashText")
                //{
                //    if (Infos["useRandomSplash"] == "1")
                //        return Utils.GetRandomSplash();
                //    else
                //        return Infos[name];
                //}
                //else
                    return Infos[name];
            }
        }
    }
}
