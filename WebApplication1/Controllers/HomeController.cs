using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : ApiController
    {
        [Route("index"),HttpGet]
        public string Index()
        {
            GenerateService.GenerateService generateService = new GenerateService.GenerateService();
            var result = generateService.Generate();
            return generateService.Message ?? "生成完成";
        }
    }
}
