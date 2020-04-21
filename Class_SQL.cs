using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Zuby.ADGV;

namespace ABS_C
{
    class Class_SQL
    {
        public static string connectString;

      public  static SqlConnection myConnection;

        static SqlCommand myCommandString;

        static SqlDataReader myDataReader;

        static string SqlServerName = "10.12.1.220";    //имя сервере

        static string SqlDbName = "BI"; ///имя подключаемое БД

        static string sqlUserName = "test_node";

        static String sqlPassword = "C375yj23Sa1";
 
        public static void OpenConnection()
        {
            try
            {
                connectString = "Data Source=" + SqlServerName + ";Initial Catalog=" + SqlDbName + ";User ID=" + sqlUserName + ";Password=" + sqlPassword + "";

                myConnection = new SqlConnection(connectString);

                myConnection.Open();

                Console.WriteLine("Подключение установлено.");
            }

                catch
            {
                Console.WriteLine("Подключение не установлено.");
            }
            
        }

        //Закрытие подключения к БД
        public static void CloseConnection()
        {
            myConnection.Close();
            Console.WriteLine("Подключение отключено.");
        }

        // 
        public static string  SelectString(string cmd)
        {
            SqlCommand command = new SqlCommand(cmd, myConnection);

            SqlDataReader reader = command.ExecuteReader();

            string  response = null;

            if (reader.Read())
            {
                response = reader[0].ToString();
                reader.Close();
            }
            
            reader.Close();

            return response;
        }

        public static double SelectFloat(string cmd)
        {
            SqlCommand command = new SqlCommand(cmd, myConnection);


            SqlDataReader reader = command.ExecuteReader();

            Double response = 0;

            if (reader.Read())
            {
                response = Convert .ToDouble(reader[0].ToString());
                reader.Close();
            }

            reader.Close();

            return response;
        }

        public static int SelectNextNumber()
        {
            int Number = 0;

            OpenConnection();
            SqlCommand command = new SqlCommand("exec bi.dbo.GetNextNumber @nextNumKey=N'ABS_Service_NUM'", myConnection);

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Number = Convert.ToInt32(reader[0].ToString());
                reader.Close();
            }

            reader.Close();

            CloseConnection();
            return Number;
        }
        class DoubleBufferedDataGridView : DataGridView
        {
            protected override bool DoubleBuffered { get => true; }
        }

        public static void LoadGridDB(DataGridView DataGridName, string cmd)
        {

            DataGridName.DataSource = null;
           
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd, myConnection);

                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridName.DataSource = dt;
                DataGridName. AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                DataGridName.AllowDrop = false;
                DataGridName.AllowUserToAddRows = false;
                DataGridName.AllowUserToDeleteRows = false;
                DataGridName.AllowUserToResizeColumns = true;
                DataGridName.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridName.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                DataGridName.RowHeadersVisible = false;              
                DataGridName.AllowUserToOrderColumns = false;
                DataGridName.AllowUserToResizeRows = false;
                DataGridName.ShowRowErrors = false;

                DataGridName.AllowUserToAddRows = false;
                DataGridName.AllowUserToDeleteRows = false;
                DataGridName.AllowUserToResizeRows = false;
                DataGridName.ReadOnly = true;
                DataGridName.RowHeadersVisible = false;
                DataGridName.MultiSelect = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузки данных. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void LoadComboBox(ComboBox comboName, string cmd)
        {
            
            try
            {

                SqlDataAdapter da = new SqlDataAdapter(cmd, myConnection);

                DataTable dt = new DataTable();

                da.Fill(dt);
                comboName.DataSource = dt;
                comboName.DisplayMember = "NAME";
                comboName.ValueMember = "ID";
                comboName.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузки данных. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           

        }

        public static void RunCommand(string cmd)
        {
            try
            {
                SqlCommand sc = new SqlCommand(cmd, myConnection);
                sc.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void LoadComboBoxDopRazdel(ComboBox ComboName)
        {
            ComboName.Items.Clear();
            ComboName.Items.Add("Материал");
            ComboName.Items.Add("Операция");
            ComboName.Items.Add("Услуга");
            ComboName.Items.Add("Первичная агрегация");
            ComboName.Items.Add("Обработка сериализованной продук.");
            ComboName.SelectedIndex = -1;            
        }

        public static void LoadComboBoxStatus(ComboBox ComboName)
        {
            ComboName.Items.Clear();
            ComboName.Items.Add("Зарегистрировано");
            ComboName.Items.Add("ТС прибыло");
            ComboName.Items.Add("В обработке");
            ComboName.Items.Add("Готов к отгрузке");
            ComboName.Items.Add("Выполнено");
            ComboName.Items.Add("Расформировано");
            ComboName.Items.Add("Удалено");        
            ComboName.SelectedIndex = -1;
        }

        internal static void LoadGridDB(AdvancedDataGridView dataGrid_Shipment_Table, string v1, string v2)
        {
            throw new NotImplementedException();
        }



        public static void Update_sevise(int tid, string status, int total_line, string receipt_date_time, string edit_user, string messanger)
        {

            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand("ABS_UPDATE_SERVICE", myConnection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TID", tid);
                command.Parameters.AddWithValue("@STATUS", status);
                command.Parameters.AddWithValue("@TOTAL_LINES", total_line);
                command.Parameters.AddWithValue("@RECIPT_DATE_TIME", receipt_date_time);
                command.Parameters.AddWithValue("@EDIT_USER", edit_user);
                command.Parameters.AddWithValue("@messanger", messanger);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                CloseConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static void Insert_Service(string NameType, string NameOrder, string Company, string Warehouse, string Osnovanie, string Messanger, string DateOrder, string Login)
        {
            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand("ABS_INSERT_SERVICE", myConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NameType", NameType);
                command.Parameters.AddWithValue("@NameNumberOrders", NameOrder);
                command.Parameters.AddWithValue("@Warehouse", Warehouse);
                command.Parameters.AddWithValue("@Company", Company);
                command.Parameters.AddWithValue("@osnovanie", Osnovanie);
                command.Parameters.AddWithValue("@messanger", Messanger);
                command.Parameters.AddWithValue("@DateOrders", DateOrder);
                command.Parameters.AddWithValue("@logins", Login);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Insert_Dop_Receipt(string OrderNumber, string Razdel, string NameRazdel, string Uroven2, string Uroven3, decimal Qty, string En, string UserName, decimal UserDef)
        {
            try
            {
                OpenConnection();
                SqlCommand command = new SqlCommand("ABS_INSERT_DOP_RAZDEL", myConnection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@order_number", OrderNumber);
                command.Parameters.AddWithValue("@razdel", Razdel);
                command.Parameters.AddWithValue("@descr", NameRazdel);
                command.Parameters.AddWithValue("@uroven2", Uroven2);
                command.Parameters.AddWithValue("@uroven3", Uroven3);
                command.Parameters.AddWithValue("@qty", Qty);
                command.Parameters.AddWithValue("@en", En);
                command.Parameters.AddWithValue("@user_name", UserName);
                command.Parameters.AddWithValue("@user_def", UserDef);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Execute_Library_En(string name_library, string type_library, decimal key_id, string name, string uroven2, string uroven3, string active, string userName, decimal key_en, decimal tid)
        {
            try
            {

                OpenConnection();
                SqlCommand cmd = new SqlCommand("ABS_LIB", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name_library", name_library);
                cmd.Parameters.AddWithValue("@type_library", type_library);
                cmd.Parameters.AddWithValue("@key_id", key_id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@uroven2", uroven2);
                cmd.Parameters.AddWithValue("@uroven3", uroven3);
                cmd.Parameters.AddWithValue("@active", active);
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@key_en", key_en);
                cmd.Parameters.AddWithValue("@tid", tid);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                CloseConnection();
            } catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Save_Shipment_Order(decimal tid,  string status, int total_line, string plan_ship_date, string fact_ship_date, string actual_ondock_ats_date, string actual_arrival_ats_date,
                                                int count_term_pallet, int count_mix_pallet, int count_all_box, string compl_load_ats_date, int count_pallet, 
                                                int count_ros_pallet, string users, string messanger, string edit_user, int count_all_pallet, int count_term_box, decimal user_def8)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("ABS_UPDATE_SHIPMENT", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TID", tid);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@TOTAL_LINES", total_line);
                cmd.Parameters.AddWithValue("@PLAN_SHIPMEN_DATE_TIME", plan_ship_date);
                cmd.Parameters.AddWithValue("@FACT_SHIPMENT_DATE_TIME", fact_ship_date);
                cmd.Parameters.AddWithValue("@ACTUAL_ONDOCK_ATS_DATE_TIME", actual_ondock_ats_date);
                cmd.Parameters.AddWithValue("@ACTUAL_ARRIVAL_ATS_DATE_TIME", actual_arrival_ats_date);
                cmd.Parameters.AddWithValue("@COUNT_TERM_pallet", count_term_pallet);
                cmd.Parameters.AddWithValue("@COUNT_MIX_PALLET", count_mix_pallet);
                cmd.Parameters.AddWithValue("@COUNT_ALL_BOX", count_all_box);
                cmd.Parameters.AddWithValue("@COMPLETION_LOADING_ATS_DATE_TIME", compl_load_ats_date);
                cmd.Parameters.AddWithValue("@COUNT_PALLET", count_pallet);
                cmd.Parameters.AddWithValue("@COUNT_PALLET_ROS", count_ros_pallet);
                cmd.Parameters.AddWithValue("@USERS", users);
                cmd.Parameters.AddWithValue("@MESSANGER", messanger);
                cmd.Parameters.AddWithValue("@EDIT_USER", edit_user);
                cmd.Parameters.AddWithValue("@COUNT_ALL_PALLET", count_all_pallet);
                cmd.Parameters.AddWithValue("@COUNT_TERM_BOX", count_term_box);                
                cmd.Parameters.AddWithValue("@USER_DEF8", user_def8);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                CloseConnection();


            }
            catch ( Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Save_Receipt_Order(int TID,  string login, string status, string GruzName, int total_line, string plan_arrival_date,int count_pallet, int count_ros_box,
                                               string FactArrivalAtsDateTime, string ActualOnDockAts,
                                              string ComplDateTime, string AccepDateTime, string messanger, string user_name, string invoice, decimal user_def8)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("ABS_UPDATE_RECEIPT", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tid", TID);
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@gruzName", GruzName);
                cmd.Parameters.AddWithValue("@totalLines", total_line);
                cmd.Parameters.AddWithValue("@PlanArrivalDateTime", plan_arrival_date);
                cmd.Parameters.AddWithValue("@CountPallet", count_pallet);
                cmd.Parameters.AddWithValue("@CountBosRos", count_ros_box);
                cmd.Parameters.AddWithValue("@FactArrivalAtsDateTime", FactArrivalAtsDateTime);
                cmd.Parameters.AddWithValue("@ActualOnDockAts", ActualOnDockAts);
                cmd.Parameters.AddWithValue("@ComplDateTime", ComplDateTime);
                cmd.Parameters.AddWithValue("@AccepDateTime", AccepDateTime);
                cmd.Parameters.AddWithValue("@Messanger", messanger);
                cmd.Parameters.AddWithValue("@UserName", user_name);
                cmd.Parameters.AddWithValue("@Invoice", invoice);
                cmd.Parameters.AddWithValue("@user_def8", user_def8);         
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Save_Service_Order(decimal tid, string status, int total_line, string receipt_date, string user_edit, decimal user_def8,  string messanger)
        {
            try
            {
            
                OpenConnection();
                SqlCommand cmd = new SqlCommand("ABS_UPDATE_SERVICE", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TID", tid);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@TOTAL_LINES", total_line);
                cmd.Parameters.AddWithValue("@RECIPT_DATE_TIME", receipt_date);
                cmd.Parameters.AddWithValue("@EDIT_USER", user_edit);                
                cmd.Parameters.AddWithValue("@user_def8", user_def8);
                cmd.Parameters.AddWithValue("@messanger", messanger);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                CloseConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static int SelectComboIndex(string Razdel, string Name, string Uroven2, string Uroven3)
        {

            int response = -1 ;
            //Console.WriteLine("Razdel = " + Razdel);
            //Console.WriteLine("Name  = " + Name);
            //Console.WriteLine("Uroven 2 = " + Uroven2);
            //Console.WriteLine("Uroven 3 = " + Uroven3);

            if (Razdel == "Материал")
            {
                if (Name != "System.Data.DataRowView" && !string.IsNullOrEmpty(Name))
                {
                    OpenConnection();

                    if (Convert.ToInt32(SelectFloat("select COUNT(*) from  bi.dbo.ABS_MATERIAL where name = N'" + Name + "' and SELECT_ACTIV ='Y' and POD_NAME = N'" + Uroven2 + "' and uroven3=N'"+ Uroven3 +"'")) == 1)
                    {
                        response = 0;
                    }
                    CloseConnection();
                }
            }
            else if (Razdel == "Операция")
            {
                if (Name != "System.Data.DataRowView" && !string.IsNullOrEmpty(Name))
                {
                    OpenConnection();
                    if (Convert.ToInt32(SelectFloat("select COUNT(*) from  bi.dbo.ABS_OPERATION where name = N'" + Name + "' and SELECT_ACTIV ='Y' and dop_name = N'" + Uroven2 + "'")) == 1)
                    {
                        response = 0;
                    }

                    CloseConnection();
                }                
            }
            else if (Razdel == "Услуга")
            {
                if (Name != "System.Data.DataRowView" && !string.IsNullOrEmpty(Name))
                {
                    OpenConnection();

                    if (Convert.ToInt32(SelectFloat("select COUNT(*) from  bi.dbo.ABS_AMENITIES where name = N'" + Name + "' and SELECT_ACTIV ='Y' and pod_vid = N'" + Uroven2 + "' and level_3 = N'" + Uroven3 + "'")) == 1)
                    {
                        response = 0;
                    }
                    CloseConnection();
                }
            }
            else if (Razdel == "Первичная агрегация")
            {
                if (Name != "System.Data.DataRowView" && !string.IsNullOrEmpty(Name))
                {
                    OpenConnection();

                    if (Convert.ToInt32(SelectFloat("select COUNT(*) from  bi.dbo.ABS_FIRST_AREG where name = N'" + Name + "' and SELECT_ACTIV ='Y' and uroven2 = N'" + Uroven2 + "' and uroven3 = N'" + Uroven3 + "'")) == 1)
                    {
                        response = 0;
                    }
                    CloseConnection();
                }
            }
            else if (Razdel == "Обработка сериализованной продук.")
            {
                if (Name != "System.Data.DataRowView" && !string.IsNullOrEmpty(Name))
                {
                    OpenConnection();

                    if (Convert.ToInt32(SelectFloat("select COUNT(*) from  bi.dbo.ABS_SERIAL_PROD where name = N'" + Name + "' and SELECT_ACTIV ='Y' and uroven2 = N'" + Uroven2 + "' and uroven3 = N'" + Uroven3 + "'")) == 1)
                    {
                        response = 0;
                    }
                    CloseConnection();
                }
            }
            //Console.WriteLine("response = " + response);
            return response;
        }


        public static void Del_Receipt_Dop(string tid , string UserName)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("bi.dbo.ABS_DELETE_DOP_RAZDEL", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@num_rows", tid);
                cmd.Parameters.AddWithValue("@user_name", UserName);
                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Insert_Dop_Ship_streich(decimal tid, decimal user_def8, string UserName, int countPalet )
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("bi.dbo.ABS_AVTO_INSERT_DOP_OPERATION", myConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tid", tid);
                cmd.Parameters.AddWithValue("@user_def8", user_def8);
                cmd.Parameters.AddWithValue("@User_name", UserName);
                cmd.Parameters.AddWithValue("@count_pallet", countPalet);
                cmd.ExecuteNonQuery();
                CloseConnection();

            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ReopenUserArhiv(string UserName)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand("EXEC aurora.SNT.DBO.SP_RESTORE_USER_PROFILE @USER_NAME = N'"+UserName +"'");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("", UserName);
                cmd.BeginExecuteNonQuery();

                CloseConnection();
            }
            catch(Exception ex)
            {

            }
        }


        public static string StringActivCompany(string UserName)
        {
            string StringActivUserCompany = null;

            OpenConnection();
            
            SqlCommand cmd = new SqlCommand("select company from aurora.snt.dbo.COMPANY_ACCESS_ALL where USER_NAME = N'" + UserName + "'", myConnection);

            SqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {              
                if (string.IsNullOrEmpty(StringActivUserCompany))
                {
                    StringActivUserCompany += "'"+ (string)dataReader["company"]+"'";
                }
                else
                {
                    StringActivUserCompany += ", '" + (string)dataReader["company"]+"'";
                }
            }

            dataReader.Close();
            CloseConnection();

            if (string.IsNullOrEmpty(StringActivUserCompany))
            {
                StringActivUserCompany = "'T1'";
            }

            return StringActivUserCompany;
        }

        public static string StringActivWarehouse(string UserName)
        {
            string StringActivUserWarehouse = null;

            OpenConnection();

            SqlCommand cmd = new SqlCommand("select warehouse from aurora.snt.dbo.WAREHOUSE_ACCESS_ALL where USER_NAME = N'" + UserName + "'", myConnection);

            SqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                if (string.IsNullOrEmpty(StringActivUserWarehouse))
                {
                    StringActivUserWarehouse += "'" + (string)dataReader["warehouse"] + "'";
                }
                else
                {
                    StringActivUserWarehouse += ", '" + (string)dataReader["warehouse"] + "'";
                }
            }

            dataReader.Close();
            CloseConnection();

            if (string.IsNullOrEmpty(StringActivUserWarehouse))
            {
                StringActivUserWarehouse = "'T1'";
            }

            return StringActivUserWarehouse;
        }


    }
}
