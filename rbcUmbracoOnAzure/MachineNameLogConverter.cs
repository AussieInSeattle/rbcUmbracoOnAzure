using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace rbcUmbracoOnAzure
{
    public sealed class MachineNameLogConverter : log4net.Util.PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            writer.Write(Environment.MachineName);
        }
    }
}