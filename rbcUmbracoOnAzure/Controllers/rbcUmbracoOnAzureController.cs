using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace rbcUmbracoOnAzure.Controllers
{
    public class rbcUmbracoOnAzureController : UmbracoController
    {
        public ActionResult CurrentInstances()
        {
            //var db = ApplicationContext.DatabaseContext.Database;
            //var instances = db.Query<AzureWebsitesInstanceDto>("WHERE IsActive=1");
            //string instanceId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");

            return View("~/App_Plugins/rbcUmbracoOnAzure/Views/CurrentInstances.cshtml", null);
        }
    }
}