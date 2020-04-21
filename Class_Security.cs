using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABS_C
{
    class Class_Security
    {

        // Проверка уровня Администратора для снятия ограничения блокировок по заявкам
        public static int DostupAdmin(string UserName)
        {
            int Result = 0;
            int IdGroup = 0;

            Class_SQL.OpenConnection();

            IdGroup = Convert.ToInt32(Class_SQL.SelectString("select  isnull(SECURITY_GROUP_ID,1)  from  aurora.ils.dbo.USER_PROFILE where USER_NAME = N'" + UserName + "'"));

            if ((Class_SQL.SelectString("select isnull(SECURITY_GROUP,'n/a' ) from aurora.ils.dbo.SECURITY_GROUP where OBJECT_ID=" + IdGroup)) == "Administrators")
            {
                Result = 1;
            }

            Class_SQL.CloseConnection();
            return Result;
        }


        



    }
}
