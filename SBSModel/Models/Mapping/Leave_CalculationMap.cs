using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_CalculationMap : EntityTypeConfiguration<Leave_Calculation>
    {
        public Leave_CalculationMap()
        {
            // Primary Key
            this.HasKey(t => t.Calculation_ID);

            // Properties
            this.Property(t => t.Year_Assigned)
                .HasMaxLength(4);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Leave_Calculation");
            this.Property(t => t.Calculation_ID).HasColumnName("Calculation_ID");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Entitle).HasColumnName("Entitle");
            this.Property(t => t.Adjustment).HasColumnName("Adjustment");
            this.Property(t => t.Bring_Forward).HasColumnName("Bring_Forward");
            this.Property(t => t.CutOff).HasColumnName("CutOff");
            this.Property(t => t.Leave_Used).HasColumnName("Leave_Used");
            this.Property(t => t.Expiry_Date).HasColumnName("Expiry_Date");
            this.Property(t => t.Year_Assigned).HasColumnName("Year_Assigned");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Relationship_ID).HasColumnName("Relationship_ID");

            // Relationships
            this.HasOptional(t => t.Designation)
                .WithMany(t => t.Leave_Calculation)
                .HasForeignKey(d => d.Designation_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Leave_Calculation)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Relationship)
                .WithMany(t => t.Leave_Calculation)
                .HasForeignKey(d => d.Relationship_ID);
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Calculation)
                .HasForeignKey(d => d.Leave_Config_ID);

        }
    }
}
