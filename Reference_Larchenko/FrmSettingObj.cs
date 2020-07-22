using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using Reference_Larchenko;
using System.IO;

namespace Reference_Larchenko
{
    public partial class FrmSettingObj : Form
    {
        FrmChangeMenu frmChangeMenu = null; // об'єкт для попередньої форми
        //об'єкти для роботи з БД
        SQLiteDataAdapter adapterAll = null;
        SQLiteDataReader dataReader = null;
        SQLiteCommand command = null;
        SQLiteCommandBuilder commandBuilder = null;
        DataTable dataTableAll = null;
        BindingSource bindingSource = null;
        DataSet set = new DataSet();
        bool handlerAttached = true;

        private int activeRbtn = 1; //поточний перемикач курсу
        //конструктор
        public FrmSettingObj(FrmChangeMenu frmChangeMenu)
        {
            InitializeComponent();
            this.frmChangeMenu = frmChangeMenu; //отриманя посилання

            numericSemester.ValueChanged += numericSemester_ValueChanged;
            handlerAttached = true;
            //підпис на подію вибору номеру курсу
            radioButton1.Click += rbtn_Click;
            radioButton2.Click += rbtn_Click;
            radioButton3.Click += rbtn_Click;
            rrr.Click += rbtn_Click;
            //вибір першого курсу за замовчуванням
            radioButton1.Checked = true;
            //події для корректного вводу
            textBox2.KeyPress += KeyPressTextBoxUA;
            textBox3.KeyPress += KeyPressTextBoxEN;
            textBox5.KeyPress += Key;
        }

        //заборона вводу букв
        private void Key(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar) return; 
            e.Handled = true;
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
        //подія закриття вікна
        private void FrmSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmChangeMenu.Visible = true; //відображення попереднього вікна
        }

        //заповнення списків з бази даних
        private void FrmSetting_Load(object sender, EventArgs e)
        {
            //вивід в список відділень
            command = new SQLiteCommand("SELECT idDepartment, NameUA FROM Department", Program.conn);
            DataView(command, dataReader, comboBox1); //оновлення списку
            try
            {  //обирання за замовчуванням
                comboBox1.SelectedIndex = 0;
            }
            catch
            {
                //повідомлення про помилку
                MessageBox.Show("Відсутні відділення!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); //закриття форми
            }
            //вивід у список спеціальностей
            command = new SQLiteCommand("SELECT idSpecialty, NameUA FROM Specialty WHERE idDepartament = (SELECT Department.idDepartment FROM Department WHERE NameUA = @name)", Program.conn);
            //доавдання параметру
            command.Parameters.Add(new SQLiteParameter("@name", comboBox1.Text));
            DataView(command, dataReader, comboBox2);//оновлення списку
            //вибір першої спеціальності за замовчуванням
            try
            {   //обирання за замовчуванням
                comboBox2.SelectedIndex = 0;
            }
            catch
            {
                //повідомлення про помилку
                MessageBox.Show("В даному відділені немає спеціальностей!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            View(comboBox3, 1, 1); //оновлення даних в таблиці
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {   //перевірка існування потрібних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSettingSubject.htm"))
            {   //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSettingSubject.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //вивід даних
        private void DataView(SQLiteCommand command, SQLiteDataReader dataReader, ComboBox box)
        {
            dataReader = command.ExecuteReader(); //виконання команди

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    //додавання в список
                    box.Items.Add(dataReader[1].ToString());
                }
            }
            else
            {   //деактивація кнопок редагування та видалення
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        //зміна спеціальностей у списку
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            try
            {
                dataTableAll.Clear(); //очищення об'єкту БД
            }
            catch { }

            //вимикання оновлення
            if (btnUpdate == true)
            {
                btnCancel_Click(sender, e);
                btnUpdate = false;
            }
            //очищення списків
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            //команда пошуку спеціальностей
            command = new SQLiteCommand("SELECT idSpecialty, NameUA FROM Specialty WHERE idDepartament = (SELECT Department.idDepartment FROM Department WHERE NameUA = @name)", Program.conn);
            command.Parameters.Add(new SQLiteParameter("@name", comboBox1.Text));
            DataView(command, dataReader, comboBox2); //оновлення даних в  списку
            try
            {
                comboBox2.SelectedIndex = 0; //обирання за замовчуванням
            }
            catch
            {
                comboBox2.SelectedIndex = -1;
                btnAdd.Enabled = false;
                //блокування перемикачів при помилці
                numericSemester.Enabled = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                rrr.Enabled = false;
                btnCancel.Enabled = false;
                btnSaveData.Enabled = false;

                //очищення полів
                textBox2.Clear();
                textBox3.Clear();
                textBox5.Clear();
                //Блокування вводу
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true; ;
                textBox5.ReadOnly = true;
                //очищення джерел даних
                textBox2.DataBindings.Clear();
                textBox3.DataBindings.Clear();
                textBox5.DataBindings.Clear();

                if (btnUpdate == true)
                {
                    if (handlerAttached == false)
                    {
                        numericSemester.ValueChanged += numericSemester_ValueChanged;
                        handlerAttached = true;
                    }
                    btnUpdate = false;
                }
                try
                {
                    dataTableAll.Clear(); //очищення об'єкту БД
                }
                catch { }
            }
        }

        //перемикання номеру курсу
        private void rbtn_Click(object sender, EventArgs e)
        {
            var r = sender as RadioButton; //приведення типу
            activeRbtn = Convert.ToInt32(r.Tag); //отримання номеру курсу по тегу
            //перемикамикання пемикача з семестрами
            if(activeRbtn == 1) { numericSemester.Minimum = 1;  numericSemester.Maximum = 2; }
            else if(activeRbtn == 2) { numericSemester.Minimum = 3; numericSemester.Maximum = 4; }
            else if (activeRbtn == 3) { numericSemester.Minimum = 5; numericSemester.Maximum = 6; }
            else { numericSemester.Minimum = 7; numericSemester.Maximum = 8; }
        }

        //відображення даних у таблиці
        private void View(ComboBox comboBox3, int x, int semester)
        {
            //формування запиту
            command = new SQLiteCommand("SELECT * FROM Subject WHERE id_specialization = (SELECT id_specialization FROM specialization WHERE name_ua_spec = @name) AND SemesterNum = @semester", Program.conn);
            //додавання параметрів
            command.Parameters.Add(new SQLiteParameter("@name", comboBox3.Text));
            command.Parameters.Add(new SQLiteParameter("@semester", semester));
            //для виводу в таблицю
            SQLiteViewInTable.OutInTable(command, ref adapterAll, ref dataTableAll, ref bindingSource,
                                ref commandBuilder, ref dataGridView1, ref bindingNavigator1);

            //якщо таблиця пуста то блокуємо деякі кнопки
            if(dataGridView1.Rows.Count == 0)
            {   //декативація оновлення та видалення
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {   //кативація оновлення та видалення
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }

            //підпис колонок
            dataGridView1.Columns[0].HeaderText = "Код Предмету";
            dataGridView1.Columns[1].HeaderText = "Код Спеціальності";
            dataGridView1.Columns[3].HeaderText = "Назва українська";
            dataGridView1.Columns[4].HeaderText = "Назва англійська";
            dataGridView1.Columns[5].HeaderText = "Загальний обсяг";

            //зкриття непотрібних комірок
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;

            //ширина комірок
            dataGridView1.Columns[3].Width = 180;
            dataGridView1.Columns[4].Width = 180;

            //очищення біндінга і підвязка полів до джерела даних
            textBox2.DataBindings.Clear();
            textBox2.Clear();
            textBox2.DataBindings.Add("Text", bindingSource, "NameUA");
            textBox3.DataBindings.Clear();
            textBox3.Clear();
            textBox3.DataBindings.Add("Text", bindingSource, "NameEN");
            textBox5.DataBindings.Clear();
            textBox5.Clear();
            textBox5.DataBindings.Add("Text", bindingSource, "OverSize");

            //вставновлення підсказки на заголовках стовпців
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.ToolTipText = "Натисніть для сортування по стовпцю";
            }
            //активація панелі та кнопки додавання
            groupBox2.Enabled = true;
            btnAdd.Enabled = true;
            //активація всіх перемикачів
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;
            rrr.Enabled = true;
            numericSemester.Enabled = true;
        }

        //номер предмету
        string idSubjectd = string.Empty;
        //збереження даних в базі 
        private void btnSaveData_Click(object sender, EventArgs e)
        {
            //перевірка введення даних
            if (!CheckField()) { return; }

            if (btnAddEnabled) //якщо кнопка додати
            {
                //додавання в таблицю бази даних
                command = new SQLiteCommand("INSERT INTO Subject VALUES (NULL, @idSpec, @semester, @NameUA, @NameEN, @Size)", Program.conn);
                //команда пошуку коду спеціалізації
                SQLiteCommand command2 = new SQLiteCommand("SELECT id_specialization FROM specialization WHERE name_ua_spec = @name", Program.conn);
                //доадвання параметру
                command2.Parameters.Add(new SQLiteParameter("@name", comboBox3.Text));
                dataReader = command2.ExecuteReader(); //виконаня команди

                int idSpec = 0; //код спеціалізації
                while (dataReader.Read())
                {
                    idSpec = Convert.ToInt32(dataReader[0]);//знайдений код спеціалізації
                }
                //доадвання параметру
                command.Parameters.Add(new SQLiteParameter("@idSpec", idSpec));
                command.Parameters.Add(new SQLiteParameter("@semester", numericSemester.Value));
                command.Parameters.Add(new SQLiteParameter("@NameUA", textBox2.Text));
                command.Parameters.Add(new SQLiteParameter("@NameEN", textBox3.Text));
                command.Parameters.Add(new SQLiteParameter("@Size", textBox5.Text));
                //виконання запиту
                SQLiteViewInTable.AddDataInBD(command);
                //оновлення виводу таблиці
                View(comboBox3, activeRbtn, Convert.ToInt32(numericSemester.Value));
                //Активація та деактивація потрібних кнопок
                btnAddEnabled = false;
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
                btnAdd.Enabled = true;
                //повідомлення про успіх
                MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (btnUpdate) //якщо натиснута кнопку редагувати
            {
                //знаходження коду спеціальності
                SQLiteCommand command2 = new SQLiteCommand("SELECT id_specialization FROM specialization WHERE name_ua_spec = @name", Program.conn);
                //доадвання параметру
                command2.Parameters.Add(new SQLiteParameter("@name", comboBox3.Text));
                dataReader = command2.ExecuteReader();//виконаня команди

                int idSpec = 0;//код спеціалізації
                while (dataReader.Read())
                {
                    idSpec = Convert.ToInt32(dataReader[0]);//знайдений код спеціалізації
                }

                //додавання в таблицю бази даних
                command = new SQLiteCommand("UPDATE Subject SET id_specialization = @idSpec, SemesterNum = @semester, NameUA = @nameUA," +
                                            " NameEN = @nameEN, OverSize = @size WHERE idSubject = @idSubjectd", Program.conn);
                //доадвання параметру
                command.Parameters.Add(new SQLiteParameter("@idSpec", idSpec));
                command.Parameters.Add(new SQLiteParameter("@semester", numericSemester.Value));
                command.Parameters.Add(new SQLiteParameter("@nameUA", textBox2.Text));
                command.Parameters.Add(new SQLiteParameter("@nameEN", textBox3.Text));
                command.Parameters.Add(new SQLiteParameter("@size", textBox5.Text));
                command.Parameters.Add(new SQLiteParameter("@idSubjectd", idSubjectd));
                //виконання запиту
                SQLiteViewInTable.UpdateDateInBD(command);
                //оновлення виводу таблиці
                View(comboBox3, activeRbtn, Convert.ToInt32(numericSemester.Value));
                //Активація та деактивація потрібних кнопок
                btnUpdate = false;
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
                btnAdd.Enabled = true;
                //повідомлення про успіх
                MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (handlerAttached == false)
            {
                numericSemester.ValueChanged += numericSemester_ValueChanged;
                handlerAttached = true;
            }
            btnSaveData.Enabled = false;
            FieldDeActive();
            btnCancel.Enabled = false;
        }

        //перевірка полів
        private bool CheckField()
        {
            if(textBox2.Text == "")
            {
                MessageBox.Show("Введіть українську назву!");
                textBox2.Focus();
                return false;
            }

            if (textBox3.Text == "")
            {
                MessageBox.Show("Введіть англійську назву!");
                textBox3.Focus();
                return false;
            }

            if (textBox5.Text == "")
            {
                MessageBox.Show("Введіть загальний обсяг!");
                textBox5.Focus();
                return false;
            }

            return true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //додавання даних
        bool btnAddEnabled = false;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;

            textBox2.Clear();
            textBox3.Clear();
            textBox5.Clear();
            
            FieldActive();
            FieldUntie();
            btnAddEnabled = true;

            btnDelete.Enabled = false;
            btnChange.Enabled = false;
            btnCancel.Enabled = true;
            btnAdd.Enabled = false;
            btnSaveData.Enabled = true;
        }

        //активація полів
        private void FieldActive()
        {
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox5.ReadOnly = false;
            btnSaveData.Visible = true;
            btnSaveData.Enabled = true;
        }

        //деактивація полів
        private void FieldDeActive()
        {
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox5.ReadOnly = true;
            btnCancel.Enabled = false;
            btnSaveData.Enabled = false;
        }

        //відвязування полів від бази даних
        private void FieldUntie()
        {
            textBox2.DataBindings.RemoveAt(0);
            textBox3.DataBindings.RemoveAt(0);
            textBox5.DataBindings.RemoveAt(0);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ви точно хочете видалити запис?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                FieldUntie();
                //код предмету в бд
                int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value);
                //видалення з таблиці бази даних
                command = new SQLiteCommand("DELETE FROM Subject WHERE idSubject = @id", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@id", id));
                //виконання запиту
                SQLiteViewInTable.DeleteDataInBD(command);

                MessageBox.Show("Дані видалено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //оновлення виводу таблиці
                View(comboBox3, activeRbtn, Convert.ToInt32(numericSemester.Value));
            }
        }

        private bool btnUpdate = false;
        private void btnChange_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnChange.Enabled = false;

            FieldActive();
            FieldUntie();
            btnUpdate = true;
            btnCancel.Enabled = true;
            btnSaveData.Enabled = true;

            try
            {
                idSubjectd = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            }
            catch { }

            if (handlerAttached == true)
            {
                numericSemester.ValueChanged -= numericSemester_ValueChanged; //відв'язка від події
                handlerAttached = false;
            }
        }

        //відміна дії
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (handlerAttached == false)
            {
                numericSemester.ValueChanged += numericSemester_ValueChanged;
                handlerAttached = true;
            }
            btnCancel.Enabled = false;
            btnUpdate = false;
            btnAddEnabled = false;

            FieldDeActive();
            View(comboBox3, activeRbtn, Convert.ToInt32(numericSemester.Value));

            btnAdd.Enabled = true;
            if (dataGridView1.Rows.Count == 0)
            {
                btnDelete.Enabled = false;
                btnChange.Enabled = false;
            }
            else
            {
                btnDelete.Enabled = true;
                btnChange.Enabled = true;
            }
        }

        private void numericSemester_ValueChanged(object sender, EventArgs e)
        {
            if (textBox2.DataBindings.Count != 0)
            {
                FieldUntie();
            }
            View(comboBox3, activeRbtn, Convert.ToInt32(numericSemester.Value));
        }

        //кнопка додати на навігаторі
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        //кнопка видалити на навігаторі
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        //вивід даних при виборі спеціальності
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //перевірка що спеціальність обрана
            if(comboBox2.SelectedIndex == -1) {
                btnAdd.Enabled = false;
                return;
            }
            
            //
            if(btnUpdate == true)
            {
                btnCancel_Click(sender, e);
                btnUpdate = false;
            }
            comboBox3.Items.Clear();//очищення списку спеціалізацій
            //команда на отримання спеціалізацій
            SQLiteCommand command2 = new SQLiteCommand("SELECT name_ua_spec FROM specialization WHERE id_specialty = (SELECT idSpecialty FROM Specialty WHERE NameUA = @name)", Program.conn);
            command2.Parameters.Add(new SQLiteParameter("@name", comboBox2.Text));
            SQLiteDataReader dataReader2 = command2.ExecuteReader(); //виконання команди

            //додавання спеціалізацій у список
            while (dataReader2.Read())
            {
                //додавання в список
                comboBox3.Items.Add(dataReader2[0].ToString());
            }

            try
            {
                //виставлення за замовчуванням 
                comboBox3.SelectedIndex = 0;
            }
            catch {
                btnAdd.Enabled = false;
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
                btnCancel.Enabled = false;
                btnSaveData.Enabled = false;
                numericSemester.Enabled = false;
                
                if(btnUpdate == true)
                {
                    //btnCancel_Click(sender, e);
                    if (handlerAttached == false)
                    {
                        numericSemester.ValueChanged += numericSemester_ValueChanged;
                        handlerAttached = true;
                    }
                    btnUpdate = false;
                }
                //очищення полів
                textBox2.Clear();
                textBox3.Clear();
                textBox5.Clear();
                //Блокування вводу
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true; ;
                textBox5.ReadOnly = true;
                //очищення джерел даних
                textBox2.DataBindings.Clear();
                textBox3.DataBindings.Clear();
                textBox5.DataBindings.Clear();
                //деактивація перемикачів
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                rrr.Enabled = false;
                try
                {
                    bindingSource.DataSource = null;//онулювання джерела даних
                }
                catch { }
            }
        }
        //подія перемикання семестру
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(btnUpdate == true)
            {
                btnCancel_Click(sender, e);
                btnUpdate = false;
            }
            numericSemester_ValueChanged(sender, e); //оновлення даних в таблиці
        }
    }
}
