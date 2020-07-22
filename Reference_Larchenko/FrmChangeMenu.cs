using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace Reference_Larchenko
{
    public partial class FrmChangeMenu : Form
    {
        FrmMain frmMain = null; //об'єкт головного вікна
        //конструктор
        public FrmChangeMenu(FrmMain frmMain)
        {
            InitializeComponent();

            this.frmMain = frmMain; //отримання посилання
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування необхідних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSettingMenu.htm"))
            {
                //відкриття довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSettingMenu.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //подія закриття вікна
        private void FrmChangeMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmMain.Visible = true; //відображення попереднього вікна
        }
        //налаштування предметів
        private void btnObj_Click(object sender, EventArgs e)
        {
            this.Visible = false; //зкриття поточного вікна
            FrmSettingObj frmSetting = new FrmSettingObj(this);
            frmSetting.ShowDialog();
        }
        //повернутися назад
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.frmMain.Visible = true; //відображення попереднього вікна
            this.Close(); //закриття поточного вікна
        }
        //налаштування відділень
        private void btnDepart_Click(object sender, EventArgs e)
        {
            this.Visible = false; //зкриття поточного вікна
            //відображення вікна відділень
            FrmSettingDepart frmSettingDepart = new FrmSettingDepart(this);
            frmSettingDepart.ShowDialog();
        }
        //налаштування спеціальностей
        private void btnSpec_Click(object sender, EventArgs e)
        {
            this.Visible = false; //зкриття поточного вікна
            //відображення вікна спеціальностей
            FrmSettingSpec frmSettingSpec = new FrmSettingSpec(this);
            frmSettingSpec.ShowDialog();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            this.Visible = false; //зкриття поточного вікна
            //відображення вікна розширених налаштувань
            FrmSettingFtp frmSettingFtp = new FrmSettingFtp(this);
            frmSettingFtp.ShowDialog();
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            this.Visible = false; //зкриття поточного вікна
            //відображення вікна користувачів
            FrmSettingUser frmSettingUser = new FrmSettingUser(this);
            frmSettingUser.ShowDialog();
        }

        private void btnSpecialization_Click(object sender, EventArgs e)
        {
            this.Visible = false; //зкриття поточного вікна
            //відображення вікна спеціалізацій
            FrmSettingSpecialization frmSettingSpecialization = new FrmSettingSpecialization(this);
            frmSettingSpecialization.ShowDialog();
        }
    }
}
