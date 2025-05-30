namespace Assignment7
{
    /// <summary>
    /// an editor for the deck, to make a deck of cards and possible adjust it, methods to make new concoctions of cards are to be placed here
    /// </summary>
    internal static class DeckEditor
    {
        internal static List<Card> GenerateDefaultDeck()
        {
            List<Card> cards = [];
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    Card card = new(rank, suit);
                    cards.Add(card);
                }
            }
            return cards;
        }


    }
}
