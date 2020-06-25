using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_ConfigMap : EntityTypeConfiguration<Leave_Config>
    {
        public Leave_ConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Config_ID);

            // Properties
            this.Property(t => t.Leave_Name)
                .HasMaxLength(300);

            this.Property(t => t.Leave_Description)
                .HasMaxLength(300);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Type)
                .HasMaxLength(50);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Leave_Config");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Leave_Name).HasColumnName("Leave_Name");
            this.Property(t => t.Leave_Description).HasColumnName("Leave_Description");
            this.Property(t => t.Bring_Forward).HasColumnName("Bring_Forward");
            this.Property(t => t.Bring_Forward_Percent).HasColumnName("Bring_Forward_Percent");
            this.Property(t => t.Upload_Document).HasColumnName("Upload_Document");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Deduct_In_Payroll).HasColumnName("Deduct_In_Payroll");
            this.Property(t => t.Months_To_Expiry).HasColumnName("Months_To_Expiry");
            this.Property(t => t.Allowed_Probation).HasColumnName("Allowed_Probation");
            this.Property(t => t.Bring_Forward_Days).HasColumnName("Bring_Forward_Days");
            this.Property(t => t.Is_Bring_Forward_Days).HasColumnName("Is_Bring_Forward_Days");
            this.Property(t => t.Is_Default).HasColumnName("Is_Default");
            this.Property(t => t.Flexibly).HasColumnName("Flexibly");
            this.Property(t => t.Continuously).HasColumnName("Continuously");
            this.Property(t => t.Valid_Period).HasColumnName("Valid_Period");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Allowed_Notice_Period).HasColumnName("Allowed_Notice_Period");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Leave_Config_Parent_ID).HasColumnName("Leave_Config_Parent_ID");
            this.Property(t => t.Is_Accumulative).HasColumnName("Is_Accumulative");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Leave_Config)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Leave_Config2)
                .WithMany(t => t.Leave_Config1)
                .HasForeignKey(d => d.Leave_Config_Parent_ID);

        }
    }
}
