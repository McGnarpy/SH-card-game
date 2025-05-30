using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Assignment7
{
    /// <summary>
    /// Interaction logic for ViewDeckWindow.xaml
    /// </summary>
    public partial class ViewDeckWindow : Window
    {
        private readonly List<Card> deck;
        private int currentShow = -1;
        /*
         * constructors
         */
        internal ViewDeckWindow(Deck deck)
        {
            InitializeComponent();
            this.deck = deck.Cards;
            AddCard(deck.Cards);
        }
        internal ViewDeckWindow(List<Card> deck)
        {
            InitializeComponent();
            this.deck = deck;
            AddCard(deck);
        }


        /// <summary>
        /// adds all the cards to the display area
        /// 
        /// uses a grid to allow the cards to seemingly overlap but since this is not rally the intended usecase for it
        /// the code is a bit complicated, or not really but more so thaan one would have hoped for, makes an image with source of the card info
        /// puts the image in a column and row depending on index of added card in relation to total amount of cards
        /// </summary>
        /// <param name="deck"></param>
        private void AddCard(List<Card> deck)
        {
            if (deck.Count <= 0)
            {
                return;
            }
            currentShow = 0;

            ContentGrid.Opacity = 1;
            int i = 0;
            foreach (Card card in deck)
            {
                ColumnDefinition column = new();
                Image image = new()
                {
                    Source = new BitmapImage(new Uri("CardImgs/" + card.Rank.ToString() + "-" + card.Suit.ToString() + ".png", UriKind.Relative)),
                    Width = 75
                };
                if (i <= (deck.Count / 2))
                {
                    ContentGrid.ColumnDefinitions.Add(column);
                }
                Grid.SetColumn(image, i % ((deck.Count / 2) + 1));
                Grid.SetColumnSpan(image, 2);
                Grid.SetRow(image, (i <= (deck.Count / 2) ? 0 : 1));
                i++;
                ContentGrid.Children.Add(image);
            }
            UpdateDisplay(currentShow);
        }
        /// <summary>
        /// adjusts the current show variable decramenting by one, unless at lowest value in which case set to max value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            currentShow--;
            if (currentShow == -1)
            {
                currentShow = deck.Count - 1;
            }


            UpdateDisplay(currentShow);
        }
        /// <summary>
        /// adjust the current show variable incramenting by one, unless at highest value in which case set to lowest value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (currentShow == -1)
            {
                return;
            }
            currentShow++;
            currentShow %= deck.Count;

            UpdateDisplay(currentShow);
        }
        /// <summary>
        /// uses current show variable to show a card from the deck in higher detail
        /// </summary>
        /// <param name="index"></param>
        private void UpdateDisplay(int index)
        {
            DisplayImg.Source = new BitmapImage(new Uri("CardImgs/" + deck[index].Rank.ToString() + "-" + deck[index].Suit.ToString() + ".png", UriKind.Relative));
        }
    }
}
