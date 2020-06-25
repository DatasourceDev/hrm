using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Activation_LinkMap : EntityTypeConfiguration<Activation_Link>
    {
        public Activation_LinkMap()
        {
            // Primary Key
            this.HasKey(t => t.Activation_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Activation_Link");
            this.Property(t => t.Activation_ID).HasColumnName("Activation_ID");
            this.Property(t => t.User_Authentication_ID).HasColumnName("User_Authentication_ID");
            this.Property(t => t.Activation_Code).HasColumnName("Activation_Code");
            this.Property(t => t.Time_Limit).HasColumnName("Time_Limit");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.User_Authentication)
                .WithMany(t => t.Activation_Link)
                .HasForeignKey(d => d.User_Authentication_ID);

        }
    }
}
