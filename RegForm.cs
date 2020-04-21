using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Principal;
 



namespace ABS_C
{
  


    public partial class RegForm : Form
    {
        public RegForm()
        {
            InitializeComponent();
            ComboBox_Reg_Login.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboBox_Reg_Login.AutoCompleteSource = AutoCompleteSource.ListItems;
            
            Class_SQL.OpenConnection();
            this.ComboBox_Reg_Login.Focus();
            Class_SQL.LoadComboBox(this.ComboBox_Reg_Login, "select user_name as 'ID', description  as 'NAME' from bi.dbo.ABS_USER_PROFILE where active= 'Y' and DOSTUP_ABS = N'Y' order by description asc");
                      
        }

        private void Reg_Button_Chancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Load_User()
        {

            if (this.ComboBox_Reg_Login.SelectedIndex != -1)
            {
                string stLogin = this.ComboBox_Reg_Login.SelectedValue.ToString().ToLower();
            
            string stPassword = this.Reg_Textbox_Password.Text;

            if (!string.IsNullOrEmpty (stLogin))
            {
                if (!string.IsNullOrEmpty(stPassword))
                {
                    
                    double d = 0;

                    string stPass = null;
                                        
                    d = Class_SQL.SelectFloat("select count(*) from bi.dbo.ABS_USER_PROFILE where active= 'Y' and DOSTUP_ABS = N'Y' and USER_NAME = N'" + stLogin + "'");

                    stPass = Class_SQL.SelectString("select PASSWORD from bi.dbo.ABS_USER_PROFILE where active= 'Y' and DOSTUP_ABS = N'Y' and USER_NAME = N'" + stLogin + "'");
                    
                    if (stPass == Class_SQL.SelectString("select SNT.dbo.svc_md5_ngen(N'" + stPassword + "')"))

                    {
                        this.Hide();
                        FirstForm f2 = new FirstForm(stLogin);
                        f2.Show();
                        
                    }
                    else
                    {
                        MessageBox.Show("Не верно указан логин или пароль.", "АБС", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Reg_Textbox_Password.Focus();

                    }
                }
                else
                {
                    MessageBox.Show("Поле Пароль не заполнено.", "АБС", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Reg_Textbox_Password.Focus();
                }
            }
            else
            {
                MessageBox.Show( "Поле Логин не заполнено.", "АБС", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Reg_Textbox_Password.Focus();
            }
            }
            else
            {
                MessageBox.Show("Поле Логин не заполнено.", "АБС", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ComboBox_Reg_Login.Focus();
            }
        }
        
        private void Reg_Button_Ok_Click(object sender, EventArgs e)
        {
            Load_User();
        }

        private void Reg_Textbox_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode  == Keys.Enter)
                {
                Load_User();
            }
        }

        private void Reg_Textbox_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Load_User();
            }
        }

        private void Reg_Button_Ok_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Load_User();
            }
        }

        public void Close_form()
        {
            this.Close();
            Class_SQL.CloseConnection();
        }

      

        private void ComboBox_Reg_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Load_User();
            }
        }

        private void RegForm_Load(object sender, EventArgs e)
        {          
            
        }
    }
}
