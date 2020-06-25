using Microsoft.AspNet.Identity.EntityFramework;
using Authentication.Models;

namespace Authentication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /*public int User_Authentication_ID { get; set; }
        public virtual User_Authentication User_Authentication { get; set; }*/

    }

    /*
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("SBSDBContext")
        {
        }
    }*/
}