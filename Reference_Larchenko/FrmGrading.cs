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
using System.IO;

namespace Reference_Larchenko
{
    public partial class FrmGrading : Form
    {
        //winword
        Microsoft.Office.Interop.Word.Application application;
        //об'єкти для працювання з БД та документом
        SQLiteCommand command = null;
        SQLiteDataReader dataReader = null;

        //об'єкт студента
        Student student = null;
        //загальний список дисциплін
        List<CheckedObj> allCheckedList = new List<CheckedObj>();
        FrmSubject createReference2 = null; //об'єкт попроеднього вікна
        //таблиці для данних
        System.Data.DataTable table = null;
        System.Data.DataTable ds = null;

        //масиви з обраними предметами по семестрово
        public List<CheckedObj> liFisrt = new List<CheckedObj>();
        public List<CheckedObj> liSecond = new List<CheckedObj>();
        public List<CheckedObj> liThree = new List<CheckedObj>();
        public List<CheckedObj> liFour = new List<CheckedObj>();
        public List<CheckedObj> liFive = new List<CheckedObj>();
        public List<CheckedObj> liSix = new List<CheckedObj>();
        public List<CheckedObj> liSeven = new List<CheckedObj>();
        public List<CheckedObj> liEight = new List<CheckedObj>();

        //сортування обраних предметів по семестрах
        private void SortSemester(List<CheckedObj> list)
        {
            //цикл
            foreach(var l in list)
            { //сортування
                if(l.SemesterNum == 1) { liFisrt.Add(l); }
                if (l.SemesterNum == 2) { liSecond.Add(l); }
                if (l.SemesterNum == 3) { liThree.Add(l); }
                if (l.SemesterNum == 4) { liFour.Add(l); }
                if (l.SemesterNum == 5) { liFive.Add(l); }
                if (l.SemesterNum == 6) { liSix.Add(l); }
                if (l.SemesterNum == 7) { liSeven.Add(l); }
                if (l.SemesterNum == 8) { liEight.Add(l); }
            }
        }

        //конструктор
        public FrmGrading(FrmSubject createReference2, List<CheckedObj> allCheckedList, Student student)
        {
            InitializeComponent();
            //отримання посилань
            this.allCheckedList = allCheckedList;
            this.createReference2 = createReference2;
            this.student = student;
            //підпис рангу користувача в шапці вікна
            if(createReference2.createReference.account.rang == "+")
            {
                this.Text += " (Адміністратор)";
            }
            else
            {
                this.Text += " (Користувач)";
            }

            //вибір системи числення
            comboBox1.SelectedIndex = 0;
            //створення та заповнення таблиць з дисциплінами
            ds = new System.Data.DataTable();
            ds = ConvertListToDataTable(allCheckedList);
            //створеня джерела даних
            bindingSource1.DataSource = ds;
            //підв'язка до джерела
            dataGridView1.DataSource = bindingSource1;
            bindingNavigator1.BindingSource = bindingSource1;
            //підписи столбців
            dataGridView1.Columns[0].HeaderText = "Навчальні дисципліни";
            dataGridView1.Columns[1].HeaderText = "Семестр";
            dataGridView1.Columns[2].HeaderText = "Загальний обсяг";
            dataGridView1.Columns[3].HeaderText = "Тип оцінки";
            dataGridView1.Columns[4].HeaderText = "Оцінка";

            dataGridView1.Columns[0].Width = 140; //ширина першого стовпця
            //причеплення полів до курсору джерела даних
            richTextBox1.DataBindings.Add(new Binding("Text", bindingSource1, "Column1"));
            cmbxType.SelectedIndex = cmbxType.Items.IndexOf(dataGridView1.Rows[0].Cells[3].Value);
            tbOverSize.DataBindings.Add(new Binding("Text", bindingSource1, "Column3"));

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
            //перевірка існування необхідних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormGrading.htm"))
            {
                //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormGrading.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //конвертація коллекції в таблицю
        private System.Data.DataTable ConvertListToDataTable(List<CheckedObj> list)
        {
            //нова таблиця
            table = new System.Data.DataTable();
            int coll = 5;

            //додавання колонок
            for(int i = 0; i < coll; i++)
            {
                table.Columns.Add();
            }

            //додавання рядків
            foreach(var arr in list)
            {
                //достаємо з БД загальний обсяг предмету
                command = new SQLiteCommand("SELECT OverSize FROM Subject WHERE id_specialization = (SELECT id_specialization FROM specialization WHERE name_ua_spec = @nameSpec) AND NameUA = @name AND SemesterNum = @semester", Program.conn);
                command.Parameters.Add(new SQLiteParameter("@nameSpec", student.specialization));
                command.Parameters.Add(new SQLiteParameter("@name", arr.Name));
                command.Parameters.Add(new SQLiteParameter("@semester", arr.SemesterNum));
                dataReader = command.ExecuteReader();
                //заповнення рядка
                while (dataReader.Read())
                {
                    //додавання в рядок
                    table.Rows.Add(arr.Name, arr.SemesterNum, dataReader[0], arr.Type, arr.Value);
                }
            }
            return table;
        }

        //змінення системи оцінювання
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQLiteCommand command = null; //об'єкт для команди до БД
            if (comboBox1.SelectedIndex == 0) //якщо п'ятибальна система
            { //виконуємо команду до таблиці з оцінками 5тибальної системи
                command = new SQLiteCommand("SELECT Value FROM ValueFive", Program.conn);
                numericUpDown1.Maximum = 5; //вмикання обмеження перемикача
            }
            else
            { //виконуємо команду до таблиці з оцінками 12тибальної системи
                command = new SQLiteCommand("SELECT Value FROM ValueTwelve", Program.conn);
                numericUpDown1.Maximum = 12; //вмикання обмеження перемикача
            }
            SQLiteDataReader dataReader = command.ExecuteReader(); //виконання запиту
            dataReader.Close(); //закриття об'єкту БД
        }

        //повернення на попередню форму
        private void btnBack_Click(object sender, EventArgs e)
        {
            createReference2.Visible = true; //відображення попереднього вікна
            //закриття та знищення ресурсів
            this.Close();
            this.Dispose();
        }

        private void btnCreateRef_Click(object sender, EventArgs e)
        {
            //перевірка існування шаблону довідки
            if (!File.Exists(System.IO.Path.GetFullPath(@"template.docx")))
            {
                //повідомлення про помилку
                MessageBox.Show("Файл шаблону не знайдений!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;//завершення функції
            }

            //перевірка, що всі оцінки виставлені
            if (CheckInputPoints(dataGridView1) == false)
            {
                return;
            }

            DialogResult result = MessageBox.Show("Створити довідку?", "Підтвердження створення довідки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //збереження файлу
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Microsoft Word(*.docx)|*.docx"; //встановлення фільтру типу файлів
                saveFileDialog.Title = "Сохранение документа"; //заголовок вікна
                //назва файлу за замовчуванням
                saveFileDialog.FileName = "Академічна довідка_" + student.secNameUA + " " + student.firstNameUA[0] + "." + student.petrNameUA[0] + ".";
                string initPath = @"Created"; // почтаковий шлях
                saveFileDialog.InitialDirectory = Path.GetFullPath(initPath);
                saveFileDialog.RestoreDirectory = true;

                DialogResult dialogResult;
                try
                {
                    //отримання шляху від користувача
                    dialogResult = saveFileDialog.ShowDialog();

                    if(dialogResult == DialogResult.Cancel) { return; }
                }
                catch { return; }

                //сортування предметів посемемтрово
                SortSemester(allCheckedList);

                //створюємо об'єкт додатку ворд
                application = new Microsoft.Office.Interop.Word.Application();
                Object templatePathObj = null;
                if (FrmInputData.templatePath == null)
                {
                    // створюємо шлях файлу
                    templatePathObj = System.IO.Path.GetFullPath(@"template.docx");
                }
                else
                {
                    //шлях до заповненного шаблону
                    templatePathObj = FrmInputData.templatePath;
                }
                //відкриваємо файл
                application.Documents.Open(templatePathObj);

                if (FrmInputData.templatePath == null)
                {
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
                    //додаваення параметрів
                    command.Parameters.Add(new SQLiteParameter("@id", student.specNum.ToString()));
                    dataReader = command.ExecuteReader(); //виконання команди

                    while (dataReader.Read())
                    {
                        //номер спеціальності
                        application.Documents[1].Bookmarks["CodeRus"].Range.Text = student.specNum.ToString() + " " + dataReader[0].ToString();
                        application.Documents[1].Bookmarks["CodeEn"].Range.Text = student.specNum.ToString() + " " + dataReader[1].ToString();
                    }
                    dataReader.Close();//закриття об'єкту БД

                    //спеціалізація
                    command = new SQLiteCommand("SELECT name_ua_spec, name_en_spec FROM specialization WHERE name_ua_spec = @name", Program.conn);
                    //додаваення параметрів
                    command.Parameters.Add(new SQLiteParameter("@name", student.specialization));
                    dataReader = command.ExecuteReader();//виконання команди

                    while (dataReader.Read())
                    {
                        //номер спеціалізації
                        application.Documents[1].Bookmarks["StudyProgramRus"].Range.Text = dataReader[0].ToString();
                        application.Documents[1].Bookmarks["StudyProgramEn"].Range.Text = dataReader[1].ToString();
                    }

                    //назва навчального закладу
                    command = new SQLiteCommand("SELECT NameUA, NameEN FROM College", Program.conn);
                    dataReader = command.ExecuteReader();//виконання команди
                    while (dataReader.Read())
                    {
                        //вставка назви закладу
                        application.Documents[1].Bookmarks["InstNameRus"].Range.Text = dataReader[0].ToString();
                        application.Documents[1].Bookmarks["InstNameEn"].Range.Text = dataReader[1].ToString();
                    }

                    //назва відділення
                    command = new SQLiteCommand("SELECT NameUA, NameEN FROM Department WHERE NameUA = @name", Program.conn);
                    command.Parameters.Add(new SQLiteParameter("@name", student.faculty));
                    dataReader = command.ExecuteReader();//виконання команди
                    while (dataReader.Read())
                    {
                        //вставка назви відділення
                        application.Documents[1].Bookmarks["FacultyNameRus"].Range.Text = dataReader[0].ToString();
                        application.Documents[1].Bookmarks["FacultyNameEn"].Range.Text = dataReader[1].ToString();
                    }
                }
                
                //заповнення таблиці з успішностю
                if(liFisrt.Count != 0) { GradingBall(liFisrt); }
                if (liSecond.Count != 0) { GradingBall(liSecond); }
                if (liThree.Count != 0) { GradingBall(liThree); }
                if (liFour.Count != 0) { GradingBall(liFour); }
                if (liFive.Count != 0) { GradingBall(liFive); }
                if (liSix.Count != 0) { GradingBall(liSix); }
                if (liSeven.Count != 0) { GradingBall(liSeven); }
                if (liEight.Count != 0) { GradingBall(liEight); }
                //вставка в поле номеру наказу
                application.Documents[1].Bookmarks["OrderUA"].Range.Text = "№ " + student.orderNumber;
                application.Documents[1].Bookmarks["OrderEN"].Range.Text = "№ " + student.orderNumber;

                try
                {   
                    if (dialogResult == DialogResult.OK) //якщо настиснуто ОК
                    {
                        //збереження файлу за шляхом користувача
                        application.ActiveDocument.SaveAs2(saveFileDialog.FileName);
                        //закриття додатку ворд
                        application.ActiveDocument.Close();
                        application.Quit();
                        //створюємо об'єкт додатку ворд
                        application = new Word.Application();
                        templatePathObj = saveFileDialog.FileName;
                        //відкриваємо файл
                        application.Documents.Open(templatePathObj);
                        //повідомлення про успішність операції
                        MessageBox.Show("Академічна довідка успішно створена!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        application.Visible = true; //відображення документу

                        //якшо довідка була раніше збережена -- видаляємо
                        if (createReference2.createReference.frmOpenReference != null && createReference2.createReference.selectId != null)
                        {
                            //запит на видалення збережнної довідки, що створена
                            SQLiteCommand command2 = new SQLiteCommand("DELETE FROM SavedData WHERE idSave = @idSave", Program.conn);
                            command2.Parameters.Add(new SQLiteParameter("@idSave", createReference2.createReference.selectId));
                            //виконання
                            command2.ExecuteNonQuery();
                            command2.Dispose();
                        }

                        //повернення на стартову сторінку
                        try
                        {
                            createReference2.createReference.frmStart.Visible = true;
                        }
                        catch
                        {
                            createReference2.createReference.frmOpenReference.frmStart.Visible = true;
                        }
                        //закриття всіх вікон
                        createReference2.createReference.Close();
                        createReference2.Close();
                        this.Close();
                        //збір сміття
                        GC.Collect();
                    }
                    else if(dialogResult == DialogResult.Cancel) //якщо скасовано
                    {
                        //закриття додатку ворд
                        application.ActiveDocument.Close();
                        application.Quit();
                    }
                }
                catch { }
            }
            else if (result == DialogResult.No) //якщо ні
            {
                return;
            }
            else if (result == DialogResult.Cancel) { return; }//якщо скасовано
        }

        //заповнення таблиці
        bool firstTable = true;
        //оцінювання
        private void GradingBall(List<CheckedObj> list)
        {
            Word.Table table = application.Documents[1].Tables[12]; //об'єкт таблиці для заповнення

            if(firstTable == false) //якщо це перша таблиця
            { //додавання два рядка
                table.Rows.Add();
                table.Rows.Add();
            }
            firstTable = false; //змінюємо прапорець
            //відображення поточного семестру у шапку таблиці
            table.Cell(table.Rows.Count , 1).Range.Text = list[0].Course + " курс " + list[0].SemesterNum + " семестр" + " / " + list[0].Course + " course " + list[0].SemesterNum + " semester";
            //встановлення стилю тексту
            table.Cell(table.Rows.Count, 1).Range.Bold = 1;
            table.Cell(table.Rows.Count, 1).Range.Font.Size = 14;
            table.Rows.Add(); //додавання рядка

            int count = 1; // лічільник семестр
            for (int i = 0; i < list.Count; i++) //загальний цикл для виставлення оцінок
            {
                table.Rows.Add(); //додавання рядка
                //змінення стилю тексту 
                table.Cell(table.Rows.Count, 1).Range.Bold = 0;
                table.Cell(table.Rows.Count, 1).Range.Font.Size = 11;
                //якщо тип оцінки ЗНО
                if (list[i].Type == "ЗНО")
                {
                    table.Cell(table.Rows.Count, 1).Range.Text = count + "." + list[i].Name + "\n" + list[i].NameEN + "(ЗНО/IEE)";                    
                }
                else if (list[i].Type == "ДПА")//якщо тип оцінки ДПА
                {
                    table.Cell(table.Rows.Count, 1).Range.Text = count + "." + list[i].Name + "\n" + list[i].NameEN;
                }
                else //всі інщі типи
                {
                    table.Cell(table.Rows.Count, 1).Range.Text = count + "." + list[i].Name + "\n" + list[i].NameEN + " - " + list[i].OverSize + " г. / h.";
                }
                
                //якщо система числення <= 5
                if (list[i].Value <= 5)
                {
                    //створення запиту
                    command = new SQLiteCommand("SELECT ValueText, ValueTextEN FROM ValueFive WHERE Value = @value", Program.conn);
                }
                else //в інших випадках
                {
                    //створення запиту
                    command = new SQLiteCommand("SELECT ValueText, ValueTextEN FROM ValueTwelve WHERE Value = @value", Program.conn);
                }
                //додавання параметрів
                command.Parameters.Add(new SQLiteParameter("@value", list[i].Value.ToString()));
                dataReader = command.ExecuteReader(); //виконання команди
                while (dataReader.Read())
                {
                    if (list[i].Type == "ДПА") //якщо тип оцінки ДПА
                    {
                        table.Rows[table.Rows.Count].Cells[2].Range.Text = list[i].Value.ToString() + "(" + dataReader[0].ToString() + "/" + dataReader[1].ToString() + ") - Державний іспит/State Exam";
                    }
                    else if(list[i].Type == "ЗНО") //якщо ЗНО
                    {
                        table.Rows[table.Rows.Count].Cells[2].Range.Text = list[i].Value.ToString() + "(" + dataReader[0].ToString() + "/" + dataReader[1].ToString() + ") - Іспит/Exam";
                    }
                    else //в інших випадках
                    {
                        table.Rows[table.Rows.Count].Cells[2].Range.Text = list[i].Value.ToString() + "(" + dataReader[0].ToString() + "/" + dataReader[1].ToString() + ") - " + list[i].Type + "/Exam";
                    }
                }
                count++; //наступний семестр
            }
        }

        //перевірка, що всі оцінки виставлені
        private bool CheckInputPoints(DataGridView dataGridView)
        {
            for(int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if(dataGridView[4, i].Value.ToString() == "0") //якщо не виставлена оцінка
                {
                    //повідомлення про помилку
                    MessageBox.Show("Введені не всі оцінки!\n\nПредмет: "+ dataGridView[0, i].Value.ToString() + "\nСеместер: "+ dataGridView[1, i].Value.ToString(), 
                                    "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //видалення будь-якого виділення в таблиці
                    dataGridView.ClearSelection();
                    //виділення потрібного рядка
                    dataGridView.Rows[i].Cells[4].Selected = true;
                    //виставлення поточної комірки
                    dataGridView.CurrentCell = dataGridView.Rows[i].Cells[4];
                    return false;
                }
            }
            return true;
        }

        //запис оцінки при зміненні значення перимикача
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //вписування оцінки в таблицю
                dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[4].Value = numericUpDown1.Value;
                //вписування оцінки в масив
                allCheckedList[dataGridView1.CurrentRow.Index].Value = Convert.ToInt32(numericUpDown1.Value);
            }
            catch(Exception ex)
            {
            }
            try
            {
                //вписування оцінки в масив
                allCheckedList[dataGridView1.CurrentRow.Index].Value = Convert.ToInt32(numericUpDown1.Value);
            }
            catch { }
        }

        //скидання значення оцінки при перемиканні на інший предмет
        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
            try
            {
                //вписування значення перемикача з таблиці
                numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[4].Value);
                //встановлення типу оцінки
                cmbxType.SelectedIndex = cmbxType.Items.IndexOf(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[3].Value);
            }
            catch
            {
                //обнуляння при помилці
                numericUpDown1.Value = 0; 
                cmbxType.SelectedIndex = 0;
            }
        }
        //скидання значення оцінки при перемиканні на інший предмет
        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            try
            {
                //вписування значення перемикача з таблиці
                numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[4].Value);
                //встановлення типу оцінки
                cmbxType.SelectedIndex = cmbxType.Items.IndexOf(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[3].Value);
            }
            catch
            {  //обнуляння при помилці
                numericUpDown1.Value = 0;
                cmbxType.SelectedIndex = 0;
            }
        }
        //скидання значення оцінки при перемиканні на інший предмет
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //вписування значення перемикача з таблиці
                numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[4].Value);
                //встановлення типу оцінки
                cmbxType.SelectedIndex = cmbxType.Items.IndexOf(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[3].Value);
            }
            catch
            {   //обнуляння при помилці
                numericUpDown1.Value = 0;
                cmbxType.SelectedIndex = 0;
            }
        }
        //зміна значення типу оцінки
        private void cmbxType_DropDownClosed(object sender, EventArgs e)
        {
            //вписування значення типу оцінки при зміні
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[3].Value = cmbxType.Text;
            try
            {
                //зміна типу оцінки в масиві
                allCheckedList[dataGridView1.CurrentRow.Index].Type = cmbxType.Text;
            }
            catch { }
        }
        //скидання значення оцінки при перемиканні на інший предмет
        private void bindingNavigatorMoveFirstItem_Click(object sender, EventArgs e)
        {
            //вписування значення перемикача з таблиці
            numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[4].Value);
            //встановлення типу оцінки
            cmbxType.SelectedIndex = cmbxType.Items.IndexOf(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[3].Value);
        }
        //скидання значення оцінки при перемиканні на інший предмет
        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {
            //вписування значення перемикача з таблиці
            numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[4].Value);
            //встановлення типу оцінки
            cmbxType.SelectedIndex = cmbxType.Items.IndexOf(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[3].Value);
        }

        //блокування вводу символів для годин
        private void tbOverSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back) return;
                e.Handled = true;
        }

        //автоматичне внесення змін годин в таблицю
        private void tbOverSize_KeyUp(object sender, KeyEventArgs e)
        {
            //вписування часу в таблицю при зміні
            dataGridView1[2, dataGridView1.CurrentRow.Index].Value = tbOverSize.Text;
        }

        //подія закриття вікна
        private void FrmGrading_FormClosing(object sender, FormClosingEventArgs e)
        {
            //знищення об'єктів БД
            dataReader.Close();
            command = null;
        }
    }
}
