using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Win32;
using System.Diagnostics;

namespace BizDataLayerGen.ConnectingWithRegister
{
    public class clsRegister
    {
        static public void AddDataToRegister(string UserId, string Password)
        {

            string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\BizDataLayerGen";

            string valueUserIdName = "UserId";
            string valueUserIdData = UserId;

            string valuePasswordName = "Password";
            string valuePasswordData = Password;

            try
            {
                Registry.SetValue(keyPath, valueUserIdName, valueUserIdData, RegistryValueKind.String);
                Registry.SetValue(keyPath, valuePasswordName, valuePasswordData, RegistryValueKind.String);

            }
            catch(Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
                return;
            }
        }

        static public void DeleteDataSignInRegister()
        {
            string keypath = @"SOFTWARE\BizDataLayerGen";
            string valueUserId = "UserId";
            string valuePasswordName = "Password";

            try
            {
                using (RegistryKey basekey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    using (RegistryKey key = basekey.OpenSubKey(keypath, true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue(valueUserId);
                            key.DeleteValue(valuePasswordName);

                        }
                        else
                            return;
                    }
                }
            }
            catch(Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);

                return;
            }
        }

        static public bool LoadUserIdAndPassword(ref string UserId, ref string Password)
        {
            bool IsAvailableData = false;

            // Using Windows Registry

            string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\BizDataLayerGen";

            string valueUserId = "UserId";

            string valuePasswordName = "Password";

            try
            {
                UserId = Registry.GetValue(keyPath, valueUserId, null) as string;
                Password = Registry.GetValue(keyPath, valuePasswordName, null) as string;

                if (UserId != null && Password != null)
                    IsAvailableData = true;
                else
                    IsAvailableData = false;

            }
            catch(Exception ex)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);
                var method = frame.GetMethod();
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                ErrorHandler.RaiseError(ex, className, methodName);
            }

            return IsAvailableData;
        }


    }
}
