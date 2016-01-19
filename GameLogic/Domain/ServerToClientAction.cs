namespace GameLogic.Domain
{
    public enum ServerToClientAction
    {
        Unknown = 0,
        Wait = 1,
        DrawAndDiscard = 2,
        PlayBlind = 3,
        PlayEquipment = 4,
        ViewEquipment = 5,
        ViewHand = 6,
    }
}