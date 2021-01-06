﻿namespace Mtcg
{
    public class Stats
    {
        public string Username { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public float WinningRate => (float)Wins/(float)Losses;
        public int PlayedGames => Wins + Losses;
    }

}