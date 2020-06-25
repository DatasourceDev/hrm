using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class IR8A_BankMap : EntityTypeConfiguration<IR8A_Bank>
    {
        public IR8A_BankMap()
        {
            // Primary Key
            this.HasKey(t => t.Bank_Code);

            // Properties
            this.Property(t => t.Bank_Code)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IR8A_Bank");
            this.Property(t => t.Bank_Code).HasColumnName("Bank_Code");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SBS_Banking_Info_ID).HasColumnName("SBS_Banking_Info_ID");

            // Relationships
            this.HasOptional(t => t.Banking_Info)
                .WithMany(t => t.IR8A_Bank)
                .HasForeignKey(d => d.SBS_Banking_Info_ID);

        }
    }
}
