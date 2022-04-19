using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace MockTerminal
{
    public class MatrixLine : System.Windows.Forms.Label
    {
        private NewMonitorCover ParentForm;
        private int modifier = 0;
        private List<Color> Colors = new List<Color>() { 
            Color.White, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.FromArgb(0, 20, 0), Color.FromArgb(0, 20, 0), Color.FromArgb(0, 33, 0), Color.FromArgb(0, 54, 0), Color.FromArgb(0, 84, 0), Color.FromArgb(0, 84, 0), Color.FromArgb(0, 153, 0), Color.FromArgb(0, 207, 0), Color.FromArgb(0, 255, 0)
        };
        private List<char> MatrixSymbols = new List<char>();
        public string textmod = "";
        public int posLead = 18;

        public MatrixLine()
        {
            //Sequence:  White, Black for 9, (0, 20, 0), (0, 20, 0), (0, 33, 0), (0, 54, 0), (0, 84, 0), (0, 84, 0), (0, 153, 0), (0, 207, 0), (0, 255, 0)
            
            this.ForeColor = Color.Lime;
            this.ParentForm = this.Parent as NewMonitorCover;
            this.posLead = this.Modifier;
            foreach (char c in "ﾊﾐﾋｰｳｼﾅﾓﾆｻﾜﾂｵﾘｱﾎﾃﾏｹﾒｴｶｷﾑﾕﾗｾﾈｽﾀﾇﾍ日二コソヤ012345789:・.=*+012345789-<>¦｜ZƐﾊﾐﾋｰｳｼﾅﾓﾆｻﾜﾂｵﾘｱﾎﾃﾏｹﾒｴｶｷﾑﾕﾗｾﾈｽﾀﾇﾍ日二コソヤ")
            {
                MatrixSymbols.Add(c);
            }



            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += new EventHandler(this.Animate);
            timer.Start();
        }

        private void Animate(object sender, EventArgs e)
        {
            if (this.posLead == 18)
            {
                this.posLead = 0;
            }
            else
            {
                this.posLead += 1;
            }
            this.CreateGraphics().Clear(ConsoleForm.TheConsole.BackColor);
            Random r = new Random();
            List<string> textmodarr = textmod.Split('\n').ToList<string>();
            textmodarr.RemoveAt(textmodarr.Count - 1);
            string Whitespacer(int spaces)
            {
                string whitespace = "";
                for (int i = 0; i < spaces; i++)
                {
                    whitespace += "\n";
                }
                return whitespace;
            }
            this.AutoSize = false;
            this.Size = new Size(48, 870);
            this.Font = new Font(this.Font.FontFamily, 34.25f, this.Font.Style);

            SolidBrush nonLeadBrush = new SolidBrush(ConsoleForm.TheConsole.ForeColor);
            SolidBrush leadBrush = new SolidBrush(Color.White);
            if ( ConsoleForm.TheConsole.ForeColor == Color.White)
            {
                leadBrush.Color = Color.Yellow;
            }
            else if (ConsoleForm.LightsAreOn)
            {
                leadBrush.Color = Color.Black;
            }
            
            string withoutLead = string.Join("\n", textmodarr);
            Console.WriteLine(textmod);
            string lead = Whitespacer(posLead) + textmodarr[posLead];

            this.CreateGraphics().DrawString(withoutLead, this.Font, nonLeadBrush, 0, 0);
            this.CreateGraphics().DrawString(lead, this.Font, leadBrush, 0, 0);
        }

        public int Modifier
        {
            get
            {
                return modifier;
            }
            set
            {
                this.modifier = value;
                Invalidate();
            }
        }
    }
}
