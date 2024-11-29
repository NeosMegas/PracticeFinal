using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeFinal.WebAPI.Models;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeFinal.WebAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("[controller]")]
    [ApiController]
    public class SiteInfoController : ControllerBase
    {
        // GET: api/<SiteInfoController>
        [AllowAnonymous]
        [HttpGet("{name:alpha}")]
        public string Get(string name, [FromServices] ApplicationContext appContext)
        {
            return appContext.SiteInfos.FirstOrDefault(si => si.StringId == name)?.Content ?? "";
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public KeyValuePair<string, string> Get(int id, [FromServices] ApplicationContext appContext)
        {
            SiteInfo? si = appContext.SiteInfos.FirstOrDefault(si => si.Id == id);
            if (si == null) return new KeyValuePair<string, string>("", "");
            return new KeyValuePair<string, string>(si.StringId, si.Content);
        }
        // POST api/<SiteInfoController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/<SiteInfoController>/5
        [HttpPut]
        public IActionResult Put(string name, string value, [FromServices] ApplicationContext appContext)
        {
            SiteInfo? si = appContext.SiteInfos.FirstOrDefault(si => si.StringId == name);
            if (si == null) return new BadRequestResult();
            si.Content = value;
            if (appContext.Update(si).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                if (appContext.SaveChanges() > 0)
                    return new JsonResult(si);
            return new StatusCodeResult(418);
        }

        // DELETE api/<SiteInfoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
