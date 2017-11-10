using Poker;

namespace PokerServ
{
    public interface ITexasHoldemGame
    {
        int HandsPlayed { get; }

        IPlayer Start();
    }
}
