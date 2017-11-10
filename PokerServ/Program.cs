using System;
using System.Collections.Generic;
using System.Linq;
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using System.Net;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using Poker;
using System.Threading;

namespace PokerServ
{
    class Program
    {

        static void Main(string[] args)
        {
            Serv serv = new Serv();
            serv.Run();
        }
    }

    class Serv
    {
        public IPEndPoint lastServerIPEndPoint = null;
        private List<InternalPlayer> Clients = new List<InternalPlayer>();
        public DataSerializer Serializer { get; set; }
        Dictionary<ShortGuid, HandShake> lastPeerMessageDict = new Dictionary<ShortGuid, HandShake>();
        public ITexasHoldemGame game;


        public void Run()
        {

            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", PrintIncomingMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<Protocol>("Protocol", HandleIncomingProtocol);
            NetworkComms.AppendGlobalIncomingPacketHandler<HandShake>("HandShake", HandleIncomingHandShake);
            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);
            this.Serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(Serializer, NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, 0));
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (IPEndPoint l in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            {
                Console.WriteLine("{0}:{1}", l.Address, l.Port);
            }
            lastServerIPEndPoint = (IPEndPoint)Connection.ExistingLocalListenEndPoints(ConnectionType.TCP).Last();
            while (true)
            {
                if (Clients.Count() == 2)
                {
                    game = new TwoPlayersTexasHoldemGame(Clients.Last(), Clients.First());
                    game.Start();
                }
                Thread.Sleep(200);
            }

            //NetworkComms.Shutdown();
        }

        private static void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine(message.ToString());
        }

        private static void HandleIncomingProtocol(PacketHeader header, Connection connection, Protocol incomingMessage)
        {
            Console.WriteLine(incomingMessage.SourceName + " - " + incomingMessage.Message);
        }

        protected virtual void HandleIncomingHandShake(PacketHeader header, Connection connection, HandShake incomingMessage)
        {
            if (Clients.Count() <= 2)
            {
                //IPEndPoint clientIPEndPoint = (IPEndPoint) connection.ExistingLocalListenEndPoints(ConnectionType.TCP).Last();
                //NetworkComms.SendObject("Protocol", clientIPEndPoint.Address.ToString(), clientIPEndPoint.Port, "Connected");

                lock (lastPeerMessageDict)
                {
                    /*
                    if (lastPeerMessageDict.ContainsKey(incomingMessage.SourceIdentifier))
                    {
                        if (lastPeerMessageDict[incomingMessage.SourceIdentifier].MessageIndex < incomingMessage.MessageIndex)
                        {
                            
                            lastPeerMessageDict[incomingMessage.SourceIdentifier] = incomingMessage;
                        }
                    }
                    else
                    {
                    */
                    lastPeerMessageDict.Add(incomingMessage.SourceIdentifier, incomingMessage);
                    Clients.Add(new InternalPlayer(new Player(0, incomingMessage.Name), connection));
                    connection.SendObject("Message", "Your are Connected to Powker! Wait Another Player...");
                    //}
                }
            }
        }

        private void HandleConnectionClosed(Connection connection)
        {
        }
    }
}
