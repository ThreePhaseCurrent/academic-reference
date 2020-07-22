using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Reference_Larchenko
{
    public partial class FrmSubject : Form
    {
        public FrmInputData createReference = null; //об'єкт для попереднього вікна
        //предмети по семестрам
        public List<CheckedObj> liFisrt = new List<CheckedObj>();
        public List<CheckedObj> liSecond = new List<CheckedObj>();
        public List<CheckedObj> liThree = new List<CheckedObj>();
        public List<CheckedObj> liFour = new List<CheckedObj>();
        public List<CheckedObj> liFive = new List<CheckedObj>();
        public List<CheckedObj> liSix = new List<CheckedObj>();
        public List<CheckedObj> liSeven = new List<CheckedObj>();
        public List<CheckedObj> liEight = new List<CheckedObj>();

        //поточний обраний курс
        private string currentCheck = "1";
        //обрані предмети
        public List<CheckedObj> allCheckedList = new List<CheckedObj>();
        //код обраної спеціалізації
        private string nameSpec = string.Empty;
        //масив для перевірки що хоча б один предмет в семестрі обраний
        private bool[] selectedS = new bool[8];
        //конструктор
        public FrmSubject(FrmInputData createReference, int course)
        {
            InitializeComponent();
            this.createReference = createReference; //отримання посилання
            //отримання обаного номеру спеціальності
            nameSpec = createReference.cbSpecialization.Items[createReference.cbSpecialization.SelectedIndex].ToString();
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //підготовка до відображення вікна
        private void FrmSubject_Shown(object sender, EventArgs e)
        {
            //визначення режиму облікового запису
            if (createReference.account.rang == "+")
            {
                this.Text += " (Адміністратор)";
            }
            else
            {
                this.Text += " (Користувач)";
            }

            //заповнення масиву з предметами
            if(GetStartObj(nameSpec, 1, 1, ref liFisrt) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 1 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if (GetStartObj(nameSpec, 1, 2, ref liSecond) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 2 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if(GetStartObj(nameSpec, 2, 3, ref liThree) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 3 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if (GetStartObj(nameSpec, 2, 4, ref liFour) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 4 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if(GetStartObj(nameSpec, 3, 5, ref liFive) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 5 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if(GetStartObj(nameSpec, 3, 6, ref liSix) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 6 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if(GetStartObj(nameSpec, 4, 7, ref liSeven) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 7 семестрі в обраній спеціалізації!");
                this.Close();
            } else
            if(GetStartObj(nameSpec, 4, 8, ref liEight) == false)
            {
                MessageBox.Show("Відсутні будь-які навчальні дисципліни у 8 семестрі в обраній спеціалізації!");
                this.Close();
            }

            //вивід дисциплін для поточного курсу
            if (currentCheck == "1") { radioButton1.Checked = true; }
            else if (currentCheck == "2") { radioButton2.Checked = true; }
            else if (currentCheck == "3") { radioButton3.Checked = true; }
            else if (currentCheck == "4") { rrr.Checked = true; }

            //встановлення меж для перемикачів
            numericSemester.Value = numericSemester.Maximum;
            numericSemester.Value = numericSemester.Minimum;
            //встановлення списку обраних в початковий вигляд
            lblSemester1.Text = "Обрано " + 0 + " з " + liFisrt.Count;
            lblSemester2.Text = "Обрано " + 0 + " з " + liSecond.Count;
            lblSemester3.Text = "Обрано " + 0 + " з " + liThree.Count;
            lblSemester4.Text = "Обрано " + 0 + " з " + liFour.Count;
            lblSemester5.Text = "Обрано " + 0 + " з " + liFive.Count;
            lblSemester6.Text = "Обрано " + 0 + " з " + liSix.Count;
            lblSemester7.Text = "Обрано " + 0 + " з " + liSeven.Count;
            lblSemester8.Text = "Обрано " + 0 + " з " + liEight.Count;

            //вимикання непотрібних перемикачів курсів
            DisabledRadioButton();
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            //перевірка потрібних файлів
            if (File.Exists("Help/ReferenceHelp.chm") && File.Exists("Help/FormSelectSubject.htm"))
            {
                //відображення довідки
                Help.ShowHelp(this, "Help/ReferenceHelp.chm", HelpNavigator.Topic, "FormSelectSubject.htm");
            }
            else
            {
                MessageBox.Show("Файл довідки для цього вікна не знайдений! Зверніться до адміністратора.", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //вимикання непотрібних перемикачів курсів
        private void DisabledRadioButton()
        {
            //якщо поточний семестр 1 або 2
            if(createReference.currentSemester == 1 || createReference.currentSemester == 2) {
                //вимикання 2, 3 та 4 курсу
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                rrr.Enabled = false;
            }
            //якщо семестр 3 або 4
            else if(createReference.currentSemester == 3 || createReference.currentSemester == 4)
            {
                //вимикання 3 та 4 курсу
                radioButton3.Enabled = false;
                rrr.Enabled = false;
            //якщо 5 або 6 семестр
            } else if(createReference.currentSemester == 5 || createReference.currentSemester == 6)
            {
                //вимикання 4 курсу
                rrr.Enabled = false;
            }
        }

        //вибір всього списку
        private void AutoCheck(ref List<CheckedObj> list)
        {
            //цикл всього списку дисциплін
            foreach(var l in list)
            {
                l.Check = true; //програмне обирання
            }
        }

        //заповнення масивів з предметами
        private bool GetStartObj(string nameSpec, int course, int semester, ref List<CheckedObj> list)
        {
            //формування запиту 
            SQLiteCommand command = new SQLiteCommand("SELECT NameUA, OverSize, NameEN FROM Subject WHERE id_specialization=(SELECT id_specialization FROM specialization WHERE name_ua_spec = @name) AND SemesterNum = @semester", Program.conn);
            //додавання параметрів
            command.Parameters.Add(new SQLiteParameter("@name", nameSpec));
            command.Parameters.Add(new SQLiteParameter("@semester", semester));
            SQLiteDataReader dataReader = command.ExecuteReader(); // виконання запиту

            if (dataReader.HasRows)//перевірка існування рядків
            {
                while (dataReader.Read())
                {
                    //заповнення массиву по семестрово
                    list.Add(new CheckedObj(dataReader[0].ToString(), dataReader[2].ToString(), false, course, semester, Convert.ToInt32(dataReader[1])));
                }
            }
            else
            {
                //очищення об'єктів
                command = null;
                dataReader.Close();
                return false; //вихід з функції
            }
            //очищення об'єктів
            dataReader.Close();
            command = null;
            return true; //повернення true
        }

        //закриття вікна
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); //закриття
        }

        //зміна списків предметів
        private void radioBtn_CheckChange(object sender, EventArgs e)
        {
            var rb = sender as RadioButton; //приведення типу
            currentCheck = rb.Tag.ToString(); //отримання номеру перемикача по тегу

            //перемикамикання пемикача з семестрами
            if (currentCheck == "1") { numericSemester.Minimum = 1; numericSemester.Maximum = 2; }
            else if (currentCheck == "2") { numericSemester.Minimum = 3; numericSemester.Maximum = 4; }
            else if (currentCheck == "3") { numericSemester.Minimum = 5; numericSemester.Maximum = 6; }
            else { numericSemester.Minimum = 7; numericSemester.Maximum = 8; }
        }

        //вивід потрібного в список
        private void ObjInListBox(List<CheckedObj> list, int semester, CheckedListBox checkedListBox)
        {
            checkedListBox.Items.Clear(); //очищення списку
            //цикл додавання дисциплін для відображення
            foreach (var s in list)
            {
                //додавання в елемент
                checkedListBox.Items.Add(s.Name, s.Check);
            }
            //перевірка для активації кнопки обрати все
            CheckAllSelected(checkedListBox);
        }

        //перехід до наступного вікна
        private void nextButton4_Click(object sender, EventArgs e)
        {
            //перевірка, що хоча б одна диципліна для кожного семестру обрана
            if (CheckSelectedSubject(selectedS) == false)
            {
                //повідомлення про помилку
                MessageBox.Show("Оберіть предмети для кожного семестру!","Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; //достроковий вихід з функції
            }

            //очистка масиву з предметами
            allCheckedList.Clear();
            //завантаження всіх масивів в один
            AllListInOne(ref allCheckedList, liFisrt);
            AllListInOne(ref allCheckedList, liSecond);
            AllListInOne(ref allCheckedList, liThree);
            AllListInOne(ref allCheckedList, liFour);
            AllListInOne(ref allCheckedList, liFive);
            AllListInOne(ref allCheckedList, liSix);
            AllListInOne(ref allCheckedList, liSeven);
            AllListInOne(ref allCheckedList, liEight);
            // якщо маасив з обраними порожній
            if (allCheckedList.Count == 0)
            {
                //повідомлення про помилку
                MessageBox.Show("Не обрано жодної дисципліни!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; //достроковий вихід з функції
            }

            this.Visible = false; //зкриття поточного вікна
            //відображення наступного вікна
            FrmGrading frmGrading = new FrmGrading(this, allCheckedList, createReference.student);
            frmGrading.ShowDialog();      
        }

        //перевірка шо хоча б один предмет обраний в кожному семестрі
        private bool CheckSelectedSubject(bool[] selectedS)
        {
            for (int i = 0; i < createReference.currentSemester; i++)
            {
                if (selectedS[i] == false)
                {
                    return false;
                }
            }
            return true;
        }

        //з'єдання масиву з іншим
        private void AllListInOne(ref List<CheckedObj> list, List<CheckedObj> li)
        {
            foreach (var l in li)
            {
                if (l.Check == true) //якщо обрана
                {
                    //додавання в масив
                    list.Add(new CheckedObj(l.Name, l.NameEN, l.Check, l.Course, l.SemesterNum, l.OverSize));
                }
            }
        }

        //запам'ятовування обраного предмету та відображення кількості обраних
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //поточний семестр
            switch (Convert.ToInt32(numericSemester.Value))
            { //для кожного семестру свій лічильник
                case 1:
                    ChangeCheckInList(liFisrt, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester1.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 2:
                    ChangeCheckInList(liSecond, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester2.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 3:
                    ChangeCheckInList(liThree, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester3.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 4:
                    ChangeCheckInList(liFour, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester4.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 5:
                    ChangeCheckInList(liFive, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester5.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 6:
                    ChangeCheckInList(liSix, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester6.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 7:
                    ChangeCheckInList(liSeven, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester7.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
                case 8:
                    ChangeCheckInList(liEight, checkedListBox1.SelectedIndex, Convert.ToInt32(numericSemester.Value));
                    lblSemester8.Text = "Обрано " + checkedListBox1.CheckedItems.Count + " з " + checkedListBox1.Items.Count;
                    break;
            }
            //перевірка для активації кнопки обрати все
            CheckAllSelected(checkedListBox1);
        }

        //перевірка обраних дисциплін для активації кнопки обрати все
        private void CheckAllSelected(CheckedListBox checkedList)
        {
            //якщо кількість обраних менше, ніж інснує
            if(checkedList.CheckedItems.Count < checkedList.Items.Count)
            {
                btnAllSelect.Enabled = true; //вмикання кнопки 
            }
            else
            {
                btnAllSelect.Enabled = false; //блокування кнопки
            }
            //якщо список обраних порожній
            if(checkedList.CheckedItems.Count == 0)
            {
                btnDisableSelect.Enabled = false;
            }
            else
            {
                btnDisableSelect.Enabled = true;
            }
        }

        //зміна обраний чи ні
        private void ChangeCheckInList(List<CheckedObj> list, int index, int numS)
        {
            if (index != -1) //якщо не за межами індексу
            {
                if (list[index].Check == false) // якщо не відмічений
                {
                    list[index].Check = true; //відмічання
                    if (CountCheckList(list) == 1) 
                    {
                        selectedS[numS - 1] = true;
                    }
                }
                else
                {
                    list[index].Check = false; //зняття прапорця
                    if (CountCheckList(list) == 0)
                    {
                        selectedS[numS - 1] = false;
                    }
                }
            }
        }


        //Визначення кількості вже обраних предметів
        private int CountCheckList(List<CheckedObj> list)
        {
            int i = 0; //лічільник
            //загальний цикл 
            foreach(var l in list)
            {
                if(l.Check == true) //якщо обрано
                {
                    i++; //збільшення лічільника
                }
            }

            return i;
        }

        //обрати все (для обраного курсу)
        private void btnAllSelect_Click(object sender, EventArgs e)
        {
            //цикл для всєї колекції
            for(int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (!checkedListBox1.GetItemChecked(i))
                {
                    //виставлення прапорця обраного предмета
                    checkedListBox1.SetItemChecked(i, true);
                    //програмно активуємо подію зміни індексу
                    checkedListBox1.SelectedIndex = -1;
                    checkedListBox1.SelectedIndex = i;
                }
            }
            btnDisableSelect.Enabled = true;
            btnAllSelect.Enabled = false;
        }

        //подія закриття вікна
        private void CreateReference2_FormClosing(object sender, FormClosingEventArgs e)
        {
            createReference.Visible = true; // відображення попереднього вікна
        }

        //премикання семестру та вивід списку предметів
        private void numericSemester_ValueChanged(object sender, EventArgs e)
        {
            //якщо обраний семестер більше ніж поточний
            if(numericSemester.Value > createReference.currentSemester) {
                numericSemester.Value = numericSemester.Value - 1;
                return;
            }
            //якщо 1 курс
            if (currentCheck == "1")
            {
                if (numericSemester.Value == 1)
                {
                    //вивід у список
                    ObjInListBox(liFisrt, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liSecond, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
            else //якщо 2 курс
            if (currentCheck == "2")
            {
                if (numericSemester.Value == 3)
                {
                    //вивід у список
                    ObjInListBox(liThree, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liFour, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
            else //якщо 3 курс
            if (currentCheck == "3")
            {
                if (numericSemester.Value == 5)
                {
                    //вивід у список
                    ObjInListBox(liFive, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liSix, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
            else //якщо 4 курс
            if (currentCheck == "4")
            {
                if (numericSemester.Value == 7)
                {
                    //вивід у список
                    ObjInListBox(liSeven, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liEight, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
        }

        //зняти все
        private void btnDisableSelect_Click(object sender, EventArgs e)
        {
            //цикл для всєї колекції
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    //виставлення прапорця обраного предмета
                    checkedListBox1.SetItemChecked(i, false);
                    //програмно активуємо подію зміни індексу
                    checkedListBox1.SelectedIndex = -1;
                    checkedListBox1.SelectedIndex = i;
                }
            }
            btnDisableSelect.Enabled = false;
        }
    }
}
