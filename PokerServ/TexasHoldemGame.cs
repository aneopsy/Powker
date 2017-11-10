using System;
using System.Collections.Generic;
using System.Linq;
using Poker;
using NetworkCommsDotNet;

namespace PokerServ
{
    public interface IHandLogic
    {
        void Play();
    }

    public class TwoPlayersTexasHoldemGame : ITexasHoldemGame
    {
        private static readonly int[] SmallBlinds =
            {
                1, 2, 3, 5, 10, 15, 20, 25, 30, 40, 50, 60, 80, 100, 150, 200, 300,
                400, 500, 600, 800, 1000, 1500, 2000, 3000, 4000, 5000, 6000, 8000,
                10000, 15000, 20000, 30000, 40000, 50000, 60000, 80000, 100000
            };

        private readonly InternalPlayer firstPlayer;

        private readonly InternalPlayer secondPlayer;

        private List<InternalPlayer> allPlayers = new List<InternalPlayer>();

        private readonly int initialMoney;

        public TwoPlayersTexasHoldemGame(InternalPlayer firstPlayer, InternalPlayer secondPlayer, int initialMoney = 1000)
        {
            if (firstPlayer == null)
            {
                throw new ArgumentNullException(nameof(firstPlayer));
            }

            if (secondPlayer == null)
            {
                throw new ArgumentNullException(nameof(secondPlayer));
            }

            if (initialMoney <= 0 || initialMoney > 200000)
            {
                throw new ArgumentOutOfRangeException(nameof(initialMoney), "Initial money should be greater than 0 and less than 200000");
            }

            if (firstPlayer.Name == secondPlayer.Name)
            {
                throw new ArgumentException($"Both players have the same name: \"{firstPlayer.Name}\"");
            }

            this.firstPlayer = firstPlayer;
            this.secondPlayer = secondPlayer;
            this.allPlayers.Add(this.firstPlayer);
            this.allPlayers.Add(this.secondPlayer);
            this.initialMoney = initialMoney;
            this.HandsPlayed = 0;
        }

        public int HandsPlayed { get; private set; }

        public IPlayer Start()
        {
            Console.WriteLine("Game started....");
            var allRelayConnections = (from current in NetworkComms.GetExistingConnection() where current != null select current).ToArray();
            foreach (var relayConnection in allRelayConnections)
            {
                relayConnection.SendObject("Message", "Game started...");
            }
            List<string> playerNames = this.allPlayers.Select(x => x.Name).ToList();
            foreach (InternalPlayer player in this.allPlayers)
            {
                StartGameContext StartGameContext = new StartGameContext(playerNames, this.initialMoney);
                player.StartGame(StartGameContext);
                player.Connection.SendObject("StartGameContext", StartGameContext);
            }

            while (this.allPlayers.Count(x => x.PlayerMoney.Money > 0) > 1)
            {
                var smallBlind = SmallBlinds[(this.HandsPlayed) / 10];
                this.HandsPlayed++;
                IHandLogic hand = this.HandsPlayed % 2 == 1
                               ? new TwoPlayersHandLogic(new[] { this.firstPlayer, this.secondPlayer }, this.HandsPlayed, smallBlind)
                               : new TwoPlayersHandLogic(new[] { this.secondPlayer, this.firstPlayer }, this.HandsPlayed, smallBlind);

                hand.Play();
            }

            var winner = this.allPlayers.FirstOrDefault(x => x.PlayerMoney.Money > 0);
            foreach (var player in this.allPlayers)
            {
                EndGameContext EndGameContext = new EndGameContext(winner.Name);
                player.EndGame(EndGameContext);
                player.Connection.SendObject("EndGameContext", EndGameContext);
            }

            return winner;
        }
    }
}
