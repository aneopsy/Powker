using ProtoBuf;

namespace Poker
{
    [ProtoContract]
    public class PlayerAction
    {
        private static readonly PlayerAction FoldObject = new PlayerAction(PlayerActionType.Fold);
        private static readonly PlayerAction CheckCallObject = new PlayerAction(PlayerActionType.CheckCall);

        private PlayerAction(PlayerActionType type)
        {
            this.Type = (int)type;
        }

        private PlayerAction(int money)
        {
            this.Type = 2;
            this.Money = money;
        }

        private PlayerAction() { }

        [ProtoMember(1)]
        public int Type { get; set; }

        [ProtoMember(2)]
        public int Money { get; set; }

        public static PlayerAction Fold()
        {
            return FoldObject;
        }

        public static PlayerAction CheckOrCall()
        {
            return CheckCallObject;
        }

        public static PlayerAction Raise(int withAmount)
        {
            if (withAmount <= 0)
            {
                return CheckOrCall();
            }

            return new PlayerAction(withAmount);
        }

        public override string ToString()
        {
            if (this.Type == (int)PlayerActionType.Raise)
            {
                return $"{this.Type}({this.Money})";
            }
            else
            {
                return this.Type.ToString();
            }
        }
    }

    public struct PlayerActionName
    {
        public PlayerActionName(string playerName, PlayerAction action)
        {
            this.PlayerName = playerName;
            this.Action = action;
        }

        public string PlayerName { get; }

        public PlayerAction Action { get; }
    }

    public enum PlayerActionType
    {
        Fold = 0,
        CheckCall = 1,
        Raise = 2,
    }
}
