using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace rbcUmbracoOnAzure.Controllers
{
    public class RbcUmbracoOnAzureController : UmbracoController
    {
        public ActionResult CurrentInstances()
        {
            return View("~/App_Plugins/rbcUmbracoOnAzure/Views/CurrentInstances.cshtml", null);
        }
    }
}