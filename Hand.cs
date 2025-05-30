namespace Assignment7
{
    /// <summary>
    /// the hand is responsible for all the cards a has player in the game at any given moment as well as game state
    /// </summary>
    internal class Hand
    {
        private List<Card> hand, bottomTable, topTable;
        private List<int> selectedIndecies;
        private GameState gameState;
        internal Hand()
        {
            hand = [];
            bottomTable = [];
            topTable = [];
            selectedIndecies = [];
        }
        internal List<Card> HandP
        {
            get { return hand; }
        }
        internal List<Card> BottomTable
        {
            get { return bottomTable; }
        }
        internal List<Card> TopTable
        {
            get { return topTable; }
        }
        internal List<int> SelectedIndecies
        {
            get { return selectedIndecies; }
        }
        internal GameState GameState
        {
            get { return gameState; }
            set { gameState = value; }
        }
        internal int TotalCards
        {
            get
            {
                return hand.Count + topTable.Count + bottomTable.Count;
            }
        }
        public event System.EventHandler<Card>? OnPlayEvent;
        protected virtual void OnPlay(Card eventArg)
        {
            OnPlayEvent?.Invoke(this, eventArg);

        }

        /// <summary>
        /// arranges cards in order of rank
        /// </summary>
        /// <param name="cards"></param>
        internal void Sort(List<Card> cards)
        {
            cards.Sort(delegate (Card x, Card y)
            {
                return ((int)x.Rank).CompareTo((int)y.Rank);
            });
        }
        internal void OpponentSort()
        {
            Sort(hand);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < hand.Count - 1; j++)
                {
                    if (j < hand.Count && (hand[j].Rank == Rank.TWO || hand[j].Rank == Rank.TEN))
                    {
                        (hand[j], hand[j + 1]) = (hand[j + 1], hand[j]);
                    }
                }
            }
        }
        /// <summary>
        /// adds or removes a specific card represented by index to selected indecies one at a time, if handed 
        /// an already selected card (index), deselect that card. functunally removes its index from the list
        /// </summary>
        /// <param name="index"></param>
        internal void SelectIdex(int index)
        {
            if (selectedIndecies.Contains(index))
            {
                selectedIndecies.Remove(index);
                return;
            }
            if (gameState == GameState.HIDEN_CARDS)
            {
                selectedIndecies.Clear();
            }
            selectedIndecies.Add(index);
        }
        /// <summary>
        /// clears selected indecies, deselects cards
        /// </summary>
        internal void ClearSelect()
        {
            selectedIndecies.Clear();
        }
        /// <summary>
        /// returns a card from selected index, selected from appropriate location determined by the game state variable
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal Card Play(int index)
        {
            Card selected;
            switch (GameState)
            {
                default:
                case GameState.DRAWPILE_ACTIVE:
                case GameState.DRAWPILE_INACTIVE:

                    selected = hand[index];
                    hand.Remove(selected);
                    break;

                case GameState.TABEL_CARDS:

                    selected = topTable[index];
                    topTable.Remove(selected);
                    break;

                case GameState.HIDEN_CARDS:
                    selected = bottomTable[index];
                    bottomTable.Remove(selected);
                    break;
            }
            selected.Revealed = true;
            OnPlay(selected);
            return selected;
        }
        /// <summary>
        /// puts a card into one of the three locations "hand"  "toptable"  "bottomtable"
        /// defaults to "hand"
        /// </summary>
        /// <param name="card"></param>
        /// <param name="destination"></param>
        internal void AddCard(Card card, string destination = "hand")
        {
            switch (destination)
            {
                case "hand":
                    hand.Add(card);
                    break;
                case "toptable":
                    topTable.Add(card);
                    break;
                case "bottomtable":
                    bottomTable.Add(card);
                    break;
            }
        }
        /*
         * misc
         */
        /// <summary>
        /// tries to close all child processies
        /// </summary>
        internal void CloseCascade()
        {
            hand.Clear();
            topTable.Clear();
            bottomTable.Clear();
            selectedIndecies.Clear();

        }
    }
}
