using BizDataLayerGen.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizDataLayerGen
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //
            if (clsGeneralWithData.TestDatabaseConnection())
            {
                Application.Run(new CodeGenratorForm());
            }
            else
            {
                Application.Run(new frmLogin());
            }


        }
    }
}
