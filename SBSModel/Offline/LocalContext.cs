using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SBSModel.Offline
{
    public partial class LocalContext : DbContext
    {
        static LocalContext()
        {
            Database.SetInitializer<LocalContext>(null);
        }

        public LocalContext()
            : base("Name=LocalContext")
        {
        }

      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
    }

    public partial class LocalInitialContext : DbContext
    {
        static LocalInitialContext()
        {
            Database.SetInitializer<LocalInitialContext>(null);
        }

        public LocalInitialContext()
            : base("Name=LocalInitialContext")
        {
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }
}
