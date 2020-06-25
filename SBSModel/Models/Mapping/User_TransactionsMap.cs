using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class User_TransactionsMap : EntityTypeConfiguration<User_Transactions>
    {
        public User_TransactionsMap()
        {
            // Primary Key
            this.HasKey(t => t.Transaction_ID);

            // Properties
            this.Property(t => t.Activate_By)
                .HasMaxLength(50);

            this.Property(t => t.Deactivate_By)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("User_Transactions");
            this.Property(t => t.Transaction_ID).HasColumnName("Transaction_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Activate_On).HasColumnName("Activate_On");
            this.Property(t => t.Deactivate_On).HasColumnName("Deactivate_On");
            this.Property(t => t.Activate_By).HasColumnName("Activate_By");
            this.Property(t => t.Deactivate_By).HasColumnName("Deactivate_By");

            // Relationships
            this.HasRequired(t => t.Company)
                .WithMany(t => t.User_Transactions)
                .HasForeignKey(d => d.Company_ID);
            this.HasRequired(t => t.User_Profile)
                .WithMany(t => t.User_Transactions)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
