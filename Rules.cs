using System.Windows.Controls;

namespace Assignment7
{/// <summary>
/// the rules is the framework for the game, it holds a lot of the logic and sinue y parts that tie all of it together, the rules allow or dissalow plays and handel all interaction
/// between hands and boards 
/// 
/// the opponent logic is based of the same ruleset but is not dependant on it codewise, to modefi such behavior it is recomended to change the asess method and coupled private 
/// methods as well
/// </summary>
    internal class Rules
    {
        private Board board;
        private Hand playerhand;
        private List<Opponent> opponents;
        private OptionsWindow options;
        private bool startingUp;
        /*
         * constructor
         */
        internal Rules(Deck deck, OptionsWindow options)
        {
            board = new(deck);
            playerhand = new();
            opponents = [];
            this.options = options;
        }
        /*
         * properties
         */
        internal Hand Playerhand
        {
            get { return playerhand; }
        }
        internal Board Board
        {
            get { return board; }
        }
        internal List<Opponent> Opponents
        {
            get { return opponents; }
        }

        /*
         * 
         *  custome event set up
         * 
         */
        public event System.EventHandler? CardpileChange;
        protected virtual void OnCardpileChange()
        {
            if (startingUp) { return; }
            CardpileChange?.Invoke(this, EventArgs.Empty);
        }
        public event System.EventHandler? DiscardChange;
        protected virtual void OnDiscardChange()
        {
            if (startingUp) { return; }
            DiscardChange?.Invoke(this, EventArgs.Empty);
        }
        public event System.EventHandler? DrawpileChange;
        protected virtual void OnDrawpileChange()
        {
            if (startingUp) { return; }
            DrawpileChange?.Invoke(this, EventArgs.Empty);
        }
        public event System.EventHandler? ENDOFGAME;
        protected virtual void OnEndOfGame()
        {
            ENDOFGAME?.Invoke(this, EventArgs.Empty);
        }
        public event System.EventHandler<Card>? OpponentPlay;
        protected virtual void OnOpponentPlay(Card eventArg)
        {
            OpponentPlay?.Invoke(this, eventArg);
        }

        /// <summary>
        /// start up sequence, shuffles the drawpile and places the table and adds opponents and sets the game state of all hands to drawpile active
        /// </summary>
        internal void Start()
        {
            startingUp = true;
            board.ShuffleDraw();
            for (int i = 0; i < options.NumOfOpponents; i++)
            {
                opponents.Add(new(options.Seed));
            }
            PlaceTable();
            playerhand.Sort(playerhand.HandP);

            SetGlobalGameState(GameState.DRAWPILE_ACTIVE);
            startingUp = false;
        }
        /// <summary>
        /// handels the game play loop
        /// grants extra turn to player if played something gicing a play again effect, adjusts game state then draw cards then sort playerhand
        /// 
        /// loops through opponents and does their turn unless they have lost from going out on a 2 or a 10
        /// temporarily disables the button controll handed in trhough parameters to show that the game is over as a placeholder
        /// </summary>
        /// <param name="button"></param>
        internal void NextTurn(Button button)
        {

            DrawCardIfAvailable(playerhand);
            playerhand.Sort(playerhand.HandP);


            foreach (Opponent opponent in opponents)
            {
                SetSpecificGameState(opponent.Hand);
                OpponentTurn(opponent);
            }
        }

        /// <summary>
        /// player input 
        /// the player version of the opponent check play method and the opponent turn method in one, checks if the given playerhand is allowed and of what sort it is.
        /// then plays the playerhand if allowed or other acceptable sort, only the allowed playerhand activatess next turn, despite others namely 4 of a kind, 2s and 10s also play 
        /// possible hands and effects there of:
        /// 
        /// [empty]             nothing
        /// FORBIDE             nothing
        /// ALLOWED             i(s) is(/are) played
        /// TWO                 i is played (irregardless of last rank) & extra turn
        /// TEN                 i is played (irregardless of last rank) & extra turn & discard cardpile
        /// FOUR_OF_A_KIND      i is played & extra turn & discard cardpile
        /// 
        /// </summary>
        /// <param name="playerhand"></param>
        /// <returns></returns>
        internal AllowOutcome Turn()
        {
            List<Card> cards = [];
            foreach (int i in playerhand.SelectedIndecies)
            {/// rconstructing a lit of cards for the allow play method
                switch (playerhand.GameState)
                {
                    case GameState.TABEL_CARDS:
                        cards.Add(playerhand.TopTable[i]);
                        break;
                    case GameState.HIDEN_CARDS:
                        cards.Add(playerhand.BottomTable[i]);
                        break;
                    default:
                        cards.Add(playerhand.HandP[i]);
                        break;
                }
            }

            AllowOutcome turnOutcome;
            bool discard = false;
            switch (AllowPlay(cards))
            {
                default:
                case AllowOutcome.FORBIDE:
                    return turnOutcome = AllowOutcome.FORBIDE;

                case AllowOutcome.TWO:
                    turnOutcome = AllowOutcome.TWO;
                    break;

                case AllowOutcome.TEN:
                    turnOutcome = AllowOutcome.TEN;
                    discard = true;
                    break;
                case AllowOutcome.FOUR_OF_A_KIND:

                    discard = true;
                    turnOutcome = AllowOutcome.FOUR_OF_A_KIND;
                    break;
                case AllowOutcome.ALLOWED:

                    turnOutcome = AllowOutcome.ALLOWED;
                    break;
            }

            for (int i = 0; i < playerhand.SelectedIndecies.Count; i++)
            {
                board.Cardpile.Add(playerhand.Play(playerhand.SelectedIndecies[i] - i));
                OnCardpileChange();
            }

            DrawCardIfAvailable(playerhand);
            playerhand.ClearSelect();

            if (discard)
            {
                board.DiscardCardPile();
                OnCardpileChange();
                OnDiscardChange();
            }

            SetSpecificGameState(playerhand);
            return turnOutcome;
        }

        /// <summary>
        /// set all hands to a specific game state 
        /// </summary>
        /// <param name="gameState"></param>
        private void SetGlobalGameState(GameState gameState)
        {
            playerhand.GameState = gameState;
            foreach (Opponent opponent in opponents)
            {
                opponent.Hand.GameState = gameState;
            }
        }
        /// <summary>
        /// as long as the drawpile isnt empty, retrives a i from the top of the pile. 
        /// monitors the drawpile, if the drawpile becomes empty sets all hands gamestate to drawpile inactive
        /// after this point this method will be skipped 
        /// </summary>
        /// <param name="hand"></param>
        private void DrawCardIfAvailable(Hand hand)
        {
            while (hand.GameState == GameState.DRAWPILE_ACTIVE && hand.HandP.Count < options.MinHandAmount)
            {
                DrawFromDeck(hand);
                if (board.Drawpile.Cards.Count == 0)
                {
                    SetGlobalGameState(GameState.DRAWPILE_INACTIVE);
                }

            }
        }
        /// <summary>
        /// changes the game state after certain criteria, method is skiped if drawpile is active since you cant get a 0 count playerhand at the start of the next players playerhand
        /// if there are still cards in the drawpile
        /// 
        /// the drawpile status is monitoured by the draw cards if available method using the set global game state method
        /// 
        /// - firstly if the playerhand inst empty (1 or more cards in the playerhand) the game state is set to drawpile inactive
        /// - if the playerhand is empty:             table cards
        /// - if playerhand and table cards empty:    hidden cards
        /// - if all i locations empty:      victory
        /// 
        /// if the lst played i was a 10 or a 2 the player will lose and the game goes on with the rest of the players
        /// and if somehow in some way you have an empty discardpile and an empty cardpile the player will also lose since that is probobly cheating
        /// 
        /// </summary>
        /// <param name="hand"></param>
        private void SetSpecificGameState(Hand hand)
        {

            if (hand.HandP.Count != 0)
            {
                switch (board.Drawpile.Cards.Count == 0)
                {
                    case false:
                        hand.GameState = GameState.DRAWPILE_ACTIVE;
                        break;
                    case true:
                        hand.GameState = GameState.DRAWPILE_INACTIVE;
                        break;
                }
            }
            else if (hand.TopTable.Count != 0)
            {

                hand.GameState = GameState.TABEL_CARDS;
            }
            else if (hand.BottomTable.Count == hand.TotalCards)
            {
                hand.GameState = GameState.HIDEN_CARDS;
            }


            if (hand.TotalCards == 0)
            {
                if (board.Cardpile.Count > 0)
                {
                    if (board.TopCardpile().Rank != Rank.TWO)
                    {
                        hand.GameState = GameState.VICTORY;
                        EndOfGame();
                        return;
                    }
                    else
                    {
                        hand.GameState = GameState.DEFEAT;
                        EndOfGame();
                    }
                }
                else if (board.TopDiscard(out Card? card))
                {
                    if ((card ?? new(Rank.THREE, Suit.SPADES)).Rank != Rank.TEN)
                    {
                        hand.GameState = GameState.VICTORY;
                        EndOfGame();
                        return;
                    }
                    else
                    {
                        hand.GameState = GameState.DEFEAT;
                    }
                }
                hand.GameState=GameState.DEFEAT;
            }

        }
        /// <summary>
        /// sets all players gamestate to either defeat or victory 
        /// </summary>
        private void EndOfGame()
        {
            if (playerhand.GameState != GameState.VICTORY)
            {
                playerhand.GameState = GameState.DEFEAT;
            }
            board.Drawpile.IntegrateCards(playerhand.HandP);
            board.Drawpile.IntegrateCards(playerhand.TopTable);
            board.Drawpile.IntegrateCards(playerhand.BottomTable);
            foreach (Opponent opponent in opponents)
            {
                if (opponent.Hand.GameState != GameState.VICTORY)
                {
                    opponent.Hand.GameState = GameState.DEFEAT;
                }
                board.Drawpile.IntegrateCards(opponent.Hand.HandP);
                board.Drawpile.IntegrateCards(opponent.Hand.TopTable);
                board.Drawpile.IntegrateCards(opponent.Hand.BottomTable);
            }
            board.Drawpile.IntegrateCards(board.Discard);
            board.Drawpile.IntegrateCards(board.Cardpile);
            OnEndOfGame();

        }
        /// <summary>
        /// opponent plays playerhand after internal logic, or if it fails to do so picks up the cardpile
        /// and draws i if available
        /// 
        /// this is the end point of the our variables discard and play again, thier effects are as followed if true:
        /// 
        /// play again      lets this opponent make another turn
        /// discard         discards the cardpile
        /// 
        /// </summary>
        /// <param name="opponent"></param>
        private void OpponentTurn(Opponent opponent)
        {
            if (opponent.Hand.GameState == GameState.VICTORY || opponent.Hand.GameState == GameState.DEFEAT)
            {
                return;
            }

            DrawCardIfAvailable(opponent.Hand);
            bool discard = false, playagain = false;
            switch (opponent.Asess((int)board.TopCardpile().Rank, out playagain, out discard))
            {
                case true:
                    for (int i = 0; i < opponent.Hand.SelectedIndecies.Count; i++)
                    {
                        Card playedCard = opponent.Hand.Play(opponent.Hand.SelectedIndecies[0]);
                        board.Cardpile.Add(playedCard);
                        OnOpponentPlay(playedCard);
                        OnCardpileChange();

                        GameState previousGamestate = opponent.Hand.GameState;
                        SetSpecificGameState(opponent.Hand);
                        if (previousGamestate != opponent.Hand.GameState)
                        {
                            break;
                        }
                    }
                    DrawCardIfAvailable(opponent.Hand);
                    break;
                case false:
                    opponent.Hand.HandP.AddRange(board.PickupPile());
                    OnCardpileChange();
                    break;
            }
            opponent.Hand.ClearSelect();
            if (discard)
            {
                board.DiscardCardPile();
                OnCardpileChange();
                OnDiscardChange();
            }
            if (playagain)
            {
                OpponentTurn(opponent);
            }
        }

        /// <summary>
        /// sets upp the i distrobution for the begining of the game, default 3 cards hiden 3 cards shown and 3 cards in playerhand for each player
        /// </summary>
        private void PlaceTable()
        {

            //creates a singular list with all playing parties hands
            List<Hand> hands = [];
            hands.Add(playerhand);
            foreach (Opponent opponent in opponents)
            {
                hands.Add(opponent.Hand);
            }

            //supply each playing party with the cards needed to play the game
            int emergencyShutOff1 = 0, emergencyShutOff2 = 0, emergencyShutOff3 = 0, emergencyLimit = 10;
            foreach (Hand hand in hands)
            {
                emergencyShutOff1 = 0;
                emergencyShutOff2 = 0;
                emergencyShutOff3 = 0;
                for (int i = 0; i < options.HidenCardsAmount; i++) ///3 hidden table cards
                {
                    DrawFromDeck(hand, "bottomtable");
                    emergencyShutOff1++;
                    if (emergencyShutOff1 > emergencyLimit)
                    {
                        break;
                    }
                }
                for (int i = 0; i < options.ShownCardsAmount; i++) /// 3 table cards
                {
                    DrawFromDeck(hand, "toptable");
                    emergencyShutOff2++;
                    if (emergencyShutOff2 > emergencyLimit)
                    {
                        break;
                    }
                }
                while (hand.HandP.Count < options.MinHandAmount)
                {
                    DrawFromDeck(hand);
                    emergencyShutOff3++;
                    if (emergencyShutOff3 > emergencyLimit)
                    {
                        break;
                    }
                }

            }
        }



        /// <summary>
        /// retrives a i from the drawpile (in board) and adds it to specified playerhand, optional location default playerhand.
        /// 
        /// possible locations:
        /// "playerhand"
        /// "toptable"
        /// "bottomtable"
        /// 
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="destination"></param>
        private void DrawFromDeck(Hand hand, string destination = "hand")
        {
            Card? card = board.Drawpile.Draw(destination != "bottomtable");
            if (card != null)
            {
                hand.AddCard(card, destination);
                OnDrawpileChange();
            }
        }


        /// <summary>
        /// take a list of cards and checks if it is an allowed play and also what sort of playerhand it is since some hands have special effects
        /// the results of this method can be:
        /// 
        /// FORBIDE  
        /// ALLOWED
        /// TWO
        /// TEN
        /// FOUR_OF_A_KIND
        /// 
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private AllowOutcome AllowPlay(List<Card> cards)
        {
            switch (cards.Count)
            {
                case 0:
                    /// does nothing 
                    return AllowOutcome.FORBIDE;

                case 1:
                    ///for high i/ single cards, checks if intended played i iis above curent top i
                    if (cards[0].Rank == Rank.TEN)
                    {
                        return AllowOutcome.TEN;
                    }

                    if (cards[0].Rank == Rank.TWO)
                    {
                        return AllowOutcome.TWO;
                    }

                    if ((int)cards[0].Rank >= (int)board.TopCardpile().Rank)
                    {
                        return AllowOutcome.ALLOWED;
                    }
                    return AllowOutcome.FORBIDE;

                default:


                    /*
                     * buggyness: please read the comment in *opponent* "SecondaryCheck" for more information
                     * countermeasure temporary implament shut off for all multi i plays
                     */
                    /*  [RESOLVED]
                     * 
                     * remove above when the bug is fixed, solutions can be found in opponent -> "SecomdaryCheck"
                     */



                    /// for multiple cards, strights or several of a kind are the only current allowed ones

                    if ((int)cards[0].Rank < (int)board.TopCardpile().Rank)
                    {/// checks if lowest i is higher or equal to last played i
                        return AllowOutcome.FORBIDE;
                    }


                    if (cards.Count > 2) /// straight
                    {
                        bool chainBroken = false;
                        Card currentCard = cards[0];
                        for (int i = 1; i < cards.Count; i++)
                        {
                            if ((int)cards[i].Rank - (int)currentCard.Rank != 1)
                            {
                                chainBroken = true;
                                break;
                            }
                            currentCard = cards[i];
                        }
                        if (!chainBroken)
                        {
                            return AllowOutcome.ALLOWED;
                        }
                    }

                    Card firstCard = cards[0]; /// several of a kind
                    int sameCards = 0;
                    foreach (Card card in cards)
                    {
                        sameCards++;
                        if (card.Rank != firstCard.Rank)
                        {
                            return AllowOutcome.FORBIDE;
                        }
                    }
                    if (sameCards == 4)
                    {
                        return AllowOutcome.FOUR_OF_A_KIND;
                    }

                    return AllowOutcome.ALLOWED; /// if none of above checks boots the proces returns true
            }
        }
        /*
         * misc
         */
        /// <summary>
        /// tries to close child processies
        /// </summary>
        internal void CloseCascade()
        {
            EndOfGame();
            options.Close();
            opponents.Clear();
            board.CloseCascade();
            playerhand.CloseCascade();
        }
    }
}
