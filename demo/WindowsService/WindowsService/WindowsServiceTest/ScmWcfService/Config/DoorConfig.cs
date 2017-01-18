using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brilliantech.Framwork.Utils.ConfigUtil;
using ScmWcfService.Model.Enum;

namespace ScmWcfService.Config
{
    public class DoorConfig
    {
        private static ConfigUtil config;

        static DoorConfig()
        {
            try
            {
                config = new ConfigUtil("DOOR", "Ini/door.ini");
                Enabled = bool.Parse(config.Get("Enabled"));
                Type = (DoorType)int.Parse(config.Get("Type"));
                SN = int.Parse(config.Get("SN"));
                IP = config.Get("IP");
                Port = int.Parse(config.Get("Port"));
                DoorNo = int.Parse(config.Get("DoorNo"));

                User = config.Get("User");
                Password = config.Get("Password");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool Enabled { get; set; }
        public static DoorType Type { get; set; }
        public static int SN { get; set; }
        public static string IP { get; set; }
        public static int Port { get; set; }
        public static int DoorNo { get; set; }


        public static string User { get; set; }
        public static string Password { get; set; }
    }
}