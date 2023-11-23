using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;

public class FriendsListPacket : BaseListPacket<FriendInfo>
{
    protected override FriendInfo ReadItem(EndianBinaryReader reader)
    {
        return new FriendInfo()
        {
            Id = reader.ReadString(),
            Username = reader.ReadString(),
            Avatar = reader.ReadString(),
            Status = (FriendStatus)reader.ReadByte(),
            Properties = reader.ReadDictionary()
        };
    }

    protected override void WriteItem(FriendInfo item, EndianBinaryWriter writer)
    {
        writer.Write(item.Id);
        writer.Write(item.Username);
        writer.Write(item.Avatar);
        writer.Write((byte)item.Status);
        writer.Write(item.Properties);
    }
}
