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
    public partial class Form_library : Form
    {
        public Form_library()
        {
            InitializeComponent();
        }

        private void Form_library_Load(object sender, EventArgs e)
        {

        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Name.ToString() =="dop_uslugi")
            {
                Load_Lib_Dop_Usliga();
            }else if (treeView1.SelectedNode.Name.ToString() == "dop_meterial")
            {
                Load_Lib_Dop_Material();
            }else if (treeView1.SelectedNode.Name.ToString() == "dop_operation")
            {
                Load_Lib_Dop_Operation();
            }else if (treeView1.SelectedNode.Name.ToString() == "lib_en")
            {
                Load_Lib_EN();
            }
        }


        public void Load_Lib_Dop_Usliga()
        {
            Class_SQL.OpenConnection();
            Class_SQL.LoadGridDB(dataGrid_Library, "select o.TID, o.NAME, o.POD_VID as UROVEN2, o.LEVEL_3 as UROVEN3 ,e.NAME as NAME_EN ,o.ACTIVE,e.KEY_ID from bi.dbo.ABS_AMENITIES  o join bi.dbo.ABS_EN e on o.KEY_EN=e.KEY_ID");
            Class_SQL.CloseConnection();
            dataGrid_Library.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGrid_Library.Columns["NAME"].HeaderText = "Наименование";
            dataGrid_Library.Columns["UROVEN2"].HeaderText = " Уровень 2";
            dataGrid_Library.Columns["UROVEN3"].HeaderText = " Уровень 3";
            dataGrid_Library.Columns["NAME_EN"].HeaderText = "Ед Измерения";
            dataGrid_Library.Columns["ACTIVE"].HeaderText = "Статус";

            dataGrid_Library.Columns["TID"].Visible = false;
            dataGrid_Library.Columns["KEY_ID"].Visible = false;
        }


        public void Load_Lib_Dop_Material()
        {
            Class_SQL.OpenConnection();
            Class_SQL.LoadGridDB(dataGrid_Library, "select m.TID, m.name as NAME , POD_NAME as UROVEN2, e.NAME as NAME_EN,m.ACTIVE, e.KEY_ID from bi.dbo.ABS_MATERIAL m join bi.dbo.ABS_EN e on m.KEY_EN=e.KEY_ID");
            Class_SQL.CloseConnection();
            dataGrid_Library.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGrid_Library.Columns["NAME"].HeaderText = "Наименование";
            dataGrid_Library.Columns["UROVEN2"].HeaderText = " Уровень 2";
            dataGrid_Library.Columns["NAME_EN"].HeaderText = "Ед Измерения";
            dataGrid_Library.Columns["ACTIVE"].HeaderText = "Статус";

            dataGrid_Library.Columns["TID"].Visible = false;
            dataGrid_Library.Columns["KEY_ID"].Visible = false;
        }

        public void Load_Lib_Dop_Operation()
        {
            Class_SQL.OpenConnection();
            Class_SQL.LoadGridDB(dataGrid_Library, "select o.TID, o.NAME, o.DOP_NAME as UROVEN2,e.NAME as NAME_EN ,o.ACTIVE,e.KEY_ID from bi.dbo.ABS_OPERATION  o join bi.dbo.ABS_EN e on o.KEY_EN=e.KEY_ID");
            Class_SQL.CloseConnection();
            dataGrid_Library.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGrid_Library.Columns["NAME"].HeaderText = "Наименование";
            dataGrid_Library.Columns["UROVEN2"].HeaderText = " Уровень 2";
            dataGrid_Library.Columns["NAME_EN"].HeaderText = "Ед Измерения";
            dataGrid_Library.Columns["ACTIVE"].HeaderText = "Статус";

            dataGrid_Library.Columns["TID"].Visible = false;
            dataGrid_Library.Columns["KEY_ID"].Visible = false;
        }

        public  void Load_Lib_EN()
        {
            Class_SQL.OpenConnection();
            Class_SQL.LoadGridDB(dataGrid_Library, "select KEY_ID as TID,NAME,ACTIVE from  bi.dbo.ABS_EN ");
            Class_SQL.CloseConnection();
            dataGrid_Library.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGrid_Library.Columns["NAME"].HeaderText = "Наименование";           
            dataGrid_Library.Columns["ACTIVE"].HeaderText = "Статус";

          
            dataGrid_Library.Columns["TID"].Visible = false;
        }

        private void НовыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_Library_Add fm = new Form_Library_Add();
            fm.TextForm = "Добавить ";
            fm.FormType = treeView1.SelectedNode.Text;
            fm.TypeNme = treeView1.SelectedNode.Name.ToString();
            fm.Type = "NEW";
            fm.Act = true;
            fm.Show();
        }

        private void РедактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dataGrid_Library.SelectedRows.Count > 0)
            {
                           
                Form_Library_Add fm = new Form_Library_Add();
                fm.TextForm = "Редактировать ";
                fm.FormType = treeView1.SelectedNode.Text;
                fm.TypeNme = treeView1.SelectedNode.Name.ToString();
                fm.Type = "EDIT";
                fm.TID = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["TID"].Value);
                fm.Names = dataGrid_Library.CurrentRow.Cells["NAME"].Value.ToString();

                 string a = dataGrid_Library.CurrentRow.Cells["ACTIVE"].Value.ToString();

                if (a == "Y")
                {
                    fm.Act = true;
                }
                else
                {
                    fm.Act = false;
                }


                if (treeView1.SelectedNode.Name == "lib_en")
                {
          
                    fm.Key_id = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["TID"].Value);
                }
                else if (treeView1.SelectedNode.Name == "dop_uslugi")
                {
                    fm.Uroven2 = dataGrid_Library.CurrentRow.Cells["UROVEN2"].Value.ToString();
                    fm.Uroven3 = dataGrid_Library.CurrentRow.Cells["UROVEN3"].Value.ToString();
                    fm.Key_id = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["KEY_ID"].Value);
                }
                else
                {
                    fm.Uroven2 = dataGrid_Library.CurrentRow.Cells["UROVEN2"].Value.ToString();
                    fm.Key_id = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["KEY_ID"].Value);
                }               

                fm.Show();
            }
        }

      

        private void ОбновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Name.ToString() == "dop_uslugi")
            {
                Load_Lib_Dop_Usliga();
            }
            else if (treeView1.SelectedNode.Name.ToString() == "dop_meterial")
            {
                Load_Lib_Dop_Material();
            }
            else if (treeView1.SelectedNode.Name.ToString() == "dop_operation")
            {
                Load_Lib_Dop_Operation();
            }
            else if (treeView1.SelectedNode.Name.ToString() == "lib_en")
            {
                Load_Lib_EN();
            }
        }

        private void DataGrid_Library_DoubleClick(object sender, EventArgs e)
        {

        }

        private void КопироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGrid_Library.SelectedRows.Count > 0)
            {

                Form_Library_Add fm = new Form_Library_Add();
                fm.TextForm = "Копировать ";
                fm.FormType = treeView1.SelectedNode.Text;
                fm.TypeNme = treeView1.SelectedNode.Name.ToString();
                fm.Type = "COPY";
                fm.TID = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["TID"].Value);
                fm.Names = dataGrid_Library.CurrentRow.Cells["NAME"].Value.ToString();

                string a = dataGrid_Library.CurrentRow.Cells["ACTIVE"].Value.ToString();

                if (a == "Y")
                {
                    fm.Act = true;
                }
                else
                {
                    fm.Act = false;
                }

                if (treeView1.SelectedNode.Name == "lib_en")
                {

                    fm.Key_id = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["TID"].Value);
                }
                else if (treeView1.SelectedNode.Name == "dop_uslugi")
                {
                    fm.Uroven2 = dataGrid_Library.CurrentRow.Cells["UROVEN2"].Value.ToString();
                    fm.Uroven3 = dataGrid_Library.CurrentRow.Cells["UROVEN3"].Value.ToString();
                    fm.Key_id = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["KEY_ID"].Value);
                }
                else
                {
                    fm.Uroven2 = dataGrid_Library.CurrentRow.Cells["UROVEN2"].Value.ToString();
                    fm.Key_id = Convert.ToDecimal(dataGrid_Library.CurrentRow.Cells["KEY_ID"].Value);
                }

                fm.Show();
            }
        }
    }
}
