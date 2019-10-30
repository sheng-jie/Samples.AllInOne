using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Options.Demo
{
    public class DeveloperOptions
    {
        public Dictionary<string, WeixinOptions> Developer { get; set; }
    }

    public class WeixinOptions
    {
        public string WeixinAppId { get; set; }
        public string WeixinAppSecret { get; set; }
        public Templates Templates { get; set; }
    }



    public class Templates
    {
        public string BindingRequest { get; set; }
        public string ApproveBindingRequest { get; set; }
        public string OnlineNotice { get; set; }
        public string WarningNotice { get; set; }
    }
}
