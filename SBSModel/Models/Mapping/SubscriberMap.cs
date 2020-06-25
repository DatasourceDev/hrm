using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class SubscriberMap : EntityTypeConfiguration<Subscriber>
    {
        public SubscriberMap()
        {
            // Primary Key
            this.HasKey(t => t.Subscriber_ID);

            // Properties
            this.Property(t => t.Subscriber_Email)
                .HasMaxLength(150);

            this.Property(t => t.Subscriber_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Subscriber");
            this.Property(t => t.Subscriber_ID).HasColumnName("Subscriber_ID");
            this.Property(t => t.Subscriber_Email).HasColumnName("Subscriber_Email");
            this.Property(t => t.Subscriber_Status).HasColumnName("Subscriber_Status");
            this.Property(t => t.Subscribed_On).HasColumnName("Subscribed_On");
        }
    }
}
