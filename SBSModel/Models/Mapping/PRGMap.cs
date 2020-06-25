using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class PRGMap : EntityTypeConfiguration<PRG>
    {
        public PRGMap()
        {
            // Primary Key
            this.HasKey(t => t.PRG_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PRG");
            this.Property(t => t.PRG_ID).HasColumnName("PRG_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.PRGs)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
