using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Reference_Larchenko
{
    public partial class CreateReference2 : Form
    {
        public CreateReference createReference = null;
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
        //код обраної спеціальності
        private int idSpec = 0;
        //масив для перевірки що хоча б один предмет в семестрі обраний
        private bool[] selectedS = new bool[8];

        public CreateReference2()
        {
            InitializeComponent();
        }
        public CreateReference2(CreateReference createReference, int course)
        {
            InitializeComponent();
            this.createReference = createReference;
            idSpec = Convert.ToInt32(createReference.idSpecText.Items[createReference.idSpecText.SelectedIndex]);
            //заповнення масиву з предметами
            GetStartObj(idSpec, 1, 1, ref liFisrt);
            GetStartObj(idSpec, 1, 2, ref liSecond);
            GetStartObj(idSpec, 2, 3, ref liThree);
            GetStartObj(idSpec, 2, 4, ref liFour);
            GetStartObj(idSpec, 3, 5, ref liFive);
            GetStartObj(idSpec, 3, 6, ref liSix);
            GetStartObj(idSpec, 4, 7, ref liSeven);
            GetStartObj(idSpec, 4, 8, ref liEight);

            //вивід дисциплін для поточного курсу
            //currentCheck = createReference.student.course;
            if(currentCheck == "1") { radioButton1.Checked = true; }
            else if(currentCheck == "2") { radioButton2.Checked = true; }
            else if (currentCheck == "3") { radioButton3.Checked = true; }
            else if (currentCheck == "4") { rrr.Checked = true; }

            numericSemester.Value = numericSemester.Maximum;
            numericSemester.Value = numericSemester.Minimum;

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
            //подія виклику інструкції
            this.HelpButtonClicked += HelpButton_Click;
        }

        //виклик інструкції
        private void HelpButton_Click(object sender, CancelEventArgs e)
        {
            FrmStart.ShowInstruction(@"Instructions\3.Вікно вибору дисциплін.docx");
        }

        //вимикання непотрібних перемикачів курсів
        private void DisabledRadioButton()
        {
            if(createReference.currentSemester == 1 || createReference.currentSemester == 2) {
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
                rrr.Enabled = false;
            }
            else if(createReference.currentSemester == 3 || createReference.currentSemester == 4)
            {
                radioButton3.Enabled = false;
                rrr.Enabled = false;
            } else if(createReference.currentSemester == 5 || createReference.currentSemester == 6)
            {
                rrr.Enabled = false;
            }
        }

        //вибір всього списку
        private void AutoCheck(ref List<CheckedObj> list)
        {
            foreach(var l in list)
            {
                l.Check = true;
            }
        }

        //заповнення масивів з предметами
        private void GetStartObj(int idSpec, int course, int semester, ref List<CheckedObj> list)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT NameUA, OverSize, NameEN FROM Subject WHERE idSpecialty=@idSpec AND SemesterNum = @semester", Program.conn);
            command.Parameters.Add(new SQLiteParameter("@idSpec", idSpec));
            command.Parameters.Add(new SQLiteParameter("@semester", semester));
            SQLiteDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                list.Add(new CheckedObj(dataReader[0].ToString(), dataReader[2].ToString(), false, course, semester, Convert.ToInt32(dataReader[1])));
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //зміна списків предметів
        private void radioBtn_CheckChange(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            currentCheck = rb.Tag.ToString();

            //перемикамикання пемикача з семестрами
            if (currentCheck == "1") { numericSemester.Minimum = 1; numericSemester.Maximum = 2; }
            else if (currentCheck == "2") { numericSemester.Minimum = 3; numericSemester.Maximum = 4; }
            else if (currentCheck == "3") { numericSemester.Minimum = 5; numericSemester.Maximum = 6; }
            else { numericSemester.Minimum = 7; numericSemester.Maximum = 8; }
        }

        //вивід потрібного в список
        private void ObjInListBox(List<CheckedObj> list, int semester, CheckedListBox checkedListBox)
        {
            checkedListBox.Items.Clear();
            foreach (var s in list)
            {
                checkedListBox.Items.Add(s.Name, s.Check);
            }
            //перевірка для активації кнопки обрати все
            CheckAllSelected(checkedListBox);
        }

        private void nextButton4_Click(object sender, EventArgs e)
        {
            if (CheckSelectedSubject(selectedS) == false)
            {
                MessageBox.Show("Оберіть предмети для кожного семестру!","Увага!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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

            if (allCheckedList.Count == 0)
            {
                MessageBox.Show("Не обрано жодної дисципліни!", "Увага!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.Visible = false;
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
                if (l.Check == true)
                {
                    list.Add(new CheckedObj(l.Name, l.NameEN, l.Check, l.Course, l.SemesterNum, l.OverSize));
                }
            }
        }

        //запам'ятовування обраного предмету та відображення кількості обраних
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Convert.ToInt32(numericSemester.Value))
            {
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
            if(checkedList.CheckedItems.Count < checkedList.Items.Count)
            {
                btnAllSelect.Enabled = true;
            }
            else
            {
                btnAllSelect.Enabled = false;
            }

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
            if (index != -1)
            {
                if (list[index].Check == false)
                {
                    list[index].Check = true;
                    if (CountCheckList(list) == 1)
                    {
                        selectedS[numS - 1] = true;
                    }
                }
                else
                {
                    list[index].Check = false;
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
            int i = 0;

            foreach(var l in list)
            {
                if(l.Check == true)
                {
                    i++;
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

        private void CreateReference2_FormClosing(object sender, FormClosingEventArgs e)
        {
            createReference.Visible = true;
        }

        //премикання семестру та вивід списку предметів
        private void numericSemester_ValueChanged(object sender, EventArgs e)
        {
            //якщо обраний семестер більше ніж поточний
            if(numericSemester.Value > createReference.currentSemester) {
                numericSemester.Value = numericSemester.Value - 1;
                return;
            }

            if (currentCheck == "1")
            {
                if (numericSemester.Value == 1)
                {
                    ObjInListBox(liFisrt, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liSecond, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
            else
            if (currentCheck == "2")
            {
                if (numericSemester.Value == 3)
                {
                    ObjInListBox(liThree, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liFour, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
            else
            if (currentCheck == "3")
            {
                if (numericSemester.Value == 5)
                {
                    ObjInListBox(liFive, Convert.ToInt32(numericSemester.Value), checkedListBox1);
                }
                else { ObjInListBox(liSix, Convert.ToInt32(numericSemester.Value), checkedListBox1); }
            }
            else
            if (currentCheck == "4")
            {
                if (numericSemester.Value == 7)
                {
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
