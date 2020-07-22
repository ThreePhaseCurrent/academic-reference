using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace Reference_Larchenko
{
    public partial class FrmSettingFtp : Form
    {
        FrmChangeMenu frmChangeMenu = null;//об'єкт для попереднього вікна
        //об'єкти для роботи з БД
        SQLiteCommand command = null;
        SQLiteDataReader dataReader = null;

        string newIpAddress = string.Empty; //новий адресс
        //логін та пароль
        string login = string.Empty;
        string password = string.Empty;
        //конструктор
        public FrmSettingFtp(FrmChangeMenu frmChangeMenu)
        {
            InitializeComponent();

            this.frmChangeMenu = frmChangeMenu; //отримання посилань
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSettingFtp.htm"))
            {   //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSettingFtp.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //налаштування перед відображенням вікна
        private void FrmSettingFtp_Load(object sender, EventArgs e)
        {
            //запит на збережені дані про сервер
            command = new SQLiteCommand("SELECT * FROM ftp_server", Program.conn);
            dataReader = command.ExecuteReader();

            //якщо є дані
            if (dataReader.Read() == true)
            {
                //відображення існуючого адресу
                tbIpAddress.Text = dataReader[1].ToString();

                //якщо логін і пароль є
                if (dataReader[2].ToString() != string.Empty)
                {
                    tbLogin.Text = dataReader[2].ToString();
                    tbPassword.Text = dataReader[3].ToString();
                }
                else
                {
                    cbNoAuth.Checked = true;
                }
                //блокування та активація кнопок
                btnSaveFtp.Enabled = false;
                btnUpdate.Enabled = true;
                btnCheckConn.Enabled = true;
                //блокування полів вводу
                DeactivateTextBox();
                //кнопки роботи з сервером
                btnCreateBackup.Enabled = true;
                btnUploadBackup.Enabled = true;
            }
            else //якщо даних немає
            {
                //активація полів вводу
                ActivateTextBox();
                //блокування та активація кнопок
                btnSaveFtp.Enabled = true;
                btnUpdate.Enabled = false;
                btnCheckConn.Enabled = false;
                //кнопки роботи з сервером
                btnCreateBackup.Enabled = false;
                btnUploadBackup.Enabled = false;
            }

            dataReader.Close();//закриття об'єкту БД
        }

        //декативація полів вводу
        private void DeactivateTextBox()
        {
            tbIpAddress.Enabled = false;
            tbLogin.Enabled = false;
            tbPassword.Enabled = false;
            cbNoAuth.Enabled = false;
        }
        //активація полів вводу
        private void ActivateTextBox()
        {
            tbIpAddress.Enabled = true;
            tbLogin.Enabled = true;
            tbPassword.Enabled = true;
            cbNoAuth.Enabled = true;
        }

        //зберегти новий адрес
        private void btnSaveFtp_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress; //об'єкт адресси

            //перевіряємо вірність введенного ip-адресу
            if (IPAddress.TryParse(tbIpAddress.Text, out ipAddress))
            {
                newIpAddress = tbIpAddress.Text.ToString();

                //якщо логін і пароль потрібні
                if (cbNoAuth.Checked == false) {

                    //перевіряємо чи введені дані
                    if (tbLogin.Text == string.Empty)
                    {
                        MessageBox.Show("Введіть логін!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tbLogin.Focus();
                        return;
                    }

                    if (tbPassword.Text == string.Empty)
                    {
                        MessageBox.Show("Введіть пароль!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tbPassword.Focus();
                        return;
                    }

                    //зчитування даних
                    login = tbLogin.Text;
                    password = tbPassword.Text;
                }

                //перевіряємо чи були введенні до цього дані сервера
                command = new SQLiteCommand("SELECT * FROM ftp_server", Program.conn);
                dataReader = command.ExecuteReader();

                //якщо знайдено
                if (dataReader.Read() == true)
                {
                    //змінюємо адрес серверу в бд
                    command = new SQLiteCommand("UPDATE ftp_server SET ip_adress = @ip, login = @login, password = @pass", Program.conn);
                }
                else
                {
                    //додаємо адрес серверу в БД
                    command = new SQLiteCommand("INSERT INTO ftp_server VALUES (NULL, @ip, @login, @pass)", Program.conn);
                }
                dataReader.Close(); //закриття об'єкту БД

                //додаємо параметри
                command.Parameters.Add(new SQLiteParameter("@ip", newIpAddress));
                command.Parameters.Add(new SQLiteParameter("@login", login));
                command.Parameters.Add(new SQLiteParameter("@pass", password));
                //виконуємо запит в бд
                try
                {
                    dataReader = command.ExecuteReader(); //виконання запиту
                    dataReader.Close();//закриття об'єкту БД
                    MessageBox.Show("Дані сервера успішно збережені.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //кнопки роботи з сервером
                    btnUploadBackup.Enabled = true;
                    btnCreateBackup.Enabled = true;
                    //кнопки редагування та перевірки
                    btnSaveFtp.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnCheckConn.Enabled = true;
                    //деактивація полів вводу
                    DeactivateTextBox();

                    FrmSettingFtp_Load(sender, e);//оновлення даних
                }
                catch
                {
                    //повідомлення про помилку
                    MessageBox.Show("Помилка додавання даних.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
            else
            {
                //повідомлення про невірний адрес
                MessageBox.Show("IP-адрес має невірний формат", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }
        //кнопка повернутися
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття вікна
        }
        //подія закриття вікна
        private void FrmSettingFtp_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmChangeMenu.Visible = true; //віображення попереднього вікна
        }

        //редагування даних про сервер
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ActivateTextBox(); //активація полів вводу
            //активація кнопок
            btnUpdate.Enabled = false;
            btnCheckConn.Enabled = false;
            btnSaveFtp.Enabled = true;
        }
        
        //локальне збереження копії бази данних
        private void btnLocalBackup_Click(object sender, EventArgs e)
        {
            //закриття об'єктів БД
            dataReader.Close();
            command.Dispose();
            //закриття з'єднання з бд
            Program.conn.Close();
            GC.Collect();

            //робимо копію бази даних
            using (var location = new SQLiteConnection(@"Data Source=Reference.db; Version=3;"))
            using (var destination = new SQLiteConnection(@"Data Source=backupDb.db; Version=3;"))
            {
                location.Open();
                destination.Open();
                location.BackupDatabase(destination, "main", "main", -1, null, 0);
            }

            //відкриття файлу БД
            FileStream stream = new FileStream("backupDb.db", FileMode.Open);

            //отримуємо шлях збереження та назву файлу
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DataBase | *.db";
            saveFileDialog.Title = "Збереження копії бази данних";
            saveFileDialog.FileName = DateTime.Now.ToShortDateString() + "_Reference.db";
            string filename = string.Empty;
            DialogResult dialogResul = saveFileDialog.ShowDialog();

            if (dialogResul == DialogResult.OK)
            {
                filename = saveFileDialog.FileName;

                //збереження копії бд
                SaveDb(filename, stream);
                //повідомлення про успішність операції
                MessageBox.Show("Локальна копія бази даних успішна створена!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            File.Delete("backupDb.db"); //видалення копії бази даних
            //оновлення підключення до бд
            FrmAuthorization.DBConnection();
        }

        //збереження копії бд
        private void SaveDb(string filename, FileStream stream)
        {            
            //створення нового файду
            FileStream fileStream = File.Create(filename, (int)stream.Length);
            //запис файлу
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)data.Length);
            fileStream.Write(data, 0, data.Length);
            //закриття потоків
            fileStream.Close();
            stream.Close();
        }

        //встановлення локальної копії бд
        private void btnLocalBackupInstall_Click(object sender, EventArgs e)
        {
            //закриття об'єктів БД
            dataReader.Close();
            command.Dispose();
            //закриття з'єднання з бд
            Program.conn.Close();
            GC.Collect(); //виклик збірника сміття
            //вікно вибору файлу користувачем
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "DataBase | *.db"; //фільтр для файлів
            openFile.Title = "Відкриття копії бази данних"; //напис в шапці вікна
            string filename = string.Empty; //назва файлу
            //результат від користувача
            DialogResult dialogResul = openFile.ShowDialog();

            if (dialogResul == DialogResult.OK) //якщо файл обрано
            {
                filename = openFile.FileName; //отримання шляху
                FileStream stream = null;
                //відкриття файлу БД
                try
                {
                    //відкриття файлу
                    stream = new FileStream(filename, FileMode.Open);
                }
                catch
                {
                    //повідомлення про помилку відкриття файлу
                    MessageBox.Show("Помилка відкриття файлу бази даних!\nСпробуйте інший файл.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //збереження копії бд
                SaveDb("Reference.db", stream);
                //повідомлення про успішність операції
                MessageBox.Show("Обрана база даних успішна встановлена!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //оновлення підключення до бд
            FrmAuthorization.DBConnection();
        }

        //завантаження даних на сервер
        private void btnCreateBackup_Click(object sender, EventArgs e)
        {
            //закриття з'єднання з бд
            dataReader.Close();
            command.Dispose();
            GC.Collect();
            //об'єкт для працювання з сервером
            FtpClient ftpClient = new FtpClient();
            ftpClient.UploadFile(); //завантаження файлів 

            FrmAuthorization.DBConnection(); //оновлення з'єднання з БД
        }

        //завантаження з серверу
        private void btnUploadBackup_Click(object sender, EventArgs e)
        {
            //об'єкт для працювання з сервером
            FtpClient ftpClient = new FtpClient();
            ftpClient.DownloadFile();//завантаження файлів з серверу
        }

        //перевірка з'єднання з сервером
        private void btnCheckConn_Click(object sender, EventArgs e)
        {
            //об'єкт для працювання з сервером
            FtpClient ftp = new FtpClient();
            ftp.CheckDirForUpload(); //перевірка директорії
        }
    }
}
