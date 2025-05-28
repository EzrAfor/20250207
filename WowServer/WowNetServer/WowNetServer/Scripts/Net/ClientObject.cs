using System.Net.Sockets;
/// <summary>
/// 客户端对象
/// </summary>
public class ClientObject
{
    public Socket socket;
    public ByteObject bo=new ByteObject();
    public long lastPingTime = 0;
    public Player player;
}
