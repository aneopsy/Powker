using Poker;

namespace PokerServ
{
    public interface IPoker
    {
        int HandsPlayed { get; }

        IPlayer Start();
    }
}
