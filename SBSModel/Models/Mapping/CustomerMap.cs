using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            this.HasKey(t => t.Customer_ID);

            // Properties
            this.Property(t => t.Customer_No)
                .HasMaxLength(150);

            this.Property(t => t.Customer_Name)
                .HasMaxLength(300);

            this.Property(t => t.Person_In_Charge)
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Mobile_Phone)
                .HasMaxLength(100);

            this.Property(t => t.Office_Phone)
                .HasMaxLength(100);

            this.Property(t => t.Website)
                .HasMaxLength(150);

            this.Property(t => t.Billing_Address)
                .HasMaxLength(500);

            this.Property(t => t.Billing_Street)
                .HasMaxLength(500);

            this.Property(t => t.Billing_City)
                .HasMaxLength(500);

            this.Property(t => t.Billing_Postal_Code)
                .HasMaxLength(10);

            this.Property(t => t.Record_Status)
                .HasMaxLength(50);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            this.Property(t => t.Fax)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Customer");
            this.Property(t => t.Customer_ID).HasColumnName("Customer_ID");
            this.Property(t => t.Company_ID).HasColumnName("Company_ID");
            this.Property(t => t.Customer_No).HasColumnName("Customer_No");
            this.Property(t => t.Customer_Name).HasColumnName("Customer_Name");
            this.Property(t => t.Person_In_Charge).HasColumnName("Person_In_Charge");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Mobile_Phone).HasColumnName("Mobile_Phone");
            this.Property(t => t.Office_Phone).HasColumnName("Office_Phone");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.Billing_Address).HasColumnName("Billing_Address");
            this.Property(t => t.Billing_Street).HasColumnName("Billing_Street");
            this.Property(t => t.Billing_City).HasColumnName("Billing_City");
            this.Property(t => t.Billing_Country_ID).HasColumnName("Billing_Country_ID");
            this.Property(t => t.Billing_State_ID).HasColumnName("Billing_State_ID");
            this.Property(t => t.Billing_Postal_Code).HasColumnName("Billing_Postal_Code");
            this.Property(t => t.Record_Status).HasColumnName("Record_Status");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Fax).HasColumnName("Fax");

            // Relationships
            this.HasOptional(t => t.Company)
                .WithMany(t => t.Customers)
                .HasForeignKey(d => d.Company_ID);
            this.HasOptional(t => t.Country)
                .WithMany(t => t.Customers)
                .HasForeignKey(d => d.Billing_Country_ID);
            this.HasOptional(t => t.State)
                .WithMany(t => t.Customers)
                .HasForeignKey(d => d.Billing_State_ID);

        }
    }
}
