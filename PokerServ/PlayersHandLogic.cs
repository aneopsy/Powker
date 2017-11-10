using System;
using System.Collections.Generic;
using System.Linq;
using Poker;
using System.Threading;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

namespace PokerServ
{
    internal class TwoPlayersBettingLogic
    {
        private readonly IList<InternalPlayer> allPlayers;

        private readonly int smallBlind;

        public TwoPlayersBettingLogic(IList<InternalPlayer> players, int smallBlind)
        {
            this.allPlayers = players;
            this.smallBlind = smallBlind;
            this.RoundBets = new List<PlayerActionName>();
            NetworkComms.AppendGlobalIncomingPacketHandler<PlayerAction>("ReplyTurnContext", ReplyTurnContext);
        }

        public int Pot
        {
            get
            {
                return this.allPlayers.Sum(x => x.PlayerMoney.CurrentlyInPot);
            }
        }

        public List<PlayerActionName> RoundBets { get; }
        public PlayerAction action = null;

        private void ReplyTurnContext(PacketHeader header, Connection connection, PlayerAction incomingMessage)
        {
            action = incomingMessage;
        }

        public void Bet(GameRoundType gameRoundType)
        {
            this.RoundBets.Clear();
            var playerIndex = 0;

            if (gameRoundType == GameRoundType.PreFlop)
            {
                this.PlaceBlinds();
                playerIndex = 2;
            }

            while (this.allPlayers.Count(x => x.PlayerMoney.InHand) >= 2
                   && this.allPlayers.Any(x => x.PlayerMoney.ShouldPlayInRound))
            {
                var player = this.allPlayers[playerIndex % this.allPlayers.Count];
                if (!player.PlayerMoney.InHand || !player.PlayerMoney.ShouldPlayInRound)
                {
                    continue;
                }

                var maxMoneyPerPlayer = this.allPlayers.Max(x => x.PlayerMoney.CurrentRoundBet);
                TurnContext TurnContext = new TurnContext(
                            (int)gameRoundType,
                            //this.RoundBets,
                            this.smallBlind,
                            player.PlayerMoney.Money,
                            this.Pot,
                            player.PlayerMoney.CurrentRoundBet,
                            maxMoneyPerPlayer);

                player.Connection.SendObject("TurnContext", TurnContext);
                action = null;
                while (action == null)
                {
                    Thread.Sleep(200);
                }
                action = player.PlayerMoney.DoPlayerAction(action, maxMoneyPerPlayer);
                this.RoundBets.Add(new PlayerActionName(player.Name, action));

                if (action.Type == (int)PlayerActionType.Raise)
                {
                    foreach (var playerToUpdate in this.allPlayers)
                    {
                        playerToUpdate.PlayerMoney.ShouldPlayInRound = true;
                    }
                }
                action = null;
                player.PlayerMoney.ShouldPlayInRound = false;
                playerIndex++;
            }

            this.ReturnMoneyInCaseOfAllIn();
        }

        private void PlaceBlinds()
        {
            var smallBlindAction = PlayerAction.Raise(this.smallBlind);

            this.RoundBets.Add(
                new PlayerActionName(
                    this.allPlayers[0].Name,
                    this.allPlayers[0].PlayerMoney.DoPlayerAction(smallBlindAction, 0)));

            this.allPlayers[1].PlayerMoney.DoPlayerAction(smallBlindAction, this.smallBlind);
            this.RoundBets.Add(
                new PlayerActionName(
                    this.allPlayers[1].Name,
                    this.allPlayers[0].PlayerMoney.DoPlayerAction(smallBlindAction, 0)));
        }

        private void ReturnMoneyInCaseOfAllIn()
        {
            var minMoneyPerPlayer = this.allPlayers.Min(x => x.PlayerMoney.CurrentRoundBet);
            foreach (var player in this.allPlayers)
            {
                player.PlayerMoney.NormalizeBets(minMoneyPerPlayer);
            }
        }
    }

    internal class TwoPlayersHandLogic : IHandLogic
    {
        private readonly int handNumber;

        private readonly int smallBlind;

        private readonly IList<InternalPlayer> players;

        private readonly Deck deck;

        private readonly List<Card> communityCards;

        private readonly TwoPlayersBettingLogic bettingLogic;

        private Dictionary<string, List<Card>> showdownCards;

        private bool waitPlayer = false;

        public TwoPlayersHandLogic(IList<InternalPlayer> players, int handNumber, int smallBlind)
        {
            this.handNumber = handNumber;
            this.smallBlind = smallBlind;
            this.players = players;
            this.deck = new Deck();
            this.communityCards = new List<Card>(5);
            this.bettingLogic = new TwoPlayersBettingLogic(this.players, smallBlind);
            this.showdownCards = new Dictionary<string, List<Card>>();
        }

        public void Play()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Reply", Reply);

            foreach (var player in this.players)
            {
                waitPlayer = false;
                var startHandContext = new StartHandContext(
                    this.deck.GetNextCard(),
                    this.deck.GetNextCard(),
                    this.handNumber,
                    player.PlayerMoney.Money,
                    this.smallBlind,
                    this.players[0].Name);
                player.StartHand(startHandContext);
                player.Connection.SendObject("StartHandContext", startHandContext);
                while (!waitPlayer)
                {
                    Thread.Sleep(200);
                }
            }

            this.PlayRound(GameRoundType.PreFlop, 0);

            if (this.players.Count(x => x.PlayerMoney.InHand) > 1)
            {
                this.PlayRound(GameRoundType.Flop, 3);
            }

            if (this.players.Count(x => x.PlayerMoney.InHand) > 1)
            {
                this.PlayRound(GameRoundType.Turn, 1);
            }

            if (this.players.Count(x => x.PlayerMoney.InHand) > 1)
            {
                this.PlayRound(GameRoundType.River, 1);
            }

            this.DetermineWinnerAndAddPot(this.bettingLogic.Pot);

            foreach (var player in this.players)
            {
                waitPlayer = false;
                EndHandContext EndHandContext = new EndHandContext(this.showdownCards);
                player.EndHand(EndHandContext);

                player.Connection.SendObject("EndHandContext", EndHandContext);
                while (!waitPlayer)
                {
                    Thread.Sleep(200);
                }
            }
        }

        private void Reply(PacketHeader header, Connection connection, string incomingMessage)
        {
            waitPlayer = true;
        }

        private void DetermineWinnerAndAddPot(int pot)
        {
            if (this.players.Count(x => x.PlayerMoney.InHand) == 1)
            {
                var winner = this.players.FirstOrDefault(x => x.PlayerMoney.InHand);
                winner.PlayerMoney.Money += pot;
            }
            else
            {
                foreach (var player in this.players)
                {
                    if (player.PlayerMoney.InHand)
                    {
                        this.showdownCards.Add(player.Name, player.Cards);
                    }
                }
                Console.WriteLine(this.players.First().Cards.Count());
                Console.WriteLine(this.players.Last().Cards.Count());
                Console.WriteLine(this.communityCards.Count());
                var betterHand = Logic.CompareCards(
                    this.players.First().Cards.Concat(this.communityCards).ToList(),
                    this.players.Last().Cards.Concat(this.communityCards).ToList());
                if (betterHand > 0)
                {
                    this.players[0].PlayerMoney.Money += pot;
                }
                else if (betterHand < 0)
                {
                    this.players[1].PlayerMoney.Money += pot;
                }
                else
                {
                    this.players[0].PlayerMoney.Money += pot / 2;
                    this.players[1].PlayerMoney.Money += pot / 2;
                }
            }
        }

        private void PlayRound(GameRoundType gameRoundType, int communityCardsCount)
        {
            for (var i = 0; i < communityCardsCount; i++)
            {
                this.communityCards.Add(this.deck.GetNextCard());
            }

            foreach (var player in this.players)
            {
                waitPlayer = false;
                var startRoundContext = new StartRoundContext(
                    gameRoundType,
                    this.communityCards,
                    player.PlayerMoney.Money,
                    this.bettingLogic.Pot);
                player.StartRound(startRoundContext);
                player.Connection.SendObject("StartRoundContext", startRoundContext);
                while (!waitPlayer)
                {
                    Thread.Sleep(200);
                }
            }

            this.bettingLogic.Bet(gameRoundType);

            foreach (var player in this.players)
            {
                //waitPlayer = false;
                var endRoundContext = new EndRoundContext(this.bettingLogic.RoundBets);
                player.EndRound(endRoundContext);
                /*player.Connection.SendObject("EndRoundContext", endRoundContext);
                while (!waitPlayer)
                {
                    Thread.Sleep(200);
                }*/
            }
        }
    }
}
