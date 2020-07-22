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
    public partial class FrmSettingDepart : Form
    {
        FrmChangeMenu frmChangeMenu = null; //об'єкт для попереднього вікна
        SQLiteViewInTable inTable = new SQLiteViewInTable(); // об'єкт роботи з SQLite

        //об'єкти для працювання з БД
        SQLiteDataAdapter adapterAll = null;
        SQLiteCommand command = null;
        DataTable dataTableAll = null;
        BindingSource bindingSource = null;
        //конструктор
        public FrmSettingDepart(FrmChangeMenu frmChangeMenu)
        {
            InitializeComponent();

            this.frmChangeMenu = frmChangeMenu;//отримання посилання
            //події корректоного вводу
            textBox1.KeyPress += KeyPressTextBoxUA;
            textBox2.KeyPress += KeyPressTextBoxEN;
            textBox3.KeyPress += Number_KeyPress;
            textBox4.KeyPress += KeyPressTextBoxUA;
            //корегування ширини стовпців
            dataGridView1.AutoResizeColumns();
        }

        //заборона вводу букв
        private void Key(object sender, KeyPressEventArgs e)
        {
            //дозовіл на числа
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar) return; 
            e.Handled = true;//блокування
            return;
        }

        //заборона вводу букв
        private void Number_KeyPress(object sender, KeyPressEventArgs e)
        {
            //дозвіл чисел, зтирання та спец символів
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar || e.KeyChar == '+' || e.KeyChar == '(' || e.KeyChar == ')') return;
            e.Handled = true; //блокування
            return;
        }

        //ввод в поля українською мовою
        private void KeyPressTextBoxUA(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'А' || e.KeyChar > 'я') && e.KeyChar != (char)Keys.Space && e.KeyChar != '\b' && e.KeyChar != 'і' && e.KeyChar != 'І'
                && e.KeyChar != 'ї' && e.KeyChar != 'Ї' && e.KeyChar != 'є' && e.KeyChar != 'Є' && e.KeyChar != Convert.ToChar("'") && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
        }

        //ввод в поля англійською мовою
        private void KeyPressTextBoxEN(object sender, KeyPressEventArgs e)
        {
            //дозвіл вводу англійського мовою
            if ((e.KeyChar < 'A' || e.KeyChar > 'z') && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
        }
        //закриття поточного вікна
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття
        }
        //подія закриття вікна
        private void FrmSettingDepart_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmChangeMenu.Visible = true; //відображення попереднього вікна
        }
        //подія збереження даних
        private void btnSave_Click(object sender, EventArgs e)
        {
            //перевірка, що всі поля заповнені
            if (CheckField() != false)
            {
                if (btnAddEnabled) //якщо обране додавання нового рядка
                {
                    //команда додавання в таблицю бази даних
                    command = new SQLiteCommand("INSERT INTO Department VALUES (@Id, NULL, @NameUA, @NameEN, @ReceptNum, @Fio)", Program.conn);
                    //додавання параметрів
                    command.Parameters.Add(new SQLiteParameter("@Id", 1));
                    command.Parameters.Add(new SQLiteParameter("@NameUA", textBox1.Text));
                    command.Parameters.Add(new SQLiteParameter("@NameEN", textBox2.Text));
                    command.Parameters.Add(new SQLiteParameter("@ReceptNum", textBox3.Text));
                    command.Parameters.Add(new SQLiteParameter("@Fio", textBox4.Text));
                    //виконання запиту
                    SQLiteViewInTable.AddDataInBD(command);
                    //оновлення виводу таблиці
                    FrmSettingDepart_Load(sender, e);
                    //активація та деактивація кнопок
                    btnAddEnabled = false;
                    btnAdd.Enabled = true;
                    //повідомлення про успішну операцію
                    MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (btnUpdate) //якщо обране редагування поточного рядка
                {
                    //додавання в таблицю бази даних
                    command = new SQLiteCommand("UPDATE Department SET NameUA = @NameUA, NameEN = @NameEN, ReceptNumb = @ReceptNumb, Fio = @Fio WHERE idDepartment = @id", Program.conn);
                    //додавання параметрів
                    command.Parameters.Add(new SQLiteParameter("@Id", dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[1].Value));
                    command.Parameters.Add(new SQLiteParameter("@NameUA", textBox1.Text));
                    command.Parameters.Add(new SQLiteParameter("@NameEN", textBox2.Text));
                    command.Parameters.Add(new SQLiteParameter("@ReceptNumb", textBox3.Text));
                    command.Parameters.Add(new SQLiteParameter("@Fio", textBox4.Text));
                    //виконання запиту
                    SQLiteViewInTable.UpdateDateInBD(command);
                    //оновлення виводу таблиці
                    FrmSettingDepart_Load(sender, e);
                    //активація та деактивація кнопок
                    btnUpdate = false;
                    btnChange.Enabled = true;
                    //повідомлення про успішну операцію
                    MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                TextBoxDeActive();//декативація полів вводу
                //активація та деактивація кнопок
                btnSave.Enabled = false;
                btnCancel.Enabled = false;
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
                btnChange.Enabled = true;
            }
        }
        //перевірка полів
        private bool CheckField()
        {
            //перевірка назви відділення (укр)
            if(textBox1.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть українську назву!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            ////перевірка назви відділення (англ)
            if (textBox2.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть англійську назву!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox2.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            //перевірка нмоеру телефону
            if (textBox3.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть номер телефону відділення!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox3.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            //перевірка вводу ПІБ завідувача відділення
            if (textBox4.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть ПІБ завідувача відділення!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox4.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }

            return true;
        }

        private void FrmSettingDepart_Load(object sender, EventArgs e)
        {
            //формування запиту
            command = new SQLiteCommand("SELECT * FROM Department", Program.conn);

            //для виводу в таблицю
            SQLiteViewInTable.OutInTable(command, ref adapterAll, ref dataTableAll, 
                                         ref bindingSource, ref dataGridView1, ref bindingNavigator1);

            //якщо таблиця пуста то блокуємо деякі кнопки
            if (dataGridView1.Rows.Count == 0)
            {
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }

            //підписи стовпців
            dataGridView1.Columns[0].HeaderText = "Код коледжу";
            dataGridView1.Columns[1].HeaderText = "Код відділення";
            dataGridView1.Columns[2].HeaderText = "Назва українська";
            dataGridView1.Columns[3].HeaderText = "Назва англійська";
            dataGridView1.Columns[4].HeaderText = "Номер телефону";
            dataGridView1.Columns[5].HeaderText = "ПІБ завідувача";

            //скриття непотрібних для відображення стовпців
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;

            //підв'язування полів до даних з БД
            textBox1.DataBindings.Add("Text", bindingSource, "NameUA");
            textBox2.DataBindings.Add("Text", bindingSource, "NameEN");
            textBox3.DataBindings.Add("Text", bindingSource, "ReceptNumb");
            textBox4.DataBindings.Add("Text", bindingSource, "Fio");

            //активація потрібних кнопок
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            btnSave.Enabled = false;

            //вставновлення підсказки на заголовках стовпців
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.ToolTipText = "Натисніть для сортування по стовпцю";
            }

            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування потрібних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSettingDepart.htm"))
            {
                //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSettingDepart.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //видалення рядку
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ви точно хочете видалити запис?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                //відв'язування полів від джерела даних
                textBox1.DataBindings.RemoveAt(0);
                textBox2.DataBindings.RemoveAt(0);
                textBox3.DataBindings.RemoveAt(0);
                textBox4.DataBindings.RemoveAt(0);
                //видалення з таблиці
                int index = dataGridView1.SelectedCells[0].RowIndex;
                //видалення з таблиці бази даних
                command = new SQLiteCommand("DELETE FROM Department WHERE idDepartment = @id", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@id", dataGridView1.Rows[index].Cells[1].Value.ToString()));
                //виконання запиту
                SQLiteViewInTable.DeleteDataInBD(command);
                //оновлення виводу таблиці
                FrmSettingDepart_Load(sender, e);
                //повідомлення про успішне видалення
                MessageBox.Show("Дані видалено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool btnUpdate = false; //прапорець для редагування 
        //кнопка редагувати
        private void btnChange_Click(object sender, EventArgs e)
        {
            TextBoxActive(); //активація полів для вводу
            //активація та деактивація необхідних кнопок
            btnUpdate = true;
            btnCancel.Visible = true;
            btnChange.Enabled = false;

            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
        }

        //додавання даних
        bool btnAddEnabled = false;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //очищення полів
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            //активація та деактивація необхідних кнопок
            btnChange.Enabled = false;
            btnDelete.Enabled = false;
            btnAddEnabled = true;
            btnCancel.Enabled = true;
            btnAdd.Enabled = false;
            TextBoxActive();//активація полів для вводу
        }

        //Активація полів та кнопок
        private void TextBoxActive()
        {
            //активація можливості вводу для полів
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            //активація кнопок зберегти та відмінити
            btnSave.Enabled = true;
            btnCancel.Enabled = true;

            //відвязка від бази даних
            textBox1.DataBindings.RemoveAt(0);
            textBox2.DataBindings.RemoveAt(0);
            textBox3.DataBindings.RemoveAt(0);
            textBox4.DataBindings.RemoveAt(0);
        }

        //деактивація полів
        private void TextBoxDeActive()
        {
            //блокування вводу для полів
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            //блокування кнопок зберегти та відмінити
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        //відміна дії
        private void btnCancel_Click(object sender, EventArgs e)
        {   //блокування кнопок зберегти та відмінити
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            TextBoxDeActive(); //деактивація полів
            FrmSettingDepart_Load(sender, e); //оновлення даних в таблиці
            //активація та деактивація потрібних кнопок
            btnUpdate = false;
            btnAddEnabled = false;
            btnAdd.Enabled = true;
            //якщо таблиця пуста
            if (dataGridView1.Rows.Count == 0)
            {
                //блокування кнопок редагування та видалення
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                //активація кнопок редагування та видалення
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }
        }
    }
}
