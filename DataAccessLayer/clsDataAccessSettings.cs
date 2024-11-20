using System;
using System.Configuration;

namespace BizDataLayerGen.DataAccessLayer
{
    static class clsDataAccessSettings
    {
        //public static string UserId = "";
        //public static string Password = "";

        public static string ConnectionString = $"Server=.;User Id={clsGlobal.UserId};Password={clsGlobal.Password};";

        //static public string ConnectionString = ConfigurationManager.AppSettings["add"];


    }
}