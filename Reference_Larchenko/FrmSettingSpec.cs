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
    public partial class FrmSettingSpec : Form
    {
        FrmChangeMenu frmChangeMenu = null; //об'єкт для попереднього вікна
        //об'єкти для роботи з БД
        SQLiteDataAdapter adapterAll = null;
        SQLiteDataReader dataReader = null;
        SQLiteCommand command = null;
        SQLiteCommandBuilder commandBuilder = null;
        DataTable dataTableAll = null;
        BindingSource bindingSource = null;
        //конструктор
        public FrmSettingSpec(FrmChangeMenu frmChangeMenu)
        {
            InitializeComponent();

            this.frmChangeMenu = frmChangeMenu;//отримання посилання
            //події корректоного вводу
            textBox1.KeyPress += Key;
            textBox2.KeyPress += KeyPressTextBoxUA;
            textBox3.KeyPress += KeyPressTextBoxEN;
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
        {   //дозвіл вводу англійського мовою
            if ((e.KeyChar < 'A' || e.KeyChar > 'z') && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
        }

        //заборона вводу букв
        private void Key(object sender, KeyPressEventArgs e)
        {   //дозвіл чисел, зтирання та спец символів
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar) return; 
            e.Handled = true;
            return;
        }
        //повернення на попеерднє вікно
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття поточного вікна
        }
        //подія закриття вікна
        private void FrmSettingSpec_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmChangeMenu.Visible = true; //відображення попереднього вікна
        }
        //налаштування до появи вікна
        private void FrmSettingSpec_Load(object sender, EventArgs e)
        {
            //команда на вивід відділень в список
            command = new SQLiteCommand("SELECT idDepartment, NameUA FROM Department", Program.conn);
            dataReader = command.ExecuteReader(); //виконання команди

            while (dataReader.Read())
            {
                //додавання в список
                cmbxDepart.Items.Add(dataReader[1].ToString());
            }

            try
            {
                //виставлення вибору за замовчуванням
                cmbxDepart.SelectedIndex = 0;
            }
            catch
            {
                //повідомлення про помилку
                MessageBox.Show("Відділення відсутні!","Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); //закриття вікна
            }
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування потрібних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSettingSpecialty.htm"))
            {   //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSettingSpecialty.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //подія змніи відділення
        private void cmbxDepart_SelectedIndexChanged(object sender, EventArgs e)
        {
            //якщо поля прив'язані до бази даних -- відвязуємо
            if (textBox1.DataBindings.Count == 1)
            {
                FieldUntie(); //відв'язування полів
            }
            //формування запиту
            command = new SQLiteCommand("SELECT * FROM Specialty WHERE idDepartament = (SELECT idDepartment FROM Department WHERE NameUA = @name)", Program.conn);
            command.Parameters.Add(new SQLiteParameter("@name", cmbxDepart.Text)); //додвання парметрів

            //вивід даних в таблицю
            SQLiteViewInTable.OutInTable(command, ref adapterAll, ref dataTableAll, ref bindingSource,
                                ref commandBuilder, ref dataGridView1, ref bindingNavigator1);

            //якщо таблиця пуста то блокуємо деякі кнопки
            if(dataGridView1.Rows.Count == 0)
            {
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
            else //або активуємо
            {
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }
            //зкриття деяких стовпців
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[0].Visible = false;
            //підписи стовпців в таблиці
            dataGridView1.Columns[1].HeaderText = "Код спеціальності";
            dataGridView1.Columns[3].HeaderText = "Спеціальність українською";
            dataGridView1.Columns[4].HeaderText = "Спеціальність англійською";

            //підвязуємо до бази даних за необхідності
            if (textBox1.DataBindings.Count == 0)
            {
                //очищення полів
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                //підв'язка до джерела даних
                textBox1.DataBindings.Add("Text", bindingSource, "idSpecialty");
                textBox2.DataBindings.Add("Text", bindingSource, "NameUA");
                textBox3.DataBindings.Add("Text", bindingSource, "NameEN");
            }

            //вставновлення підсказки на заголовках стовпців
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.ToolTipText = "Натисніть для сортування по стовпцю";
            }
        }

        //відвязування полів від бази даних
        private void FieldUntie()
        {
            //відв'язка від джерела данних
            textBox1.DataBindings.RemoveAt(0);
            textBox2.DataBindings.RemoveAt(0);
            textBox3.DataBindings.RemoveAt(0);
        }

        //подія видалення рядка
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ви точно хочете видалити запис?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                //видалення з таблиці
                int index = dataGridView1.SelectedCells[0].RowIndex;
                //видалення з таблиці бази даних
                command = new SQLiteCommand("DELETE FROM Specialty WHERE id_row = @id", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@id", dataGridView1.Rows[index].Cells[0].Value.ToString()));
                //виконання запиту
                SQLiteViewInTable.DeleteDataInBD(command);
                //оновлення виводу таблиці
                cmbxDepart_SelectedIndexChanged(sender, e);
                //повідомлення про видалення
                MessageBox.Show("Дані видалено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //додавання даних
        bool btnAddEnabled = false;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //очищення полів вводу
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();

            FieldActive(); //активація полів вводу
            FieldUntie(); //відв'язка полів вводу від БД
            //активація та деактивація потрбіних кнопок
            btnAddEnabled = true;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnChange.Enabled = false;
        }

        //активація полів
        private void FieldActive()
        {
            //дозвіл на вписування в поля
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            //активація кнопки зберегти
            btnSave.Visible = true;
            btnSave.Enabled = true;
        }

        //деактивація полів
        private void FieldDeActive()
        {
            //блокування вводу в поля
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            //деактивація кнопок зберегти та відмінити
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
        }

        //додавання даних
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckField() != false) //перевірка правильності введення даних
            {
                if (btnAddEnabled) //якщо натистута кнопка додати
                {
                    //команда додавання в таблицю бази даних
                    command = new SQLiteCommand("INSERT INTO Specialty VALUES (NULL, @id, @idDepart, @NameUA, @NameEN)", Program.conn);
                    //комада пошуку коду відділення
                    SQLiteCommand command2 = new SQLiteCommand("SELECT idDepartment FROM Department WHERE NameUA = @name", Program.conn);
                    //додавання параметру
                    command2.Parameters.Add(new SQLiteParameter("@name", cmbxDepart.Text));
                    dataReader = command2.ExecuteReader(); //виконання запиту

                    int id = 0; //код за замовчуванням
                    while (dataReader.Read())
                    {
                        id = Convert.ToInt32(dataReader[0]); //отримання коду відділення з БД
                    }
                    //додавання параметрів
                    command.Parameters.Add(new SQLiteParameter("@id", textBox1.Text));
                    command.Parameters.Add(new SQLiteParameter("@idDepart", id));
                    command.Parameters.Add(new SQLiteParameter("@NameUA", textBox2.Text));
                    command.Parameters.Add(new SQLiteParameter("@NameEN", textBox3.Text));
                    //виконання запиту
                    SQLiteViewInTable.AddDataInBD(command);
                    //оновлення виводу таблиці
                    cmbxDepart_SelectedIndexChanged(sender, e);
                    //зміна прапорця додавання
                    btnAddEnabled = false;
                    //повідомлення про виконання додавання
                    MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (btnUpdate) // якщо натиснута кнопка редагувати
                {
                    try
                    {
                        //комада пошуку коду відділення
                        SQLiteCommand command2 = new SQLiteCommand("SELECT idDepartment FROM Department WHERE NameUA = @name", Program.conn);
                        //додавання параметру
                        command2.Parameters.Add(new SQLiteParameter("@name", cmbxDepart.Text));
                        dataReader = command2.ExecuteReader();//виконання запиту

                        int idDepartment = 0;//код за замовчуванням
                        while (dataReader.Read())
                        {
                            idDepartment = Convert.ToInt32(dataReader[0]);//отримання коду відділення з БД
                        }

                        //додавання в таблицю бази даних
                        command = new SQLiteCommand("UPDATE Specialty SET idSpecialty = @idSpec, idDepartament = @idDepart, NameUA = @nameUA," +
                                                    " NameEN = @nameEN WHERE id_row = @idCurrent", Program.conn);
                        //додавання параметрів
                        command.Parameters.Add(new SQLiteParameter("@idSpec", textBox1.Text));
                        command.Parameters.Add(new SQLiteParameter("@idDepart", idDepartment));
                        command.Parameters.Add(new SQLiteParameter("@nameUA", textBox2.Text));
                        command.Parameters.Add(new SQLiteParameter("@nameEN", textBox3.Text));
                        command.Parameters.Add(new SQLiteParameter("@idCurrent", dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value));

                        //виконання запиту
                        SQLiteViewInTable.UpdateDateInBD(command);
                        
                        //якщо відділення змінювалось
                        if (updateDepart == true)
                        {
                            btnSaveUpdateDepartForSpec();
                            updateDepart = false; //деацтивація прапорця редагування
                        }
                        //змінаподії для випадаючого списку
                        cmbxDepart.SelectedIndexChanged -= cmbxDepart_IndexChangedForUpdate;
                        cmbxDepart.SelectedIndexChanged += cmbxDepart_SelectedIndexChanged;

                        //оновлення виводу таблиці
                        cmbxDepart_SelectedIndexChanged(sender, e);
                    } //повідомення про помилку
                    catch { MessageBox.Show("Cпеціальність з кодом " + textBox1.Text + " вже існує!"); return; }

                    btnUpdate = false;
                    //повідомлення про успіх
                    MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //активація та деактивація потрібних кнопок
                btnSave.Enabled = false;
                btnCancel.Enabled = false;
                btnAdd.Enabled = true;
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
                FieldDeActive();//деактивація полів
            }
        }

        //перевірка полів
        private bool CheckField()
        {
            //перевірка вводу коду спеціальності
            if (textBox1.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть код спеціальності!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            //перевірка вводу назви спеціальності (укр)
            if (textBox2.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть українську назву спеціальності!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox2.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            //перевірка вводу назви спеціальності (англ)
            if (textBox3.Text == "")
            {  //повідомлення про помилку
                MessageBox.Show("Введіть англійську назву спеціальності!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox3.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            return true;
        }

        //оновлення даних
        private bool btnUpdate = false;
        private void btnChange_Click(object sender, EventArgs e)
        {
            FieldActive(); //активація полів вводу
            FieldUntie(); //відв'язування полів від БД
            btnUpdate = true; //прапорець оновлення
            btnAddEnabled = false;
            //активація та деактивація потрібних кнопок
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnChange.Enabled = false;
            //відвязування події виводу інших спеціальностей
            cmbxDepart.SelectedIndexChanged -= cmbxDepart_SelectedIndexChanged;
            cmbxDepart.SelectedIndexChanged += cmbxDepart_IndexChangedForUpdate;
        }

        //відміна дії
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //деактивація кнопок збереження, додавання та скасування 
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnAdd.Enabled = true;
            btnAddEnabled = false; //прапорець додавання

            FieldDeActive(); //деактивація полів вводу
            cmbxDepart_SelectedIndexChanged(sender, e); //оновлення списку спеціальностей

            //якщо таблиця порожня
            if (dataGridView1.Rows.Count == 0)
            {  //деактивація кнопок оновлення та редагування
                btnDelete.Enabled = false;
                btnChange.Enabled = false;
            }
            else
            {   //активація кнопок оновлення та редагування
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }

            //поновлення події виводу інших спеціальностей
            if(btnUpdate == true)
            {
                cmbxDepart.SelectedIndexChanged -= cmbxDepart_IndexChangedForUpdate;
                cmbxDepart.SelectedIndexChanged += cmbxDepart_SelectedIndexChanged;
            }
            //скидання прапорців оновлення
            updateDepart = false;
            btnUpdate = false;
        }
    
        //кнопка видалення в навігаторі
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e); //видалення рядку
        }

        private bool updateDepart = false; //прапорець оновлення
        //подія чи було змінене відділення при оновленні даних про спеціальність
        private void cmbxDepart_IndexChangedForUpdate(object sender, EventArgs e)
        {
            updateDepart = true;//прапорець оновлення
        }

        //оновлення відділення для спеціальності (за потреби)
        private void btnSaveUpdateDepartForSpec()
        {
            //назва обраної нового відділення для спеціальності
            string selectedDepart = cmbxDepart.Items[cmbxDepart.SelectedIndex].ToString();
            //код обраного відділення
            int idSelectedDepart = 0;

            //отримуємо код відділення з БД
            command = new SQLiteCommand("SELECT idDepartment FROM Department WHERE NameUA = @selectedDepart", Program.conn);
            //додавання параметру
            command.Parameters.Add(new SQLiteParameter("@selectedDepart", selectedDepart));

            dataReader = command.ExecuteReader(); //виконання команди
            while (dataReader.Read())
            {
                //отримання коду відділення
                idSelectedDepart = Convert.ToInt32(dataReader[0]);
            }

            //оновлюємо відділення для спеціальності
            command = new SQLiteCommand("UPDATE Specialty SET idDepartament = @idSelectedDepart WHERE NameUA = @nameUASpec", Program.conn);
            //додавання параметрів
            command.Parameters.Add(new SQLiteParameter("@idSelectedDepart", idSelectedDepart));
            command.Parameters.Add(new SQLiteParameter("@nameUASpec", textBox2.Text));
            command.ExecuteReader(); //виконання команди
        } 
        //подія додавання на навігаторі
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e); //подія додавання
        }
    }
}
