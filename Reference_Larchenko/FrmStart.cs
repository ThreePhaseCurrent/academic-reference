using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reference_Larchenko
{
    public partial class FrmStart : Form
    {
        //конструктор
        public FrmStart()
        {
            InitializeComponent();
        }

        //завантаження форми
        private void FrmStart_Load(object sender, EventArgs e)
        {
            timer1.Start();//запуск таймеру
        }

        //відраховування 5 секунд
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();//зупинка таймеру
            this.Close();//закриття вікна
        }
    }
}
