namespace MasterServerToolkit.MasterServer
{
    public interface ILeaderInfo
    {
        string Id { get; set; }
        string DisplayName { get; set; }
        int Score { get; set; }
    }
}