using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthorizeTest4.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize]
        public string login()
        {
            return "login";
        }

        public string index()
        {
            return "I'm idnex Page!";
        }

        [Authorize(Policy = "Claim.DoB")]
        public string SecretPolicy()
        {
            return "Hello, I'm policy secret!";
        }

        [Authorize(Roles = "Admin")]
        public string SecretRole()
        {
            return "Hello, I'm role secret!";
        }

        [Authorize(Roles = "Admin222")]
        public string SecretRole2()
        {
            return "Hello, I'm role secret!";
        }

        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "Bob@famil.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2020"),
                new Claim(ClaimTypes.Role, "Admin"),
		        new Claim("Grandma.Says", "Very nice boi."),

            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob K Foo"),
                new Claim("DrivingLicense", "A+"),
            };


            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);
            return RedirectToAction("index");

            //var str = HttpContext.Request.Query["ReturnUrl"];   // 拿到原来路径 /api/user/login
            //return Redirect(str);   //  跳回原来请求地址 -- 注意前面代码要修改，成功后才跳回去

        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
