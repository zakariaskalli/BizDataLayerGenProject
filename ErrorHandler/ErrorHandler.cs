using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen
{

    public class HandleErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public string TableName { get; }
        public string MethodName { get; }

        public HandleErrorEventArgs(Exception exception, string tableName, string methodName)
        {
            Exception = exception;
            TableName = tableName ?? "Unknown";
            MethodName = methodName ?? "Unknown";
        }
    }

    public static class ErrorHandler
    {
        public static event EventHandler<HandleErrorEventArgs> OnErrorOccurred;

        public static void RaiseError(Exception ex, string tableName, string methodName)
        {
            var args = new HandleErrorEventArgs(ex, tableName, methodName);
            OnErrorOccurred?.Invoke(null, args);
        }

        public static void RaiseErrorWithArgs(HandleErrorEventArgs e)
        {
            OnErrorOccurred?.Invoke(null, e);

        }

        
        public static void ShowErrorMessage(object sender, HandleErrorEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(
                $"Error Occurred:\n\n" +
                $"Table: {e.TableName}\n" +
                $"Method: {e.MethodName}\n" +
                $"Exception: {e.Exception.Message}",
                "Error",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error
            );
        }
    }


}
