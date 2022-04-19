using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MockTerminal
{
    public partial class MonitorCover : Form
    {
        public PictureBox mbox1;
        public  PictureBox mbox2;
        public MonitorCover()
        {
            InitializeComponent();
        }

        private void MonitorCover_Load(object sender, EventArgs e)
        {
            mbox1 = this.pictureBox1;
            mbox2 = this.pictureBox2;

            if (ConsoleForm.isMatrix == true)
            {
                this.pictureBox1.Visible = true;
                this.pictureBox2.Visible = true;
            }
        }
    }
}
