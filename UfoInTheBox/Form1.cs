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
        Thread bg, moveUfo;
        bool flag = false;
        int screenWidth;
        const int MENU_STRIP_HEIGHT = 25;
        bool gameRunning = true;
        const int MOVE_UFO = 2;
        private List<Keys> isKeyDown = new List<Keys>();

        //Delegaatin luonti
        private delegate void SetBackgroundPosition(int x, int y, int bgNum);

        public Form1()
        {
            InitializeComponent();
            screenWidth = ClientSize.Width;
            //Ensimmäisen taustakuvan alustus
            this.background.Location = new System.Drawing.Point(0, MENU_STRIP_HEIGHT);

            //Ufon alustukset
            ufoBox.Location = new Point(screenWidth / 2 - 31 - 17, (MENU_STRIP_HEIGHT + ClientSize.Height) / 2);
            ufoBox.Size = new Size(31, 17);
            ufoBox.BackgroundImage = Properties.Resources.Ufo;

            this.DoubleBuffered = true;

            bg = new Thread(new ThreadStart(bgProcedure));
            bg.Start();

            moveUfo = new Thread(new ThreadStart(move));
            moveUfo.Start();
        }

        public Point UfoBox
        {
            get { return ufoBox.Location; }
            set { ufoBox.Location = value; }
        }
        //Taustasäie, joka hoitaa ufon liikutuksen ruudulla
        #region Ufo Movement
        //Lisätään listalle painettu näppäin. Jos listalla ei ole nappia, se lisätään siihen
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            if (!isKeyDown.Contains(key))
            {
                isKeyDown.Add(key);
            }
        }
        //Näppäimen noustessa, poistetaan listalta nouseva näppäin
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            if (isKeyDown.Contains(key))
            {
                isKeyDown.Remove(key);
            }
        }
        //Invoke, jotta ufoa voidaan liikuttaa UI threadissa
        void invoker(int movedY, int movedX)
        {
            Invoke((MethodInvoker)delegate
            {
                ufoBox.Location = new Point(
                        ufoBox.Location.X + movedX,
                        ufoBox.Location.Y + movedY);
            });
        }
        //Ufon liikutus
        private void move()
        {
            while (gameRunning)
            {
                if (isKeyDown.Contains(Keys.Down))
                {
                    invoker(MOVE_UFO, 0);
                }
                if (isKeyDown.Contains(Keys.Up))
                {
                    invoker(-MOVE_UFO, 0);
                }

                if (isKeyDown.Contains(Keys.Left))
                {
                    invoker(0, -MOVE_UFO);
                }
                if (isKeyDown.Contains(Keys.Right))
                {
                    invoker(0, MOVE_UFO);
                }

                Thread.Sleep(100);
            }
        }
        #endregion
        //Taustasäie, joka hoitaa taustakuvan liikutuksen
        #region Background
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
        #endregion
        //Threadien keskeytykset ja sovelluksen sulkeminen
        #region Exits and stops
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = true;
            gameRunning = false;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!flag || gameRunning)
            {
                gameRunning = false;
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
            gameRunning = false;
            Application.Exit();
        }
        #endregion

 
    }
}