using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using SBSModel.Common;
using System.Data.Entity.SqlServer;


namespace SBSModel.Models
{
    public class TaxServie
    {
        //----------------------Tax------------------------------------------------------------------------
        public ServiceObjectResult LstTax(TaxCriteria criteria)
        {
            var result = new ServiceObjectResult();
            result.Object = new List<Tax>();
            using (var db = new SBS2DBContext())
            {
                var tax = db.Taxes
                   .Where(w => w.Company_ID == criteria.Company_ID)
                   .Include(i => i.Tax_Surcharge)
                   .Include(i => i.Tax_GST);

                if (criteria.Record_Status != null)
                {
                    tax = tax.Where(w => w.Record_Status == criteria.Record_Status);
                }

                var obj = new List<Tax>();
                obj = tax.ToList();
                result.Object = obj;
                return result;
            }
        }

        public ServiceResult InsertTax(Tax pTaxPreference)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Taxes.Add(pTaxPreference);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Tax" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Tax" };
            }
        }

        public Tax GetTax(Nullable<int> pCompanyID)
        {
            Tax tax = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    tax = db.Taxes
                   .Include(i => i.Tax_Surcharge)
                   .Include(i => i.Tax_GST)
                   .Where(w => w.Company_ID == pCompanyID)
                   .FirstOrDefault();

                    if (tax != null)
                    {
                        tax.Tax_Surcharge = tax.Tax_Surcharge.Where(w => w.Record_Status != RecordStatus.Delete).ToList();
                        tax.Tax_GST = tax.Tax_GST.Where(w => w.Record_Status != RecordStatus.Delete).ToList();
                    }

                }
            }
            catch
            {
            }
            return tax;
        }

        public ServiceResult UpdateTax(Tax pTaxPreference)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    List<Tax_Surcharge> currTaxSur = new List<Tax_Surcharge>();
                    List<Tax_GST> currTaxGST = new List<Tax_GST>();

                    var taxSurDelete = new List<Tax_Surcharge>();
                    var taxGstDelete = new List<Tax_GST>();

                    if (pTaxPreference.Tax_ID == 0)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = "Tax" + " not found.", Field = "Tax" };

                    var currtaxes = db.Taxes.Where(w => w.Tax_ID == pTaxPreference.Tax_ID).FirstOrDefault();
                    if (currtaxes == null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = "Tax" + " not found.", Field = "Tax" };


                    //if (pTaxPreference.Include_Surcharge.Value == true)
                    //{
                    currTaxSur = (from a in db.Tax_Surcharge where a.Tax_Preference_ID == pTaxPreference.Tax_ID select a).ToList();
                    foreach (var row in currTaxSur)
                    {
                        if (pTaxPreference.Tax_Surcharge == null || !pTaxPreference.Tax_Surcharge.Select(s => s.Tax_Surcharge_ID).Contains(row.Tax_Surcharge_ID))
                        {
                            var tsurD = db.Tax_Surcharge.Where(w => w.Tax_Surcharge_ID == row.Tax_Surcharge_ID).FirstOrDefault();
                            if (tsurD != null)
                            {
                                taxSurDelete.Add(tsurD);
                            }
                        }
                    }

                    //Add delete status 
                    if (taxSurDelete.Count > 0)
                    {
                        foreach (var row in taxSurDelete)
                        {
                            var currSurDe = db.Tax_Surcharge.Where(w => w.Tax_Surcharge_ID == row.Tax_Surcharge_ID).FirstOrDefault();
                            if (currSurDe != null)
                            {
                                currSurDe.Record_Status = RecordStatus.Delete;
                                currSurDe.Update_By = pTaxPreference.Update_By;
                                currSurDe.Update_On = pTaxPreference.Update_On;
                                db.Entry(currSurDe).State = EntityState.Modified;
                            }
                        }
                    }

                    if (pTaxPreference.Tax_Surcharge.Count != 0)
                    {
                        foreach (var row in pTaxPreference.Tax_Surcharge)
                        {
                            if (row.Tax_Surcharge_ID == 0 || !currTaxSur.Select(s => s.Tax_Surcharge_ID).Contains(row.Tax_Surcharge_ID))
                            {
                                db.Tax_Surcharge.Add(row);
                            }
                            else
                            {
                                var currSur = db.Tax_Surcharge.Where(w => w.Tax_Surcharge_ID == row.Tax_Surcharge_ID).FirstOrDefault();
                                if (currSur != null || !taxSurDelete.Select(s => s.Tax_Surcharge_ID).Contains(currSur.Tax_Surcharge_ID))
                                {
                                    currSur.Tax_Title = row.Tax_Title;
                                    currSur.Tax = row.Tax;
                                    currSur.Record_Status = row.Record_Status;
                                    currSur.Update_By = pTaxPreference.Update_By;
                                    currSur.Update_On = pTaxPreference.Update_On;
                                    db.Entry(currSur).State = EntityState.Modified;
                                }
                            }
                        }
                    }
                    //}


                    //if (pTaxPreference.Include_GST.Value == true)
                    //{
                    currTaxGST = (from a in db.Tax_GST where a.Tax_Preference_ID == pTaxPreference.Tax_ID select a).ToList();
                    foreach (var row in currTaxGST)
                    {
                        if (pTaxPreference.Tax_GST == null || !pTaxPreference.Tax_GST.Select(s => s.Tax_GST_ID).Contains(row.Tax_GST_ID))
                        {
                            var tgstD = db.Tax_GST.Where(w => w.Tax_GST_ID == row.Tax_GST_ID).FirstOrDefault();
                            if (tgstD != null)
                            {
                                taxGstDelete.Add(tgstD);
                            }
                        }
                    }

                    //Add delete status 
                    if (taxGstDelete.Count > 0)
                    {
                        foreach (var row in taxGstDelete)
                        {
                            var currGSTDe = db.Tax_GST.Where(w => w.Tax_GST_ID == row.Tax_GST_ID).FirstOrDefault();
                            if (currGSTDe != null)
                            {
                                currGSTDe.Record_Status = RecordStatus.Delete;
                                currGSTDe.Update_By = pTaxPreference.Update_By;
                                currGSTDe.Update_On = pTaxPreference.Update_On;
                                db.Entry(currGSTDe).State = EntityState.Modified;
                            }
                        }
                    }

                    if (pTaxPreference.Tax_GST.Count != 0)
                    {
                        foreach (var row in pTaxPreference.Tax_GST)
                        {
                            if (row.Tax_GST_ID == 0 || !currTaxGST.Select(s => s.Tax_GST_ID).Contains(row.Tax_GST_ID))
                            {
                                db.Tax_GST.Add(row);
                            }
                            else
                            {
                                var currGst = db.Tax_GST.Where(w => w.Tax_GST_ID == row.Tax_GST_ID).FirstOrDefault();
                                if (currGst != null || !taxGstDelete.Select(s => s.Tax_GST_ID).Contains(currGst.Tax_GST_ID))
                                {
                                    currGst.Tax_Title = row.Tax_Title;
                                    currGst.Tax = row.Tax;
                                    currGst.Tax_Type = row.Tax_Type;
                                    currGst.Effective_Date = row.Effective_Date;
                                    currGst.Is_Default = row.Is_Default;
                                    currGst.Record_Status = row.Record_Status;
                                    currGst.Update_By = pTaxPreference.Update_By;
                                    currGst.Update_On = pTaxPreference.Update_On;
                                    db.Entry(currGst).State = EntityState.Modified;
                                }
                            }
                        }
                    }
                    //}

                    db.Entry(currtaxes).CurrentValues.SetValues(pTaxPreference);
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Tax" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Tax" };
            }
        }
    }

    public class PrefixConfigServie
    {

        public Inventory_Prefix_Config GetPrefigConfig(int pCompany_ID, string type)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Inventory_Prefix_Config.Where(w => w.Company_ID == pCompany_ID & w.Inventory_Type == type).FirstOrDefault();
            }
        }

        public ServiceResult InsertPrefigConfig(Inventory_Prefix_Config pQconfig)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Inventory_Prefix_Config.Add(pQconfig);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Prefix" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Prefix" };
            }
        }

        public ServiceResult UpdatePrefigConfig(Inventory_Prefix_Config pQconfig)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.Inventory_Prefix_Config where a.Prefix_Config_ID == pQconfig.Prefix_Config_ID select a).FirstOrDefault();
                    if (current != null)
                    {
                        db.Entry(current).CurrentValues.SetValues(pQconfig);
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Prefix" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Prefix" };
            }
        }

        public bool PrefixConfigExists(int pCompany_ID, string pInventoryType, ref int pConfigID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var prefix = db.Inventory_Prefix_Config.SingleOrDefault(x => x.Inventory_Type.ToLower() == pInventoryType.ToLower()
                        && x.Company_ID == pCompany_ID);

                    if (prefix != null)
                    {
                        pConfigID = prefix.Prefix_Config_ID;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }



    public class ApprovalWorkflowService
    {
        public List<IG> LstIG(int pCompany_ID, string type)
        {
            using (var db = new SBS2DBContext())
            {
                return db.IGs
                    .Include(i => i.IAs)
                    .Include(i => i.IRs)
                    .Where(w => w.Company_ID == pCompany_ID & w.Type == type).ToList();
            }
        }

        public IG GetIG(int pIgID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.IGs
                    .Include(i => i.IRs)
                    .Include(i => i.IAs)
                    .Where(w => w.IG_ID == pIgID).FirstOrDefault();
            }
        }

        public ServiceResult InsertIG(IG pIG, int[] pIAs, int[] pIRs, Nullable<int> approval1, Nullable<int> approval2)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pIRs != null)
                    {
                        foreach (var pid in pIRs)
                        {
                            var ir = new IR();
                            ir.Profile_ID = pid;
                            pIG.IRs.Add(ir);
                        }
                    }
                    if (pIAs != null)
                    {
                        foreach (var pid in pIAs)
                        {
                            var ia = new IA();
                            ia.Profile_ID = pid;
                            ia.Is_Approval1 = false;
                            ia.Is_Approval2 = false;
                            if (pid == approval1)
                            {
                                ia.Is_Approval1 = true;
                            }
                            if (pid == approval2)
                            {
                                ia.Is_Approval2 = true;
                            }
                            pIG.IAs.Add(ia);
                        }
                    }
                    db.IGs.Add(pIG);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Approval Work flow" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Approval Work flow" };
            }
        }

        public ServiceResult UpdateIG(IG pIG, int[] pIAs, int[] pIRs, Nullable<int> approval1, Nullable<int> approval2)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.IGs where a.IG_ID == pIG.IG_ID select a).FirstOrDefault();
                    if (current != null)
                    {

                        if (pIRs != null)
                        {
                            foreach (var pid in pIRs)
                            {
                                var ir = db.IRs.Where(w => w.Profile_ID == pid & w.IG_ID == pIG.IG_ID).FirstOrDefault();
                                if (ir == null)
                                {
                                    //Insert
                                    ir = new IR();
                                    ir.Profile_ID = pid;
                                    ir.IG_ID = pIG.IG_ID;
                                    db.IRs.Add(ir);
                                }

                            }

                            var delIRs = db.IRs.Where(w => w.IG_ID == pIG.IG_ID & !pIRs.Contains(w.Profile_ID.Value));
                            db.IRs.RemoveRange(delIRs);
                        }
                        else
                        {
                            var delIRs = db.IRs.Where(w => w.IG_ID == pIG.IG_ID);
                            db.IRs.RemoveRange(delIRs);
                        }



                        if (pIAs != null)
                        {
                            foreach (var pid in pIAs)
                            {
                                var ia = db.IAs.Where(w => w.Profile_ID == pid & w.IG_ID == pIG.IG_ID).FirstOrDefault();
                                if (ia == null)
                                {
                                    //Insert
                                    ia = new IA();
                                    ia.Profile_ID = pid;
                                    ia.IG_ID = pIG.IG_ID;
                                    ia.Is_Approval1 = false;
                                    ia.Is_Approval2 = false;
                                    if (pid == approval1)
                                    {
                                        ia.Is_Approval1 = true;
                                    }
                                    if (pid == approval2)
                                    {
                                        ia.Is_Approval2 = true;
                                    }
                                    db.IAs.Add(ia);
                                }
                                else
                                {
                                    ia.Is_Approval1 = false;
                                    ia.Is_Approval2 = false;
                                    if (pid == approval1)
                                    {
                                        ia.Is_Approval1 = true;
                                    }
                                    if (pid == approval2)
                                    {
                                        ia.Is_Approval2 = true;
                                    }
                                }

                            }
                            var delIAs = db.IAs.Where(w => w.IG_ID == pIG.IG_ID & !pIAs.Contains(w.Profile_ID.Value));
                            db.IAs.RemoveRange(delIAs);
                        }
                        else
                        {
                            var delIAs = db.IAs.Where(w => w.IG_ID == pIG.IG_ID);
                            db.IAs.RemoveRange(delIAs);
                        }
                        db.Entry(current).CurrentValues.SetValues(pIG);
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Approval Work flow" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Approval Work flow" };
            }
        }


        public ServiceResult DeleteIG(int pIgID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.IGs where a.IG_ID == pIgID select a).FirstOrDefault();
                    if (current != null)
                    {
                        var currentIA = (from a in db.IAs where a.IG_ID == pIgID select a);
                        db.IAs.RemoveRange(currentIA);

                        var currentIR = (from a in db.IRs where a.IG_ID == pIgID select a);
                        db.IRs.RemoveRange(currentIR);

                        db.IGs.Remove(current);
                        db.SaveChanges();
                    }
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Category" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Category" };
            }
        }
    }







}