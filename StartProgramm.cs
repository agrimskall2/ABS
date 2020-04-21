 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ABS_C
{
    class StartProgramm
    {

    
        public static void NewStartProgramm()
        {
            string line = null;

            string ipadress =null;

            try
            {

                //MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory + @"st.txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                using (var reader = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory+@"st.txt"))
                {

                    while ((line = reader.ReadLine()) != null)
                    {

                        ipadress = line;

                        Console.WriteLine("LINE = " + line);
                    }
                }

                if (!string.IsNullOrEmpty(ipadress))
                {
                    string localIP;
                    using (System.Net.Sockets.Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("8.8.8.8", 65530);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint.Address.ToString();

                        if (ipadress != localIP)
                        {

                            Application.Run(new RegForm());
                        }
                        else
                        {
                            Programm.Proverka();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error  ");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обработки комманды. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

         


    }
}
