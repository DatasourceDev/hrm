using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Employment_History_AllowanceMap : EntityTypeConfiguration<Employment_History_Allowance>
    {
        public Employment_History_AllowanceMap()
        {
            // Primary Key
            this.HasKey(t => t.Employment_History_Allowance_ID);

            // Properties
            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Employment_History_Allowance");
            this.Property(t => t.Employment_History_Allowance_ID).HasColumnName("Employment_History_Allowance_ID");
            this.Property(t => t.History_ID).HasColumnName("History_ID");
            this.Property(t => t.PRT_ID).HasColumnName("PRT_ID");
            this.Property(t => t.PRC_ID).HasColumnName("PRC_ID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Employment_History)
                .WithMany(t => t.Employment_History_Allowance)
                .HasForeignKey(d => d.History_ID);
            this.HasOptional(t => t.PRC)
                .WithMany(t => t.Employment_History_Allowance)
                .HasForeignKey(d => d.PRC_ID);
            this.HasOptional(t => t.PRT)
                .WithMany(t => t.Employment_History_Allowance)
                .HasForeignKey(d => d.PRT_ID);

        }
    }
}
