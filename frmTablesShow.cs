using BizDataLayerGen.AI;
using BizDataLayerGen.DataAccessLayer;
using BizDataLayerGen.DocumentationGenerator;
using BizDataLayerGen.GeneralClasses;
using BizDataLayerGen.Project_Structure_Generation__Principale_.Solution;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Suite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

            //foreach (var TableName in AllTables)
            //{
            //    dvTables.Rows.Add(true,TableName,true);
            //}
            

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

        private static string GetDotnetVersionNumber(int cbSelectedIndex)
        {
            switch (cbSelectedIndex)
            {
                case 0:
                    return "8.0";
                case 1:
                    return "9.0";
                case 2:
                    return "10.0";
                default:
                    return "8.0"; // Default to .NET 8.0 if not specified
            }
        }


        
        private async  void btnGenerate_Click(object sender, EventArgs e)
        {
            clsGlobal.DotnetCoreVersion = GetDotnetVersionNumber(cbDotNetVersion.SelectedIndex);

            if (!clsGeneraleThings.HasInternetConnection())
            {
               MessageBox.Show("No internet connection detected.Run this command when you have an internet connection Available to Restore the required packages. (dotnet restore)", "Internet Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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


            clsGlobal.AICodeDocsEnabled = ckAiCodeDocs.Checked;

            
            bool FkOfAll = rbJustThis.Checked == false && rbAll.Checked == true;

            bool AddingStaticMethods = rbAddingStaticMethodsYes.Checked == true && rbAddingStaticMethodsNo.Checked == false;

            bool AutoExcuteSP = switchAutoExcuteSP.Checked;

            bool UseDTO = switchUsingDTO.Checked;

            bool AddAPI = ckGenerateAPI.Checked;

            if (rbAsynchronous.Checked)
            {
                clsGlobal.ExuctionMethod = clsGlobal.enExuctionMethods.enAsynchronous;
            }
            //else if (rbSynchronous.Checked)
            //{
            //    clsGlobal.ExuctionMethod = clsGlobal.enExuctionMethods.enSynchronous;
            //}
            else
            {
                clsGlobal.ExuctionMethod = clsGlobal.enExuctionMethods.enBoth;
            }


            if (rbPaggination.Checked)
            {
                clsGlobal.EnablePagination = true;
            }
            else
                { clsGlobal.EnablePagination = false; }

            Stopwatch stopwatch1 = Stopwatch.StartNew();

            await GenerateProjectStructureAsync(AddAPI, UseDTO);


            if (await clsAddLayersCode.AddLayers(NameTables, FkOfAll, AddingStaticMethods, AutoExcuteSP, UseDTO, AddAPI, clsGlobal.EnablePagination, clsGlobal.ExuctionMethod) == clsGlobal.enTypeRaisons.enPerfect)
            {
                stopwatch1.Stop();

                // استخراج الدقائق والثواني مباشرة من الـ Stopwatch
                int minutes = stopwatch1.Elapsed.Minutes;
                double seconds = stopwatch1.Elapsed.Seconds + (stopwatch1.Elapsed.Milliseconds / 1000.0);

                // صياغة النص بالشكل المطلوب (مثال: 2m 15.42s أو 0m 5.12s)
                string formattedTime = $"{minutes}m {seconds:0.00}s";

                clsGlobal.TimeInMillisecond = formattedTime;

                MessageBox.Show($"Project structure generated successfully :), In: {formattedTime}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            if (ckAiCodeDocs.Checked)
            {


                await GenerateDocumentationAsync();

            }


        }


        private async Task GenerateDocumentationAsync()
        {
            AIDocumentationGenerator aiDocumentationService = new AIDocumentationGenerator();
            DocumentationProcessor documentationProcessor = new DocumentationProcessor(aiDocumentationService);

            var progress = new Progress<DocumentationProgress>(p =>
            {
                progressBar.Maximum = p.TotalFiles;
                progressBar.Value = p.ProcessedFiles;

                lbCurrentFile.Text =
                    $"Processing: {p.CurrentFile} ({p.ProcessedFiles}/{p.TotalFiles}) - {p.Percentage:F2}%";
            });

            await documentationProcessor.ProcessDirectoryAsync(
                clsGlobal.PathFilesToGenerate,
                progress);

            MessageBox.Show(
                "Documentation generated successfully :)",
                "Done",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private async Task GenerateProjectStructureAsync(bool AddAPI,bool UseDTO)
        {

            var progress = new Progress<ProjectStructureGenerationProgress>(p =>
            {
                progressBar.Maximum = p.TotalSteps;
                progressBar.Value = p.ProcessedSteps;

                lbCurrentFile.Text =
                    $"Processing: {p.CurrentStep} ({p.ProcessedSteps}/{p.TotalSteps}) - {p.Percentage:F2}%";
            });

            // Generate the solution using clsSolutionGenerator 
            await clsSolutionGenerator.GenerateSolutionAsync(new SolutionConfiguration
            {
                SolutionName = clsGlobal.ProjectName,
                EnableSwagger = true,
                EnableApiVersioning = true,
                IncludeApi = AddAPI,
                IncludeBusiness = true,
                IncludeDataAccess = true,
                IncludeDto = UseDTO,
                IncludeMigrations = true,
                DotNetVersion = clsGlobal.DotnetCoreVersion,
                OutputDirectory = clsGlobal.PathFilesToGenerate,


            },progress);


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

        private void ckGenerateAPI_CheckedChanged(object sender, EventArgs e)
        {
            switchUsingDTO.Checked = ckGenerateAPI.Checked;
        }

        private void switchUsingDTO_CheckedChanged(object sender, EventArgs e)
        {
            if (ckGenerateAPI.Checked)
            {
                switchUsingDTO.Checked = true;

                MessageBox.Show(
                    "API requires DTOs! You cannot select 'Don't Use DTO' while API is enabled.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void rbBoth_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBoth.Checked && !switchUsingDTO.Checked)
            {
                switchUsingDTO.Checked = true;

                MessageBox.Show(
                    "Both operations require DTOs!",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

        }

        private void rbAsynchronous_CheckedChanged(object sender, EventArgs e)
        {

            if (rbAsynchronous.Checked && !switchUsingDTO.Checked)
            {
                switchUsingDTO.Checked = true;

                    MessageBox.Show(
                        "Asynchronous operations require DTOs!",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

            }
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
