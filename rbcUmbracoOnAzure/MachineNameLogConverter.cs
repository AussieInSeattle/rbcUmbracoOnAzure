using System;
using System.IO;

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