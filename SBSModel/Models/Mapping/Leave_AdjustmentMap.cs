using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_AdjustmentMap : EntityTypeConfiguration<Leave_Adjustment>
    {
        public Leave_AdjustmentMap()
        {
            // Primary Key
            this.HasKey(t => t.Adjustment_ID);

            // Properties
            this.Property(t => t.Reason)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Leave_Adjustment");
            this.Property(t => t.Adjustment_ID).HasColumnName("Adjustment_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Adjustment_Amount).HasColumnName("Adjustment_Amount");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Year_2).HasColumnName("Year_2");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Leave_Adjustment)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Department)
                .WithMany(t => t.Leave_Adjustment)
                .HasForeignKey(d => d.Department_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Leave_Adjustment)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Adjustment)
                .HasForeignKey(d => d.Leave_Config_ID);

        }
    }
}
