using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeFinal.WebAPI.Models;
using PracticeFinal.WebAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeFinal.WebAPI
{
    [Authorize(Roles = "admin")]
    [Route("[controller]")]
    [ApiController]
    public class UserRequestController : ControllerBase
    {
        // GET: api/<UserRequestController>
        [HttpGet]
        public IEnumerable<UserRequest> Get([FromServices] ApplicationContext appContext)
        {
            return appContext.UserRequests.ToArray();
        }

        // GET api/<UserRequestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserRequestController>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post(UserRequest userRequest, [FromServices] ApplicationContext appContext)
        {
            //string userName = Request.Form["name"].ToString();
            //string email = Request.Form["email"].ToString();
            //string message = Request.Form["message"].ToString();
            //UserRequest? userRequest = new UserRequest() { Name = userName, Email = email, MessageText = message };
            if(appContext.UserRequests.Add(userRequest).State == EntityState.Added)
                if(appContext.SaveChanges() > 0)
                    return new JsonResult(userRequest);
            return StatusCode(500);
        }

        // PUT api/<UserRequestController>/5
        [HttpPut]
        public IActionResult Put(int id, int status, [FromServices] ApplicationContext appContext)
        {
            UserRequest? userRequest = appContext.UserRequests.FirstOrDefault(x => x.Id == id);
            if (userRequest == null) return StatusCode(404);
            if (status < 0 || status > 4) return StatusCode(418);
            userRequest.RequestStatus = (RequestStatus)status;
            if(appContext.Update(userRequest).State == EntityState.Modified)
                if(appContext.SaveChanges() > 0)
                    return new JsonResult(userRequest);
            return StatusCode(500);
        }

        //// DELETE api/<UserRequestController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
