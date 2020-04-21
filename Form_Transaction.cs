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
  
    public partial class Form_Transaction : Form
    {
        private string Tid;
        public Form_Transaction(string stTid,  string stOrderName)
        {
            Tid = stTid;
            this.Text = "История транзакций по заявке № " + stOrderName;
            InitializeComponent();
        }

        private void Form_Transaction_Load(object sender, EventArgs e)
        {


            
            Load_Transaction(Tid);



            
        }

        private void Load_Transaction(string Tid)
        {
            Console.WriteLine(" tid = " + Tid);

            Class_SQL.LoadGridDB(DGW_list, "select tid, add_date_time, messanger, USER_NAME from bi.dbo.ABS_TRANSACTION_HISTORY where hdr_tid = N'" + Tid + "' order by tid");

            DGW_list.Columns["add_date_time"].HeaderText = "Дата и время изменения";
            DGW_list.Columns["tid"].Visible = false;
            DGW_list.Columns["messanger"].Visible = false;
            DGW_list.Columns["USER_NAME"].Visible = false;
            DGW_list.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void DGW_list_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DGW_list.SelectedRows.Count > 0)
            {
                txt_Edit_User.Text = DGW_list.CurrentRow.Cells["USER_NAME"].Value.ToString();
                txt_messanger.Text = DGW_list.CurrentRow.Cells["messanger"].Value.ToString();
            }
        }
    }
}
