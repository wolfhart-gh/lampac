using Shared.Models.Base;
using System;

namespace RezkaFree
{
    public class RezkaFreeSettings : BaseSettings, ICloneable
    {
        public RezkaFreeSettings(string plugin, string host, bool streamproxy = false)
        {
            enable = true;
            this.plugin = plugin;
            this.streamproxy = streamproxy;

            if (host != null)
                this.host = host.StartsWith("http") ? host : Decrypt(host);
        }


        public bool premium { get; set; }

        public bool reserve { get; set; }

        public string uacdn { get; set; }

        public bool forceua { get; set; }

        public bool? ajax { get; set; }


        public RezkaFreeSettings Clone()
        {
            return (RezkaFreeSettings)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
    }
}
