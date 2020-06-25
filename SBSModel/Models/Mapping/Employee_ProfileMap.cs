using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class Employee_ProfileMap : EntityTypeConfiguration<Employee_Profile>
    {
        public Employee_ProfileMap()
        {
            // Primary Key
            this.HasKey(t => t.Employee_Profile_ID);

            // Properties
            this.Property(t => t.CVResume)
                .HasMaxLength(300);

            this.Property(t => t.Position_Apply)
                .HasMaxLength(100);

            this.Property(t => t.Mobile_No)
                .HasMaxLength(150);

            this.Property(t => t.Certification)
                .HasMaxLength(300);

            this.Property(t => t.Passport)
                .HasMaxLength(100);

            this.Property(t => t.Porsonal_Details)
                .HasMaxLength(300);

            this.Property(t => t.NRIC)
                .HasMaxLength(30);

            this.Property(t => t.Remark)
                .HasMaxLength(500);

            this.Property(t => t.Emp_Status)
                .HasMaxLength(150);

            this.Property(t => t.Emergency_Name)
                .HasMaxLength(150);

            this.Property(t => t.Emergency_Contact_No)
                .HasMaxLength(15);

            this.Property(t => t.Residential_No)
                .HasMaxLength(150);

            this.Property(t => t.Residential_Address_1)
                .HasMaxLength(300);

            this.Property(t => t.Residential_Address_2)
                .HasMaxLength(300);

            this.Property(t => t.Postal_Code_1)
                .HasMaxLength(20);

            this.Property(t => t.Postal_Code_2)
                .HasMaxLength(20);

            this.Property(t => t.FIN_No)
                .HasMaxLength(100);

            this.Property(t => t.Residential_Status)
                .HasMaxLength(100);

            this.Property(t => t.PR_No)
                .HasMaxLength(150);

            this.Property(t => t.Immigration_No)
                .HasMaxLength(150);

            this.Property(t => t.WP_No)
                .HasMaxLength(150);

            this.Property(t => t.Employee_No)
                .HasMaxLength(150);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            this.Property(t => t.Update_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Employee_Profile");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Profile_ID).HasColumnName("Profile_ID");
            this.Property(t => t.Nationality_ID).HasColumnName("Nationality_ID");
            this.Property(t => t.CVResume).HasColumnName("CVResume");
            this.Property(t => t.Position_Apply).HasColumnName("Position_Apply");
            this.Property(t => t.Mobile_No).HasColumnName("Mobile_No");
            this.Property(t => t.DOB).HasColumnName("DOB");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.Certification).HasColumnName("Certification");
            this.Property(t => t.Passport).HasColumnName("Passport");
            this.Property(t => t.Porsonal_Details).HasColumnName("Porsonal_Details");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Marital_Status).HasColumnName("Marital_Status");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Emp_Status).HasColumnName("Emp_Status");
            this.Property(t => t.Race).HasColumnName("Race");
            this.Property(t => t.Emergency_Name).HasColumnName("Emergency_Name");
            this.Property(t => t.Emergency_Contact_No).HasColumnName("Emergency_Contact_No");
            this.Property(t => t.Emergency_Relationship).HasColumnName("Emergency_Relationship");
            this.Property(t => t.Residential_No).HasColumnName("Residential_No");
            this.Property(t => t.Residential_Address_1).HasColumnName("Residential_Address_1");
            this.Property(t => t.Residential_Address_2).HasColumnName("Residential_Address_2");
            this.Property(t => t.Postal_Code_1).HasColumnName("Postal_Code_1");
            this.Property(t => t.Postal_Code_2).HasColumnName("Postal_Code_2");
            this.Property(t => t.FIN_No).HasColumnName("FIN_No");
            this.Property(t => t.Residential_Status).HasColumnName("Residential_Status");
            this.Property(t => t.PR_No).HasColumnName("PR_No");
            this.Property(t => t.PR_Start_Date).HasColumnName("PR_Start_Date");
            this.Property(t => t.PR_End_Date).HasColumnName("PR_End_Date");
            this.Property(t => t.Immigration_No).HasColumnName("Immigration_No");
            this.Property(t => t.WP_Class).HasColumnName("WP_Class");
            this.Property(t => t.WP_No).HasColumnName("WP_No");
            this.Property(t => t.WP_Start_Date).HasColumnName("WP_Start_Date");
            this.Property(t => t.WP_End_Date).HasColumnName("WP_End_Date");
            this.Property(t => t.Contract_Start_Date).HasColumnName("Contract_Start_Date");
            this.Property(t => t.Contract_End_Date).HasColumnName("Contract_End_Date");
            this.Property(t => t.Employee_No).HasColumnName("Employee_No");
            this.Property(t => t.Religion).HasColumnName("Religion");
            this.Property(t => t.Expiry_Date).HasColumnName("Expiry_Date");
            this.Property(t => t.Confirm_Date).HasColumnName("Confirm_Date");
            this.Property(t => t.Hired_Date).HasColumnName("Hired_Date");
            this.Property(t => t.Opt_Out).HasColumnName("Opt_Out");
            this.Property(t => t.NRIC_FIN_Issue_Date).HasColumnName("NRIC_FIN_Issue_Date");
            this.Property(t => t.NRIC_FIN_Expire_Date).HasColumnName("NRIC_FIN_Expire_Date");
            this.Property(t => t.Passport_Issue_Date).HasColumnName("Passport_Issue_Date");
            this.Property(t => t.Passpor_Expire_Date).HasColumnName("Passpor_Expire_Date");
            this.Property(t => t.Work_Pass_Type).HasColumnName("Work_Pass_Type");
            this.Property(t => t.Contribute_Rate1).HasColumnName("Contribute_Rate1");
            this.Property(t => t.Contribute_Rate2).HasColumnName("Contribute_Rate2");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
            this.Property(t => t.Update_By).HasColumnName("Update_By");
            this.Property(t => t.Update_On).HasColumnName("Update_On");
            this.Property(t => t.Residential_Country_1).HasColumnName("Residential_Country_1");
            this.Property(t => t.Residential_Country_2).HasColumnName("Residential_Country_2");

            // Relationships
            this.HasOptional(t => t.Country)
                .WithMany(t => t.Employee_Profile)
                .HasForeignKey(d => d.Residential_Country_1);
            this.HasOptional(t => t.Country1)
                .WithMany(t => t.Employee_Profile1)
                .HasForeignKey(d => d.Residential_Country_2);
            this.HasOptional(t => t.Global_Lookup_Data)
                .WithMany(t => t.Employee_Profile)
                .HasForeignKey(d => d.Emergency_Relationship);
            this.HasOptional(t => t.Global_Lookup_Data1)
                .WithMany(t => t.Employee_Profile1)
                .HasForeignKey(d => d.Gender);
            this.HasOptional(t => t.Global_Lookup_Data2)
                .WithMany(t => t.Employee_Profile2)
                .HasForeignKey(d => d.Marital_Status);
            this.HasOptional(t => t.Nationality)
                .WithMany(t => t.Employee_Profile)
                .HasForeignKey(d => d.Nationality_ID);
            this.HasOptional(t => t.Global_Lookup_Data3)
                .WithMany(t => t.Employee_Profile3)
                .HasForeignKey(d => d.Race);
            this.HasOptional(t => t.Global_Lookup_Data4)
                .WithMany(t => t.Employee_Profile4)
                .HasForeignKey(d => d.Religion);
            this.HasOptional(t => t.User_Profile)
                .WithMany(t => t.Employee_Profile)
                .HasForeignKey(d => d.Profile_ID);
            this.HasOptional(t => t.Global_Lookup_Data5)
                .WithMany(t => t.Employee_Profile5)
                .HasForeignKey(d => d.Work_Pass_Type);
            this.HasOptional(t => t.Global_Lookup_Data6)
                .WithMany(t => t.Employee_Profile6)
                .HasForeignKey(d => d.WP_Class);

        }
    }
}
