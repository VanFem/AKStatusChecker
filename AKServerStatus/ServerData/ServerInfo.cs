using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKServerStatus.ServerData
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Ping { get; set; }
        public string LastStatusChange { get; set; }
        public bool AlertOnChange { get; set; }

        public bool IsOffline
        {
            get { return Ping == "OFFLINE"; }
        }
    }
}
