using BizDataLayerGen.DataAccessLayer;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using BizDataLayerGen.GeneralClasses;
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

            LBTables.Items.Clear();

            LBTables.Items.AddRange(AllTables);

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


             if (chBAllTables.Checked == false && !SelectItem)
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

            // Thank you commit
            if ( clsAddDataAccessAndBusinessLayers.AddDataAndBusinessLayers(NameTables) == clsGlobal.enTypeRaisons.enPerfect)
                MessageBox.Show("Created Success");
            else
                MessageBox.Show("Error Created Success");

        }
    }
}
