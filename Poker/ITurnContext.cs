
namespace Poker
{
    public interface ITurnContext
    {
        bool CanCheck { get; }
        int CurrentMaxBet { get; }
        int CurrentPot { get; }
        bool IsAllIn { get; }
        int MoneyLeft { get; }
        int MoneyToCall { get; }
        int MyMoneyInTheRound { get; }
        //List<PlayerActionName> PreviousRoundActions { get; }
        int RoundType { get; }
        int SmallBlind { get; }
    }
}
