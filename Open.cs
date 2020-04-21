using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Principal;
using System.Windows.Forms;

namespace ABS_C
{
    class Programm
    {
       public static void Proverka()
        {
 
            





            if (ProverkaUser(UserName()) == 1)
            {
                
                Application.Run(new FirstForm(UserName()));
                                             
                


            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Пользователь " +UserName() +" не имеет доступа.", "АБС", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

        }


        private static string UserName()
        {

            Class_SQL.OpenConnection();

            IIdentity identity = WindowsIdentity.GetCurrent();
            string ManhUser = (identity.Name).Substring(((identity.Name)).IndexOf(@"\") + 1, (identity.Name).Length - (((identity.Name)).IndexOf(@"\") + 1));

            return ManhUser;
        }
        


        private static double ProverkaUser(string UserName)
        {
            double Result = 0;

            Result = Class_SQL.SelectFloat("select count(*) from bi.dbo.ABS_USER_PROFILE where active= 'Y' and user_name=N'" + UserName + "'");


            Console.WriteLine("Уровень доступа =  "  + Result);
            return Result;
        }



    }
}
