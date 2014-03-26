using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace rbcUmbracoOnAzure
{
    [TableName("_AzureWebsitesInstance")]
    [PrimaryKey("id", autoIncrement = true)]
    public class AzureWebsitesInstanceDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public string WebsiteName { get; set; }

        public string InstanceId { get; set; }

        public string ComputerName { get; set; }

        public DateTime RegisteredDate { get; set; }

        public DateTime LastNotifiedDate { get; set; }

        public bool IsActive { get; set; }
    }
}