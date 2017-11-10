using System.Collections.Generic;

namespace Poker
{

    public abstract class APlayer : IPlayer
    {

        public abstract string Name { get; set; }

        protected List<Card> HandCard = new List<Card>();

        protected IReadOnlyCollection<Card> BoardCards { get; private set; }

        public virtual void StartHand(IStartHandContext context)
        {
            this.HandCard.Add(context.FirstCard);
            this.HandCard.Add(context.SecondCard);
        }

        public virtual void StartGame(IStartGameContext context) { }

        public virtual void StartRound(IStartRoundContext context)
        {
            this.BoardCards = context.BoardCards;
        }

        public abstract PlayerAction GetTurn(ITurnContext context);

        public virtual void EndRound(IEndRoundContext context) { }

        public virtual void EndHand(IEndHandContext context) { }

        public virtual void EndGame(IEndGameContext context) { }
    }
}
