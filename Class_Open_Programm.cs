using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
///   проверка запущеного приложения ранее у пользователя
/// </summary>
namespace ABS_C
{
    class Class_Open_Programm
    {
        static int maxClientCount = 5;
        public static int StartProcess (){

            int Result = 0;

                foreach (var process in System.Diagnostics.Process.GetProcessesByName("ABS_C"))
            {

                Result =  1;

                Console.WriteLine("Приложение запущено  под пользователем = " + Environment.UserName);


                //process.Kill();
            }


            //bool semaphoreWasCreated; //Был ли создан семафор в данном потоке
            //Semaphore sem = new Semaphore(maxClientCount - 1, maxClientCount, "ABS_C",
            //    out semaphoreWasCreated);



            return Result;
        }


    }
}
