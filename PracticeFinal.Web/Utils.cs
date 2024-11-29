using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using PracticeFinal.Web.Models;
using PracticeFinal.WebAPI.Models;
using PracticeFinal.WebAPI.Models.SiteItems;
using SQLitePCL;
using System.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PracticeFinal.Web
{
    public static class Utils
    {

        static string[] splashes = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "splashes.txt"));
        public static string GetRandomImageName()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\meme"));
            return Path.GetFileName(files.ElementAt((new Random()).Next(0, files.Count() - 1)));
        }

        public static string GetRandomSplash()
        {
            return splashes[(new Random()).Next(0, splashes.Length - 1)];
        }

        public static List<UserRequest> GetUserRequests(IConfiguration appsettings, HttpContext httpContext)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return null!;
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer" , jwt);
            var response = httpClient.GetAsync("userrequest");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                List<UserRequest> projects = JsonConvert.DeserializeObject<List<UserRequest>>(result)!;
                return projects;
            }
            return null!;
        }

        public static List<UserRequest> GetUserRequestsDates(IConfiguration appsettings, HttpContext httpContext, DateTime dateFrom, DateTime dateTo)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return null!;
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            var response = httpClient.GetAsync("userrequest");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                List<UserRequest> projects = (JsonConvert.DeserializeObject<List<UserRequest>>(result)!);
                projects = new List<UserRequest>(projects.Where(ur => (ur.Created >= dateFrom && ur.Created <= dateTo)));
                return projects;
            }
            return null!;
        }

        public static List<MusketeerProject> GetMusketeerProjects(IConfiguration appsettings)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            var response = httpClient.GetAsync("site/projects");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                List<MusketeerProject> projects = JsonConvert.DeserializeObject<List<MusketeerProject>>(result)!;
                return projects;
            }
            return null!;
        }

        public static List<MusketeerService> GetMusketeerServices(IConfiguration appsettings)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            var response = httpClient.GetAsync("site/services");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                List<MusketeerService> services = JsonConvert.DeserializeObject<List<MusketeerService>>(result)!;
                return services;
            }
            return null!;
        }

        public static List<MusketeerBlogItem> GetMusketeerBlogItems(IConfiguration appsettings)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            var response = httpClient.GetAsync("site/blogitems");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                List<MusketeerBlogItem> blogItems = JsonConvert.DeserializeObject<List<MusketeerBlogItem>>(result)!;
                return blogItems;
            }
            return null!;
        }

        public static async Task<bool> SendUserRequest(UserRequest userRequest, IConfiguration appsettings)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            
            JsonContent jsonContent = JsonContent.Create(userRequest);
            using var response = await httpClient.PostAsync("userrequest", jsonContent);
            UserRequest? userRequestResponse = await response.Content.ReadFromJsonAsync<UserRequest>();
            if (userRequestResponse != null)
            {
                return true;
            }
            return false;
        }

        public static async Task<HttpStatusCode> ChangeRequestStatus(IConfiguration appsettings, HttpContext httpContext, int id, RequestStatus status)
        {
            UserRequest? userRequest = GetUserRequests(appsettings, httpContext).FirstOrDefault(r => r.Id == id);
            if (userRequest == null) return HttpStatusCode.NotFound;

            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return HttpStatusCode.Unauthorized;

            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            //JsonContent jsonContent = JsonContent.Create(userRequest);
            HttpContent httpContent = new StringContent("");
            using var response = await httpClient.PutAsync($"userrequest?id={id}&status={(int)status}", httpContent);
            UserRequest? userRequestResponse = await response.Content.ReadFromJsonAsync<UserRequest>();
            return response.StatusCode;
        }

        public static string GetSiteInfo(string name, IConfiguration appsettings, HttpContext httpContext)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            var response = httpClient.GetAsync($"siteinfo/{name}");
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                string result = response.Result.Content.ReadAsStringAsync().Result;
                string valueString = (result);
                return valueString;
            }
            return null!;
        }


        public static async Task<HttpStatusCode> PutSiteInfo(Dictionary<string, string> siteInfos, IConfiguration appsettings, HttpContext httpContext)
        {
            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return HttpStatusCode.Unauthorized;
            if (siteInfos.Count == 0) return HttpStatusCode.NotModified;

            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            HttpContent httpContent = new StringContent("");
            bool isOk = true;
            foreach (KeyValuePair<string, string> siteInfo in siteInfos)
            {
                using var response = await httpClient.PutAsync($"siteinfo?name={siteInfo.Key}&value={siteInfo.Value}", httpContent);
                SiteInfo? siteInfoResponse = await response.Content.ReadFromJsonAsync<SiteInfo>();
                if (siteInfoResponse != null && response.StatusCode == HttpStatusCode.OK)
                    isOk = true;
                else
                    isOk = false;
            }
            return isOk ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }

        public static async Task<HttpStatusCode> SiteItemSubmit(MusketeerSiteItem siteItem, IConfiguration appsettings, HttpContext httpContext)
        {
            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return HttpStatusCode.Unauthorized;

            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            //JsonContent jsonContent = JsonContent.Create(siteItem);
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                ["id"] = siteItem.Id.ToString(),
                ["name"] = siteItem.Name!,
                ["description"] = siteItem.Description!,
                ["position"] = siteItem.Position.ToString()
            };
            string route = string.Empty;
            IEnumerable<MusketeerSiteItem>? items = null;
            switch (siteItem.GetType().Name)
            {
                case "MusketeerService":
                    route = "site/services";
                    items = GetMusketeerServices(appsettings);
                    break;
                case "MusketeerProject":
                    route = "site/projects";
                    items = GetMusketeerProjects(appsettings);
                    data.Add("image", (siteItem as MusketeerProject)?.Image!);
                    break;
                case "MusketeerBlogItem":
                    route = "site/blog";
                    items = GetMusketeerBlogItems(appsettings);
                    data.Add("image", (siteItem as MusketeerBlogItem)?.Image!);
                    data.Add("header", (siteItem as MusketeerBlogItem)?.Header!);
                    data.Add("content", (siteItem as MusketeerBlogItem)?.Content!);
                    data.Add("largeimage", (siteItem as MusketeerBlogItem)?.LargeImage!);
                    data.Add("publishdate", (siteItem as MusketeerBlogItem)?.PublishDate.ToString()!);
                    break;
            }
            if (items == null) return HttpStatusCode.BadRequest;
            HttpContent formContent = new FormUrlEncodedContent(data);
            HttpResponseMessage response;
            if (items.Any(i => i.Id == siteItem.Id))
                response = await httpClient.PutAsync(route, formContent);
                //response = await httpClient.PutAsync(route, jsonContent);
            else
                response = await httpClient.PostAsync(route, formContent);
                //response = await httpClient.PostAsync(route, jsonContent);
            //MusketeerSiteItem? musketeerSiteItemResponse = await response.Content.ReadFromJsonAsync<MusketeerSiteItem>();
            return response.StatusCode;
        }

        public static async Task<HttpStatusCode> SiteItemDelete(MusketeerSiteItem siteItem, IConfiguration appsettings, HttpContext httpContext)
        {
            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return HttpStatusCode.Unauthorized;

            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            JsonContent jsonContent = JsonContent.Create(siteItem.Id);
            string route = string.Empty;
            IEnumerable<MusketeerSiteItem>? items = null;
            switch (siteItem.GetType().Name)
            {
                case "MusketeerService":
                    route = $"site/services?id={siteItem.Id}";
                    items = GetMusketeerServices(appsettings);
                    break;
                case "MusketeerProject":
                    route = $"site/projects?id={siteItem.Id}";
                    items = GetMusketeerProjects(appsettings);
                    break;
                case "MusketeerBlogItem":
                    route = $"site/blog?id={siteItem.Id}";
                    items = GetMusketeerBlogItems(appsettings);
                    break;
            }
            if (items == null) return HttpStatusCode.BadRequest;
            //HttpContent formContent = new FormUrlEncodedContent(data);
            HttpResponseMessage response = await httpClient.DeleteAsync(route);
            return response.StatusCode;
        }

        public static async Task<string> CopyImage(IFormFile file, string path)
        {
            string fileName = file.FileName;
            string filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath))
            {
                fileName = Path.GetFileNameWithoutExtension(filePath) + Guid.NewGuid().ToString() + Path.GetExtension(filePath);
                filePath = Path.Combine(path, fileName);
            }
            if (File.Exists(filePath)) return "";
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fs);
            return fileName;
        }

        public static async Task<Microsoft.AspNetCore.Identity.SignInResult> SignIn(string userName, string password, IConfiguration appsettings, HttpContext httpContext)
        {
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["username"] = userName,
                ["password"] = password
            };
            HttpContent contentForm = new FormUrlEncodedContent(data);
            using var response = await httpClient.PostAsync("login", contentForm);
            string loginResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK && loginResponse != null)
            {
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                httpContext.User = new ClaimsPrincipal(identity);
                httpContext.Response.Cookies.Append("jwt", loginResponse, new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                });
                httpContext.Response.Cookies.Append("userName", userName, new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                });
                await httpContext.SignInAsync(httpContext.User);
                
                return Microsoft.AspNetCore.Identity.SignInResult.Success;
            }
            return Microsoft.AspNetCore.Identity.SignInResult.Failed;
        }

        public static void Logout(IConfiguration appsettings, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("jwt");
            httpContext.Response.Cookies.Delete("userName");
        }


        public static async Task<bool> IsUserSignedIn(IConfiguration appsettings, HttpContext httpContext)
        {
            string? jwt = httpContext.Request.Cookies["jwt"];
            if (jwt == null) return false;
            string userName = httpContext.Request.Cookies["userName"]?? "error";
            HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(appsettings.GetValue<string>("ServerUrl")!) };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            using var response = await httpClient.GetAsync($"login");
            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString != null && response.StatusCode == HttpStatusCode.OK)
            {
                httpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                });
                httpContext.Response.Cookies.Append("userName", userName, new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10)
                });
                return true;
            }
            return false;
        }

    }
}
