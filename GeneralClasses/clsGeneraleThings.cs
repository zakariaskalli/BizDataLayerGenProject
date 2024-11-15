using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen
{
    public class clsGeneraleThings
    {
        public static bool IsValidDatabaseName(string dbName)
        {
            // تحقق أن اسم قاعدة البيانات يحتوي على أحرف آمنة فقط
            return System.Text.RegularExpressions.Regex.IsMatch(dbName, @"^[a-zA-Z0-9_]+$");
        }

        public static bool IsValidPath(string path)
        {
            // التحقق من أن المسار غير فارغ وأنه موجود
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

    }
}
