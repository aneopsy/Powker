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

        [ProtoMember(1)]
        public string WinName { get; }
    }

    [ProtoContract]
    public class EndHandContext : IEndHandContext
    {
        public EndHandContext(Dictionary<string, List<Card>> showdownCards)
        {
            this.ShowdownCards = showdownCards;
        }

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

        [ProtoMember(1)]
        public List<PlayerActionName> RoundActions { get; set; }
    }
}
