using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmailVerifyTest.Common
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public string Index()
        {
            return "I'm Index Page!";
        }

        [Authorize] // 登录后请求还是被拦截
        [HttpGet]
        public string Test()
        {
            return "test";
        }

        [HttpGet]   // 不加会自动拦截
        public string Login()
        {
            return "Hello I'm Login Page!";
        }


        //[HttpGet] // 用来跳转的页面不指定请求类型,不然重定向要对应请求类型,但不能直接请求(加了认证后)
        public string Login2()  // 登录失败过来的，以便测试方便
        {
            return "Hello I'm Login2 Page!";
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // login functionality
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                // sign in
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                } else
                {
                    // return "密码错误";
                    return RedirectToAction("Login2");
                }
            }
            // 当前用户不存在
            return RedirectToAction("Login2");   // 直接返回会重定向到 post 的 login 请求, 就算不同名也不行, 若该login2 没有显示声明请求类型可以跳转
        }

        [HttpGet]
        public string Register()
        {
            return "Hello, I'm register page!";
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {

            // register functionality
            var user = new IdentityUser
            {
                UserName = username,
                Email = "",
            };
        

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // sign user here
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {

                    // generation of the email token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code },
                        Request.Scheme, Request.Host.ToString());
                    
                    await _emailService.SendAsync("testo@Testo.com", "email verify", $"<a href=\"{link}\">Verify Email</a>", true);

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest();

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return View();
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
