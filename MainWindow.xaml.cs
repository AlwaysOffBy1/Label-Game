using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Animation_WPF
{
    public partial class MainWindow : Window
    {
        public int animationTime = 1;
        public System.Windows.Media.Animation.DoubleAnimation barSizeAnimate = new System.Windows.Media.Animation.DoubleAnimation();
        public Color topBarColor, midBarColor, botBarColor;
        public Random rand = new Random();
        public TimeSpan animationDelay;
        public int topBarM, midBarM, botBarM;
        public int topBarV = 0; public int midBarV = 0; public int botBarV = 0;
        public int timesClicked = 0;
        public double l1multiplier = 1;
        public int animationDelayMilli = 300;
        public int mostRecentVictory = 0;
        public bool updatedl1 = true;
        public bool stopTimer = false;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            topBarColor = Color.FromArgb(255, 255, 0, 0);
            midBarColor = Color.FromArgb(255, 0, 255, 0);
            botBarColor = Color.FromArgb(255, 0, 0, 255);

            animationDelay = new TimeSpan(0, 0, 0, 0, animationDelayMilli);
            barSizeAnimate.AccelerationRatio = 0;
            barSizeAnimate.DecelerationRatio = 1;
            barSizeAnimate.Duration = new Duration(new TimeSpan(0, 0, 0, animationTime));
        }
        private double CalculateTopBarVal()
        {
            if (topBarM > 255) { topBarM = 255; }
            return rand.Next(topBarM, 256);
        }
        private int CalculateMidBarVal()
        {
            if(timesClicked * 2 >= 255) { timesClicked = 127; }
            return rand.Next(timesClicked * 2, 256);
        }
        private int CalculateBotBarVal()
        {
            return rand.Next(botBarM, 256);
        }

        private int CalculateTopBarM(double currValue)
        {
            return Convert.ToInt32(currValue * (.75 * l1multiplier));
        }
        private int CalculateBotBarM()
        {
            int tmp = botBarM;
            bool breaker = rand.Next(0, 101) < 10;
            if (!breaker) { tmp += rand.Next(5, 10); }
            if (breaker) { tmp -= rand.Next(10, 20); }
            if (tmp < 0) { tmp = 0; }
            if (tmp > 230) { tmp /= rand.Next(1, 5); }
            return tmp;
        }
        private void Victory(Label winner)
        {
            stopTimer = true;
            if (winner == topBar) { MessageBox.Show("Label 1 WON!!!"); topBarV++; mostRecentVictory = 1; this.topBarMin.Content = topBarV; }
            if (winner == midBar) { MessageBox.Show("Label 2 WON!!!"); midBarV++; mostRecentVictory = 2; this.midBarMin.Content = midBarV; }
            if (winner == botBar) { MessageBox.Show("Label 3 WON!!!"); botBarV++; mostRecentVictory = 3; this.botBarMin.Content = botBarV; }
            updatedl1 = false;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            stopTimer = true;
            RunAnimation();
        }
        private void RunAnimation()
        {
            timesClicked++;
            this.labelTurn.Content = "Turn " + timesClicked;

            double topBarVal = CalculateTopBarVal();
            double midBarVal = CalculateMidBarVal();
            double botBarVal = CalculateBotBarVal();
            topBarM = CalculateTopBarM(topBarVal);
            midBarM = timesClicked * 2;
            botBarM = CalculateBotBarM();

            if (topBarVal == 255) { Victory(topBar); l1multiplier += .08; }
            if (midBarVal == 255) { Victory(midBar); l1multiplier += .04; }
            if (botBarVal == 255) { Victory(botBar); l1multiplier += .04; }
            //topBar
            barSizeAnimate.BeginTime = new TimeSpan(0);
            barSizeAnimate.From = topBar.Width;
            barSizeAnimate.To = topBarVal;
            this.topBar.BeginAnimation(Label.WidthProperty, barSizeAnimate);
            //midBar
            barSizeAnimate.BeginTime += animationDelay;
            barSizeAnimate.From = midBar.Width;
            barSizeAnimate.To = midBarVal;
            this.midBar.BeginAnimation(Label.WidthProperty, barSizeAnimate);
            //botBar
            barSizeAnimate.BeginTime += animationDelay;
            barSizeAnimate.From = botBar.Width;
            barSizeAnimate.To = botBarVal;
            this.botBar.BeginAnimation(Label.WidthProperty, barSizeAnimate);
            //topBarMin
            barSizeAnimate.BeginTime = new TimeSpan(0, 0, Convert.ToInt32(animationTime / barSizeAnimate.SpeedRatio));
            barSizeAnimate.From = topBarMin.Width;
            barSizeAnimate.To = topBarM;
            this.topBarMin.BeginAnimation(Label.WidthProperty, barSizeAnimate);
            //botBarMin
            barSizeAnimate.From = botBarMin.Width;
            barSizeAnimate.To = botBarM;
            this.botBarMin.BeginAnimation(Label.WidthProperty, barSizeAnimate);
            //topBar ON VICTORY
            if (!updatedl1)
            {
                updatedl1 = true;
                barSizeAnimate.BeginTime += animationDelay + new TimeSpan(0, 0, Convert.ToInt32(animationTime));
                barSizeAnimate.From = topBarMin.Width;
                barSizeAnimate.To = CalculateTopBarVal();
                this.topBarMin.BeginAnimation(Label.WidthProperty, barSizeAnimate);
            }

        }

        #region "Size Changed"
        private void topBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            topBar.Content = Convert.ToInt32(topBar.Width);
            this.topBar.Background = new SolidColorBrush(Color.FromArgb(255, (byte)topBar.Width, 0,0));
            UpdateBGColor();
        }
        private void midBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            midBar.Content = Convert.ToInt32(midBar.Width);
            this.midBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(midBar.Width), 0));
            this.midBarMin.Width = timesClicked*2;
            UpdateBGColor();
        }



        private void botBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            botBar.Content = Convert.ToInt32(botBar.Width);
            this.botBar.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, (byte)botBar.Width));
            UpdateBGColor();
        }
        private void UpdateBGColor()
        {
            this.MainGrid.Background = new SolidColorBrush(Color.FromArgb(255, (byte)topBar.Width, (byte)midBar.Width, (byte)botBar.Width));
        }
        #endregion

        private void buttonGoToVictory_Click(object sender, RoutedEventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,0,Convert.ToInt32(2000/ slider.Value));
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            RunAnimation();
            if (stopTimer) { timer.Stop(); stopTimer = false;}
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBoxSpeed.Text = Math.Round(this.slider.Value,2).ToString();
            int val = Convert.ToInt32(this.slider.Value);
            if(val == 0) { val = 1; }
            animationDelay = new TimeSpan(0, 0, 0, 0, animationDelayMilli / val);
            timer.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(2000 / slider.Value));
        }
        private void textBoxSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            String strVal = textBoxSpeed.Text;
            double dblVal = 0;

            if(strVal.Equals("")) { return; }
            if(strVal[strVal.Length-1].Equals(".")) { return; }
            try
            {
                dblVal = Convert.ToDouble(textBoxSpeed.Text);
            }
            catch(Exception err)
            {
                return;
            }
            if (this.slider != null)
            {
                this.slider.Value = dblVal;
                barSizeAnimate.SpeedRatio = dblVal;
            }
        }
    }
}
