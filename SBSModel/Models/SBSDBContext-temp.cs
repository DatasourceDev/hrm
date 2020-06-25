//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using Authentication.Models.Mapping;
//using Microsoft.AspNet.Identity.EntityFramework;
//namespace Authentication.Models
//{
//    public partial class SBS2DBContext : IdentityDbContext<ApplicationUser>
//    {
//        static SBS2DBContext()
//        {
//            Database.SetInitializer<SBS2DBContext>(null);
//        }

//        public SBS2DBContext()
//            : base("Name=SBS2DBContext")
//        {
//        }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);
//          
//        }
//    }
//}
