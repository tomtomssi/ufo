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
        int screenWidth;
        const int MENU_STRIP_HEIGHT = 25;

        //Delegaatin luonti
        private delegate void SetBackgroundPosition(int x, int y, int bgNum);

        public Form1()
        {
            InitializeComponent();
            screenWidth = ClientSize.Width;
            this.background.Location = new System.Drawing.Point(0, MENU_STRIP_HEIGHT);
            g = this.CreateGraphics();
            bg = new Thread(new ThreadStart(bgProcedure));
            bg.IsBackground = false;
            bg.Start();
        }

        //Liikkuvan taustan proseduuri
        public void bgProcedure()
        {
            while (!flag)
            {
                const int MOVE_BG_X = 1;
                //Ensimmäisen taustakuvan X-koordinaatti
                int BG1currentLocation = background.Location.X;

                /* Kun ensimmäisen taustakuvan X-koordinaatti on 0, asetetaan taustakuva 2 sen jatkeeksi */
                if (BG1currentLocation == 0)
                {
                    moveBg(screenWidth, 2);
                }

                //Toisen taustakuvan X-koordinaatti
                int BG2currentLocation = bg2.Location.X;

                //Liikutetaan ensimmäistä taustakuvaa vasemmalle
                moveBg(BG1currentLocation - MOVE_BG_X, 1);

                /* Kun toisen taustakuvan X-koordinaatti on 0, asetetaan taustakuva 1 sen jatkeeksi */
                if (BG2currentLocation == 0)
                {
                    moveBg(screenWidth, 1);
                }

                //Liikutetaan toista taustakuvaa vasemmalle
                moveBg(BG2currentLocation - MOVE_BG_X, 2);

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
            if (bg.IsAlive)
            {
                flag = true;
                bg.Abort();
                Thread.Sleep(100);
            }
            Application.Exit();
        }
        #endregion

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            g.DrawImage(Properties.Resources.Ufo, 100, 100);
        }

        /* Funktio kutsuu bg-threadista main-threadin picturebox-kontrolleja background ja bg2 thread safesti*/
        private void moveBg(int positionX, int bgNum, int positionY = MENU_STRIP_HEIGHT)
        {
            if (this.bg2.InvokeRequired || this.background.InvokeRequired)
            {
                SetBackgroundPosition d = new SetBackgroundPosition(moveBg);
                this.Invoke(d, new object[] { positionX, bgNum, positionY });

            }
            else
            {
                switch (bgNum)
                {
                    case 1:
                        this.background.Location = new Point(positionX, positionY);
                        break;
                    case 2:
                        this.bg2.Location = new Point(positionX, positionY);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
