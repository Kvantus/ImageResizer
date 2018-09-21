using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ImageResizer
{
    /// <summary>
    /// Класс, предназначенный для уменьшения размеров файлов изображений
    /// </summary>
    static class Resizer
    {
        /// <summary>
        /// Метод уменьшает размер картинок в одной папке и выдает результирующие файлы в другой
        /// </summary>
        /// <param name="inPath">Папка, содержащая файлы изображений для сжатия</param>
        /// <param name="outPath">Папка, куда необходимо поместить уменьшенные файлы изображений</param>
        /// <param name="logWriter">Интерфейс для записи логов работы программы</param>
        /// <param name="ignoringFiles">Строковой массив с именами файлов, которые необходимо игнорировать (не уменьшать и не удалять по завершении)</param>
        public static void Resize(string inPath, string outPath, ILogWriter logWriter, params string[] ignoringFiles)
        {
            long limit = 1048576; // размер в байтах. если файл меньше указанного размера, уменьшение не происходит

            // предварительное создание папок, если их не существует
            if (!Directory.Exists(inPath))
            {
                Directory.CreateDirectory(inPath);
            }
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }


            DirectoryInfo inDirectory = new DirectoryInfo(inPath);
            DirectoryInfo outDirectory = new DirectoryInfo(outPath);

            // Удаление файлов в результирующей папке
            foreach (var file in outDirectory.GetFiles())
            {
                file.Delete();
            }

            FileInfo[] myFiles = inDirectory.GetFiles("*.*", SearchOption.AllDirectories);

            // проверка совпадений переданных для игнорирования имен файлов и имен файлов в исходной папке
            var query = from x in myFiles
                        join y in ignoringFiles
                        on x.Name equals y
                        select y;

            // завершение работы метода, в случае, если кроме игнорируемых файлов в исходной папке больше ничего нет
            if (myFiles.Length == query.Count())
            {
                MessageManager.WriteErrorMessage($"В папке {inPath} нечего обрабатывать");
                logWriter.WriteLogText($"В папке {inPath} нечего обрабатывать");
                return;
            }


            foreach (FileInfo oldFile in myFiles)
            {
                // Если текущий файл в списке игнорируемых - переход на следующую итерацию
                if (ignoringFiles.Contains(oldFile.Name))
                {
                    continue;
                };

                // Если файл весит меньше лимита, выводится сообщение и переходим к следующей итерации
                if (oldFile.Length < limit)
                {
                    string logSmallFile = $"Файл {oldFile.Name} весит {oldFile.Length / 1024}кб. Не обработан.";
                    MessageManager.WriteWarningMessage(logSmallFile);

                    logWriter.WriteLogText(logSmallFile);

                    File.Move(oldFile.FullName, $"{outPath}\\{oldFile.Name}"); // перемещение файла в новую папку.
                    continue;
                }

                Bitmap oldImage = null;
                try
                {
                    oldImage = new Bitmap(oldFile.FullName); // попытка опознать текущий файл, как изображение
                }
                catch (Exception ex)
                {
                    string logError = $"Файл \"{oldFile.Name}\" не получилось обработать, ошибка: {ex.Message}";

                    MessageManager.WriteErrorMessage(logError);
                    logWriter.WriteLogText(logError);

                    File.Move(oldFile.FullName, $"{outPath}\\{oldFile.Name}"); // перемещение файла в новую папку.
                    continue;
                }

                int height = oldImage.Height;
                int width = oldImage.Width;

                // в зависимости от размера файла, получаем для него новые ширину и высоту, уменьшенные на соответствующую величину
                if (oldFile.Length < 2 * limit)
                {
                    DecreaceSize(ref height, ref width, 2);
                }
                else if (oldFile.Length < 3 * limit)
                {
                    DecreaceSize(ref height, ref width, 2.20);
                }
                else if (oldFile.Length < 5 * limit)
                {
                    DecreaceSize(ref height, ref width, 2.50);
                }
                else if (oldFile.Length < 15 * limit)
                {
                    DecreaceSize(ref height, ref width, 2.7);
                }
                else
                {
                    DecreaceSize(ref height, ref width, 3);
                }

                // изменение разрешения картинки отменяем
                //float hor = oldImage.HorizontalResolution;
                //float vert = oldImage.VerticalResolution;
                //oldImage.SetResolution(hor / 2f, vert / 2f);

                Size mySize = new Size(width, height);

                Bitmap newImageFile = new Bitmap(oldImage, mySize); // создание нового изображения на основании старого с новыми шириной и высотой

                string oldFileReplaced = oldFile.Name.Replace(oldFile.Extension, ".JPEG");
                string newFile = $"{outPath}\\{oldFileReplaced}";
                newImageFile.Save(newFile, ImageFormat.Jpeg);
                FileInfo fileJustSaved = new FileInfo(newFile);

                oldImage.Dispose();

                string logSuccess = $"Файл \"{oldFile.Name}\" обработан. " +
                    $"Было {oldFile.Length / 1024}кб, стало {fileJustSaved.Length / 1024}кб";
                MessageManager.WriteOkMessage(logSuccess);
                logWriter.WriteLogText(logSuccess);
            }

            // в конце удалаем все файлы из исходной папки, кроме игнорируемых
            foreach (FileInfo oldFile in myFiles)
            {
                if (!ignoringFiles.Contains(oldFile.Name))
                {
                    oldFile.Delete();
                }
            }

            DirectoryInfo[] oldDirs = inDirectory.GetDirectories();
            foreach (var dir in oldDirs)
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Метод принимает по ссылке 2 интовых аргумента (высота и ширина) и делит каждый из них на указанную величину
        /// </summary>
        /// <param name="height">Высота изображения</param>
        /// <param name="width">Ширина изображения</param>
        /// <param name="divider">Делитель, на который необходимо разделить переданные аргументы</param>
        static void DecreaceSize(ref int height, ref int width, double divider)
        {
            height = (int)(height / divider);
            width = (int)(width / divider);
        }
    }
}
