using System.Windows;

namespace Assignment7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Deck deck;
        OptionsWindow optionsWindow;
        public MainWindow()
        {
            InitializeComponent();
            deck = new(0, DeckEditor.GenerateDefaultDeck());
            optionsWindow = new OptionsWindow();

        }
        /// <summary>
        /// opens the window for the actual game, tries to use options but incase the usser closed it by accident it creates a new option with 
        /// default values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                deck.Rnd = new(optionsWindow.Seed);
                ShitHeadWindow gameWindow = new(deck, optionsWindow);
                gameWindow.Show();
            }
            catch
            {
                deck.Rnd = new(new OptionsWindow().Seed);
                ShitHeadWindow gameWindow = new(deck, new OptionsWindow());
                gameWindow.Show();
            }

        }
        /// <summary>
        /// opens the deck view window which lets you see all the cards and also on the top look aat them one by one, all made by me
        /// quality is iffy but yk its not an art assignment 
        /// 
        /// cards here are not sorted, they apear in which ever order they were last integrated into the deck, meaning after a game the cards
        /// will remain shuffled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeck_Click(object sender, RoutedEventArgs e)
        {
            ViewDeckWindow viewDeckWindow = new(deck);
            viewDeckWindow.Show();
        }
        /// <summary>
        /// opens options window, if it previously was losed by accident creates a new options window with default values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                optionsWindow.Show();
            }
            catch
            {
                optionsWindow = new();
                optionsWindow.Show();
            }
        }
        /*
         * misc
         */
        /// <summary>
        /// tries to close all child processies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            optionsWindow.Close();
        }
    }
}