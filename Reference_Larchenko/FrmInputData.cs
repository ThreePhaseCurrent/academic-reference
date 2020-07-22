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
using System.IO;

namespace Reference_Larchenko
{
    public partial class FrmInputData : Form
    {
        //об'єкти для працювання з БД та докуметом
        Word.Application application;
        public FrmMain frmStart = null;
        public Student student = new Student();
        SQLiteCommand command = null;
        SQLiteDataReader dataReader = null;

        //поточний обраний семестр
        public int currentSemester = 1;

        //шлях до існуючого заповненого шаблону
        public static string templatePath = null;
        //попередня форма
        public FrmOpenReference frmOpenReference = null;
        public Account account; //поточний аккаунт

        #region Конструктори
        //звичайний конструктор
        public FrmInputData(FrmMain frmStart, Account account)
        {
            InitializeComponent();
            //отримання посилань
            this.frmStart = frmStart;
            this.account = account;

            //визначення режиму облікового запису
            if (account.rang == "+")
            {
                this.Text += " (Адміністратор)";
            }
            else
            {
                this.Text += " (Користувач)";
            }

            //номер курсу
            courseNum.SelectedIndex = 0;
            //причина відрахування
            comboBoxExpelled.SelectedIndex = 0;
            //запит для заповнення списку
            command = new SQLiteCommand("SELECT NameUA, NameEN FROM Department", Program.conn);
            dataReader = command.ExecuteReader(); //виконання запиту
            while (dataReader.Read()) //перегляд поверненних даних
            {
                //заповнення списку даними
                FacultyText.Items.Add(dataReader[0].ToString());
            }
            try
            {
                FacultyText.SelectedIndex = 0; //виставлення індексу за замовчуванням
            }
            catch { }
            //заборона вводу цифр
            CreateAction();
        }


        public int? selectId = null;
        //конструктор для збереженної довідки
        public FrmInputData(FrmOpenReference frmOpenReference, int selectId, Account account)
        {
            InitializeComponent();
            //отримання посилань
            this.frmOpenReference = frmOpenReference;
            this.account = account;
            this.selectId = selectId;

            //визначення режиму облікового запису
            if (account.rang == "+")
            {
                this.Text += " (Адміністратор)";
            }
            else
            {
                this.Text += " (Користувач)";
            }

            //заповнення полів з бд
            OpenSavedReference(selectId);
            //заборона вводу цифр
            CreateAction();
            //зчитування поточного семестру
            currentSemester = Convert.ToInt32(courseNum.Items[courseNum.SelectedIndex]);
        }

        //підв'язка подій
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
        }
        #endregion

        //завантаження форми
        private void CreateReference_Load(object sender, EventArgs e)
        {
            //підв'язка події для довідки
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка існування потрібних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormInputData.htm"))
            {
                //виклик довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormInputData.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //об'єкт наступного вікна
        FrmSubject createReference2 = null;

        //переход ко второму шагу
        private void NextButton_Click(object sender, EventArgs e)
        {
            //перевірка правильності введення даних
            if (DataCheck(student) == false)
            {
                return; //вихід з функції
            }

            this.Visible = false; //зкриття поточного вікна
            //відкриття наступного вікна
            createReference2 = new FrmSubject(this, Convert.ToInt32(courseNum.Text));
            createReference2.ShowDialog();
        }

        //попередній перегляд документу
        private void btnPreview_Click(object sender, EventArgs e)
        {
            //перевірка існування шаблону довідки
            if (!File.Exists(System.IO.Path.GetFullPath(@"template.docx")))
            {
                //повідомлення про помилку
                MessageBox.Show("Файл шаблону не знайдений!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;//завершення функції
            }
            //перевірка правильності введення даних
            if (DataCheck(student) == false)
            {
                return; //вихід з функції
            }

            //підтвердження огляду документа від користувача
            DialogResult result = MessageBox.Show("Перейти до попередьного огляду документа?", "Організація попереднього огляду", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) //якщо так
            {
                //створюємо об'єкт додатку
                application = new Word.Application();
                // створюємо шлях до файлу
                Object templatePathObj = System.IO.Path.GetFullPath(@"template.docx");
                //відкриття документу в режимі читання
                application.Documents.Open(templatePathObj, ReadOnly: true);

                //заповнення номеру довідки
                application.Documents[1].Tables[2].Cell(1, 1).Range.Font.Size = 24;
                application.Documents[1].Tables[2].Cell(1, 1).Range.Bold = 0;
                application.Documents[1].Bookmarks["numberreference"].Range.Text = "№ " + student.orderNumber;

                //ім'я
                application.Documents[1].Bookmarks["Name"].Range.Text = student.secNameUA + " " + student.firstNameUA + " " + student.petrNameUA;
                application.Documents[1].Bookmarks["Name2"].Range.Text = student.secNameEN + " " + student.firstNameEN + " " + student.petrNameEN; ;

                //день початку (українське поле)
                application.Documents[1].Bookmarks["day1_1"].Range.Text = (student.dateStart.Day < 10) ? "0" + student.dateStart.Day.ToString() :
                                                                            student.dateStart.Day.ToString(); ;
                //місяць і рік початку (українське поле)
                application.Documents[1].Bookmarks["lastday1_1"].Range.Text = (student.dateStart.Month < 10) ? "0" + student.dateStart.Month.ToString() + "." + student.dateStart.Year.ToString() :
                                                                               student.dateStart.Month.ToString() + "." + student.dateStart.Year.ToString();
                //день завершення (українське поле)
                application.Documents[1].Bookmarks["day1_2"].Range.Text = (student.dateFinish.Day < 10) ? "0" + student.dateFinish.Day.ToString() :
                                                                           student.dateFinish.Day.ToString();
                //місяць і рік завершення (українське поле)
                application.Documents[1].Bookmarks["lastday1_2"].Range.Text = (student.dateFinish.Month < 10) ? "0" + student.dateFinish.Month.ToString() + "." + student.dateFinish.Year.ToString() :
                                                                               student.dateFinish.Month.ToString() + "." + student.dateFinish.Year.ToString();
                //день початку (англійське поле)
                application.Documents[1].Bookmarks["day2_1"].Range.Text = (student.dateStart.Day < 10) ? "0" + student.dateStart.Day.ToString() :
                                                                           student.dateStart.Day.ToString();
                //місяць і рік початку (англійське поле)
                application.Documents[1].Bookmarks["lastday2_1"].Range.Text = (student.dateStart.Month < 10) ? "0" + student.dateStart.Month.ToString() + "." + student.dateStart.Year.ToString() :
                                                                               student.dateStart.Month.ToString() + "." + student.dateStart.Year.ToString();
                //день завершення (англійське поле)
                application.Documents[1].Bookmarks["day2_2"].Range.Text = (student.dateFinish.Day < 10) ? "0" + student.dateFinish.Day.ToString() : student.dateFinish.Day.ToString();
                //місяць і рік початку (англійське поле)
                application.Documents[1].Bookmarks["lastday2_2"].Range.Text = (student.dateFinish.Month < 10) ? "0" + student.dateFinish.Month.ToString() + "." + student.dateFinish.Year.ToString() :
                                                                                student.dateFinish.Month.ToString() + "." + student.dateFinish.Year.ToString();

                //день народження українське поле
                application.Documents[1].Bookmarks["Birth1_1Rus"].Range.Text = (student.dateBirthday.Day < 10) ? "0" + student.dateBirthday.Day.ToString() :
                                                                                student.dateBirthday.Day.ToString();
                //місяць і рік народження українське поле
                application.Documents[1].Bookmarks["Birth1_2Rus"].Range.Text = (student.dateBirthday.Month < 10) ? "0" + student.dateBirthday.Month.ToString() + "." + student.dateBirthday.Year.ToString() :
                                                                                student.dateBirthday.Month.ToString() + "." + student.dateBirthday.Year.ToString();
                //день народження англійське поле
                application.Documents[1].Bookmarks["Birth2_1En"].Range.Text = (student.dateBirthday.Day < 10) ? "0" + student.dateBirthday.Day.ToString() :
                                                                               student.dateBirthday.Day.ToString();
                //місяць і рік народження англійське поле
                application.Documents[1].Bookmarks["Birth2_2En"].Range.Text = (student.dateBirthday.Month < 10) ? "0" + student.dateBirthday.Month.ToString() + "." + student.dateBirthday.Year.ToString() :
                                                                               student.dateBirthday.Month.ToString() + "." + student.dateBirthday.Year.ToString();

                //місце народження
                application.Documents[1].Bookmarks["CountryRus"].Range.Text = student.placeBirthUA;
                application.Documents[1].Bookmarks["CountryEn"].Range.Text = student.placeBirthEn;

                //причина відрахування
                application.Documents[1].Bookmarks["ExpelledUa"].Range.Text = student.expelled;
                application.Documents[1].Bookmarks["ExpelledEn"].Range.Text = student.expelledEN[student.numexpelled];

                //запит на назву спеціальності за кодом
                command = new SQLiteCommand("SELECT NameUA, NameEN FROM Specialty WHERE idSpecialty = @id", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@id", student.specNum.ToString()));
                dataReader = command.ExecuteReader(); //виконання команди

                //назва навчального закладу
                while (dataReader.Read())
                {
                    //номер спеціальності
                    application.Documents[1].Bookmarks["CodeRus"].Range.Text = student.specNum.ToString() + " " + dataReader[0].ToString();
                    application.Documents[1].Bookmarks["CodeEn"].Range.Text = student.specNum.ToString() + " " + dataReader[1].ToString();
                }
                dataReader.Close(); //закриття

                //спеціалізація
                command = new SQLiteCommand("SELECT name_ua_spec, name_en_spec FROM specialization WHERE name_ua_spec = @name", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@name", student.specialization));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    //номер спеціалізації
                    application.Documents[1].Bookmarks["StudyProgramRus"].Range.Text = dataReader[0].ToString();
                    application.Documents[1].Bookmarks["StudyProgramEn"].Range.Text = dataReader[1].ToString();
                }

                //назва навчального закладу
                command = new SQLiteCommand("SELECT NameUA, NameEN FROM College", Program.conn);
                dataReader = command.ExecuteReader(); //виконання запиту
                while (dataReader.Read())
                {
                    application.Documents[1].Bookmarks["InstNameRus"].Range.Text = dataReader[0].ToString();
                    application.Documents[1].Bookmarks["InstNameEn"].Range.Text = dataReader[1].ToString();
                }

                //назва відділення
                command = new SQLiteCommand("SELECT NameUA, NameEN FROM Department WHERE NameUA = @name", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@name", student.faculty));
                dataReader = command.ExecuteReader(); //виконання запиту
                while (dataReader.Read())
                {
                    //вставлення назви відділення
                    application.Documents[1].Bookmarks["FacultyNameRus"].Range.Text = dataReader[0].ToString();
                    application.Documents[1].Bookmarks["FacultyNameEn"].Range.Text = dataReader[1].ToString();
                }
                //номер наказу
                application.Documents[1].Bookmarks["OrderUa"].Range.Text = tbOrderNumber.Text;
                application.Documents[1].Bookmarks["OrderEn"].Range.Text = tbOrderNumber.Text;
                application.Visible = true;
            }
            else if (result == DialogResult.No) //якщо ні
            {
            }
            else if (result == DialogResult.Cancel) // якщо скасування
            {
            }
        }

        //перевірка всіх введених даних
        private bool DataCheck(Student student)
        {
            //перевірка введення призвища (укр)
            if (SecNameUAText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть прізвище українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SecNameUAText.Focus(); //встановлення фокусу на поле
                return false; //повернення false
            }

            //перевірка введення призвища (англ)
            if (SecNameENText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть прізвище англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SecNameENText.Focus(); //встановлення фокусу на поле
                return false; //повернення false
            }

            //перевірка введення ім'я (укр)
            if (FistNameUAText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть ім'я українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FistNameUAText.Focus(); //встановлення фокусу на поле
                return false; //повернення false
            }

            //перевірка введення ім'я (англ)
            if (FistNameENText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть ім'я англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FistNameENText.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //перевірка введення по батькові (укр)
            if (PetrNameUAText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть по батькові українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PetrNameUAText.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //перевірка введення по батькові (англ)
            if (PetrNameENText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть по батькові англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PetrNameENText.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //обрання причини відрахування
            if (comboBoxExpelled.SelectedIndex == -1)
            {
                //повідомлення про помилку
                MessageBox.Show("Оберіть причину відрахуваня!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;//повернення false
            }

            //обрання спеціальнсоті
            if (idSpecText.SelectedIndex == -1)
            {
                //повідомлення про помилку
                MessageBox.Show("Оберіть спеціальність!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;//повернення false
            }

            //обрання відділення
            if (FacultyText.SelectedIndex == -1)
            {
                //повідомлення про помилку
                MessageBox.Show("Оберіть відділення!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FacultyText.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //обрання спеціалізації
            if (cbSpecialization.SelectedIndex == -1)
            {
                //повідомлення про помилку
                MessageBox.Show("Оберіть спеціалізацію!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbSpecialization.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //місце народження (укр)
            if (placeBirthUAText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть місце народження українською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FacultyText.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //місце народження (англ)
            if (placeBirthENText.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть місце народження англійською!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FacultyText.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //номер наказу
            if (tbOrderNumber.Text == "")
            {
                //повідомлення про помилку
                MessageBox.Show("Введіть номер наказу!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbOrderNumber.Focus(); //встановлення фокусу на поле
                return false;//повернення false
            }

            //заповнення особистих даних студента
            student.secNameUA = SecNameUAText.Text;
            student.secNameEN = SecNameENText.Text;
            student.firstNameUA = FistNameUAText.Text;
            student.firstNameEN = FistNameENText.Text;
            student.petrNameUA = PetrNameUAText.Text;
            student.petrNameEN = PetrNameENText.Text;
            //дати навчання
            student.dateStart = dateTimePicker1.Value;
            student.dateFinish = dateTimePicker2.Value;
            //дата народження
            student.dateBirthday = dateTimePicker3.Value;
            //місце народження
            student.placeBirthUA = placeBirthUAText.Text;
            student.placeBirthEn = placeBirthENText.Text;
            //номер спеціальності
            student.specNum = idSpecText.Text;
            student.faculty = FacultyText.Text; // відділення
            student.specialization = cbSpecialization.Text; //спеціалізація
            //причина відрахування
            student.expelled = comboBoxExpelled.Text;
            student.numexpelled = comboBoxExpelled.SelectedIndex;
            student.course = courseNum.Text; // номер курсу
            student.orderNumber = tbOrderNumber.Text; //номер наказу

            return true; //повернення true
        }

        //повернення на головну форму
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття вікна
        }

        //відображення попередньої форми
        private void CreateReference_FormClosing(object sender, FormClosingEventArgs e)
        {
            //знищення об'єктів для роботи з БД
            dataReader.Close();
            command = null;
            //перевірка до якого вікна повертатися
            if (frmOpenReference != null)
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
            idSpecText.Items.Clear(); //очищення списку
            //створення зпиту
            command = new SQLiteCommand("SELECT idSpecialty FROM Specialty WHERE idDepartament = (SELECT idDepartment FROM Department WHERE NameUA = @name)", Program.conn);
            //додавання параметру
            command.Parameters.Add(new SQLiteParameter("@name", FacultyText.Items[FacultyText.SelectedIndex]));
            dataReader = command.ExecuteReader(); //виконання запиту
            while (dataReader.Read())
            {
                //заповнення списку даними
                idSpecText.Items.Add(dataReader[0].ToString());
            }

            try
            {
                //обирання індексу за замовчуванням
                idSpecText.SelectedIndex = 0;
            }
            catch {
                //обчищення списків при помилці
                idSpecText.Items.Clear();
                cbSpecialization.Items.Clear();
            }
        }

        //активація вікна
        private void CreateReference_Activated(object sender, EventArgs e)
        {
            //зтираємо шлях до існуючого заповненого шаблону
            templatePath = null;
        }

        //збереження даних
        private void btnSaveExit_Click(object sender, EventArgs e)
        {
            //створення запиту
            SQLiteCommand command2 = new SQLiteCommand("SELECT id_specialization FROM specialization WHERE name_ua_spec = @name", Program.conn);
            //додавання параметрів
            command2.Parameters.Add(new SQLiteParameter("@name", cbSpecialization.Text));
            SQLiteDataReader dataReader2 = command2.ExecuteReader(); // виконання запиту

            string id_specialization = string.Empty; //код спеціалізації
            while (dataReader2.Read())
            {
                id_specialization = dataReader2[0].ToString();
            }
            //створення запиту на збереження даних
            command = new SQLiteCommand(@"INSERT INTO SavedData VALUES (NULL, @SurnameUA, @NameUA, @PetrNameUA,
                                        @SurnameEN, @NameEN, @PetrNameEN, @BirthDay, @PlaceBirthUA, @PlaceBirthEN,
                                        @IndexDepartament, @NumSpeciality, @Specialization, @IndexCourse, @StartTeach, @FinishTeach,
                                        @IndexDeduct, @NumberOrder, @DateRecord, @Log)", Program.conn);
            //додавання параметрів
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
            command.Parameters.Add(new SQLiteParameter("@Specialization", id_specialization));
            command.Parameters.Add(new SQLiteParameter("@IndexCourse", courseNum.SelectedIndex));
            command.Parameters.Add(new SQLiteParameter("@StartTeach", dateTimePicker1.Value));
            command.Parameters.Add(new SQLiteParameter("@FinishTeach", dateTimePicker2.Value));
            command.Parameters.Add(new SQLiteParameter("@IndexDeduct", comboBoxExpelled.SelectedIndex));
            command.Parameters.Add(new SQLiteParameter("@NumberOrder", tbOrderNumber.Text));
            command.Parameters.Add(new SQLiteParameter("@DateRecord", DateTime.Now));
            command.Parameters.Add(new SQLiteParameter("@Log", account.login));
            //виконання запиту
            SQLiteViewInTable.AddDataInBD(command);
            //повідомлення про виконання запиту
            MessageBox.Show("Дані збережено!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dataReader2.Close(); //закриття об'єкту БД
            this.Close();//закриття вікна
        }

        //заповнення полів з бд (збереженна довідка)
        private void OpenSavedReference(int idSave)
        {
            //створення запиту
            SQLiteCommand command2 = new SQLiteCommand(@"SELECT * FROM SavedData WHERE idSave = @id", Program.conn);
            //додавання параметру
            command2.Parameters.Add(new SQLiteParameter("@id", idSave));
            //виконання запиту
            SQLiteDataReader dataReader2 = command2.ExecuteReader(); 

            while (dataReader2.Read())
            {
                //вставка даних в відповідні поля для них
                SecNameUAText.Text = dataReader2[1].ToString();
                FistNameUAText.Text = dataReader2[2].ToString();
                PetrNameUAText.Text = dataReader2[3].ToString();
                SecNameENText.Text = dataReader2[4].ToString();
                FistNameENText.Text = dataReader2[5].ToString();
                PetrNameENText.Text = dataReader2[6].ToString();
                dateTimePicker3.Value = (DateTime)dataReader2[7];
                placeBirthUAText.Text = dataReader2[8].ToString();
                placeBirthENText.Text = dataReader2[9].ToString();

                //запит для заповнення списку з відділеннями
                SQLiteCommand command3 = new SQLiteCommand("SELECT NameUA, NameEN FROM Department", Program.conn);
                //виконання запиту
                SQLiteDataReader dataReader3 = command3.ExecuteReader();
                while (dataReader3.Read())
                {
                    FacultyText.Items.Add(dataReader3[0].ToString()); // заповнення списку
                }
                FacultyText.SelectedIndex = Convert.ToInt32(dataReader2[10]); // обирання за індексом
                //обирання за індексом спеціальність
                idSpecText.SelectedIndex = idSpecText.FindString(dataReader2[11].ToString());

                //запит на отримання назв спеціалізацій
                command3 = new SQLiteCommand("SELECT name_ua_spec FROM specialization WHERE id_specialization = @name", Program.conn);
                command3.Parameters.Add(new SQLiteParameter("@name", dataReader2));
                dataReader3 = command3.ExecuteReader(); // виконання запиту
                while (dataReader3.Read())
                {
                    //обирання потрібної спеціалізації
                    try
                    {
                        cbSpecialization.SelectedIndex = idSpecText.FindString(dataReader3[0].ToString());
                    }
                    catch { }
                }
                courseNum.SelectedIndex = Convert.ToInt32(dataReader2[13]);
                dateTimePicker1.Value = (DateTime)dataReader2[14];
                dateTimePicker2.Value = (DateTime)dataReader2[15];
                comboBoxExpelled.SelectedIndex = Convert.ToInt32(dataReader2[16]);
                tbOrderNumber.Text = dataReader2[17].ToString();
                dataReader3.Close();//закриття об'єкту БД
            }
            dataReader2.Close();//закриття об'єкту БД
        }

        #region Корректне введення
        //ввод символів (код студента)
        private void idSpecText_KeyPress(object sender, KeyPressEventArgs e)
        {
            //якщо число або зтирання
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                return; // пропускаємо
            }
            e.Handled = true; //блокуємо
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
            //якщо не англійські букви
            if ((e.KeyChar < 'A' || e.KeyChar > 'z') && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Space)
                e.Handled = true; //блокуємо
        }

        //заборона вводу букв
        private void Key(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true || (char)Keys.Back == e.KeyChar) return; // Якщо символ число, то повертаємося из функції
            e.Handled = true;
            return;
        }
        #endregion

        //зміна номеру поточного семестру
        private void courseNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            //щміна поточного семестру
            currentSemester = Convert.ToInt32(courseNum.Items[courseNum.SelectedIndex]);
        }

        //заповнення списку спеціалізацій при виборі спеціальності
        private void idSpecText_SelectedIndexChanged(object sender, EventArgs e)
        {
            //якщо не обрана спеціальінсть то повертаємося
            if (idSpecText.SelectedIndex == -1) { return; }
            cbSpecialization.Items.Clear(); //очищаємо список
            //формування запиту
            SQLiteCommand command2 = new SQLiteCommand("SELECT name_ua_spec FROM specialization WHERE id_specialty = @id_spec", Program.conn);
            //додавання параметрів
            command2.Parameters.Add(new SQLiteParameter("@id_spec", idSpecText.Text));
            SQLiteDataReader dataReader2 = command2.ExecuteReader();

            if (dataReader2.Read())
            {
                //заповнення списку спеціалізацій
                cbSpecialization.Items.Add(dataReader2[0].ToString());
            }

            try
            {
                //обирання спеціальності за замовчуванням 
                cbSpecialization.SelectedIndex = 0;
            }
            catch {
                //очищення списку при помилці
                cbSpecialization.Items.Clear();
            }
            dataReader.Close(); //закриття об'єкту БД
        }
    }
}
