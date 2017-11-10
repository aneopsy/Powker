using System.Collections.Generic;
using ProtoBuf;

namespace Poker
{
    [ProtoContract]
    public class EndGameContext : IEndGameContext
    {
        public EndGameContext(string WinName)
        {
            this.WinName = WinName;
        }

        public EndGameContext() { }

        [ProtoMember(1)]
        public string WinName { get; set; }
    }

    [ProtoContract]
    public class EndHandContext : IEndHandContext
    {
        public EndHandContext(Dictionary<string, List<Card>> showdownCards)
        {
            this.ShowdownCards = showdownCards;
        }

        public EndHandContext() { }

        [ProtoMember(1)]
        public Dictionary<string, List<Card>> ShowdownCards { get; set; }
    }

    [ProtoContract]
    public class EndRoundContext : IEndRoundContext
    {
        public EndRoundContext(List<PlayerActionName> roundActions)
        {
            this.RoundActions = roundActions;
        }

        public EndRoundContext() { }

        [ProtoMember(1)]
        public List<PlayerActionName> RoundActions { get; set; }
    }
}
