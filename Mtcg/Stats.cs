namespace Mtcg
{
    public class Stats
    {
        public string Username { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public string WinningRate {
            get
            {
                if ((float) PlayedGames != 0)
                {
                    float rate = ((float) Wins / ((float) PlayedGames)) * 100;
                    return rate + "%";
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        public int PlayedGames => Wins + Losses;
    }

}