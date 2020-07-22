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
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using System.Threading;

namespace Reference_Larchenko
{
    public partial class CreateReference : Form
    {
        //об'єкти для працювання з БД та докуметом
        Word.Application application;
        public FrmStart frmStart = null;
        public Student student = new Student();
        SQLiteCommand command = null;
        SQLiteDataReader dataReader = null;

        public int currentSemester = 1;

        //шлях до існуючого заповненого шаблону
        public static string templatePath = null;

        public FrmOpenReference frmOpenReference = null;
        Account account;

        #region Конструктори
        //звичайний конструктор
        public CreateReference(FrmStart frmStart, Account account)
        {
            InitializeComponent();

            this.frmStart = frmStart;
            this.account = account;
            //номер курсу
            courseNum.SelectedIndex = 0;
            //причина відрахування
            comboBoxExpelled.SelectedIndex = 0;
            //запит для заповнення списку
            command = new SQLiteCommand("SELECT NameUA, NameEN FROM Department", Program.conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                FacultyText.Items.Add(dataReader[0].ToString());
            }
            FacultyText.SelectedIndex = 0;
            //заборона вводу цифр
            CreateAction();
        }

        //конструктор для збереженної довідки
        public CreateReference(FrmOpenReference frmOpenReference, int selectId, Account account)
        {
            InitializeComponent();

            this.frmOpenReference = frmOpenReference;
            this.account = account;
            //заповнення полів з бд
            OpenSavedReference(selectId);
            //заборона вводу цифр
            CreateAction();
            //зчитування поточного семестру
            currentSemester = Convert.ToInt32(courseNum.Items[courseNum.SelectedIndex]);
        }

        //підвязка подій
        public void CreateAction()
        {
            SecNameUAText.KeyPress += KeyPressTextBoxUA;
            SecNameENText.KeyPress += KeyPressTextBoxEN;
            FistNameUAText.KeyPress += KeyPressTextBoxUA;
            FistNameENText.KeyPress += KeyPressTextBoxEN;
            PetrNameUAText.KeyPress += KeyPressTextBoxUA;
            PetrNameENText.KeyPress += KeyPressTextBoxEN;
            placeBirthUAText.KeyPress += KeyPressTextBoxUA;
            placeBirthENText.KeyPress += KeyPressTextBoxEN;
            numEdboText.KeyPress += Key;
        }
        #endregion

        //загрузка форми
        private void CreateReference_Load(object sender, EventArgs e)
        {
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            FrmStart.ShowInstruction(@"Instructions\2.Вікно введення інформації про студента.docx");
        }

        CreateReference2 createReference2 = null;
        
        //переход ко второму шагу
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (DataCheck(student) == false)
            {
                return;
            }

            this.Visible = false;

            createReference2 = new CreateReference2(this, Convert.ToInt32(courseNum.Text));
            createReference2.ShowDialog();
        }

        //попередній перегляд документу
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (DataCheck(student) == false)
            {
                return;
            }

            DialogResult result = MessageBox.Show("Перейти до попередьного огляду документа?", "Організація попереднього огляду", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //створюємо об'єкт додатку
                application = new Word.Application();
                // створюємо шлях до файлу
                Object templatePathObj = System.IO.Path.GetFullPath(@"12.docx");
                application.Documents.Open(templatePathObj, ReadOnly: true);

                //заповнення номеру довідки
                application.Documents[1].Tables[2].Cell(1, 1).Range.Font.Size = 24;
                application.Documents[1].Tables[2].Cell(1, 1).Range.Bold = 0;
                application.Documents[1].Bookmarks["numberreference"].Range.Text = "№ " + student.orderNumber;

                //ім'я
                application.Documents[1].Bookmarks["Name"].Range.Text = student.secNameUA + " " + student.firstNameUA + " " + student.petrNameUA;
                application.Documents[1].Bookmarks["Name2"].Range.Text = student.secNameEN + " " + student.firstNameEN + " " + student.petrNameEN; ;

                //день початку та завершення навчання
                application.Documents[1].Bookmarks["day1_1"].Range.Text = student.dateStart.Day.ToString();
                application.Documents[1].Bookmarks["lastday1_1"].Range.Text = student.dateStart.Month.ToString() + "." + student.dateStart.Year.ToString();
                application.Documents[1].Bookmarks["day1_2"].Range.Text = student.dateFinish.Day.ToString();
                application.Documents[1].Bookmarks["lastday1_2"].Range.Text = student.dateFinish.Month.ToString() + "." + student.dateFinish.Year.ToString();
                application.Documents[1].Bookmarks["day2_1"].Range.Text = student.dateStart.Day.ToString();
                application.Documents[1].Bookmarks["lastday2_1"].Range.Text = student.dateStart.Month.ToString() + "." + student.dateStart.Year.ToString();
                application.Documents[1].Bookmarks["day2_2"].Range.Text = student.dateFinish.Day.ToString();
                application.Documents[1].Bookmarks["lastday2_2"].Range.Text = student.dateFinish.Month.ToString() + "." + student.dateFinish.Year.ToString();

                //номер в ЄДБО
                application.Documents[1].Bookmarks["Edbo"].Range.Text = student.numEdbo;
                application.Documents[1].Bookmarks["EdboEn"].Range.Text = student.numEdbo;

                //день народження
                application.Documents[1].Bookmarks["Birth1_1Rus"].Range.Text = student.dateBirthday.Day.ToString();
                application.Documents[1].Bookmarks["Birth1_2Rus"].Range.Text = student.dateBirthday.Month.ToString() + "." + student.dateBirthday.Year.ToString();
                application.Documents[1].Bookmarks["Birth2_1En"].Range.Text = student.dateBirthday.Day.ToString();
                application.Documents[1].Bookmarks["Birth2_2En"].Range.Text = student.dateBirthday.Month.ToString() + "." + student.dateBirthday.Year.ToString();

                //місце народження
                application.Documents[1].Bookmarks["CountryRus"].Range.Text = student.placeBirthUA;
                application.Documents[1].Bookmarks["CountryEn"].Range.Text = student.placeBirthEn;

                //причина відрахування
                application.Documents[1].Bookmarks["ExpelledUa"].Range.Text = student.expelled;
                application.Documents[1].Bookmarks["ExpelledEn"].Range.Text = student.expelledEN[student.numexpelled];

                //запит на назву спеціальності за кодом
                command = new SQLiteCommand("SELECT NameUA, NameEN FROM Specialty WHERE idSpecialty = @id", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@id", student.specNum.ToString()));
                dataReader = command.ExecuteReader();

                //назва навчального закладу
                while (dataReader.Read())
                {
                    //номер спеціальності
                    application.Documents[1].Bookmarks["CodeRus"].Range.Text = student.specNum.ToString() + " " + dataReader[0].ToString();
                    application.Documents[1].Bookmarks["CodeEn"].Range.Text = student.specNum.ToString() + " " + dataReader[1].ToString();
                }
                dataReader.Close();

                //назва навчального закладу
                command = new SQLiteCommand("SELECT NameUA, NameEN FROM College", Program.conn);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    application.Documents[1].Bookmarks["InstNameRus"].Range.Text = dataReader[0].ToString();
                    application.Documents[1].Bookmarks["InstNameEn"].Range.Text = dataReader[1].ToString();
                }

                //назва відділення
                command = new SQLiteCommand("SELECT NameUA, NameEN FROM Department WHERE NameUA = @name", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@name", student.faculty));
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    application.Documents[1].Bookmarks["FacultyNameRus"].Range.Text = dataReader[0].ToString();
                    application.Documents[1].Bookmarks["FacultyNameEn"].Range.Text = dataReader[1].ToString();
                }
                application.Visible = true;
            }
            else if (result == DialogResult.No)
            {
            }
            else if (result == DialogResult.Cancel)
            {
            }
        }

        //перевірка всіх введених даних
        private bool DataCheck(Student student)
        {
            if (SecNameUAText.Text == "")
            {
                MessageBox.Show("Введіть прізвище українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SecNameUAText.Focus();
                return false;
            }

            if (SecNameENText.Text == "")
            {
                MessageBox.Show("Введіть прізвище англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SecNameENText.Focus();
                return false;
            }

            if (FistNameUAText.Text == "")
            {
                MessageBox.Show("Введіть ім'я українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FistNameUAText.Focus();
                return false;
            }

            if (FistNameENText.Text == "")
            {
                MessageBox.Show("Введіть ім'я англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FistNameENText.Focus();
                return false;
            }

            if (PetrNameUAText.Text == "")
            {
                MessageBox.Show("Введіть по батькові українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PetrNameUAText.Focus();
                return false;
            }

            if (PetrNameENText.Text == "")
            {
                MessageBox.Show("Введіть по батькові англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PetrNameENText.Focus();
                return false;
            }

            if (numEdboText.Text == "")
            {
                MessageBox.Show("Введіть номер ЕДБО!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                numEdboText.Focus();
                return false;
            }

            if (comboBoxExpelled.SelectedIndex == -1)
            {
                MessageBox.Show("Оберіть причину відрахуваня!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (idSpecText.SelectedIndex == -1)
            {
                MessageBox.Show("Оберіть спеціальність!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (FacultyText.SelectedIndex == -1)
            {
                MessageBox.Show("Оберіть відділення!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FacultyText.Focus();
                return false;
            }

            if (placeBirthUAText.Text == "")
            {
                MessageBox.Show("Введіть місце народження українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FacultyText.Focus();
                return false;
            }

            if (placeBirthENText.Text == "")
            {
                MessageBox.Show("Введіть місце народження англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FacultyText.Focus();
                return false;
            }

            if(tbOrderNumber.Text == "")
            {
                MessageBox.Show("Введіть номер наказу!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbOrderNumber.Focus();
                return false;
            }

            //заповнення особистих даних студента
            student.secNameUA = SecNameUAText.Text;
            student.secNameEN = SecNameENText.Text;
            student.firstNameUA = FistNameUAText.Text;
            student.firstNameEN = FistNameENText.Text;
            student.petrNameUA = PetrNameUAText.Text;
            student.petrNameEN = PetrNameENText.Text;

            student.dateStart = dateTimePicker1.Value;
            student.dateFinish = dateTimePicker2.Value;
            student.dateBirthday = dateTimePicker3.Value;

            student.numEdbo = numEdboText.Text;
            student.placeBirthUA = placeBirthUAText.Text;
            student.placeBirthEn = placeBirthENText.Text;
            student.specNum = Convert.ToInt32(idSpecText.Text);
            student.faculty = FacultyText.Text;

            student.expelled = comboBoxExpelled.Text;
            student.numexpelled = comboBoxExpelled.SelectedIndex;
            student.course = courseNum.Text;
            student.orderNumber = tbOrderNumber.Text;

            return true;
        }

        //повернення на головну форму
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //відображення попередньої форми
        private void CreateReference_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(frmOpenReference != null)
            {
                this.frmOpenReference.Visible = true;
            }
            else
            {
                this.frmStart.Visible = true;
            }
        }
        
        

        //вивід кодів спецальностей при виборі відділення
        private void FacultyText_SelectedIndexChanged(object sender, EventArgs e)
        {
            idSpecText.Items.Clear();
            command = new SQLiteCommand("SELECT idSpecialty FROM Specialty WHERE idDepartament = (SELECT idDepartment FROM Department WHERE NameUA = @name)", Program.conn);
            command.Parameters.Add(new SQLiteParameter("@name", FacultyText.Items[FacultyText.SelectedIndex]));
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                idSpecText.Items.Add(dataReader[0].ToString());
            }

            try
            {
                idSpecText.SelectedIndex = 0;
            }
            catch{ }
        }

        private void CreateReference_Activated(object sender, EventArgs e)
        {
            //зтираємо шлях до існуючого заповненого шаблону
            templatePath = null;
        }

        //збереження даних
        private void btnSaveExit_Click(object sender, EventArgs e)
        {
            command = new SQLiteCommand(@"INSERT INTO SavedData VALUES (NULL, @SurnameUA, @NameUA, @PetrNameUA,
                                        @SurnameEN, @NameEN, @PetrNameEN, @BirthDay, @PlaceBirthUA, @PlaceBirthEN,
                                        @IndexDepartament, @NumSpeciality, @IndexCourse, @StartTeach, @FinishTeach,
                                        @EDBONumber, @IndexDeduct, @NumberOrder, @DateRecord, @Log)", Program.conn);
            command.Parameters.Add(new SQLiteParameter("@SurnameUA", SecNameUAText.Text));
            command.Parameters.Add(new SQLiteParameter("@NameUA", FistNameUAText.Text));
            command.Parameters.Add(new SQLiteParameter("@PetrNameUA", PetrNameUAText.Text));
            command.Parameters.Add(new SQLiteParameter("@SurnameEN", SecNameENText.Text));
            command.Parameters.Add(new SQLiteParameter("@NameEN", FistNameENText.Text));
            command.Parameters.Add(new SQLiteParameter("@PetrNameEN", PetrNameENText.Text));
            command.Parameters.Add(new SQLiteParameter("@BirthDay", dateTimePicker3.Value));
            command.Parameters.Add(new SQLiteParameter("@PlaceBirthUA", placeBirthUAText.Text));
            command.Parameters.Add(new SQLiteParameter("@PlaceBirthEN", placeBirthENText.Text));
            command.Parameters.Add(new SQLiteParameter("@IndexDepartament", FacultyText.SelectedIndex));
            command.Parameters.Add(new SQLiteParameter("@NumSpeciality", idSpecText.Text));
            command.Parameters.Add(new SQLiteParameter("@IndexCourse", courseNum.SelectedIndex));
            command.Parameters.Add(new SQLiteParameter("@StartTeach", dateTimePicker1.Value));
            command.Parameters.Add(new SQLiteParameter("@FinishTeach", dateTimePicker2.Value));
            command.Parameters.Add(new SQLiteParameter("@EDBONumber", numEdboText.Text));
            command.Parameters.Add(new SQLiteParameter("@IndexDeduct", comboBoxExpelled.SelectedIndex));
            command.Parameters.Add(new SQLiteParameter("@NumberOrder", tbOrderNumber.Text));
            command.Parameters.Add(new SQLiteParameter("@DateRecord", DateTime.Now));
            command.Parameters.Add(new SQLiteParameter("@Log", account.login));

            SQLiteViewInTable.AddDataInBD(command);
            MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        //заповнення полів з бд (збереженна довідка)
        private void OpenSavedReference(int idSave)
        {
            SQLiteCommand command2 = new SQLiteCommand(@"SELECT * FROM SavedData WHERE idSave = @id", Program.conn);
            command2.Parameters.Add(new SQLiteParameter("@id", idSave));
            SQLiteDataReader dataReader2 = command2.ExecuteReader();

            while (dataReader2.Read())
            {
                SecNameUAText.Text = dataReader2[1].ToString();
                FistNameUAText.Text = dataReader2[2].ToString();
                PetrNameUAText.Text = dataReader2[3].ToString();
                SecNameENText.Text = dataReader2[4].ToString();
                FistNameENText.Text = dataReader2[5].ToString();
                PetrNameENText.Text = dataReader2[6].ToString();
                dateTimePicker3.Value = (DateTime)dataReader2[7];
                placeBirthUAText.Text = dataReader2[8].ToString();
                placeBirthENText.Text = dataReader2[9].ToString();

                //запит для заповнення списку
                SQLiteCommand command3 = new SQLiteCommand("SELECT NameUA, NameEN FROM Department", Program.conn);
                SQLiteDataReader dataReader3 = command3.ExecuteReader();
                while (dataReader3.Read())
                {
                    FacultyText.Items.Add(dataReader3[0].ToString());
                }
                FacultyText.SelectedIndex = Convert.ToInt32(dataReader2[10]);

                idSpecText.SelectedIndex = idSpecText.FindString(dataReader2[11].ToString());
                courseNum.SelectedIndex = Convert.ToInt32(dataReader2[12]);
                dateTimePicker1.Value = (DateTime)dataReader2[13];
                dateTimePicker2.Value = (DateTime)dataReader2[14];
                numEdboText.Text = dataReader2[15].ToString();
                comboBoxExpelled.SelectedIndex = Convert.ToInt32(dataReader2[16]);
                tbOrderNumber.Text = dataReader2[17].ToString();
            }
        }

        #region Корректне введення
        //ввод символов (код студ)
        private void idSpecText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                return;
            }
            e.Handled = true;
        }

        //ввод в поля українською мовою
        private void KeyPressTextBoxUA(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'А' || e.KeyChar > 'я') && e.KeyChar != '\b' && e.KeyChar != 'і' && e.KeyChar != Convert.ToChar("'"))
                e.Handled = true;
        }

        //ввод в поля англійською мовою
        private void KeyPressTextBoxEN(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'A' || e.KeyChar > 'z') && e.KeyChar != '\b')
                e.Handled = true;
        }

        //заборона вводу букв
        private void Key(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar) return; // Если символ цифра, то возвращаемся из метода
            e.Handled = true;
            return;
        }
        #endregion

        //зміна номеру поточного семестру
        private void courseNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentSemester = Convert.ToInt32(courseNum.Items[courseNum.SelectedIndex]);
        }
    }
}
