using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Resources;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

//... ^ misc requirements ^ ...\\

namespace MockTerminal
{
    public partial class ConsoleForm : Form
    {
        public string appversion = "  MockTerminal Sweet Tea (Beta 6.0)";
        //... ^ Application Version ^ ...\\

        public static RichTextBox TheConsole;
        private int currentLine; // an integer that represents the current line number in the rich text box used on the form (zero-based)
        private int index; // i actually forgot what this does (probably zero-based)
        private string currentLineText; // the text contained on the line that the cursor is currently on
        private bool NoCommandGiven; // a bool used to tell if a command was just given or not. idk i never use this for anything really
        private bool TypeOutIsRunning = false; // this is important and should never be touched... jk its kinda useless and is supposed to stop the keyboard
        private bool awaitingName = false; // never used.
        public string UserName; // basically never used
        private int timesHello = 0; // used for the 'hello' command to tell how many times the user has previously said hello
        public string prefix = ">"; // variable used for the prefix of the program
        private string ForeColor; // current forecolor of the form rich text box
        public string event_filename; //filename used in the 'create event' command
        public string event_title; //title used in the 'create event' command
        public string event_desc; //description used in the 'create event' command
        public DateTime event_date; //date used in the 'create event' command
        private bool commandOverride = false; // used for the 'create event' command so that the enter key doesnt fuck it all up. basically, don't touch
        bool? datewait = null; // too lazy to explain why its not a true boolean
        bool titlewait = false; // used to tell the program that it's waiting on the title to be entered
        bool descwait = false; // used to tell the program that it's waiting on the description to be entered
        public static bool isMatrix = false; // used to tell the program that the matrix mode is enabled or disabled
        public static bool LightsAreOn = false;
        SoundPlayer lowbeep = new SoundPlayer("./assets/audio/beep-low-medium.wav"); //lowbeep command sound
        SoundPlayer beep = new SoundPlayer("./assets/audio/beep-high-medium.wav"); //beep command sound
        public List<Event> EventList = new List<Event>(); // a list of events to enumerate or iterate through
        public List<MonitorCover> MonitorCovers = new List<MonitorCover>(); // a list of monitor covers to iterate through
        public List<NewMonitorCover> NewMonitorCovers = new List<NewMonitorCover>();// a list of the new monitor cover type
        //... ^ Global Variables ^ ...\\



        [DllImport("user32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);
        public string FirstUse = File.ReadAllText("checks/FirstUse.CAMPH");

        //... ^ Weird Shit I Copy And Pasted For The Look Of The Cursor ^ ...\\


        public ConsoleForm()
        {
            InitializeComponent();
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            ConsoleForm.TheConsole = richTextBox1;
            foreach(Screen screen in Screen.AllScreens)
            {
                if (screen == Screen.PrimaryScreen)
                {
                    this.Location = screen.WorkingArea.Location;
                }
                else
                {
                    MonitorCover mc = new MonitorCover();
                    mc.Location = screen.WorkingArea.Location;
                    mc.Show();
                    mc.WindowState = FormWindowState.Normal;
                    mc.FormBorderStyle = FormBorderStyle.None;
                    mc.WindowState = FormWindowState.Maximized;
                    MonitorCovers.Add(mc);
                }
            }


            

            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;

            //... ^ used to set the program to it's fullscreen look ^ ...\\


            try //first time use of console
            {
                UserName = File.ReadLines("checks/data.term").Skip(0).Take(1).First();
            }
            catch (System.InvalidOperationException)
            {
                FirstUse = "yes";
                File.WriteAllText("checks/FirstUse.CAMPH", "yes");
            }

            try //prefix
            {
                prefix = File.ReadLines("checks/data.term").Skip(1).Take(1).First();
            }
            catch (System.InvalidOperationException)
            {
                prefix = "> ";
                string[] filebefore = File.ReadAllLines("checks/data.term");
                filebefore[1] = "> ";
                File.WriteAllLines("checks/data.term", filebefore);
            }

            try //back colors
            {
                string backColor = File.ReadLines("checks/data.term").Skip(3).Take(1).First();
                if (backColor == "on")
                {
                    LightsAreOn = true;
                    richTextBox1.BackColor = Color.White;
                    this.BackColor = Color.White;
                    foreach (NewMonitorCover nnmc in NewMonitorCovers)
                    {
                        nnmc.BackColor = Color.White;
                    }
                    foreach (MonitorCover mmc in MonitorCovers)
                    {
                        mmc.BackColor = Color.White;
                    }
                }
                else
                {
                    LightsAreOn = false;
                }
            }
            catch
            {
                string[] filebefore = File.ReadAllLines("checks/data.term");
                filebefore[3] = "off";
                File.WriteAllLines("checks/data.term", filebefore);
            }

            try //colors
            {
                ForeColor = File.ReadAllLines("checks/data.term").Skip(2).Take(1).First();
                switch (ForeColor)
                {
                    case "red":
                        richTextBox1.ForeColor = Color.Red;
                        break;
                    case "orange":
                        richTextBox1.ForeColor = Color.Orange;
                        break;
                    case "yellow":
                        richTextBox1.ForeColor = Color.Yellow;
                        break;
                    case "green":
                        richTextBox1.ForeColor = Color.LimeGreen;
                        break;
                    case "blue":
                        richTextBox1.ForeColor = Color.FromArgb(59, 122, 237);
                        break;
                    case "cyan":
                        richTextBox1.ForeColor = Color.Cyan;
                        break;
                    case "purple":
                        richTextBox1.ForeColor = Color.FromArgb(170, 0, 255);
                        break;
                    case "magenta":
                        richTextBox1.ForeColor = Color.Magenta;
                        break;
                    case "pink":
                        richTextBox1.ForeColor = Color.FromArgb(240, 125, 232);
                        break;
                    case "grey":
                        richTextBox1.ForeColor = Color.Gray;
                        break;
                    case "gray":
                        richTextBox1.ForeColor = Color.Gray;
                        break;
                    case "white":
                        if (LightsAreOn)
                        {
                            TypeOut(100, "\n\n! ERROR !\nUNKNOWN COLOR GIVEN IN STATEMENT 'colors'.\n\n\n" + prefix + " ");
                            break;
                        }
                        else
                        {
                            richTextBox1.ForeColor = Color.White;
                            break;
                        }
                    case "brown":
                        richTextBox1.ForeColor = Color.FromArgb(160, 82, 45);
                        break;
                    case "black":
                        if (LightsAreOn)
                        {
                            richTextBox1.ForeColor = Color.Black;
                            break;
                        }
                        else
                        {
                            TypeOut(100, "\n\n! ERROR !\nUNKNOWN COLOR GIVEN IN STATEMENT 'colors'.\n\n\n" + prefix + " ");
                            break;
                        }
                    default:
                        TypeOut(100, "\n\n! ERROR !\nUNKNOWN COLOR GIVEN IN STATEMENT 'colors'.\n\n\n" + prefix + " ");
                        
                        break;
                }
            }
            catch (System.InvalidOperationException)
            {
                ForeColor = "green";
                string[] filebefore = File.ReadAllLines("checks/data.term");
                filebefore[2] = "green";
                File.WriteAllLines("checks/data.term", filebefore);
            }

            try //events
            {
                foreach(var file in Directory.GetFiles("./customs/events", "*.event"))
                {
                    string[] filelines = File.ReadAllLines(file);
                    EventList.Add(new Event(DateTime.Parse(filelines[0]), filelines[1], filelines[2]));
                }
            }
            catch(System.IO.FileNotFoundException)
            {
                TypeOut(100, "\n\n! ERROR !\nCOULD NOT FIND EVENT FILES/FOLDER/DIRECTORY\n\n\n" + prefix + " ");
            }



            WelcomeSequence(); // this is where it all starts, bucko.
        }

        void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            this.ChangeCaret();
            index = richTextBox1.SelectionStart;
            currentLine = richTextBox1.GetLineFromCharIndex(index);
        }


        void richTextBox1_MouseDown(object sender, MouseEventArgs e)

        {
            this.ChangeCaret();
        }


        void richTextBox1_GotFocus(object sender, EventArgs e)

        {
            this.BeginInvoke(new MethodInvoker(ChangeCaret));

        }

        void ConsoleForm_Shown(object sender, EventArgs e)
        {
            this.ChangeCaret();
        }

        void richTextBox1_KeyDown(object sender, KeyEventArgs e)        // this is the most massive function i've ever written.
        {
                currentLineText = richTextBox1.Lines[currentLine];
                
            if (e.KeyCode == Keys.Enter && commandOverride == false)
            {
                currentLineText = richTextBox1.Lines[currentLine];
                if (currentLineText == $"{prefix} ping")
                {
                    TypeOut(100, "\n\npong!\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText.Contains($"{prefix} help "))
                {
                    string[] linetext = currentLineText.Split(' ');
#pragma warning disable CS4014  //don't touch this it's for stupid shit
                    if (linetext[2] == "1") //command list page 1
                        TypeOut(1, "\n\n----------------\n--COMMAND LIST--          PAGE #1\n----------------" +
                            "\n!-IMPORTANT-!  USE 'clean console' IF YOU FIND YOUR COMPUTER IS RUNNING SLOW OR THE TERMINAL IS RUNNING SLOW.  !-IMPORTANT-!\n!-IMPORTANT-!  AFTER EXTENDED USE, THE PROGRAM CAN BUILD UP TOO MUCH TEXT HISTORY AND WILL TAKE UP MEMORY.   !-IMPORTANT-!" +
                            "\n\n * 'ping' tests the response time and/or the response functionality of the terminal. It is also a very fun thing to try out." +
                            "\n * 'datetime' displays the current date & time in MONTH/DAY/YEAR + HOUR:MINUTE:SECOND AM/PM format." +
                            "\n * 'hello' lets you say hello to the program. And who knows, it might just say hi back. :)" +
                            "\n * 'prefix yourprefixhere' lets you edit the prefix displayed before every command line. The current prefix is '" + prefix + "'." +
                            "\n * 'colors [fontcolor]' sets the colors of the console to a custom color set." +
                            "\n * 'colorlist' displays the entire list of colors available for usage in the 'colors' command." +
                            "\n * 'help [pagenumber 1 - 3]' displays a list of commands depending on the page for the user." +
                            "\n * 'terminate' exits the program immediately." +
                            "\n * 'matrixmode' toggles the 'hidden' matrix mode. IF YOU WISH TO SWITCH BACK TO NORMAL MODE, JUST TYPE 'matrixmode' again." +
                            "\n\n\n" + prefix + " ");
                    else if (linetext[2] == "2") //command list page 2
                        TypeOut(1, "\n\n----------------\n--COMMAND LIST--          PAGE #2\n----------------" +
                            "\n\n * 'longblink' makes the console to a long blink." +
                            "\n * 'blink' makes the console to a blink." +
                            "\n * 'say [message]' where 'message' is all text after 'say'. Will print all text after 'say' into the console." +
                            "\n * 'lowbeep' plays a low-pitched beep sound." +
                            "\n * 'beep' plays a high-pitched beep sound." +
                            "\n * 'close' will close the application after a short delay." +
                            "\n * 'version' displays the current application version that you are using." +
                            "\n * 'fuck you' will allow you to engage in a mutual banter session momentarily with the conosle." + 
                            "\n\n\n" + prefix + " ");
                    else if (linetext[2] == "3") //command list page 3
                        TypeOut(1, "\n\n----------------\n--COMMAND LIST--          PAGE #3\n----------------" +
                            "\n\n * 'create event' starts a prompt sequence in which you will set up a new event to be created and stored." +
                            "\n * 'eventlist' lists all events currently stored in the computer." +
                            "\n * 'create event f' will start a prompt sequence in which you will set up a new event to be created via file and then stored" +
                            "\n * 'remove event [exact event name]' where 'exact event title' is the exact event title of your event that you wish to delete." +
                            "\n * 'create command' will create a new file in the 'creations' folder in the app directory that can be used for your own commands." +
                            "\n * 'unload command [exact comm name]' will unload a custom command module of your specification." +
                            "\n * 'load command [exact file name]' excluding the file extension will load a custom command from the 'commands' folder via file." +
                            "\n * 'let there be light' will turn on the lights. God save you." +
                            "\n\n\n" + prefix + " ");
                    else
                        TypeOut(100, "\n\n! ERROR !\nINVALID ARGUMENT GIVEN IN COMMAND 'help [pagenumber]'.\n\n\n" + prefix + " ");
#pragma warning restore CS4014
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} hello")  //hello command
                {
                    timesHello++;
                    if (timesHello == 5)
                    {
                        TypeOut(300, "\n\nWow! You really like to say hello! :)\n\n\n" + prefix + " ");
                        timesHello = 0;
                    }
                    else
                    {
                        TypeOut(100, "\n\nHello! :)\n\n\n" + prefix + " ");
                    }
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} terminate")
                {
                    Environment.Exit(0);
                }
                else if (currentLineText == $"{prefix} let there be light")
                {
                    if (LightsAreOn == true)
                    {
                        richTextBox1.BackColor = Color.Black;
                        richTextBox1.ForeColor = Color.White;
                        this.BackColor = Color.Black;
                        foreach (NewMonitorCover nnmc in NewMonitorCovers)
                        {
                            nnmc.BackColor = Color.Black;
                        }
                        foreach (MonitorCover mmc in MonitorCovers)
                        {
                            mmc.BackColor = Color.Black;
                        }
                        string[] filebefore = File.ReadAllLines("checks/data.term");
                        filebefore[3] = "off";
                        File.WriteAllLines("checks/data.term", filebefore);
                        LightsAreOn = false;
                        TypeOut(100, "\n\nThere is now no light.\n\n\n" + prefix + " ");
                    }
                    else
                    {
                        richTextBox1.BackColor = Color.White;
                        richTextBox1.ForeColor = Color.Black;
                        this.BackColor = Color.White;
                        foreach (NewMonitorCover nnmc in NewMonitorCovers)
                        {
                            nnmc.BackColor = Color.White;
                        }
                        foreach (MonitorCover mmc in MonitorCovers)
                        {
                            mmc.BackColor = Color.White;
                        }
                        string[] filebefore = File.ReadAllLines("checks/data.term");
                        filebefore[3] = "on";
                        File.WriteAllLines("checks/data.term", filebefore);
                        LightsAreOn = true;
                        TypeOut(100, "\n\nThere is now light.\n\n\n" + prefix + " ");
                    }
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} matrixmode")
                {
                    NewMonitorCovers.Clear();
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        if (screen != Screen.PrimaryScreen)
                        {
                            NewMonitorCover nmc = new NewMonitorCover(this);
                            nmc.Location = screen.WorkingArea.Location;
                            nmc.WindowState = FormWindowState.Normal;
                            nmc.FormBorderStyle = FormBorderStyle.None;
                            nmc.WindowState = FormWindowState.Maximized;
                            if (LightsAreOn)
                            {
                                    nmc.BackColor = Color.White;
                                foreach (MatrixLine line in nmc.Controls)
                                {
                                    line.BackColor = Color.White;
                                }
                            }
                            else
                            {
                                    nmc.BackColor = Color.Black;
                                foreach (MatrixLine line in nmc.Controls)
                                {
                                    line.BackColor = Color.Black;
                                }
                            }
                            NewMonitorCovers.Add(nmc);
                        }
                    }

                    if (isMatrix == true)
                    {
                        
                        foreach (NewMonitorCover nmc in NewMonitorCovers)
                        {
                            nmc.Hide();
                        }
                        isMatrix = false;
                        TypeOut(100, "\n\nMatrixMode set to inactive.\n\n\n" + prefix + " ");
                        /*for (int i = 0; i < MonitorCovers.Count(); i++)
                        {
                            MonitorCovers[i].mbox1.Visible = false;
                            MonitorCovers[i].mbox2.Visible = false;
                        }*/
                    }
                    else
                    {
                        foreach (NewMonitorCover nmc in NewMonitorCovers)
                        {
                            nmc.Show();
                        }
                        this.Focus();
                        isMatrix = true;
                        TypeOut(100, "\n\nMatrixMode set to active.\n\n\n" + prefix + " ");
                        /*for (int i = 0; i < MonitorCovers.Count(); i++)
                        {
                            MonitorCovers[i].mbox1.Visible = true;
                            MonitorCovers[i].mbox2.Visible = true;
                        }*/
                    }

                    NoCommandGiven = false;
                }
                else if (currentLineText.Contains($"{prefix} prefix "))  //prefix command
                {
                    string[] prefixArr;
                    prefixArr = currentLineText.Split(' ');
                    try
                    {
                        prefix = prefixArr[2].ToString();
                        string[] filebefore = File.ReadAllLines("checks/data.term");
                        filebefore[1] = prefix;
                        File.WriteAllLines("checks/data.term", filebefore);
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        TypeOut(100, "\n\n! ERROR !\nMISSING ARGUMENT IN STATEMENT 'prefix'." + "\n\n\n" + prefix + " ");
                    }
                    TypeOut(100, "\n\nPrefix set to " + prefix + ".\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} datetime")  //datetime command
                {
                    TypeOut(100, "\n\nCurrent datetime is: " + DateTime.Now + ".\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText.Contains($"{prefix} colors "))  //colors command
                {
                    string[] colorsArr;
                    colorsArr = currentLineText.Split(' ');
                    switch (colorsArr[2])
                    {
                        case "red":
                            richTextBox1.ForeColor = Color.Red;
                            break;
                        case "orange":
                            richTextBox1.ForeColor = Color.Orange;
                            break;
                        case "yellow":
                            richTextBox1.ForeColor = Color.Yellow;
                            break;
                        case "green":
                            richTextBox1.ForeColor = Color.LimeGreen;
                            break;
                        case "blue":
                            richTextBox1.ForeColor = Color.FromArgb(59, 122, 237);
                            break;
                        case "cyan":
                            richTextBox1.ForeColor = Color.Cyan;
                            break;
                        case "purple":
                            richTextBox1.ForeColor = Color.FromArgb(170, 0, 255);
                            break;
                        case "magenta":
                            richTextBox1.ForeColor = Color.Magenta;
                            break;
                        case "pink":
                            richTextBox1.ForeColor = Color.FromArgb(240, 125, 232);
                            break;
                        case "grey":
                            richTextBox1.ForeColor = Color.Gray;
                            break;
                        case "gray":
                            richTextBox1.ForeColor = Color.Gray;
                            break;
                        case "white":
                            if (LightsAreOn)
                            {
                                TypeOut(100, "\n\n! ERROR !\nIMPROPER ARGUMENT GIVEN IN STATEMENT 'colors'.\nListen, are you crazy? You won't be able to see the letters!\n\n\n" + prefix + " ");
                                break;
                            }
                            else
                            {
                                richTextBox1.ForeColor = Color.White;
                                break;
                            }
                        case "brown":
                            richTextBox1.ForeColor = Color.FromArgb(160, 82, 45);
                            break;
                        case "black":
                            if (LightsAreOn)
                            {
                                richTextBox1.ForeColor = Color.Black;
                                break;
                            }
                            else
                            {
                                TypeOut(100, "\n\n! ERROR !\nIMPROPER ARGUMENT GIVEN IN STATEMENT 'colors'.\nListen, are you crazy? You won't be able to see the letters!\n\n\n" + prefix + " ");
                                break;
                            }
                        default:
                            TypeOut(100, "\n\n! ERROR !\nIMPROPER ARGUMENT GIVEN IN STATEMENT 'colors'.\n\n\n" + prefix + " ");
                            break;
                    }
                    try
                    {
                        ForeColor = colorsArr[2].ToString();
                        string[] filebefore = File.ReadAllLines("checks/data.term");
                        filebefore[2] = ForeColor;
                        File.WriteAllLines("checks/data.term", filebefore);
                        TypeOut(100, "\n\nFont color set to " + ForeColor + ".\n\n\n" + prefix + " ");
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        TypeOut(100, "\n\n! ERROR !\nMISSING ARGUMENT GIVEN IN COMMAND 'colors'.\n\n\n" + prefix + " ");
                    }
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} colorlist")  //colorlist command
                {
                    TypeOut(50, "\n\nred\norange\nyellow\ngreen\nblue\ncyan\npurple\nmagenta\npink\nwhite (only while in darkness)\ngray\nbrown\nblack (only while in the light)\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText.Contains($"{prefix} say"))  //say command
                {
                    string message = currentLineText.Substring(7);
                    TypeOut(50, "\n\n" + '"' + message + '"' + "\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} vaultboy")  //vaultboy command
                {
                    easteregg(1, "./assets/EEs/vaultboy.egg");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} eyesupguardian")  //eyesupguardian command
                {
                    easteregg(1, "./assets/EEs/eyesupguardian.egg");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} invaders")  //invaders command
                {
                    easteregg(1, "./assets/EEs/invaders.egg");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} pacman")  //pacman command
                {
                    easteregg(1, "./assets/EEs/pacman.egg");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} sus")  //sus command
                {
                    easteregg(1, "./assets/EEs/sus.egg");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} longblink")  //longblink command
                {
                    blink(500);
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} blink")  //blink command
                {
                    blink(100);
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} lowbeep")  //lowbeep command
                {
                    lowbeep.Play();
                    NoCommandGiven = false;
                    TypeOut(1, "\n\n\n" + prefix + " ");
                }
                else if (currentLineText == $"{prefix} beep")  //beep command
                {
                    beep.Play();
                    NoCommandGiven = false;
                    TypeOut(1, "\n\n\n" + prefix + " ");
                }
                else if (currentLineText == $"{prefix} close")  //close command
                {
                    CloseConsole();
                }
                else if (currentLineText == $"{prefix} version")  //version command
                {
                    TypeOut(50, "\n\nThe current application version is: " + appversion + "\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} fuck you")  //fuck you command
                {
                    TypeOut(100, "\n\nNo, fuck you." + "\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} Neo.")  //neo command
                {
                    WakeUpNeo();
                    TypeOut(50, "\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} create event")  //create event command
                {
                    commandOverride = true;
                    Plan();
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} eventlist")  //eventlist command
                {
                    int filecount = EventList.Count();

                    async void do_thing()
                    {
                        for(var i = 0; i < filecount; i++)
                        {
                            string date = EventList.ElementAt<Event>(i).ActivationDate.ToString();
                            string title = EventList.ElementAt<Event>(i).EventTitle;
                            string description = EventList.ElementAt<Event>(i).EventDescription;
                            int line_length()
                            {
                                if((date.Length > title.Length && date.Length > description.Length) || (date.Length == title.Length || date.Length == description.Length))  //if date is biggest or same
                                {
                                    return date.Length;
                                }
                                else if ((title.Length > description.Length) || (title.Length == description.Length))  //if title is biggest or same
                                {
                                    return title.Length;
                                }
                                else  //if description is biggest or same
                                {
                                    return description.Length;
                                }
                            }
                            string card()
                            {
                                string line = "";
                                date = "   " + date + "   ";
                                title = "   " + title + "   ";
                                description = "   " + description + "   ";
                                for(int a = 0; a < line_length(); a++)
                                {
                                    line = line + "=";

                                    if(date.Length < line_length())
                                    {
                                        date = date.Insert(date.Length, " ");
                                    }
                                    else
                                    {
                                        //... nothing ...\\
                                    }
                                    
                                    if(title.Length < line_length())
                                    {
                                        title = title.Insert(title.Length, " ");
                                    }
                                    else
                                    {
                                        //... nothing ...\\
                                    }

                                    if(description.Length < line_length())
                                    {
                                        description = description.Insert(description.Length, " ");
                                    }
                                    else
                                    {
                                        //... nothing ...\\
                                    }
                                }
                                description = description.Remove(description.Length - 7, 3);
                                return "\n|" + line + "|\n|" + date + "|\n|" + title + "|\n|" + description + $"#{i+1} |\n|" + line +"|\n";
                            }

                            await TypeOut(1, "\n\n" + card());
                        }

                        await TypeOut(50, "\n\n\n" + prefix + " ");
                    }

                    do_thing();
                    TypeOut(50, "\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText.Contains($"{prefix} create event f"))  //create event f command
                {
                    string[] lines = File.ReadAllLines("./customs/events/" + currentLineText.Substring(prefix.Length + 16) + ".event");
                    Event newevent = new Event(DateTime.Parse(lines[0]), lines[1], lines[2]);
                    EventList.Add(newevent);
                    string date = newevent.ActivationDate.ToString();
                    string title = newevent.EventTitle;
                    string description = newevent.EventDescription;
                    int line_length()
                    {
                        if ((date.Length > title.Length && date.Length > description.Length) || (date.Length == title.Length || date.Length == description.Length))  //if date is biggest or same
                        {
                            return date.Length;
                        }
                        else if ((title.Length > description.Length) || (title.Length == description.Length))  //if title is biggest or same
                        {
                            return title.Length;
                        }
                        else  //if description is biggest or same
                        {
                            return description.Length;
                        }
                    }
                    string card()
                    {
                        string line = "";
                        date = "   " + date + "   ";
                        title = "   " + title + "   ";
                        description = "   " + description + "   ";
                        for (int a = 0; a < line_length(); a++)
                        {
                            line = line + "=";

                            if (date.Length < line_length())
                            {
                                date = date.Insert(date.Length, " ");
                            }
                            else
                            {
                                //... nothing ...\\
                            }

                            if (title.Length < line_length())
                            {
                                title = title.Insert(title.Length, " ");
                            }
                            else
                            {
                                //... nothing ...\\
                            }

                            if (description.Length < line_length())
                            {
                                description = description.Insert(description.Length, " ");
                            }
                            else
                            {
                                //... nothing ...\\
                            }
                        }
                        return "\n|" + line + "|\n|" + date + "|\n|" + title + "|\n|" + description + "|\n|" + line + "|\n";
                    }

                    TypeOut(1, "\n\n" + card() + "\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText == $"{prefix} clean console")  //clean console command
                {
                    richTextBox1.Text = "";
                    TypeOut(50, "\n\n\nConsole cleaned.  :)\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
                else if (currentLineText.Contains($"{prefix} remove event"))  //remove event command
                {
                    string eventname = currentLineText.Substring(prefix.Length + 14);
                    Console.WriteLine(eventname);
                    bool FoundItOrNot = false;

                    foreach(var file in Directory.GetFiles("./customs/events", "*.event"))
                    {
                        Console.WriteLine(File.ReadAllLines(file).ElementAt<string>(1));
                        if (File.ReadAllLines(file).ElementAt<string>(1) == eventname)
                        {
                            foreach(var item in EventList)
                            {
                                if(item.EventTitle == eventname)
                                {
                                    EventList.Remove(item);
                                    break;
                                }
                                else
                                {
                                    //... nothing ...\\
                                }
                            }
                            File.Delete(file);
                            FoundItOrNot = true;
                        }
                        else
                        {
                            FoundItOrNot = false;
                        }
                    }

                    if(FoundItOrNot == false)
                    {
                        TypeOut(50, "\n\n" + "Event deleted. Exiting command process..." + "\n\n\n\n" + prefix + " ");
                        NoCommandGiven = false;
                    }
                    else if(FoundItOrNot == true)
                    {
                        TypeOut(50, "\n\nEvent does not exist. Exiting command process...\n\n\n" + prefix + " ");
                        NoCommandGiven = false;
                    }
                }
                else if(currentLineText.Contains($"{prefix} event"))  //event command
                {
                    string eventindex = currentLineText.Substring(prefix.Length + 6);

                    string date = EventList.ElementAt<Event>(Int32.Parse(eventindex) - 1).ActivationDate.ToString();
                    string title = EventList.ElementAt<Event>(Int32.Parse(eventindex) - 1).EventTitle;
                    string description = EventList.ElementAt<Event>(Int32.Parse(eventindex) - 1).EventDescription;
                    int line_length()
                    {
                        if ((date.Length > title.Length && date.Length > description.Length) || (date.Length == title.Length || date.Length == description.Length))  //if date is biggest or same
                        {
                            return date.Length;
                        }
                        else if ((title.Length > description.Length) || (title.Length == description.Length))  //if title is biggest or same
                        {
                            return title.Length;
                        }
                        else  //if description is biggest or same
                        {
                            return description.Length;
                        }
                    }
                    string card()
                    {
                        string line = "";
                        date = "   " + date + "   ";
                        title = "   " + title + "   ";
                        description = "   " + description + "   ";
                        for (int a = 0; a < line_length(); a++)
                        {
                            line = line + "=";

                            if (date.Length < line_length())
                            {
                                date = date.Insert(date.Length, " ");
                            }
                            else
                            {
                                //... nothing ...\\
                            }

                            if (title.Length < line_length())
                            {
                                title = title.Insert(title.Length, " ");
                            }
                            else
                            {
                                //... nothing ...\\
                            }

                            if (description.Length < line_length())
                            {
                                description = description.Insert(description.Length, " ");
                            }
                            else
                            {
                                //... nothing ...\\
                            }
                        }
                        return "\n|" + line + "|\n|" + date + "|\n|" + title + "|\n|" + description + "|\n|" + line + "|\n";
                    }

                    TypeOut(1, "\n\n" + card() + "\n\n" + prefix + " ");
                }
                else          // if there's something weird put it, just say no to that shit.
                {
                    TypeOut(100, "\n\n! ERROR !\nUNKNOWN COMMAND." + "\n\n\n" + prefix + " ");
                    NoCommandGiven = false;
                }
            }
            else if (e.KeyCode == Keys.Back && currentLineText == prefix + " ")  // stops backkey from deleting shit
            {
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Left && currentLineText == prefix + " ")  // stops the arrow key from going fucking places
            {
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter && commandOverride == true) // something i never even used. not needed anyways i think. might use it later
            {
                //...
            }
        }

        void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            //...
        }

        private void ChangeCaret()

        {
            CreateCaret(this.richTextBox1.Handle, IntPtr.Zero, 10, 20);

            ShowCaret(this.richTextBox1.Handle);
        }

        public async Task TypeOut(int delay, string message)  // custom typing function that gives the form that typing effect that it has
        {
            char[] messageArr = message.ToCharArray();

            if (TypeOutIsRunning == true)
            {
                return;
            }
            else
            {
                TypeOutIsRunning = true;
                foreach (var character in messageArr)
                {
                    richTextBox1.AppendText(character.ToString());
                    await Task.Delay(delay);
                }
            }

            TypeOutIsRunning = false;
        }

        public async void WelcomeSequence()
        {
            if (FirstUse == "yes")
            {
                await TypeOut(100, "Welcome, User.\n");
                await Task.Delay(2000);
                await TypeOut(100, "This is the console.\n");
                await Task.Delay(2000);
                await TypeOut(100, "What shall I call you?\n\n> ");
                awaitingName = true;
            }
            else
            {
                await TypeOut(25, "! TYPE 'help [pagenumber 1 - 3]' TO SEE A LIST OF COMMANDS !\n\n\n" + prefix + " ");
            }
        }  // a welcome sequence

        public async void easteregg(int delay, string path)  //function for easter eggs
        {
            string file = File.ReadAllText(path);
            richTextBox1.AppendText("\n\n");
            foreach (char character in file)
            {
                richTextBox1.AppendText(character.ToString());
                await Task.Delay(delay);
            }
            richTextBox1.AppendText("\n\n\n" + prefix + " ");
            NoCommandGiven = false;
        }
        
        public async void blink(int delay)
        {
            await TypeOut(1, "\n\n\n" + prefix + " ");
            Color consoleColor = richTextBox1.ForeColor;
            richTextBox1.ForeColor = Color.Black;
            await Task.Delay(delay);
            richTextBox1.ForeColor = consoleColor;
        }  // blink function

        public async void CloseConsole()
        {
            await TypeOut(300, "\n\nGoodbye........");
            await Task.Delay(2000);
            Application.Exit();
        }

        public void WakeUpNeo()  // neo function for easter egg
        {
            //...
        }
        async void CheckDataEvent(object sender, KeyEventArgs e)  // can't explain, it does things for event creation
        {
            currentLineText = richTextBox1.Lines[currentLine];
            
            if (e.KeyCode == Keys.Enter && currentLineText == $"{prefix} end")
            {
                await TypeOut(50, $"\n\n\nYou have exited event creation.\n\n\n{prefix} ");
                Console.WriteLine("You just ended event creation");
                descwait = false;
                titlewait = false;
                datewait = null;
                KeyDown -= CheckDataEvent;
                KeyDown += richTextBox1_KeyDown;
                commandOverride = false;
            }
            else if (e.KeyCode == Keys.Enter && datewait == true && titlewait == false && descwait == false && currentLineText != $"{prefix} end")
            {
                DateTime.TryParse(currentLineText.Replace(prefix + " ", ""), out event_date);



                Console.WriteLine(event_date);
                Console.WriteLine("You just typed the date of the event.");
                datewait = false;
                await TypeOut(25, "\n\nPlease type out the title of the event you wish to set. Example: My Wonderful Event\n\n\n" + prefix + " ");
                titlewait = true;
            }
            else if (e.KeyCode == Keys.Enter && datewait == false && titlewait == true && descwait == false && currentLineText != $"{prefix} end")
            {
                event_title = currentLineText.Replace(prefix + " ", "");



                Console.WriteLine(event_title);
                Console.WriteLine("You just typed the title of the event.");
                titlewait = false;
                await TypeOut(25, "\n\nPlease type out the description and the file name of the event you wish to set. Example: My wonderful description. -myfilename\n(make sure to include the hyphen before the filename. make sure not to use a hyphen anywhere else.)\n\n\n" + prefix + " ");
                descwait = true;
            }
            else if (e.KeyCode == Keys.Enter && datewait == false && titlewait == false && descwait == true && currentLineText != $"{prefix} end")
            {
                int indexofhyphen = currentLineText.IndexOf("-");
                event_desc = currentLineText.Remove(indexofhyphen).Replace(prefix + " ", "");
                event_filename = currentLineText.Substring(indexofhyphen + 1);



                if (File.Exists(event_filename))
                {
                    //ask if user wants to delete then do...
                }
                else
                {
                    var newfile = File.CreateText("./customs/events/" + event_filename + ".event");
                    newfile.WriteLine(event_date);
                    newfile.WriteLine(event_title);
                    newfile.WriteLine(event_desc);
                    newfile.Close();


                    EventList.Add(new Event(event_date, event_title, event_desc));
                    Console.WriteLine(EventList.ElementAt<Event>(0).EventTitle);
                    await TypeOut(25, $"\n\n\nYou have set an event.\n\n\n{prefix} ");
                }


                descwait = false;
                titlewait = false;
                datewait = null;
                KeyDown -= CheckDataEvent;
                KeyDown += richTextBox1_KeyDown;
                commandOverride = false;
            }
            else if (e.KeyCode == Keys.Back && currentLineText == prefix + " ")
            {
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Left && currentLineText == prefix + " ")
            {
                e.SuppressKeyPress = true;
            }
        }

        public async void Plan()  //does preliminary shit for event creation
        {
            KeyDown -= richTextBox1_KeyDown;
            KeyDown += CheckDataEvent;

            await TypeOut(25, "\n\nPlease type out the date of the event you wish to set. Example: 01/23/45 12:00:00 AM\n(pssst... you can stop the creation process anytime by typing 'end')\n\n\n" + prefix + " ");
            datewait = true;
        }
    }

    public class Event
    {
        private string title;
        private string description;
        private DateTime activation;

        public Event(DateTime activation, string title, string description)
        {
            this.title = title;
            this.activation = activation;
            this.description = description;
        }

        public string EventTitle
        {
            get { return title; }
            set { title = value; }
        }
        public string EventDescription
        {
            get { return description; }
            set { description = value; }
        }
        public DateTime ActivationDate
        {
            get { return activation; }
            set { activation = value; }
        }
    }  // event class for events to be stored in.
}