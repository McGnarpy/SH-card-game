namespace Assignment7
{
    /// <summary>
    /// the board the game is played on, it is responsible for drawpiles discardpiles and cardpiles
    /// </summary>
    internal class Board
    {
        private List<Card> discard, cardpile;
        private Deck drawpile;

        internal Board(Deck deck)
        {
            this.drawpile = deck;
            this.discard = [];
            this.cardpile = [];
        }
        internal Deck Drawpile
        {
            get { return drawpile; }
        }
        internal List<Card> Discard
        {
            get { return discard; }
        }
        internal List<Card> Cardpile
        {
            get { return cardpile; }
        }
        public event System.EventHandler? ShuffleChange;
        public virtual void OnShuffleChange()
        {
            ShuffleChange?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// returns the topmost card in the drawpile
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal bool TopDraw(out Card? card)
        {
            if (drawpile.Cards.Count < 1)
            {
                card = null;
                return false;
            }
            card = drawpile.Cards[drawpile.Cards.Count - 1];
            return true;
        }
        /// <summary>
        /// returns the topmost or latest added card in the "discarded cards" pile
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal bool TopDiscard(out Card? card)
        {
            if (discard.Count < 1)
            {
                card = null;
                return false;
            }
            card = discard[discard.Count - 1];
            return true;
        }
        /// <summary>
        /// returns the topmost, or latest added card in the "played cards" pile
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal Card TopCardpile()
        {
            if (cardpile.Count == 0)
            {
                return new(Rank.TWO, Suit.SPADES);
            }
            return cardpile[cardpile.Count - 1];
        }
        /// <summary>
        /// suffles the draw pile
        /// </summary>
        internal void ShuffleDraw()
        {
            drawpile.Shuffle();
            OnShuffleChange();
        }
        /// <summary>
        /// move the "played cards" pile
        /// </summary>
        /// <returns></returns>
        internal List<Card> PickupPile()
        {
            List<Card> temp = [];
            foreach (Card c in cardpile)
            {
                temp.Add(c);
            }
            cardpile.Clear();
            return temp;
        }
        /// <summary>
        /// moves the "played cards" pile to the "discarded cards" pile
        /// </summary>
        internal void DiscardCardPile()
        {
            discard.AddRange(cardpile);
            cardpile.Clear();
        }
        /*
         * misc
         */
        /// <summary>
        /// tries to close all child processies
        /// </summary>
        internal void CloseCascade()
        {
            discard.Clear();
            cardpile.Clear();
        }
    }
}
