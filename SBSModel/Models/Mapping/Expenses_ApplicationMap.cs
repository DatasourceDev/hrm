using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Expenses_ApplicationMap : EntityTypeConfiguration<Expenses_Application>
    {
        public Expenses_ApplicationMap()
        {
            // Primary Key
            this.HasKey(t => t.Expenses_Application_ID);

            // Properties
            this.Property(t => t.Expenses_No)
                .HasMaxLength(150);

            this.Property(t => t.Expenses_Title)
                .HasMaxLength(300);

            this.Property(t => t.Overall_Status)
                .HasMaxLength(30);

            this.Property(t => t.Approval_Status_1st)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Approval_Status_2st)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Approval_Cancel_Status)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Cancel_Status)
                .HasMaxLength(50);

            this.Property(t => t.Approver)
                .HasMaxLength(150);

            this.Property(t => t.Next_Approver)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Expenses_Application");
            this.Property(t => t.Expenses_Application_ID).HasColumnName("Expenses_Application_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Expenses_No).HasColumnName("Expenses_No");
            this.Property(t => t.Expenses_Title).HasColumnName("Expenses_Title");
            this.Property(t => t.Date_Applied).HasColumnName("Date_Applied");
            this.Property(t => t.Overall_Status).HasColumnName("Overall_Status");
            this.Property(t => t.Approval_Status_1st).HasColumnName("Approval_Status_1st");
            this.Property(t => t.Approval_Status_2st).HasColumnName("Approval_Status_2st");
            this.Property(t => t.Approval_Cancel_Status).HasColumnName("Approval_Cancel_Status");
            this.Property(t => t.Last_Date_Approved).HasColumnName("Last_Date_Approved");
            this.Property(t => t.Request_ID).HasColumnName("Request_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Cancel_Status).HasColumnName("Cancel_Status");
            this.Property(t => t.Request_Cancel_ID).HasColumnName("Request_Cancel_ID");
            this.Property(t => t.Supervisor).HasColumnName("Supervisor");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Approver).HasColumnName("Approver");
            this.Property(t => t.Next_Approver).HasColumnName("Next_Approver");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Expenses_Application)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Expenses_Application)
                .HasForeignKey(d => d.Employee_Profile_ID);

        }
    }
}
