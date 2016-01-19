namespace GameLogic.Domain
{
    public enum GameState
    {
        TurnStart = 1,
        PlayBlind = 2,
        Reveal = 3,
        PlayEquip = 4,
        PlayAmbushAttack = 5,
        PlayDraw = 6
    }
}