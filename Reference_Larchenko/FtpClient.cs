using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Reference_Larchenko
{
    class FtpClient
    {
        //для роботи з БД
        SQLiteDataReader dataReader = null;
        SQLiteCommand command = null;
        //дані для авторизації на сервері
        private string password = string.Empty;
        private string userName = string.Empty;
        private string uri = string.Empty;
        //допоміжні поля для параметрів серверу
        public bool Passive = true;
        public bool Binary = true;
        public bool EnableSsl = false;
        public bool Hash = false;
        //конструктор
        public FtpClient() {
            GetLoginPassword(ref uri, ref userName, ref password);
        }
        //створення запиту
        private FtpWebRequest createRequest(string method)
        {
            return createRequest(uri, method);
        }
        //створення запиту
        private FtpWebRequest createRequest(string uri, string method)
        {
            var r = (FtpWebRequest)WebRequest.Create(uri); //створення шляху
            //допоміжні поля для підключення до серверу
            r.Credentials = new NetworkCredential(userName, password);
            r.Method = method;
            r.UseBinary = Binary;
            r.EnableSsl = EnableSsl;
            r.UsePassive = Passive;
            return r;
        }

        //завантадення даних на сервер
        public void UploadFile()
        {
            //отримання даних про сервер
            bool getDataFtp = GetLoginPassword(ref uri, ref userName, ref password);

            if (getDataFtp == false) //якщо данні надано з помилкою
            {
                return;
            }

            //створення директорії
            CheckDirForUpload();

            //перевірка файлу
            if (!File.Exists("Reference.db"))
            {
                MessageBox.Show("База даних не знайдена!","Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ///робимо копію бази даних
            using (var location = new SQLiteConnection(@"Data Source=Reference.db; Version=3;"))
            using (var destination = new SQLiteConnection(@"Data Source=backupDb.db; Version=3;"))
            {
                location.Open();
                destination.Open();
                location.BackupDatabase(destination, "main", "main", -1, null, 0);
            }

            //формування запиту
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{uri}/Reference_Backup/{DateTime.Now.ToShortDateString()}_backup.db");
            request.Credentials = new NetworkCredential(userName, password);
            // встановлюємо для створення директорії
            request.Method = WebRequestMethods.Ftp.UploadFile;

            //якщо повернуло пустоту
            if (request == null)
            {
                return;
            }

            //закриваємо з'єднання
            Program.conn.Close();

            // создаем поток для загрузки файла
            FileStream fs = new FileStream("backupDb.db", FileMode.Open);
            byte[] fileContents = new byte[fs.Length];
            fs.Read(fileContents, 0, fileContents.Length);
            fs.Close();
            request.ContentLength = fileContents.Length;

            // пишем считанный в массив байтов файл в выходной поток
            try
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                // получаем ответ от сервера в виде объекта FtpWebResponse
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                MessageBox.Show("Файл завантажено на сервер!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Файл з цією копією вже існує!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            File.Delete("backupDb.db");       
        }

        //завантаження з серверу
        public void DownloadFile()
        {
            //отримання даних про сервер
            bool getDataFtp = GetLoginPassword(ref uri, ref userName, ref password);

            if (getDataFtp == false)
            {
                return;
            }

            //перевірка з'єднання з сервером
            if(CheckDirForUpload() == false)
            {
                return;
            }

            //знаходження файлів в потрібній директорії
            var request = createRequest($"ftp://{uri}/Reference_Backup/", WebRequestMethods.Ftp.ListDirectoryDetails);
            FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
            
            var sr = new StreamReader(ftpResponse.GetResponseStream());

            //отримання списку всіх файлів директорії
            string line = sr.ReadLine();
            //список знайдених файлів БД
            Dictionary<string, DateTime> files = new Dictionary<string, DateTime>();

            while (!string.IsNullOrEmpty(line))
            {
                //якщо файл відповідає виразу
                if (line.Substring(line.LastIndexOf('.'), 3) == ".db")
                {
                    //знаходимо дату резервного копіювання
                    DateTime dateFile;
                    try
                    {
                        dateFile = Convert.ToDateTime(line.Substring(line.LastIndexOf('_') - 10, 10) + " " + line.Substring(line.LastIndexOf(" ") - 5, 5));
                        files.Add(line.Substring(line.LastIndexOf(" ") + 1), dateFile);
                    }
                    catch { }
                }
                line = sr.ReadLine();
            }

            if (files.Count == 0)
            {
                MessageBox.Show("На сервері не знайдено жодного файлу резервного копіювання!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //знаходимо максимально новий файл
            DateTime maxDate = files.Values.Max();
            //отримуємо назву файлу
            string path = files.FirstOrDefault(x => x.Value == maxDate).Key;

            ftpResponse.Close();
            sr.Close();

            //отримуємо шлях збереження та назву файлу
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DataBase | *.db";
            saveFileDialog.Title = "Збереження копії бази данних";
            saveFileDialog.FileName = "Reference.db";
            string filename = string.Empty;
            DialogResult dialogResul = saveFileDialog.ShowDialog();

            //початок завантаження
            if (dialogResul == DialogResult.OK)
            {
                filename = saveFileDialog.FileName;
                //створюємо запит
                request = createRequest($"ftp://{uri}/Reference_Backup/{path}", WebRequestMethods.Ftp.DownloadFile);
                byte[] buffer = new byte[1024];
                var response = (FtpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                //створення файлу
                var fs = new FileStream(filename, FileMode.OpenOrCreate);
                int readCount = stream.Read(buffer, 0, 1024);
                //запис в файл
                while (readCount > 0)
                {
                    fs.Write(buffer, 0, readCount);
                    readCount = stream.Read(buffer, 0, 1024);
                }
                //повідомлення про успіх
                MessageBox.Show("Файл завантажено з серверу!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //закриття  з'єднання
                response.Close();
                stream.Close();
                fs.Close();
            }

            //відхилення завантаження
            if (dialogResul == DialogResult.Cancel)
            {
                return;
            }
        }

        //отримання статусу виконання
        private string getStatusDescription(FtpWebRequest request)
        {
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                return response.StatusDescription;
            }
        }

        //отримання даних про фтп сервер
        private bool GetLoginPassword(ref string uri, ref string login, ref string pass)
        {
            //запит на отримання з БД
            command = new SQLiteCommand("SELECT ip_adress, login, password FROM ftp_server", Program.conn);
            dataReader = command.ExecuteReader();//виконання запиту

            if(dataReader != null)
            {
                while (dataReader.Read())
                {
                    //отримання даних
                    uri = dataReader[0].ToString();
                    login = dataReader[1].ToString();
                    pass = dataReader[2].ToString();
                }
            }
            else
            {
                dataReader.Close(); //закриття об'єкту БД
                //повідомлення про помилку
                MessageBox.Show("Помилка. Додайте дані FTP-сервера!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            dataReader.Close();//закриття об'єкту БД
            return true;
        }

        //створення директорії для файлів
        public bool CheckDirForUpload()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{uri}/Reference_Backup/");
            request.Credentials = new NetworkCredential(userName, password);
            // встановлюємо для створення директорії
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = null;

            try
            {   //виконуємо запит на сервер
                response = (FtpWebResponse)request.GetResponse();
                MessageBox.Show("З'єднання встановлено.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                //запит на створення директорії
                FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create($"ftp://{uri}/Reference_Backup/");
                request2.Credentials = new NetworkCredential(userName, password);
                request2.Method = WebRequestMethods.Ftp.MakeDirectory;
                try
                {
                    response = (FtpWebResponse)request2.GetResponse();
                    //повідомлення про створення директорії
                    MessageBox.Show("Директорія для копій на сервері успішно створена. З'єднання встановлено.", "Увага!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    //повідомлення про помилку
                    MessageBox.Show("Нажаль сервер недоступний! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            response.Close(); //закриття з'єднання

            return true;
        }
    }
}
