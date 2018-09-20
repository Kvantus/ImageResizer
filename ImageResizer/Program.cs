using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Security.Principal;
using System.Security;
using System.DirectoryServices.AccountManagement;


namespace ImageResizer
{
    class Program
    {
        static string myProgram = AppDomain.CurrentDomain.FriendlyName;

        static void Main(string[] args)
        {
            if (DateTime.Today > new DateTime(2019, 04, 6))
            {
                return;
            }

            // проверка, является ли текущий пользователь сотрудников перечисленных подразделений
            UserVerificator verificator = new UserVerificator("Development Managers", "Base Managers");
            if (!verificator.IsUserValid())
            {
                MessageManager.WriteErrorMessage("NO!");
                Console.ReadKey();
                return;
            }

            Console.WindowWidth = 110;

            // получаем путь к рабочему столу и к рабочим папкам
            string myDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string inPath = myDesktop + "\\" + "InputImages";
            string outPath = myDesktop + "\\" + "OutputImages";

            using (LogWriter logWriter = new LogWriter(inPath))
            {                                            //игнорировать файлы - текущее приложение + логи
                Resizer.Resize(inPath, outPath, logWriter, myProgram, logWriter.FileName);
            }

            EndProgramm();
        }

        /// <summary>
        /// Завершение работы программы. Закрытие консоли с небольшой задержкой
        /// </summary>
        private static void EndProgramm()
        {
            MessageManager.WriteInfoMessage(new string('-', 50));
            MessageManager.WriteInfoMessage("Программа закроется через:");
            for (int i = 3; i > 0; i--)
            {
                Console.WriteLine(i);
                Thread.Sleep(1000);
            }
        }
    }
}
