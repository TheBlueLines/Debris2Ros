using Debris;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Net.Sockets;

namespace Debris2Ros
{
    internal class Program
    {
        private static RosSocket? rosSocket = null;
        private static string publication_id = string.Empty;
        static void Main(string[] args)
        {
            rosSocket = new RosSocket(new WebSocketNetProtocol("ws://192.168.10.1:9090"));
            publication_id = rosSocket.Advertise<Twist>("cmd_vel");
            Engine.handle = new Fly();
            Server.StartServer();
        }
        public static void SendTwist(Twist twist)
        {
            if (rosSocket != null && !string.IsNullOrEmpty(publication_id))
            {
                rosSocket.Publish(publication_id, twist);
            }
        }
    }
    public class Fly : Handle
    {
        public override Packet? Request(Packet packet, NetworkStream stream)
        {
            if (packet.id == 0)
            {
                Twist twist = new()
                {
                    linear = new()
                    {
                        x = BitConverter.ToDouble(packet.data, 0),
                        y = BitConverter.ToDouble(packet.data, 8),
                        z = BitConverter.ToDouble(packet.data, 16),
                    },
                    angular = new()
                    {
                        x = BitConverter.ToDouble(packet.data, 24),
                        y = BitConverter.ToDouble(packet.data, 32),
                        z = BitConverter.ToDouble(packet.data, 40),
                    }
                };
                Program.SendTwist(twist);
            }
            return null;
        }
    }
}