using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Web.Cache;
using System.Web.Routing;
using System.Web.Mvc;

namespace rbcUmbracoOnAzure
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;

            //setup table if we need to
            if (!db.TableExist("_AzureWebsitesInstance"))
                db.CreateTable<AzureWebsitesInstanceDto>(false);

            bool isUmbracoBackOffice = Convert.ToBoolean(ConfigurationManager.AppSettings["IsUmbracoBackOffice"]);

            if (isUmbracoBackOffice)
            {
                //hook into event that will push out to all front end servers
                CacheRefresherBase<PageCacheRefresher>.CacheUpdated += PublishedPageCacheRefresherCacheUpdated;
            }
            else
            {
                //register this azure websites instance in the database
                string websiteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
                string instanceId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
                string computerName = Environment.MachineName;
                if (!String.IsNullOrEmpty(websiteName) && !String.IsNullOrEmpty(instanceId))
                {
                    var instance = db.SingleOrDefault<AzureWebsitesInstanceDto>("WHERE InstanceId=@0 AND WebsiteName=@1", instanceId, websiteName);
                    if (instance == null)  //create new isntance
                        instance = new AzureWebsitesInstanceDto() { InstanceId = instanceId, WebsiteName = websiteName, ComputerName = computerName, RegisteredDate = DateTime.Now };

                    //update existing/new instance
                    instance.LastNotifiedDate = DateTime.Now;
                    instance.IsActive = true;

                    db.Save(instance); 

                    LogHelper.Info<UmbracoStartup>("Azure Website Instance Started " + instanceId);

                    //add in some logging when distributed front end servers are hit
                    umbraco.content.AfterUpdateDocumentCache += content_AfterUpdateDocumentCache;
                    umbraco.content.AfterClearDocumentCache += content_AfterClearDocumentCache;
                }
                else
                    LogHelper.Warn<UmbracoStartup>("Azure Website InstanceId or WebsiteName blank");

                //add a view so we can see all front-end instances for debugging
                RouteTable.Routes.MapRoute(
                    name: "rbcUmbracoOnAzure",
                    url: "App_Plugins/rbcUmbracoOnAzure/{action}",
                    defaults: new { controller = "rbcUmbracoOnAzure", action = "CurrentInstances" });
            }
        }

        //event called from backoffice server only
        void PublishedPageCacheRefresherCacheUpdated(ICacheRefresher sender, CacheRefresherEventArgs e)
        {
            UmbracoDistributedCall.RefreshServers(e);
        }

        //these two events only ever called from front-end servers
        void content_AfterClearDocumentCache(umbraco.cms.businesslogic.web.Document sender, umbraco.cms.businesslogic.DocumentCacheEventArgs e)
        {
            LogHelper.Info<UmbracoStartup>("content_AfterClearDocumentCache " +
                Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") + "." +
                Environment.MachineName + "." +
                Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
        }

        void content_AfterUpdateDocumentCache(umbraco.cms.businesslogic.web.Document sender, umbraco.cms.businesslogic.DocumentCacheEventArgs e)
        {
            LogHelper.Info<UmbracoStartup>("content_AfterUpdateDocumentCache " +
                Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") + "." +
                Environment.MachineName + "." +
                Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
        }
    }
}