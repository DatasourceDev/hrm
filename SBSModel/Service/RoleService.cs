using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    public class RoleService
    {
        public List<Banking_Info> LstBankInfo(Nullable<int> pEmpID, Nullable<int> pProfileID)
        {
            if (pEmpID.HasValue && pProfileID.HasValue)
            {
                var db = new SBS2DBContext();
                return (from a in db.Banking_Info
                        where a.Employee_Profile_ID == pEmpID.Value
                        & a.Profile_ID == pProfileID
                        select a).ToList();
            }
            return null;
        }

        public Banking_Info GetBankInfo(Nullable<int> pBankInfoID)
        {
            if (pBankInfoID.HasValue)
            {
                var db = new SBS2DBContext();

                var q = (from a in db.Banking_Info
                         where a.Banking_Info_ID == pBankInfoID
                         select a).FirstOrDefault();

                return q;
            }
            return null;
        }

        public Banking_Info GetCurrentBankInfo(Nullable<int> pEmployeeID, Nullable<DateTime> rundate = null)
        {
           
            if (pEmployeeID.HasValue)
            {
                using (var db = new SBS2DBContext())
                {
                    if (rundate == null)
                    {
                        var currentdate = StoredProcedure.GetCurrentDate();
                        return db.Banking_Info
                          .Include(i => i.Global_Lookup_Data)
                          .Where(w => w.Employee_Profile_ID == pEmployeeID & w.Effective_Date <= currentdate)
                          .OrderByDescending(o => o.Effective_Date)
                          .FirstOrDefault();
                    }
                    else
                    {
                        return db.Banking_Info
                          .Include(i => i.Global_Lookup_Data)
                          .Where(w => w.Employee_Profile_ID == pEmployeeID & w.Effective_Date <= rundate)
                          .OrderByDescending(o => o.Effective_Date)
                          .FirstOrDefault();
                    }
                  
                }
            }
            return null;
        }

        public bool UpDateBankInfo(Banking_Info pBankInfo)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBankInfo != null && pBankInfo.Banking_Info_ID > 0 && pBankInfo.Banking_Info_ID > 0)
                    {
                        var current = (from a in db.Banking_Info
                                       where a.Banking_Info_ID == pBankInfo.Banking_Info_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            pBankInfo.Create_On = current.Create_On;
                            pBankInfo.Create_By = current.Create_By;
                            db.Entry(current).CurrentValues.SetValues(pBankInfo);
                        }
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch 
            {
                return false;
            }

        }

        public bool InsertBankInfo(Banking_Info pBankInfo)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBankInfo != null)
                    {
                        //Insert                        
                        db.Banking_Info.Add(pBankInfo);
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch 
            {
                return false;
            }

        }
        public bool DeleteBankInfo(int pBankInfoID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBankInfoID > 0)
                    {
                        var current = (from a in db.Banking_Info where a.Banking_Info_ID == pBankInfoID select a).FirstOrDefault();
                        if (current != null)
                        {
                            db.Banking_Info.Remove(current);
                            db.SaveChanges();
                        }

                    }
                    return true;
                }
            }
            catch 
            {
                return false;
            }

        }

    }
}
