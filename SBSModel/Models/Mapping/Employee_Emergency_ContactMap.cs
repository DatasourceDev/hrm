using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Employee_Emergency_ContactMap : EntityTypeConfiguration<Employee_Emergency_Contact>
    {
        public Employee_Emergency_ContactMap()
        {
            // Primary Key
            this.HasKey(t => t.Employee_Emergency_Contact_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Contact_No)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Employee_Emergency_Contact");
            this.Property(t => t.Employee_Emergency_Contact_ID).HasColumnName("Employee_Emergency_Contact_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Contact_No).HasColumnName("Contact_No");
            this.Property(t => t.Relationship).HasColumnName("Relationship");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Employee_Emergency_Contact)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Employee_Emergency_Contact)
                .HasForeignKey(d => d.Relationship);

        }
    }
}
