using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using System.Collections;


namespace Dynamic_Matching_Game
{


    public class MatchingElement
    {
        public Panel panel = new Panel();
        public PictureBox picturebox = new PictureBox();
        public Control parent;

        public MatchingElement(Control hostForm, int top, int left, string name)
        {
            this.panel.Name = name;
            this.panel.BackColor = Color.Transparent;
            this.panel.BorderStyle = BorderStyle.FixedSingle;
            this.panel.Size = new Size(100, 100);
            this.panel.Location = new Point(left, top);
            this.panel.Visible = false;

            this.picturebox.Size = new Size(83, 83);
            this.picturebox.Location = new Point(8, 8);
            this.picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picturebox.Parent = this.panel;
            this.picturebox.Visible = false;

            this.panel.Parent = hostForm;

            Application.DoEvents();

        }
    }

    public class MatchingGame : IDisposable
    {

        private MainForm HostForm;
        
        public List<MatchingElement> Container = new List<MatchingElement>();

        private Timer timer1 = new Timer();
        private Timer timer2 = new Timer();
        private DateTime dt = new DateTime();


        int LastTag = -1;
        int GameTime = 0;
        int Score = 0;

        public MatchingGame(MainForm HostForm, int rows, int columns, int CornerTop, int CornerLeft, int interval, int blink)
        {


            this.HostForm = HostForm;
            this.HostForm.toolStripStatusLabel1.Text = "     ";
            this.HostForm.toolStripStatusLabel2.Text = "       ";
            this.HostForm.toolStripProgressBar1.Value = 0;
            this.HostForm.toolStripProgressBar1.Visible = false;
            
            int i, j;

            int Last_J = 0;

            for (i = 1; i < rows; i++)
            {
                for (j = 1; j < columns; j++)
                {

                    Container.Add(new MatchingElement(HostForm, CornerTop, CornerLeft, "panel_" + (Last_J + j).ToString()));
                    CornerLeft = CornerLeft + 110;

                }

                HostForm.Size = new Size(CornerLeft + 15, CornerTop + 170);
                Last_J = (j - 1) * i;
                CornerLeft = 10;
                CornerTop = CornerTop + 110;

            }

            HostForm.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - HostForm.Width) / 2,
                                      (Screen.PrimaryScreen.WorkingArea.Height - HostForm.Height) / 2);

            int resnum;

            ResourceSet resourceSet = Dynamic_Matching_Game.Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                resnum = Convert.ToInt32(entry.Key.ToString().Substring(1, 2));

                if (resnum < (this.Container.Count / 2))
                {
                    this.Container[(resnum * 2)].panel.Tag = resnum;
                    this.Container[(resnum * 2)].panel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnPanelMouseClick);
                    this.Container[(resnum * 2)].picturebox.Tag = resnum;
                    this.Container[(resnum * 2)].picturebox.Image = (entry.Value as Image);


                    this.Container[(resnum * 2) + 1].panel.Tag = resnum;
                    this.Container[(resnum * 2) + 1].panel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnPanelMouseClick);
                    this.Container[(resnum * 2) + 1].picturebox.Tag = resnum;
                    this.Container[(resnum * 2) + 1].picturebox.Image = (entry.Value as Image);
                }
            }


            this.timer1.Interval = interval;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);

            this.RandomizePanelPos();

            foreach (MatchingElement me in this.Container)
                me.panel.Visible = true;

            this.Blink(blink);

            this.timer2.Enabled = true;

        }

        private void OnPanelMouseClick(object sender, MouseEventArgs e)
        {
            (sender as Panel).Controls[0].Visible = true;


            if (LastTag == -1)

                LastTag = Convert.ToInt32((sender as Panel).Tag);

            else
            {
                if (Convert.ToInt32((sender as Panel).Tag) == LastTag)
                {
                    foreach (MatchingElement me in this.Container)
                        if (Convert.ToInt32(me.panel.Tag) == LastTag)
                            me.panel.Tag = -1;

                    this.Score = this.Score + 10;
                }

                else

                    this.Score = this.Score - 2;

            }

            this.timer1.Enabled = true;
        }

        private void RandomizePanelPos()
        {
            List<Point> Pos = new List<Point>();
            List<Int32> DonePanels = new List<Int32>();
            Random rnd = new Random();

            foreach (MatchingElement me in this.Container)
                Pos.Add(new Point(me.panel.Location.X, me.panel.Location.Y));

            int RandomPanel = -1;
            int PanelOrd = 0;

            while (DonePanels.Count < this.Container.Count)
            {
                //System.Threading.Thread.Sleep(2);
                RandomPanel = rnd.Next(this.Container.Count);

                if (DonePanels.IndexOf(RandomPanel) == -1)
                {
                    this.Container[RandomPanel].panel.Location = Pos[PanelOrd];
                    DonePanels.Add(RandomPanel);
                    PanelOrd++;
                }

                else
                    continue;
            }

        }

        private void Blink(int time)
        {

            foreach (MatchingElement me in this.Container)
                me.picturebox.Visible = true;

            Application.DoEvents();

            System.Threading.Thread.Sleep(time);

            foreach (MatchingElement me in this.Container)
                me.picturebox.Visible = false;

            Application.DoEvents();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (this.Score >= 0 && this.Score <= 100)
                this.HostForm.toolStripProgressBar1.Value = this.Score;
            else if (this.Score < 0)
                this.HostForm.toolStripProgressBar1.Value = 0;
            else if (this.Score > 100)
                this.HostForm.toolStripProgressBar1.Value = 100;
            
            int UnSolvedPanels = 0;
            
            foreach (MatchingElement me in this.Container)
                if (Convert.ToInt32(me.panel.Tag) >= 0)
                {
                    me.picturebox.Visible = false;
                    UnSolvedPanels++;
                }


            if (UnSolvedPanels == 0)
            {

                this.timer1.Enabled = false;
                this.timer2.Enabled = false;

                string WinMsg = @"You won the game in "
                                + this.dt.AddSeconds(this.GameTime).ToString("mm:ss")
                                + "\n"
                                + "Seconds with " + this.Score.ToString()
                                + "\n"
                                + "Scores, Would you like to continue ? ";

                DialogResult rslt =  MessageBox.Show(WinMsg, "Game over", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

                if (rslt == DialogResult.Yes)
                {

                    this.HostForm.GameRestart();

                }
                else
                {
                    (this.HostForm as Form).Close();
                }
            }
            else
            {
                LastTag = -1;
                timer1.Enabled = false;
            }

            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.GameTime++;
            dt.AddSeconds(GameTime);
            this.HostForm.toolStripStatusLabel1.Text = this.dt.AddSeconds(this.GameTime).ToString("mm:ss");
            this.HostForm.toolStripStatusLabel2.Text = " Score ";
            this.HostForm.toolStripProgressBar1.Visible = true;

        }

        public void Dispose()
        {
            this.timer1.Dispose();
            this.timer2.Dispose();
            this.Container.Clear();
        }


    }

} 

