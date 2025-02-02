using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen
{
    public class clsGlobal
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

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
        public static string JsonFilePath = "";

        public static string TimeInMillisecond = "";
    }
}
