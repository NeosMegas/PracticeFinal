using Microsoft.AspNetCore.Mvc;
using PracticeFinal.Web.Models;
using System.Diagnostics;
using PracticeFinal.WebAPI.Models;
using PracticeFinal.WebAPI.Models.SiteItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
using System;

namespace PracticeFinal.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _appsettings;
        private readonly IWebHostEnvironment _appEnvironment;
        private SiteInfos _siteInfos;

        public HomeController(ILogger<HomeController> logger, IConfiguration appsettings, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _appsettings = appsettings;
            _siteInfos = new SiteInfos(_appsettings, HttpContext);
            _appEnvironment = appEnvironment;
        }

        async Task UpdateSiteInfo()
        {
            await IsUserSignedIn();
            foreach (KeyValuePair<string, string> kvp in _siteInfos)
            {
                if (kvp.Key == "") continue;
                //ViewData[kvp.Key] = kvp.Value;
                HttpContext.Response.Cookies.Append(kvp.Key, kvp.Value);
            }
        }

        public async Task<IActionResult> PostUserRequest(string name, string email, string message)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(message))
            {
                UserRequest userRequest = new UserRequest() {
                    Created = DateTime.Now,
                    Name = name,
                    Email = email,
                    MessageText = message,
                    RequestStatus = RequestStatus.NewRequest
                };
                var result = await Utils.SendUserRequest(userRequest, _appsettings);
                if (result)
                    TempData["UserRequestSent"] = "ok";
                else
                    TempData["UserRequestSent"] = "error";

            }
            else
                TempData["UserRequestSent"] = "warning";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UserRequestSentIndex()
        {
            await UpdateSiteInfo();
            return View("Index");
        }

        public async Task<IActionResult> Index()
        {
            await UpdateSiteInfo();
            return View(_siteInfos);
        }

        public async Task<IActionResult> Services()
        {
            await UpdateSiteInfo();
            List<MusketeerService> services = Utils.GetMusketeerServices(_appsettings);
            return View(services);
        }

        public async Task<IActionResult> Contacts()
        {
            await UpdateSiteInfo();
            return View(_siteInfos);
        }

        public async Task<IActionResult> Projects()
        {
            await UpdateSiteInfo();
            List<MusketeerProject> projects = Utils.GetMusketeerProjects(_appsettings);
            return View(projects);
        }

        public async Task<IActionResult> Project(int id)
        {
            await UpdateSiteInfo();
            MusketeerProject? project = Utils.GetMusketeerProjects(_appsettings).FirstOrDefault(p => p.Id == id);
            if (project == null) return NotFound();
            return View(project);
        }

        public async Task<IActionResult> Blog()
        {
            await UpdateSiteInfo();
            List<MusketeerBlogItem> blogItems = Utils.GetMusketeerBlogItems(_appsettings);
            if (blogItems == null) return NotFound();
            return View(blogItems);
        }

        public async Task<IActionResult> BlogItem(int id)
        {
            await UpdateSiteInfo();
            MusketeerBlogItem? blogItem = Utils.GetMusketeerBlogItems(_appsettings).FirstOrDefault(p => p.Id == id);
            if (blogItem == null) return NotFound();
            return View(blogItem);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region WorkWithUserRequests

        public async Task<IActionResult> CRM()
        {
            await UpdateSiteInfo();
            ViewBag.TotalUserRequests = Utils.GetUserRequests(_appsettings, HttpContext)?.Count;
            List<UserRequest> userRequests = Utils.GetUserRequests(_appsettings, HttpContext);
            return View(userRequests);
        }

        public async Task<IActionResult> ShowUserRequestsToday()
        {
            return await ShowUserRequestsPeriod(DateTime.Today, DateTime.Today.AddDays(1));
        }

        public async Task<IActionResult> ShowUserRequestsYesterday()
        {
            return await ShowUserRequestsPeriod(DateTime.Today.AddDays(-1), DateTime.Today);
        }

        public async Task<IActionResult> ShowUserRequestsWeek()
        {
            DateTime t2 = DateTime.Now;
            DateTime t1 = t2.AddDays(-7);
            return await ShowUserRequestsPeriod(t1, t2);
        }

        public async Task<IActionResult> ShowUserRequestsMonth()
        {
            DateTime t2 = DateTime.Now;
            DateTime t1 = t2.AddDays(-30);
            return await ShowUserRequestsPeriod(t1, t2);
        }

        public async Task<IActionResult> ShowUserRequestsPeriod(DateTime startDate, DateTime endDate)
        {
            await UpdateSiteInfo();
            ViewBag.TotalUserRequests = Utils.GetUserRequests(_appsettings, HttpContext)?.Count;
            DateTime t1 = startDate;
            DateTime t2 = endDate;
            List<UserRequest> userRequests = Utils.GetUserRequestsDates(_appsettings, HttpContext, t1, t2);
            ViewData["t1"] = t1;
            ViewData["t2"] = t2;
            return View("CRM", userRequests);
        }

        public async Task<IActionResult> ChangeRequestStatus(int id, int status)
        {
            HttpStatusCode returnCode = await Utils.ChangeRequestStatus(_appsettings, HttpContext, id, (RequestStatus)status);
            if (returnCode == HttpStatusCode.OK)
                return RedirectToAction("CRM");
            else
                return StatusCode(((int)returnCode));
        }

        #endregion

        #region SiteEditor

        public async Task<IActionResult> SiteInfo(int mode)
        {
            await UpdateSiteInfo();
            ViewData["mode"] = mode;
            return View(_siteInfos);
        }

        [HttpPost]
        public async Task<IActionResult> PutSiteInfo(int mode, string mainTitle,
            string useRandomSplash, string splashText, string mainButtonText,
            string projectsButtonText, string servicesButtonText, string blogButtonText, string contactsButtonText,
            string contactAddress, string contactPhone, string contactEmail, string contactName)
        {
            if (
                (mode == 1 && (string.IsNullOrEmpty(mainTitle) || //string.IsNullOrEmpty(useRandomSplash) ||
                string.IsNullOrEmpty(splashText) || string.IsNullOrEmpty(mainButtonText) ||
                string.IsNullOrEmpty(projectsButtonText) || string.IsNullOrEmpty(servicesButtonText) ||
                string.IsNullOrEmpty(blogButtonText) || string.IsNullOrEmpty(contactsButtonText)))
                ||
                (mode == 2 && (string.IsNullOrEmpty(contactAddress) ||
                string.IsNullOrEmpty(contactPhone) || string.IsNullOrEmpty(contactEmail) ||
                string.IsNullOrEmpty(contactName)))
                )
                return RedirectToAction("CRM");


            Dictionary<string, string> siteInfos = new Dictionary<string, string>();
            if(mode == 1)
            {
                if (_siteInfos["mainTitle"] != mainTitle) siteInfos.Add("mainTitle", mainTitle);
                //if (_siteInfos[useRandomSplash] != useRandomSplash) siteInfos.Add("useRandomSplash", useRandomSplash);
                if (_siteInfos["splashText"] != splashText) siteInfos.Add("splashText", splashText);
                if (_siteInfos["mainButtonText"] != mainButtonText) siteInfos.Add("mainButtonText", mainButtonText);
                if (_siteInfos["projectsButtonText"] != projectsButtonText) siteInfos.Add("projectsButtonText", projectsButtonText);
                if (_siteInfos["servicesButtonText"] != servicesButtonText) siteInfos.Add("servicesButtonText", servicesButtonText);
                if (_siteInfos["blogButtonText"] != blogButtonText) siteInfos.Add("blogButtonText", blogButtonText);
                if (_siteInfos["contactsButtonText"] != contactsButtonText) siteInfos.Add("contactsButtonText", contactsButtonText);
            }
            else if (mode == 2)
            {
                if (_siteInfos["contactAddress"] != contactAddress) siteInfos.Add("contactAddress", contactAddress);
                if (_siteInfos["contactPhone"] != contactPhone) siteInfos.Add("contactPhone", contactPhone);
                if (_siteInfos["contactEmail"] != contactEmail) siteInfos.Add("contactEmail", contactEmail);
                if (_siteInfos["contactName"] != contactName) siteInfos.Add("contactName", contactName);
            }

            HttpStatusCode result = await Utils.PutSiteInfo(siteInfos, _appsettings, HttpContext);
            if (result != HttpStatusCode.OK)
                return new StatusCodeResult(((int)result));
            //_siteInfos = new SiteInfos(_appsettings, HttpContext);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SiteItems(string type)
        {
            await UpdateSiteInfo();
            switch (type)
            {
                case "MusketeerProject":
                    List<MusketeerProject> projects = Utils.GetMusketeerProjects(_appsettings);
                    return View(projects);
                case "MusketeerService":
                    List<MusketeerService> services = Utils.GetMusketeerServices(_appsettings);
                    return View(services);
                case "MusketeerBlogItem":
                    List<MusketeerBlogItem> blogItems = Utils.GetMusketeerBlogItems(_appsettings);
                    return View(blogItems);
            }
            return View("CRM");
        }

        public async Task<IActionResult> SiteItemEdit(string type, int id)
        {
            await UpdateSiteInfo();
            IEnumerable<MusketeerSiteItem> items;
            switch (type)
            {
                case "MusketeerService":
                    items = Utils.GetMusketeerServices(_appsettings);
                    MusketeerService? service;
                    if (id > -1)
                    {
                        service = items.FirstOrDefault(p => p.Id == id) as MusketeerService;
                        if (service == null) return new NotFoundResult();
                    }
                    else
                        service = new MusketeerService() { Id = -1, Name = "", Description = "", Position = items.Max(i => i.Position) + 1 };
                    return View(service);
                case "MusketeerProject":
                    items = Utils.GetMusketeerProjects(_appsettings);
                    MusketeerProject? project;
                    if (id > -1)
                    {
                        project = items.FirstOrDefault(p => p.Id == id) as MusketeerProject;
                        if (project == null) return new NotFoundResult();
                    }
                    else
                        project = new MusketeerProject() { Id = -1, Name = "", Description = "",
                            Position = items.Max(i => i.Position) + 1, Image = "" }; 
                    return View(project);
                case "MusketeerBlogItem":
                    items = Utils.GetMusketeerBlogItems(_appsettings);
                    MusketeerBlogItem? blogItem;
                    if (id > -1)
                    {
                        blogItem = items.FirstOrDefault(p => p.Id == id) as MusketeerBlogItem;
                        if (blogItem == null) return new NotFoundResult();
                    }
                    else
                        blogItem = new MusketeerBlogItem() { Id = -1, Name = "", Description = "",
                            Position = items.Max(i => i.Position) + 1, Image = "",
                            Header = "", Content = "", LargeImage = "", PublishDate = DateTime.Now};
                    return View(blogItem);
            }
            return RedirectToAction("CRM");
        }

        public async Task<IActionResult> SiteItemSubmit(string type, int id, string name, string description, int position,
            IFormFile image,
            string header, string content, IFormFile largeImage, DateTime publishDate)
        {
            await UpdateSiteInfo();
            IEnumerable<MusketeerSiteItem> items;
            HttpStatusCode result;
            switch (type)
            {
                case "MusketeerService":
                    items = Utils.GetMusketeerServices(_appsettings);
                    MusketeerService? service;
                    if (id > -1)
                    {
                        service = items.FirstOrDefault(p => p.Id == id) as MusketeerService;
                        if (service == null) return new NotFoundResult();
                        service.Name = name;
                        service.Description = description;
                        service.Position = position;
                    }
                    else
                        service = new MusketeerService() { Name = name, Description = description, Position = position };

                    result = await Utils.SiteItemSubmit(service, _appsettings, HttpContext);
                    return RedirectToAction("Services");
                case "MusketeerProject":
                    items = Utils.GetMusketeerProjects(_appsettings);
                    MusketeerProject? project;
                    if (id > -1)
                    {
                        project = items.FirstOrDefault(p => p.Id == id) as MusketeerProject;
                        if (project == null) return new NotFoundResult();
                        project.Name = name;
                        project.Description = description;
                        project.Position = position;
                    }
                    else
                        project = new MusketeerProject() { Name = name, Description = description, Position = position };
                    
                    if (image != null)
                    {
                        await Utils.CopyImage(image, Path.Combine(_appEnvironment.WebRootPath, "images", "projects"));
                        project.Image = image.FileName;
                    }

                    result = await Utils.SiteItemSubmit(project, _appsettings, HttpContext);
                    return RedirectToAction("Projects");
                
                case "MusketeerBlogItem":
                    items = Utils.GetMusketeerBlogItems(_appsettings);
                    MusketeerBlogItem? blogItem;
                    if (id > -1)
                    {
                        blogItem = items.FirstOrDefault(p => p.Id == id) as MusketeerBlogItem;
                        if (blogItem == null) return new NotFoundResult();
                        blogItem.Name = name;
                        blogItem.Description = description;
                        blogItem.Header = header;
                        blogItem.Content = content;
                        blogItem.Position = position;
                        blogItem.PublishDate = publishDate;
                    }
                    else
                        blogItem = new MusketeerBlogItem() { Name = name, Description = description, Position = position,
                            Header = header, Content = content, PublishDate = publishDate};

                    if (image != null)
                    {
                        await Utils.CopyImage(image, Path.Combine(_appEnvironment.WebRootPath, "images", "blog"));
                        blogItem.Image = image.FileName;
                    }
                    if (largeImage != null)
                    {
                        await Utils.CopyImage(largeImage, Path.Combine(_appEnvironment.WebRootPath, "images", "blog", "large"));
                        blogItem.LargeImage = largeImage.FileName;
                    }

                    result = await Utils.SiteItemSubmit(blogItem, _appsettings, HttpContext);
                    return RedirectToAction("Blog");
            }
            return View("SiteItems");
        }

        public async Task<IActionResult> SiteItemDelete(string type, int id)
        {
            await UpdateSiteInfo();
            IEnumerable<MusketeerSiteItem> items;
            HttpStatusCode result;
            switch (type)
            {
                case "MusketeerService":
                    items = Utils.GetMusketeerServices(_appsettings);
                    MusketeerService? servce = items.FirstOrDefault(p => p.Id == id) as MusketeerService;
                    if (servce == null) return new NotFoundResult();
                    result = await Utils.SiteItemDelete(servce, _appsettings, HttpContext);
                    return RedirectToAction("Services");
                case "MusketeerProject":
                    items = Utils.GetMusketeerProjects(_appsettings);
                    MusketeerProject? project = items.FirstOrDefault(p => p.Id == id) as MusketeerProject;
                    if (project == null) return new NotFoundResult();
                    result = await Utils.SiteItemDelete(project, _appsettings, HttpContext);
                    return RedirectToAction("Projects");
                case "MusketeerBlogItem":
                    items = Utils.GetMusketeerBlogItems(_appsettings);
                    MusketeerBlogItem? blogItem = items.FirstOrDefault(p => p.Id == id) as MusketeerBlogItem;
                    if (blogItem == null) return new NotFoundResult();
                    result = await Utils.SiteItemDelete(blogItem, _appsettings, HttpContext);
                    return RedirectToAction("Blog");
            }
            return View("SiteItems");
        }


        #endregion

        public async Task IsUserSignedIn()
        {
            if(await Utils.IsUserSignedIn(_appsettings, HttpContext))
                TempData["IsUserSignedIn"] = true;
            else
                TempData["IsUserSignedIn"] = false;
        }
        
        public IActionResult Logout()
        {
            Utils.Logout(_appsettings, HttpContext);
            TempData["IsUserSignedIn"] = false;
            return RedirectToAction("Index");
        }
    }
}
