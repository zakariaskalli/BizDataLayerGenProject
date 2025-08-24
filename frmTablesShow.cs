using BizDataLayerGen.DataAccessLayer;
using BizDataLayerGen.GeneralClasses;
using Guna.UI2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using GymDB_DataLayer;
//using GymDB_BusinessLayer;

namespace BizDataLayerGen
{
    public partial class frmTablesShow : Form
    {

        public frmTablesShow()
        {
            InitializeComponent();

        }

        string[] AllTables = clsGeneralWithData.GetAllTablesByDBName(clsGlobal.DataBaseName);

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chBAllTables.Checked == true)
            {
                //All check is true

                for (int i = 0; i < LBTables.Items.Count; i++)
                {
                    LBTables.SetItemChecked(i, true); // Select each item in the ListBox
                }
                
                //LBTables.Enabled = false;

            }
            else
            {

                //LBTables.Enabled = true;

                for (int i = 0; i < LBTables.Items.Count; i++)
                {
                    LBTables.SetItemChecked(i, false); // Select each item in the ListBox
                }


            }

        }

        private void frmTablesShow_Load(object sender, EventArgs e)
        {
            this.Region = System.Drawing.Region.FromHrgn(clsGlobal.CreateRoundRectRgn(0, 0, Width, Height, 35, 35));

            //LBTables.Items.Clear();

            //LBTables.Items.AddRange(AllTables);

            LBTables.Items.Clear();
            LBTables.Items.AddRange(AllTables);

            chBAllTables.Checked = true;

            for (int i = 0; i < LBTables.Items.Count; i++)
            {
                LBTables.SetItemChecked(i, true); // Select each item in the ListBox
            }

            //LBTables.Enabled = false;


        }

        private void cbTablesName_DropDown(object sender, EventArgs e)
        {
            //guna2CheckBox1.Visible = true;

        }


        /*
                public static void ShowDataTableContents(DataTable dt)
        {
            if (dt == null)
            {
                MessageBox.Show("DataTable is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use StringBuilder for efficient string concatenation
            var result = new StringBuilder();
            result.AppendLine("DataTable Contents:");
            result.AppendLine(); // Optional: add a blank line after the title

            // Loop through each row and column to build the output string
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    result.AppendFormat("{0}: {1}\t", column.ColumnName, row[column]);
                }
                result.AppendLine(); // End the current row
                result.AppendLine(); // Extra blank line between rows
            }

            // Display the results in a MessageBox
            MessageBox.Show(result.ToString(), "DataTable Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

         
         */

        private void btnGenerate_Click(object sender, EventArgs e)
        {

            bool SelectItem = false;

            for (int i = 0; i < LBTables.Items.Count; i++)
            {
                if (LBTables.GetItemChecked(i))
                {
                    SelectItem = true;
                    break;
                }
            }


            if ((chBAllTables.Checked == false && !SelectItem))
            {
                MessageBox.Show("Please Select An Tables", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string[] NameTables;

            if (chBAllTables.Checked == true)
            {
                NameTables = AllTables;
            }
            else
            {
                int checkedCount = 0;
                for (int i = 0; i < LBTables.Items.Count; i++)
                {
                    if (LBTables.GetItemChecked(i))
                    {
                        checkedCount++;
                    }
                }



                // Create the array with the exact size
                NameTables = new string[checkedCount];

                // Populate the array with the checked items
                int index = 0;
                for (int i = 0; i < LBTables.Items.Count; i++)
                {
                    if (LBTables.GetItemChecked(i))
                    {
                        NameTables[index++] = LBTables.Items[i].ToString(); // Add checked item to the array
                    }
                }
            }


            // Test NameTables

            // the CreatebyUseName in People in GymDB is Not Nullable

            // Adding Record

            /*
            MessageBox.Show(clsPeopleData.AddNewPeople(" Zamil Moha     ", "  Zomala  ", "nan", "Malki", "Arzaz@gmail.com",
                                   "   064545125455  ", DateTime.Now, true, "Fes", 123, DateTime.Now, DateTime.Now,
                                   null).ToString());
            */


            /*
            MessageBox.Show(clsUsersData.AddNewUsers(74, "  Zakaria Ziko  ", "1212 ", DateTime.Now, null, true).ToString());
            */




            //GetPeopleInfoByID


            /*
            int PersonID = 56;
            string FirstName = "";
            string? SecondName = "";
            string? ThirdName = "";
            string LastName = "";
            string? Email = "";
            string? Phone = "";
            DateTime? DateOfBirth = DateTime.Now;
            bool Gender = false;
            string? Address = "";
            int CityID = -1;
            DateTime CreatedTime = DateTime.Now;
            DateTime LastUpdate = DateTime.Now; // Corrected variable name
            string? ProfilePicture = "";
            
           clsPeopleData.GetPeopleInfoByID(PersonID, ref FirstName, ref SecondName, ref ThirdName,
               ref LastName, ref Email, ref Phone, ref DateOfBirth, ref Gender, ref Address, ref CityID,
               ref CreatedTime, ref LastUpdate, ref ProfilePicture); // Removed extra paramet
            
            


            // GetAllPeople


            /*
            DataTable dt = clsPeopleData.GetAllPeople();

            // تأكد من أن الجدول يحتوي على بيانات
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data available.");
                return;
            }

            StringBuilder sb = new StringBuilder();

            // إضافة أسماء الأعمدة
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append(column.ColumnName).Append("\t");
            }
            sb.AppendLine();

            // إضافة الصفوف
            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    sb.Append(item.ToString()).Append("\t");
                }
                sb.AppendLine();
            }

            // عرض البيانات في رسالة
            MessageBox.Show(sb.ToString(), "People Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

            */




            // First Mythologic to read Last Error from JsonFile

            /*
            
            // Path to the JSON file provided by the user
            string userProvidedPath = "C:\\Programation Level 2\\BizDataLayerGen\\TestCodeGenerator\\GymDB_DataAccess\\ErrorHandler\\JsonFile\\ErrorHandling_JsonFile.json"; 

            
            try
            {
                // Read the JSON file
                string jsonContent = File.ReadAllText(userProvidedPath);

                // Deserialize the JSON content into a list of ErrorLog objects
                List<Log> errorLogs = JsonConvert.DeserializeObject<List<Log>>(jsonContent);

                // Check if there are any errors in the file
                if (errorLogs == null || errorLogs.Count == 0)
                {
                    MessageBox.Show("No errors found in the file.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Replace the problematic line with the following code
                Log lastError = errorLogs.Last(); // Retrieves the last element

                // Prepare the message to display
                string errorMessage = $"Error Message: {lastError.ErrorMessage}\n" +
                                      $"Severity: {lastError.Severity}\n" +
                                      $"Additional Info: {lastError.AdditionalInfo}\n" +
                                      $"Stack Trace: {lastError.StackTrace}";

                // Display the error in a MessageBox
                MessageBox.Show(errorMessage, "Last Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"The file was not found at the provided path: {userProvidedPath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (JsonException)
            {
                MessageBox.Show("The file does not contain valid JSON data.", "Invalid JSON", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            */

            // Second Mythologic to read Last Error from JsonFile

            /*

           // Path to the JSON file provided by the user
           string userProvidedPath = "C:\\Programation Level 2\\BizDataLayerGen\\TestCodeGenerator\\GymDB_DataAccess\\ErrorHandler\\JsonFile\\ErrorHandling_JsonFile.json"; 


           try
           {
               // Open the file for reading
               using (StreamReader file = File.OpenText(userProvidedPath))
               using (JsonTextReader reader = new JsonTextReader(file))
               {
                   // Parse the JSON file as a JArray
                   JArray errorsArray = JArray.Load(reader);

                   // Check if the JSON array has any errors
                   if (errorsArray.Count == 0)
                   {
                       MessageBox.Show("No errors found in the file.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       return;
                   }

                   // Get the last error as a JObject
                   JObject lastError = (JObject)errorsArray.Last;

                   // Extract error details
                   string errorMessage = lastError["ErrorMessage"]?.ToString();
                   string stackTrace = lastError["StackTrace"]?.ToString();
                   string severity = lastError["Severity"]?.ToString();
                   string additionalInfo = lastError["AdditionalInfo"]?.ToString();

                   // Prepare the message to display
                   string displayMessage = $"Error Message: {errorMessage}\n" +
                                           $"Severity: {severity}\n" +
                                           $"Additional Info: {additionalInfo}\n" +
                                           $"Stack Trace: {stackTrace}";

                   // Display the error in a MessageBox
                   MessageBox.Show(displayMessage, "Last Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
           }
           catch (FileNotFoundException)
           {
               MessageBox.Show($"The file was not found at the provided path: {userProvidedPath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           }
           catch (JsonException)
           {
               MessageBox.Show("The file does not contain valid JSON data.", "Invalid JSON", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           }
           catch (Exception ex)
           {
               MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           }

           */



            // Update Person

            /*
            MessageBox.Show(clsPeopleData.UpdatePeopleByID(74, "Zakaria", "zaki", "nan", "Malki", "Arzaz@gmail.com",
                                   "  06454556 ", DateTime.Now, true, "Fes hakma l3alam", 125, DateTime.Now, DateTime.Now,
                                   null).ToString());
            */

            // Update Payment

            //MessageBox.Show(clsPaymentsData.UpdatePaymentsByID(3,1, 3, DateTime.Now, true, 2).ToString());




            // Tests For Delete 


            //MessageBox.Show(clsPeopleData.DeletePeople(28).ToString());
            //MessageBox.Show(clsPeople.DeletePeople(30).ToString());



            //MessageBox.Show(clsPayments.DeletePayments(7).ToString());


            // Test SearchData

            /*
            
            DataTable dt = clsUsers.SearchData(clsUsers.UsersColumn.CreatedDate, "-");
            ShowDataTableContents(dt);

            
            //string result = "DataTable Contents:\n";
            //foreach (DataRow row in dt.Rows)
            //{
                //foreach (DataColumn column in dt.Columns)
                //{
                    //result += $"{column.ColumnName}: {row[column]} \t";
                //}
                //result += "\n";
            //}

            //MessageBox.Show(result, "DataTable Data");
            


            */





            
            bool FkOfAll = rbJustThis.Checked == false && rbAll.Checked == true;

            bool AddingStaticMethods = rbAddingStaticMethodsYes.Checked == true && rbAddingStaticMethodsNo.Checked == false;

            bool AutoExcuteSP = switchAutoExcuteSP.Checked;

            bool UseDTO = switchUsingDTO.Checked;


            if (clsAddDataAccessAndBusinessLayers.AddDataAndBusinessLayers(NameTables, FkOfAll, AddingStaticMethods, AutoExcuteSP, UseDTO) == clsGlobal.enTypeRaisons.enPerfect)
                MessageBox.Show($"Created Success, In: {clsGlobal.TimeInMillisecond}ms", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            



        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbAddingStaticMethodsNo_CheckedChanged(object sender, EventArgs e)
        {
            // In Button Ok Less rbNo And Cancel Checked RbYes
            if (rbAddingStaticMethodsNo.Checked == true )
            {
                if (MessageBox.Show(@"
                If You Select This You Didn't Have All This     Methods in    Code:
                        1) Static Adding New Row
                        2) Static Update Row
                        3) Static Find
                        4) Get All Rows
                        5) Delete Row
                        6) Search Data And Return DataTable
                Do you Want to Let?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    rbAddingStaticMethodsNo.Checked = false;
                    rbAddingStaticMethodsYes.Checked = true;
                }

            }

        }

        private void LBTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Assume all items are selected initially
            bool IsSelectedAll = true;

            // Create lists to store checked and unchecked items
            List<string> checkedItems = new List<string>();
            List<string> uncheckedItems = new List<string>();

            for (int i = 0; i < LBTables.Items.Count; i++)
            {
                // Check if the item is selected (adjust the property based on your ListBox or item type)
                if (LBTables.GetItemChecked(i)) // Assuming you are using a CheckedListBox
                {
                    // Add to checked items list
                    checkedItems.Add(LBTables.Items[i].ToString());
                }
                else
                {
                    // Add to unchecked items list
                    uncheckedItems.Add(LBTables.Items[i].ToString());
                    IsSelectedAll = false; // If any item is not selected, set to false
                }
            }

            // Update the checkbox based on whether all items are selected
            chBAllTables.Checked = IsSelectedAll;

            // Display the state of each item
            for (int i = 0; i < LBTables.Items.Count; i++)
            {
                if (checkedItems.Contains(LBTables.Items[i].ToString()))
                {
                    LBTables.SetItemChecked(i, true);
                }
                else if (uncheckedItems.Contains(LBTables.Items[i].ToString()))
                {
                    LBTables.SetItemChecked(i, false);

                }


            }


        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
