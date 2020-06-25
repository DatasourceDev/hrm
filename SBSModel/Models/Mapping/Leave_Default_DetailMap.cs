using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Default_DetailMap : EntityTypeConfiguration<Leave_Default_Detail>
    {
        public Leave_Default_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Default_Detail_ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Leave_Default_Detail");
            this.Property(t => t.Default_Detail_ID).HasColumnName("Default_Detail_ID");
            this.Property(t => t.Default_ID).HasColumnName("Default_ID");
            this.Property(t => t.Default_Leave_Amount).HasColumnName("Default_Leave_Amount");
            this.Property(t => t.Year_Service).HasColumnName("Year_Service");
            this.Property(t => t.Bring_Forward_Days).HasColumnName("Bring_Forward_Days");
            this.Property(t => t.Is_Bring_Forward_Percent).HasColumnName("Is_Bring_Forward_Percent");

            // Relationships
            this.HasOptional(t => t.Leave_Default)
                .WithMany(t => t.Leave_Default_Detail)
                .HasForeignKey(d => d.Default_ID);

        }
    }
}
