using BizDataLayerGen.ConnectingWithRegister;
using BizDataLayerGen.DataAccessLayer;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizDataLayerGen
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            lblLogin.BackColor = Color.Transparent;
            btnLogin.BackColor = Color.Transparent;
            lblRemeberMe.BackColor = Color.Transparent;
            switch1.BackColor = Color.Transparent;

            string UserId = "";

            string Password = "";

            if (clsRegister.LoadUserIdAndPassword(ref UserId,ref Password))
            {
                tbUserID.Text = UserId;
                tbPassword.Text = Password;

                this.AcceptButton = btnLogin;
                
                //btnLogin.Checked = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (tbUserID.Text == "" || tbPassword.Text == "")
            {
                MessageBox.Show("Please Enter The UserName/Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }



            string UserID = tbUserID.Text;
            string Password = tbPassword.Text;

            string ConnectionString = $"Server=.;User Id={UserID};Password= {Password};";

            if (clsGeneralWithData.TestDatabaseConnection(ConnectionString))
            {
                clsDataAccessSettings.UserId = UserID;
                clsDataAccessSettings.Password = Password;

                if (switch1.Checked == true)
                {
                    clsRegister.DeleteDataSignInRegister();
                    clsRegister.AddDataToRegister(UserID, Password);

                }
                else
                {
                    clsRegister.DeleteDataSignInRegister();
                }


                this.Hide();
                CodeGenratorForm frm = new CodeGenratorForm();
                frm.ShowDialog();

            }
            else
            {
                MessageBox.Show("Error Connecting","Please Enter A True Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbUserID.Focus();
            }

        }

        private void tbUserID_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(tbUserID.Text))
                errorProvider1.SetError(tbUserID,"Please Enter UserID");
            else
            {
                errorProvider1.SetError(tbUserID, "");
            }

        }

        private void tbPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPassword.Text))
                errorProvider1.SetError(tbPassword, "Please Enter UserID");
            else
            {
                errorProvider1.SetError(tbPassword, "");
            }

        }


        private void tbUserID_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Tab)
            {
                if (string.IsNullOrEmpty(tbUserID.Text) == true)
                {
                    errorProvider1.SetError(tbUserID, "Please Enter UserID");
                }
                e.SuppressKeyPress = true; // Prevents the 'Tab' from adding whitespace in the textbox
                tbPassword.Focus(); // Move focus to the next textbox
            }
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPassword.Text) == true)
            {
                errorProvider1.SetError(tbPassword, "Please Enter Password");
                return;
            }

            if (e.KeyCode == Keys.Tab)
            {
                
                e.SuppressKeyPress = true; // Prevents the 'Tab' from adding whitespace in the textbox
                tbUserID.Focus(); // Move focus to the next textbox
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents the 'Tab' from adding whitespace in the textbox
                btnLogin.PerformClick();
            }
        }

        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void tbUserID_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
