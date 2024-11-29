using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeFinal.WebAPI.Models;
using PracticeFinal.WebAPI.Models.SiteItems;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeFinal.WebAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("[controller]")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{subRoute}")]
        public IEnumerable<MusketeerSiteItem> Get(string subRoute, [FromServices] ApplicationContext appContext)
        {
            switch (subRoute)
            {
                case "projects":
                    return appContext.MusketeerProjects;
                case "blogitems":
                    return appContext.MusketeerBlogItems;
                case "services":
                    return appContext.MusketeerServices;
                default:
                    return null!;
            }
        }

        // POST <SiteController>
        [HttpPost("{subRoute}")]
        public IActionResult Post(string subRoute, [FromServices] ApplicationContext appContext)
        {
            string name = Request.Form["name"].ToString();
            string description = Request.Form["description"].ToString();
            string positionString = Request.Form["position"].ToString();
            int? position;
            int s;
            switch (subRoute)
            {
                case "projects":
                    position = appContext.MusketeerProjects.OrderBy(s => s.Position).LastOrDefault()?.Position + 1;
                    if (int.TryParse(positionString, out s))
                        position = s;
                    position ??= 0;
                    string image = Request.Form["image"].ToString();
                    MusketeerProject project = new MusketeerProject()
                    {
                        Name = name,
                        Description = description,
                        Position = position.Value,
                        Image = image
                    };
                    if (appContext.MusketeerProjects.Add(project).State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                case "blog":
                    position = appContext.MusketeerBlogItems.OrderBy(s => s.Position).LastOrDefault()?.Position + 1;
                    if (int.TryParse(positionString, out s))
                        position = s;
                    position ??= 0;
                    image = Request.Form["image"].ToString();
                    string header = Request.Form["header"].ToString();
                    string content = Request.Form["content"].ToString();
                    string largeimage = Request.Form["largeimage"].ToString();
                    string publishdate = Request.Form["publishdate"].ToString();
                    MusketeerBlogItem blogItem = new MusketeerBlogItem()
                    {
                        Name = name,
                        Description = description,
                        Position = position.Value,
                        Image = image,
                        Header = header,
                        Content = content,
                        LargeImage = largeimage,
                        PublishDate = DateTime.Parse(publishdate)
                    };
                    if (appContext.MusketeerBlogItems.Add(blogItem).State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                case "services":
                    position = appContext.MusketeerServices.OrderBy(s => s.Position).LastOrDefault()?.Position + 1;
                    if (int.TryParse(positionString, out s))
                        position = s;
                    position ??= 0;
                    MusketeerService service = new MusketeerService()
                    {
                        Name = name,
                        Description = description,
                        Position = position.Value
                    };
                    if(appContext.MusketeerServices.Add(service).State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                default:
                    return null!;
            }
        }

        // PUT api/<SiteController>/5
        [HttpPut("{subRoute}")]
        public IActionResult Put(string subRoute, [FromServices] ApplicationContext appContext)
        {
            string idString = Request.Form["id"].ToString();
            int id;
            if (!int.TryParse(idString, out id) || id < 0) return new NotFoundResult();
            string name = Request.Form["name"].ToString();
            string description = Request.Form["description"].ToString();
            string positionString = Request.Form["position"].ToString();
            int position;
            switch (subRoute)
            {
                case "projects":
                    MusketeerProject? project = appContext.MusketeerProjects.FirstOrDefault(p => p.Id == id);
                    if (project == null) return new NotFoundResult();
                    if(int.TryParse(positionString, out position))
                        project.Position = position;
                    string image = Request.Form["image"].ToString();
                    if (image != null)
                        project.Image = image;
                    project.Name = name;
                    project.Description = description;
                    if (appContext.MusketeerProjects.Update(project).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                case "blog":
                    MusketeerBlogItem? blogItem = appContext.MusketeerBlogItems.FirstOrDefault(p => p.Id == id);
                    if (blogItem == null) return new NotFoundResult();
                    if (int.TryParse(positionString, out position))
                        blogItem.Position = position;
                    image = Request.Form["image"].ToString();
                    string largeImage = Request.Form["largeimage"].ToString();
                    string header = Request.Form["header"].ToString();
                    string content = Request.Form["content"].ToString();
                    string publishDateString = Request.Form["publishdate"].ToString();
                    if (image != null)
                        blogItem.Image = image;
                    if (largeImage != null)
                        blogItem.LargeImage = largeImage;
                    blogItem.Name = name;
                    blogItem.Description = description;
                    blogItem.Header = header;
                    blogItem.Content = content;
                    blogItem.PublishDate = DateTime.Parse(publishDateString);
                    if (appContext.MusketeerBlogItems.Update(blogItem).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                case "services":
                    MusketeerService? service = appContext.MusketeerServices.FirstOrDefault(s => s.Id == id);
                    if (service == null) return new NotFoundResult();
                    if (int.TryParse(positionString, out position))
                        service.Position = position;
                    service.Name = name;
                    service.Description = description;
                    if (appContext.MusketeerServices.Update(service).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                default:
                    return null!;
            }
        }

        // DELETE api/<SiteController>/5
        [HttpDelete("{subRoute}")]
        public IActionResult Delete(string subRoute, int id, [FromServices] ApplicationContext appContext)
        {
            //string idString = Request.Form["id"].ToString();
            //int id;
            ///if (!int.TryParse(idString, out id) || id < 0) return new NotFoundResult();
            switch (subRoute)
            {
                case "projects":
                    MusketeerProject? project = appContext.MusketeerProjects.FirstOrDefault(p => p.Id == id);
                    if (project == null) return new NotFoundResult();
                    if (appContext.MusketeerProjects.Remove(project).State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                case "blog":
                    MusketeerBlogItem? blogItem = appContext.MusketeerBlogItems.FirstOrDefault(p => p.Id == id);
                    if (blogItem == null) return new NotFoundResult();
                    if (appContext.MusketeerBlogItems.Remove(blogItem).State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                case "services":
                    MusketeerService? service = appContext.MusketeerServices.FirstOrDefault(s => s.Id == id);
                    if (service == null) return new NotFoundResult();
                    if (appContext.MusketeerServices.Remove(service).State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
                    {
                        if (appContext.SaveChanges() > 0)
                            return Ok();
                        else
                            return StatusCode(417);
                    }
                    return StatusCode(418);
                default:
                    return null!;
            }
        }
    }
}
