using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BizDataLayerGen
{
    public class clsGeneraleThings
    {
        public static bool IsValidDatabaseName(string dbName)
        {
            // check if the database name is not null or empty and contains only letters, numbers, and underscores
            return System.Text.RegularExpressions.Regex.IsMatch(dbName, @"^[a-zA-Z0-9_]+$");
        }


        public static bool HasInternetConnection()
        {
            try
            {
                using var ping = new Ping();
                var reply = ping.Send("8.8.8.8", 3000); // 3-second timeout
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public static string CleanDatabaseName(string dbName)
        {
            if (string.IsNullOrWhiteSpace(dbName)) return dbName;

            string cleaned = dbName.Trim();

            // Suffixes to strip, ordered longest-first so "Database" is checked before "DB"
            string[] suffixesToRemove = { "_DATABASE", "_DB", "DATABASE", "DB" };

            foreach (var suffix in suffixesToRemove)
            {
                if (cleaned.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Substring(0, cleaned.Length - suffix.Length);
                    break; 
                }
            }

            return cleaned.TrimEnd('_', '-', ' ');
        }
       
        
        public static bool IsValidPath(string path)
        {
            // التحقق من أن المسار غير فارغ وأنه موجود
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        static public string Singularize(string word)
        {

            if (string.IsNullOrEmpty(word))
                return word;
            return word.Singularize();

        }

        public static string Pluralize(string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;

           return word.Pluralize();
        }

        public static List<string> Singularize(List<string> words)
        {
            List<string> singularWords = new List<string>();
            foreach (var word in words)
            {
                singularWords.Add(Singularize(word));
            }
            return singularWords;

        }

        public static List<string> Pluralize(List<string> words)
        {
            List<string> PluralWords = new List<string>();
            foreach (var word in words)
            {
                PluralWords.Add(Pluralize(word));
            }
            return PluralWords;
        }

        
    }
}
