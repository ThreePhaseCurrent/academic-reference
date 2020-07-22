using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace Reference_Larchenko
{
    public partial class FrmOpenReference : Form
    {
        //об'єкт для попередньої форми
        public FrmMain frmStart = null;
        Account account; //поточний аккаунт
        //для роботи з базою даних
        SQLiteCommand command = null;
        BindingSource bindingSource = null;
        SQLiteDataAdapter dataAdapter = null;
        DataTable dataTable = null;
        //конструктор
        public FrmOpenReference(FrmMain frmStart, Account account)
        {
            InitializeComponent();
            //отримання посилань
            this.frmStart = frmStart;
            this.account = account;

            //підпис рангу в шапці форми
            if (account.rang == "+")
            {
                this.Text += " (Адміністратор)";
            }
            else
            {
                this.Text += " (Користувач)";
            }

        }

        //виведення даних в таблицю
        private void FrmOpenReference_Load(object sender, EventArgs e)
        {
            //створення запиту для користувача
            if (account.rang != "+")
            {
                command = new SQLiteCommand("SELECT SavedData.idSave, SavedData.SurnameUA, SavedData.NameUA, SavedData.PatronymicUA, Specialty.NameUA, " +
                                            "specialization.name_ua_spec, SavedData.DateRecord FROM (SavedData INNER JOIN Specialty ON SavedData.NumSpeciality = Specialty.idSpecialty) " +
                                            "INNER JOIN specialization ON SavedData.Specialization = specialization.id_specialization WHERE LoginCreated = @log", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@log", account.login));
            }//створення запиут для адміністратора
            else
            {
                command = new SQLiteCommand("SELECT SavedData.idSave, SavedData.SurnameUA, SavedData.NameUA, SavedData.PatronymicUA, Specialty.NameUA AS SPEC, " +
                                            "specialization.name_ua_spec AS SPECIAL, SavedData.DateRecord FROM (SavedData INNER JOIN Specialty ON SavedData.NumSpeciality = Specialty.idSpecialty) " +
                                            "INNER JOIN specialization ON SavedData.Specialization = specialization.id_specialization", Program.conn);
            }
            //виконання запиту
            SQLiteViewInTable.OutInTable(command, ref dataAdapter, ref dataTable, ref bindingSource, ref dataGridView1, ref bindingNavigator1);

            //підпис стовпців таблиці
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Прізвище";
            dataGridView1.Columns[2].HeaderText = "Ім'я";
            dataGridView1.Columns[3].HeaderText = "По батькові";
            dataGridView1.Columns[4].HeaderText = "Спеціальність";
            dataGridView1.Columns[5].HeaderText = "Спеціалізація";
            dataGridView1.Columns[6].HeaderText = "Дата збереження";
            //очищення полів
            tbSurname.DataBindings.Clear();
            tbName.DataBindings.Clear();
            tbPatronimyc.DataBindings.Clear();
            tbSpeciality.DataBindings.Clear();
            tbSpecialization.DataBindings.Clear();
            tbDateRecord.DataBindings.Clear();
            //підвязка текстових полів до стовпців таблиці
            tbSurname.DataBindings.Add("Text", bindingSource, "SurnameUA");
            tbName.DataBindings.Add("Text", bindingSource, "NameUA");
            tbPatronimyc.DataBindings.Add("Text", bindingSource, "PatronymicUA");
            tbSpeciality.DataBindings.Add("Text", bindingSource, "SPEC");
            tbSpecialization.DataBindings.Add("Text", bindingSource, "SPECIAL");
            tbDateRecord.DataBindings.Add("Text", bindingSource, "DateRecord");

            //вставновлення підсказки на заголовках стовпців
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.ToolTipText = "Натисніть для сортування по стовпцю";
            }

            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;

            //блокування кнопки видалення
            if(dataGridView1.Rows.Count == 0)
            {
                btnDelete.Enabled = false;
            }
            else
            {
                btnDelete.Enabled = true;
            }
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormOpenReference.htm"))
            {
                //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormOpenReference.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //відкриття обраної довідки
        private void btnRefOpen_Click(object sender, EventArgs e)
        {
            try
            {
                //отримання номеру збереженної довідки
                int selectId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value);
                this.Visible = false; //зкриття поточного вікна
                //відкриття вікна введення даних
                FrmInputData createReference = new FrmInputData(this, selectId, account);
                createReference.ShowDialog();
                this.Close();//закриття вікна
            }
            catch
            {
                //повідомлення про помилку
                MessageBox.Show("Помилка! Немає збережених довідок.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //відкриття домашньої папки
        private void btnOpenFileExpl_Click(object sender, EventArgs e)
        {
            //перевірка існування каталогу
            if (!File.Exists(@"Created\" + account.login))
            {
                //створення нової директорії
                Directory.CreateDirectory(@"Created\" + account.login);
                //перехід у створену директорію
                Process.Start(@"Created\" + account.login);
            }
            else
            {
                //перехід в директорію
                Process.Start(@"Created\" + account.login);
            }
        }

        //видалення обраної збереженої довідки
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ви точно хочете видалити запис?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (dialogResult == DialogResult.Yes)
            {
                //запит в базу даних
                command = new SQLiteCommand("DELETE FROM SavedData WHERE idSave = @id", Program.conn);
                //індекс обраної строки
                int rowNum = dataGridView1.SelectedCells[0].RowIndex;
                command.Parameters.Add(new SQLiteParameter("@id", dataGridView1.Rows[rowNum].Cells[0].Value));
                SQLiteViewInTable.DeleteDataInBD(command);
                //повідомлення про успішність операції
                MessageBox.Show("Довідка видалена!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //відвязка текстових полів до стовпців таблиці
                tbSurname.DataBindings.RemoveAt(0);
                tbName.DataBindings.RemoveAt(0);
                tbPatronimyc.DataBindings.RemoveAt(0);
                tbSpeciality.DataBindings.RemoveAt(0);
                tbSpecialization.DataBindings.RemoveAt(0);
                tbDateRecord.DataBindings.RemoveAt(0);
                //очищення полів
                tbSurname.Clear();
                tbName.Clear();
                tbPatronimyc.Clear();
                tbSpeciality.Clear();
                tbSpecialization.Clear();
                tbDateRecord.Clear();
                //оновлення даних у таблиці
                this.FrmOpenReference_Load(sender, e);
            }
        }

        //закриття вікна
        private void FrmOpenReference_FormClosing(object sender, FormClosingEventArgs e)
        {
            //відображення попереднього вікна
            frmStart.Visible = true;
            //знищення об'єктів для роботи з БД
            command = null;
            bindingSource = null;
            dataAdapter = null;
            dataTable.Clear();
        }
        //повернення до попереднього вікна
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття вікна
            frmStart.Visible = true; //відображення попереднього вікна
        }

        //кнопка видалити для навігатора
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }
    }
}
