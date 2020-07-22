using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace Reference_Larchenko
{
    public partial class FrmMain : Form
    {
        FrmAuthorization frmAuthorization = null; //об'єкт для попередньої форми
        public Account account = new Account(); //інформація про поточний аккаунт

        //конструктор форми
        public FrmMain(FrmAuthorization frmAuthorization)
        {
            InitializeComponent();
            //отримання відомостей про аккаунт
            this.account = frmAuthorization.account;
            //отримання ссилання на форму авторизації
            this.frmAuthorization = frmAuthorization;
            //підпис типу облікового запису в шапці вікна
            if(account.rang == "+")
            {
                this.Text += " (Адміністратор)";
            }
            else
            {
                this.Text += " (Користувач)";
            }

            labelAccount.Text = account.fio; //вивід логіну поточного облікового запису
            this.HelpButtonClicked += HelpButton_Click; // подія виклику довідки до програми
        }

        //перехід до створення довідки
        private void CreateRef_Click(object sender, EventArgs e)
        {
            this.Visible = false;//зкриття поточного вікна
            //відображення наступного вікна
            FrmInputData createReference = new FrmInputData(this, account);
            createReference.ShowDialog();
        }

        //визов форми налаштувань
        private void btnSetting_Click(object sender, EventArgs e)
        {
            //перевірка чи має користувач права адміністратора
            if(account.rang != "+")
            {
                MessageBox.Show("Немає прав адміністратора!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            this.Visible = false; //зкриття поточного вікна
            //відображення вікна з меню
            FrmChangeMenu changeMenu = new FrmChangeMenu(this);
            changeMenu.ShowDialog();
        }

        //відкриття збереженої довідки
        private void btnOpenRef_Click(object sender, EventArgs e)
        {
            this.Visible = false;//зкриття поточного вікна
            //відображення вікна з збереженними довідками
            FrmOpenReference frmOpenReference = new FrmOpenReference(this, account);
            frmOpenReference.ShowDialog();
        }   

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка чи існіють необхідні файли
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/Intro.htm"))
            {
                //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "Intro.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //вихід з облікового запису
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();//закриття поточного вікна
        }

        //закриття поточної форми
        private void FrmStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            //отримання відповіді від користувача
            DialogResult dialogResult = MessageBox.Show("Вийти з облікового запису?", "Увага!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //якщо так, то закриваемо форму
            if (dialogResult == DialogResult.Yes)
            {
                //відображення форми авторизації
                this.Visible = false;
                frmAuthorization.Visible = true;
            } else if (dialogResult == DialogResult.No) // якщо ні
            {
                e.Cancel = true; //скасування виходу з облікового запису
            }
        }
    }
}
