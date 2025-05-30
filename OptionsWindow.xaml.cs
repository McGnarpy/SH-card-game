using System.Windows;
using System.Windows.Controls;

namespace Assignment7
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private bool allSeeingEye;
        private int shownCardsAmount, hidenCardsAmount, minHandAmount;
        private int numOfOpponents;
        private int seed;

        internal bool AllSeeingEye { get { return allSeeingEye; } }
        internal int ShownCardsAmount { get { return shownCardsAmount; } }
        internal int HidenCardsAmount { get { return hidenCardsAmount; } }
        internal int MinHandAmount { get { return minHandAmount; } }
        internal int NumOfOpponents { get { return numOfOpponents; } }
        internal int Seed { get { return seed; } }

        /// <summary>
        /// sets number of opponents variable to 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void one_Click(object sender, RoutedEventArgs e)
        {
            numOfOpponents = 1;
        }
        /// <summary>
        /// sets number of opponents varibale to 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void two_Click(object sender, RoutedEventArgs e)
        {
            numOfOpponents = 2;
        }
        /// <summary>
        /// sets number of oponnts variable to 3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void three_Click(object sender, RoutedEventArgs e)
        {
            numOfOpponents = 3;
        }
        /// <summary>
        /// enebales or dissables the all seeing eye, it lets the player see all hiden cards and unlocks the servaliance button ingame 
        /// that lets you see all the opponents cards even those hidden even to them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbxAllSeeingEye_Click(object sender, RoutedEventArgs e)
        {
            allSeeingEye = ckbxAllSeeingEye.IsEnabled;
        }
        /// <summary>
        /// sets variable value for slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void sldrHandCards_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            minHandAmount = (int)sldrHandCards.Value;
        }
        /// <summary>
        /// sets variable value for slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void sldrHidenCards_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            hidenCardsAmount = (int)sldrHidenCards.Value;
        }
        /// <summary>
        /// sets variable value for slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldrShownCards_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            shownCardsAmount = (int)sldrShownCards.Value;
        }
        /// <summary>
        /// updates the seed variable, the seed is used as a random number generator seed
        /// 
        /// if inputed string is not of type only numbers then it will be parsed as a char array where eqach chards unicode value
        /// is summed up, the sum being the seed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txbSeed.Text, out seed))
            {
                return;
            }

            char[] chars = txbSeed.Text.ToCharArray();
            int tally = 0;
            foreach (char c in chars)
            {
                tally += (int)c;
            }
            seed = tally;
        }
        /// <summary>
        /// closes window without disposing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        /// <summary>
        /// sets default values to gui elements as well as variables 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            one.IsChecked = false;
            two.IsChecked = true;
            three.IsChecked = false;
            ckbxAllSeeingEye.IsChecked = false;
            sldrShownCards.Value = 3;
            sldrHidenCards.Value = 3;
            sldrHandCards.Value = 3;

            numOfOpponents = 2;
            allSeeingEye = false;
            shownCardsAmount = 3;
            hidenCardsAmount = 3;
            minHandAmount = 3;
            seed = DateTime.Now.Millisecond * DateTime.Now.Microsecond + DateTime.Now.Second;
        }
        /// <summary>
        /// constructor, uses btnReset to set default values
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
            btnReset.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
