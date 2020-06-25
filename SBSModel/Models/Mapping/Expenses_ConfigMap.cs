using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_ConfigMap : EntityTypeConfiguration<Expenses_Config>
    {
        public Expenses_ConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.Expenses_Config_ID);

            // Properties
            this.Property(t => t.Expenses_Name)
                .HasMaxLength(300);

            this.Property(t => t.Expenses_Description)
                .HasMaxLength(300);

            this.Property(t => t.Claimable_Type)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Expenses_Config");
            this.Property(t => t.Expenses_Config_ID).HasColumnName("Expenses_Config_ID");
            this.Property(t => t.Department_ID).HasColumnName("Department_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Expenses_Name).HasColumnName("Expenses_Name");
            this.Property(t => t.Expenses_Description).HasColumnName("Expenses_Description");
            this.Property(t => t.Claimable_Type).HasColumnName("Claimable_Type");
            this.Property(t => t.Allowed_Probation).HasColumnName("Allowed_Probation");
            this.Property(t => t.Allowed_Over_Amount_Per_Year).HasColumnName("Allowed_Over_Amount_Per_Year");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Is_MileAge).HasColumnName("Is_MileAge");
            this.Property(t => t.UOM_ID).HasColumnName("UOM_ID");
            this.Property(t => t.Amount_Per_UOM).HasColumnName("Amount_Per_UOM");
            this.Property(t => t.Expenses_Category_ID).HasColumnName("Expenses_Category_ID");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Is_Accumulative).HasColumnName("Is_Accumulative");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Expenses_Config)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Department)
                .WithMany(t => t.Expenses_Config)
                .HasForeignKey(d => d.Department_ID);
            this.HasOptional(t => t.Expenses_Category)
                .WithMany(t => t.Expenses_Config)
                .HasForeignKey(d => d.Expenses_Category_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Expenses_Config)
                .HasForeignKey(d => d.UOM_ID);

        }
    }
}
