using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SBSTimeModel.Models.Mapping;

namespace SBSTimeModel.Models
{
    public partial class SBS2TimeDBContext : DbContext
    {
        static SBS2TimeDBContext()
        {
            Database.SetInitializer<SBS2TimeDBContext>(null);
        }

        public SBS2TimeDBContext()
            : base("Name=SBS2TimeDBContext")
        {
        }

        public DbSet<Time_Arrangement> Time_Arrangement { get; set; }
        public DbSet<Time_Device> Time_Device { get; set; }
        public DbSet<Time_Device_Map> Time_Device_Map { get; set; }
        public DbSet<Time_Transaction> Time_Transaction { get; set; }
        public DbSet<ZK_Users> ZK_Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Time_ArrangementMap());
            modelBuilder.Configurations.Add(new Time_DeviceMap());
            modelBuilder.Configurations.Add(new Time_Device_MapMap());
            modelBuilder.Configurations.Add(new Time_TransactionMap());
            modelBuilder.Configurations.Add(new ZK_UsersMap());
        }
    }
}
