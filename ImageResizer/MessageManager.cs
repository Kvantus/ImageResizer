using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageResizer
{
    /// <summary>
    /// Класс предоставляющий методы для выведения на консоль различных типов сообщений различным цветом
    /// </summary>
    static class MessageManager
    {
        /// <summary>
        /// Вывести сообщение об ошибке красным цветом
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Вывести сообщение о завершении какой-либо операции зеленым цветом
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void WriteOkMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Вывести предупреждение желтым цветом
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void WriteWarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Вывести информационное сообщение белым цветом
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void WriteInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
