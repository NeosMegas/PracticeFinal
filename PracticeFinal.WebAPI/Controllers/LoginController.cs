using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PracticeFinal.WebAPI.Models;
using PracticeFinal.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PracticeFinal.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        bool CheckUserNameAndPassword(string userName, string password, ref User? user, [FromServices] ApplicationContext userContext)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                user = userContext.Users.FirstOrDefault(u => u.Name == userName && u.Password == password);
                if (user != null)
                    return true;
                else
                    return false;
                //user = new User(0, userName, "email", password, new string[] { "admin" });
            }
            return false;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public StatusCodeResult Get()
        {
            return new StatusCodeResult(200);
        }

        // GET api/<LoginController>/5
        //[HttpGet("{user}")]
        //public string Get(User user)
        //{
        //    string userName = Request.Query.FirstOrDefault(x => x.Key == "Name").Value.ToString();
        //    return userName;
        //    //List<Claim> claims = [new Claim(ClaimTypes.Name, userName)];
        //    // создаем JWT-токен
        //    //var jwt = new JwtSecurityToken(
        //    //        issuer: AuthOptions.ISSUER,
        //    //        audience: AuthOptions.AUDIENCE,
        //    //        claims: claims,
        //    //        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
        //    //        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        //    //return new JwtSecurityTokenHandler().WriteToken(jwt);
        //}

        // POST api/<LoginController>
        [HttpPost]
        public IActionResult Post(AuthService service, [FromServices] ApplicationContext appContext)
        {
            string userName = Request.Form["username"].ToString();
            string password = Request.Form["password"].ToString();
            User? user = null;
            if (CheckUserNameAndPassword(userName, password, ref user!, appContext))
            {
                return new ContentResult() { Content = service.CreateToken(user) };
            }
            return new UnauthorizedResult();
        }

        //// PUT api/<LoginController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<LoginController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
