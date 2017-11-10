using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System.Threading;

namespace Poker
{
    class Manager
    {
        private ConsoleInterface consoleInterface { get; set; }
        private PlayerUI consolePlayer { get; set; }
        private string serverIP;
        private int serverPort;

        public const string VERSION = "v0.0.1";

        public Manager(ConsoleInterface consoleInterface, PlayerUI consolePlayer)
        {
            this.consoleInterface = consoleInterface;
            this.consolePlayer = consolePlayer;
        }

        public void Start()
        {
            bool connected = false;
            while (!connected)
            {
                try
                {
                    consoleInterface.SetMsg("Please enter the server IP and port (192.168.0.1:10000):");
                    consoleInterface.SetInput();
                    string serverInfo = Console.ReadLine();
                    serverIP = serverInfo.Split(':').First();
                    serverPort = int.Parse(serverInfo.Split(':').Last());
                    consoleInterface.ClearMsg();
                    HandShake hs = new HandShake(NetworkComms.NetworkIdentifier, consolePlayer.Name, VERSION);
                    NetworkComms.SendObject("HandShake", serverIP, serverPort, hs);
                    connected = true;
                }
                catch
                {
                    consoleInterface.SetMsg("\n Host not found...");

                }
            }

            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", PrintIncomingMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<StartGameContext>("StartGameContext", StartGameContextMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<StartHandContext>("StartHandContext", StartHandContextMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<StartRoundContext>("StartRoundContext", StartRoundContextMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<EndRoundContext>("EndRoundContext", EndRoundContextMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<EndRoundContext>("EndHandContext", EndRoundContextMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<TurnContext>("TurnContext", TurnContextMessage);

            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, 0));

            while (true)
            {
                /*
               consoleInterface.SetMsg("\nPlease enter your message and press enter (Type 'exit' to quit):");
               consoleInterface.SetInput();
               string stringToSend = Console.ReadLine();

               if (stringToSend == "exit")
                   break;
               else if (stringToSend == "/list")
               {
                   NetworkComms.SendObject("Message", serverIP, serverPort, "list");
               }
              else
               {
                   Protocol msg = new Protocol(NetworkComms.NetworkIdentifier, "lolipop", stringToSend, 0);
                   NetworkComms.SendObject("Protocol", serverIP, serverPort, msg);
               }*/
                Thread.Sleep(20);
            }

            NetworkComms.Shutdown();
        }

        private void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            consoleInterface.ClearMsg();
            if (message == "Game started...")
            {
                consoleInterface.DrawGameBoard();
            }
            else if (message == "Your are Connected to Powker! Wait Another Player...")
            {
                ConsoleConfig.WriteOnConsole(6, 55, "Connected");
            }
            consoleInterface.SetMsg(message.ToString());
        }

        private void StartGameContextMessage(PacketHeader header, Connection connection, StartGameContext message)
        {
            consoleInterface.ClearMsg();
            consolePlayer.StartGame(message);
            consoleInterface.SetMsg("Game started...");
            NetworkComms.SendObject("Reply", serverIP, serverPort, "OK");
        }

        private void StartHandContextMessage(PacketHeader header, Connection connection, StartHandContext message)
        {
            consoleInterface.ClearMsg();
            consolePlayer.StartHand(message);
            consoleInterface.SetMsg("Hand ready...");
            NetworkComms.SendObject("Reply", serverIP, serverPort, "OK");
        }

        private void StartRoundContextMessage(PacketHeader header, Connection connection, StartRoundContext message)
        {
            consoleInterface.ClearMsg();
            consolePlayer.StartRound(message);
            consoleInterface.SetMsg("Round start");
            NetworkComms.SendObject("Reply", serverIP, serverPort, "OK");
        }

        private void EndRoundContextMessage(PacketHeader header, Connection connection, EndRoundContext message)
        {
            consoleInterface.ClearMsg();
            consolePlayer.EndRound(message);
            consoleInterface.SetMsg("Round end");
            NetworkComms.SendObject("Reply", serverIP, serverPort, "OK");
        }

        private void EndHandContextMessage(PacketHeader header, Connection connection, EndHandContext message)
        {
            consoleInterface.ClearMsg();
            consolePlayer.EndHand(message);
            consoleInterface.SetMsg("Hand end");
            NetworkComms.SendObject("Reply", serverIP, serverPort, "OK");
        }

        private void TurnContextMessage(PacketHeader header, Connection connection, TurnContext message)
        {
            consoleInterface.ClearMsg();
            consoleInterface.SetMsg("Your turn...");
            var action = consolePlayer.GetTurn(message);
            NetworkComms.SendObject("ReplyTurnContext", serverIP, serverPort, action);
            consoleInterface.ClearMsg();
        }
    }
}
