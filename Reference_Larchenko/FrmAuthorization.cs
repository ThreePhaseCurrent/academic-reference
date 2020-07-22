using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reference_Larchenko
{
    public partial class FrmAuthorization : Form
    {
        //об'єкт для працювання з БД
        SQLiteCommand command = null;
        SQLiteDataReader dataReader = null;
        
        string fio = string.Empty;//ПІБ користувача
        string login = string.Empty;//логін користувача
        string password = string.Empty;//пароль 
        string repeatPassword = string.Empty;//повторення пароля

        public Account account = null;//об'єкт аккаунта

        public static bool correctWord = true;
        //конструктор форми
        public FrmAuthorization()
        {
            InitializeComponent();

            //шукаємо шлях встановленного додатку Word
            string path = GetWordPath();
            //одержання версії додатку
            int version = GetMajorVersion(path);
            //якщо версія старіша ніж 2010
            if (version < 14)
            {
                correctWord = false;
                MessageBox.Show("Для корректної роботи програми встановіть додаток Word 2010 або більш нову версію.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                DialogResult dialogResult = MessageBox.Show("Бажаєте встановити додаток Word 2010?", "Увага!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(dialogResult == DialogResult.Yes) // якщо встановлюємо додаток
                {
                    //перевіряємо чи є файл встановлення
                    if (File.Exists(@"Additional programs\Word\Word_2010.exe"))
                    {
                        try
                        {
                            //початок встановлення ворда
                            Process.Start(@"Additional programs\Word\Word_2010.exe");
                            correctWord = false;
                        }
                        catch
                        {
                            //помилка встановлення
                            MessageBox.Show("Не вдалося розпочати установку!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            correctWord = false;
                            Application.Exit();
                        }
                    }
                    else
                    {
                        correctWord = false;
                        //файл не знайдено
                        MessageBox.Show("Виконуваний файл не знайдено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
                else if(dialogResult == DialogResult.No)
                {
                    correctWord = false;
                    Application.Exit();
                }
            }
            //події для корректного вводу символів в поля
            tbLog.KeyPress += TextBoxLogPass_KeyPress;
            tbPass.KeyPress += TextBoxLogPass_KeyPress;
            //подія виклику довідки
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик довідки
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/Outro.htm"))
            {
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "Outro.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //таймер появлення панелі реєстрації
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (panel1.Height < 360)
            {
                panel1.Height += 20;//плавно вспливаюче вікно
            }
            else
            {
                timer1.Stop();//зупинка таймеру
            }
        }

        //початок реєстрації
        private void btnToRegister_Click(object sender, EventArgs e)
        {
            timer1.Start();//запуск таймера вспливаючого вікна
            //очистка полів логіна та паролю
            tbLog.Clear();
            tbPass.Clear();
        }

        //таймер звертування панелі реєстрації
        private void timer_Back_Tick(object sender, EventArgs e)
        {
            if (panel1.Height > 11)
            {
                panel1.Height -= 20;//зворачуємо панель
            }
            else
            {
                timer_Back.Stop();//зупинка звертування
            }
        }

        //повернення до авторизації
        private void btnBack_Click(object sender, EventArgs e)
        {
            timer_Back.Start(); // початок звертання вікна
            ClearTextBoxRegister(); //очищення полів створення аккаунту
        }

        //вхід у систему
        private void btnLogin_Click(object sender, EventArgs e)
        {
            account = new Account(); // створюємо об'єкт аккаунту
            //перевірка заповнення полів
            if(tbLog.Text == "")
            {
                MessageBox.Show("Введіть логін!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbLog.Focus();
                return;
            }

            if (tbPass.Text == "")
            {
                MessageBox.Show("Введіть пароль!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbPass.Focus();
                return;
            }

            //запит на пошук у бд
            command = new SQLiteCommand("SELECT login, password, fio, rang FROM User WHERE login = @log AND password = @pass", Program.conn);
            //додавання параметрів
            command.Parameters.Add(new SQLiteParameter("@log", tbLog.Text));
            command.Parameters.Add(new SQLiteParameter("@pass", tbPass.Text));

            dataReader = command.ExecuteReader(); //виконання запиту

            //якщо нічого не знайдено
            if(dataReader.Read() == false)
            {
                labelErrorAuth.Visible = true;
                return;
            }
            else // якщо знайшли
            {
                //зчитування даних про аккаунт
                account.fio = dataReader[2].ToString();
                account.rang = dataReader[3].ToString();
                account.login = dataReader[0].ToString();
                this.Visible = false;
                //закриття об'єктів роботи з БД
                dataReader.Close();
                command.Dispose();
                //перехід на головну форму
                FrmMain frmStart = new FrmMain(this);
                frmStart.ShowDialog();
                //очищення полів
                tbLog.Text = string.Empty;
                tbPass.Text = string.Empty;
                labelErrorAuth.Visible = false;
            }
        }

        //створення нового аккаунту
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            //перевірка заповнення полів
            if (tbFio.Text == "")
            {
                MessageBox.Show("Введіть ваше ПІБ!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbFio.Focus();
                return;
            }

            if (tbCreateLog.Text == "")
            {
                MessageBox.Show("Введіть логін!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbCreateLog.Focus();
                return;
            }

            if (tbCreatePass.Text == "")
            {
                MessageBox.Show("Введіть пароль!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbCreatePass.Focus();
                return;
            }

            if (tbRepeatPass.Text == "")
            {
                MessageBox.Show("Повторіть пароль!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbRepeatPass.Focus();
                return;
            }

            //зчитування введених даних
            fio = tbFio.Text;
            login = tbCreateLog.Text;
            password = tbCreatePass.Text;
            repeatPassword = tbRepeatPass.Text;

            //перевірка унікальності логіну
            command = new SQLiteCommand("SELECT login FROM User WHERE login = @log", Program.conn);
            command.Parameters.Add(new SQLiteParameter("@log", login));
            dataReader = command.ExecuteReader();//виконання команди

            //якщо знайшли такий логін
            if(dataReader.Read() == true)
            {
                MessageBox.Show("Такий логін вже існує!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                tbCreateLog.Focus();
                labelErrorCreateAccount.Text = "Такий логін вже існує";
                labelErrorCreateAccount.Visible = true;
                return;
            }

            //якщо паролі не співпали
            if (password != repeatPassword)
            {
                MessageBox.Show("Паролі не співпадають!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelErrorCreateAccount.Text = "Паролі не співпадають";
                labelErrorCreateAccount.Visible = true;
                return;
            }

            //додавання в бд
            command = new SQLiteCommand("INSERT INTO User VALUES(NULL, @fio, @log, @pass, @rang)", Program.conn);
            //додавання параметрів
            command.Parameters.Add(new SQLiteParameter("@fio", fio));
            command.Parameters.Add(new SQLiteParameter("@log", login));
            command.Parameters.Add(new SQLiteParameter("@pass", password));
            command.Parameters.Add(new SQLiteParameter("@rang", "-"));
            //виконання запиту
            SQLiteViewInTable.AddDataInBD(command);

            MessageBox.Show("Ваш аккаунт створено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //перехід в панель авторизації
            timer_Back.Start();
            //очищення полів створення аккаунту
            ClearTextBoxRegister();
        }

        //очищення полів створення аккаунту
        private void ClearTextBoxRegister()
        {
            tbFio.Clear();
            tbCreateLog.Clear();
            tbCreatePass.Clear();
            tbRepeatPass.Clear();
        }

        //закриття форми
        private void FrmAuthorization_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (error == false) //якщо вихід не в наслідок помилки
            {
                if (correctWord != false)
                {
                    //підтвердження виходу
                    DialogResult dialogResult = MessageBox.Show("Вийти з програми?", "Увага!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.No)
                    {
                        e.Cancel = true; // скасування виходу
                    }
                }
            }
        }

        //корректний ввод логіну та паролю
        private void TextBoxLogPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z') 
                || (Char.IsDigit(e.KeyChar)) || (e.KeyChar == (char)Keys.Back))
            {
                return;
            }
            e.Handled = true;
        }

        //корректний ввід ПІБ
        private void KeyPressTextBoxUA(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'А' || e.KeyChar > 'я') && e.KeyChar != (char)Keys.Space && e.KeyChar != '\b' && e.KeyChar != 'і' && e.KeyChar != 'І'
                && e.KeyChar != 'ї' && e.KeyChar != 'Ї' && e.KeyChar != 'є' && e.KeyChar != 'Є' && e.KeyChar != Convert.ToChar("'") && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
        }

        //підключення бд
        private void FrmAuthorization_Load(object sender, EventArgs e)
        {
            //показ стартового вікна
            FrmStart frmStart = new FrmStart();
            frmStart.ShowDialog();

            //підключення БД
            DBConnection();
        }

        static bool error = false; //чи була помилка

        //підключення бд
        public static void DBConnection()
        {
            if(!File.Exists("Reference.db")) //якщо файл БД не знайдено
            {
                //повідомлення
                MessageBox.Show("Файл бази данних не знайдено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                error = true; //встановлення шо знайдено помилку
                Application.Exit(); //вихід з програми
                return; //вихід з функції
            }

            Program.conn = new SQLiteConnection("Data Source=Reference.db; Version=3");

            try
            {
                Program.conn.Open(); // відкриття з'єднання
            }
            catch { MessageBox.Show("Зв'язок з базою данних не встановленно!"); }
        }

        //отримання шляху додатку Word
        private string GetWordPath()
        {
            //шлях в реєстрі
            const string RegKey = @"Software\Microsoft\Windows\CurrentVersion\App Paths";
            string toReturn = string.Empty;

            //шкумаємо у CURRENT_USER:
            RegistryKey _mainKey = Registry.CurrentUser;
            try
            {
                //відкриваємо за шляхом
                _mainKey = _mainKey.OpenSubKey(RegKey + "\\winword.exe", false);
                if (_mainKey != null)//якщо ключ відсутній
                {
                    toReturn = _mainKey.GetValue(string.Empty).ToString();
                }
            }
            catch
            { }

            //якщо не знайшли, то шукаємо у LOCAL_MACHINE:
            _mainKey = Registry.LocalMachine;
            if (string.IsNullOrEmpty(toReturn))
            {
                try
                {
                    //відкриваємо за шляхом
                    _mainKey = _mainKey.OpenSubKey(RegKey + "\\winword.exe", false);
                    if (_mainKey != null) //якщо ключ відсутній
                    {
                        toReturn = _mainKey.GetValue(string.Empty).ToString();
                    }
                }
                catch
                { }
            }

            if (_mainKey != null)//закриття об'єкту
                _mainKey.Close();

            return toReturn;//повернення інформації
        }

        //отримання версії
        private int GetMajorVersion(string _path)
        {
            int toReturn = 0;
            //шукаємо за шляхом
            if (File.Exists(_path))
            {
                try
                {
                    //отримуємо повну версію
                    FileVersionInfo _fileVersion = FileVersionInfo.GetVersionInfo(_path);
                    toReturn = _fileVersion.FileMajorPart;//отримуємо частину номера версії
                }
                catch
                { }
            }
            return toReturn;//повертаємо значення
        }

        private void FrmAuthorization_Shown(object sender, EventArgs e)
        {
            if (correctWord == false)
            {
                Application.Exit();
            }
        }
    }
}
