using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SBSModel.Models.Mapping
{
    public class HR_FileExport_IRA8A_DetailMap : EntityTypeConfiguration<HR_FileExport_IRA8A_Detail>
    {
        public HR_FileExport_IRA8A_DetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Detail_ID);

            // Properties
            this.Property(t => t.Employee_No)
                .HasMaxLength(150);

            this.Property(t => t.NRIC)
                .HasMaxLength(30);

            this.Property(t => t.Name)
                .HasMaxLength(150);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.Create_By)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("HR_FileExport_IRA8A_Detail");
            this.Property(t => t.Detail_ID).HasColumnName("Detail_ID");
            this.Property(t => t.Generated_ID).HasColumnName("Generated_ID");
            this.Property(t => t.Process_Year).HasColumnName("Process_Year");
            this.Property(t => t.Employee_Profile_ID).HasColumnName("Employee_Profile_ID");
            this.Property(t => t.Employee_No).HasColumnName("Employee_No");
            this.Property(t => t.NRIC).HasColumnName("NRIC");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Period_From).HasColumnName("Period_From");
            this.Property(t => t.Period_To).HasColumnName("Period_To");
            this.Property(t => t.Days).HasColumnName("Days");
            this.Property(t => t.Shared_Employees).HasColumnName("Shared_Employees");
            this.Property(t => t.Annual_Value).HasColumnName("Annual_Value");
            this.Property(t => t.Furniture_Value).HasColumnName("Furniture_Value");
            this.Property(t => t.Rent_Paid_To_Landlord).HasColumnName("Rent_Paid_To_Landlord");
            this.Property(t => t.Taxable_AV).HasColumnName("Taxable_AV");
            this.Property(t => t.TTL_Rent_PaidBy_Emp).HasColumnName("TTL_Rent_PaidBy_Emp");
            this.Property(t => t.TTL_Taxalbe_AV).HasColumnName("TTL_Taxalbe_AV");
            this.Property(t => t.Utilities).HasColumnName("Utilities");
            this.Property(t => t.Driver).HasColumnName("Driver");
            this.Property(t => t.Servant).HasColumnName("Servant");
            this.Property(t => t.Taxable_Utilities).HasColumnName("Taxable_Utilities");
            this.Property(t => t.Hotel).HasColumnName("Hotel");
            this.Property(t => t.Amt_PaidBy_Emp).HasColumnName("Amt_PaidBy_Emp");
            this.Property(t => t.Taxable_Hotel).HasColumnName("Taxable_Hotel");
            this.Property(t => t.Others).HasColumnName("Others");
            this.Property(t => t.TTL_Benefit_In_Kind).HasColumnName("TTL_Benefit_In_Kind");
            this.Property(t => t.Create_By).HasColumnName("Create_By");
            this.Property(t => t.Create_On).HasColumnName("Create_On");
        }
    }
}
