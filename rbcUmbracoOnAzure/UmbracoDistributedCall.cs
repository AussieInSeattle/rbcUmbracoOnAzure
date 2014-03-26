using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Sync;
using Umbraco.Web.Cache;

namespace rbcUmbracoOnAzure
{
    public class UmbracoDistributedCall
    {
        public static void RefreshServers(CacheRefresherEventArgs e)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var instances = db.Query<AzureWebsitesInstanceDto>("WHERE IsActive=1");
            if (instances != null && instances.Any())
            {
                var user = umbraco.BusinessLogic.User.GetUser(0); //admin user

                foreach (var instance in instances)
                {
                    if (instance.WebsiteName.Length > 0 && instance.InstanceId.Length > 0)
                    {
                        LogHelper.Info<UmbracoDistributedCall>("Calling Refresh on Azure Website Instance " + instance.InstanceId);
                        string domain = instance.WebsiteName + ".azurewebsites.net";
                        using (var cacheRefresher = new ServerSyncWebServiceClient(domain))
                        {
                            switch (e.MessageType)
                            {
                                case MessageType.RefreshById:
                                    cacheRefresher.RefreshById(new Guid(DistributedCache.PageCacheRefresherId), (int)e.MessageObject, user.LoginName, user.Password, instance.InstanceId, domain);
                                    break;
                                case MessageType.RemoveById:
                                    cacheRefresher.RemoveById(new Guid(DistributedCache.PageCacheRefresherId), (int)e.MessageObject, user.LoginName, user.Password, instance.InstanceId, domain);
                                    break;
                                case MessageType.RefreshByInstance:
                                    var c3 = e.MessageObject as IContent;
                                    if (c3 != null)
                                        cacheRefresher.RefreshById(new Guid(DistributedCache.PageCacheRefresherId), c3.Id, user.LoginName, user.Password, instance.InstanceId, domain);
                                    break;
                                case MessageType.RemoveByInstance:
                                    var c4 = e.MessageObject as IContent;
                                    if (c4 != null)
                                        cacheRefresher.RemoveById(new Guid(DistributedCache.PageCacheRefresherId), c4.Id, user.LoginName, user.Password, instance.InstanceId, domain);
                                    break;
                            }
                        }
                    }
                }
            }

        }
    }

    [WebServiceBinding(
        Name = "CacheRefresherSoap",
        Namespace = "http://umbraco.org/webservices/")]
    internal class ServerSyncWebServiceClient : SoapHttpClientProtocol
    {
        public ServerSyncWebServiceClient(string domain)
        {
            Url = "http://" + domain.Trim() + "/umbraco/webservices/cacheRefresher.asmx";
        }

        [SoapDocumentMethod("http://umbraco.org/webservices/RefreshAll",
            RequestNamespace = "http://umbraco.org/webservices/",
            ResponseNamespace = "http://umbraco.org/webservices/",
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RefreshAll(Guid uniqueIdentifier, string Login, string Password, string instanceId, string domain)
        {
            //pass in instance id cookie so we get to the right server
            var cookieContainer = new System.Net.CookieContainer();
            cookieContainer.Add(new Uri("http://" + domain + "/"), new System.Net.Cookie("ARRAffinity", instanceId));
            this.CookieContainer = cookieContainer;
            //CookieContainer.Add(new System.Net.Cookie("ARRAffinity", instanceId));
            BeginInvoke("RefreshAll", new object[] { uniqueIdentifier, Login, Password }, null, null);
        }



        [SoapDocumentMethod("http://umbraco.org/webservices/RefreshById",
            RequestNamespace = "http://umbraco.org/webservices/",
            ResponseNamespace = "http://umbraco.org/webservices/",
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RefreshById(Guid uniqueIdentifier, int Id, string Login, string Password, string instanceId, string domain)
        {
            //pass in instance id cookie so we get to the right server
            var cookieContainer = new System.Net.CookieContainer();
            cookieContainer.Add(new Uri("http://" + domain + "/"), new System.Net.Cookie("ARRAffinity", instanceId));
            this.CookieContainer = cookieContainer;
            //CookieContainer.Add(new System.Net.Cookie("ARRAffinity", instanceId));
            BeginInvoke("RefreshById", new object[] { uniqueIdentifier, Id, Login, Password }, null, null);
        }



        [SoapDocumentMethod("http://umbraco.org/webservices/RemoveById",
            RequestNamespace = "http://umbraco.org/webservices/",
            ResponseNamespace = "http://umbraco.org/webservices/",
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveById(Guid uniqueIdentifier, int Id, string Login, string Password, string instanceId, string domain)
        {
            //pass in instance id cookie so we get to the right server
            var cookieContainer = new System.Net.CookieContainer();
            cookieContainer.Add(new Uri("http://" + domain + "/"), new System.Net.Cookie("ARRAffinity", instanceId));
            this.CookieContainer = cookieContainer;
            //CookieContainer.Add(new System.Net.Cookie("ARRAffinity", instanceId));
            BeginInvoke("RemoveById", new object[] { uniqueIdentifier, Id, Login, Password }, null, null);
        }
    }
}