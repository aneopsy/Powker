namespace PokerServ
{
    using System.Collections.Generic;
    using NetworkCommsDotNet.Connections;
    using Poker;
    using System;

    public class InternalPlayer : APlayerUI
    {
        public Connection Connection { get; set; }

        public InternalPlayer(IPlayer player, Connection connection)
            : base(player)
        {
            this.Cards = new List<Card>();
            this.Connection = connection;
        }

        public List<Card> Cards { get; }

        public InternalPlayerMoney PlayerMoney { get; private set; }

        public override void StartGame(IStartGameContext context)
        {
            this.PlayerMoney = new InternalPlayerMoney(context.StartMoney);
            base.StartGame(context);
        }

        public override void StartHand(IStartHandContext context)
        {
            this.Cards.Clear();
            this.Cards.Add(context.FirstCard);
            this.Cards.Add(context.SecondCard);

            this.PlayerMoney.NewHand();

            base.StartHand(context);
        }

        public override void StartRound(IStartRoundContext context)
        {
            this.PlayerMoney.NewRound();
            base.StartRound(context);
        }
    }

    public class InternalPlayerMoney
    {
        public InternalPlayerMoney(int startMoney)
        {
            this.Money = startMoney;
            this.NewHand();
            this.NewRound();
        }

        public int Money { get; set; }

        public int CurrentlyInPot { get; private set; }

        public int CurrentRoundBet { get; private set; }

        public bool InHand { get; private set; }

        public bool ShouldPlayInRound { get; set; }

        public void NewHand()
        {
            this.CurrentlyInPot = 0;
            this.CurrentRoundBet = 0;
            this.InHand = true;
            this.ShouldPlayInRound = true;
        }

        public void NewRound()
        {
            this.CurrentRoundBet = 0;
            if (this.InHand)
            {
                this.ShouldPlayInRound = true;
            }
        }

        public PlayerAction DoPlayerAction(PlayerAction action, int maxMoneyPerPlayer)
        {
            if (action.Type == (int)PlayerActionType.Raise)
            {
                this.CallTo(maxMoneyPerPlayer);

                if (this.Money <= 0)
                {
                    return PlayerAction.CheckOrCall();
                }

                if (this.Money > action.Money)
                {
                    this.PlaceMoney(action.Money);
                }
                else
                {
                    action.Money = this.Money;
                    this.PlaceMoney(action.Money);
                }
            }
            else if (action.Type == (int)PlayerActionType.CheckCall)
            {
                this.CallTo(maxMoneyPerPlayer);
            }
            else
            {
                this.InHand = false;
                this.ShouldPlayInRound = false;
            }

            return action;
        }

        public void NormalizeBets(int moneyPerPlayer)
        {
            if (moneyPerPlayer < this.CurrentRoundBet)
            {
                var diff = this.CurrentRoundBet - moneyPerPlayer;
                this.PlaceMoney(-diff);
            }
        }

        private void PlaceMoney(int money)
        {
            this.CurrentRoundBet += money;
            this.CurrentlyInPot += money;
            this.Money -= money;
        }

        private void CallTo(int maxMoneyPerPlayer)
        {
            var moneyToPay = Math.Min(this.CurrentRoundBet + this.Money, maxMoneyPerPlayer);
            var diff = moneyToPay - this.CurrentRoundBet;
            this.PlaceMoney(diff);
        }
    }
}
