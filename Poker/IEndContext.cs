using System.Collections.Generic;

namespace Poker
{
    public interface IEndGameContext
    {
        string WinName { get; }
    }

    public interface IEndHandContext
    {
        Dictionary<string, List<Card>> ShowdownCards { get; }
    }

    public interface IEndRoundContext
    {
        List<PlayerActionName> RoundActions { get; set; }
    }
}
