using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class RelationshipMap : EntityTypeConfiguration<Relationship>
    {
        public RelationshipMap()
        {
            // Primary Key
            this.HasKey(t => t.Relationship_ID);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.NRIC)
                .HasMaxLength(30);

            this.Property(t => t.Company_Name)
                .HasMaxLength(50);

            this.Property(t => t.Company_Position)
                .HasMaxLength(50);

            this.Property(t => t.Passport)
                .HasMaxLength(100);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Child_Type)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Relationship");
            this.Property(t => t.Relationship_ID).HasColumnName("Relationship_ID");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Nationality_ID).HasColumnName("Nationality_ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Relationship1).HasColumnName("Relationship");
            this.Property(t => t.DOB).HasColumnName("DOB");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Working).HasColumnName("Working");
            this.Property(t => t.Company_Name).HasColumnName("Company_Name");
            this.Property(t => t.Company_Position).HasColumnName("Company_Position");
            this.Property(t => t.Passport).HasColumnName("Passport");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Child_Type).HasColumnName("Child_Type");
            this.Property(t => t.Is_Maternity).HasColumnName("Is_Maternity");
            this.Property(t => t.Is_Maternity_Share_Father).HasColumnName("Is_Maternity_Share_Father");
            this.Property(t => t.Is_Paternity).HasColumnName("Is_Paternity");

            // Relationships
            this.HasOptional(t => t.Employee_Profile)
                .WithMany(t => t.Relationships)
                .HasForeignKey(d => d.Employee_Profile_ID);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Relationships)
                .HasForeignKey(d => d.Gender);
            this.HasOptional(t => t.Global_Lookup_Data1)
                .WithMany(t => t.Relationships1)
                .HasForeignKey(d => d.Relationship1);
            this.HasOptional(t => t.Nationality)
                .WithMany(t => t.Relationships)
                .HasForeignKey(d => d.Nationality_ID);
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.Relationships)
                .HasForeignKey(d => d.Profile_ID);

        }
    }
}
