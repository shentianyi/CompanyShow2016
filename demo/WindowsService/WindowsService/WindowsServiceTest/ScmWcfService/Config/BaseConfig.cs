
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brilliantech.Framwork.Utils.ConfigUtil;

namespace ScmWcfService.Config
{
    public class BaseConfig
    {
        private static ConfigUtil config;

        static BaseConfig()
        {
            try
            {
                config = new ConfigUtil("Base", "Ini/base.ini");
                ForceWeightPass = bool.Parse(config.Get("ForceWeightPass"));
                PlayWeightSound = bool.Parse(config.Get("PlayWeightSound"));
                PlayPickPositionVoice = bool.Parse(config.Get("PlayPickPositionVoice"));
                ImageExtends = config.Get("ImageExtends").Split(',').ToList();
                ForceNetWeight = bool.Parse(config.Get("ForceNetWeight"));
                ShowOpenDoor = bool.Parse(config.Get("ShowOpenDoor"));
                AutoWeightConfirm = bool.Parse(config.Get("AutoWeightConfirm"));
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool ForceWeightPass { get; set; }
        public static bool PlayWeightSound { get; set; }
        public static bool PlayPickPositionVoice { get; set; }
        public static List<string> ImageExtends { get; set; }
        public static bool ForceNetWeight { get; set; }
        public static bool ShowOpenDoor { get; set; }
        public static bool AutoWeightConfirm { get; set; }
    }
}