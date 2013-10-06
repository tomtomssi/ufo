using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UfoInTheBox
{
    public partial class Form1 : Form
    {
        Thread bg;
        bool flag = false;
        Graphics g;

        //Delegaatin luonti
        private delegate void SetBackgroundPosition(int x, int y, int bgNum);
       
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            bg = new Thread(new ThreadStart(bgProcedure));
            bg.Start();
        }

        //Delegaatin funktio valitsee bgNum:n perusteella sen, liikutetaanko ensimmäistä vai toista taustakuvaa
        //1 = background, 2 = bg2
        public void SetBackgroundPositionDelegate(int x, int y, int bgNum)
        {
            switch (bgNum)
            {
                case 1:
                    this.background.Location = new Point(x, y);
                    break;
                case 2:
                    this.bg2.Location = new Point(x, y);
                    break;
                default:
                    break;
            }
            
        }

        //Liikkuvan taustan proseduuri
        public void bgProcedure()
        {
            while (!flag)
            {
                const int MOVE_BG_X = 1;
                SetBackgroundPosition m = SetBackgroundPositionDelegate;
                //Ensimmäisen taustakuvan X-koordinaatti
                int BG1currentLocation = background.Location.X;
                //Ensimmäisen taustakuvan X-koordinaatti
                int BG2currentLocation = bg2.Location.X;

                /* Kun ensimmäisen taustakuvan X-koordinaatti on 0, asetetaan taustakuva 2 sen jatkeeksi */
                if (BG1currentLocation == 0)
                {
                    m.Invoke(799, 25, 2);
                }

                //Liikutetaan ensimmäistä taustakuvaa vasemmalle
                m.Invoke(BG1currentLocation - MOVE_BG_X, background.Location.Y, 1);

                /* Kun toisen taustakuvan X-koordinaatti on 0, asetetaan taustakuva 1 sen jatkeeksi */
                if (BG2currentLocation == 0)
                {
                    m.Invoke(780, 25,1);
                }

                //Liikutetaan toista taustakuvaa vasemmalle
                m.Invoke(BG2currentLocation - MOVE_BG_X, bg2.Location.Y,2);

                Thread.Sleep(10);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Up:
                    
                    break;
            }
        }

        //Threadien keskeytykset ja sovelluksen sulkeminen
        #region Exits and stops
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = true;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!flag)
            {
                flag = true;
            }
            Thread.Sleep(100);
            Application.Exit();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!flag)
            {
                flag = true;
            }
            Thread.Sleep(100);
            Application.Exit();
        }
        #endregion

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            g.DrawImage(Properties.Resources.Ufo, 100, 100);
        }

    }
}
