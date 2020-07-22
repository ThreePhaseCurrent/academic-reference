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
    public partial class FrmSettingSpecialization : Form
    {
        FrmChangeMenu frmChangeMenu = null;//об'єкт для попереднього вікна
        //об'єкти для роботи з БД
        SQLiteDataAdapter adapterAll = null;
        SQLiteDataReader dataReader = null;
        SQLiteCommand command1 = null;
        DataTable dataTableAll = null;
        BindingSource bindingSource = null;
        SQLiteCommandBuilder commandBuilder = null;
        //конструктор
        public FrmSettingSpecialization(FrmChangeMenu frmChangeMenu)
        {
            InitializeComponent();

            this.frmChangeMenu = frmChangeMenu;//отримання посилання
            //події корректоного вводу
            tbKey.KeyPress += KeyPress_Key;
            tbNameEN.KeyPress += KeyPressTextBoxEN;
            tbNameUA.KeyPress += KeyPressTextBoxUA;
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування потрібних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSelectSpecialization.htm"))
            {   //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSelectSpecialization.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //обирання відділеня при першій появі форми
        private void FrmSettingSpecialization_Shown(object sender, EventArgs e)
        {
            //команда отримання назв відділень
            SQLiteCommand command2 = new SQLiteCommand("SELECT NameUA FROM Department", Program.conn);
            dataReader = command2.ExecuteReader(); //виконання запиту
            //перевірка, що відділення є в бд
            if (dataReader.HasRows == false)
            {
                //якщо відділення відстуні
                MessageBox.Show("Відсутні відділення! Додайте хоча б одне відділення в налаштуваннях.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); //закриття поточного вікна
            }

            //команда на отримання всіх відділень 
            command1 = new SQLiteCommand("SELECT NameUA FROM Department", Program.conn);
            //заповнення комбінованого списку з відділеннями
            InsertInComboBox(command1, dataReader, cbDepart);

            //виставлення першого відділення за замовчуванням
            try
            {
                cbDepart.SelectedIndex = 1;
            }
            catch
            { }
        }

        //заповнення списку
        private void InsertInComboBox(SQLiteCommand command, SQLiteDataReader dataReader, ComboBox box)
        {
            dataReader = command.ExecuteReader(); //виконання запиту

            if (dataReader.HasRows) //перевірка на існування рядків
            {
                while (dataReader.Read())
                {
                    //додавання в список
                    box.Items.Add(dataReader[0].ToString());
                }
            }
            else
            {
                //деактивація кнопок редагування та видалення
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        //заборона вводу букв
        private void KeyPress_Key(object sender, KeyPressEventArgs e)
        {  //дозвіл чисел, зтирання та спец символів
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar || e.KeyChar == '.') return;
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
        {   //дозвіл вводу англійського мовою
            if ((e.KeyChar < 'A' || e.KeyChar > 'z') && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
        }

        //кнопка закриття форми
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття
        }

        //подія закриття вікна
        private void FrmSettingSpecialization_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmChangeMenu.Visible = true;//відображення попередньої форми
        }

        //відображення нових спеціальностей при зміні відділення
        private void cbDepart_SelectedIndexChanged(object sender, EventArgs e)
        { 
            //перевірка, що обране відділення
            if(cbDepart.SelectedIndex == -1) { return; }
            cbSpec.Items.Clear(); //очищення списку
            //запит на отримання всіх спеціальностей обраного відделення
            SQLiteCommand command2 = new SQLiteCommand($"SELECT NameUA FROM Specialty WHERE idDepartament = (SELECT Department.idDepartment FROM Department WHERE NameUA = @name)", Program.conn);
            command2.Parameters.Add(new SQLiteParameter("@name", cbDepart.Text));
            //вивід спеціальностей у комбінований список
            InsertInComboBox(command2, dataReader, cbSpec);

            //обрання за замовченням першу спеціальність
            try
            {
                cbSpec.SelectedIndex = 0;
            }
            catch
            {
                if (btnUpdate == false)
                {
                    MessageBox.Show("На обраному відділені відсутні спеціальності!\nДодати спеціальності можна в налаштуваннях.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnAdd.Enabled = false;
                //очищення біндінга всіх полів
                tbKey.Clear();
                tbNameEN.Clear();
                tbNameUA.Clear();
                tbKey.DataBindings.Clear();
                tbNameEN.DataBindings.Clear();
                tbNameUA.DataBindings.Clear();
                tbKey.ReadOnly = true;
                tbNameEN.ReadOnly = true;
                tbNameUA.ReadOnly = true;
                btnDelete.Enabled = false;
                btnChange.Enabled = false;

                if (btnUpdate == true)
                {
                    btnCancel_Click(sender, e);
                    cbDepart_SelectedIndexChanged(sender, e);
                }
                try
                {
                    bindingSource.DataSource = null;
                }
                catch { }
                return;
            }
        }

        //обирання спеціальності
        private void cbSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnUpdate == false && btnAddEnabled == false)
            {
                //перевірка, що спеціальність обрана
                if (cbSpec.SelectedIndex == -1) { return; }
                //оновлення даних
                ViewData();
            }
        }

        //відображення даних у таблиці
        private void ViewData()
        {
            //формування запиту
            command1 = new SQLiteCommand("SELECT * FROM specialization WHERE id_specialty = (SELECT idSpecialty FROM Specialty WHERE NameUA = @name)", Program.conn);
            command1.Parameters.Add(new SQLiteParameter("@name", cbSpec.Text));
            //для виводу в таблицю
            SQLiteViewInTable.OutInTable(command1, ref adapterAll, ref dataTableAll, ref bindingSource,
                                ref commandBuilder, ref dataGridView1, ref bindingNavigator1);
            //якщо таблиця пуста то блокуємо деякі кнопки
            if (dataGridView1.Rows.Count == 0)
            {  //деактивація кнопок редагування та видалення
                btnChange.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {  //активація кнопок редагування та видалення
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
            }

            //підпис колонок
            dataGridView1.Columns[1].HeaderText = "Код спеціалізації";
            dataGridView1.Columns[2].HeaderText = "Назва спеціалізації українською";
            dataGridView1.Columns[3].HeaderText = "Назва спеціалізації англійською";

            //зкриття непотрібних стовпців
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[4].Visible = false;

            //очищення біндінга і підвязка полів до джерела даних
            tbKey.DataBindings.Clear();
            tbKey.Clear();
            tbKey.DataBindings.Add("Text", bindingSource, "id_specialization");
            tbNameEN.DataBindings.Clear();
            tbNameEN.Clear();
            tbNameEN.DataBindings.Add("Text", bindingSource, "name_en_spec");
            tbNameUA.DataBindings.Clear();
            tbNameUA.Clear();
            tbNameUA.DataBindings.Add("Text", bindingSource, "name_ua_spec");

            //вставновлення підсказки на заголовках стовпців
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.ToolTipText = "Натисніть для сортування по стовпцю";
            }

            btnAdd.Enabled = true; //активація кнопки додавання
        }

        //перевірка полів
        private bool CheckField()
        {
            //перевірка вводу коду спеціалізації
            if (tbKey.Text == string.Empty)
            {   //повідомлення про помилку
                MessageBox.Show("Введіть код спеціалізації!");
                tbKey.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            //перевірка вводу назви спеціалізації (укр)
            if (tbNameUA.Text == string.Empty)
            {   //повідомлення про помилку
                MessageBox.Show("Введіть українську назву спеціалізації!");
                tbNameUA.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            //перевірка вводу назви спеціалізації (англ)
            if (tbNameEN.Text == string.Empty)
            {   //повідомлення про помилку
                MessageBox.Show("Введіть англійську назву спеціалізації!");
                tbNameEN.Focus();//встановлення фокусу на поле
                return false;//повернення false
            }
            return true;
        }

        //активація полів
        private void FieldActive()
        {
            //активація вводу в полях
            tbKey.ReadOnly = false;
            tbNameUA.ReadOnly = false;
            tbNameEN.ReadOnly = false;
            btnSave.Enabled = true; //активація кнопки зберегти
        }

        //деактивація полів
        private void FieldDeActive()
        {
            //блокування вводу в полях
            tbKey.ReadOnly = true;
            tbNameUA.ReadOnly = true;
            tbNameEN.ReadOnly = true;
            //деактивація кнпоок скасування та збереження
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
        }

        //відвязування полів від бази даних
        private void FieldUntie()
        {
            //відв'язування полів від джерела данних
            tbKey.DataBindings.RemoveAt(0);
            tbNameUA.DataBindings.RemoveAt(0);
            tbNameEN.DataBindings.RemoveAt(0);
        }

        //додавання даних
        bool btnAddEnabled = false;
        //підготовка полів та кнопок для додавання
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //очищення полів
            tbKey.Clear();
            tbNameUA.Clear();
            tbNameEN.Clear();

            FieldActive(); //активація полів вводу
            FieldUntie(); //деактивація полів вводу
            btnAddEnabled = true; 
            //активація та деактивація потрібних кнопок 
            btnDelete.Enabled = false;
            btnChange.Enabled = false;
            btnCancel.Enabled = true;
            btnAdd.Enabled = false;
            btnSave.Enabled = true;
        }

        private bool btnUpdate = false; //прппорцеь для оновлення 
        //підготовка полів та кнопок для оновлення
        private void btnChange_Click(object sender, EventArgs e)
        {
            //активація кнопок додавання, видалення та оновлення
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnChange.Enabled = false;

            FieldActive(); //активація полів вводу
            FieldUntie(); //відв'язування полів вводу
            btnUpdate = true;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
        }

        //відміна дії
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //деактивація кнопки сказсування 
            btnCancel.Enabled = false;
            //зкидання прапорців оновлення та додавання
            btnUpdate = false;
            btnAddEnabled = false;

            FieldDeActive();//деактивація полів вводу

            btnAdd.Enabled = true; //активація кнопки додавання
            //якщо таблиця пуста, то блокуємо видалення та редагування
            if (dataGridView1.Rows.Count == 0)
            {   //деактивація кнопок видалення та редагування
                btnDelete.Enabled = false;
                btnChange.Enabled = false;
            }
            else
            {   //активація кнопок видалення та редагування
                btnDelete.Enabled = true;
                btnChange.Enabled = true;
            }
            ViewData(); //оновлення даних
        }

        //видалення обраного рядка
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Ви точно хочете видалити запис?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                FieldUntie(); //відв'язування полів вводу
                              //код предмету в бд
                int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value);
                //видалення з таблиці бази даних
                command1 = new SQLiteCommand("DELETE FROM specialization WHERE id = @id", Program.conn);
                //додвання параметру
                command1.Parameters.Add(new SQLiteParameter("@id", id));
                //виконання запиту
                SQLiteViewInTable.DeleteDataInBD(command1);
                //повідомлення про видалення
                MessageBox.Show("Дані видалено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //оновлення виводу таблиці
                ViewData();
            }
        }

        //кнопка додавання на навігаторі
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e); //подія додавання
        }

        //кнопка видалення на навігаторі
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e); //подія видалення
        }

        //збереження даних
        private void btnSave_Click(object sender, EventArgs e)
        {
            //перевірка введених даних
            if (!CheckField()) { return; }

            //додавання нового рядка
            if (btnAddEnabled)
            {
                //команда пошуку коду спеціалізації
                SQLiteCommand command3 = new SQLiteCommand("SELECT id_specialization FROM specialization WHERE id_specialization = @id", Program.conn);
                //додавання параметру
                command3.Parameters.Add(new SQLiteParameter("@id", tbKey.Text));
                dataReader = command3.ExecuteReader(); //виконання команди

                if (dataReader.HasRows) //перевірка існування рядкуів
                {
                    //повідомлення про дублікат спеціалізацій
                    MessageBox.Show("Спеціалізація з введеним кодом вже існує!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //додавання в таблицю бази даних
                command1 = new SQLiteCommand("INSERT INTO specialization VALUES (NULL, @id_specialization, @name_ua_spec, @name_en_spec, @id_specialty)", Program.conn);
                //пошук коду спеціальності
                SQLiteCommand command2 = new SQLiteCommand("SELECT idSpecialty FROM Specialty WHERE NameUA = @name", Program.conn);
                //додавання параметру
                command2.Parameters.Add(new SQLiteParameter("@name", cbSpec.Text));
                dataReader = command2.ExecuteReader();//виконання команди

                int id = 0; //код спеціальності
                while (dataReader.Read())
                {
                    id = Convert.ToInt32(dataReader[0]); //знайдений код спеціальності
                }
                //Додавання параметрів
                command1.Parameters.Add(new SQLiteParameter("@id_specialization", tbKey.Text));
                command1.Parameters.Add(new SQLiteParameter("@id_specialty", id));
                command1.Parameters.Add(new SQLiteParameter("@name_ua_spec", tbNameUA.Text));
                command1.Parameters.Add(new SQLiteParameter("@name_en_spec", tbNameEN.Text));
                //виконання запиту
                SQLiteViewInTable.AddDataInBD(command1);
                //оновлення виводу таблиці
                ViewData();
                //активація та деактивація потрібних кнопок
                btnAddEnabled = false;
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
                btnAdd.Enabled = true;
                //повідомлення про успіх
                MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (btnUpdate) //оновлення данних
            {
                //поточний код запису
                string id_row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();

                //команда пошуку спеціалізації
                SQLiteCommand command3 = new SQLiteCommand("SELECT id_specialization, id FROM specialization WHERE id_specialization = @id_spec", Program.conn);
                //Додавання параметрів
                command3.Parameters.Add(new SQLiteParameter("@id_spec", tbKey.Text));
                dataReader = command3.ExecuteReader(); //виконання запиту
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if(dataReader[1].ToString() != id_row)
                        {
                            //повідомлення про дублікат спеціалізації
                            MessageBox.Show($"Помилка! Існує інша спеціалізація з кодом {dataReader[0].ToString()}.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                //знаходження коду спеціальності
                SQLiteCommand command2 = new SQLiteCommand("SELECT idSpecialty FROM Specialty WHERE NameUA = @name", Program.conn);
                //Додавання параметрів
                command2.Parameters.Add(new SQLiteParameter("@name", cbSpec.Text));
                dataReader = command2.ExecuteReader();//виконання запиту

                int id = 0; //код спеціальності
                while (dataReader.Read())
                {
                    id = Convert.ToInt32(dataReader[0]); //знайдений код спеціальності
                }

                //додавання в таблицю бази даних
                command1 = new SQLiteCommand("UPDATE specialization SET id_specialization = @id_specialization, id_specialty = @id_specialty," +
                                            " name_ua_spec = @name_ua_spec, name_en_spec = @name_en_spec WHERE id = @id_row", Program.conn);
                //Додавання параметрів
                command1.Parameters.Add(new SQLiteParameter("@id_specialization", tbKey.Text));
                command1.Parameters.Add(new SQLiteParameter("@id_specialty", id));
                command1.Parameters.Add(new SQLiteParameter("@name_ua_spec", tbNameUA.Text));
                command1.Parameters.Add(new SQLiteParameter("@name_en_spec", tbNameEN.Text));
                command1.Parameters.Add(new SQLiteParameter("@id_row", id_row));

                //виконання запиту
                SQLiteViewInTable.UpdateDateInBD(command1);
                //оновлення виводу таблиці
                ViewData();
                //активація та деактивація потрібних кнопок
                btnUpdate = false;
                btnChange.Enabled = true;
                btnDelete.Enabled = true;
                btnAdd.Enabled = true;
                //повідомлення про успіх
                MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btnSave.Enabled = false;
            FieldDeActive(); //деактивація полів вводу
            btnCancel.Enabled = false;
        }
    }
}
