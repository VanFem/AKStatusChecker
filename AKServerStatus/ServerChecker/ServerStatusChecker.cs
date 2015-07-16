using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKServerStatus.ServerData;
using HtmlAgilityPack;

namespace AKServerStatus.ServerChecker
{
    public class ServerStatusChecker
    {
        public static string ServerUrl = "http://www.gameserverstatus.com/aura_kingdom_server_status";

        public static int numberResponse = 0;

        public static List<ServerInfo> GetServerStatus()
        {
            // Test data because I'm too lazy to interface

            //switch (numberResponse)
            //{
            //    case 0:
            //        numberResponse++;
            //        return new List<ServerInfo>
            //        {
            //            new ServerInfo()
            //            {
            //                LastStatusChange = "x",
            //                Name = "Server 1",
            //                Ping = "OFFLINE",
            //            },
            //            new ServerInfo()
            //            {
            //                LastStatusChange = "ax",
            //                Name = "Server 2",
            //                Ping = "OFFLINE",
            //            }
            //        };
            //    case 1:
            //        numberResponse++;
            //        return new List<ServerInfo>
            //        {
            //            new ServerInfo()
            //            {
            //                LastStatusChange = "x",
            //                Name = "Server 1",
            //                Ping = "10ms",
            //            },
            //            new ServerInfo()
            //            {
            //                LastStatusChange = "ax",
            //                Name = "Server 2",
            //                Ping = "OFFLINE",
            //            }
            //        };
            //    case 2:
            //        numberResponse++;
            //        return new List<ServerInfo>
            //        {
            //            new ServerInfo()
            //            {
            //                LastStatusChange = "x",
            //                Name = "Server 1",
            //                Ping = "OFFLINE",
            //            },
            //            new ServerInfo()
            //            {
            //                LastStatusChange = "ax",
            //                Name = "Server 2",
            //                Ping = "20ms",
            //            }
            //        };
            //}




            var web = new HtmlWeb();
            var document = web.Load(ServerUrl);


            var serverStatusNodes =
                document.DocumentNode.SelectNodes("//table[contains(concat(' ',@class,' '),'status-table')]//tr[.//td]");

            var serverList = new List<ServerInfo>();


            foreach (var node in serverStatusNodes)
            {
                var contentNodes = node.SelectNodes("td[contains(concat(' ',@class,' '),'status-cell')]");
                try
                {
                    var serverInfo = new ServerInfo
                    {
                        Name = contentNodes[0].InnerText,
                        Ping = contentNodes[1].InnerText,
                        LastStatusChange = contentNodes[2].InnerText,
                        AlertOnChange = false
                    };
                    serverList.Add(serverInfo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[Error] GetServerStatus: " + ex.Message);
                }
            }

            return serverList;
        }
    }
}
