using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Leave_Application_DocumentMap : EntityTypeConfiguration<Leave_Application_Document>
    {
        public Leave_Application_DocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.Leave_Application_Document_ID);

            // Properties
            this.Property(t => t.Reasons)
                .HasMaxLength(300);

            this.Property(t => t.Address_While_On_Leave)
                .HasMaxLength(300);

            this.Property(t => t.Contact_While_Overseas)
                .HasMaxLength(30);

            this.Property(t => t.Period)
                .HasMaxLength(30);

            this.Property(t => t.Remark)
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

            this.Property(t => t.Start_Date_Period)
                .HasMaxLength(2);

            this.Property(t => t.End_Date_Period)
                .HasMaxLength(2);

            this.Property(t => t.Payroll_Flag)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Second_Contact_While_Overseas)
                .HasMaxLength(30);

            this.Property(t => t.Cancel_Status)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Leave_Application_Document");
            this.Property(t => t.Leave_Application_Document_ID).HasColumnName("Leave_Application_Document_ID");
            this.Property(t => t.Leave_Config_ID).HasColumnName("Leave_Config_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Start_Date).HasColumnName("Start_Date");
            this.Property(t => t.End_Date).HasColumnName("End_Date");
            this.Property(t => t.Reasons).HasColumnName("Reasons");
            this.Property(t => t.Last_Date_Approved).HasColumnName("Last_Date_Approved");
            this.Property(t => t.Address_While_On_Leave).HasColumnName("Address_While_On_Leave");
            this.Property(t => t.Contact_While_Overseas).HasColumnName("Contact_While_Overseas");
            this.Property(t => t.Period).HasColumnName("Period");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Date_Applied).HasColumnName("Date_Applied");
            this.Property(t => t.Overall_Status).HasColumnName("Overall_Status");
            this.Property(t => t.Approval_Status_1st).HasColumnName("Approval_Status_1st");
            this.Property(t => t.Approval_Status_2st).HasColumnName("Approval_Status_2st");
            this.Property(t => t.Approval_Cancel_Status).HasColumnName("Approval_Cancel_Status");
            this.Property(t => t.Start_Date_Period).HasColumnName("Start_Date_Period");
            this.Property(t => t.End_Date_Period).HasColumnName("End_Date_Period");
            this.Property(t => t.Days_Taken).HasColumnName("Days_Taken");
            this.Property(t => t.Payroll_Flag).HasColumnName("Payroll_Flag");
            this.Property(t => t.Processed_Day).HasColumnName("Processed_Day");
            this.Property(t => t.Balance_Day).HasColumnName("Balance_Day");
            this.Property(t => t.Leave_Config_Detail_ID).HasColumnName("Leave_Config_Detail_ID");
            this.Property(t => t.Request_ID).HasColumnName("Request_ID");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Second_Contact_While_Overseas).HasColumnName("Second_Contact_While_Overseas");
            this.Property(t => t.Relationship_ID).HasColumnName("Relationship_ID");
            this.Property(t => t.Weeks_Taken).HasColumnName("Weeks_Taken");
            this.Property(t => t.Cancel_Status).HasColumnName("Cancel_Status");
            this.Property(t => t.Request_Cancel_ID).HasColumnName("Request_Cancel_ID");
            this.Property(t => t.Supervisor).HasColumnName("Supervisor");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Leave_Application_Document)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Relationship)
                .WithMany(t => t.Leave_Application_Document)
                .HasForeignKey(d => d.Relationship_ID);
            this.HasOptional(t => t.Leave_Config)
                .WithMany(t => t.Leave_Application_Document)
                .HasForeignKey(d => d.Leave_Config_ID);
            this.HasOptional(t => t.Leave_Config_Detail)
                .WithMany(t => t.Leave_Application_Document)
                .HasForeignKey(d => d.Leave_Config_Detail_ID);

        }
    }
}
