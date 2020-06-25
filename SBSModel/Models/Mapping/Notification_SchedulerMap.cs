using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Notification_SchedulerMap : EntityTypeConfiguration<Notification_Scheduler>
    {
        public Notification_SchedulerMap()
        {
            // Primary Key
            this.HasKey(t => t.Notification_Scheduler_ID);

            // Properties
            this.Property(t => t.Notice_Type)
                .HasMaxLength(150);

            this.Property(t => t.Trigger_Period)
                .HasMaxLength(50);

            this.Property(t => t.Selected_Days)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Notification_Scheduler");
            this.Property(t => t.Notification_Scheduler_ID).HasColumnName("Notification_Scheduler_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Notice_Type).HasColumnName("Notice_Type");
            this.Property(t => t.Trigger_Set_Up).HasColumnName("Trigger_Set_Up");
            this.Property(t => t.Trigger_Period).HasColumnName("Trigger_Period");
            this.Property(t => t.Start_Time).HasColumnName("Start_Time");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.Recur_Every_Days).HasColumnName("Recur_Every_Days");
            this.Property(t => t.Recur_Every_Weeks).HasColumnName("Recur_Every_Weeks");
            this.Property(t => t.Selected_Sunday).HasColumnName("Selected_Sunday");
            this.Property(t => t.Selected_Monday).HasColumnName("Selected_Monday");
            this.Property(t => t.Selected_Tuesday).HasColumnName("Selected_Tuesday");
            this.Property(t => t.Selected_Wednesday).HasColumnName("Selected_Wednesday");
            this.Property(t => t.Selected_Thursday).HasColumnName("Selected_Thursday");
            this.Property(t => t.Selected_Friday).HasColumnName("Selected_Friday");
            this.Property(t => t.Selected_Saturday).HasColumnName("Selected_Saturday");
            this.Property(t => t.Selected_Months).HasColumnName("Selected_Months");
            this.Property(t => t.Selected_Days).HasColumnName("Selected_Days");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Notification_Scheduler)
                .HasForeignKey(d => d.Company_ID);

        }
    }
}
