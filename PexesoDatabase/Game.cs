using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PexesoDatabase
{
    public enum GameSize
    {
        [Description("3x2")]
        size3x2 = 6,
        [Description("4x3")]
        size4x3 = 12,
        [Description("4x4")]
        size4x4 = 16,
        [Description("5x4")]
        size5x4 = 20,
        [Description("6x5")]
        size6x5 = 30,
        [Description("6X6")]
        size6x6 = 36,
        [Description("7X6")]
        size7x6 = 42,
        [Description("8X7")]
        size8x7 = 56,
        [Description("8X8")]
        size8x8 = 64,
    }
    [Serializable]
    public class Game
    {
        public Game()
        {
        }

        public Game(int id, GameSize gameSize, int countOfMovesCompetitor, int countOfUncoveredCardsCompetitor,
            int countOfUncoveredCardsChallenger, int countOfMovesChallenger, TimeSpan gameTime,
            string nickOfChallenger, string winner, string nickOfCompetitor)
        {
            Id = id;
            GameSize = gameSize;
            CountOfMovesCompetitor = countOfMovesCompetitor;
            CountOfUncoveredCardsCompetitor = countOfUncoveredCardsCompetitor;
            CountOfUncoveredCardsChallenger = countOfUncoveredCardsChallenger;
            CountOfMovesChallenger = countOfMovesChallenger;
            GameTime = gameTime;
            NickOfChallenger = nickOfChallenger;
            Winner = winner;
            NickOfCompetitor = nickOfCompetitor;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public GameSize GameSize { get; set; }
        public int CountOfMovesCompetitor { get; set; }
        public int CountOfUncoveredCardsCompetitor { get; set; }
        public int CountOfUncoveredCardsChallenger { get; set; }
        public int CountOfMovesChallenger { get; set; }
        public TimeSpan GameTime { get; set; }
        public string NickOfChallenger { get; set; }
        public string Winner { get; set; }
        public string NickOfCompetitor { get; set; }

    }
}
