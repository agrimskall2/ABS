using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Zuby.ADGV;
using Microsoft;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data.SqlClient;

namespace ABS_C
{
    public partial class FirstForm : Form
    {
        public static string UserName;
        public static int strUrovenDostupa = 0;
        private static string Serch_receipt_string = null;

        private static string Sort_receipt_string = null;
        public static string Receipt_Select_Column_Name;


        private static string Serch_Shipment_string = null;
        private static string Sort_Shipment_string = null;
        public static string Shipment_Select_Column_Name;

        private static string Serch_Service_string = null;
        private static string Sort_Service_string = null;
        public static string Service_Select_Column_Name;

        private static int Count_pl = 0;

        public string SelectCompany;
        public string SelectWarehouse;

        List<SortListName> RecSortList = new List<SortListName>();
        List<SortListName> ShipSortList = new List<SortListName>();
        List<SortListName> SerSortList = new List<SortListName>();
        List<TableListDopOperation> ListDopOperation = new List<TableListDopOperation>();

        class SortListName
        {
            public string ColumnName { get; set; }
            public string ColumnValue { get; set; }
        }

        public class TableListDopOperation
        {
            public string Name { get; set; }
            public string Uroven2 { get; set; }
            public string Uroven3 { get; set; }
            public string En { get; set; }

        }

        public FirstForm(string Logins)
        {
            InitializeComponent();

            UserName = Logins;

            this.DatePicker_Receipt_Date_Plan_Arrival_Ats.Value = DateTime.Now;
            this.DatePicker_Receipt_Date_Acceptance.Value = DateTime.Now;
            this.DatePicker_Receipt_Date_Actual_onDock_Ats.Value = DateTime.Now;
            this.DatePicker_Receipt_Date_Completion_Discharge_Ats.Value = DateTime.Now;
            this.DatePicker_Receipt_Date_Fact_Arrival_Ats.Value = DateTime.Now;

            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);

            this.KeyPreview = true;
            Load_Form();

        }


        private void Load_Form()
        {

            //StreamReader sr = new StreamReader("st.txt");
            string line, Version = null;

            //while (!sr.EndOfStream)
            //{
            //    Version = sr.ReadLine();
            //}

            
            Class_Open_Programm.StartProcess();

            // Вызов процедуры по разблокировки из архива учетной записи
            Class_SQL.ReopenUserArhiv(UserName); 

            string stUserFIO = Class_SQL.SelectString("select top 1 DESCRIPTION from dbo.ABS_USER_PROFILE where active = 'Y' and USER_NAME = N'" + UserName + "'");

            strUrovenDostupa = Class_Security.DostupAdmin(UserName);





            this.Text = "ABS C# " + stUserFIO + " " + this.ProductVersion ;
            if (strUrovenDostupa == 1)
            {
                this.tsmi_library.Visible = true;
            }else
            {
                this.tsmi_library.Visible = false;
            }

            Console.WriteLine(UserName);

            



            Class_SQL.LoadComboBoxDopRazdel(ComboBox_Receipt_DopRazdel);
  
            ComboBox_Receipt_Status.Items.Clear();
            ComboBox_Receipt_Status.Items.Add("Зарегистрировано");
            ComboBox_Receipt_Status.Items.Add("ТС прибыло");
            ComboBox_Receipt_Status.Items.Add("В обработке");
            ComboBox_Receipt_Status.Items.Add("Готов к отгрузке");            
            ComboBox_Receipt_Status.Items.Add("Выполнено");            
            ComboBox_Receipt_Status.Items.Add("Расформировано");
            ComboBox_Receipt_Status.Items.Add("Удалено");
            ComboBox_Receipt_Status.SelectedIndex = -1;
          

            Class_SQL.LoadComboBox(ComboBox_Receipt_Users, "select  USER_NAME as 'ID', description as 'NAME' from dbo.ABS_USER_PROFILE where ACTIVE=N'Y' order by description");
            this.ComboBox_Receipt_Users.SelectedIndex = -1;
            this.ComboBox_Receipt_Users.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.ComboBox_Receipt_Users.AutoCompleteSource = AutoCompleteSource.ListItems;


            Class_SQL.LoadComboBoxStatus(ComboBox_Shipment_Status);
            Class_SQL.LoadComboBoxDopRazdel(ComboBox_Shipment_DopRazdel);

            Class_SQL.LoadComboBoxStatus(ComboBox_Service_Status);
            Class_SQL.LoadComboBoxDopRazdel(ComboBox_Service_DopRazdel);

            this.TextBox_Receipt_Company.Enabled = false;
            this.TextBox_Receipt_OrderErp.Enabled = false;
            this.TextBox_Receipt_OrderNumber.Enabled = false;
            this.TextBox_Receipt_OrderType.Enabled = false;
            this.TextBox_Receipt_Warehouse.Enabled = false;
            this.DatePicker_Receipt_Date_Create.Enabled = false;
            this.DatePicker_Receipt_Time_Create.Enabled = false;


            SelectCompany = Class_SQL.StringActivCompany(UserName);
            SelectWarehouse = Class_SQL.StringActivWarehouse(UserName);

            Load_Receipt_Table();
            this.TextBox_Receipt_DopRazdel_Qty.Text = "0";

            Load_Shipment_Table();
            Class_SQL.LoadComboBox(this.ComboBox_Shipment_Users, "select  USER_NAME as 'ID', description as 'NAME' from dbo.ABS_USER_PROFILE where ACTIVE=N'Y' order by description");
            this.ComboBox_Shipment_Users.SelectedIndex = -1;
            this.ComboBox_Shipment_Users.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.ComboBox_Shipment_Users.AutoCompleteSource = AutoCompleteSource.ListItems;

            this.TextBox_Shipment_DopRazdel_Qty.Text = "0";

            Load_Service_Table();
            this.TextBox_Service_DopRazdel_Qty.Text = "0";
        }

        public void Load_Receipt_Table()
        {
            Class_SQL.LoadGridDB(DataGrid_Receipt_Table, "SELECT  TID, TYPE, STATUS, WAREHOUSE, COMPANY, ORDER_NUMBER, ERP_NUMBER, GRUZNAME, INVOICE, RECEIPT_DATE, RECEIPT_TIME, " +
                                                         " DAY_WEEK, isnull(TOTAL_LINES,0) as TOTAL_LINES, PLAN_ARRIVAL_ATS_DATE, PLAN_ARRIVAL_ATS_TIME, isnull(COUNT_BOX_ROS ,0) as COUNT_BOX_ROS, isnull(COUNT_PALLET,0) as COUNT_PALLET, FACT_ARRIVAL_ATS_DATE, " +
                                                         " FACT_ARRIVAL_ATS_TIME, ACTUAL_ONDOCK_ATS_DATE, ACTUAL_ONDOCK_ATS_TIME, COMPLETION_DISCHARGE_ATS_DATE, " +
                                                         " COMPLETION_DISCHARGE_ATS_TIME, RECEIPT_ACCEPTANCE_DATE, RECEIPT_ACCEPTANCE_TIME, USER_NAME, " +
                                                         " MESSANGER, USERS, USER_DEF8 FROM BI.dbo.V_ABS_ORDER_RECEIPT WHERE " +
                                                         " COM_CODE in (" + SelectCompany + ") and " +
                                                         " war_code in (" + SelectWarehouse + ") " + Serch_receipt_string + 
                                                         Sort_receipt_string);


            DataGrid_Receipt_Table.Columns["TYPE"].HeaderText = "Тип документа";
            DataGrid_Receipt_Table.Columns["STATUS"].HeaderText = "Статус";
            DataGrid_Receipt_Table.Columns["WAREHOUSE"].HeaderText = "Склад";
            DataGrid_Receipt_Table.Columns["COMPANY"].HeaderText = "Компания";
            DataGrid_Receipt_Table.Columns["ORDER_NUMBER"].HeaderText = "Номер заявки";
            DataGrid_Receipt_Table.Columns["ERP_NUMBER"].HeaderText = "Номер заявки клиента";
            DataGrid_Receipt_Table.Columns["GRUZNAME"].HeaderText = "Грузоотправитель";
            DataGrid_Receipt_Table.Columns["INVOICE"].HeaderText = "Инвойс";
            DataGrid_Receipt_Table.Columns["RECEIPT_DATE"].HeaderText = "Дата поступления заявки";
            DataGrid_Receipt_Table.Columns["RECEIPT_TIME"].HeaderText = "Время поступления заявки";
            DataGrid_Receipt_Table.Columns["DAY_WEEK"].HeaderText = "День недели";
            DataGrid_Receipt_Table.Columns["TOTAL_LINES"].HeaderText = "Кол-во строк";
            DataGrid_Receipt_Table.Columns["PLAN_ARRIVAL_ATS_DATE"].HeaderText = "Плановая дата прихода АТС";
            DataGrid_Receipt_Table.Columns["PLAN_ARRIVAL_ATS_TIME"].HeaderText = "Плановое время прихода АТС";
            DataGrid_Receipt_Table.Columns["COUNT_PALLET"].HeaderText = "Выгружено паллет";
            DataGrid_Receipt_Table.Columns["COUNT_BOX_ROS"].HeaderText = "Выгружено коробов в навал";
            DataGrid_Receipt_Table.Columns["FACT_ARRIVAL_ATS_DATE"].HeaderText = "Фактическая дата прихода АТС";
            DataGrid_Receipt_Table.Columns["FACT_ARRIVAL_ATS_TIME"].HeaderText = "Фактическое время прихода АТС";
            DataGrid_Receipt_Table.Columns["ACTUAL_ONDOCK_ATS_DATE"].HeaderText = "Фактическая дата постановки на док";
            DataGrid_Receipt_Table.Columns["ACTUAL_ONDOCK_ATS_TIME"].HeaderText = "Фактическое время постановки на док";
            DataGrid_Receipt_Table.Columns["COMPLETION_DISCHARGE_ATS_DATE"].HeaderText = "Дата окончания выгрузки АТС";
            DataGrid_Receipt_Table.Columns["COMPLETION_DISCHARGE_ATS_TIME"].HeaderText = "Время окончания выгрузки АТС";
            DataGrid_Receipt_Table.Columns["RECEIPT_ACCEPTANCE_DATE"].HeaderText = "Дата окончания приемки";
            DataGrid_Receipt_Table.Columns["RECEIPT_ACCEPTANCE_TIME"].HeaderText = "Время окончания приемки";
            DataGrid_Receipt_Table.Columns["MESSANGER"].HeaderText = "Примечание";
            DataGrid_Receipt_Table.Columns["USER_NAME"].HeaderText = "Контролер";

            DataGrid_Receipt_Table.Columns["TID"].Visible = false;
            DataGrid_Receipt_Table.Columns["USERS"].Visible = false;
            DataGrid_Receipt_Table.Columns["USER_DEF8"].Visible = false;            

            DataGrid_Receipt_Table.SetFilterAndSortEnabled(DataGrid_Receipt_Table.Columns["TYPE"], false);

            if (string.IsNullOrEmpty(Sort_receipt_string))
            {
                DataGrid_Receipt_Table.Sort(DataGrid_Receipt_Table.Columns["TID"], ListSortDirection.Descending);
            }
        }

        private void Load_Shipment_Table()
        {
            Class_SQL.LoadGridDB(DataGrid_Shipment_Table, "SELECT   TID, TYPE, ORDER_NUMBER, ERP_NUMBER, GRUZNAME, STATUS, isnull(TOTAL_LINES,0) as TOTAL_LINES, COMPANY, WAREHOUSE, SHIP_DATE, SHIP_TIME " + //10
                                                               ", DAY_WEEK, MESSANGER, USERS, isnull(COUNT_PALLET_ROS, 0) as COUNT_PALLET_ROS, isnull(COUNT_PALLET, 0) as COUNT_PALLET, " +                 //15
                                                               "  COMPLETION_LOADING_ATS_DATE, COMPLETION_LOADING_ATS_TIME, isnull(COUNT_ALL_PALLET, 0) as COUNT_ALL_PALLET, " +                            //18
                                                               "  isnull(COUNT_ALL_BOX,0) as COUNT_ALL_BOX, isnull(COUNT_MIX_PALLET,0) as COUNT_MIX_PALLET, isnull(COUNT_TERM_BOX, 0) as COUNT_TERM_BOX, " + //21
                                                               "  ACTUAL_ARRIVAL_ATS_DATE, ACTUAL_ARRIVAL_ATS_TIME, ACTUAL_ONDOCK_ATS_DATE, ACTUAL_ONDOCK_ATS_TIME, PLAN_SHIPMEN_DATE, PLAN_SHIPMEN_TIME," + //27
                                                               "  FACT_SHIPMENT_DATE, FACT_SHIPMENT_TIME, isnull(COUNT_TERM_PALLET, 0) as COUNT_TERM_PALLET , description, USER_DEF8 FROM dbo.V_ABS_ORDER_SHIPMENT WHERE " +        //29
                                                               " COM_CODE in (" + SelectCompany +") and " +
                                                               " war_code in (" + SelectWarehouse  + ") " + Serch_Shipment_string +
                                                               Sort_Shipment_string);

            // определяем видимость  столбцов
            DataGrid_Shipment_Table.Columns["TID"].Visible = false;
            DataGrid_Shipment_Table.Columns["USERS"].Visible = false;

            // переименовываем заголовки табюлицы
            DataGrid_Shipment_Table.Columns["TYPE"].HeaderText = "Тип документа";
            DataGrid_Shipment_Table.Columns["ORDER_NUMBER"].HeaderText = "Номер заявки";
            DataGrid_Shipment_Table.Columns["ERP_NUMBER"].HeaderText = "Номер заявки клиента";
            DataGrid_Shipment_Table.Columns["GRUZNAME"].HeaderText = "Грузополучатель";
            DataGrid_Shipment_Table.Columns["STATUS"].HeaderText = "Статус";
            DataGrid_Shipment_Table.Columns["TOTAL_LINES"].HeaderText = "Кол-во строк";
            DataGrid_Shipment_Table.Columns["SHIP_DATE"].HeaderText = "Дата поступления заявки";
            DataGrid_Shipment_Table.Columns["SHIP_TIME"].HeaderText = "Время поступления заявки";
            DataGrid_Shipment_Table.Columns["DAY_WEEK"].HeaderText = "День недели";
            DataGrid_Shipment_Table.Columns["MESSANGER"].HeaderText = "Примечание";
            DataGrid_Shipment_Table.Columns["COUNT_PALLET_ROS"].HeaderText = "Загружено россыпью (короб)";
            DataGrid_Shipment_Table.Columns["COUNT_PALLET"].HeaderText = "Загружено паллет";
            DataGrid_Shipment_Table.Columns["COMPLETION_LOADING_ATS_DATE"].HeaderText = "Дата окончания погрузки АТС";
            DataGrid_Shipment_Table.Columns["COMPLETION_LOADING_ATS_TIME"].HeaderText = "Время окончания погрузки АТС";
            DataGrid_Shipment_Table.Columns["COUNT_ALL_PALLET"].HeaderText = "Всего паллет к отгрузке ";
            DataGrid_Shipment_Table.Columns["COUNT_MIX_PALLET"].HeaderText = "Микспаллет";
            DataGrid_Shipment_Table.Columns["COUNT_TERM_BOX"].HeaderText = "Термолабильный товар (короб)";
            DataGrid_Shipment_Table.Columns["COUNT_TERM_PALLET"].HeaderText = "Термолабильный товар (паллет)";
            DataGrid_Shipment_Table.Columns["ACTUAL_ARRIVAL_ATS_DATE"].HeaderText = "Фактическая дата прибытия АТС";
            DataGrid_Shipment_Table.Columns["ACTUAL_ARRIVAL_ATS_TIME"].HeaderText = "Фактическое время прибытия АТС";
            DataGrid_Shipment_Table.Columns["ACTUAL_ONDOCK_ATS_DATE"].HeaderText = "Дата постановки на док АТС";
            DataGrid_Shipment_Table.Columns["ACTUAL_ONDOCK_ATS_TIME"].HeaderText = "Время постановки на док АТС";
            DataGrid_Shipment_Table.Columns["PLAN_SHIPMEN_DATE"].HeaderText = "Плановая дата отгрузки";
            DataGrid_Shipment_Table.Columns["PLAN_SHIPMEN_TIME"].HeaderText = "Плановое время отгрузки";
            DataGrid_Shipment_Table.Columns["FACT_SHIPMENT_DATE"].HeaderText = "Фактическая дата отгрузки";
            DataGrid_Shipment_Table.Columns["FACT_SHIPMENT_TIME"].HeaderText = "Фактическое время отгрузки";
            DataGrid_Shipment_Table.Columns["COMPANY"].HeaderText = "Компания";
            DataGrid_Shipment_Table.Columns["WAREHOUSE"].HeaderText = "Склад";
            DataGrid_Shipment_Table.Columns["COUNT_ALL_BOX"].HeaderText = "Всего коробов";
            DataGrid_Shipment_Table.Columns["description"].HeaderText = "Контролер";

            DataGrid_Shipment_Table.SetFilterAndSortEnabled(this.DataGrid_Shipment_Table.Columns["TYPE"], false);
            DataGrid_Shipment_Table.Columns["USER_DEF8"].Visible = false;
            DataGrid_Shipment_Table.Columns["FACT_SHIPMENT_DATE"].Visible = false;
            DataGrid_Shipment_Table.Columns["FACT_SHIPMENT_TIME"].Visible = false;

            if (string.IsNullOrEmpty(Sort_Shipment_string))
            {
                DataGrid_Shipment_Table.Sort(DataGrid_Shipment_Table.Columns["TID"], ListSortDirection.Descending);
            }
        }

        public  void Load_Service_Table()
        {
            Class_SQL.LoadGridDB(DataGrid_Service_Table, "select TID, TYPE, COMPANY, WAREHOUSE, ORDER_NUMBER, OSNOVANIE, STATUS, isnull(TOTAL_LINES, 0) as TOTAL_LINES, RECEIPT_DATE, " +
                                                         " RECEIPT_TIME, DAY_WEEK, ADD_DATE, ADD_TIME, MESSANGER from dbo.V_ABS_ORDER_SERVICE where com_code " +
                                                         " in (" + SelectCompany + ") " +
                                                         " and war_code in ("+ SelectWarehouse+ ") " + Serch_Service_string + Sort_Service_string );
                       
            DataGrid_Service_Table.Columns["TYPE"].HeaderText = "Тип документа";
            DataGrid_Service_Table.Columns["COMPANY"].HeaderText = "Компания";
            DataGrid_Service_Table.Columns["WAREHOUSE"].HeaderText = "Склад";
            DataGrid_Service_Table.Columns["ORDER_NUMBER"].HeaderText = "Номер заявки";
            DataGrid_Service_Table.Columns["OSNOVANIE"].HeaderText = "Основание";
            DataGrid_Service_Table.Columns["STATUS"].HeaderText = "Статус";
            DataGrid_Service_Table.Columns["TOTAL_LINES"].HeaderText = "Кол-во строк";
            DataGrid_Service_Table.Columns["RECEIPT_DATE"].HeaderText = "Дата получения запроса";
            DataGrid_Service_Table.Columns["RECEIPT_TIME"].HeaderText = "Время получения запроса";
            DataGrid_Service_Table.Columns["DAY_WEEK"].HeaderText = "День недели";
            DataGrid_Service_Table.Columns["ADD_DATE"].HeaderText = "Дата создания запроса";
            DataGrid_Service_Table.Columns["ADD_TIME"].HeaderText = "Время создания запроса";
            DataGrid_Service_Table.Columns["MESSANGER"].HeaderText = "Примечание";

            DataGrid_Service_Table.Columns["TID"].Visible = false;

            DataGrid_Service_Table.SetFilterAndSortEnabled(DataGrid_Service_Table.Columns["TYPE"], false);
            if (string.IsNullOrEmpty(Sort_Service_string))
            {
                DataGrid_Service_Table.Sort(DataGrid_Service_Table.Columns["TID"], ListSortDirection.Descending);
            }
            
        }

        private void SelectReceiptTable()
        {

            if (DataGrid_Receipt_Table.SelectedRows.Count > 0)
            {

                //Console.WriteLine("select test company = " + DataGrid_Receipt_Table.CurrentRow.Cells ["Company"].Value.ToString());


                TextBox_Receipt_OrderType.Text = DataGrid_Receipt_Table.CurrentRow.Cells["TYPE"].Value.ToString();
                ComboBox_Receipt_Status.SelectedItem = DataGrid_Receipt_Table.CurrentRow.Cells["STATUS"].Value.ToString();
                TextBox_Receipt_Warehouse.Text = DataGrid_Receipt_Table.CurrentRow.Cells["WAREHOUSE"].Value.ToString();
                TextBox_Receipt_Company.Text = DataGrid_Receipt_Table.CurrentRow.Cells["COMPANY"].Value.ToString();
                TextBox_Receipt_OrderNumber.Text = DataGrid_Receipt_Table.CurrentRow.Cells["ORDER_NUMBER"].Value.ToString();
                TextBox_Receipt_OrderErp.Text = DataGrid_Receipt_Table.CurrentRow.Cells["ERP_NUMBER"].Value.ToString();
                TextBox_Receipt_GrusName.Text = DataGrid_Receipt_Table.CurrentRow.Cells["GRUZNAME"].Value.ToString();
                TextBox_Receipt_Invoice.Text = DataGrid_Receipt_Table.CurrentRow.Cells["INVOICE"].Value.ToString();

                if (DataGrid_Receipt_Table.CurrentRow.Cells["RECEIPT_DATE"].Value.ToString() != null)
                {

                    DatePicker_Receipt_Date_Create.Value = Convert.ToDateTime(DataGrid_Receipt_Table.CurrentRow.Cells["RECEIPT_DATE"].Value.ToString());
                    DatePicker_Receipt_Time_Create.Text = DataGrid_Receipt_Table.CurrentRow.Cells["RECEIPT_TIME"].Value.ToString();
                }
                Label_Receipt_CreateWeekDay.Text = DataGrid_Receipt_Table.CurrentRow.Cells["DAY_WEEK"].Value.ToString();
                TextBox_Receipt_CountStrok.Text = DataGrid_Receipt_Table.CurrentRow.Cells["TOTAL_LINES"].Value.ToString();


                if (!(string.IsNullOrEmpty(DataGrid_Receipt_Table.SelectedCells[13].Value.ToString())))
                {

                    DatePicker_Receipt_Date_Plan_Arrival_Ats.Value = Convert.ToDateTime(DataGrid_Receipt_Table.SelectedCells[13].Value.ToString());
                    DatePicker_Receipt_Time_Plan_Arrival_Ats.Text = DataGrid_Receipt_Table.SelectedCells[14].Value.ToString();
                }
                else
                {
                    DatePicker_Receipt_Date_Plan_Arrival_Ats.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Receipt_Time_Plan_Arrival_Ats.Text = "00:00:00";

                }

                TextBox_Receipt_CountBoxRos.Text = DataGrid_Receipt_Table.CurrentRow.Cells["COUNT_BOX_ROS"].Value.ToString();
                TextBox_Receipt_CountPallet.Text = DataGrid_Receipt_Table.CurrentRow.Cells["COUNT_PALLET"].Value.ToString();

                if (!(string.IsNullOrEmpty(DataGrid_Receipt_Table.SelectedCells[17].Value.ToString())))
                {
                    DatePicker_Receipt_Date_Fact_Arrival_Ats.Value = Convert.ToDateTime(DataGrid_Receipt_Table.SelectedCells[17].Value.ToString());
                    DatePicker_Receipt_Time_Fact_Arrival_Ats.Text = DataGrid_Receipt_Table.SelectedCells[18].Value.ToString();
                }
                else
                {
                    DatePicker_Receipt_Date_Fact_Arrival_Ats.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Receipt_Time_Fact_Arrival_Ats.Text = "00:00:00";
                }

                if (!(string.IsNullOrEmpty(DataGrid_Receipt_Table.SelectedCells[19].Value.ToString())))
                {
                    DatePicker_Receipt_Date_Actual_onDock_Ats.Value = Convert.ToDateTime(DataGrid_Receipt_Table.SelectedCells[19].Value.ToString());
                    DatePicker_Receipt_Time_Actual_onDock_Ats.Text = DataGrid_Receipt_Table.SelectedCells[20].Value.ToString();
                }
                else
                {
                    DatePicker_Receipt_Date_Actual_onDock_Ats.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Receipt_Time_Actual_onDock_Ats.Text = "00:00:00";
                }

                if (!(string.IsNullOrEmpty(DataGrid_Receipt_Table.SelectedCells[21].Value.ToString())))
                {
                    DatePicker_Receipt_Date_Completion_Discharge_Ats.Value = Convert.ToDateTime(DataGrid_Receipt_Table.SelectedCells[21].Value.ToString());
                    DatePicker_Receipt_Time_Completion_Discharge_Ats.Text = DataGrid_Receipt_Table.SelectedCells[22].Value.ToString();
                }
                else
                {
                    DatePicker_Receipt_Date_Completion_Discharge_Ats.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Receipt_Time_Completion_Discharge_Ats.Text = "00:00:00";
                }

                if (!(string.IsNullOrEmpty(DataGrid_Receipt_Table.SelectedCells[23].Value.ToString())))
                {
                    DatePicker_Receipt_Date_Acceptance.Value = Convert.ToDateTime(DataGrid_Receipt_Table.SelectedCells[23].Value.ToString());
                    DatePicker_Receipt_Time_Acceptance.Text = DataGrid_Receipt_Table.SelectedCells[24].Value.ToString();
                }
                else
                {
                    DatePicker_Receipt_Date_Acceptance.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Receipt_Time_Acceptance.Text = "00:00:00";
                }


                TextBox_Receipt_Messanger.Text = DataGrid_Receipt_Table.SelectedCells[26].Value.ToString();
                ComboBox_Receipt_Users.SelectedValue = DataGrid_Receipt_Table.SelectedCells[27].Value.ToString();

                Load_Receipt_Dop(DataGrid_Receipt_Table.SelectedCells[5].Value.ToString(), Convert.ToDecimal(DataGrid_Receipt_Table.CurrentRow.Cells["USER_DEF8"].Value));


                

                    if ((DataGrid_Receipt_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Выполнено" ||
                         DataGrid_Receipt_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Удалено" ||
                         DataGrid_Receipt_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Расформировано") && (strUrovenDostupa !=1)
                    )
                {
                    TextBox_Receipt_GrusName.Enabled = false;
                    TextBox_Receipt_Invoice.Enabled = false;
                    TextBox_Receipt_CountStrok.Enabled = false;
                    ComboBox_Receipt_Status.Enabled = false;
                    DatePicker_Receipt_Date_Plan_Arrival_Ats.Enabled = false;
                    DatePicker_Receipt_Time_Plan_Arrival_Ats.Enabled = false;
                    DatePicker_Receipt_Date_Fact_Arrival_Ats.Enabled = false;
                    DatePicker_Receipt_Time_Fact_Arrival_Ats.Enabled = false;
                    DatePicker_Receipt_Date_Actual_onDock_Ats.Enabled = false;
                    DatePicker_Receipt_Time_Actual_onDock_Ats.Enabled = false;
                    DatePicker_Receipt_Date_Completion_Discharge_Ats.Enabled = false;
                    DatePicker_Receipt_Time_Completion_Discharge_Ats.Enabled = false;
                    TextBox_Receipt_CountPallet.Enabled = false;
                    TextBox_Receipt_CountBoxRos.Enabled = false;
                    DatePicker_Receipt_Date_Acceptance.Enabled = false;
                    DatePicker_Receipt_Time_Acceptance.Enabled = false;
                    ComboBox_Receipt_Users.Enabled = false;
                    TextBox_Receipt_Messanger.Enabled = false;
                    Button_Receipt_Save.Enabled = false;
                    ComboBox_Receipt_DopRazdel.Enabled = false;
                    ComboBox_Receipt_DopRazdel_Uroven2.Enabled = false;
                    TextBox_Receipt_DopRazdel_Qty.Enabled = false;
                    ComboBox_Receipt_DopRazdel_Name.Enabled = false;
                    ComboBox_Receipt_DopRazdel_Uroven3.Enabled = false;
                    ComboBox_Receipt_DopRazdel_En.Enabled = false;
                    Button_Receipt_DopRazdel_Add.Enabled = false;
                    Button_Receipt_DopRazdel_Del.Enabled = false;
                }
                else
                {
                    TextBox_Receipt_GrusName.Enabled = true;
                    TextBox_Receipt_Invoice.Enabled = true;
                    TextBox_Receipt_CountStrok.Enabled = true;
                    ComboBox_Receipt_Status.Enabled = true;
                    DatePicker_Receipt_Date_Plan_Arrival_Ats.Enabled = true;
                    DatePicker_Receipt_Time_Plan_Arrival_Ats.Enabled = true;
                    DatePicker_Receipt_Date_Fact_Arrival_Ats.Enabled = true;
                    DatePicker_Receipt_Time_Fact_Arrival_Ats.Enabled = true;
                    DatePicker_Receipt_Date_Actual_onDock_Ats.Enabled = true;
                    DatePicker_Receipt_Time_Actual_onDock_Ats.Enabled = true;
                    DatePicker_Receipt_Date_Completion_Discharge_Ats.Enabled = true;
                    DatePicker_Receipt_Time_Completion_Discharge_Ats.Enabled = true;
                    TextBox_Receipt_CountPallet.Enabled = true;
                    TextBox_Receipt_CountBoxRos.Enabled = true;
                    DatePicker_Receipt_Date_Acceptance.Enabled = true;
                    DatePicker_Receipt_Time_Acceptance.Enabled = true;
                    ComboBox_Receipt_Users.Enabled = true;
                    TextBox_Receipt_Messanger.Enabled = true;
                    Button_Receipt_Save.Enabled = true;
                    ComboBox_Receipt_DopRazdel.Enabled = true;
                    ComboBox_Receipt_DopRazdel_Uroven2.Enabled = true;
                    TextBox_Receipt_DopRazdel_Qty.Enabled = true;
                    ComboBox_Receipt_DopRazdel_Name.Enabled = true;
                    ComboBox_Receipt_DopRazdel_Uroven3.Enabled = true;
                    ComboBox_Receipt_DopRazdel_En.Enabled = true;
                    Button_Receipt_DopRazdel_Add.Enabled = true;
                    Button_Receipt_DopRazdel_Del.Enabled = true;
                }
            }
        }

        private void SelectShipmentTable()
        {
            if ( DataGrid_Shipment_Table.SelectedRows.Count>0)
            {
                TextBox_Shipment_OrderType.Text = DataGrid_Shipment_Table.SelectedCells[1].Value.ToString();
                TextBox_Shipment_OrderNumber.Text = DataGrid_Shipment_Table.SelectedCells[2].Value.ToString();
                TextBox_Shipment_OrderErp.Text = DataGrid_Shipment_Table.SelectedCells[3].Value.ToString();
                TextBox_Shipment_GruzName.Text = DataGrid_Shipment_Table.SelectedCells[4].Value.ToString();
                ComboBox_Shipment_Status.SelectedItem = DataGrid_Shipment_Table.SelectedCells[5].Value.ToString();
                TextBox_Shipment_CountStrok.Text = DataGrid_Shipment_Table.SelectedCells[6].Value.ToString();
                TextBox_Shipment_Company.Text = DataGrid_Shipment_Table.SelectedCells[7].Value.ToString();
                TextBox_Shipment_Warehouse.Text = DataGrid_Shipment_Table.SelectedCells[8].Value.ToString();

                if (!string.IsNullOrEmpty(this.DataGrid_Shipment_Table.SelectedCells[9].Value.ToString()))
                {
                    DatePicker_Shipment_Date_Create.Value = Convert.ToDateTime(DataGrid_Shipment_Table.SelectedCells[9].Value.ToString());
                    DatePicker_Shipment_Time_Create.Text = DataGrid_Shipment_Table.SelectedCells[10].Value.ToString();
                }
                else
                {
                    DatePicker_Shipment_Date_Create.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Shipment_Time_Create.Text = "00:00:00";
                }

                Label_Shipment_CreateWeekDay.Text = DataGrid_Shipment_Table.SelectedCells[11].Value.ToString();
                TextBox_Shipment_Messanger.Text = DataGrid_Shipment_Table.SelectedCells[12].Value.ToString();

                TextBox_Shipment_Count_Pallet_Ros.Text = DataGrid_Shipment_Table.SelectedCells[14].Value.ToString();
                TextBox_Shipment_Count_Pallet.Text = DataGrid_Shipment_Table.SelectedCells[15].Value.ToString();

                if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.SelectedCells[16].Value.ToString()))
                {
                    DatePicker_Shipment_Completion_Loading_Ats_Date.Value = Convert.ToDateTime(DataGrid_Shipment_Table.SelectedCells[16].Value.ToString());
                    DatePicker_Shipment_Completion_Loading_Ats_Time.Text = DataGrid_Shipment_Table.SelectedCells[17].Value.ToString();
                }
                else
                {
                    DatePicker_Shipment_Completion_Loading_Ats_Date.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Shipment_Completion_Loading_Ats_Time.Text = "00:00:00";
                }


                TextBox_Shipment_Count_All_Pallet.Text = DataGrid_Shipment_Table.SelectedCells[18].Value.ToString();
                TextBox_Shipment_Count_All_Box.Text = DataGrid_Shipment_Table.SelectedCells[19].Value.ToString();
                TextBox_Shipment_Count_Mix_Pallet.Text = DataGrid_Shipment_Table.SelectedCells[20].Value.ToString();
                TextBox_Shipment_Count_Term_Box.Text = DataGrid_Shipment_Table.SelectedCells[21].Value.ToString();


                if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.SelectedCells[22].Value.ToString()))
                {

                    DatePicker_Shipment_Actual_Arrival_Ats_Date.Value = Convert.ToDateTime(DataGrid_Shipment_Table.SelectedCells[22].Value.ToString());
                    DatePicker_Shipment_Actual_Arrival_Ats_Time.Text = DataGrid_Shipment_Table.SelectedCells[23].Value.ToString();
                }
                else
                {
                    DatePicker_Shipment_Actual_Arrival_Ats_Date.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Shipment_Actual_Arrival_Ats_Time.Text = "00:00:00";
                }

                if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.SelectedCells[24].Value.ToString()))
                {
                    DatePicker_Shipment_Actual_OnDock_Ats_Date.Value = Convert.ToDateTime(DataGrid_Shipment_Table.SelectedCells[24].Value.ToString());
                    DatePicker_Shipment_Actual_OnDock_Ats_Time.Text = DataGrid_Shipment_Table.SelectedCells[25].Value.ToString();
                }
                else
                {
                    DatePicker_Shipment_Actual_OnDock_Ats_Date.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Shipment_Actual_OnDock_Ats_Time.Text = "00:00:00";
                }

                if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.SelectedCells[26].Value.ToString()))
                {
                    DatePicker_Shipment_Plan_Shipment_Date.Value = Convert.ToDateTime(DataGrid_Shipment_Table.SelectedCells[26].Value.ToString());
                    DatePicker_Shipment_Plan_Shipment_Time.Text = DataGrid_Shipment_Table.SelectedCells[27].Value.ToString();
                }
                else
                {
                    DatePicker_Shipment_Plan_Shipment_Date.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Shipment_Plan_Shipment_Time.Text = "00:00:00";
                }



                if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.SelectedCells[28].Value.ToString()))
                {
                    DatePicker_Shipment_Fact_Shipment_Date.Value = Convert.ToDateTime(DataGrid_Shipment_Table.SelectedCells[28].Value.ToString());
                    DatePicker_Shipment_Fact_Shipment_Time.Text = DataGrid_Shipment_Table.SelectedCells[29].Value.ToString();
                }
                else
                {
                    DatePicker_Shipment_Fact_Shipment_Date.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Shipment_Fact_Shipment_Time.Text = "00:00:00";
                }
;
                TextBox_Shipment_Count_Term_Pallet.Text = DataGrid_Shipment_Table.SelectedCells[30].Value.ToString();

                ComboBox_Shipment_Users.SelectedValue = DataGrid_Shipment_Table.CurrentRow.Cells["USERS"].Value.ToString();

                if ((DataGrid_Shipment_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Выполнено" ||
                     DataGrid_Shipment_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Удалено" ||
                     DataGrid_Shipment_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Расформировано") && (strUrovenDostupa != 1))
                {
                    TextBox_Shipment_CountStrok.Enabled = false;
                    ComboBox_Shipment_Status.Enabled = false;
                    DatePicker_Shipment_Plan_Shipment_Date.Enabled = false;
                    DatePicker_Shipment_Plan_Shipment_Time.Enabled = false;
                    TextBox_Shipment_Count_Term_Pallet.Enabled = false;
                    TextBox_Shipment_Count_Mix_Pallet.Enabled = false;
                    TextBox_Shipment_Count_All_Pallet.Enabled = false;
                    TextBox_Shipment_Count_All_Box.Enabled = false;
                    ComboBox_Shipment_Users.Enabled = false;
                    DatePicker_Shipment_Fact_Shipment_Date.Enabled = false;
                    DatePicker_Shipment_Fact_Shipment_Time.Enabled = false;
                    DatePicker_Shipment_Actual_OnDock_Ats_Date.Enabled = false;
                    DatePicker_Shipment_Actual_OnDock_Ats_Time.Enabled = false;
                    TextBox_Shipment_Count_Pallet.Enabled = false;
                    DatePicker_Shipment_Actual_Arrival_Ats_Date.Enabled = false;
                    DatePicker_Shipment_Actual_Arrival_Ats_Time.Enabled = false;
                    DatePicker_Shipment_Completion_Loading_Ats_Date.Enabled = false;
                    DatePicker_Shipment_Completion_Loading_Ats_Time.Enabled = false;
                    TextBox_Shipment_Count_Pallet_Ros.Enabled = false;
                    TextBox_Shipment_Messanger.Enabled = false;
                    Button_Shipment_Save.Enabled = false;
                    ComboBox_Shipment_DopRazdel.Enabled = false;
                    ComboBox_Shipment_DopRazdel_Uroven2.Enabled = false;
                    TextBox_Shipment_DopRazdel_Qty.Enabled = false;
                    ComboBox_Shipment_DopRazdel_Name.Enabled = false;
                    ComboBox_Shipment_DopRazdel_Uroven3.Enabled = false;
                    ComboBox_Shipment_DopRazdel_En.Enabled = false;
                    Button_Shipment_DopRazdel_Add.Enabled = false;
                    Button_Shipment_DopRazdel_Del.Enabled = false;
                    TextBox_Shipment_Count_Term_Pallet.Enabled = false;
                }
                else
                {
                    TextBox_Shipment_CountStrok.Enabled = true;
                    ComboBox_Shipment_Status.Enabled = true;
                    DatePicker_Shipment_Plan_Shipment_Date.Enabled = true;
                    DatePicker_Shipment_Plan_Shipment_Time.Enabled = true;
                    TextBox_Shipment_Count_Term_Pallet.Enabled = true;
                    TextBox_Shipment_Count_Mix_Pallet.Enabled = true;
                    TextBox_Shipment_Count_All_Pallet.Enabled = true;
                    TextBox_Shipment_Count_All_Box.Enabled = true;
                    ComboBox_Shipment_Users.Enabled = true;
                    DatePicker_Shipment_Fact_Shipment_Date.Enabled = true;
                    DatePicker_Shipment_Fact_Shipment_Time.Enabled = true;
                    DatePicker_Shipment_Actual_OnDock_Ats_Date.Enabled = true;
                    DatePicker_Shipment_Actual_OnDock_Ats_Time.Enabled = true;
                    TextBox_Shipment_Count_Pallet.Enabled = true;
                    DatePicker_Shipment_Actual_Arrival_Ats_Date.Enabled = true;
                    DatePicker_Shipment_Actual_Arrival_Ats_Time.Enabled = true;
                    DatePicker_Shipment_Completion_Loading_Ats_Date.Enabled = true;
                    DatePicker_Shipment_Completion_Loading_Ats_Time.Enabled = true;
                    TextBox_Shipment_Count_Pallet_Ros.Enabled = true;
                    TextBox_Shipment_Messanger.Enabled = true;
                    Button_Shipment_Save.Enabled = true;
                    ComboBox_Shipment_DopRazdel.Enabled = true;
                    ComboBox_Shipment_DopRazdel_Uroven2.Enabled = true;
                    TextBox_Shipment_DopRazdel_Qty.Enabled = true;
                    ComboBox_Shipment_DopRazdel_Name.Enabled = true;
                    ComboBox_Shipment_DopRazdel_Uroven3.Enabled = true;
                    ComboBox_Shipment_DopRazdel_En.Enabled = true;
                    Button_Shipment_DopRazdel_Add.Enabled = true;
                    Button_Shipment_DopRazdel_Del.Enabled = true;
                    TextBox_Shipment_Count_Term_Pallet.Enabled = true;
                }

                Load_Shipment_Dop(DataGrid_Shipment_Table.CurrentRow.Cells["ORDER_NUMBER"].Value.ToString(),Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["USER_DEF8"].Value.ToString()) );
            }
        }

        private void SelectServiceTable()
        {
            TextBox_Service_OrderType.Text = null;
            TextBox_Service_Company.Text = null;
            TextBox_Service_Warehouse.Text = null;
            TextBox_Service_OrderNumber.Text = null;
            TextBox_Service_GruzName.Text = null;

            TextBox_Service_CountStrok.Text = "0";

            Label_Service_CreateWeekDay.Text = null;
            txt_Service_Messanger.Text = null;


            if ( !string.IsNullOrEmpty(DataGrid_Service_Table.CurrentRow.Cells["TID"].Value.ToString()))
            {
                TextBox_Service_OrderType.Text = DataGrid_Service_Table.CurrentRow.Cells["TYPE"].Value.ToString();
                TextBox_Service_Company.Text = DataGrid_Service_Table.CurrentRow.Cells["COMPANY"].Value.ToString();
                TextBox_Service_Warehouse.Text = DataGrid_Service_Table.CurrentRow.Cells["WAREHOUSE"].Value.ToString();
                TextBox_Service_OrderNumber.Text = DataGrid_Service_Table.CurrentRow.Cells["ORDER_NUMBER"].Value.ToString();
                TextBox_Service_GruzName.Text = DataGrid_Service_Table.CurrentRow.Cells["OSNOVANIE"].Value.ToString();

                if (!string.IsNullOrEmpty(DataGrid_Service_Table.CurrentRow.Cells["STATUS"].Value.ToString()))
                {
                    ComboBox_Service_Status.SelectedItem = DataGrid_Service_Table.CurrentRow.Cells["STATUS"].Value.ToString();
                }
                else
                {
                    ComboBox_Service_Status.SelectedItem = "Зарегистрировано";
                }

                TextBox_Service_CountStrok.Text = DataGrid_Service_Table.CurrentRow.Cells["TOTAL_LINES"].Value.ToString();

                if (!string.IsNullOrEmpty(DataGrid_Service_Table.SelectedCells[8].Value.ToString()))
                {
                    DatePicker_Service_Date_Receiving.Value = Convert.ToDateTime(DataGrid_Service_Table.SelectedCells[8].Value.ToString());
                    DatePicker_Service_Time_Receiving.Text = DataGrid_Service_Table.SelectedCells[9].Value.ToString();
                }
                else
                {
                    DatePicker_Service_Date_Receiving.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Service_Time_Receiving.Text = "00:00:00";
                }

                Label_Service_CreateWeekDay.Text = DataGrid_Service_Table.CurrentRow.Cells["DAY_WEEK"].Value.ToString();

                if (!string.IsNullOrEmpty(DataGrid_Service_Table.SelectedCells[11].Value.ToString()))
                {
                    DatePicker_Service_Date_Create.Value = Convert.ToDateTime(DataGrid_Service_Table.SelectedCells[11].Value.ToString());
                    DatePicker_Service_Time_Create.Text = DataGrid_Service_Table.SelectedCells[12].Value.ToString();

                }
                else
                {
                    DatePicker_Service_Date_Create.Value = Convert.ToDateTime(DateTime.Now);
                    DatePicker_Service_Time_Create.Text = "00:00:00";
                }

                txt_Service_Messanger.Text = DataGrid_Service_Table.CurrentRow.Cells["MESSANGER"].Value.ToString();

                Load_Service_Dop(DataGrid_Service_Table.CurrentRow .Cells["ORDER_NUMBER"].Value.ToString());


                if ((DataGrid_Service_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Выполнено" ||
                     DataGrid_Service_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Удалено" ||
                     DataGrid_Service_Table.CurrentRow.Cells["STATUS"].Value.ToString() == "Расформировано") && (strUrovenDostupa != 1))
                {
                    TextBox_Service_CountStrok.Enabled = false;
                    ComboBox_Service_Status.Enabled = false;
                    DatePicker_Service_Date_Receiving.Enabled = false;
                    DatePicker_Service_Time_Receiving.Enabled = false;
                    ComboBox_Service_DopRazdel.Enabled = false;
                    ComboBox_Service_DopRazdel_Uroven2.Enabled = false;
                    TextBox_Service_DopRazdel_Qty.Enabled = false;
                    ComboBox_Service_DopRazdel_Name.Enabled = false;
                    ComboBox_Service_DopRazdel_Uroven3.Enabled = false;
                    ComboBox_Service_DopRazdel_En.Enabled = false;
                    Button_Service_DopRazdel_Add.Enabled = false;
                    Button_Service_DopRazdel_Del.Enabled = false;
                    Button_Service_Save.Enabled = false;
                    txt_Service_Messanger.Enabled = false;

                }
                else
                {
                    TextBox_Service_CountStrok.Enabled = true;
                    ComboBox_Service_Status.Enabled = true;
                    DatePicker_Service_Date_Receiving.Enabled = true;
                    DatePicker_Service_Time_Receiving.Enabled = true;
                    ComboBox_Service_DopRazdel.Enabled = true;
                    ComboBox_Service_DopRazdel_Uroven2.Enabled = true;
                    TextBox_Service_DopRazdel_Qty.Enabled = true;
                    ComboBox_Service_DopRazdel_Name.Enabled = true;
                    ComboBox_Service_DopRazdel_Uroven3.Enabled = true;
                    ComboBox_Service_DopRazdel_En.Enabled = true;
                    Button_Service_DopRazdel_Add.Enabled = true;
                    Button_Service_DopRazdel_Del.Enabled = true;
                    Button_Service_Save.Enabled = true;
                    txt_Service_Messanger.Enabled = true;

                }
            }
        }

        private void Load_Receipt_Dop(string Number, decimal UserDef)
        {
            Class_SQL.LoadGridDB(GridView_Receipt_Dop, "select  INTERNAL_NUMBER, RAZDEL, DESCRIPTION, UROVEN_2, UROVEN_3, cast(QTY as decimal (18, 0)) as 'QTY', EN  from dbo.ABS_ORDER_DOPOLN_OPERATION (nolock) where ORDERNUMBER = N'" + Number + "' and user_def8 = " + UserDef);

            GridView_Receipt_Dop.Columns["RAZDEL"].HeaderText = "Раздел";
            GridView_Receipt_Dop.Columns["DESCRIPTION"].HeaderText = "Наименование";
            GridView_Receipt_Dop.Columns["UROVEN_2"].HeaderText = "Уровень 2";
            GridView_Receipt_Dop.Columns["UROVEN_3"].HeaderText = "Уровень 3";
            GridView_Receipt_Dop.Columns["QTY"].HeaderText = "Количество";
            GridView_Receipt_Dop.Columns["EN"].HeaderText = "Ед. Измерения";

            GridView_Receipt_Dop.Columns["INTERNAL_NUMBER"].Visible = false;
            GridView_Receipt_Dop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void Load_Shipment_Dop(string Number, decimal UserDef)
        {
            Class_SQL.LoadGridDB(GridView_Shipment_Dop, "select  INTERNAL_NUMBER, RAZDEL, DESCRIPTION, UROVEN_2, UROVEN_3, cast(QTY as decimal (18, 0)) as 'QTY', EN  from dbo.ABS_ORDER_DOPOLN_OPERATION (nolock) where ORDERNUMBER = N'" + Number + "' and user_def8 =" + UserDef);

            GridView_Shipment_Dop.Columns["RAZDEL"].HeaderText = "Раздел";
            GridView_Shipment_Dop.Columns["DESCRIPTION"].HeaderText = "Наименование";
            GridView_Shipment_Dop.Columns["UROVEN_2"].HeaderText = "Уровень 2";
            GridView_Shipment_Dop.Columns["UROVEN_3"].HeaderText = "Уровень 3";
            GridView_Shipment_Dop.Columns["QTY"].HeaderText = "Количество";
            GridView_Shipment_Dop.Columns["EN"].HeaderText = "Ед. Измерения";

            GridView_Shipment_Dop.Columns["INTERNAL_NUMBER"].Visible = false;
            GridView_Shipment_Dop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void Load_Service_Dop(string Number)
        {
            Class_SQL.LoadGridDB(GridView_Service_Dop, "select  INTERNAL_NUMBER, RAZDEL, DESCRIPTION, UROVEN_2, UROVEN_3, cast(QTY as decimal (18, 0)) as 'QTY', EN  from dbo.ABS_ORDER_DOPOLN_OPERATION (nolock) where ORDERNUMBER = N'" + Number + "'");

            GridView_Service_Dop.Columns["RAZDEL"].HeaderText = "Раздел";
            GridView_Service_Dop.Columns["DESCRIPTION"].HeaderText = "Наименование";
            GridView_Service_Dop.Columns["UROVEN_2"].HeaderText = "Уровень 2";
            GridView_Service_Dop.Columns["UROVEN_3"].HeaderText = "Уровень 3";
            GridView_Service_Dop.Columns["QTY"].HeaderText = "Количество";
            GridView_Service_Dop.Columns["EN"].HeaderText = "Ед. Измерения";

            GridView_Service_Dop.Columns["INTERNAL_NUMBER"].Visible = false;
            GridView_Service_Dop.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }      

        private void advancedDataGridViewSearchToolBar_main_Search(object sender, Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            bool restartsearch = true;
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = DataGrid_Receipt_Table.CurrentCell.ColumnIndex + 1 >= DataGrid_Receipt_Table.ColumnCount;
                bool endrow = DataGrid_Receipt_Table.CurrentCell.RowIndex + 1 >= DataGrid_Receipt_Table.RowCount;

                if (endcol && endrow)
                {
                    startColumn = DataGrid_Receipt_Table.CurrentCell.ColumnIndex;
                    startRow = DataGrid_Receipt_Table.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : DataGrid_Receipt_Table.CurrentCell.ColumnIndex + 1;
                    startRow = DataGrid_Receipt_Table.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }

           
            DataGridViewCell c = DataGrid_Receipt_Table.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);

            if (c == null && restartsearch)
                c = DataGrid_Receipt_Table.FindCell(
                    e.ValueToSearch,
                    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                    0,
                    0,
                    e.WholeWord,
                    e.CaseSensitive);
            if (c != null)
                DataGrid_Receipt_Table.CurrentCell = c;


        }

        private void File_Exit_Click(object sender, EventArgs e)
        {
            Class_SQL.CloseConnection();
            Application.Exit();
        }

        private void DataGrid_Receipt_Table_FilterStringChanged(object sender, Zuby.ADGV.AdvancedDataGridView.FilterEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.FilterString))
            {
                e.FilterString = " and " + e.FilterString;
                e.FilterString = e.FilterString.Replace("('", "(N'");
                e.FilterString = e.FilterString.Replace("', '", "', N'");
                Serch_receipt_string = Serch_receipt_string + ' ' + e.FilterString;
            }
            
            Load_Receipt_Table();

            if (!string.IsNullOrEmpty(TextBox_Receipt_OrderNumber.Text))
            {
                Clear_Receipt();
            }
        }

        private void Button_Receipt_Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox_Receipt_OrderNumber.Text))
            {
                if (ComboBox_Receipt_Status.SelectedItem.ToString() == "Выполнено")
                {
                    if (DatePicker_Receipt_Time_Plan_Arrival_Ats.Text != "00:00")
                    {
                        if (DatePicker_Receipt_Time_Fact_Arrival_Ats.Text != "00:00")
                        {
                            if (DatePicker_Receipt_Time_Actual_onDock_Ats.Text != "00:00")
                            {
                                if (DatePicker_Receipt_Time_Completion_Discharge_Ats.Text != "00:00")
                                {
                                    if (DatePicker_Receipt_Time_Acceptance.Text != "00:00")
                                    {
                                        if (Convert.ToInt32(TextBox_Receipt_CountPallet.Text) != 0)
                                        {
                                            ReceiptSave();
                                        }
                                        else
                                        {
                                            if (MessageBox.Show("Уверены что в заявке № " + TextBox_Receipt_OrderNumber.Text + " нет Выгруженных паллет?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                            {
                                                ReceiptSave();
                                            }
                                            else
                                            {
                                                TextBox_Receipt_CountPallet.Focus();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Поле Дата и время окончания приемки не заполнено.", "ABS", MessageBoxButtons.OK);
                                        DatePicker_Receipt_Time_Acceptance.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Поле Время окончания выгрузки АТС не заполнено.", "ABS", MessageBoxButtons.OK);
                                    DatePicker_Receipt_Time_Completion_Discharge_Ats.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Поле Фактическая дата и время постановки на док не заполнено.", "ABS", MessageBoxButtons.OK);
                                DatePicker_Receipt_Time_Actual_onDock_Ats.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Поле Фактическая дата и время прихода АТС не заполнено.", "ABS", MessageBoxButtons.OK);
                            DatePicker_Receipt_Time_Fact_Arrival_Ats.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле Плановая дата и время прихода АТС не заполнено.", "ABS", MessageBoxButtons.OK);
                        DatePicker_Receipt_Time_Plan_Arrival_Ats.Focus();
                    }
                }
                else
                {
                    ReceiptSave();
                }
            }
            else
            {
                MessageBox.Show("Поле номер документа не заполнено.", "ABS", MessageBoxButtons.OK);
            }
        }

        private void ReceiptSave()
        {
            string strUsers = null;

            if (!string.IsNullOrEmpty(DataGrid_Receipt_Table.CurrentRow.Cells["TID"].Value.ToString()) && ComboBox_Receipt_Status.SelectedIndex != -1)
            {
                            
                if (ComboBox_Receipt_Users.SelectedIndex > -1)
                {
                    strUsers = ComboBox_Receipt_Users.SelectedValue.ToString();
                }

                Class_SQL.Save_Receipt_Order(Convert.ToInt32(DataGrid_Receipt_Table.CurrentRow.Cells["TID"].Value),
                                              UserName,
                                              ComboBox_Receipt_Status.SelectedItem.ToString(),
                                              TextBox_Receipt_GrusName.Text,
                                              Convert.ToInt32(TextBox_Receipt_CountStrok.Text),
                                              Convert.ToString(DatePicker_Receipt_Date_Plan_Arrival_Ats.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Receipt_Time_Plan_Arrival_Ats.Text + ":00",
                                              Convert.ToInt32(TextBox_Receipt_CountPallet.Text),
                                              Convert.ToInt32(TextBox_Receipt_CountBoxRos.Text),
                                              Convert.ToString(DatePicker_Receipt_Date_Fact_Arrival_Ats.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Receipt_Time_Fact_Arrival_Ats.Text + ":00",
                                              Convert.ToString(DatePicker_Receipt_Date_Actual_onDock_Ats.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Receipt_Time_Actual_onDock_Ats.Text + ":00",
                                              Convert.ToString(DatePicker_Receipt_Date_Completion_Discharge_Ats.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Receipt_Time_Completion_Discharge_Ats.Text + ":00",
                                              Convert.ToString(DatePicker_Receipt_Date_Acceptance.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Receipt_Time_Acceptance.Text + ":00",
                                              TextBox_Receipt_Messanger.Text,
                                              strUsers,
                                              TextBox_Receipt_Invoice.Text,
                                              Convert.ToDecimal(DataGrid_Receipt_Table.CurrentRow.Cells["USER_DEF8"].Value)
                                             );                                                
                Load_Receipt_Table();
                Clear_Receipt();
            }
        }

        private void Clear_Receipt()
        {
            TextBox_Receipt_OrderType.Text = null;
            TextBox_Receipt_OrderNumber.Text = null;
            TextBox_Receipt_OrderErp.Text = null;
            TextBox_Receipt_GrusName.Text = null;
            TextBox_Receipt_Warehouse.Text = null;
            TextBox_Receipt_Company.Text = null;
            TextBox_Receipt_Invoice.Text = null;
            TextBox_Receipt_CountStrok.Text = "0";
            DatePicker_Receipt_Date_Plan_Arrival_Ats.Value = DateTime.Now;
            DatePicker_Receipt_Time_Plan_Arrival_Ats.Text = "00:00";
            TextBox_Receipt_CountPallet.Text = "0";
            TextBox_Receipt_CountBoxRos.Text = "0";
            DatePicker_Receipt_Date_Fact_Arrival_Ats.Value = DateTime.Now;
            DatePicker_Receipt_Time_Fact_Arrival_Ats.Text = "00:00";
            DatePicker_Receipt_Date_Actual_onDock_Ats.Value = DateTime.Now;
            DatePicker_Receipt_Time_Actual_onDock_Ats.Text = "00:00";
            DatePicker_Receipt_Date_Completion_Discharge_Ats.Value = DateTime.Now;
            DatePicker_Receipt_Time_Completion_Discharge_Ats.Text = "00:00";
            ComboBox_Receipt_Users.SelectedIndex = -1;
            TextBox_Receipt_Messanger.Text = null;
            TextBox_Receipt_Invoice.Text = null;
            GridView_Receipt_Dop.DataSource = null;
        }

        private void DataGrid_Receipt_Table_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectReceiptTable();
        }

        private void DataGrid_Receipt_Table_KeyDown(object sender, KeyEventArgs e)
        {
            SelectReceiptTable();
        }

        private void DataGrid_Receipt_Table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0)
            {
                Receipt_Select_Column_Name = DataGrid_Receipt_Table.Columns[e.ColumnIndex].Name.ToString();
            }
            SelectReceiptTable();
        }

        private void TextBox_Receipt_CountStrok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Receipt_CountPallet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Receipt_CountBoxRos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Button_Receipt_Clean_Filter_Click(object sender, EventArgs e)
        {

            DataGrid_Receipt_Table.CleanFilterAndSort();
            RecSortList.Clear();
            Sort_receipt_string = null;
            Serch_receipt_string = null;
            Load_Receipt_Table();
        }


        private void Button_Receipt_DopRazdel_Add_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TextBox_Receipt_OrderNumber.Text))
            {
                if (ComboBox_Receipt_DopRazdel.SelectedIndex != -1)
                {
                    if (ComboBox_Receipt_DopRazdel_Name.SelectedIndex != -1)
                    {
                        if (TextBox_Receipt_DopRazdel_Qty.Text != "0")
                        {

                            if (ComboBox_Receipt_DopRazdel_En.SelectedIndex != -1)
                            {
                                string NameUroven2 = null, NameUroven3 = null;
                                
                                if (ComboBox_Receipt_DopRazdel_Uroven2.SelectedIndex != -1)
                                {
                                    NameUroven2 = ComboBox_Receipt_DopRazdel_Uroven2.SelectedItem.ToString();
                                }
                                if (ComboBox_Receipt_DopRazdel_Uroven3.SelectedIndex != -1)
                                {
                                    NameUroven3 = ComboBox_Receipt_DopRazdel_Uroven3.SelectedItem.ToString();
                                }
                                
                                Class_SQL.Insert_Dop_Receipt(TextBox_Receipt_OrderNumber.Text, 
                                                             ComboBox_Receipt_DopRazdel.SelectedItem.ToString(), 
                                                             ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString(), 
                                                             NameUroven2, 
                                                             NameUroven3, 
                                                             Convert.ToDecimal(TextBox_Receipt_DopRazdel_Qty.Text),
                                                             ComboBox_Receipt_DopRazdel_En.SelectedItem.ToString(), 
                                                             UserName, 
                                                             Convert.ToDecimal(DataGrid_Receipt_Table.CurrentRow.Cells["USER_DEF8"].Value));                               

                                ComboBox_Receipt_DopRazdel.SelectedIndex = -1;
                                ComboBox_Receipt_DopRazdel_Name.DataSource = null;
                                ComboBox_Receipt_DopRazdel_Uroven2.DataSource = null;
                                ComboBox_Receipt_DopRazdel_Uroven3.DataSource = null;
                                ComboBox_Receipt_DopRazdel_En.DataSource = null;
                                TextBox_Receipt_DopRazdel_Qty.Text = "0";

                                Load_Receipt_Dop(TextBox_Receipt_OrderNumber.Text, Convert.ToDecimal(DataGrid_Receipt_Table.CurrentRow.Cells["USER_DEF8"].Value));                                

                            }
                            else
                            {
                                MessageBox.Show("Не указана Ед.измерения.", "ABS", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не верно указано количество.", "ABS", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не указано наименование.", "ABS", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Не указан дополнительный раздел.", "ABS", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Не указан номер заявки.", "ABS", MessageBoxButtons.OK);
            }
        }

        private void Add_Dop_Razel(string OrderNumber, string Razdel, string NameRazdel, string Uroven2, string Uroven3, int Qty, string En)
        {
            
        }

        private void Button_Receipt_DopRazdel_Del_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBox_Receipt_OrderNumber.Text.ToString()))
            {

                Class_SQL.Del_Receipt_Dop(GridView_Receipt_Dop.SelectedCells[0].Value.ToString(), UserName);
                Class_SQL.OpenConnection();                                
                Load_Receipt_Dop(TextBox_Receipt_OrderNumber.Text.ToString(), Convert.ToDecimal(DataGrid_Receipt_Table.CurrentRow.Cells["USER_DEF8"].Value));
                ComboBox_Receipt_DopRazdel.SelectedIndex = -1;
                ComboBox_Receipt_DopRazdel_Name.DataSource = null;
                ComboBox_Receipt_DopRazdel_Uroven2.DataSource = null;
                ComboBox_Receipt_DopRazdel_Uroven3.DataSource = null;
                ComboBox_Receipt_DopRazdel_En.DataSource = null;
                TextBox_Receipt_DopRazdel_Qty.Text = "0";
                Class_SQL.CloseConnection();
            }
        }

        private void DataGrid_Shipment_Table_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectShipmentTable();
        }

        private void DataGrid_Shipment_Table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex >= 0)
            {
                Shipment_Select_Column_Name = DataGrid_Shipment_Table.Columns[e.ColumnIndex].Name.ToString();
            }
            SelectShipmentTable();

        }

        private void DataGrid_Shipment_Table_KeyDown(object sender, KeyEventArgs e)
        {
            SelectShipmentTable();
        }

        private void DataGrid_Shipment_Table_KeyUp(object sender, KeyEventArgs e)
        {
            SelectShipmentTable();
        }

        private void DataGrid_Receipt_Table_KeyUp(object sender, KeyEventArgs e)
        {
            SelectReceiptTable();
        }

        private void Button_Shipment_Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBox_Shipment_OrderNumber.Text))
            {
                if (ComboBox_Shipment_Status.SelectedItem.ToString() == "Выполнено")
                {
                    if (DatePicker_Shipment_Actual_Arrival_Ats_Time.Text != "00:00")
                    {
                        if (DatePicker_Shipment_Actual_OnDock_Ats_Time.Text != "00:00")
                        {
                            if (DatePicker_Shipment_Completion_Loading_Ats_Time.Text != "00:00")
                            {
                                if (Convert.ToDecimal(TextBox_Shipment_Count_All_Pallet.Text) == 0)
                                {
                                    if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Всего паллет к отгрузке' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==
                                        System.Windows.Forms.DialogResult.OK)
                                    {
                                        if (Convert.ToDecimal(TextBox_Shipment_Count_Mix_Pallet.Text) == 0)
                                        {
                                            if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Микспаллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==
                                                System.Windows.Forms.DialogResult.OK)
                                            {
                                                if (Convert.ToDecimal(TextBox_Shipment_Count_All_Box.Text) == 0)
                                                {
                                                    if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Всего коробов' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                    {
                                                        if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                        {
                                                            if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                            {
                                                                SaveShipment();
                                                            }
                                                            else
                                                            {
                                                                TextBox_Shipment_Count_Pallet.Focus();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            SaveShipment();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        TextBox_Shipment_Count_All_Box.Focus();
                                                    }
                                                }
                                                else
                                                {
                                                    if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                    {
                                                        if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                        {
                                                            SaveShipment();
                                                        }
                                                        else
                                                        {
                                                            TextBox_Shipment_Count_Pallet.Focus();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SaveShipment();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TextBox_Shipment_Count_Mix_Pallet.Focus();
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDecimal(TextBox_Shipment_Count_All_Box.Text) == 0)
                                            {
                                                if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Всего коробов' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                {
                                                    if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                    {
                                                        if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                        {
                                                            SaveShipment();
                                                        }
                                                        else
                                                        {
                                                            TextBox_Shipment_Count_Pallet.Focus();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SaveShipment();
                                                    }
                                                }
                                                else
                                                {
                                                    TextBox_Shipment_Count_All_Box.Focus();
                                                }
                                            }
                                            else
                                            {
                                                if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                {
                                                    if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                    {
                                                        SaveShipment();
                                                    }
                                                    else
                                                    {
                                                        TextBox_Shipment_Count_Pallet.Focus();
                                                    }
                                                }
                                                else
                                                {
                                                    SaveShipment();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TextBox_Shipment_Count_All_Pallet.Focus();
                                    }
                                }
                                else
                                {
                                    if (Convert.ToDecimal(TextBox_Shipment_Count_Mix_Pallet.Text) == 0)
                                    {
                                        if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Микспаллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                        {
                                            if (Convert.ToDecimal(TextBox_Shipment_Count_All_Box.Text) == 0)
                                            {
                                                if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Всего коробов' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                {
                                                    if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                    {
                                                        if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                                                        {
                                                            SaveShipment();
                                                        }
                                                        else
                                                        {
                                                            TextBox_Shipment_Count_Pallet.Focus();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SaveShipment();
                                                    }
                                                }
                                                else
                                                {
                                                    TextBox_Shipment_Count_All_Box.Focus();
                                                }
                                            }
                                            else
                                            {
                                                if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                {
                                                    if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                                                        == System.Windows.Forms.DialogResult.OK)
                                                    {
                                                        SaveShipment();
                                                    }
                                                    else
                                                    {
                                                        TextBox_Shipment_Count_Pallet.Focus();
                                                    }
                                                }
                                                else
                                                {
                                                    SaveShipment();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            TextBox_Shipment_Count_Mix_Pallet.Focus();
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(TextBox_Shipment_Count_All_Box.Text) == 0)
                                        {
                                            if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Всего коробов' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==
                                                System.Windows.Forms.DialogResult.OK)
                                            {
                                                if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                                {
                                                    if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                                                        == System.Windows.Forms.DialogResult.OK)
                                                    {
                                                        SaveShipment();
                                                    }
                                                    else
                                                    {
                                                        TextBox_Shipment_Count_Pallet.Focus();
                                                    }
                                                }
                                                else
                                                {
                                                    SaveShipment();
                                                }
                                            }
                                            else
                                            {
                                                TextBox_Shipment_Count_All_Box.Focus();
                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToDecimal(TextBox_Shipment_Count_Pallet.Text) == 0)
                                            {
                                                if (MessageBox.Show("Уверены, что в заявке № " + TextBox_Shipment_OrderNumber.Text + " нет 'Загружено паллет' ?", "ABS", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                                                    == System.Windows.Forms.DialogResult.OK)
                                                {
                                                    SaveShipment();
                                                }
                                                else
                                                {
                                                    TextBox_Shipment_Count_Pallet.Focus();
                                                }
                                            }
                                            else
                                            {
                                                SaveShipment();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("'Поле Дата и время окончания погрузки АТС' не заполнено.", "ABS", MessageBoxButtons.OK);
                                DatePicker_Shipment_Completion_Loading_Ats_Time.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("'Поле Дата и время постановки на док АТС' не заполнено.", "ABS", MessageBoxButtons.OK);
                            DatePicker_Shipment_Actual_OnDock_Ats_Time.Focus();
                        }
                    }
                    else
                    {   
                        MessageBox.Show("'Поле Фактическая дата и время прибытия АТС' не заполнено.", "ABS", MessageBoxButtons.OK);
                        DatePicker_Shipment_Actual_Arrival_Ats_Time.Focus();
                    }
                }
                else
                {
                    SaveShipment();
                }
            }
            else
            {
                MessageBox.Show("Поле Номер документа не заполнено.", "ABS", MessageBoxButtons.OK);
            }
        }


        private void SaveShipment()
        {
            
            if (DataGrid_Shipment_Table.SelectedRows.Count > 0)
            {
                if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.CurrentRow.Cells["TID"].Value.ToString()) && ComboBox_Shipment_Status.SelectedIndex != -1)
                {
                    string stUsers = null;

                    if (ComboBox_Shipment_Users.SelectedIndex != -1)
                    {
                        stUsers = ComboBox_Shipment_Users.SelectedValue.ToString();
                    }

                    Class_SQL.Save_Shipment_Order(Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["TID"].Value),
                                                  ComboBox_Shipment_Status.SelectedItem.ToString(),
                                                  Convert.ToInt32(TextBox_Shipment_CountStrok.Text),
                                                  Convert.ToString(DatePicker_Shipment_Plan_Shipment_Date.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Shipment_Plan_Shipment_Time.Text + ":00",
                                                  Convert.ToString(DatePicker_Shipment_Fact_Shipment_Date.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Shipment_Fact_Shipment_Time.Text + ":00",
                                                  Convert.ToString(DatePicker_Shipment_Actual_OnDock_Ats_Date.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Shipment_Actual_OnDock_Ats_Time.Text + ":00",
                                                  Convert.ToString(DatePicker_Shipment_Actual_Arrival_Ats_Date.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Shipment_Actual_Arrival_Ats_Time.Text + ":00",
                                                  Convert.ToInt32(TextBox_Shipment_Count_Term_Pallet.Text),
                                                  Convert.ToInt32(TextBox_Shipment_Count_Mix_Pallet.Text),
                                                  Convert.ToInt32(TextBox_Shipment_Count_All_Box.Text),
                                                  Convert.ToString(DatePicker_Shipment_Completion_Loading_Ats_Date.Value.ToString("yyyy-MM-dd")) + ' ' + DatePicker_Shipment_Completion_Loading_Ats_Time.Text + ":00",
                                                  Convert.ToInt32(TextBox_Shipment_Count_Pallet.Text),
                                                  Convert.ToInt32(TextBox_Shipment_Count_Pallet_Ros.Text),
                                                  stUsers,
                                                  TextBox_Shipment_Messanger.Text,
                                                  UserName,
                                                  Convert.ToInt32(TextBox_Shipment_Count_All_Pallet.Text),
                                                  Convert.ToInt32(TextBox_Shipment_Count_Term_Box.Text),
                                                  Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["USER_DEF8"].Value));

                    Class_SQL.Insert_Dop_Ship_streich(Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["TID"].Value),
                                                      Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["USER_DEF8"].Value),
                                                      UserName,
                                                      Convert.ToInt32(TextBox_Shipment_Count_Mix_Pallet.Text));
                             

                    Load_Shipment_Table();
                    Clear_Shipment();
                }
            }
        }

        private void Clear_Shipment()
        {
            TextBox_Shipment_OrderType.Text = null;
            TextBox_Shipment_OrderNumber.Text = null;
            TextBox_Shipment_OrderErp.Text = null;
            TextBox_Shipment_GruzName.Text = null;
            TextBox_Shipment_Warehouse.Text = null;
            TextBox_Shipment_Company.Text = null;
            TextBox_Shipment_CountStrok.Text = "0";
            ComboBox_Shipment_Status.SelectedIndex = 0;
            DatePicker_Shipment_Plan_Shipment_Date.Value = DateTime.Now;
            DatePicker_Shipment_Plan_Shipment_Time.Text = "00:00";
            DatePicker_Shipment_Fact_Shipment_Date.Value = DateTime.Now;
            DatePicker_Shipment_Fact_Shipment_Time.Text = "00:00";
            DatePicker_Shipment_Actual_OnDock_Ats_Date.Value = DateTime.Now;
            DatePicker_Shipment_Actual_OnDock_Ats_Time.Text = "00:00";
            DatePicker_Shipment_Actual_Arrival_Ats_Date.Value = DateTime.Now;
            DatePicker_Shipment_Actual_Arrival_Ats_Time.Text = "00:00";
            TextBox_Shipment_Count_Mix_Pallet.Text = "0";
            TextBox_Shipment_Count_Term_Pallet.Text = "0";
            TextBox_Shipment_Count_All_Box.Text = "0";
            DatePicker_Shipment_Completion_Loading_Ats_Date.Value = DateTime.Now;
            DatePicker_Shipment_Completion_Loading_Ats_Time.Text = "00:00";
            TextBox_Shipment_Count_Pallet.Text = "0";
            TextBox_Shipment_Messanger.Text = null;
            TextBox_Shipment_Count_Pallet_Ros.Text = "0";
            TextBox_Shipment_Count_All_Pallet.Text = "0";
            TextBox_Shipment_Count_Term_Box.Text = "0";
            ComboBox_Shipment_Users.SelectedIndex = -1;
            GridView_Shipment_Dop.DataSource = null;
        }
               
        private void Button_Shipment_DopRazdel_Add_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TextBox_Shipment_OrderNumber.Text))
            {
                if (this.ComboBox_Shipment_DopRazdel.SelectedIndex != -1)
                {
                    if (this.ComboBox_Shipment_DopRazdel_Name.SelectedIndex != -1)
                    {
                        if (this.TextBox_Shipment_DopRazdel_Qty.Text != "0")
                        {

                            if (this.ComboBox_Shipment_DopRazdel_En.SelectedIndex != -1)
                            {
                                string NameOrder = null, NameRazdel = null, NameDop = null, NameUroven2 = null, NameUroven3 = null, NameEn = null;
                                int Qty;

                                NameOrder = this.TextBox_Shipment_OrderNumber.Text;
                                NameRazdel = this.ComboBox_Shipment_DopRazdel.SelectedItem.ToString();
                                NameDop = this.ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString();
                                if (this.ComboBox_Shipment_DopRazdel_Uroven2.SelectedIndex != -1)
                                {
                                    NameUroven2 = this.ComboBox_Shipment_DopRazdel_Uroven2.SelectedItem.ToString();
                                }
                                if (this.ComboBox_Shipment_DopRazdel_Uroven3.SelectedIndex != -1)
                                {
                                    NameUroven3 = this.ComboBox_Shipment_DopRazdel_Uroven3.SelectedItem.ToString();
                                }
                                NameEn = this.ComboBox_Shipment_DopRazdel_En.SelectedItem.ToString();
                                Qty = Convert.ToInt32(this.TextBox_Shipment_DopRazdel_Qty.Text);

                                Class_SQL.Insert_Dop_Receipt(TextBox_Shipment_OrderNumber.Text,
                                                            ComboBox_Shipment_DopRazdel.SelectedItem.ToString(),
                                                            ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString(),
                                                            NameUroven2,
                                                            NameUroven3,
                                                            Convert.ToDecimal(this.TextBox_Shipment_DopRazdel_Qty.Text),
                                                            ComboBox_Shipment_DopRazdel_En.SelectedItem.ToString(),
                                                            UserName,
                                                            Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["USER_DEF8"].Value));

                                this.ComboBox_Shipment_DopRazdel.SelectedIndex = -1;
                                this.ComboBox_Shipment_DopRazdel_Name.DataSource = null;
                                this.ComboBox_Shipment_DopRazdel_Uroven2.DataSource = null;
                                this.ComboBox_Shipment_DopRazdel_Uroven3.DataSource = null;
                                this.ComboBox_Shipment_DopRazdel_En.DataSource = null;
                                this.TextBox_Shipment_DopRazdel_Qty.Text = "0";
                            }
                            else
                            {
                                MessageBox.Show("Не указана Ед.измерения.", "ABS", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не верно указано количество.", "ABS", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не указано наименование.", "ABS", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Не указан дополнительный раздел.", "ABS", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Не указан номер заявки.", "ABS", MessageBoxButtons.OK);
            }
            Load_Shipment_Dop(TextBox_Shipment_OrderNumber.Text.ToString(), Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["USER_DEF8"].Value.ToString()));
        }

        private void Button_Shipment_DopRazdel_Del_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBox_Shipment_OrderNumber.Text.ToString()))
            {
                Class_SQL.Del_Receipt_Dop(GridView_Shipment_Dop.SelectedCells[0].Value.ToString(), UserName);
                Class_SQL.OpenConnection();          
                Load_Shipment_Dop(TextBox_Shipment_OrderNumber.Text.ToString(),Convert.ToDecimal(DataGrid_Shipment_Table.CurrentRow.Cells["USER_DEF8"].Value.ToString()));
                ComboBox_Shipment_DopRazdel.SelectedIndex = -1;
                ComboBox_Shipment_DopRazdel_Name.DataSource = null;
                ComboBox_Shipment_DopRazdel_Uroven2.DataSource = null;
                ComboBox_Shipment_DopRazdel_Uroven3.DataSource = null;
                ComboBox_Shipment_DopRazdel_En.DataSource = null;
                TextBox_Shipment_DopRazdel_Qty.Text = "0";
                Class_SQL.CloseConnection();
            }
        }

        private void DataGrid_Service_Table_KeyDown(object sender, KeyEventArgs e)
        {
            SelectServiceTable();
        }

        private void DataGrid_Service_Table_KeyUp(object sender, KeyEventArgs e)
        {
            SelectServiceTable();
        }

        private void DataGrid_Service_Table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0)
            {
                Service_Select_Column_Name = DataGrid_Service_Table.Columns[e.ColumnIndex].Name.ToString();
            }
            SelectServiceTable();
        }

        private void Button_Service_Save_Click(object sender, EventArgs e)
        {
            SaveService();
        }

        private void SaveService()
        {
            if (DataGrid_Service_Table.SelectedRows.Count > 0)
            {
                if (!string.IsNullOrEmpty(DataGrid_Service_Table.CurrentRow.Cells["TID"].Value.ToString()) && ComboBox_Service_Status.SelectedIndex != -1)
                {
                    Class_SQL.Save_Service_Order(Convert.ToDecimal(DataGrid_Service_Table.CurrentRow.Cells["TID"].Value),
                                                 ComboBox_Service_Status.SelectedItem.ToString(),
                                                 Convert.ToInt32(TextBox_Service_CountStrok.Text),
                                                 Convert.ToString(DatePicker_Service_Date_Receiving.Value.ToString("yyyy-MM-dd")) + " " + DatePicker_Service_Time_Receiving.Text + ":00",
                                                 UserName ,
                                                 0,
                                                 txt_Service_Messanger.Text
                                                 );
                    Load_Service_Table();
                }
            }
        }

     

        private void Button_Service_DopRazdel_Add_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBox_Service_OrderNumber.Text))
            {
                if (ComboBox_Service_DopRazdel.SelectedIndex != -1)
                {
                    if (ComboBox_Service_DopRazdel_Name.SelectedIndex != -1)
                    {
                        if (TextBox_Service_DopRazdel_Qty.Text != "0")
                        {

                            if (ComboBox_Service_DopRazdel_En.SelectedIndex != -1)
                            {
                                string NameOrder = null, NameRazdel = null, NameDop = null, NameUroven2 = null, NameUroven3 = null, NameEn = null ;
                                int Qty;
                                Decimal user_def8 = 0;

                                NameOrder = TextBox_Service_OrderNumber.Text;
                                NameRazdel = ComboBox_Service_DopRazdel.SelectedItem.ToString();
                                NameDop = ComboBox_Service_DopRazdel_Name.SelectedItem.ToString();
                                if (ComboBox_Service_DopRazdel_Uroven2.SelectedIndex != -1)
                                {
                                    NameUroven2 = ComboBox_Service_DopRazdel_Uroven2.SelectedItem.ToString();
                                }
                                if (ComboBox_Service_DopRazdel_Uroven3.SelectedIndex != -1)
                                {
                                    NameUroven3 = ComboBox_Service_DopRazdel_Uroven3.SelectedItem.ToString();
                                }
                                NameEn = ComboBox_Service_DopRazdel_En.SelectedItem.ToString();
                                Qty = Convert.ToInt32(TextBox_Service_DopRazdel_Qty.Text);

                                Class_SQL.Insert_Dop_Receipt(NameOrder, NameRazdel, NameDop, NameUroven2, NameUroven3, Qty, NameEn, UserName, user_def8);

                                ComboBox_Service_DopRazdel.SelectedIndex = -1;
                                ComboBox_Service_DopRazdel_Name.DataSource = null;
                                ComboBox_Service_DopRazdel_Uroven2.DataSource = null;
                                ComboBox_Service_DopRazdel_Uroven3.DataSource = null;
                                ComboBox_Service_DopRazdel_En.DataSource = null;
                                TextBox_Service_DopRazdel_Qty.Text = "0";
                                Load_Service_Dop(NameOrder);
                            }
                            else
                            {
                                MessageBox.Show("Не указана Ед.измерения.", "ABS", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не верно указано количество.", "ABS", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не указано наименование.", "ABS", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Не указан дополнительный раздел.", "ABS", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Не указан номер заявки.", "ABS", MessageBoxButtons.OK);
            }
        }

        private void Button_Service_DopRazdel_Del_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.TextBox_Service_OrderNumber.Text.ToString()))
            {

                Class_SQL.Del_Receipt_Dop(GridView_Service_Dop.SelectedCells[0].Value.ToString(), UserName);

                Class_SQL.OpenConnection();               
                Load_Service_Dop(this.TextBox_Service_OrderNumber.Text.ToString());
                this.ComboBox_Service_DopRazdel.SelectedIndex = -1;
                this.ComboBox_Service_DopRazdel_Name.DataSource = null;
                this.ComboBox_Service_DopRazdel_Uroven2.DataSource = null;
                this.ComboBox_Service_DopRazdel_Uroven3.DataSource = null;
                this.ComboBox_Service_DopRazdel_En.DataSource = null;
                this.TextBox_Service_DopRazdel_Qty.Text = "0";
                Class_SQL.CloseConnection();
            }

        }

        private void FirstForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Class_SQL.CloseConnection();
            Application.Exit();
        }

        private void Button_Receipt_Excel_Click(object sender, EventArgs e)
        {
            Excel_Export(DataGrid_Receipt_Table, "ABS_Receipt_Export");
        }   
        
        private void  Excel_Export (DataGridView DataGridName, string DefaultFileName)
        {
            DataGridName.MultiSelect = true;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xls)|*.xls";
            sfd.FileName =  DefaultFileName +"_" + DateTime.Now.ToShortDateString() + ".xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Copy DataGridView results to clipboard

                copyAlltoClipboard(DataGridName);

                object misValue = System.Reflection.Missing.Value;
                Excel.Application xlexcel = new Excel.Application();

                xlexcel.DisplayAlerts = false; // Without this you will get two confirm overwrite prompts
                Excel.Workbook xlWorkBook = xlexcel.Workbooks.Add(misValue);
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // Format column D as text before pasting results, this was required for my data
                Excel.Range rng = xlWorkSheet.get_Range("D:D").Cells;
                rng.NumberFormat = "@";

                // Paste clipboard results to worksheet range
                Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                // For some reason column A is always blank in the worksheet. ¯\_(ツ)_/¯
                // Delete blank column A and select cell A1
                Excel.Range delRng = xlWorkSheet.get_Range("A:A").Cells;
                delRng.Delete(Type.Missing);
                xlWorkSheet.get_Range("A1").Select();

                // Save the excel file under the captured location from the SaveFileDialog
                xlWorkBook.SaveAs(sfd.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlexcel.DisplayAlerts = true;
                xlWorkBook.Close(true, misValue, misValue);
                xlexcel.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlexcel);

                // Clear Clipboard and DataGridView selection
                Clipboard.Clear();
                DataGridName.ClearSelection();
                DataGridName.MultiSelect = false;
                // Open the newly saved excel file
                //if (File.Exists(sfd.FileName))
                //System.Diagnostics.Process.Start(sfd.FileName);
            }

        }

        private void copyAlltoClipboard(DataGridView DataGridName)
        {
            DataGridName.SelectAll();
            DataObject dataObj = DataGridName.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void Button_Shipment_Excel_Click(object sender, EventArgs e)
        {
            Excel_Export(DataGrid_Shipment_Table, "ABS_Shipment_Export");
        }

        private void Button_Service_Excel_Click(object sender, EventArgs e)
        {
            Excel_Export(DataGrid_Service_Table, "ABS_Service_Export");
        }

        private void TextBox_Shipment_CountStrok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Shipment_Count_Mix_Pallet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Receipt_DopRazdel_Qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Shipment_Count_All_Pallet_KeyPress(object sender, KeyPressEventArgs e)
        {
           

            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;

            }
                
        }

        private void TextBox_Shipment_Count_Term_Box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Shipment_Count_All_Box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Shipment_Count_Pallet_Ros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Shipment_Count_Pallet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Shipment_DopRazdel_Qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Service_CountStrok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Service_DopRazdel_Qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DataGrid_Shipment_Table_FilterStringChanged(object sender, AdvancedDataGridView.FilterEventArgs e)
        {
            

            if (!string.IsNullOrEmpty(e.FilterString))
            {
                e.FilterString = " and " + e.FilterString;
                e.FilterString = e.FilterString.Replace("('", "(N'");
                e.FilterString = e.FilterString.Replace("', '", "', N'");
                Serch_Shipment_string = Serch_Shipment_string + ' ' + e.FilterString;
            }

            Load_Shipment_Table();

            if (!string.IsNullOrEmpty(TextBox_Shipment_OrderNumber.Text))
            {

                Clear_Shipment();
            }


        }

        private void DataGrid_Service_Table_FilterStringChanged(object sender, AdvancedDataGridView.FilterEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.FilterString))
            {
                e.FilterString = " and " + e.FilterString;
                e.FilterString = e.FilterString.Replace("('", "(N'");
                e.FilterString = e.FilterString.Replace("', '", "', N'");
                Serch_Service_string = Serch_Service_string + ' ' + e.FilterString;
            }

            Load_Service_Table();
        }

        private void TextBox_Shipment_Count_Term_Box_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        { //функционал  убран  по согласованию
            //e.Handled = true;
            //if (e.KeyCode == Keys.Enter)
            //{
            //    Console.WriteLine("Наименование таба " + this.tabControl.SelectedTab.Name.ToString());

            //    string TabName = this.tabControl.SelectedTab.Name.ToString();
            //    switch (TabName)
            //    {
            //        case "tabPageReceipt":
            //            ReceiptSave();
            //            break;
            //        case "tabPageShipment":
            //            SaveShipment();
            //            break;
            //        case "tabPageService":
            //            SaveService();
            //            break;
            //        default:
            //            break;
    
            //    }
            //}                
        }

        private void ДобавитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }


        private void Button_Shipment_Clean_Filter_Click(object sender, EventArgs e)
        {
            DataGrid_Shipment_Table.CleanFilterAndSort();
            ShipSortList.Clear();
            Serch_Shipment_string = null;
            Load_Shipment_Table();

        }

        private void Button_Service_Clean_Filter_Click(object sender, EventArgs e)
        {
            DataGrid_Service_Table.CleanFilterAndSort();
            SerSortList.Clear();
            Serch_Service_string = null;
            Load_Service_Table();
        }

        private void FirstForm_Load(object sender, EventArgs e)
        {

        }

        private void DataGrid_Service_Table_SortStringChanged(object sender, AdvancedDataGridView.SortEventArgs e)
        {
            Console.WriteLine(e.SortString);
            Console.WriteLine("StringSortTable = " + Service_Select_Column_Name);

            if (!string.IsNullOrEmpty(e.SortString))
            {
                string stsort = e.SortString.Substring(1, e.SortString.IndexOf("]") - 1);

                if (Service_Select_Column_Name == stsort)
                {
                    string sortStr = e.SortString.Replace("[", "").Replace("]", "");

                    // Проверяем наличие ранее добавленного столбца в список сортировки 
                    if (Convert.ToInt32(SerSortList.FindIndex(s => string.Equals(s.ColumnName, Service_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))) != -1)
                    {
                        //удаляем по индексу 
                        SerSortList.RemoveAt(Convert.ToInt32(SerSortList.FindIndex(s => string.Equals(s.ColumnName, Service_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))));
                        SerSortList.Add(new SortListName() { ColumnName = stsort, ColumnValue = sortStr });
                    }
                    else
                    {
                        SerSortList.Add(new SortListName() { ColumnName = stsort, ColumnValue = sortStr });
                    }
                }
            }
            else if (!string.IsNullOrEmpty(Service_Select_Column_Name) && string.IsNullOrEmpty(e.SortString))
            {
                if (Convert.ToInt32(SerSortList.FindIndex(s => string.Equals(s.ColumnName, Service_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))) != -1)
                {
                    //удаляем по индексу 
                    SerSortList.RemoveAt(Convert.ToInt32(SerSortList.FindIndex(s => string.Equals(s.ColumnName, Service_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))));
                }
            }

            Sort_Service_string = null;

            for (int i = 0; i < SerSortList.Count; i++)
            {
                SortListName item = SerSortList[i];
                if (!string.IsNullOrEmpty(Sort_Service_string))
                {
                    Sort_Service_string = Sort_Service_string + ", " + item.ColumnValue;
                }
                else
                {
                    Sort_Service_string = "order by  " + item.ColumnValue;
                }
            }

            Console.WriteLine("Sort_receipt_string = " + Sort_Service_string);
           
            Load_Service_Table();
        }

        private void DataGrid_Receipt_Table_SortStringChanged(object sender, AdvancedDataGridView.SortEventArgs e)
        {
            Console.WriteLine(e.SortString);
            Console.WriteLine("StringSortTable = " + Receipt_Select_Column_Name);

            if (!string.IsNullOrEmpty(e.SortString))
            {
                string stsort = e.SortString.Substring(1, e.SortString.IndexOf("]") - 1);

                if (Receipt_Select_Column_Name == stsort)
                {
                    string sortStr = e.SortString.Replace("[", "").Replace("]", "");

                    // Проверяем наличие ранее добавленного столбца в список сортировки 
                    if (Convert.ToInt32(RecSortList.FindIndex(s => string.Equals(s.ColumnName, Receipt_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))) != -1)
                    {
                        //удаляем по индексу 
                        RecSortList.RemoveAt(Convert.ToInt32(RecSortList.FindIndex(s => string.Equals(s.ColumnName, Receipt_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))));
                        RecSortList.Add(new SortListName() { ColumnName = stsort, ColumnValue = sortStr });
                    }
                    else
                    {
                        RecSortList.Add(new SortListName() { ColumnName = stsort, ColumnValue = sortStr });
                    }
                } 
            }else if (!string.IsNullOrEmpty(Receipt_Select_Column_Name) && string.IsNullOrEmpty(e.SortString))
            {
                if (Convert.ToInt32(RecSortList.FindIndex(s => string.Equals(s.ColumnName, Receipt_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))) != -1)
                {
                    //удаляем по индексу 
                    RecSortList.RemoveAt(Convert.ToInt32(RecSortList.FindIndex(s => string.Equals(s.ColumnName, Receipt_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))));                   
                }
            }

            Sort_receipt_string = null;

            for (int i = 0; i < RecSortList.Count; i++)
               {
                SortListName item = RecSortList[i];
                if (!string.IsNullOrEmpty(Sort_receipt_string))
                    {
                        Sort_receipt_string = Sort_receipt_string + ", " + item.ColumnValue;
                    }
                    else
                    {
                        Sort_receipt_string = "order by  " + item.ColumnValue;
                    }
                }

                Console.WriteLine("Sort_receipt_string = " + Sort_receipt_string);
                Load_Receipt_Table();


            if (!string.IsNullOrEmpty(TextBox_Receipt_OrderNumber.Text))
            {
                Clear_Receipt();
            }
        }

        private void DataGrid_Shipment_Table_SortStringChanged(object sender, AdvancedDataGridView.SortEventArgs e)
        {
            Console.WriteLine(e.SortString);
            Console.WriteLine("StringSortTable = " + Shipment_Select_Column_Name);

            if (!string.IsNullOrEmpty(e.SortString))
            {
                string stsort = e.SortString.Substring(1, e.SortString.IndexOf("]") - 1);

                if (Shipment_Select_Column_Name == stsort)
                {
                    string sortStr = e.SortString.Replace("[", "").Replace("]", "");

                    // Проверяем наличие ранее добавленного столбца в список сортировки 
                    if (Convert.ToInt32(ShipSortList.FindIndex(s => string.Equals(s.ColumnName, Shipment_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))) != -1)
                    {
                        //удаляем по индексу 
                        ShipSortList.RemoveAt(Convert.ToInt32(ShipSortList.FindIndex(s => string.Equals(s.ColumnName, Shipment_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))));
                        ShipSortList.Add(new SortListName() { ColumnName = stsort, ColumnValue = sortStr });
                    }
                    else
                    {
                        ShipSortList.Add(new SortListName() { ColumnName = stsort, ColumnValue = sortStr });
                    }
                }
            }
            else if (!string.IsNullOrEmpty(Shipment_Select_Column_Name) && string.IsNullOrEmpty(e.SortString))
            {
                if (Convert.ToInt32(ShipSortList.FindIndex(s => string.Equals(s.ColumnName, Shipment_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))) != -1)
                {
                    //удаляем по индексу 
                    ShipSortList.RemoveAt(Convert.ToInt32(ShipSortList.FindIndex(s => string.Equals(s.ColumnName, Shipment_Select_Column_Name, StringComparison.CurrentCultureIgnoreCase))));
                }
            }

            Sort_Shipment_string = null;

            for (int i = 0; i < ShipSortList.Count; i++)
            {
                SortListName item = ShipSortList[i];
                if (!string.IsNullOrEmpty(Sort_Shipment_string))
                {
                    Sort_Shipment_string = Sort_Shipment_string + ", " + item.ColumnValue;
                }
                else
                {
                    Sort_Shipment_string = "order by  " + item.ColumnValue;
                }
            }
            Console.WriteLine("Sort_receipt_string = " + Sort_Shipment_string);
            Load_Shipment_Table();
            Clear_Shipment();
        }

        private void Tsmi_library_Click(object sender, EventArgs e)
        {
            Form_library fm = new Form_library();
            fm.Show();
        }

        private void DataGrid_Receipt_Table_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Console.WriteLine(  e.ColumnIndex);
        }
       


        private void ComboBox_Receipt_DopRazdel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DataGrid_Receipt_Table.CurrentRow.Cells["TID"].Value.ToString()))
                {
                    if (ComboBox_Receipt_DopRazdel.SelectedIndex != -1)
                    {
                    ComboBox_Receipt_DopRazdel_Name.DataSource = null;
                    ComboBox_Receipt_DopRazdel_Name.ResetText();

                    ComboBox_Receipt_DopRazdel_Uroven2.DataSource = null;
                    ComboBox_Receipt_DopRazdel_Uroven2.ResetText();

                    ComboBox_Receipt_DopRazdel_Uroven3.DataSource = null;
                    ComboBox_Receipt_DopRazdel_Uroven3.ResetText();

                    ComboBox_Receipt_DopRazdel_En.DataSource = null;
                    ComboBox_Receipt_DopRazdel_En.ResetText();

                    LOAD_DOP(ComboBox_Receipt_DopRazdel.SelectedItem.ToString(), ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel);
                    }
                }
            else
                {
                    MessageBox.Show("Поле Номер заявки не указан.", "ABS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

             this.TextBox_Receipt_DopRazdel_Qty.Text = "0";
        }

        private void ComboBox_Receipt_DopRazdel_Name_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox_Receipt_DopRazdel_Uroven2.Items.Clear();
            ComboBox_Receipt_DopRazdel_Uroven2.ResetText();
            ComboBox_Receipt_DopRazdel_Uroven3.Items.Clear();
            ComboBox_Receipt_DopRazdel_Uroven3.ResetText();
            ComboBox_Receipt_DopRazdel_En.DataSource = null;
            ComboBox_Receipt_DopRazdel_En.Items.Clear();
            ComboBox_Receipt_DopRazdel_En.ResetText();

            if (ComboBox_Receipt_DopRazdel_Name.SelectedIndex != -1)
            {
                for (int i = 0; i < ListDopOperation.Count; i++)
                {
                    ///заполнение выпадающего списка уровня 2
                    if (ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name)
                    {
                        if (!ComboBox_Receipt_DopRazdel_Uroven2.Items.Contains(ListDopOperation[i].Uroven2))
                        {
                            ComboBox_Receipt_DopRazdel_Uroven2.Items.Add(ListDopOperation[i].Uroven2);
                        }

                        /// Уровень 3 заполняется в том случае не имеет привязку к Уровню 2
                        if (string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                        {
                            if (!ComboBox_Receipt_DopRazdel_Uroven3.Items.Contains(ListDopOperation[i].Uroven3))
                            {
                                ComboBox_Receipt_DopRazdel_Uroven3.Items.Add(ListDopOperation[i].Uroven3);
                            }
                        }
                        //Заполняется если Уровнь 2 и 3 пустые
                        if (string.IsNullOrEmpty(ListDopOperation[i].Uroven2) && string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                        {
                            if (!ComboBox_Receipt_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                            {
                                ComboBox_Receipt_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                            }

                            SELECT_EN(ComboBox_Receipt_DopRazdel, ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel_Uroven2, ComboBox_Receipt_DopRazdel_Uroven3, ComboBox_Receipt_DopRazdel_En);
                        }
                    }
                }
            }


          

        }

        private void ComboBox_Receipt_DopRazdel_Uroven2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox_Receipt_DopRazdel_Uroven3.Items.Clear();
            ComboBox_Receipt_DopRazdel_Uroven3.ResetText();
            ComboBox_Receipt_DopRazdel_En.Items.Clear();
            ComboBox_Receipt_DopRazdel_En.ResetText();

            if (ComboBox_Receipt_DopRazdel_Name.SelectedIndex != -1)
            {
                if (ComboBox_Receipt_DopRazdel_Uroven2.SelectedIndex != -1)
                {
                    //uroven3
                    for (int i = 0; i < ListDopOperation.Count; i++)
                    {
                        if ((ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Receipt_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2))
                        {
                            if (!string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                            {
                                if (!ComboBox_Receipt_DopRazdel_Uroven3.Items.Contains(ListDopOperation[i].Uroven3))
                                {
                                    ComboBox_Receipt_DopRazdel_Uroven3.Items.Add(ListDopOperation[i].Uroven3);
                                }
                            }
                        }
                    }
                    //en
                    for (int i = 0; i < ListDopOperation.Count; i++)
                    {
                        if ((ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Receipt_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                            (string.IsNullOrEmpty(ListDopOperation[i].Uroven3)))
                        {
                            if (!string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                            {
                                if (!ComboBox_Receipt_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Receipt_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Receipt_DopRazdel, ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel_Uroven2, ComboBox_Receipt_DopRazdel_Uroven3, ComboBox_Receipt_DopRazdel_En);
                            }
                        }
                    }
                }
            }
        }

        private void ComboBox_Receipt_DopRazdel_Uroven3_SelectionChangeCommitted(object sender, EventArgs e)
        {

            ComboBox_Receipt_DopRazdel_En.Items.Clear();
            ComboBox_Receipt_DopRazdel_En.ResetText();

            if (ComboBox_Receipt_DopRazdel_Name.SelectedIndex != -1)
            {

                if (ComboBox_Receipt_DopRazdel_Uroven2.SelectedIndex != -1)
                {
                    if (ComboBox_Receipt_DopRazdel_Uroven3.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Receipt_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                (ComboBox_Receipt_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Receipt_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Receipt_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Receipt_DopRazdel, ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel_Uroven2, ComboBox_Receipt_DopRazdel_Uroven3, ComboBox_Receipt_DopRazdel_En);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Receipt_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                (string.IsNullOrEmpty(ComboBox_Receipt_DopRazdel_Uroven3.SelectedItem.ToString())))
                            {
                                if (!ComboBox_Receipt_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Receipt_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Receipt_DopRazdel, ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel_Uroven2, ComboBox_Receipt_DopRazdel_Uroven3, ComboBox_Receipt_DopRazdel_En);
                            }
                        }
                    }
                }
                else
                {
                    if (ComboBox_Receipt_DopRazdel_Uroven3.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (string.IsNullOrEmpty(ListDopOperation[i].Uroven2)) &&
                                (ComboBox_Receipt_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Receipt_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Receipt_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Receipt_DopRazdel, ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel_Uroven2, ComboBox_Receipt_DopRazdel_Uroven3, ComboBox_Receipt_DopRazdel_En);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Receipt_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (string.IsNullOrEmpty(ComboBox_Receipt_DopRazdel_Uroven2.SelectedItem.ToString())) &&
                                (ComboBox_Receipt_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Receipt_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Receipt_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Receipt_DopRazdel, ComboBox_Receipt_DopRazdel_Name, ComboBox_Receipt_DopRazdel_Uroven2, ComboBox_Receipt_DopRazdel_Uroven3, ComboBox_Receipt_DopRazdel_En);
                            }
                        }
                    }
                }
            }
        }

        private void ComboBox_Shipment_DopRazdel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DataGrid_Shipment_Table.CurrentRow.Cells["TID"].Value.ToString()))
            {
                if (ComboBox_Shipment_DopRazdel.SelectedIndex != -1)
                {
                    ComboBox_Shipment_DopRazdel_Name.DataSource = null;
                    ComboBox_Shipment_DopRazdel_Name.ResetText();

                    ComboBox_Shipment_DopRazdel_Uroven2.DataSource = null;
                    ComboBox_Shipment_DopRazdel_Uroven2.ResetText();

                    ComboBox_Shipment_DopRazdel_Uroven3.DataSource = null;
                    ComboBox_Shipment_DopRazdel_Uroven3.ResetText();

                    ComboBox_Shipment_DopRazdel_En.DataSource = null;
                    ComboBox_Shipment_DopRazdel_En.ResetText();

                    LOAD_DOP(ComboBox_Shipment_DopRazdel.SelectedItem.ToString(), ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel);
                }
            }
            else
            {
                MessageBox.Show("Поле Номер заявки не указан.", "ABS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            TextBox_Shipment_DopRazdel_Qty.Text = "0";
        }

        private void ComboBox_Shipment_DopRazdel_Name_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ComboBox_Shipment_DopRazdel.SelectedIndex != -1)
            {
                if (ComboBox_Shipment_DopRazdel_Name.SelectedIndex != -1)
                {
                    ComboBox_Shipment_DopRazdel_Uroven2.Items.Clear();
                    ComboBox_Shipment_DopRazdel_Uroven2.ResetText();
                    ComboBox_Shipment_DopRazdel_Uroven3.Items.Clear();
                    ComboBox_Shipment_DopRazdel_Uroven3.ResetText();
                    ComboBox_Shipment_DopRazdel_En.DataSource = null;
                    ComboBox_Shipment_DopRazdel_En.Items.Clear();
                    ComboBox_Shipment_DopRazdel_En.ResetText();

                    if (ComboBox_Shipment_DopRazdel_Name.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            ///заполнение выпадающего списка уровня 2
                            if (ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name)
                            {
                                if (!ComboBox_Shipment_DopRazdel_Uroven2.Items.Contains(ListDopOperation[i].Uroven2))
                                {
                                    ComboBox_Shipment_DopRazdel_Uroven2.Items.Add(ListDopOperation[i].Uroven2);
                                }

                                /// Уровень 3 заполняется в том случае не имеет привязку к Уровню 2
                                if (string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                {
                                    if (!ComboBox_Shipment_DopRazdel_Uroven3.Items.Contains(ListDopOperation[i].Uroven3))
                                    {
                                        ComboBox_Shipment_DopRazdel_Uroven3.Items.Add(ListDopOperation[i].Uroven3);
                                    }
                                }
                                //Заполняется если Уровнь 2 и 3 пустые
                                if (string.IsNullOrEmpty(ListDopOperation[i].Uroven2) && string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                {
                                    if (!ComboBox_Shipment_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                    {
                                        ComboBox_Shipment_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                    }

                                    SELECT_EN(ComboBox_Shipment_DopRazdel, ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel_Uroven2, ComboBox_Shipment_DopRazdel_Uroven3, ComboBox_Shipment_DopRazdel_En);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ComboBox_Shipment_DopRazdel_Uroven2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ComboBox_Shipment_DopRazdel.SelectedIndex != -1)
            {
                if (ComboBox_Shipment_DopRazdel_Uroven2.SelectedIndex != -1)
                {
                    if (ComboBox_Shipment_DopRazdel_Name.SelectedIndex != -1)
                    {
                        ComboBox_Shipment_DopRazdel_Uroven3.Items.Clear();
                        ComboBox_Shipment_DopRazdel_Uroven3.ResetText();
                        ComboBox_Shipment_DopRazdel_En.Items.Clear();
                        ComboBox_Shipment_DopRazdel_En.ResetText();

                        if (ComboBox_Shipment_DopRazdel_Name.SelectedIndex != -1)
                        {
                            if (ComboBox_Shipment_DopRazdel_Uroven2.SelectedIndex != -1)
                            {
                                //uroven3
                                for (int i = 0; i < ListDopOperation.Count; i++)
                                {
                                    if ((ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Shipment_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2))
                                    {
                                        if (!string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                        {
                                            if (!ComboBox_Shipment_DopRazdel_Uroven3.Items.Contains(ListDopOperation[i].Uroven3))
                                            {
                                                ComboBox_Shipment_DopRazdel_Uroven3.Items.Add(ListDopOperation[i].Uroven3);
                                            }
                                        }
                                    }
                                }

                                //en
                                for (int i = 0; i < ListDopOperation.Count; i++)
                                {
                                    if ((ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Shipment_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                        (string.IsNullOrEmpty(ListDopOperation[i].Uroven3)))
                                    {
                                        if (!string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                        {
                                            if (!ComboBox_Shipment_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                            {
                                                ComboBox_Shipment_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                            }

                                            SELECT_EN(ComboBox_Shipment_DopRazdel, ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel_Uroven2, ComboBox_Shipment_DopRazdel_Uroven3, ComboBox_Shipment_DopRazdel_En);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ComboBox_Shipment_DopRazdel_Uroven3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox_Shipment_DopRazdel_En.Items.Clear();
            ComboBox_Shipment_DopRazdel_En.ResetText();

            if (ComboBox_Shipment_DopRazdel_Name.SelectedIndex != -1)
            {

                if (ComboBox_Shipment_DopRazdel_Uroven2.SelectedIndex != -1)
                {
                    if (ComboBox_Shipment_DopRazdel_Uroven3.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Shipment_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                (ComboBox_Shipment_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Shipment_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Shipment_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Shipment_DopRazdel, ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel_Uroven2, ComboBox_Shipment_DopRazdel_Uroven3, ComboBox_Shipment_DopRazdel_En);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Shipment_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                (string.IsNullOrEmpty(ComboBox_Shipment_DopRazdel_Uroven3.SelectedItem.ToString())))
                            {
                                if (!ComboBox_Shipment_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Shipment_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Shipment_DopRazdel, ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel_Uroven2, ComboBox_Shipment_DopRazdel_Uroven3, ComboBox_Shipment_DopRazdel_En);
                            }
                        }
                    }
                }
                else
                {
                    if (ComboBox_Shipment_DopRazdel_Uroven3.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (string.IsNullOrEmpty(ListDopOperation[i].Uroven2)) &&
                                (ComboBox_Shipment_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Shipment_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Shipment_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Shipment_DopRazdel, ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel_Uroven2, ComboBox_Shipment_DopRazdel_Uroven3, ComboBox_Shipment_DopRazdel_En);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Shipment_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (string.IsNullOrEmpty(ComboBox_Shipment_DopRazdel_Uroven2.SelectedItem.ToString())) &&
                                (ComboBox_Shipment_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Shipment_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Shipment_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Shipment_DopRazdel, ComboBox_Shipment_DopRazdel_Name, ComboBox_Shipment_DopRazdel_Uroven2,ComboBox_Shipment_DopRazdel_Uroven3,ComboBox_Shipment_DopRazdel_En);
                            }
                        }
                    }
                }
            }
        }

        private void SELECT_EN(ComboBox cmd_razdel, ComboBox cmb_name, ComboBox cmb_ur2, ComboBox cmd_ur3, ComboBox cmd_en)
        {
            string Uroven2 = null;
            string Uroven3 = null;
            if (cmb_ur2.SelectedIndex != -1)
            {
                Uroven2 = cmb_ur2.SelectedItem.ToString();
            }

            if (cmd_ur3.SelectedIndex != -1)
            {
                Uroven3 = cmd_ur3.SelectedItem.ToString();
            }
            cmd_en.SelectedIndex = Class_SQL.SelectComboIndex(cmd_razdel.SelectedItem.ToString(), cmb_name.SelectedItem.ToString(), Uroven2, Uroven3);
        }


        private void ComboBox_Service_DopRazdel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DataGrid_Service_Table.CurrentRow.Cells["TID"].Value.ToString()))
            {
                if (ComboBox_Service_DopRazdel.SelectedIndex != -1)
                {

                    ComboBox_Service_DopRazdel_Name.DataSource = null;
                    ComboBox_Service_DopRazdel_Name.ResetText();

                    ComboBox_Service_DopRazdel_Uroven2.DataSource = null;
                    ComboBox_Service_DopRazdel_Uroven2.ResetText();

                    ComboBox_Service_DopRazdel_Uroven3.DataSource = null;
                    ComboBox_Service_DopRazdel_Uroven3.ResetText();

                    ComboBox_Service_DopRazdel_En.DataSource = null;
                    ComboBox_Service_DopRazdel_En.ResetText();

                    LOAD_DOP(ComboBox_Service_DopRazdel.SelectedItem.ToString(), ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel);

                }
            }
            else
            {
                MessageBox.Show("Поле Номер заявки не указан.", "ABS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            TextBox_Service_DopRazdel_Qty.Text = "0";
        }

        private void ComboBox_Service_DopRazdel_Name_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ComboBox_Service_DopRazdel.SelectedIndex != -1)
            {
                if (ComboBox_Service_DopRazdel_Name.SelectedIndex != -1)
                {
                    ComboBox_Service_DopRazdel_Uroven2.Items.Clear();
                    ComboBox_Service_DopRazdel_Uroven2.ResetText();
                    ComboBox_Service_DopRazdel_Uroven3.Items.Clear();
                    ComboBox_Service_DopRazdel_Uroven3.ResetText();
                    ComboBox_Service_DopRazdel_En.DataSource = null;
                    ComboBox_Service_DopRazdel_En.Items.Clear();
                    ComboBox_Service_DopRazdel_En.ResetText();

                    if (ComboBox_Service_DopRazdel_Name.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            ///заполнение выпадающего списка уровня 2
                            if (ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name)
                            {
                                if (!ComboBox_Service_DopRazdel_Uroven2.Items.Contains(ListDopOperation[i].Uroven2))
                                {
                                    ComboBox_Service_DopRazdel_Uroven2.Items.Add(ListDopOperation[i].Uroven2);
                                }

                                /// Уровень 3 заполняется в том случае не имеет привязку к Уровню 2
                                if (string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                {
                                    if (!ComboBox_Service_DopRazdel_Uroven3.Items.Contains(ListDopOperation[i].Uroven3))
                                    {
                                        ComboBox_Service_DopRazdel_Uroven3.Items.Add(ListDopOperation[i].Uroven3);
                                    }
                                }
                                //Заполняется если Уровнь 2 и 3 пустые
                                if (string.IsNullOrEmpty(ListDopOperation[i].Uroven2) && string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                {
                                    if (!ComboBox_Service_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                    {
                                        ComboBox_Service_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                    }

                                    SELECT_EN(ComboBox_Service_DopRazdel, ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel_Uroven2, ComboBox_Service_DopRazdel_Uroven3, ComboBox_Service_DopRazdel_En);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ComboBox_Service_DopRazdel_Uroven2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ComboBox_Service_DopRazdel.SelectedIndex != -1)
            {
                if (ComboBox_Service_DopRazdel_Uroven2.SelectedIndex != -1)
                {
                    if (ComboBox_Service_DopRazdel_Name.SelectedIndex != -1)
                    {
                        ComboBox_Service_DopRazdel_Uroven3.Items.Clear();
                        ComboBox_Service_DopRazdel_Uroven3.ResetText();
                        ComboBox_Service_DopRazdel_En.Items.Clear();
                        ComboBox_Service_DopRazdel_En.ResetText();

                        if (ComboBox_Service_DopRazdel_Name.SelectedIndex != -1)
                        {
                            if (ComboBox_Service_DopRazdel_Uroven2.SelectedIndex != -1)
                            {
                                //uroven3
                                for (int i = 0; i < ListDopOperation.Count; i++)
                                {
                                    if ((ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Service_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2))
                                    {
                                        if (!string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                        {
                                            if (!ComboBox_Service_DopRazdel_Uroven3.Items.Contains(ListDopOperation[i].Uroven3))
                                            {
                                                ComboBox_Service_DopRazdel_Uroven3.Items.Add(ListDopOperation[i].Uroven3);
                                            }
                                        }
                                    }
                                }

                                //en
                                for (int i = 0; i < ListDopOperation.Count; i++)
                                {
                                    if ((ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Service_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                        (string.IsNullOrEmpty(ListDopOperation[i].Uroven3)))
                                    {
                                        if (!string.IsNullOrEmpty(ListDopOperation[i].Uroven2))
                                        {
                                            if (!ComboBox_Service_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                            {
                                                ComboBox_Service_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                            }

                                            SELECT_EN(ComboBox_Service_DopRazdel, ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel_Uroven2, ComboBox_Service_DopRazdel_Uroven3, ComboBox_Service_DopRazdel_En);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        private void ComboBox_Service_DopRazdel_Uroven3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox_Service_DopRazdel_En.Items.Clear();
            ComboBox_Service_DopRazdel_En.ResetText();

            if (ComboBox_Service_DopRazdel_Name.SelectedIndex != -1)
            {

                if (ComboBox_Service_DopRazdel_Uroven2.SelectedIndex != -1)
                {
                    if (ComboBox_Service_DopRazdel_Uroven3.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Service_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                (ComboBox_Service_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Service_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Service_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Service_DopRazdel, ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel_Uroven2, ComboBox_Service_DopRazdel_Uroven3, ComboBox_Service_DopRazdel_En);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (ComboBox_Service_DopRazdel_Uroven2.SelectedItem.ToString() == ListDopOperation[i].Uroven2) &&
                                (string.IsNullOrEmpty(ComboBox_Shipment_DopRazdel_Uroven3.SelectedItem.ToString())))
                            {
                                if (!ComboBox_Service_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Service_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Service_DopRazdel, ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel_Uroven2, ComboBox_Service_DopRazdel_Uroven3, ComboBox_Service_DopRazdel_En);
                            }
                        }
                    }
                }
                else
                {
                    if (ComboBox_Service_DopRazdel_Uroven3.SelectedIndex != -1)
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (string.IsNullOrEmpty(ListDopOperation[i].Uroven2)) &&
                                (ComboBox_Service_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Service_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Service_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Service_DopRazdel, ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel_Uroven2, ComboBox_Service_DopRazdel_Uroven3, ComboBox_Service_DopRazdel_En);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListDopOperation.Count; i++)
                        {
                            if ((ComboBox_Service_DopRazdel_Name.SelectedItem.ToString() == ListDopOperation[i].Name) && (string.IsNullOrEmpty(ComboBox_Service_DopRazdel_Uroven2.SelectedItem.ToString())) &&
                                (ComboBox_Service_DopRazdel_Uroven3.SelectedItem.ToString() == ListDopOperation[i].Uroven3))
                            {
                                if (!ComboBox_Service_DopRazdel_En.Items.Contains(ListDopOperation[i].En))
                                {
                                    ComboBox_Service_DopRazdel_En.Items.Add(ListDopOperation[i].En);
                                }

                                SELECT_EN(ComboBox_Service_DopRazdel, ComboBox_Service_DopRazdel_Name, ComboBox_Service_DopRazdel_Uroven2, ComboBox_Service_DopRazdel_Uroven3, ComboBox_Service_DopRazdel_En);
                            }
                        }
                    }
                }
            }
        }

        private void СтатОтчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://aurora/Reports/Pages/Report.aspx?ItemPath=%2f%d0%90%d0%bd%d0%b0%d0%bb%d0%b8%d1%82%d0%b8%d0%ba%d0%b0%2f%d0%a1%d1%82%d0%b0%d1%82%d0%be%d1%82%d1%87%d0%b5%d1%82&SelectedSubTabId=GenericPropertiesTab");
        }

        private void ОтчетПоДопоперациямToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://aurora/Reports/Pages/Report.aspx?ItemPath=%2f%d0%90%d0%bd%d0%b0%d0%bb%d0%b8%d1%82%d0%b8%d0%ba%d0%b0%2f%d0%9e%d1%82%d1%87%d0%b5%d1%82+%d0%bf%d0%be+%d0%b4%d0%be%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d0%b5%d0%bb%d1%8c%d0%bd%d1%8b%d0%bc+%d0%be%d0%bf%d0%b5%d1%80%d0%b0%d1%86%d0%b8%d1%8f%d0%bc+%d0%b2+%d0%90%d0%91%d0%a1&ViewMode=Detail");
        
        }


        private void Contex_menu_receipt_item_transact_Click(object sender, EventArgs e)
        {

            if (DataGrid_Receipt_Table.SelectedRows.Count > 0)
            {
                string stTid = DataGrid_Receipt_Table.CurrentRow.Cells["TID"].Value.ToString();
                string stOrderName = DataGrid_Receipt_Table.CurrentRow.Cells["ORDER_NUMBER"].Value.ToString();
                Form_Transaction ft = new Form_Transaction(stTid,  stOrderName);
                ft.Show();
                
            }          
        }

        private void Contex_menu_shipment_item_transact_Click(object sender, EventArgs e)
        {
            string stTid = DataGrid_Shipment_Table.CurrentRow.Cells["TID"].Value.ToString();
            string stOrderName = DataGrid_Shipment_Table.CurrentRow.Cells["ORDER_NUMBER"].Value.ToString();
            Form_Transaction ft = new Form_Transaction(stTid, stOrderName);
            ft.Show();
        }



        public void LOAD_DOP(string NameRazdel, ComboBox NameBox , ComboBox namePanel)
        {

            //Console.WriteLine("Наименование раздела =  " + NameRazdel + "  Combobox =  " + NameBox.Name);

           SqlCommand sqlCommand = null;
            SqlDataReader sqlReader = null;

            string StringCommand = null;

            ListDopOperation.Clear();
            NameBox.DataSource = null;
            NameBox.Items.Clear();

            if (namePanel.SelectedIndex != -1)
            {
                switch (namePanel.SelectedItem.ToString())
                {
                    case "Услуга":
                        StringCommand = "select  a.name as NAME, POD_VID as UROVEN2,  LEVEL_3 as UROVEN3, e.NAME as EN  from bi.dbo.ABS_AMENITIES a join bi.dbo.ABS_EN e on a.KEY_EN=e.KEY_ID " +
                        "                       where a.ACTIVE = 'Y'  order by a.name, POD_VID, LEVEL_3, e.NAME";
                        break;
                    case "Материал":
                        StringCommand = "select a.name as NAME, POD_NAME as UROVEN2, isnull(a.Uroven3, '') as UROVEN3,e.NAME as EN  from bi.dbo.ABS_MATERIAL a join bi.dbo.ABS_EN e on a.KEY_EN = e.KEY_ID " +
                                        " where a.ACTIVE = 'Y'  order by a.name, POD_NAME, isnull(a.Uroven3, '') ,e.NAME";

                        break;
                    case "Операция":
                        StringCommand = "select  a.name as NAME, DOP_NAME as UROVEN2, '' as UROVEN3,e.NAME as EN  from bi.dbo.ABS_OPERATION a join bi.dbo.ABS_EN e on a.KEY_EN=e.KEY_ID " +
                                        " where a.ACTIVE = 'Y' order by a.name, DOP_NAME, e.NAME";

                        break;
                    case "Первичная агрегация":
                        StringCommand = "select  a.name as NAME, isnull(a.UROVEN2,'') as UROVEN2, isnull(UROVEN3,'') as UROVEN3,e.NAME as EN  from bi.dbo.ABS_FIRST_AREG a join bi.dbo.ABS_EN e on a.KEY_EN=e.KEY_ID " +
                                        " where a.ACTIVE = 'Y' order by a.name, e.NAME, a.UROVEN2, a.UROVEN3";
                        break;

                    case "Обработка сериал.продук.":
                        StringCommand = "select  a.name as NAME,isnull(a.UROVEN2,'') as UROVEN2, isnull(UROVEN3,'') as UROVEN3, e.NAME as EN  from bi.dbo.ABS_SERIAL_PROD a join bi.dbo.ABS_EN e on a.KEY_EN=e.KEY_ID " +
                                        " where a.ACTIVE = 'Y' order by a.name, e.NAME, a.UROVEN2, a.UROVEN3";
                        break;
                    default:
                        break;
                }

                using (SqlCommand command = new SqlCommand(StringCommand, Class_SQL.myConnection))
                {
                    DataTable dt = new DataTable();

                    if (Class_SQL.myConnection.State != System.Data.ConnectionState.Open)
                        Class_SQL.myConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dt.Load(reader);

                        ListDopOperation = dt.AsEnumerable().Select(se => new TableListDopOperation() { Name = se.Field<string>("Name"), Uroven2 = se.Field<string>("Uroven2"), Uroven3 = se.Field<string>("Uroven3"), En = se.Field<string>("En") }).ToList();
                    }
                }

                Class_SQL.myConnection.Close();

                for (int i = 0; i < ListDopOperation.Count; i++)
                {
                    if (!NameBox.Items.Contains(ListDopOperation[i].Name))
                    {
                        NameBox.Items.Add(ListDopOperation[i].Name);                        
                    }                    
                }
                NameBox.SelectedIndex = -1;
            }
        }

        private void ComboBox_Shipment_DopRazdel_Name_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void БиблиотекаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_library fs = new Form_library();
            fs.Show();
        }

        private void ВыходToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Class_SQL.CloseConnection();
            Application.Exit();
        }

        private void ДобавитьToolStripMenuItem3_Click(object sender, EventArgs e)
        {

            Form_Service_Add fs = new Form_Service_Add();
            fs.Show();

        }

        private void СтатОтчетToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://aurora/Reports/Pages/Report.aspx?ItemPath=%2f%d0%90%d0%bd%d0%b0%d0%bb%d0%b8%d1%82%d0%b8%d0%ba%d0%b0%2fABS%2f%d0%a1%d1%82%d0%b0%d1%82%d0%be%d1%82%d1%87%d0%b5%d1%82");
        }

        private void ОтчетПоДопоперациямToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://aurora/Reports/Pages/Report.aspx?ItemPath=%2f%d0%90%d0%bd%d0%b0%d0%bb%d0%b8%d1%82%d0%b8%d0%ba%d0%b0%2fABS%2f%d0%9e%d1%82%d1%87%d0%b5%d1%82+%d0%bf%d0%be+%d0%b4%d0%be%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d0%b5%d0%bb%d1%8c%d0%bd%d1%8b%d0%bc+%d0%be%d0%bf%d0%b5%d1%80%d0%b0%d1%86%d0%b8%d1%8f%d0%bc+%d0%b2+%d0%90%d0%91%d0%a1");
        }

        private void ComboBox_Shipment_DopRazdel_Uroven2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ДетализированныйОтчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://aurora/Reports/Pages/Report.aspx?ItemPath=%2f%d0%90%d0%bd%d0%b0%d0%bb%d0%b8%d1%82%d0%b8%d0%ba%d0%b0%2fABS%2f%d0%94%d0%b5%d1%82%d0%b0%d0%bb%d0%b8%d0%b7%d0%b8%d1%80%d0%be%d0%b2%d0%b0%d0%bd%d0%bd%d1%8b%d0%b9+%d0%be%d1%82%d1%87%d1%91%d1%82&ViewMode=Detail");
        }

        private void TextBox_Shipment_Count_All_Pallet_TextChanged(object sender, EventArgs e)
        {
            TextBox_Shipment_Count_Pallet.Text = TextBox_Shipment_Count_All_Pallet.Text;
        }

        private void ComboBox_Receipt_DopRazdel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}