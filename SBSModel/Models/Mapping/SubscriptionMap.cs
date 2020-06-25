using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class SubscriptionMap : EntityTypeConfiguration<Subscription>
    {
        public SubscriptionMap()
        {
            // Primary Key
            this.HasKey(t => t.Subscription_ID);

            // Properties
            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Module_Code)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Subscription");
            this.Property(t => t.Subscription_ID).HasColumnName("Subscription_ID");
            this.Property(t => t.Module_Detail_ID).HasColumnName("Module_Detail_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.Subscription_Period).HasColumnName("Subscription_Period");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Period_Day).HasColumnName("Period_Day");
            this.Property(t => t.No_Of_Users).HasColumnName("No_Of_Users");
            this.Property(t => t.Period_Month).HasColumnName("Period_Month");
            this.Property(t => t.Period_Year).HasColumnName("Period_Year");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Module_Code).HasColumnName("Module_Code");
            this.Property(t => t.Price).HasColumnName("Price");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Module)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(d => d.Module_Code);
            this.HasOptional(t => t.SBS_Module_Detail)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(d => d.Module_Detail_ID);

        }
    }
}
