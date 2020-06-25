using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using SBSResourceAPI;

namespace SBSModel.Models
{

    public class ExchangeCurrencyConfigService
    {
        public List<Exchange_Rate> LstExchangeRate(Nullable<int> pCID = null, string type = null)
        {
            List<Exchange_Rate> Exchange = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    Exchange = db.Exchange_Rate
                        .Where(w => w.Exchange_Period == type && w.Exchange_Currency_ID == pCID).ToList();
                    return Exchange;

                }
            }
            catch
            {
            }

            return Exchange;
        }

        public Exchange GetExchangen(int pCompanyID, Nullable<int> pExchangeID)
        {
            Exchange exchang = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    exchang = db.Exchanges
                    .Where(w => w.Company_ID == pCompanyID)
                   .Include(i => i.Exchange_Currency)
                   .Include(i => i.Exchange_Currency.Select(s => s.Exchange_Rate))
                   .Where(w => w.Exchange_ID == pExchangeID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return exchang;
        }

        public ServiceResult InsertExchangeCurrency(Exchange pExchangeCurrency)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Exchanges.Add(pExchangeCurrency);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Exchange };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Exchange };
            }
        }

        public ServiceResult InsertAndUpdateExchangeCurrency(Exchange pExchangeCurrency)
        {
            var currentdate = StoredProcedure.GetCurrentDate();

            try
            {
                using (var db = new SBS2DBContext())
                {
                    var currexchange = new Exchange();
                    var exCurrencyRemove = new List<Exchange_Currency>();
                    List<int> chk_Exchange_Currency = new List<int>();

                    if (pExchangeCurrency.Exchange_ID != 0)
                    {
                        currexchange = GetExchangen(pExchangeCurrency.Company_ID.Value, pExchangeCurrency.Exchange_ID);
                    }

                    if (currexchange == null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Exchange + " "+ Resource.Not_Found_Msg, Field = Resource.Exchange };

                    //Edit Data                
                    foreach (var row in currexchange.Exchange_Currency)
                    {
                        if (pExchangeCurrency.Exchange_Currency == null || !pExchangeCurrency.Exchange_Currency.Select(s => s.Exchange_Currency_ID).Contains(row.Exchange_Currency_ID))
                        {
                            exCurrencyRemove.Add(row);
            
                            var EC = db.Exchange_Currency.Where(w => w.Exchange_Currency_ID == row.Exchange_Currency_ID);
                            db.Exchange_Currency.RemoveRange(EC);

                            var ER = db.Exchange_Rate.Where(w => w.Exchange_Currency_ID == row.Exchange_Currency_ID);
                            db.Exchange_Rate.RemoveRange(ER);
                        }                       
                    }

                    //if (exCurrencyRemove.Count > 0)
                    //{
                    //    db.Exchange_Currency.RemoveRange(exCurrencyRemove);
                    //}

                    if (pExchangeCurrency.Exchange_Currency.Count != 0)
                    {
                        foreach (var row in pExchangeCurrency.Exchange_Currency)
                        {
                            //Temp ch dup
                            chk_Exchange_Currency.Add(row.Currency_ID.Value);                     
                            if (row.Exchange_Currency_ID == 0 || !currexchange.Exchange_Currency.Select(s => s.Exchange_Currency_ID).Contains(row.Exchange_Currency_ID))
                            {
                                if (chk_Exchange_Currency.GroupBy(n => n).Any(c => c.Count() > 1))
                                    return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Currency + " " + Resource.Is_Duplicated_Lower, Field = Resource.Currency };
                                
                                db.Exchange_Currency.Add(row);
                                if (row.Exchange_Rate.Count != 0)
                                {
                                    db.Exchange_Rate.AddRange(row.Exchange_Rate);
                                }
                            }
                            else
                            {
                                if (chk_Exchange_Currency.GroupBy(n => n).Any(c => c.Count() > 1) && !currexchange.Exchange_Currency.Select(s => s.Exchange_Currency_ID).Contains(row.Exchange_Currency_ID))
                                    return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = Resource.Currency + " " + Resource.Is_Duplicated_Lower, Field = Resource.Currency};

                                var currExCurrency = db.Exchange_Currency.Where(w => w.Exchange_Currency_ID == row.Exchange_Currency_ID).FirstOrDefault();
                                if (currExCurrency != null)
                                {
                                    currExCurrency.Exchange_ID = row.Exchange_ID;
                                    currExCurrency.Currency_ID = row.Currency_ID;
                                    currExCurrency.Exchange_Period = row.Exchange_Period;
                                    db.Entry(currExCurrency).State = EntityState.Modified;

                                    //--------------------------Exchange Rate------------------------//

                                    List<Exchange_Rate> currExRate = (from a in db.Exchange_Rate where a.Exchange_Currency_ID == row.Exchange_Currency_ID select a).ToList();
                                    List<int> chk_Using_Deleted = new List<int>();

                                    //foreach (Exchange_Rate ERrow in currExRate)
                                    //{
                                    //    if (row.Exchange_Rate == null || !row.Exchange_Rate.Select(s => s.Exchange_Currency_ID).Contains(ERrow.Exchange_Currency_ID))
                                    //    {
                                    //        db.Entry(row).State = EntityState.Deleted;
                                    //    }
                                    //    chk_Using_Deleted.Add(ERrow.Exchange_Currency_ID.Value);
                                    //}
                                    foreach (Exchange_Rate ERrow in currExRate)
                                    {
                                        if (row.Exchange_Rate == null || chk_Exchange_Currency.Contains(ERrow.Exchange_Currency_ID.Value))
                                        {
                                            db.Entry(row).State = EntityState.Deleted;

                                            chk_Using_Deleted.Add(ERrow.Exchange_Currency_ID.Value);
                                        }
                                    }

                                    foreach (var rowExRate in row.Exchange_Rate)
                                    {
                                        if (!chk_Using_Deleted.Contains(rowExRate.Exchange_Currency_ID.Value))
                                        {
                                            if (rowExRate.Exchange_Period == ExchangePeriod.ByMonth)
                                            {
                                                if (rowExRate.Exchange_Rate_ID == 0)
                                                {
                                                    rowExRate.Exchange_Period = ExchangePeriod.ByMonth;
                                                    rowExRate.Exchange_Date = null;
                                                    db.Exchange_Rate.Add(rowExRate);
                                                }
                                                else
                                                {
                                                    //var currER = db.Exchange_Rate.Where(w => w.Exchange_Month == rowExRate.Exchange_Month && w.Exchange_Currency_ID == row.Exchange_Currency_ID && w.Exchange_Period == ExchangePeriod.ByMonth).FirstOrDefault();
                                                    var currER = db.Exchange_Rate.Where(w => w.Exchange_Rate_ID == rowExRate.Exchange_Rate_ID).FirstOrDefault();
                                                    if (currER != null)
                                                    {
                                                        currER.Exchange_Currency_ID = rowExRate.Exchange_Currency_ID;
                                                        currER.Rate = rowExRate.Rate;
                                                        currER.Exchange_Period = rowExRate.Exchange_Period;
                                                        currER.Exchange_Date = null;
                                                        currER.Exchange_Month = rowExRate.Exchange_Month;
                                                        db.Entry(currER).State = EntityState.Modified;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (rowExRate.Exchange_Rate_ID == 0)
                                                {
                                                    rowExRate.Exchange_Period = ExchangePeriod.ByDate;
                                                    db.Exchange_Rate.Add(rowExRate);
                                                }
                                                else
                                                {
                                                    var currER = db.Exchange_Rate.Where(w => w.Exchange_Rate_ID == rowExRate.Exchange_Rate_ID).FirstOrDefault();
                                                    if (currER != null)
                                                    {
                                                        currER.Exchange_Currency_ID = rowExRate.Exchange_Currency_ID;
                                                        currER.Rate = rowExRate.Rate;
                                                        currER.Exchange_Period = rowExRate.Exchange_Period;
                                                        currER.Exchange_Date = rowExRate.Exchange_Date;
                                                        currER.Exchange_Month = rowExRate.Exchange_Month;
                                                        db.Entry(currER).State = EntityState.Modified;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    //---------------------------------------------------------------//
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Exchange };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Exchange };
            }
        }

        public ServiceResult UpdateMultipleDeleteExchangeStatus(Nullable<int> pExchangeID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var exchanges = db.Exchanges.Where(w => w.Exchange_ID == pExchangeID).FirstOrDefault();

                    if (exchanges != null)
                    {
                        exchanges.Record_Status = pStatus;
                        exchanges.Update_By = pUpdateBy;
                        exchanges.Update_On = currentdate;
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE) };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR) };
            }
        }

        public Exchange GetExchangeRate(int pCompanyID, int pYear)
        {
            Exchange exchang = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    exchang = db.Exchanges 
                    .Where(w => w.Company_ID == pCompanyID && w.Fiscal_Year == pYear && w.Record_Status == RecordStatus.Active)
                   .Include(i => i.Exchange_Currency)
                   .Include(i => i.Exchange_Currency.Select(s => s.Exchange_Rate))
                   .FirstOrDefault();
                }
            }
            catch
            {
            }
            return exchang;
        }
    

    }

    public class ConfigService
    {
        public List<Exchange> LstExchange(Nullable<int> pCompany_ID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Exchanges where a.Company_ID == pCompany_ID && a.Record_Status != RecordStatus.Delete orderby a.Fiscal_Year select a).ToList();
            }
        }

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

    }

    public class DesignationService
    {
        public List<Designation> LstDesignation(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Designations where a.Company_ID == pCompanyID && a.Record_Status != RecordStatus.Delete orderby a.Name select a).ToList();
            }
        }

        public Designation GetDesignation(Nullable<int> pDesignationID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Designations where a.Designation_ID == pDesignationID select a).SingleOrDefault();
            }
        }

        public Designation GetDesignation(int pCompanyID, string pDesignation_Name)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Designations where a.Company_ID == pCompanyID && a.Name == pDesignation_Name && a.Record_Status != RecordStatus.Delete select a).SingleOrDefault();
            }
        }

        public bool IsDesignationRelated(Nullable<int> pDesignationID)
        {
            using (var db = new SBS2DBContext())
            {
                var ds = (from a in db.Designations where a.Designation_ID == pDesignationID select a).FirstOrDefault();
                if (ds != null)
                {
                    if (ds.Employment_History.Count() > 0)
                        return true;

                    if (ds.Expenses_Config_Detail.Count() > 0)
                        return true;

                    if (ds.Leave_Calculation.Count() > 0)
                        return true;

                    if (ds.Leave_Config_Detail.Count() > 0)
                        return true;
                }
            }
            return false;
        }

        public ServiceResult UpdateDesignation(Designation pDesignation)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pDesignation != null && pDesignation.Designation_ID > 0 && pDesignation.Designation_ID > 0)
                    {
                        var current = (from a in db.Designations
                                       where a.Designation_ID == pDesignation.Designation_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            pDesignation.Create_On = current.Create_On;
                            pDesignation.Create_By = current.Create_By;
                            db.Entry(current).CurrentValues.SetValues(pDesignation);
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Designation };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Designation };
            }

        }

        public ServiceResult InsertDesignation(Designation pDesignation)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pDesignation != null)
                    {
                        //Insert    
                        db.Designations.Add(pDesignation);
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Designation };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Designation };
            }

        }

        public ServiceResult DeleteDesignation(Nullable<int> pDesignation)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pDesignation > 0)
                    {
                        var current = (from a in db.Designations where a.Designation_ID == pDesignation select a).FirstOrDefault();
                        if (current != null)
                        {
                            db.Designations.Remove(current);
                            db.SaveChanges();
                        }

                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Designation };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Designation };
            }

        }
        //Added by sun  15-10-2015
        public ServiceResult UpdateMultipleDesignation(int[] pDesignationID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.Designations.Where(w => pDesignationID.Contains(w.Designation_ID));
                    if (current != null)
                    {
                        foreach (var d in current)
                        {
                            d.Update_On = currentdate;
                            d.Update_By = pUpdateBy;
                            d.Record_Status = pStatus;
                        }
                        db.SaveChanges();

                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Designation };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Designation };
            }

        }
        //Purpose : to check current Designation record have any refereces or not.
        public bool chkDesignationUsed(Nullable<int> pDesignationsID)
        {
            var chkProblem = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var branch = (from a in db.Designations where a.Designation_ID == pDesignationsID select a).ToList();

                    if (branch.Count > 0)
                        chkProblem = true;
                }
                return chkProblem;
            }
            catch
            {
                return true;
            }
        }
    }

    public class DepartmentService
    {
        public List<Department> LstDepartment(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Departments where a.Company_ID == pCompanyID && a.Record_Status != RecordStatus.Delete orderby a.Name select a).ToList();
            }
        }

        public Department GetDepartment(Nullable<int> pDepartmentID)
        {
            using (var db = new SBS2DBContext())
            {
               return (from a in db.Departments where a.Department_ID == pDepartmentID && a.Record_Status != RecordStatus.Delete select a).FirstOrDefault();
            }
        }

        public Department GetDepartment(int pCompanyID, string pDept_Name)
        {
            using (var db = new SBS2DBContext())
            {
                return (from a in db.Departments where a.Company_ID == pCompanyID && a.Name == pDept_Name && a.Record_Status != RecordStatus.Delete select a).FirstOrDefault();
            }
        }

        public bool IsDepartmentRelated(Nullable<int> pDepartmentID)
        {
            using (var db = new SBS2DBContext())
            {
                var dp = (from a in db.Departments where a.Department_ID == pDepartmentID select a).FirstOrDefault();
                if (dp != null)
                {
                    if (dp.Employment_History.Count() > 0)
                        return true;

                    if (dp.Leave_Adjustment.Count() > 0)
                        return true;

                    if (dp.Expenses_Application_Document.Count() > 0)
                        return true;

                    if (dp.Expenses_Config.Count() > 0)
                        return true;

                    if (dp.PRC_Department.Count() > 0)
                        return true;

                    if (dp.PREDLs.Count() > 0)
                        return true;
                }
            }
            return false;
        }

        public ServiceResult UpdateDepartment(Department pDepartment)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pDepartment != null && pDepartment.Department_ID > 0 && pDepartment.Department_ID > 0)
                    {
                        var current = (from a in db.Departments
                                       where a.Department_ID == pDepartment.Department_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            pDepartment.Create_On = current.Create_On;
                            pDepartment.Create_By = current.Create_By;
                            db.Entry(current).CurrentValues.SetValues(pDepartment);
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Department };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Department };
            }

        }

        public ServiceResult InsertDepartment(Department pDepartment)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pDepartment != null)
                    {
                        //Insert                        
                        db.Departments.Add(pDepartment);
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Department };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Department };

            }

        }

        public ServiceResult DeleteDepartment(Nullable<int> pDepartment)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pDepartment > 0)
                    {
                        var current = (from a in db.Departments where a.Department_ID == pDepartment select a).FirstOrDefault();
                        if (current != null)
                        {
                            //if (current.Employment_History.Count() > 0)
                            //    return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourceEmployee.EmployeeHistory };

                            //if (current.Expenses_Config.Count() > 0)
                            //    return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourceExpenses.ExpensesConfig};

                            //if (current.PRC_Department.Count() > 0)
                            //    return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourcePayroll.Payroll };

                            //if (current.PREDLs.Count() > 0)
                            //    return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resources.ResourcePayroll.Payroll };

                            db.Departments.Remove(current);
                            db.SaveChanges();
                        }

                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Department };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Department };
            }

        }
        //Added by sun15-10-2015
        public ServiceResult UpdateMultipleDepartment(int[] pDepartmentID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.Departments.Where(w => pDepartmentID.Contains(w.Department_ID));
                    if (current != null)
                    {
                        foreach (var b in current)
                        {
                            b.Update_On = currentdate;
                            b.Update_By = pUpdateBy;
                            b.Record_Status = pStatus;
                        }
                        db.SaveChanges();

                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Department };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Department };
            }

        }
        //Purpose : to check current Department record have any refereces or not.
        public bool chkDepartmentUsed(Nullable<int> pDepartmentsID)
        {
            var chkProblem = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var branch = (from a in db.Departments where a.Department_ID == pDepartmentsID select a).ToList();

                    if (branch.Count > 0)
                        chkProblem = true;
                }
                return chkProblem;
            }
            catch
            {
                return true;
            }
        }

    }

    public class PatternService
    {
        public Employee_No_Pattern GetPattern(Nullable<int> pCompanyId)
        {
            if (pCompanyId.HasValue)
            {
                using (var db = new SBS2DBContext())
                {
                    return (from a in db.Employee_No_Pattern
                            where a.Company_ID == pCompanyId.Value
                            select a).FirstOrDefault();
                }
            }
            return null;
        }

        public ServiceResult SavePattern(Employee_No_Pattern pPattern)
        {
           var curdate = StoredProcedure.GetCurrentDate();
            try
            {
                using (var db = new SBS2DBContext())
                {

                    if (pPattern == null)
                    {
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND) };
                    }
                    Employee_No_Pattern current = (from a in db.Employee_No_Pattern where a.Employee_No_Pattern_ID == pPattern.Employee_No_Pattern_ID select a).FirstOrDefault();
                    if (current == null)
                    {
                        //Insert
                        db.Employee_No_Pattern.Add(pPattern);
                        db.SaveChanges();
                        return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Pattern };
                    }
                    else
                    {
                        // Update
                       var emps = db.Employee_Profile.Where(w => w.User_Profile.Company_ID == pPattern.Company_ID);
                       foreach (var emp in emps)
                       {
                          if (string.IsNullOrEmpty(emp.Employee_No))
                             continue;
                          var empNos = emp.Employee_No.Split(new string[] {"-"} ,StringSplitOptions.RemoveEmptyEntries);
                          if(empNos.Length > 0)
                          {
                             var empNo = "";
                             var year = empNos[0] ;
                             var number = empNos[empNos.Length - 1];

                             empNo += year +"-";
                             var nal = "XX";
                             var branch = "XXXXXXXXXX";
                             if(pPattern.Select_Nationality)
                             {
                                if(emp.Nationality != null)
                                   nal = emp.Nationality.Name;

                                empNo += nal + "-";
                             }
                             if(pPattern.Select_Branch_Code.HasValue && pPattern.Select_Branch_Code.Value)
                             {
                                var hist = emp.Employment_History.Where(w => w.Effective_Date <= curdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                                if (hist != null && hist.Branch != null)
                                   branch = hist.Branch.Branch_Code;
                                else if(pPattern.Branch_ID.HasValue)
                                {
                                   var b = db.Branches.Where(w => w.Branch_ID == pPattern.Branch_ID).FirstOrDefault();
                                   if(b != null)
                                      branch = b.Branch_Code;
                                }
                                   

                                empNo += branch + "-";
                             }
                             empNo += number;
                             emp.Employee_No = empNo;
                          }
                       }
                        pPattern.Initiated = true;
                        db.Entry(current).CurrentValues.SetValues(pPattern);
                        db.SaveChanges();
                        return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Pattern };
                    }

                }
            }
            catch
            {
                return new ServiceResult { Code = ERROR_CODE.ERROR_500_DB, Msg = new Error().getError(ERROR_CODE.ERROR_500_DB), Field = Resource.Pattern };
            }
        }
    }


    public class WorkingDaysService
    {
        public Working_Days GetWorkingDay(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Working_Days.Where(w => w.Company_ID == pCompanyID).FirstOrDefault();
            }
        }

        public ServiceResult UpdateWorkingDays(Working_Days pWorkingDay)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    var current = (from a in db.Working_Days
                                   where a.Working_Days_ID == pWorkingDay.Working_Days_ID
                                   select a).FirstOrDefault();

                    if (current != null)
                    {
                        //Update
                        db.Entry(current).CurrentValues.SetValues(pWorkingDay);
                        db.SaveChanges();
                    }


                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Working_Days };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Working_Days };
            }

        }

        public ServiceResult InsertWorkingDays(Working_Days pWorkingDay)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pWorkingDay != null)
                    {
                        //Insert                        
                        db.Working_Days.Add(pWorkingDay);
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Working_Days };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Working_Days };
            }

        }
    }
}
