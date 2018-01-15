using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImageResizer
{
    /// <summary>
    /// Интерфейс для осуществления записи в лог файл
    /// </summary>
    interface ILogWriter
    {
        /// <summary>
        /// Записать указанный текст в лог файл, после указания текущей даты и времени
        /// </summary>
        /// <param name="text">Текст для лог файла</param>
        void WriteLogText(string text);
    }

    /// <summary>
    /// Класс для осуществления записей в лог файл
    /// </summary>
    class LogWriter : ILogWriter, IDisposable
    {
        /// <summary>
        /// Путь к лог файлу
        /// </summary>
        public string LogPath { get; }
        StreamWriter writer;
        private string fileName = "Logs.txt";
        /// <summary>
        /// Имя лог файла
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnFileNameChange(fileName);
            }
        }


        
        /// <summary>
        ///  Инициализация пути к лог файлу. При необходимости указанный каталог создается
        /// </summary>
        /// <param name="path"></param>
        public LogWriter(string path)
        {
            LogPath = path;
            Directory.CreateDirectory(LogPath);
            writer = new StreamWriter(path + "\\" + FileName);
        }

        /// <summary>
        /// При изменении имени файла, меняется и поле StreamWriter
        /// </summary>
        /// <param name="fileName">Имя лог файла</param>
        void OnFileNameChange(string fileName)
        {
            writer = new StreamWriter(LogPath + fileName);
        }

        /// <summary>
        /// Записать в лог файл указанное сообщение с отметкой о текущих дате и времени
        /// </summary>
        /// <param name="text">Текст записи в лог файле</param>
        public void WriteLogText(string text)
        {
            writer.Write(DateTime.Now + ": ");
            writer.WriteLine(text + ".");
        }

        /// <summary>
        /// Закрытие лог файла
        /// </summary>
        public void Dispose()
        {
            writer.Close();
        }
    }
}
