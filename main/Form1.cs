using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main
{
    public partial class Form1 : Form
    {
        string kullanici = null, skor_dosya_yolu = @"C:\Users\fener\Desktop\puan.txt", yardim_dosya_yolu = @"C:\Users\fener\Desktop\oyunYardim.txt";
        public Thread thread;

        public enum Direction   //Hareket için kullanılan fonkisyon
        {
            Up,
            Down,
            Left,
            Right
        }

        public int x = 1, y = 1;    //Yılanın başlangıç konumu için tanımlanmış değişkenler
        public int a;   //Skor için tanımlanmış değişken  
        
       
            //Sayaçlar için gerekli değişkenler
        public int time = 0; 
        public double timeGame = 0; 
        public int cutTime; 
        public int firstTime = 0;

        public double minute = 0;   //Sayaç için gerekli değişken
        public double second = 0;   //Sayaç için gerekli değişken



        public int score;
        public Direction direction = Direction.Right;   
        public Location food = new Location(-1, -1);    

        Random random = new Random();   
        public List<Location> tales = new List<Location>();


        public Form1()
        {
            
            InitializeComponent();

            tales.Add(new main.Location(0, 0));

            GameTable();

            Control.CheckForIllegalCrossThreadCalls = false;

            timer_label.Text = "0";
            timer_score.Interval = 1000;
            timer_time.Interval = 1000;


            btnYardım.Enabled = true;
            btnSkorlarıGörüntüle.Enabled = true;
            btnKişiyikaydet.Enabled = true;

            thread = new Thread(new ThreadStart(new Action(() =>
            {
                while (true)
                {
                    if (direction == Direction.Right) x++;
                    if (direction == Direction.Down) y++;
                    if (direction == Direction.Left) x--;
                    if (direction == Direction.Up) y--;

                    GameTable();

                    if(kolayRdb.Checked == true) //Yılan hızını ayarlayan döngü
                    {
                        Thread.Sleep(200);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

            })));
            thread.Start();
            thread.Suspend(); // thread duraklatılır
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) direction = Direction.Down; 
            if (e.KeyCode==Keys.Left) direction = Direction.Left;
            if (e.KeyCode==Keys.Up) direction = Direction.Up;
            if (e.KeyCode==Keys.Right) direction = Direction.Right;
           
            if(e.KeyCode == Keys.B)
            {

                btnYardım.Enabled = false;  //Başlangıçta 
                btnSkorlarıGörüntüle.Enabled = false;
                btnKişiyikaydet.Enabled = false;

                zorRdb.Enabled = false;
                kolayRdb.Enabled = false;

                timer_score.Enabled = true;
                timer_time.Enabled = true;

                bilgiPanel.Visible = false;
                txtKişileriKaydet.Enabled = false;

                thread.Resume(); // Resume() metodu Suspend() metodundan sonra çalışabilir.
 
            }

            if (e.KeyCode == Keys.D)    //Oyunun durdurulmasını sağlayan fonksiyon
            {
                zorRdb.Enabled = true;
                kolayRdb.Enabled = true;

                thread.Suspend();

                bilgiPanel.Visible = true;
                timer_score.Enabled = false;
                timer_time.Enabled = false;


                btnYardım.Enabled = true;
                btnSkorlarıGörüntüle.Enabled = true;
                btnKişiyikaydet.Enabled = true;
            } 
        }
        public void GameTable()
        {
                //Yılanın kendi üzerine geldiğinde oyunun sonlanması için döngü
            if (tales.Count != 1)
            {
                for (int i = 1; i < tales.Count; i++)
                {   
                    if (tales[i].x == x && tales[i].y == y)
                    {

                        DosyaIslemi();
                        GameOver();
                    }
                }
            }

                //Yılanın yemi yemesi
            if (x == food.x && y == food.y)
            {
                tales.Add(new main.Location(food.x, food.y));
                food = new Location(-1, -1);
        
                if (x == 1 || y == 3 || x == 49 || y == 51) { Score(); }
                if (x == 49 || y == 51 || x == 1 || y == 3) { Score(); }
                if (x == 49 || y == 51 || x == 49 || y == 51) { Score(); }
                if (x == 1 || y == 3 || x == 1 || y == 3) { Score(); }
                else
                {       //Puanın hesaplandığı satırlar                  
                    cutTime = time - firstTime;
                    score = 100 / cutTime;
                    firstTime = cutTime;

                    if (cutTime > 100)  
                    {
                        a += 0; 
                        score_label.Text = a.ToString();
                        
                    }
                    else 
                    {   
                        a += score;
                        score_label.Text = a.ToString();
                        
                    }
                    
                }
             }
            Bitmap bitmap = new Bitmap(500, 500);
                
                //Yılanın oyun alanından çıkması durumlarını kontrol edilen kod parçası
            if (x == 51 || y == 51 || x ==51 || y == 51)
            {
                timer_score.Stop();
                DosyaIslemi();             
                GameOver();
            }
            else if (x == 0 || y == 0 || x == 0 || y == 0)
            {
                timer_score.Stop();
                DosyaIslemi();
                GameOver();
            }
            else
            {
                    //Yılanı oluşturan kod parçası
                for (int i = (x - 1) * 10; i < x * 10; i++)
                {
                    for (int j = (y - 1) * 10; j < y * 10; j++)
                    {

                        if (x > 510 || y > 510)
                        {                           
                            i = 0;
                            j = 0;
                        }
                        else
                        {
                            bitmap.SetPixel(i, j, Color.Green);
                        }                       
                    }
                }
            }

                //Kuyruğun oluşması için gerekli kod parçası
            if (tales.Count != 1)
            {
                for (int k = 0; k < tales.Count; k++)
                {
                    for (int i = (tales[k].x - 1) * 10; i < tales[k].x * 10; i++)
                    {
                        for (int j = (tales[k].y - 1) * 10; j < tales[k].y * 10; j++)
                        {
                            if (i < 1 || i > 510 || j < 1 || j > 510 )
                            {                              
                                i = 0;
                                j = 0;
                            }
                            else
                            {
                                bitmap.SetPixel(i, j, Color.Green);

                            }                          
                        }
                    }
                }
            }

            tales[0] = new Location(x, y);
            for (int i = tales.Count - 1; i > 0; i--)
            {
                tales[i] = tales[i - 1];
            }

            // Yemek gittiginde yeniden olusan yem 
            if (food.x == -1)
            {
                food = new Location(random.Next(1, 51), random.Next(1, 51));
            }

            for (int i = (food.x - 1) * 10; i < food.x * 10; i++)
            {
                for (int j = (food.y - 1) * 10; j < food.y * 10; j++)
                {
                    bitmap.SetPixel(i, j, Color.Red);                    
                }
            }
            game.Image = bitmap;
        }

        private void DosyaIslemi()
        {
            StreamWriter yazici = File.AppendText(skor_dosya_yolu);
            yazici.WriteLine(kullanici + " " + timer_label.Text.ToString() + "  " +"skor"+ score_label.Text.ToString());
            yazici.Close();
        }

            //Oyun süresinin hesaplandığı kod parçası
        private void timer1_Tick(object sender, EventArgs e)    
        {
            time++;
            int a = 0;

            if (time >= 59)
            {
                a++;
                
            }
            
        }

            //Oyun süresini hesaplayan kod parçası
        private void timer_time_Tick(object sender, EventArgs e)
        {

            timeGame++;
            second= timeGame;
            timer_label.Text = ((Convert.ToString(minute) + " : ") + (Convert.ToString(second)));
            if (second == 59)
            {
                timeGame = 0; 
                second = 0;
                minute = minute + 1;
            }

        }
            
            //Dosyalama işlemlerinin yapıldığı kod parçası
        private void btnSkorlarıGörüntüle_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", skor_dosya_yolu);
        }

        private void btnYardım_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", yardim_dosya_yolu);
        }

        private void btnKişiyikaydet_Click(object sender, EventArgs e)
        {
            kullanici = txtKişileriKaydet.Text.ToString();
            MessageBox.Show("Kişi Kaydedildi!");
        }
            
            //Zor konumlarda çıkan yemler için skorun hesaplandığı kod parçası(+10 puan)
        public void Score()
        {
            if (cutTime != 0)
            {
                
                cutTime = time - firstTime;
                score = (100 / cutTime) + 10;
                firstTime = cutTime;
                if (cutTime > 10)
                {
                    a += 0;
                    score_label.Text = a.ToString();                   
                }
                else
                {
                    a += score;
                    score_label.Text = a.ToString();                    
                }
                
            }

        }
            //Oyun bittiğinde kullanıcıya tekrar oynamak isteyip istemediğini soran kod parçası
        public void GameOver()
        {
            DialogResult dialogResult = new DialogResult();
            dialogResult = MessageBox.Show("Yeniden oynamak ister misiniz ? ", "Kaybettiniz", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                restart();
            }
            else
            {
                Environment.Exit(0);
            }
        }
            //Oyunun tekrar başlamasını sağlayan fonksiyon
        public void restart()
        {
            Form1 startForm = new Form1();
            this.Hide();
            startForm.Show();

            Application.Restart();
            
        }
    }
    public class Location   //Yem ve kuyruğun konumunu tutan class
    {
        public int x, y;

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;

        }
    }
}
