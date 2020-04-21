using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ABS_C
{
    public partial class Form_Library_Add : Form
    {
        public string TextForm = null;
        public string FormType = null;
        public string TypeNme = null;
        public string Type = null;        // тип записи NEW  
        public string TypeLibrary = null; //вид библиотеки

        public decimal TID = 0;
        public string Names = null;
        public string Uroven2 = null;
        public string Uroven3 = null;
        public string EN = null;
        public bool Act = true;
        public decimal Key_id = 0;

        public Form_Library_Add()
        {
            InitializeComponent();
        }

        private void Form_Library_Add_Load(object sender, EventArgs e)
        {
            this.Text = TextForm + FormType;


            txt_name.Text = Names;
            txt_uroven2.Text = Uroven2;
            txt_uroven3.Text = Uroven3;
            
            cb_active.Checked = Act;


            Class_SQL.LoadComboBox(cbm_en, "select KEY_ID as ID ,NAME  from  bi.dbo.ABS_EN where ACTIVE ='Y' order by NAME");

            if (TypeNme == "lib_en")
            {
                txt_uroven2.Visible = false;
                
                lab_uroven2.Visible = false;
              
                cbm_en.Visible = false;
                lab_en.Visible = false;

               
            }else if(TypeNme == "dop_uslugi")
            {
                txt_uroven3.Visible = true;
                lab_uroven3.Visible = true; 
            }
            else if (TypeNme == "dop_meterial")
            {
                txt_uroven3.Visible = false;
                lab_uroven3.Visible = false;
            }

            else if (TypeNme == "dop_operation")
            {
                txt_uroven3.Visible = false;
                lab_uroven3.Visible = false;
            }


            if (Type == "NEW")
                {
                    cbm_en.SelectedIndex = -1;
                }else
            {
                if (TypeNme != "lib_en")
                {
                    //cbm_en.SelectedValue = EN;
                }
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_add_Click(object sender, EventArgs e)
        {
            if (cbm_en.SelectedIndex != -1)
            {

                    if (Type == "NEW")
                    {
                        Class_SQL.Execute_Library_En(TypeNme, Type, 0, txt_name.Text, txt_uroven2.Text, txt_uroven3.Text, Convert.ToString(cb_active.Checked), FirstForm.UserName, Convert.ToDecimal(cbm_en.SelectedValue.ToString()), TID);
                        //Form_library.Load_Lib_Dop_Operation();
                        this.Close();
                    }
                    else if (Type == "EDIT")
                    {
                        Class_SQL.Execute_Library_En(TypeNme, Type, Key_id, txt_name.Text, txt_uroven2.Text, txt_uroven3.Text, Convert.ToString(cb_active.Checked), FirstForm.UserName, Convert.ToDecimal(cbm_en.SelectedValue.ToString()), TID);
                        //Form_library.Load_Lib_Dop_Operation();
                        this.Close();
                    }
                    else if (Type == "COPY")
                    {
                        Class_SQL.Execute_Library_En(TypeNme, Type, Key_id, txt_name.Text, txt_uroven2.Text, txt_uroven3.Text, Convert.ToString(cb_active.Checked), FirstForm.UserName, Convert.ToDecimal(cbm_en.SelectedValue.ToString()), TID);
                        //Form_library.Load_Lib_Dop_Operation();
                        this.Close();
                    }

            }
            else
            {
                MessageBox.Show("Не указана Ед.измерения.", "ABS", MessageBoxButtons.OK);
            }

        }
    }
}
