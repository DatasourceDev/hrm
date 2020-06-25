using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class ETIRA8
    {
        public int ETIRA8_ID { get; set; }
        public Nullable<int> Company_Running_ID { get; set; }
        public Nullable<int> Employee_Profile_ID { get; set; }
        public string P_YEAR { get; set; }
        public Nullable<System.DateTime> Date_of_Cessation { get; set; }
        public Nullable<decimal> Gross_Salary { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public Nullable<decimal> Director_Fee { get; set; }
        public Nullable<decimal> Allowance_Transport { get; set; }
        public Nullable<decimal> Allowance_Entertain { get; set; }
        public Nullable<decimal> Allowance_Others { get; set; }
        public Nullable<System.DateTime> Commission_Start { get; set; }
        public Nullable<System.DateTime> Commission_End { get; set; }
        public Nullable<decimal> Commission_Amount { get; set; }
        public Nullable<decimal> Commission_Type { get; set; }
        public Nullable<decimal> Pension { get; set; }
        public Nullable<decimal> Gratuity { get; set; }
        public Nullable<decimal> Notice_Pay { get; set; }
        public Nullable<decimal> Ex_Gratia { get; set; }
        public Nullable<decimal> Lump_Sum_Others { get; set; }
        public string Nature { get; set; }
        public Nullable<decimal> Compensation_Loss { get; set; }
        public Nullable<bool> Approval_IRAS { get; set; }
        public Nullable<System.DateTime> Date_Approval { get; set; }
        public string Reason_Payment { get; set; }
        public Nullable<decimal> Length_Service { get; set; }
        public string Basis_Payment { get; set; }
        public Nullable<decimal> Total_Lump_Sum { get; set; }
        public string Retirement_Pension { get; set; }
        public Nullable<decimal> Amount_Accured_1992 { get; set; }
        public Nullable<decimal> Amount_Accured_1993 { get; set; }
        public Nullable<decimal> Contribution_Out_Singapore { get; set; }
        public Nullable<decimal> Excess_Employer { get; set; }
        public Nullable<decimal> Excess_Less { get; set; }
        public Nullable<decimal> Gain_Profit { get; set; }
        public Nullable<decimal> Value_Benefit { get; set; }
        public Nullable<bool> Income_Tax_Employer { get; set; }
        public Nullable<decimal> Tax_Partially { get; set; }
        public Nullable<decimal> Tax_Fixed { get; set; }
        public Nullable<decimal> Employee_CPF { get; set; }
        public string Name_CPF { get; set; }
        public Nullable<decimal> Donation { get; set; }
        public Nullable<decimal> Contribution_Mosque { get; set; }
        public Nullable<decimal> Life_Insurance { get; set; }
        public Nullable<bool> Yayasan_Mendaki_Fund { get; set; }
        public Nullable<bool> Community_chest_of_Singapore { get; set; }
        public Nullable<bool> SINDA { get; set; }
        public Nullable<bool> CDAC { get; set; }
        public Nullable<bool> ECF { get; set; }
        public Nullable<bool> Other_tax { get; set; }
        public Nullable<int> Employer { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public Nullable<decimal> Contribution_With_Singapore { get; set; }
        public string Contribution_With_Name { get; set; }
        public Nullable<bool> Contribution_With_Madatory { get; set; }
        public Nullable<bool> Contribution_With_Establishment { get; set; }
        public Nullable<decimal> Remission_Income { get; set; }
        public Nullable<decimal> Non_Tax_Income { get; set; }
        public string Bank_Name { get; set; }
        public virtual Company Company { get; set; }
        public virtual Employee_Profile Employee_Profile { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}
