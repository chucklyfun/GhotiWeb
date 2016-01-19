namespace GameLogic.Domain
{
    public enum ClientToServerAction
    {
        Unknown = 0,
        Wait = 1,
        DrawAndDiscard = 2,
        PlayAction = 3,
        PlayEquipment = 4,
        StartGame = 5
    }
}