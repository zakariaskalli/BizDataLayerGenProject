using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen
{
    public class clsGlobal
    {
        public enum enTypeRaisons
        {
            enError = 1,
            enPerfect = 2
        }
        public static enTypeRaisons MyEnumProperty { get; set; }


        public static string DataBaseName = "";

        public static string PathFilesToGenerate = "";

        public static string UserId = "";
        public static string Password = "";

        public static string dataAccessLayerPath = "";
        public static string businessLayerPath = "";

        public static string TimeInMillisecond = "";
    }
}
