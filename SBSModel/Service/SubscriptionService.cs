using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Net;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Configuration;

namespace SBSModel.Models
{
    public class SubscriptionService
    {

        public List<Global_Lookup_Def> LstGlobalLookup()
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Global_Lookup_Def orderby a.Name select a).ToList();
            }

        }

        public Global_Lookup_Def GetGlobalLookup(int pDefID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Global_Lookup_Def where a.Def_ID == pDefID select a).SingleOrDefault();
            }

        }
        // VALIDATE FOR NEW USER 
        public Boolean isDuplicatedName(string Name, int pDefID, Nullable<int> pLookupDataID = null, Nullable<int> pCompanyID = null)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.Global_Lookup_Data where a.Name.Equals(Name.ToLower()) & a.Def_ID == pDefID select a);
                if (pLookupDataID != null)
                {
                    q = (from a in q where a.Lookup_Data_ID != pLookupDataID select a);
                }
                if (pCompanyID != null)
                {
                    q = (from a in q where a.Company_ID == pCompanyID select a);
                }
                else
                {
                    q = (from a in q where a.Company_ID == null select a);
                }
                if (q.FirstOrDefault() == null)
                    return false;
                else
                    return true;
            }
        }


        public List<Global_Lookup_Data> LstGlobalLookupInfo(int pDefID, bool pIsCompanyConfig = false, Nullable<int> pCompanyID = null)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.Global_Lookup_Data where a.Def_ID == pDefID select a);
                if (pIsCompanyConfig && pCompanyID.HasValue)
                {
                    q = (from a in q where a.Company_ID == pCompanyID.Value select a);
                }
                else
                {
                    q = (from a in q where a.Company_ID == null select a);
                }

                q = (from a in q orderby a.Name select a);
                return q.ToList();

            }
        }

        public Global_Lookup_Data GetGlobalLookupInfo(int pLookupDataID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Global_Lookup_Data where a.Lookup_Data_ID == pLookupDataID select a).SingleOrDefault();
            }

        }

        public bool InsertGlobalLookupInfo(Global_Lookup_Data pLookupData)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    if (pLookupData != null)
                    {
                        var def = (from a in db.Global_Lookup_Def where a.Def_ID == pLookupData.Def_ID select a).SingleOrDefault();
                        if (def != null)
                        {
                            //Insert
                            pLookupData.Create_On = currentdate;
                            pLookupData.Update_On = currentdate;
                            db.Global_Lookup_Data.Add(pLookupData);
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

        public bool UpdateGlobalLookupInfo(Global_Lookup_Data pLookupData)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    if (pLookupData != null && pLookupData.Lookup_Data_ID > 0)
                    {
                        var current = (from a in db.Global_Lookup_Data
                                       where a.Lookup_Data_ID == pLookupData.Lookup_Data_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            //    var def = (from a in db.Global_Lookup_Def where a.Def_ID == pLookupData.Def_ID select a).SingleOrDefault();
                            //    if (def != null)
                            //    {
                            //        if (def.Name == ComboType.Leave_Type)
                            //        {
                            //            var randomok = false;
                            //            while (randomok == false)
                            //            {
                            //                var randomcolour = ColourUtil.GetRandomColour();
                            //                if (pLookupData.Company_ID.HasValue)
                            //                {
                            //                    var companydefdata = (from a in db.Global_Lookup_Data
                            //                                          where a.Def_ID == def.Def_ID &
                            //                                          a.Company_ID == pLookupData.Company_ID &
                            //                                          a.Colour_Config == randomcolour
                            //                                          select a).FirstOrDefault();
                            //                    if (companydefdata == null)
                            //                    {
                            //                        randomok = true;
                            //                        pLookupData.Colour_Config = randomcolour;
                            //                    }

                            //                }
                            //                else
                            //                {
                            //                    var globaldefdata = (from a in db.Global_Lookup_Data
                            //                                         where a.Def_ID == def.Def_ID &
                            //                                         a.Company_ID == null &
                            //                                         a.Colour_Config == randomcolour
                            //                                         select a).FirstOrDefault();
                            //                    if (globaldefdata == null)
                            //                    {
                            //                        randomok = true;
                            //                        pLookupData.Colour_Config = randomcolour;
                            //                    }

                            //                }
                            //            }
                            //        }
                            pLookupData.Create_By = current.Create_By;
                            pLookupData.Create_On = current.Create_On;
                            pLookupData.Update_On = currentdate;
                            db.Entry(current).CurrentValues.SetValues(pLookupData);
                            db.SaveChanges();
                            //    }
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

        public bool DeleteGlobalLookupInfo(int pLookupDataID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pLookupDataID > 0)
                    {
                        var current = (from a in db.Global_Lookup_Data where a.Lookup_Data_ID == pLookupDataID select a).FirstOrDefault();
                        db.Global_Lookup_Data.Remove(current);
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

        public List<Module_Mapping> GetModuleMappingList(int pPromotionID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Module_Mapping where a.Promotion_ID == pPromotionID select a).ToList();
            }

        }
        public List<Invoice_Header> GetInvoiceList(int pCompanyID, int pYear, Nullable<int> pMonth = null)
        {            
            using (var db = new SBS2DBContext())
            {                
                var q = db.Invoice_Header
                  .Include(d => d.Invoice_Details)
                  .Where(w => w.Company_ID == pCompanyID);

                if (pYear > 0)
                    q = q.Where(w => w.Invoice_Year == pYear);
                if (pMonth.HasValue && pMonth > 0)
                    q = q.Where(w => w.Invoice_Month == pMonth);
                return q.OrderByDescending(o => o.Invoice_Year)
                      .ThenByDescending(o => o.Invoice_Month)
                      .ToList();
            }
        }
        public List<Storage_Upgrade> GetStorageUpgradeList(int pCompanyID, int pYear, int pMonth)
        {
            //DateTime now = DateTime.Now;
            var firstDay = new DateTime(pYear, pMonth, 1);
            var LastDay = firstDay.AddMonths(1).AddDays(-1);
            using (var db = new SBS2DBContext())
            {
                var q = db.Storage_Upgrade                 
                  .Where(w => w.Company_ID == pCompanyID);
                q = q.Where(w => w.Upgrade_On >= firstDay && w.Upgrade_On <= LastDay);

                return q.OrderBy(o => o.Transaction_ID)
                      .ToList();
            }
        }
        public Invoice_Header GetInvoice(int InvoiceID)
        {
            using (var db = new SBS2DBContext())
            {
                var q = db.Invoice_Header
                  .Include(d => d.Invoice_Details)
                  .Where(w => w.Invoice_ID == InvoiceID);
               return q.FirstOrDefault();
            }
        }
        public int GetInvoiceCount(int pCompanyID, int iYear, int iMonth)
        {
            using (var db = new SBS2DBContext())
            {
                var q = db.Invoice_Header
                  .Where(w => w.Company_ID == pCompanyID && w.Invoice_Year == iYear && w.Invoice_Month == iMonth);
                return q.ToList().Count();
            }
        }

        public List<Invoice_Header> Get_Outstanding_Invoice(int pCompanyID, int pYear, int pMonth, string pPaymentStatus)
        {
            using (var db = new SBS2DBContext())
            {
                var q = db.Invoice_Header
                  .Include(d => d.Invoice_Details)
                  .Where(w => w.Company_ID == pCompanyID & w.Invoice_Year == pYear & w.Invoice_Status == pPaymentStatus & w.Invoice_Month < pMonth)
                  .OrderBy(o => o.Invoice_Year)
                  .ThenBy(o => o.Invoice_Month);
                return q.ToList();
            }
        }        
        public string getBillingPromotionName(int PromotionID)
        {
            HttpWebResponse response = null;
            StreamReader readStream = null;
            var PromotionName = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(string.Format(WebConfigurationManager.AppSettings["WSVR_URL"] + "/GetPromotionByPromotionID?pID=" + PromotionID));
            request.Method = "Get";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                readStream = new StreamReader(receiveStream, Encoding.UTF8);
                var rawJson = readStream.ReadToEnd();
                var json = JObject.Parse(rawJson);

                PromotionName = json["Data"]["Promotion_Name"].ToString();
                
            }
            catch (Exception ex)
            {

            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }
            return PromotionName;
        }

        public bool Update_InvoiceStatus(List<Invoice_Header> pHeaders)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    if(pHeaders.Count > 0)
                    {
                        foreach(var pHeader in pHeaders)
                        {
                            var current = (from a in db.Invoice_Header
                                           where a.Invoice_ID == pHeader.Invoice_ID
                                           select a).FirstOrDefault();

                            if (current != null)
                            {
                                if (pHeader.Invoice_Status == PaymentStatus.Paid)
                                    pHeader.Paid_On = currentdate;
                                db.Entry(current).CurrentValues.SetValues(pHeader);
                                db.SaveChanges();
                            }
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

        public bool Insert_Invoice(Invoice_Header pHeader)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    if (pHeader != null)
                    {
                        //Insert
                        db.Invoice_Header.Add(pHeader);
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

        public bool Insert_Storage(Storage_Upgrade pUpgrade)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    if (pUpgrade != null)
                    {
                        //Insert
                        pUpgrade.Upgrade_On = currentdate;
                        pUpgrade.Expired_On = currentdate.AddMonths(12);
                        pUpgrade.Record_Status = RecordStatus.Active;
                        db.Storage_Upgrade.Add(pUpgrade);
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

        public decimal Get_DataSize_ByCompany(int pCompanyID)
        {
            var obj = StoredProcedure.Get_DataSize_ByCompany(pCompanyID);
            return obj;            
        }

        public decimal[] Get_Dtl_DataSize_ByCompany(int pCompanyID)
        {
            decimal[] obj = StoredProcedure.Get_Dtl_DataSize_ByCompany(pCompanyID);
            return obj;
        }

        public decimal Get_Total_Storage_ByCompany(int pCompanyID)
        {
            var obj = StoredProcedure.Get_Total_Storage_ByCompany(pCompanyID);
            return obj;
        }

        public decimal Get_BillingAmount_ByCompany(int pCompanyID)
        {
            var obj = StoredProcedure.Get_BillingAmount_ByCompany(pCompanyID);
            return obj;
        }
        public decimal Get_OutstandingBill_ByCompany(int pCompanyID)
        {
            var obj = StoredProcedure.Get_OutstandingBill_ByCompany(pCompanyID);
            return obj;
        }
    }
}
