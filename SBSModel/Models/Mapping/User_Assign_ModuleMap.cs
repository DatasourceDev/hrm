using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_Assign_ModuleMap : EntityTypeConfiguration<User_Assign_Module>
    {
        public User_Assign_ModuleMap()
        {
            // Primary Key
            this.HasKey(t => t.User_Assign_Module_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("User_Assign_Module");
            this.Property(t => t.User_Assign_Module_ID).HasColumnName("User_Assign_Module_ID");
            this.Property(t => t.Subscription_ID).HasColumnName("Subscription_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");

            // Relationships
            this.HasOptional(t => t.Subscription)
                .WithMany(t => t.User_Assign_Module)
                .HasForeignKey(d => d.Subscription_ID);
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.User_Assign_Module)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
