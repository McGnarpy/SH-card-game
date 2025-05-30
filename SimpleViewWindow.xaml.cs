using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Assignment7
{
    /// <summary>
    /// Interaction logic for SimpleViewWindow.xaml
    /// </summary>
    public partial class SimpleViewWindow : Window
    {
        private readonly List<Card> list;
        private readonly bool revealCards;
        internal SimpleViewWindow(List<Card> cardList, string name = "unset_name", bool revealed = true, int width = 800, int height = 450, string msg = " ")
        {
            InitializeComponent();
            list = cardList;
            this.Title = name;
            this.revealCards = revealed;
            this.Width = width;
            this.Height = height;
            this.lblHiddenMessage.Content = msg;
            Populate();
        }
        /// <summary>
        /// adds the cards to the wrap panel,
        /// face side up normally but if ´revealed cards is false then only ever show the backside
        /// </summary>
        private void Populate()
        {
            foreach (Card card in list)
            {
                string path = "/CardImgs/BACKSIDE.png";
                if (revealCards)
                {
                    path = "/CardImgs/" + card.Rank.ToString() + "-" + card.Suit.ToString() + ".png";
                }

                Image image = new()
                {
                    Source = new BitmapImage(new Uri(path, UriKind.Relative)),
                    Name = card.Rank.ToString(),
                    Width = 65
                };
                this.DisplayPanel.Children.Add(image);
            }
        }
        /// <summary>
        /// updates the shown cards
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayPanel.Children.Clear();
            Populate();
        }
        /// <summary>
        /// a internal handel for updating the display, other calsses use this methods in their event triggers to update the displays
        /// </summary>
        internal void EventTrigger()
        {
            btnReload.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
