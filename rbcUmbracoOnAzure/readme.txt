
Please see http://our.umbraco.org/projects/backoffice-extensions/rbcumbracoonazure for latest information

Install Instructions:
---------------------

Setup 2 Azure Websites - one that you will scale (front-end) and one that you will not (back-office). You will need to place each site inside a separate "Web hosting plan" within Azure to prevent 
them from scaling together. You can specify a web hosting plan when creating the website.

On the Backoffice website only, add an App Config name/value of IsUmbracoBackOffice=true - the package uses this to determine the back-office website from the front-end website 
(no app config name/value set)

Modify your global.asax to inherit from RbcUmbracoOnAzure.UmbracoApplication - the package uses this to hook into the ApplicationEnd event.

On the Front-end website only add an App Config name/value of umbracoContentXMLUseLocalTemp=true (as per the umbraco load balancing instructions for shared file structure)

Install the package on both websites - it will add a single dll to the /bin folder and add a folder with a view under App_Plugins

That's it - you can now scale out your front-end website in Azure and look at the page at: /App_Plugins/rbcUmbracoOnAzure/CurrentInstances/ to see the current azure instances - 
this page also allows you to force the browser to each instance by setting your ARR cookie for you - this page simply pulls values from teh custom db table (called AzureWebsitesInstance)

To test your installation:
--------------------------
create a simple doc type with one RTE on it and deploy to root of your site - remember to copy the template (and any other "on disk files" to both websites)
In the template have it pull from umbraco via Umbraco.Field as well as from the Examine index, so you can test and ensure both are updating via the distributed cache call - it is also handy to put @Environment.MachineName in the template so you can tell one server from another
spin up multiple instances of front-end website and check the url mentioned in step 6 above to ensure they are "registering on app startup" - use the "Clear My Cookie" link to "fool" ARR into possibly sending you to another server - once you have multiple instances active:
go to the back-office server /umbraco and make a change to your one piece of content and publish it
go to your front-end website root (not /umbraco) and the change should also be seen - to access each box you can use the links in step 6 above to dynamically change your cookie

Other Notes:
------------

Please provide any feedback in the forums.

Source code will be on github.

You should never access /umbraco from your front-end website

The front-end website essentially uses the load balancing setup explained at our.umbraco.org/.../load-balancing using the "File Storage with SAN/...etc" method - the only other thing 
I did not mention above is implementation of log4net to work across multiple servers with shared storage

Even though you scale out to 5 instances, it seems Azure will not actually start those instances until traffic warrants it, so you may only see 2 instances in your database table or 
at the above debugging url

If your site uses Session state, then you'll need to solve the "multiple server" issue like you'd have to with any load balanced environment - 
storing Session State in App Fabric or SQL would both work - demo's/examples available via google :-)
