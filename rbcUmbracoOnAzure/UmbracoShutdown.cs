using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;

namespace rbcUmbracoOnAzure
{
    public class UmbracoApplication : Umbraco.Web.UmbracoApplication
    {
        protected override void OnApplicationEnd(object sender, EventArgs e)
        {
            base.OnApplicationEnd(sender, e);

            //set server to not active in database
            string websiteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
            string instanceId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var instance = db.SingleOrDefault<AzureWebsitesInstanceDto>("WHERE InstanceId=@0 AND WebsiteName=@1", instanceId, websiteName);
            if (instance != null)
            {
                instance.IsActive = false;
                db.Save(instance);
            }
        }
    }
}