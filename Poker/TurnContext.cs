using ProtoBuf;

namespace Poker
{
    [ProtoContract]
    public class TurnContext : ITurnContext
    {
        public TurnContext(
            int roundType,
            //List<PlayerActionName> previousRoundActions,
            int smallBlind,
            int moneyLeft,
            int currentPot,
            int myMoneyInTheRound,
            int currentMaxBet)
        {
            this.RoundType = roundType;
            //this.PreviousRoundActions = previousRoundActions;
            this.SmallBlind = smallBlind;
            this.MoneyLeft = moneyLeft;
            this.CurrentPot = currentPot;
            this.MyMoneyInTheRound = myMoneyInTheRound;
            this.CurrentMaxBet = currentMaxBet;
            CanCheck = this.MyMoneyInTheRound == this.CurrentMaxBet;
            MoneyToCall = this.CurrentMaxBet - this.MyMoneyInTheRound;
            IsAllIn = this.MoneyLeft <= 0;
        }

        public TurnContext() { }

        [ProtoMember(1)]
        public int RoundType { get; set; }
        /*
        [ProtoMember(2)]
        public List<PlayerActionName> PreviousRoundActions { get; }
        */

        [ProtoMember(3)]
        public int SmallBlind { get; set; }

        [ProtoMember(4)]
        public int MoneyLeft { get; set; }

        [ProtoMember(5)]
        public int CurrentPot { get; set; }

        [ProtoMember(6)]
        public int MyMoneyInTheRound { get; set; }

        [ProtoMember(7)]
        public int CurrentMaxBet { get; set; }

        [ProtoMember(8)]
        public bool CanCheck { get; set; }

        [ProtoMember(9)]
        public int MoneyToCall { get; set; }

        [ProtoMember(10)]
        public bool IsAllIn { get; set; }
    }
}
