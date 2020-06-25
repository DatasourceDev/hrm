using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_AuthenticationMap : EntityTypeConfiguration<User_Authentication>
    {
        public User_AuthenticationMap()
        {
            // Primary Key
            this.HasKey(t => t.User_Authentication_ID);

            // Properties
            this.Property(t => t.Email_Address)
                .HasMaxLength(300);

            this.Property(t => t.ApplicationUser_Id)
                .HasMaxLength(128);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.User_Name)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("User_Authentication");
            this.Property(t => t.User_Authentication_ID).HasColumnName("User_Authentication_ID");
            this.Property(t => t.Email_Address).HasColumnName("Email_Address");
            this.Property(t => t.PWD).HasColumnName("PWD");
            this.Property(t => t.Login_Attempt).HasColumnName("Login_Attempt");
            this.Property(t => t.Activated).HasColumnName("Activated");
            this.Property(t => t.ApplicationUser_Id).HasColumnName("ApplicationUser_Id");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.User_Name).HasColumnName("User_Name");
            this.Property(t => t.Is_Email).HasColumnName("Is_Email");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.User_Authentication)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
