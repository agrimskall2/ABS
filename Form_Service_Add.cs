using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ABS_C
{
    public partial class Form_Service_Add : Form
    {
        
        private static string Ordernumber = "0000000" ;
        
        public Form_Service_Add()
        {
            InitializeComponent();        
            
        }
               

        private void Button_Add_Service_Add_Click(object sender, EventArgs e)
        {
                                             
            if (ComboBox_Add_Service_Order_Type.SelectedIndex != -1)
            {
                if (ComboBox_Add_Service_Company.SelectedIndex != -1)
                {

                    if (ComboBox_Add_Service_Warehouse.SelectedIndex != -1)

                    {
                        if (!string.IsNullOrEmpty(TextBox_Add_Service_Number.Text.ToString()))
                        {
                            Class_SQL.Insert_Service(ComboBox_Add_Service_Order_Type.SelectedItem.ToString(),
                                                     TextBox_Add_Service_Number.Text,
                                                     ComboBox_Add_Service_Company.SelectedValue.ToString(),
                                                     ComboBox_Add_Service_Warehouse.SelectedValue.ToString(),
                                                     TextBox_Add_Service_Osnovanie.Text,
                                                     TextBox_Add_Service_Messanger.Text,
                                                     Convert.ToString(DateTimePicker_Add_Service_Date.Value.ToString("yyyy-MM-dd")) + ' ' + DateTimePicker_Add_Service_Time.Text + ":00",
                                                     FirstForm.UserName
                                                    );

                            this.Close();


                        }
                        else
                        {
                            MessageBox.Show("Поле Номер документа не заполнено.", "ABS : Добавление Сервисной Заявки", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле Склад не заполнено.", "ABS : Добавление Сервисной Заявки", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Поле Компания не заполнено.", "ABS : Добавление Сервисной Заявки", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Поле Тип заявки не заполнено.", "ABS : Добавление Сервисной Заявки", MessageBoxButtons.OK);
            }
        }

        private void Form_Service_Add_Load(object sender, EventArgs e)
        {
          
            Ordernumber =  "00000000"  + Convert.ToString( Class_SQL.SelectNextNumber());
                       
            Ordernumber = "SER-" + RightString(Ordernumber, 9);

            ComboBox_Add_Service_Order_Type.Items.Clear();
            ComboBox_Add_Service_Order_Type.Items.Add("Сервисная заявка");
            ComboBox_Add_Service_Order_Type.SelectedItem = "Сервисная заявка";
            ComboBox_Add_Service_Order_Type.Enabled = false;
            
            TextBox_Add_Service_Number.Text = Ordernumber;
            DateTimePicker_Add_Service_Date.Value = DateTime.Now;

            DateTimePicker_Add_Service_Time.Text = "00:00:00";

            Class_SQL.LoadComboBox(ComboBox_Add_Service_Company, "select COMPANY as 'ID', DESCRIPTION as 'NAME' from bi.dbo.ABS_COMPANY where ACTIVE='Y' and COMPANY in (" + Class_SQL.StringActivCompany(FirstForm.UserName) + ")");
            ComboBox_Add_Service_Company.SelectedIndex = -1;

            Class_SQL.LoadComboBox(ComboBox_Add_Service_Warehouse, "select WAREHOUSE as 'ID', DESCRIPTION as 'NAME' from bi.dbo.ABS_WAREHOUSE where ACTIVE='Y' and WAREHOUSE in (" + Class_SQL.StringActivWarehouse(FirstForm.UserName) + ")");
            ComboBox_Add_Service_Warehouse.SelectedIndex = -1;

        }

        
        public static string RightString(string param, int length)
        {

            string result = param.Substring(param.Length - length, length);
            
            return result;
        }
    }
}
