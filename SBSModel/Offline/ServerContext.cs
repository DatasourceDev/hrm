using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SBSModel.Offline
{
    public partial class ServerContext : DbContext
    {
        static ServerContext()
        {
            Database.SetInitializer<ServerContext>(null);
        }

        public ServerContext()
            : base("Name=ServerContext")
        {
        }

      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }
}
