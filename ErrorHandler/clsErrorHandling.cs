using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDataLayerGen.DataAccessLayer
{
    public class clsErrorHandling
    {
        private static void CreateTableLog()
        {

            // SQL query to check if the table already exists
            string checkTableQuery = @$"
        USE {clsGlobal.DataBaseName};
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ErrorLog')
        BEGIN
            CREATE TABLE ErrorLog (
                ErrorID INT IDENTITY(1,1) PRIMARY KEY,         -- Unique ID for each log entry
                ErrorMessage NVARCHAR(MAX) NOT NULL,          -- Error message
                StackTrace NVARCHAR(MAX),                      -- Stack trace of the error (optional)
                Timestamp DATETIME DEFAULT GETDATE(),          -- Time when the error occurred
                Severity NVARCHAR(50),                         -- Severity level (e.g., Low, Medium, High)
                AdditionalInfo NVARCHAR(MAX)                   -- Optional additional info about the error
            );
        END;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(checkTableQuery, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();  // Execute the command to check and create the table
                    }
                }
            }
            catch
            {

            }

        }

        private static void GenerateErrorLogClassesFile(string filePath)
        {
            // Define the content of the Log and ErrorLogHandler classes
            string Code = @$"
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using {clsGlobal.DataBaseName}_DataAccess;
using Newtonsoft.Json;

// Log model to store error details
public class Log
{{
    public string ErrorMessage {{ get; set; }}
    public string StackTrace {{ get; set; }}
    public string Severity {{ get; set; }} // Examples: Low, Medium, High
    public string AdditionalInfo {{ get; set; }}

    public Log(string errorMessage, string stackTrace = null, string severity = ""Medium"", string additionalInfo = null)
    {{
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
        Severity = severity;
        AdditionalInfo = additionalInfo;
    }}
}}

// Unified interface for all subscribers
public interface IErrorSubscriber
{{
    void HandleError(Log log); // Defines how each subscriber handles the error
}}

// Subscriber for logging errors to the database
public class DatabaseErrorLogger : IErrorSubscriber
{{
    public void HandleError(Log log)
    {{
        string query = @""INSERT INTO ErrorLog (ErrorMessage, StackTrace, Severity, AdditionalInfo)
                VALUES (@ErrorMessage, @StackTrace, @Severity, @AdditionalInfo);"";

        try
        {{
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {{
                using (SqlCommand command = new SqlCommand(query, connection))
                {{
                    // Add parameters
                    command.Parameters.AddWithValue(""@ErrorMessage"", log.ErrorMessage ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue(""@StackTrace"", log.StackTrace ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue(""@Severity"", log.Severity ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue(""@AdditionalInfo"", log.AdditionalInfo ?? (object)DBNull.Value);

                    // Open connection and execute the query
                    connection.Open();
                    command.ExecuteNonQuery();
                }}
            }}
        }}
        catch (Exception ex)
        {{
            // Handle exceptions if needed
        }}
    }}
}}

// Subscriber for logging errors to a JSON file
public class JsonErrorLogger : IErrorSubscriber
{{
    public void HandleError(Log log1) // Renamed 'log' to 'log1'
    {{

        // Example path (replace with your desired path)
        /*
        string userProvidedPath = @""C:\Programation Level 2\BizDataLayerGen\TestCodeGenerator\GymDB_DataAccess\ErrorHandler\JsonFile\"";
        */

        // Enter the path where you want to save the JSON file
        string userProvidedPath = ???; // Replace with the actual path

        // Append the file name to the path
        userProvidedPath += ""ErrorHandling_JsonFile.json"";

        // Validate the path
        if (string.IsNullOrWhiteSpace(userProvidedPath) || !Directory.Exists(Path.GetDirectoryName(userProvidedPath)))
        {{
            throw new DirectoryNotFoundException(""Invalid directory path provided."");
        }}

        // Ensure the file exists or create an empty one
        if (!File.Exists(userProvidedPath))
        {{
            File.WriteAllText(userProvidedPath, ""[]"");
        }}

        // Define the method to save the new log
        void SaveNewLog(Log log2) // Renamed 'log' to 'log2'
        {{
            try
            {{
                // Read the existing JSON content (if any)
                var existingLogs = JsonConvert.DeserializeObject<List<Log>>(File.ReadAllText(userProvidedPath)) ?? new List<Log>();

                // Add the new log entry to the list
                existingLogs.Add(log2);

                // Serialize the updated log list back to JSON
                string updatedJsonContent = JsonConvert.SerializeObject(existingLogs, Formatting.Indented);

                // Write the updated JSON content to the file
                File.WriteAllText(userProvidedPath, updatedJsonContent);
            }}
            catch
            {{
                // Handle exceptions if needed
            }}
        }}

        // Pass the new log to the method
        SaveNewLog(log1); // Pass 'log1' to 'SaveNewLog'



        // Save All Version For errors in Json File

        /*
        if (string.IsNullOrWhiteSpace(userProvidedPath) || !Directory.Exists(Path.GetDirectoryName(userProvidedPath)))
        {{
            throw new DirectoryNotFoundException(""Invalid directory path provided."");
        }}

        // Ensure the file exists or create an empty list as initial content
        if (!File.Exists(userProvidedPath))
        {{
            File.WriteAllText(userProvidedPath, ""[]"");
        }}

        try
        {{
            // Read the existing JSON content (if any)
            var existingLogs = JsonConvert.DeserializeObject<List<Log>>(File.ReadAllText(userProvidedPath)) ?? new List<Log>();

            // Add the new log entry to the list
            existingLogs.Add(log);

            // Serialize the updated log list back to JSON
            string updatedJsonContent = JsonConvert.SerializeObject(existingLogs, Formatting.Indented);

            // Write the updated JSON content to the file
            File.WriteAllText(userProvidedPath, updatedJsonContent);

        }}
        catch
        {{
        }}
        */
    }}
}}

// Publisher that distributes errors to all registered subscribers
public class ErrorPublisher
{{
    private readonly List<IErrorSubscriber> subscribers = new List<IErrorSubscriber>();

    // Register a new subscriber
    public void Subscribe(IErrorSubscriber subscriber)
    {{
        subscribers.Add(subscriber);
    }}

    // Unsubscribe a subscriber
    public void Unsubscribe(IErrorSubscriber subscriber)
    {{
        subscribers.Remove(subscriber);
    }}

    // Notify all subscribers about an error
    public void Notify(Log log)
    {{
        foreach (var subscriber in subscribers)
        {{
            subscriber.HandleError(log); // Each subscriber handles the error as defined in its implementation
        }}
    }}
}}

// Centralized error handler class
public static class ErrorHandler
{{
    private static readonly ErrorPublisher errorPublisher = new ErrorPublisher();

    // Registering the subscribers
    static ErrorHandler()
    {{
        // Add all desired error logging methods (subscribers) here
        errorPublisher.Subscribe(new DatabaseErrorLogger()); // Logs error to the database
        errorPublisher.Subscribe(new JsonErrorLogger());     // Logs error to a JSON file
    }}

    // Simplified interface for error distribution
    public static void HandleError(Log log)
    {{
        errorPublisher.Notify(log); // Notify all registered subscribers about the error
    }}

    public static void HandleException(Exception ex, string methodName, string additionalInfo = null)
    {{
        // Determine severity based on exception type
        string severity = ex is SqlException ? ""High"" : ""Medium"";

        // Create a log entry
        var log = new Log(
            errorMessage: $""An error occurred in {{methodName}}."",
            stackTrace: ex.StackTrace,
            severity: severity,
            additionalInfo: $""{{additionalInfo}}, Error: {{ex.Message}}""
        );

        // Send the log to the error handler
        ErrorHandler.HandleError(log);
    }}
}}";

            // Write the content to the file
            try
            {
                // Create or overwrite the file
                System.IO.File.WriteAllText(filePath, Code);
            }
            catch
            {
                // Handle exceptions if needed
            }
        }
        private static void CreateJsonFile(string FilePath)
        {


            // Combine to create the folder path
            string errorHandlerJsonFolderPath = Path.Combine(FilePath, "JsonFile");

            // Define the path for the JSON file
            string jsonFilePath = Path.Combine(errorHandlerJsonFolderPath, $"ErrorHandling_JsonFile.json");

            // Update the global JSON file path
            clsGlobal.JsonFilePath = $"ErrorHandling_JsonFile.json";

            // Check if the folder exists, create it if it doesn't
            if (!Directory.Exists(errorHandlerJsonFolderPath))
            {
                Directory.CreateDirectory(errorHandlerJsonFolderPath);
            }

            // Check if the JSON file exists
            if (!File.Exists(jsonFilePath))
            {
                // Create an empty file (no data added)
                File.Create(jsonFilePath).Dispose();  // .Dispose() is used to release the file handle
            }

        }


        public static bool CreatingForErrorHanding(string filePath)
        {
            try
            {
                // Step 1: Create the table log
                CreateTableLog();



                // Step 2: Prepare the error handler folder path
                string errorHandlerFolderPath = Path.Combine(filePath, "ErrorHandler");

                if (!Directory.Exists(errorHandlerFolderPath))
                {
                    Directory.CreateDirectory(errorHandlerFolderPath);
                }

                // Step 3: Create the JSON file
                CreateJsonFile(errorHandlerFolderPath);

                // Step 4: Generate the class file for error logging
                string generateClassFileForErrorLog = Path.Combine(errorHandlerFolderPath, "clsErrorHandlingManager.cs");

                GenerateErrorLogClassesFile(generateClassFileForErrorLog);

                // Step 5: Double-check the folder exists (though unnecessary after creation)
                if (!Directory.Exists(errorHandlerFolderPath))
                {
                    Directory.CreateDirectory(errorHandlerFolderPath);
                }

                // If all steps succeed, return true
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
