using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Donation_TypeMap : EntityTypeConfiguration<Donation_Type>
    {
        public Donation_TypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Donation_Type_ID);

            // Properties
            this.Property(t => t.Donation_Name)
                .HasMaxLength(150);

            this.Property(t => t.Donation_Description)
                .HasMaxLength(500);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Donation_Type");
            this.Property(t => t.Donation_Type_ID).HasColumnName("Donation_Type_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Donation_Name).HasColumnName("Donation_Name");
            this.Property(t => t.Donation_Description).HasColumnName("Donation_Description");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Donation_Type)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
