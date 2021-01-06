namespace Mtcg
{
    public class Battle
    {
        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public string Logger { get; set; }
        public int RoundsRemaining { get; set; }
        public int UserCount { get; set; }

        private bool IsSlotAvailable()
        {
            return UserCount < 2;
        }
        public bool AddUserToBattle(User player2)
        {
            if (this.IsSlotAvailable())
            {
                this.Player2 = player2;
                this.UserCount++;
                return true;
            }

            return false;

        }

    }
}