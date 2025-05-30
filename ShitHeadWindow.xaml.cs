using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Assignment7
{
    /// <summary>
    /// Interaction logic for ShitHeadWindow.xaml
    /// </summary>
    public partial class ShitHeadWindow : Window
    {
        private Rules rules;
        private OptionsWindow options;
        private SimpleViewWindow? viewWindowDraw;
        private SimpleViewWindow? viewWindowCard;
        private SimpleViewWindow? viewWindowDisc;
        private List<int> highlightedList1;
        private List<int> highlightedList2;
        private List<int> highlightedList3;
        private bool startOfGame;
        /// <summary>
        /// the gui integration of the rules calss, holds some logic on a higher level than the rules sadly but its a hard thing to avoid since 
        /// anything happening here is not accessable in rules since this form hold the rules and not the other way around
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="options"></param>
        internal ShitHeadWindow(Deck deck, OptionsWindow options)
        {
            InitializeComponent();
            rules = new(deck, options);
            this.options = options;
            highlightedList1 = [];
            highlightedList2 = [];
            highlightedList3 = [];

            //event listners
            rules.Board.ShuffleChange += Board_ShuffleChange;
            rules.CardpileChange += CardpileChange;
            rules.DiscardChange += DiscardChange;
            rules.DrawpileChange += DrawpileChange;

            //graphical events
            rules.ENDOFGAME += ENDOFGAME_Trigger;
            rules.Playerhand.OnPlayEvent += Playerhand_OnPlayEvent;
            rules.OpponentPlay += OpponentPlayEvent;

            //logic
            rules.Start();

            // start game gui set up
            RefreshGui();
            if (options.AllSeeingEye)
            {
                TheAllSeeingEye();
            }
            startOfGame = true;
            btnSwap.IsEnabled = true;
            ShownCardsSpace.IsEnabled = true;
        }
        /// <summary>
        /// does nothing right now, was thinking of adding some animation and sound effect but i cant get the animation to work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpponentPlayEvent(object? sender, Card e)
        {

        }
        /// <summary>
        /// does nothing right now, was thinking of adding some animation and sound effect but i cant get the animation to work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Playerhand_OnPlayEvent(object? sender, Card e)
        {
        }
        /// <summary>
        /// dissable th action buttons and then shows a btn saying either u win or u lose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ENDOFGAME_Trigger(object? sender, EventArgs e)
        {
            foreach (Button b in wrpnlActionButtons.Children)
            {
                b.IsEnabled = false;
            }

            Button endGameMsg = new()
            {
                Content = "aaa",
                FontSize = 99,
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Colors.DarkGreen),
                ToolTip = "exit back to main menue"
            };
            switch (rules.Playerhand.GameState)
            {
                case GameState.DEFEAT:
                    endGameMsg.Content = "You Lose";
                    break;
                case GameState.VICTORY:
                    endGameMsg.Content = "You Win!";
                    break;
            }
            endGameMsg.AddHandler(Button.ClickEvent, new RoutedEventHandler(exit));

            Grid.SetZIndex(endGameMsg, 10);
            Grid.SetColumnSpan(endGameMsg, 6);
            Grid.SetRowSpan(endGameMsg, 5);
            main.Children.Add(endGameMsg);
        }
        /// <summary>
        /// closes the window, when the end of the game button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit(object? sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// updates the drawpile and the simple view window of it as well as the content of the button that opens that window, the logic is
        /// an evolutionary byproduct and its to risky to clean it now (wpf is super weird sometimes like seemingly unexplainable things
        /// go wrong for the smallest and seemingly most unsignificant things, so ive learned to be casious especially with something this complex)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawpileChange(object? sender, EventArgs e)
        {
            if (Drawpile.Children.Count! < 0)
            {
                return;
            }
            if (Drawpile.Children.Count - 1 == rules.Board.Drawpile.Cards.Count)
            {
                Drawpile.Children.RemoveAt(Drawpile.Children.Count - 1);
            }
            viewWindowDraw?.EventTrigger();
            RefreshOpponent();

            btnDrawpileClick.Content = "deck : " + rules.Board.Drawpile.Cards.Count.ToString();
        }
        /// <summary>
        /// updates the cardpiles canvas, the carpile simlpe view window, and the content of the button that opens that window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CardpileChange(object? sender, EventArgs e)
        {
            RefreshPile(rules.Board.Cardpile, Cardpile, true, 0, 0, false, true);
            viewWindowCard?.EventTrigger();
            RefreshOpponent();

            btnCardpileClick.Content = "card pile : " + rules.Board.Cardpile.Count.ToString();
        }
        /// <summary>
        /// updates the discardpiles canvas as well as the contents of the button that opens its simple view window and that window n it self
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscardChange(object? sender, EventArgs e)
        {
            RefreshPile(rules.Board.Discard, Discardpile, true, 0, 0, false, true);
            viewWindowDisc?.EventTrigger();

            btnDiscardpileClick.Content = "discard pile : " + rules.Board.Discard.Count.ToString();
        }
        /// <summary>
        /// updates the drawpile (not used yet)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Board_ShuffleChange(object? sender, EventArgs e)
        {
            RefreshPile(rules.Board.Drawpile.Cards, Drawpile);
        }
        /*
         * 
         * the left side buttons
         * 
         */
        /// <summary>
        /// reconstetutes the slected indecies list in playerhand from highlighted cards using indecSelected method, then uses turn which
        /// looks att selected indecies and determins if is an allowed play adn if it is turn also removes it from the active cardspace 
        /// and adds it to the cardpile in rules board
        /// 
        /// if a failed play occurs at the hidden cards stage of the game a click on the pick up pile button will be executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPlay(object sender, RoutedEventArgs e)
        {
            indexSelected();

            AllowOutcome turn = rules.Turn();
            if (turn != AllowOutcome.ALLOWED)
            {
                if (rules.Playerhand.GameState == GameState.HIDEN_CARDS && turn == AllowOutcome.FORBIDE)
                {
                    btnPickUpCards.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
            else
            {
                rules.NextTurn(this.btnPlay);
            }
            rules.Playerhand.ClearSelect();
            startOfGame = false;
            RefreshPlayer();
        }
        /// <summary>
        /// tries to play the top most card of the draw pile (if its still there) 
        /// the card is added to the player hand at index 0 and then immeaditly played using the normal play method "turn"
        /// 
        /// if turn decides this move to be illeagal ie that its lower than the topmost card in cardpile and its not a 2 or 10,
        /// then it will execute a click on the pick up pile button below
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#pragma warning disable CS8604 // Possible null reference argument.
        private void btnTryTopDraw_Click(object sender, RoutedEventArgs e)
        {
            if (rules.Board.TopDraw(out Card? card))
            {
                rules.Playerhand.ClearSelect();
                rules.Playerhand.SelectIdex(0);
                switch (rules.Playerhand.GameState)
                {
                    case GameState.TABEL_CARDS:
                        rules.Playerhand.TopTable.Insert(0, card);
                        break;
                    case GameState.HIDEN_CARDS:
                        rules.Playerhand.BottomTable.Insert(0, card);
                        break;
                    default:
                        rules.Playerhand.HandP.Insert(0, card);
                        break;
                }
                rules.Board.Drawpile.Cards.Remove(card);

                switch (rules.Turn())
                {
                    default:
                        rules.NextTurn(this.btnPlay);
                        break;
                    case AllowOutcome.FORBIDE:
                        btnPickUpCards.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        break;

                }

                DrawpileChange(null, e);
                CardpileChange(null, e);
                RefreshPile(rules.Board.Drawpile.Cards, Drawpile);
                rules.Playerhand.ClearSelect();
                startOfGame = false;
                RefreshPlayer();
            }
        }
#pragma warning restore CS8604 // Possible null reference argument.
        /// <summary>
        /// picks up the card pile and adds it to the players hand, then passes the turn to the opponent(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPickUpCards_Click(object sender, RoutedEventArgs e)
        {
            rules.Playerhand.ClearSelect();
            rules.Playerhand.HandP.AddRange(rules.Board.PickupPile());
            rules.Playerhand.Sort(rules.Playerhand.HandP);
            startOfGame = false;
            RefreshPlayer();
            RefreshPile(rules.Board.Cardpile, Cardpile);
            rules.NextTurn(btnPlay);
        }
        /// <summary>
        /// lets you swap selected card from hand with selected card from shown cards list, button is only available before any cards have been played
        /// only one card from each hand can be selected for this to work no less either, if it doesnt work then nothing will happen (the if statment)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSwap_Click(object sender, RoutedEventArgs e)
        {
            List<int> handCardsIndex, shownCardsIndex, hiddenCardsIndex;
            AllSpaceSelectedIndexer(out handCardsIndex, out shownCardsIndex, out hiddenCardsIndex);
            if (handCardsIndex.Count != 1 || shownCardsIndex.Count != 1)
            {
                return;
            }
            (rules.Playerhand.HandP[handCardsIndex[0]], rules.Playerhand.TopTable[shownCardsIndex[0]]) = (rules.Playerhand.TopTable[shownCardsIndex[0]], rules.Playerhand.HandP[handCardsIndex[0]]);
            RefreshPlayer();

        }
        /// <summary>
        /// opens a help window tha describes the rules and the layout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new();
            helpWindow.Show();
        }
        /*
         * 
         *  pressing the cards / selecting and deselecting cards
         * 
         */

        /// <summary>
        /// the event tied to the press of the card ui-element button, when the botton is clicked its added or if it already is added removed 
        /// from a list and the oppacity and margin thickness is changed
        /// 
        /// both margin and opacity values were chosen arbutraruly since i thought they looked nice-ish, i know this application is lacking 
        /// in the looks department XD but still
        /// 
        /// the use of the seperate higlight lists are for the sake of the swap cards section, earlier i had it determined by gamestate but since
        /// to be able to swap shown cards with hand cards you need to be able to select both cards from the shwon and hand lists.
        /// 
        /// the switcch case does the same thing in all cases basically just the list name is diffrent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Button cardButton = (Button)sender;
            WrapPanel parent = (WrapPanel)cardButton.Parent;

            int index = parent.Children.IndexOf(cardButton);
            switch (parent.Name)
            {
                case "HandSpace":
                    if (highlightedList1.Contains(index))
                    {
                        highlightedList1.Remove(index);
                    }
                    else
                    {
                        highlightedList1.Add(index);
                    }
                    if (!highlightedList1.Contains(index))
                    {
                        cardButton.Opacity = 1;
                        cardButton.Margin = new Thickness(0);
                    }
                    if (highlightedList1.Contains(index))
                    {
                        cardButton.Opacity = 0.8;
                        cardButton.Margin = new Thickness(4);
                    }
                    break;
                case "ShownCardsSpace":
                    if (highlightedList2.Contains(index))
                    {
                        highlightedList2.Remove(index);
                    }
                    else
                    {
                        highlightedList2.Add(index);
                    }
                    if (!highlightedList2.Contains(index))
                    {
                        cardButton.Opacity = 1;
                        cardButton.Margin = new Thickness(0);
                    }
                    if (highlightedList2.Contains(index))
                    {
                        cardButton.Opacity = 0.8;
                        cardButton.Margin = new Thickness(4);
                    }
                    break;
                case "HiddenCardsSpace":
                    if (highlightedList3.Contains(index))
                    {
                        highlightedList3.Remove(index);
                    }
                    else
                    {
                        highlightedList3.Add(index);
                    }
                    if (!highlightedList3.Contains(index))
                    {
                        cardButton.Opacity = 1;
                        cardButton.Margin = new Thickness(0);
                    }
                    if (highlightedList3.Contains(index))
                    {
                        cardButton.Opacity = 0.8;
                        cardButton.Margin = new Thickness(4);
                    }
                    break;
            }


        }


        /*
         * 
         * 
         *  refresh methods
         * 
         */
        /// <summary>
        /// this method executes most refresh methods, used in constructor 
        /// </summary>
        private void RefreshGui()
        {
            GenOpponentGraphics();

            RefreshPlayer();
            RefreshOpponent();
            RefreshPile(rules.Board.Drawpile.Cards, Drawpile);
        }
        /// <summary>
        /// refreshes the player gui, clearing each wrap panel and then re adding all the elemtns, much akin to how the gpu does things
        /// uses the card border method in the create visual cards section
        /// 
        /// after that some logic is done using the player gamestate and board state to enable or disable certain gui elemnts
        /// in this step i also force update the player gamestate since there were some issues in the past and this was a quick fix
        /// and it ended up sticking and now everything has grown around it and its too risky to remove it now that therere only a few hours 
        /// left if i am to follow the shcedule 
        /// 
        /// also uses logic based on the start of game boolean for the swap cards functionality switch off
        /// </summary>
        private void RefreshPlayer()
        {
            int borderWidth = 2;
            HiddenCardsSpace.Children.Clear();
            ShownCardsSpace.Children.Clear();
            HandSpace.Children.Clear();

            foreach (Card card in rules.Playerhand.BottomTable)
            {
                CardBorder(card, borderWidth, HiddenCardsSpace);
            }
            foreach (Card card in rules.Playerhand.TopTable)
            {
                CardBorder(card, borderWidth, ShownCardsSpace);
            }
            foreach (Card card in rules.Playerhand.HandP)
            {
                CardBorder(card, borderWidth, HandSpace);
            }

            if (rules.Playerhand.HandP.Count != 0)
            {
                HiddenCardsSpace.IsEnabled = false;
                ShownCardsSpace.IsEnabled = false;
                switch (rules.Board.Drawpile.Cards.Count == 0)
                {
                    case true:
                        rules.Playerhand.GameState = GameState.DRAWPILE_INACTIVE;
                        break;
                    case false:
                        rules.Playerhand.GameState = GameState.DRAWPILE_ACTIVE;
                        break;
                }
            }
            else if (rules.Playerhand.TopTable.Count != 0)
            {
                HiddenCardsSpace.IsEnabled = false;
                ShownCardsSpace.IsEnabled = true;
                rules.Playerhand.GameState = GameState.TABEL_CARDS;
            }
            else
            {
                HiddenCardsSpace.IsEnabled = true;
                ShownCardsSpace.IsEnabled = false;
                rules.Playerhand.GameState = GameState.HIDEN_CARDS;
            }
            if (startOfGame)
            {
                btnSwap.IsEnabled = true;
                ShownCardsSpace.IsEnabled = true;
            }
            else
            {
                btnSwap.IsEnabled = false;
            }
        }
        /// <summary>
        /// refreshes the opponent hand gui, locates the stack pannel for hiden shown and hand cards then updates them from the data in rules.opponents
        /// since were using the "as" thing it throws possible null refrance at every turn which is why ive supressed it, the warning is if
        /// it tries to say take the i index child of opponentspace and it *isnt* a border then none of the following stuff will function.
        /// 
        /// since i designed the opponent space layout and without there being any way to modify it this error will never occur
        /// 
        /// this method was being a bit buggy earlier so i put it in a try statement just to be safe, the buggyness was from before the current 
        /// implamentation so im not sure if it still is, regardless if an opponent skips this for one turn its not likely to destroy the user
        /// expirience so to say its not a pivital functionality, and id rather not ballon the code even more to make sure it never fails
        /// </summary>
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        private void RefreshOpponent()
        {
            for (int i = 0; i < rules.Opponents.Count; i++)
            {
                try
                {
                    var parent = OpponentSpace.Children[i] as Border;
                    var grid = parent.Child as Grid;
                    var hidden = grid.Children[0] as StackPanel;
                    var shown = grid.Children[1] as StackPanel;
                    var hand = grid.Children[2] as StackPanel;

                    hidden.Children.Clear();
                    foreach (Card c in rules.Opponents[i].Hand.BottomTable)
                    {
                        Image image = new()
                        {
                            Source = new BitmapImage(new Uri("/CardImgs/BACKSIDE.png", UriKind.Relative)),
                            ToolTip = "unkown card"
                        };
                        hidden.Children.Add(image);
                    }
                    shown.Children.Clear();
                    foreach (Card c in rules.Opponents[i].Hand.TopTable)
                    {
                        Image image = new()
                        {
                            Source = new BitmapImage(new Uri("/CardImgs/" + c.Rank.ToString() + "-" + c.Suit.ToString() + ".png", UriKind.Relative)),
                            ToolTip = ((int)c.Rank + 2).ToString()
                        };
                        shown.Children.Add(image);
                    }
                    hand.Children.Clear();
                    foreach (Card c in rules.Opponents[i].Hand.HandP)
                    {
                        Image image = new()
                        {
                            Source = new BitmapImage(new Uri("/CardImgs/BACKSIDE.png", UriKind.Relative)),
                            ToolTip = "unkown card"

                        };
                        hand.Children.Add(image);
                    }

                }
                catch { }
            }
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        /// <summary>
        /// refreshes the visual piles in the type canvas format, the cards are images and the canvas is used to give an offset to each card making 
        /// the piles more appealing and easier to gauge the contents at a glance
        /// 
        /// this method uses a lot of optional parameters most of which are past down to the lower levels of methods,
        /// this method uses the buttonless card branch of the generate vissual card methods 
        /// cards is the list of cards to be displayed,
        /// canvas is the location where they will be displayed,
        /// revealed determins if the cards are backside up or not,
        /// bottom offset determins the distance between the bottom of the canvas and the bottom of the card in pixels,
        /// left offset is like bottom offest but for the left side instead,
        /// randomize makes the offsets random if true,
        /// card specific offset makes thee offset deterministic depending on card rank and suit (used to avoid jitteryness in drawpile)
        /// 
        /// the last variable makes it possible in theory  to guess the top most cards in the drawpile from sigth alone, however this macanic
        /// is very obscure and without reading this the player is likely to never know, and hey if they do no skin off my back (: good for them
        /// 
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="canvas"></param>
        /// <param name="revealed"></param>
        /// <param name="bottomOffset"></param>
        /// <param name="leftOffset"></param>
        /// <param name="randomize"></param>
        /// <param name="cardSpecificOffset"></param>
        private void RefreshPile(List<Card> cards, Canvas canvas, bool revealed = false, int bottomOffset = 15, int leftOffset = 15, bool randomize = true, bool cardSpecificOffset = false)
        {
            canvas.Children.Clear();
            foreach (Card card in cards)
            {
                if (cardSpecificOffset)
                {
                    bottomOffset = (int)card.Rank * 2;
                    leftOffset = (int)card.Suit * 3;
                }
                PlaceCard(card, canvas.Name, revealed, bottomOffset, leftOffset, randomize);
            }
        }


        /*
         * 
         * 
         * visal card creation
         * 
         * 
         */

        /// <summary>
        /// Places a card into desired player hand space panel, uses the get rimed card method to generate a buttonless ui-element with
        /// the right picture 
        /// 
        /// card give the info for what image to use,
        /// pile is the destination for the card, handed in as a string to avoid compiler weirdness,
        /// revealed determins if the card is shown backside up or not,
        /// bottom offset determins the distance between the bottom of the canvas and image in pixels,
        /// left offset is like bottom offset but for the left instead,
        /// randomize randomizes both offsets if true
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <param name="pile"></param>
        /// <param name="revealed"></param>
        /// <param name="bottomOffset"></param>
        /// <param name="leftOffset"></param>
        /// <param name="randomize"></param>
        private void PlaceCard(Card card, string pile, bool revealed = false, int bottomOffset = 15, int leftOffset = 15, bool randomize = true)
        {
            if (randomize)
            {
                bottomOffset = rules.Board.Drawpile.Rnd.Next(0, bottomOffset);
                leftOffset = rules.Board.Drawpile.Rnd.Next(0, leftOffset);
            }
            var uiElement = GetRimedCard(card, revealed, (pile == "Cardpile") ? 130 : 95);
            Canvas.SetBottom(uiElement, bottomOffset);
            Canvas.SetLeft(uiElement, leftOffset);
            switch (pile)
            {
                case "Cardpile":
                    uiElement.ToolTip = ((int)card.Rank + 2).ToString();
                    this.Cardpile.Children.Add(uiElement);
                    break;
                case "Drawpile":
                    this.Drawpile.Children.Add(uiElement);
                    break;
                case "Discardpile":
                    uiElement.ToolTip = ((int)card.Rank + 2).ToString();
                    this.Discardpile.Children.Add(uiElement);
                    break;
            }
        }
        /// <summary>
        /// generates a buttonless card element with a border and some preedefined colors,  would be cool to allow color scheems in options
        /// would like to try that if i had more time
        /// 
        /// card gives image info for the image
        /// revelaed determins what side of the card to show
        /// size determins how large the whole element will be
        /// 
        /// the image control is set as a child of the border controll making it bound to stay inside of it
        /// </summary>
        /// <param name="card"></param>
        /// <param name="revealed"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Border GetRimedCard(Card card, bool revealed = false, int size = 95)
        {
            Color color = Colors.Transparent;
            Color borderColor = Colors.Black;
            string path = "/CardImgs/BACKSIDE.png";
            if (revealed) { path = "/CardImgs/" + card.Rank.ToString() + "-" + card.Suit.ToString() + ".png"; }
            Image image = new()
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
                Height = size,
            };
            Border border = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(color),
                BorderBrush = new SolidColorBrush(borderColor),
                BorderThickness = new Thickness(1, 1, 1, 1),
                Child = image,
                Height = size,
            };
            return border;
        }
        /// <summary>
        /// Generates and adds a button type card ui-element to to a wrappannel destination, ei hand showncards or hidden cards
        /// 
        /// card gives card image information as well as wether the ccard should be revealed or not
        /// borderwidth determins the width of the border of the border controll (has child image, and is dispalyed as content of button)
        /// destination is what wrappanel the card should be added to
        /// </summary>
        /// <param name="card"></param>
        /// <param name="borderWidth"></param>
        /// <param name="destination"></param>
        private void CardBorder(Card card, int borderWidth, WrapPanel destination)
        {
            Color color = Colors.Wheat;
            Color borderColor = Colors.Black;
            string path = "/CardImgs/BACKSIDE.png";
            if (card.Revealed) { path = "/CardImgs/" + card.Rank.ToString() + "-" + card.Suit.ToString() + ".png"; }
            Image image = new()
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
                Height = 90,
                ToolTip = ((int)card.Rank + 2).ToString(),
            };
            Border border = new()
            {

                Child = image,
                Height = 90,
                ToolTip = ((int)card.Rank + 2).ToString(),
            };
            Button button = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = border,
                Height = 90,
                IsTabStop = true,
                Background = new SolidColorBrush(color),
                BorderBrush = new SolidColorBrush(borderColor),
                BorderThickness = new Thickness(1, 1, 1, 1),
                ToolTip = ((int)card.Rank + 2).ToString(),
            };
            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button_Click));
            destination.Children.Add(button);
        }

        /*
         * 
         *  show card windows
         * 
         */


        /// <summary>
        /// all of these are seperate windows that open when ever the event tied button is pressed
        /// this one is for the drawpile making it so you can see what cards remain in the drawpile
        /// 
        /// the cards in the drawpile are face down unless the all seeing eye is activated at which point you can see them all
        /// this would likely constitute a cheat tho since the opponent ai is so predictable nothing much will change :p
        /// 
        /// good for debugging and generally for keeping track, i added a counter on the buttons content to show many too
        /// 
        /// updates the last opened window of this kind whenever a OnDrawpileChange event is triggered to refresh its contents, note that
        /// if you oppen several only the last one will be uppdated as far as my testing has shown and also if you open several and close the last
        /// one then none of them will be updated, these windows dont manipulate any of the data from the lists they display so opening, closing
        /// and so on will not break game no matter how many as long as computer resources allow ofcourse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawpileClick_Click(object sender, RoutedEventArgs e)
        {
            viewWindowDraw = new(rules.Board.Drawpile.Cards, "Drawpile", options.AllSeeingEye);
            viewWindowDraw.Show();
        }
        /// <summary>
        /// opens a window to display current cardpile, updates whenever the cardpile does. more info above
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CardpileClick_Click(object sender, RoutedEventArgs e)
        {
            viewWindowCard = new(rules.Board.Cardpile, "Card pile");
            viewWindowCard.Show();
        }
        /// <summary>
        /// opens a window to display current discardpile, updates whenever the discardpile does, more info above
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscardpileClick_Click(object sender, RoutedEventArgs e)
        {
            viewWindowDisc = new(rules.Board.Discard, "Discard pile");
            viewWindowDisc.Show();

        }
        /// <summary>
        /// oppens several windows displaying all the confidential information the opponents are holding, even that which they themselves dont 
        /// know yet
        /// 
        /// only available if the all seeing eye is checked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TheAllSeeingEye_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < options.NumOfOpponents; i++)
            {
                SimpleViewWindow oponentViewhand = new(rules.Opponents[i].Hand.HandP, "the hand of opponent " + i.ToString(), true, 400, 225, "[HAND: Manual Refresh Required]");
                SimpleViewWindow oponentViewshown = new(rules.Opponents[i].Hand.TopTable, "the shown cards of opponent " + i.ToString(), true, 400, 225, "[SHOWN_CARDS: Manual Refresh Required]");
                SimpleViewWindow oponentViewhidden = new(rules.Opponents[i].Hand.BottomTable, "the hidden cards of opponent " + i.ToString(), true, 400, 225, "[HIDEN_CARDS: Manual Refresh Required]");
                oponentViewhand.Show();
                oponentViewshown.Show();
                oponentViewhidden.Show();
            }

        }
        /// <summary>
        /// generates the controll for the all seeing eyes servailance 
        /// 
        /// only available if the all seeing eye is checked
        /// </summary>
        private void TheAllSeeingEye()
        {
            Button servaliance = new()
            {
                Width = 120,
                Height = 50,
                Content = "Servaliance",
                FontWeight = FontWeights.Bold,
                FontSize = 15,
                Background = new SolidColorBrush(Colors.LightSlateGray),
                ToolTip = "look at everything",
            };
            servaliance.AddHandler(Button.ClickEvent, new RoutedEventHandler(TheAllSeeingEye_Click));
            wrpnlActionButtons.Children.Add(servaliance);
        }


        /*
         * misc
         */

        /// <summary>
        /// attempts to close all child proceccies when this form is being closed, in this chian of events all cards will be returned to deck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            options?.Close();
            viewWindowCard?.Close();
            viewWindowDisc?.Close();
            viewWindowDraw?.Close();
            rules.CloseCascade();
        }
        /// <summary>
        /// utility method for giving all highlighted cards by index of respective cardspace, for a card to be highlighted means
        /// that the gui elements opacity is 0.8, an arbitrary number but it looked nice in my opinion
        /// 
        /// these lists of indecies are handed as out parameters 
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="shown"></param>
        /// <param name="hidden"></param>
        private void AllSpaceSelectedIndexer(out List<int> hand, out List<int> shown, out List<int> hidden)
        {
            hand = [];
            shown = [];
            hidden = [];
            foreach (Button b in HandSpace.Children)
            {
                if (b.Opacity == 0.8)
                {
                    int index = HandSpace.Children.IndexOf(b);
                    hand.Add(index);
                }
            }
            foreach (Button b in ShownCardsSpace.Children)
            {
                if (b.Opacity == 0.8)
                {
                    int index = ShownCardsSpace.Children.IndexOf(b);
                    shown.Add(index);
                }
            }
            foreach (Button b in HiddenCardsSpace.Children)
            {
                if (b.Opacity == 0.8)
                {
                    int index = HiddenCardsSpace.Children.IndexOf(b);
                    hidden.Add(index);
                }
            }
        }
        /// <summary>
        /// like the above method, this method uses the opacity of each button to determin which ones are highlighted
        /// this one uses the player dot gamestate to determin what handspace to check tho to not waste time on unecceary checking and assinging
        /// 
        /// when a highlighetd card is found its index is added to the players selected indecies list
        /// </summary>
        private void indexSelected()
        {

            switch (rules.Playerhand.GameState)
            {
                default:
                    foreach (Button b in HandSpace.Children)
                    {
                        if (b.Opacity == 0.8)
                        {
                            int index = HandSpace.Children.IndexOf(b);
                            rules.Playerhand.SelectIdex(index);
                        }
                    }
                    break;
                case GameState.TABEL_CARDS:
                    foreach (Button b in ShownCardsSpace.Children)
                    {
                        if (b.Opacity == 0.8)
                        {
                            int index = ShownCardsSpace.Children.IndexOf(b);
                            rules.Playerhand.SelectIdex(index);
                        }
                    }
                    break;
                case GameState.HIDEN_CARDS:
                    foreach (Button b in HiddenCardsSpace.Children)
                    {
                        if (b.Opacity == 0.8)
                        {
                            int index = HiddenCardsSpace.Children.IndexOf(b);
                            rules.Playerhand.SelectIdex(index);
                        }
                    }
                    break;
                case GameState.VICTORY:
                case GameState.DEFEAT:
                    break;
            }
            highlightedList1.Clear();
            highlightedList2.Clear();
            highlightedList3.Clear();
        }
        /// <summary>
        /// one of the prettiest methods but im kinda worried how many objects are being made here lol
        /// 
        /// it genertes the opponent gui for each opponent in the match 
        /// 
        /// the gui is foremost built on a border that defines each opponents max area, then within a grid to devide the space equally into two parts
        /// the top part is for the hidden cards and the shown cards and the bottom part is for the hand cards
        /// the shown cards are drawn on top of the hidden cards to mimmic the players hand arrangment 
        /// 
        /// </summary>
        private void GenOpponentGraphics()
        {
            foreach (Opponent op in rules.Opponents)
            {
                Grid grid = new();
                RowDefinition row1 = new();
                RowDefinition row2 = new();
                grid.RowDefinitions.Add(row1);
                grid.RowDefinitions.Add(row2);
                StackPanel hiddenRow = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                StackPanel shownRow = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                StackPanel handRow = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(hiddenRow, 0);
                Grid.SetRow(shownRow, 0);
                Grid.SetRow(handRow, 1);
                grid.Children.Add(hiddenRow);
                grid.Children.Add(shownRow);
                grid.Children.Add(handRow);

                Border opponentBorder = new()
                {
                    Background = new SolidColorBrush(Colors.Green),
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = new Thickness(3),
                    Width = 330,
                    Child = grid,
                    ToolTip = "opponent"
                };
                OpponentSpace.Children.Add(opponentBorder);
            }
        }


    }
}
