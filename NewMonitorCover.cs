using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace MockTerminal
{
    public partial class NewMonitorCover : Form
    {
        public List<MatrixLine> Lines = new List<MatrixLine>();
        private List<char> MatrixSymbols = new List<char>();
        private int _lastFormSize;
        public ConsoleForm nmcParent;

        public NewMonitorCover(ConsoleForm parent)
        {
            InitializeComponent();
            this.nmcParent = parent;
            this.Resize += new EventHandler(Form2_Resize);
            _lastFormSize = GetFormArea(this.Size);

            foreach (char c in "ﾊﾐﾋｰｳｼﾅﾓﾆｻﾜﾂｵﾘｱﾎﾃﾏｹﾒｴｶｷﾑﾕﾗｾﾈｽﾀﾇﾍ日二コソヤ012345789:・.=*+012345789-<>¦｜ZƐﾊﾐﾋｰｳｼﾅﾓﾆｻﾜﾂｵﾘｱﾎﾃﾏｹﾒｴｶｷﾑﾕﾗｾﾈｽﾀﾇﾍ日二コソヤ")
            {
                MatrixSymbols.Add(c);
            }

            foreach (MatrixLine line in this.Controls)
            {
                Lines.Add(line);
            }

        }

        private void NewMonitorCover_Load(object sender, EventArgs e)
        {
            this.Location = Screen.AllScreens[1].WorkingArea.Location;
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            Random r = new Random();
            foreach (MatrixLine line in this.Controls)
            {
                string text = "";
                for (int i = 0; i < 19; i++)
                {
                    int ri = r.Next(0, MatrixSymbols.Count());
                    string substring = MatrixSymbols[ri] + "\n";
                    text += substring;
                }
                line.posLead = r.Next(0, 19);
                line.textmod = text;
            }


            
        }

        private int GetFormArea(Size size)
        {
            return size.Height * size.Width;
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            float scaleFactor = (float)GetFormArea(control.Size) / (float)_lastFormSize;

            ResizeFont(this.Controls, scaleFactor);

            _lastFormSize = GetFormArea(control.Size);

        }

        private void ResizeFont(Control.ControlCollection coll, float scaleFactor)
        {
            foreach (Control c in coll)
            {
                if (c.HasChildren)
                {
                    ResizeFont(c.Controls, scaleFactor);
                }
                else
                {
                    //if (c.GetType().ToString() == "System.Windows.Form.Label")
                    if (true)
                    {
                        // scale font
                        c.Font = new Font(c.Font.FontFamily.Name, c.Font.Size * scaleFactor);
                    }
                }
            }
        }
    }
}
