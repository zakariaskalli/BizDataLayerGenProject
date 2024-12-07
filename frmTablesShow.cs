using BizDataLayerGen.DataAccessLayer;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using BizDataLayerGen.GeneralClasses;
using System.Data;
//using GymDB_DataAccess;
using Project_DataAccessLayer;

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

                LBTables.Enabled = false;

            }
            else
            {

                LBTables.Enabled = true;

                for (int i = 0; i < LBTables.Items.Count; i++)
                {
                    LBTables.SetItemChecked(i, false); // Select each item in the ListBox
                }


            }

        }

        private void frmTablesShow_Load(object sender, EventArgs e)
        {

            //LBTables.Items.Clear();

            //LBTables.Items.AddRange(AllTables);

            LBTables.Items.Clear();
            LBTables.Items.AddRange(AllTables);

            chBAllTables.Checked = true;

            for (int i = 0; i < LBTables.Items.Count; i++)
            {
                LBTables.SetItemChecked(i, true); // Select each item in the ListBox
            }

            LBTables.Enabled = false;


        }

        private void cbTablesName_DropDown(object sender, EventArgs e)
        {
            //guna2CheckBox1.Visible = true;

        }
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
            MessageBox.Show(clsPeopleData.AddNewPeople("Khalid", "bibo", "nan", "Malki", "Arzaz@gmail.com",
                                   "060044456", DateTime.Now, true, "Fes", 125, DateTime.Now, DateTime.Now,
                                   null, 1).ToString());
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
            bool Gender =  false;
            string? Address = "";
            int CityID = -1;
            DateTime CreatedTime = DateTime.Now;
            DateTime LastUpdat = DateTime.Now;
            string? ProfilePicture = "";
            int? CreatedByUserID = -1;


            clsPeopleData.GetPeopleInfoByID(PersonID,ref FirstName, ref SecondName, ref ThirdName,
               ref LastName, ref Email, ref Phone, ref DateOfBirth, ref Gender, ref Address, ref CityID,
               ref CreatedTime, ref LastUpdat, ref ProfilePicture, ref CreatedByUserID);
            */

            // Update Person

            /*
            MessageBox.Show(clsPeopleData.UpdatePeopleByID(57, "Ziko", "zaki", "nan", "Malki", "Arzaz@gmail.com",
                                   "060044456", DateTime.Now, true, "Fes hakma l3alam", 125, DateTime.Now, DateTime.Now,
                                   null, 1).ToString());
            */

            // Update Payment

            //MessageBox.Show(clsPaymentsData.UpdatePaymentsByID(3,1, 3, null, true, 2).ToString());

            // Tests For Delete 

            /*
            MessageBox.Show(clsPeopleData.deletePeople(57).ToString());

            MessageBox.Show(clsPaymentsData.deletePayments(4).ToString());
            */

            // Test SearchData

            /*
            int x = 0;
            DataTable dt = clsPeopleData.SearchData("Gender", "1");
            x = 5;

            // تحويل محتويات DataTable إلى نص لعرضه في MessageBox
            string result = "DataTable Contents:\n";
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    result += $"{column.ColumnName}: {row[column]} \t";
                }
                result += "\n";
            }

            MessageBox.Show(result, "DataTable Data");
            */


            bool FkOfAll = rbJustThis.Checked == false && rbAll.Checked == true;

            


            if ( clsAddDataAccessAndBusinessLayers.AddDataAndBusinessLayers(NameTables, FkOfAll) == clsGlobal.enTypeRaisons.enPerfect)
                MessageBox.Show("Created Success");
            else
                MessageBox.Show("Error Created Success");


        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
