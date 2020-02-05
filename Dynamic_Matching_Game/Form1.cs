using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Dynamic_Matching_Game
{
   
    public partial class MainForm : Form
    {

        int Rows = 5;
        int Columns = 7;
        int TimerInterval = 1000;
        int BlinkTime = 1500;

        MatchingGame mg;

        public int TimerInterval1 { get => TimerInterval; set => TimerInterval = value; }

        public MainForm()
        {
            InitializeComponent();
        }

        public void GameRestart()
        {


            if (mg != null)
            {
                for (int i = 0; i < mg.Container.Count; i++)
                    this.Controls["panel_" + (i + 1).ToString()].Dispose();

                mg.Dispose();
                Application.DoEvents();
                
            }

           mg = new MatchingGame(this, Rows, Columns, 35, 10, TimerInterval1, BlinkTime);

        }

        

        private void Form1_Shown(object sender, EventArgs e)
        {
            GameRestart();        
        }

        private void setHardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rows = 7;
            Columns = 9;
            TimerInterval1 = 1100;
            BlinkTime = 1200;
            GameRestart();
        }

        private void setEasyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rows = 4;
            Columns = 5;
            TimerInterval1 = 1200;
            BlinkTime = 2000;
            GameRestart();
        }

        private void setMediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rows = 5;
            Columns = 7;
            TimerInterval1 = 1000;
            BlinkTime = 1500;
            GameRestart();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameRestart();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }



        



  
    }

}
