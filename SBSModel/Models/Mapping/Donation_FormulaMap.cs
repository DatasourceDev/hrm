using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Donation_FormulaMap : EntityTypeConfiguration<Donation_Formula>
    {
        public Donation_FormulaMap()
        {
            // Primary Key
            this.HasKey(t => t.Donation_Formula_ID);

            // Properties
            this.Property(t => t.Formula_Name)
                .HasMaxLength(500);

            this.Property(t => t.Formula_Desc)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Donation_Formula");
            this.Property(t => t.Donation_Formula_ID).HasColumnName("Donation_Formula_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Donation_Type_ID).HasColumnName("Donation_Type_ID");
            this.Property(t => t.Race).HasColumnName("Race");
            this.Property(t => t.Formula).HasColumnName("Formula");
            this.Property(t => t.Formula_Name).HasColumnName("Formula_Name");
            this.Property(t => t.Formula_Desc).HasColumnName("Formula_Desc");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Donation_Formula)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Donation_Type)
                .WithMany(t => t.Donation_Formula)
                .HasForeignKey(d => d.Donation_Type_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Donation_Formula)
                .HasForeignKey(d => d.Race);

        }
    }
}
