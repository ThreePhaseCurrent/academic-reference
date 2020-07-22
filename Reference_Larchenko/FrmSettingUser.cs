using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reference_Larchenko
{
    public partial class FrmSettingUser : Form
    {
        FrmChangeMenu frmChangeMenu = null;//об'єкт для попереднього вікна
        //об'єкти для роботи з БД
        SQLiteDataAdapter adapterAll = null;
        SQLiteCommand command = null;
        DataTable dataTableAll = null;
        BindingSource bindingSource = null;

        string checkAdmin = string.Empty;

        public FrmSettingUser(FrmChangeMenu frmChangeMenu)
        {
            InitializeComponent();

            this.frmChangeMenu = frmChangeMenu;//отримання посилання
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //вивід даних у таблицю
        private void FrmSettingUser_Load(object sender, EventArgs e)
        {
            //отримання всіх користувачів
            command = new SQLiteCommand("SELECT * FROM User", Program.conn);
            //виконання команди
            SQLiteViewInTable.OutInTable(command, ref adapterAll, ref dataTableAll, 
                                            ref bindingSource, ref dataGridView1, ref bindingNavigator1);

            //якщо таблиця пуста то блокуємо деякі кнопки
            if (dataGridView1.Rows.Count == 0)
            {   //деактивація кнопок оновлення та видалення
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {   //деактивація кнопок оновлення та видалення
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }
            //зкриття перного стовпця
            dataGridView1.Columns[0].Visible = false;
            //підписи стовпців
            dataGridView1.Columns[1].HeaderText = "ПІБ";
            dataGridView1.Columns[2].HeaderText = "Логін";
            dataGridView1.Columns[3].HeaderText = "Пароль";
            dataGridView1.Columns[4].HeaderText = "Права адміністратора";

            //підв'язування полів до даних з БД
            textBox1.DataBindings.Add("Text", bindingSource, "fio");
            textBox2.DataBindings.Add("Text", bindingSource, "login");
            textBox3.DataBindings.Add("Text", bindingSource, "password");

            //перевірка прав адміністратора
            CheckAdminRight();

            //вставновлення підсказки на заголовках стовпців
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.ToolTipText = "Натисніть для сортування по стовпцю";
            }
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSettingUser.htm"))
            {//показ довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSettingUser.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //корректний ввід ПІБ
        private void KeyPressTextBoxUA(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'А' || e.KeyChar > 'я') && e.KeyChar != (char)Keys.Space && e.KeyChar != '\b' && e.KeyChar != 'і' && e.KeyChar != 'І'
                && e.KeyChar != 'ї' && e.KeyChar != 'Ї' && e.KeyChar != 'є' && e.KeyChar != 'Є' && e.KeyChar != Convert.ToChar("'") && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
        }

        //перевірка полів
        private bool CheckField()
        {
            //перевірка ввеедння ПІБ
            if (textBox1.Text == "")
            {   //повідомлення про помилку
                MessageBox.Show("Введіть ПІБ!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Focus();//встановлення фокусу на поле
                return false;
            }
            // перевірка введення логіну
            if (textBox2.Text == "")
            {   //повідомлення про помилку
                MessageBox.Show("Введіть логін!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox2.Focus();//встановлення фокусу на поле
                return false;
            }
            //перевірка введення паролю
            if (textBox3.Text == "")
            {   //повідомлення про помилку
                MessageBox.Show("Введіть пароль!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox3.Focus();//встановлення фокусу на поле
                return false;
            }

            return true;
        }

        //перевірка прав адміністратора
        private void CheckAdminRight()
        {
            try
            {
                //зчитування прав
                checkAdmin = dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[4].Value.ToString();
            }
            catch { }
            if (checkAdmin == "+") //якщо адміністратор
            {
                checkBox1.Checked = true;//прапорець стоїть
            }
            else
            {
                checkBox1.Checked = false;//немає прапорця
            }
        }
        //поідя виділення в таблиці
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            CheckAdminRight(); //перевірка прав адміністратора
        }
        //подія видалення рядка
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ви точно хочете видалити запис?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                //якщо видаляється адміністратор
                if (dataGridView1[4, dataGridView1.SelectedCells[0].RowIndex].Value.ToString() == "+")
                {
                    int countAdmin = 0;//лічільник адміністраторів
                                       //рахуємо адміністраторів
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        //якщо знайшли
                        if (dataGridView1[4, i].Value.ToString() == "+")
                            countAdmin++;//збільшуємо лічільник
                    }

                    if (countAdmin < 2) //якщо адміністраторів мешне, ніж 2
                    {
                        //повідомлення про заборону видалення
                        MessageBox.Show("Неможливо видалити лишившегося одного адміністратора!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                //відв'язка всіх полів від джерела даних
                textBox1.DataBindings.RemoveAt(0);
                textBox2.DataBindings.RemoveAt(0);
                textBox3.DataBindings.RemoveAt(0);

                //видалення з таблиці
                int index = dataGridView1.SelectedCells[0].RowIndex;
                //видалення з таблиці бази даних
                command = new SQLiteCommand("DELETE FROM User WHERE idUser = @id", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@id", dataGridView1.Rows[index].Cells[0].Value.ToString()));
                //виконання запиту
                SQLiteViewInTable.DeleteDataInBD(command);
                //оновлення виводу таблиці
                FrmSettingUser_Load(sender, e);

                MessageBox.Show("Дані видалено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Активація полів та кнопок
        private void TextBoxActive()
        {
            //активація вводу данних в поля
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            checkBox1.Enabled = true;

            btnSave.Enabled = true;
            btnCancel.Enabled = true;

            //відвязка від бази даних
            textBox1.DataBindings.RemoveAt(0);
            textBox2.DataBindings.RemoveAt(0);
            textBox3.DataBindings.RemoveAt(0);
        }

        //деактивація полів
        private void TextBoxDeActive()
        {
            //деактивація вводу данних в поля
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            checkBox1.Enabled = false;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            TextBoxDeActive(); // деактивація полів вводу
            FrmSettingUser_Load(sender, e); //оновлення виводу в таблицю
            //зкидання прапорців
            btnUpdate = false;
            btnAddEnabled = false;
            //активація функціональних кнопок
            btnChange.Enabled = true;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
        }

        //додавання даних
        bool btnAddEnabled = false;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //очищення полів вводу
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            checkBox1.Checked = false;
            //зкидання прапорців
            btnChange.Enabled = false;
            btnDelete.Enabled = false;

            TextBoxActive();//активація полів
            btnAddEnabled = true; //активація прапорця додавання
            btnCancel.Enabled = true;
            btnAdd.Enabled = false;
        }

        private bool btnUpdate = false;
        private void btnChange_Click(object sender, EventArgs e)
        {
            TextBoxActive(); //активація полів
            btnUpdate = true; //активація прапорця оновлення
            //активація та деактивація потрібних кнопок
            btnCancel.Visible = true;
            btnChange.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckField() != false) // перевірка вводу в поля
            {
                if (btnAddEnabled) //якщо додавання рядка
                {
                    //додавання в таблицю бази даних
                    command = new SQLiteCommand("INSERT INTO User VALUES (NULL, @fio, @login, @pass, @admin)", Program.conn);
                    //додавання параметрів
                    command.Parameters.Add(new SQLiteParameter("@fio", textBox1.Text));
                    command.Parameters.Add(new SQLiteParameter("@login", textBox2.Text));
                    command.Parameters.Add(new SQLiteParameter("@pass", textBox3.Text));
                    if (checkBox1.Checked)
                    {
                        command.Parameters.Add(new SQLiteParameter("@admin", "+"));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("@admin", "-"));
                    }
                    //виконання запиту
                    SQLiteViewInTable.AddDataInBD(command);
                    //оновлення виводу таблиці
                    FrmSettingUser_Load(sender, e);

                    btnAddEnabled = false;
                    btnAdd.Enabled = true;
                    //повідомлення про успіх
                    MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (btnUpdate)
                {
                    //додавання в таблицю бази даних
                    command = new SQLiteCommand("UPDATE User SET fio = @fio, login = @login, password = @pass, rang = @admin WHERE idUser = @id", Program.conn);
                    //додавання параметрів
                    command.Parameters.Add(new SQLiteParameter("@Id", dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value));
                    command.Parameters.Add(new SQLiteParameter("@fio", textBox1.Text));
                    command.Parameters.Add(new SQLiteParameter("@login", textBox2.Text));
                    command.Parameters.Add(new SQLiteParameter("@pass", textBox3.Text));
                    if (checkBox1.Checked)
                    {
                        command.Parameters.Add(new SQLiteParameter("@admin", "+"));
                    }
                    else
                    {
                        command.Parameters.Add(new SQLiteParameter("@admin", "-"));
                    }
                    //виконання запиту
                    SQLiteViewInTable.UpdateDateInBD(command);
                    //оновлення виводу таблиці
                    FrmSettingUser_Load(sender, e);

                    btnUpdate = false;
                    btnChange.Enabled = true;
                    //повідомлення про успіх
                    MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                TextBoxDeActive(); //деактивація полів вводу
                //активація та деактивація потрібних кнопок
                btnSave.Enabled = false;
                btnCancel.Enabled = false;
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
                btnChange.Enabled = true;
            }
        }

        //відображення попередньої форми
        private void FrmSettingUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            //знищення об'єктів роботи з БД
            frmChangeMenu.Visible = true;
            adapterAll.Dispose();
            command.Dispose();
            dataTableAll.Dispose();
            bindingSource = null;
        }

        //закриття форми
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();//закриття
        }
    }
}
