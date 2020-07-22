using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reference_Larchenko
{
    public class Student
    {
        //конструктор
        public Student()
        {
        }

        //name
        public string firstNameUA { get; set; }
        public string firstNameEN { get; set; }

        //second name
        public string secNameUA { get; set; }
        public string secNameEN { get; set; }

        //petrname
        public string petrNameUA { get; set; }
        public string petrNameEN { get; set; }

        //day start
        public DateTime dateStart { get; set; }

        public DateTime dateFinish { get; set; }

        public DateTime dateBirthday { get; set; }

        public string placeBirthUA { get; set; }

        public string placeBirthEn { get; set; }

        //номер специальности
        public string specNum { get; set; }
        public string specialization { get; set; }

        //факультет
        public string faculty { get; set; }

        //причина відрахування
        public string expelled { get; set; }

        //номер поточного курсу
        public string course { get; set; }

        //причина відрахування англійською
        public int numexpelled { get; set; }
        public List<string> expelledEN = new List<string>()
        {
            "did not return from academic leave",
            "at their own will",
            "failure to complete the curriculum",
            "violation of contract terms",
            "completed the training"
        };

        //номер наказу
        public string orderNumber { get; set; }
    }
}
