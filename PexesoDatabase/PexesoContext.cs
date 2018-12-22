using System.Data.Entity;


namespace PexesoDatabase
{
    public class PexesoContext : DbContext
    {
        public virtual DbSet<PexesoPlayer> PexesoPlayers { get; set; }
        public virtual DbSet<Game> Games { get; set; }

        public PexesoContext() : base("name=PexesoContext")
        {

        }
    }
}
