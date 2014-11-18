using System.Web.Http;

namespace zvsWebapi2Plugin.Controllers
{
    public class HelloController : ApiController
    {
        public string Get()
        {
            return "Hello, world!";
        }
    }

}
