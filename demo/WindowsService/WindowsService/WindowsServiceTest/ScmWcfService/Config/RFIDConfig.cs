using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brilliantech.Framwork.Utils.ConfigUtil;
using ScmWcfService.Model.Enum;
using JW.UHF;

namespace ScmWcfService.Config
{
    public class RFIDConfig
    {
        private static ConfigUtil config;
       
        static RFIDConfig()
        {
            try
            {
                config = new ConfigUtil("RFID", "Ini/rfid.ini");

               // USE_DLL = bool.Parse(config.Get("USE_DLL"));
                ReaderType = (RFIDReaderType)(int.Parse(config.Get("ReaderType")));
                RFIDCOM = config.Get("RFIDCOM");
                RFIDBaudRate = int.Parse(config.Get("RFIDBaudRate"));
                RFIDInterVal = int.Parse(config.Get("RFIDInterVal"));
                AntennaPort = config.Get("AntennaPort").Split(',').ToList();
                AntennaPower = int.Parse(config.Get("AntennaPower"));
                InventoryTime = int.Parse(config.Get("InventoryTime"));
                RegionListType = (RegionList)(int.Parse(config.Get("RegionListType")));
                SpeedModeType = (SpeedMode)(int.Parse(config.Get("SpeedModeType")));
                SearchModeType = (SearchMode)(int.Parse(config.Get("SearchModeType")));
                SessionTargetType = (SessionTarget)(int.Parse(config.Get("SessionTargetType")));
                SessionType = (Session)(int.Parse(config.Get("SessionType")));

                MessageRegex = config.Get("MessageRegex");
                LabelRegex = config.Get("LabelRegex");
                OrderCarLabelRegex = config.Get("OrderCarLabelRegex");
                OrderBoxLabelRegex = config.Get("OrderBoxLabelRegex");
                OutAutoLoadPick = bool.Parse(config.Get("OutAutoLoadPick"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static RFIDReaderType ReaderType { get; set; }
        public static string RFIDCOM { get; set; }
        public static int RFIDBaudRate { get; set; }
        public static int RFIDInterVal { get; set; }

        public static List<string> AntennaPort { get; set; }
        public static int AntennaPower { get; set; }
        public static int InventoryTime { get; set; }
        public static RegionList RegionListType { get; set; }
        public static SpeedMode SpeedModeType { get; set; }
        public static SearchMode SearchModeType { get; set; }
        public static SessionTarget SessionTargetType { get; set; }
        public static Session SessionType { get; set; }


        //public static bool USE_DLL { get; set; }
        public static string MessageRegex { get; set; }
        public static string LabelRegex { get; set; }
        public static string OrderCarLabelRegex { get; set; }
        public static string OrderBoxLabelRegex { get; set; }
        public static bool OutAutoLoadPick { get; set; }
    }
}