namespace Assignment7
{
    /// <summary>
    /// a collection of cards, the fundemental corner pice to all card games, some methods are includded like suffle which rearanges the cards randomly, mind the seed though
    /// it might not always be as random as one would wish if you use the same seed repeatedly 
    /// </summary>
    internal class Deck
    {
        private List<Card> cards;
        private Random rnd;
        internal Deck(int seed) : this(seed, [])
        {
        }
        internal Deck(int seed, List<Card> cards)
        {
            this.cards = cards;
            this.rnd = new(seed);
        }


        internal List<Card> Cards
        {
            get
            {
                return cards;
            }
        }
        internal Random Rnd
        {
            get { return rnd; }
            set { rnd = value; }
        }



        /// <summary>
        /// optional parameter 
        /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments?redirectedfrom=MSDN
        /// gives null if the deck is empty otherwise gives the top most card from the deck and removes it from the decks list
        /// </summary>
        /// <param name="revealed"></param>
        /// <returns></returns>
        internal Card? Draw(bool revealed = true)
        {
            if (cards.Count <= 0)
            {
                return null;
            }
            Card card = cards[0];
            cards.RemoveAt(0);
            card.Revealed = false;
            if (revealed)
            {
                card.Revealed = true;
            }
            return card;
        }
        /// <summary>
        /// 
        /// https://stackoverflow.com/questions/273313/randomize-a-listt
        /// inspired by and improved upon the above 
        /// shuffles the cards of the deck
        /// </summary>
        internal void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                Swap(cards, i - 1, rnd.Next(0, i));
            }
        }
        /// <summary>
        /// used in shuffle, it swaps two elemnts of a collection but in this case list of card objects
        /// </summary>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private static void Swap(List<Card> list, int i, int j)
        {
            (list[j], list[i]) = (list[i], list[j]);
        }
        /// <summary>
        /// ads a list of cards to the decks 
        /// </summary>
        /// <param name="newCards"></param>
        internal void IntegrateCards(List<Card> newCards)
        {
            this.cards.AddRange(newCards);
            newCards.Clear();
        }
    }
}
