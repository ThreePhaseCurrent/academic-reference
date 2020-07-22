using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reference_Larchenko
{
    //обрана дисципліна
    public class CheckedObj
    {
        //конструктор
        public CheckedObj() {}
        //конструктор
        public CheckedObj(string nameUA, string nameEN, bool x, int course, int semester, int overSize)
        {
            //отримання значень
            this.name = nameUA;
            this.nameEN = nameEN;
            this.check = x;
            this.course = course;
            this.semesterNum = semester;
            this.overSize = overSize;
        }

        //назва дисципліни українською
        private string name;
        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }

        //назва дисципліни англійською
        private string nameEN;
        public string NameEN
        {
            get { return nameEN; }
            set { this.nameEN = value; }
        }
        //курс на якому вивчається
        private int course;
        public int Course
        {
            get { return course; }
            set { course = value; }
        }
        //семестр на якому вивчається
        private int semesterNum;
        public int SemesterNum
        {
            get { return semesterNum; }
            set { semesterNum = value; }
        }

        //обраний чи ні
        private bool check;
        public bool Check
        {
            get { return check; }
            set { this.check = value; }
        }
        //тип оцінки для диципліни
        private string type = "Залік";
        public string Type
        {
            get { return type; }
            set { this.type = value; }
        }

        //оцінка
        private int value = 0;
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        //загальний обсяг
        private int overSize;
        public int OverSize
        {
            get { return overSize; }
            set { overSize = value; }
        }
    }
}
