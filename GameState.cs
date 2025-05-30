namespace Assignment7
{
    /// <summary>
    /// the state of the game for the specified hand, changes how some mechanics works 
    /// </summary>
    internal enum GameState
    {
        DRAWPILE_ACTIVE,
        DRAWPILE_INACTIVE,
        TABEL_CARDS,
        HIDEN_CARDS,
        VICTORY,
        DEFEAT
    }
}
