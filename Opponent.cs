namespace Assignment7
{
    /// <summary>
    /// an opponent is a rudementary algorythm that uses a hand and an int representing the lasst played card to determin what card to play
    /// </summary>
    internal class Opponent
    {
        private Hand hand;
        private Random rnd;
        private int XofAKind;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="seed"></param>
        internal Opponent(int seed)
        {
            hand = new();
            rnd = new(seed);
        }
        internal Hand Hand
        {
            get { return hand; }
        }
        /// <summary>
        /// asesses what move to make, not a very complicated algerythm. always plays the lowest layable card, 10 and 2s count as most valuable.
        /// if 2 or 10 played feeds the playagin out variable forward, if 10 also feeds the discrad out variable forward.
        /// logic changes depending on gamestate; allowing for pairs, and 3+ of a kind when playing from the hand to speed up the pace of the game at that point
        /// when playing the shown cards it will sequentially go trhough them but in a random order picking the first card above the last played card, for unpredictability
        /// and when playing the hidden cards will take a random card and play it without checking, this may lead to the opponent picking up the cardpile
        /// at the last gamestate (hiden cards) the opponent will not make use of the "play again" feature of 10s and 2s for a chance to catch up and for dramatic effect
        /// 
        /// a return of false means no allowed hands were found meaning the opponent will have to pick up the cardpile
        /// a return of true means an allowed hand was found and added to the "selected indecies" property of the opponents hand
        /// </summary>
        /// <param name="topplayedcard"></param>
        /// <param name="playagain"></param>
        /// <param name="discard"></param>
        /// <returns></returns>
        internal bool Asess(int topplayedcard, out bool playagain, out bool discard)
        {
            XofAKind = 0;
            playagain = false;
            discard = false;

            switch (hand.GameState)
            {

                default:
                    hand.OpponentSort();
                    int index = Pick(topplayedcard, out playagain, out discard);
                    switch (index)
                    {
                        case -1:
                            return false;
                        default:
                            XofAKind++;
                            hand.SelectedIndecies.Add(index);
                            SecondaryCheck(index);
                            if (XofAKind > 3)
                            {
                                discard = true;
                                playagain = true;
                            }
                            return true;
                    }

                case GameState.TABEL_CARDS:
                    List<Card> cards = RandomOrder(hand.TopTable);
                    foreach (Card card in cards)
                    {
                        if (card.Rank == Rank.TWO || card.Rank == Rank.TEN)
                        {
                            playagain = true;
                            if (LocateCard(card))
                            {
                                return true;
                            }
                        }
                        else if ((int)card.Rank >= topplayedcard)
                        {
                            if (LocateCard(card))
                            {
                                return true;
                            }
                        }
                    }
                    return false;

                case GameState.HIDEN_CARDS:
                    int pickedCard = PickRandom(hand.BottomTable);
                    hand.SelectedIndecies.Add(pickedCard);
                    return topplayedcard <= (int)hand.BottomTable[pickedCard].Rank;
            }
        }
        /// <summary>
        /// goes trhough each card in hand uses the CheckPlay method to check, carries the out varibale chain. returns -1 if none of the cards are playable
        /// </summary>
        /// <param name="topplayed"></param>
        /// <param name="playagain"></param>
        /// <param name="discard"></param>
        /// <returns></returns>
        private int Pick(int topplayed, out bool playagain, out bool discard)
        {
            playagain = false;
            discard = false;
            for (int i = 0; i < hand.HandP.Count; i++)
            {
                if (CheckPlay((int)hand.HandP[i].Rank, topplayed, out playagain, out discard))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// pics the index of a random card from a list, bounds the size of the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private int PickRandom(List<Card> list)
        {
            return rnd.Next(0, list.Count);
        }
        /// <summary>
        /// checks the rank of the given caard against the last played card (also give), excepttions for 2 and 10. out variable chain arrives in the rules class
        /// </summary>
        /// <param name="tryplay"></param>
        /// <param name="topplayedcard"></param>
        /// <param name="playagain"></param>
        /// <param name="discard"></param>
        /// <returns></returns>
        private bool CheckPlay(int tryplay, int topplayedcard, out bool playagain, out bool discard)
        {
            playagain = false;
            discard = false;
            switch (topplayedcard)
            {
                case (int)Rank.TWO:
                case (int)Rank.TEN:
                    return true;
                default:
                    break;
            }
            switch (tryplay)
            {
                case (int)Rank.TWO:
                    playagain = true;
                    return true;
                case (int)Rank.TEN:
                    discard = true;
                    playagain = true;
                    return true;
                default:
                    return tryplay >= topplayedcard;
            }
        }
        /// <summary>
        /// recursivly goes through the cards to the right and adds them to selected indecies if they are of the same kind, stops if at last card or a non matching card apears
        /// 
        /// due to buggyness this feature is temporarily disabled, the reason why its prone to crashing is becuase with this it stores
        /// several values from the index of the card, however when you go through the indecies to remove them one by one the later indecies
        /// get shifted to the left by one (decramented) for every preceeding index removed, this causes unplayable cards to be able to be played,
        /// since the check for allowed play is doneprior to the like book keeping call, as this is a part of.
        /// 
        /// this problem also effects the player multi cards selected option.
        /// 
        /// in cases where the right most card has been chosen and is a valid play in context this bug will cause a crash since the application
        /// tries to pull from the former last index number which now falls outside the bounds of the list 
        /// 
        /// posible solutions:
        ///   - make a temporary array as a simelactrum of the list, convert indecies to be removed to null or some other common value
        ///     and then send the array into a loop where each element that isnt null or the marker data is added to a list
        /// 
        ///   - instead of using a foreach index, make it a for loop, this way we can keep track of how many indecies before the current one 
        ///     that we have gone trhouhg, subtract this amount from the index (ex: selected indecies = 0, 1, 2 and list = 0, 1, 2; index 0 removed, list = 0,1; index 0 removed, list = 0; index 0 removed, list = empty;)
        /// </summary>
        /// <param name="index"></param>
        private void SecondaryCheck(int index)
        {
            if (index == hand.HandP.Count - 1)
            {
                return;
            }
            int newIndex = index + 1;
            if (hand.HandP[index].Rank == hand.HandP[newIndex].Rank)
            {
                XofAKind++;
                hand.SelectedIndecies.Add(newIndex);
                SecondaryCheck(newIndex);
            }
        }
        /// <summary>
        /// puts a list of cards in a random order
        /// </summary>
        /// <param name="orgcards"></param>
        /// <returns></returns>
        private List<Card> RandomOrder(List<Card> orgcards)
        {
            List<Card> cards = orgcards;
            for (int i = cards.Count - 1; i > 1; i--)
            {
                (cards[i - 1], cards[rnd.Next(0, i)]) = (cards[rnd.Next(0, i)], cards[i - 1]);
            }
            return cards;
        }
        /// <summary>
        /// locates a card by suit and rank, will have uninted effects if duplicates of the same cards are among th list but wont break
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private bool LocateCard(Card card)
        {
            for (int i = 0; i < hand.TopTable.Count; i++)
            {
                if (card.Rank == hand.TopTable[i].Rank && card.Suit == hand.TopTable[i].Suit)
                {
                    hand.SelectedIndecies.Add(i);
                    return true;
                }
            }
            return false;
        }
    }
}
