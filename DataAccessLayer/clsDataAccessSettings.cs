using System;

namespace BizDataLayerGen.DataAccessLayer
{
    static class clsDataAccessSettings
    {
        public static string UserId = "";
        public static string Password = "";

        public static string ConnectionString = $"Server=.;User Id={UserId};Password={Password};";

        //public static string ConnectionString = "Server=.;Database=DVLD;User Id=sa;Password=sa123456;";


    }
}