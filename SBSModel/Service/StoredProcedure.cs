using SBSModel.Offline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SBSModel.Common;
using System.Data;
using System.Data.SqlClient;

namespace SBSModel.Models
{
    public class StoredProcedure
    {
        public static DateTime GetCurrentDate()
        {
            using (var db = new SBS2DBContext())
            {
                var d = db.Database.SqlQuery<DateTime>("select distinct getdate() from Company").ToList();
                if (d.Count > 0)
                    return d[0];
            }
            return new DateTime();
        }

        public static string[] GetUserRolesByUser(int Authen_ID)
        {
            string[] lstRole = null;
            using (var db = new SBS2DBContext())
            {
                var d = db.Database.SqlQuery<string>("select Role_Name from user_assign_role a,User_Role r where a.User_Role_ID = r.User_Role_ID and a.User_Authentication_id = " + Authen_ID).ToList();
                if (d.Count > 0)
                    lstRole = d.ToArray();
            }
            return lstRole;
        }

        public static string[] GetPostPaidCompanies()
        {
            string[] lstCom = null;
            using (var db = new SBS2DBContext())
            {
                var d = db.Database.SqlQuery<string>("select distinct Company_ID from user_transactions").ToList();
                if (d.Count > 0)
                    lstCom = d.ToArray();
            }
            return lstCom;
        }

        public static bool IsExistingEmail(string pEmail)
        {            
            using (var db = new SBS2DBContext())
            {
                var d = db.Database.SqlQuery<Int32>("select Profile_ID from User_Profile where Email = '" + pEmail + "'").ToList();
                if (d.Count > 0)
                    return true;
            }
            return false;
        }

        public static bool IsExistingUserName(string pUserName)
        {
            using (var db = new SBS2DBContext())
            {
                var d = db.Database.SqlQuery<string>("select * from User_Profile where User_Name = '" + pUserName + "'").ToList();
                if (d.Count > 0)
                    return true;
            }
            return false;
        }

        public static decimal Get_Total_Storage_ByCompany(int pCompanyID)
        {            
            using (var db = new SBS2DBContext())
            {
                var inParams = new SqlParameter { ParameterName = "pCompanyID", Value = pCompanyID };
                var outParams = new SqlParameter { ParameterName = "pTotalSize", Value = 0.0, Direction = ParameterDirection.Output };
                var result = db.Database.SqlQuery<decimal>("Exec Get_Total_AvailStorage @pCompanyID, @pTotalSize out", inParams, outParams).Single();
                return Convert.ToDecimal(outParams.Value);
            }
        }

        public static decimal Get_DataSize_ByCompany(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                var inParams = new SqlParameter { ParameterName = "pCompanyID", Value = pCompanyID };
                var outParams = new SqlParameter { ParameterName = "pTotalSize", Value = 0.0, Direction = ParameterDirection.Output };
                var result = db.Database.SqlQuery<decimal>("Exec Get_DataSize_ByCompany @pCompanyID, @pTotalSize out", inParams, outParams).Single();
                return Convert.ToDecimal(outParams.Value);
            }
        }

        public static decimal[] Get_Dtl_DataSize_ByCompany(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                var inParams = new SqlParameter { ParameterName = "pCompanyID", Value = pCompanyID };

                var outCInfo = new SqlParameter { ParameterName = "pCompanyInfoSize", Value = 0.0, Direction = ParameterDirection.Output };
                var outEmpAtt = new SqlParameter { ParameterName = "pEmployeeAttSize", Value = 0.0, Direction = ParameterDirection.Output };
                var outLeaveAtt = new SqlParameter { ParameterName = "pLeaveAttSize", Value = 0.0, Direction = ParameterDirection.Output };
                var outExpenseAtt = new SqlParameter { ParameterName = "pExpenseAttSize", Value = 0.0, Direction = ParameterDirection.Output };
                var result = db.Database.SqlQuery<List<string>>("Exec Get_Dtl_DataSize_ByCompany @pCompanyID, @pCompanyInfoSize out,@pEmployeeAttSize out,@pLeaveAttSize out,@pExpenseAttSize out", 
                   inParams, outCInfo, outEmpAtt, outLeaveAtt, outExpenseAtt).Single();
                decimal[] vals = { Convert.ToDecimal(outCInfo.Value), Convert.ToDecimal(outEmpAtt.Value), Convert.ToDecimal(outLeaveAtt.Value), Convert.ToDecimal(outExpenseAtt.Value) };
                return vals;
            }
        }

        public static decimal Get_BillingAmount_ByCompany(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                var inParams = new SqlParameter { ParameterName = "pCompanyID", Value = pCompanyID };
                var outParams = new SqlParameter { ParameterName = "pBillAmt", Value = 0.0, Direction = ParameterDirection.Output };
                var result = db.Database.SqlQuery<decimal>("Exec Get_BillAmount_ByCompany @pCompanyID, @pBillAmt out", inParams, outParams).Single();
                if(outParams.Value != DBNull.Value)
                    return Convert.ToDecimal(outParams.Value);
                return 0;
            }
        }
        
        public static decimal Get_OutstandingBill_ByCompany(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                var inParams = new SqlParameter { ParameterName = "pCompanyID", Value = pCompanyID };
                var outParams = new SqlParameter { ParameterName = "pBillAmt", Value = 0.0, Direction = ParameterDirection.Output };
                var result = db.Database.SqlQuery<decimal>("Exec Get_OutstandingBill_ByCompany @pCompanyID, @pBillAmt out", inParams, outParams).Single();
                if (outParams.Value != DBNull.Value)
                    return Convert.ToDecimal(outParams.Value);
                return 0;
            }
        }
        //public static bool isDifferenceServer()
        //{
        //    // this function for checking, this pos for server or local
        //    using (var db = new SBS2DBContext())
        //    {
        //        using (var db2 = new ServerContext())
        //        {
        //            if (db.Database.Connection.ConnectionString.Replace(" ", "").ToUpper() != db2.Database.Connection.ConnectionString.Replace(" ", "").ToUpper())
        //                return true;
        //        }
        //    }
        //    return false;
        //}
    }
}