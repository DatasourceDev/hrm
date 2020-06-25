using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSWorkFlowAPI.Models.Mapping
{
    public class RequestMap : EntityTypeConfiguration<Request>
    {
        public RequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Request_ID);

            // Properties
            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Requestor_Name)
                .HasMaxLength(100);

            this.Property(t => t.Requestor_Email)
                .HasMaxLength(50);

            this.Property(t => t.Approval_Type)
                .HasMaxLength(50);

            this.Property(t => t.Module)
                .HasMaxLength(50);

            this.Property(t => t.Request_Type)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Request");
            this.Property(t => t.Request_ID).HasColumnName("Request_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Approval_Flow_ID).HasColumnName("Approval_Flow_ID");
            this.Property(t => t.Approval_Level).HasColumnName("Approval_Level");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Requestor_Profile_ID).HasColumnName("Requestor_Profile_ID");
            this.Property(t => t.Requestor_Name).HasColumnName("Requestor_Name");
            this.Property(t => t.Requestor_Email).HasColumnName("Requestor_Email");
            this.Property(t => t.Request_Date).HasColumnName("Request_Date");
            this.Property(t => t.Last_Action_Date).HasColumnName("Last_Action_Date");
            this.Property(t => t.Approval_Type).HasColumnName("Approval_Type");
            this.Property(t => t.Module).HasColumnName("Module");
            this.Property(t => t.Leave_Application_Document_ID).HasColumnName("Leave_Application_Document_ID");
            this.Property(t => t.Doc_ID).HasColumnName("Doc_ID");
            this.Property(t => t.Request_Type).HasColumnName("Request_Type");
            this.Property(t => t.Is_Indent).HasColumnName("Is_Indent");

            // Relationships
            this.HasRequired(t => t.Approval_Flow)
                .WithMany(t => t.Requests)
                .HasForeignKey(d => d.Approval_Flow_ID);

        }
    }
}
