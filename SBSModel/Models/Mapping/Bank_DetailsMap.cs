using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Bank_DetailsMap : EntityTypeConfiguration<Bank_Details>
    {
        public Bank_DetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.Bank_Detail_ID);

            // Properties
            this.Property(t => t.Bank_Account_Number)
                .HasMaxLength(30);

            this.Property(t => t.Bank_Account_Owner)
                .HasMaxLength(150);

            this.Property(t => t.Bank_Name)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Bank_Details");
            this.Property(t => t.Bank_Detail_ID).HasColumnName("Bank_Detail_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Bank_Account_Number).HasColumnName("Bank_Account_Number");
            this.Property(t => t.Bank_Account_Owner).HasColumnName("Bank_Account_Owner");
            this.Property(t => t.Bank_Name).HasColumnName("Bank_Name");
            this.Property(t => t.Display_On_Reports).HasColumnName("Display_On_Reports");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.Bank_Details)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
