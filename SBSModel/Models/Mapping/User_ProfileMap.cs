using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_ProfileMap : EntityTypeConfiguration<User_Profile>
    {
        public User_ProfileMap()
        {
            // Primary Key
            this.HasKey(t => t.Profile_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.User_Status)
                .HasMaxLength(100);

            this.Property(t => t.First_Name)
                .HasMaxLength(150);

            this.Property(t => t.Middle_Name)
                .HasMaxLength(150);

            this.Property(t => t.Last_Name)
                .HasMaxLength(150);

            this.Property(t => t.Phone)
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .HasMaxLength(150);

            this.Property(t => t.Bg)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.A7_User_ID)
                .HasMaxLength(50);

            this.Property(t => t.User_Name)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("User_Profile");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.User_Authentication_ID).HasColumnName("User_Authentication_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Registration_Date).HasColumnName("Registration_Date");
            this.Property(t => t.User_Status).HasColumnName("User_Status");
            this.Property(t => t.Latest_Connection).HasColumnName("Latest_Connection");
            this.Property(t => t.First_Name).HasColumnName("First_Name");
            this.Property(t => t.Middle_Name).HasColumnName("Middle_Name");
            this.Property(t => t.Last_Name).HasColumnName("Last_Name");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Bg).HasColumnName("Bg");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.A7_User_ID).HasColumnName("A7_User_ID");
            this.Property(t => t.User_Name).HasColumnName("User_Name");
            this.Property(t => t.Is_Email).HasColumnName("Is_Email");
            this.Property(t => t.Is_Tour_Skip).HasColumnName("Is_Tour_Skip");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.User_Profile)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.User_Authentication)
                .WithMany(t => t.User_Profile)
                .HasForeignKey(d => d.User_Authentication_ID);

        }
    }
}
