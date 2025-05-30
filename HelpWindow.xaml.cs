using System.Windows;
using System.Windows.Media.Imaging;

namespace Assignment7
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            image.Source = new BitmapImage(new Uri("/OtherImgs/ShitHeadHelpLayoutImage.png", UriKind.RelativeOrAbsolute));
        }
    }
}
