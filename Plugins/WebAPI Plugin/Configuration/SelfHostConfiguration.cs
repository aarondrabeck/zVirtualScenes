using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using System.Web.Http.SelfHost.Channels;

namespace WebAPI.Configuration
{
    public class SelfHostConfiguration : HttpSelfHostConfiguration
    {
        public SelfHostConfiguration(string baseAddress) : base(baseAddress) { }
        public SelfHostConfiguration(Uri baseAddress) : base(baseAddress) { }

        public bool EnableSSL { get; set; }
        private string sslFile = "ssl.gen";
        private string enableSSL = "enableSSL.bat";
        protected override BindingParameterCollection OnConfigureBinding(HttpBinding httpBinding)
        {
            if (EnableSSL)
            {
                //https://pfelix.wordpress.com/2012/02/26/enabling-https-with-self-hosted-asp-net-web-api/
                httpBinding.Security.Mode = HttpBindingSecurityMode.Transport;
                

            }
            return base.OnConfigureBinding(httpBinding);
        }
    }
}