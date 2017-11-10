
namespace Poker
{
    public interface IPlayer
    {
        string Name { get; }
        void StartHand(IStartHandContext context);
        void StartGame(IStartGameContext context);
        void StartRound(IStartRoundContext context);
        void EndRound(IEndRoundContext context);
        void EndHand(IEndHandContext context);
        void EndGame(IEndGameContext context);
        PlayerAction GetTurn(ITurnContext context);
    }
}
