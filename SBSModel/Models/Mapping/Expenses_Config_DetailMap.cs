using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_Config_DetailMap : EntityTypeConfiguration<Expenses_Config_Detail>
    {
        public Expenses_Config_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Expenses_Config_Detail_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Expenses_Config_Detail");
            this.Property(t => t.Expenses_Config_Detail_ID).HasColumnName("Expenses_Config_Detail_ID");
            this.Property(t => t.Expenses_Config_ID).HasColumnName("Expenses_Config_ID");
            this.Property(t => t.Designation_ID).HasColumnName("Designation_ID");
            this.Property(t => t.Amount_Per_Year).HasColumnName("Amount_Per_Year");
            this.Property(t => t.Select_Pecentage).HasColumnName("Select_Pecentage");
            this.Property(t => t.Select_Amount).HasColumnName("Select_Amount");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Pecentage).HasColumnName("Pecentage");
            this.Property(t => t.Year_Service).HasColumnName("Year_Service");
            this.Property(t => t.Group_ID).HasColumnName("Group_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Amount_Per_Month).HasColumnName("Amount_Per_Month");
            this.Property(t => t.Select_Per_Month).HasColumnName("Select_Per_Month");

            // Relationships
            this.HasOptional(t => t.Designation)
                .WithMany(t => t.Expenses_Config_Detail)
                .HasForeignKey(d => d.Designation_ID);
            this.HasOptional(t => t.Expenses_Config)
                .WithMany(t => t.Expenses_Config_Detail)
                .HasForeignKey(d => d.Expenses_Config_ID);

        }
    }
}
