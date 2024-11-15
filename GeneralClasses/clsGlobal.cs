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
            enDBName = 1,
            enReloadColumnsName = 2,
            enReloadDataTypes = 3,
            enCreateProjectFolders = 4,
            enCreateDataAccessSettingsClassFile = 5,
            enTableDontHavePK = 6,
            enPerfect = 7
        }
        public static enTypeRaisons MyEnumProperty { get; set; }


        public static string DataBaseName = "";

        public static string PathFilesToGenerate = "";

        public static string UserId = "";
        public static string Password = "";

        public static string dataAccessLayerPath = "";
        public static string businessLayerPath = "";

    }
}
