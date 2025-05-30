namespace Assignment7
{
    /// <summary>
    /// a card, it has some information on it 
    /// </summary>
    internal class Card
    {
        private readonly Rank rank;
        private readonly Suit suit;
        private bool revealed;

        internal Card(Rank rank, Suit suit, bool revealed = true)
        {
            this.suit = suit;
            this.rank = rank;
            this.revealed = revealed;
        }
        internal Rank Rank
        {
            get { return rank; }
        }
        internal Suit Suit
        {
            get { return suit; }
        }
        internal bool Revealed
        {
            get { return revealed; }
            set { revealed = value; }
        }
    }
}
