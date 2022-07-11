using EgtDemo.Common.Configuration;
using EgtDemo.Common.Helper;
using EgtDemo.Extensions;
using EgtDemo.IService;
using EgtDemo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EgtDemo.Controllers
{
    /// <summary>
    /// WeatherForecast控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDemoService _demoService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAdvertisementService _advertisementService;
        private readonly Appsetting1 appsetting1;
        private readonly IOptions<Logging> options;
        private readonly IConfiguration configuration;
        private readonly IHubContext<ChatHub> hubContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDemoService demoService, 
            IHttpContextAccessor contextAccessor,IAdvertisementService advertisementService,
            Appsetting1 appsetting1,IOptionsSnapshot<Logging> options, //热加载，如果是IOptions<>则没有这样的功能
            IConfiguration configuration,IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _demoService = demoService;
            _contextAccessor = contextAccessor;
            _advertisementService = advertisementService;
            this.appsetting1 = appsetting1;
            this.options = options;
            this.configuration = configuration;
            this.hubContext = hubContext;
        }

        /// <summary>
        /// 获取Demo实例
        /// </summary>
        /// <returns></returns>
        [HttpGet("getWeatherForecast")]
        //[Authorize(Roles ="Admin")]
        //[Authorize(Policy = "AdminOrUser")]
        //[Authorize(Policy = "AdminAndUser")]
        //[Authorize(Policy ="Email")]
        //[Authorize(Policy = "BaseOnRequirement")]
        public IEnumerable<WeatherForecast> Get()
        {
            hubContext.Clients.All.SendAsync("ReceiveMsg", "DreamyZhao", "爱你哦宝贝！").Wait();

            //_logger.LogInformation("log information!");
            _logger.LogInformation("控制台这下可以输出了吧。。。");

            var demo = _demoService.GetDemo();

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                Demo = demo
            })
            .ToArray();
        }

        [HttpGet]
        [ApiExplorerSettings(GroupName = "v2")]
        //[Route("api/v2/weatherForecast/egtDemoV2")]       
        [CustomRoute(ApiVersion.v2, "getEgtDemoV2")]
        public IActionResult V2_EgtDemo()
        {
            hubContext.Clients.All.SendAsync("ReceiveMsg", "DreamyZhao", "爱你哦宝贝！").Wait();

            using (MiniProfiler.Current.Step("开始加载数据："))
            {
                string conn = appsetting1.AppSettings.SqlServerConnection;
                var logLevel = options.Value.LogLevel;
                //通过帮助类AppSettings.cs去灵活获取需要的配置项
                var redisConnStr = Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" });
                var she = Appsettings.GetVal("Chinese");
                _logger.LogInformation($"女朋友的中文名：{she}");
                _logger.LogInformation($"Redis缓存的数据库连接字符串：{redisConnStr}");

                var name = configuration["Name"];
                var pwd = configuration["Pwd"];
                _logger.LogInformation($"Name:{name},Pwd:{pwd}");

                MiniProfiler.Current.Step("数据加载完毕...");

                return Ok(new { status = 200, data = "第二个版本的返回信息！", name, pwd });
            }
        }

        /// <summary>
        /// 创建实例接口
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("createDemo")]
        public IActionResult Post()
        {
            return NoContent();
        }

        /// <summary>
        /// 更新实例接口
        /// </summary>
        //[Obsolete]
        [HttpPut("updateDemo")]
        public IActionResult Put()
        {
            return NoContent();
        }

        /// <summary>
        /// 局部更新接口
        /// </summary>
        /// <returns></returns>
        [HttpPatch("patchDemo")]
        private IActionResult Patch()
        {
            return NoContent();
        }

        /// <summary>
        /// 获取jwt Token
        /// </summary>
        /// <returns>token</returns>
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<string>> CreateToken()
        {
            //秘钥签名
            var secret = "jimmy2022jimmy2022";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //创建 声明claim 数组
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,"JimmyWu"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.Role,"User"),
                //new Claim(ClaimTypes.NameIdentifier,"DreamyZhao"), //如果是signalR基于jwtToken去发送给某个特定人的消息，则加上此声明，然后用这个名字登录则可以发送给这个声明对应的人DreamyZhao!
                new Claim(JwtRegisteredClaimNames.Email,"jimmwy@qq.com"),
                new Claim(JwtRegisteredClaimNames.Sub,"9527")
            };

            var tokenBuild = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5002",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: sign
                );
            //生成token
            var token = new JwtSecurityTokenHandler().WriteToken(tokenBuild);

            return new string[] { token };
        }

        /// <summary>
        /// 解析token并获取用户信息
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tokenStr}")]
        [Authorize]
        public ActionResult<IEnumerable<string>> GetUserInfo(string tokenStr)
        {
            //方法一：
            var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenStr);
            //方法二：
            var name1 = User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            //方法三：
            var name2 = _contextAccessor.HttpContext.User.Identity.Name;
            var claims = _contextAccessor.HttpContext.User.Claims;
            var emails = (from item in claims
                                  where item.Type == JwtRegisteredClaimNames.Email
                                  select item.Value).ToList();

            return new string[] { JsonConvert.SerializeObject(jwtSecurityToken), name1, name2, JsonConvert.SerializeObject(emails) };
        }

        /// <summary>
        /// 测试日志AOP
        /// </summary>
        /// <returns></returns>
        [HttpGet("TestAOP")]
        public List<Advertisement> TestAdsFromAOP()
        {
            //通过帮助类AppSettings.cs去灵活获取需要的配置项
            var redisConnStr = Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" });
            _logger.LogInformation($"Redis缓存的数据库连接字符串：{redisConnStr}");

            return _advertisementService.TestAOP();
        }

        [HttpGet("getApiResource")]
        //[Authorize]
        //[AllowAnonymous]
        public IActionResult GetApiResource()
        {
            return new JsonResult(
                from c in User.Claims
                select new { c.Type, c.Value }
                );
        }
    }
}
