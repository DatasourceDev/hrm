using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Banking_InfoMap : EntityTypeConfiguration<Banking_Info>
    {
        public Banking_InfoMap()
        {
            // Primary Key
            this.HasKey(t => t.Banking_Info_ID);

            // Properties
            this.Property(t => t.Bank_Name)
                .HasMaxLength(150);

            this.Property(t => t.Bank_Account)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Account_Name)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Banking_Info");
            this.Property(t => t.Banking_Info_ID).HasColumnName("Banking_Info_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Bank_Name).HasColumnName("Bank_Name");
            this.Property(t => t.Bank_Account).HasColumnName("Bank_Account");
            this.Property(t => t.Payment_Type).HasColumnName("Payment_Type");
            this.Property(t => t.Effective_Date).HasColumnName("Effective_Date");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Account_Name).HasColumnName("Account_Name");
            this.Property(t => t.Selected).HasColumnName("Selected");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Banking_Info)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Banking_Info)
                .HasForeignKey(d => d.Payment_Type);
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.Banking_Info)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
