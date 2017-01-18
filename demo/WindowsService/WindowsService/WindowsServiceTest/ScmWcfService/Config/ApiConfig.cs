using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brilliantech.Framwork.Utils.ConfigUtil;

namespace ScmWcfService.Config
{
    public class ApiConfig
    {
        private static ConfigUtil config;
        private static string host;
        private static string port;
        private static string token;

        static ApiConfig()
        {
            try
            {
                config = new ConfigUtil("API", "Ini/api.ini");

                host = config.Get("Host");
                ApiUri = config.Get("ApiUri");
                BaseUri = host + ApiUri;
                token = config.Get("Token");
                AutoToken = bool.Parse(config.Get("AutoToken"));
                LoginAction = config.Get("LoginAction");
                GetOrderCarByNrAction = config.Get("GetOrderCarByNrAction");
                GetOrderBoxByNrAction = config.Get("GetOrderBoxByNrAction");
                GetOrderBoxByNrsAction = config.Get("GetOrderBoxByNrsAction");
                CreatePickByCarAction = config.Get("CreatePickByCarAction");
                MoveStorageByCarAction = config.Get("MoveStorageByCarAction");
                GetPickItemByCarNrAction = config.Get("GetPickItemByCarNrAction");
                WeightOrderBoxAction = config.Get("WeightOrderBoxAction");
                BindBoxAndLedAction = config.Get("BindBoxAndLedAction");
                CreateOrderByLedAction = config.Get("CreateOrderByLedAction");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
      
        public static string Host
        {
            get { return host; }
            set
            {
                host = value;
                BaseUri =  host + ApiUri;
                config.Set("Host", value);
                config.Save();
            }
        }

        public static string Token
        {
            get { return token; }
            set
            {
                token = value; 
                config.Set("Token", value);
                config.Save();
            }
        }
        
        public static string ApiUri { get; set; }
        public static string BaseUri { get; set; }
        public static string LoginAction { get; set; }
        public static bool AutoToken { get; set; }
        public static string GetOrderCarByNrAction { get; set; }
        public static string GetOrderBoxByNrAction { get; set; }
        public static string GetOrderBoxByNrsAction { get; set; }
        public static string CreatePickByCarAction { get; set; }
        public static string MoveStorageByCarAction { get; set; }
        public static string GetPickItemByCarNrAction { get; set; }
        public static string WeightOrderBoxAction { get; set; }
        public static string BindBoxAndLedAction { get; set; }
        public static string CreateOrderByLedAction { get; set; }
    }
}