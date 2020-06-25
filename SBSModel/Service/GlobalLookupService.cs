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
using System.Data;
using System.Data.Entity.Core.Objects;
using SBSResourceAPI;


namespace SBSModel.Models
{

   public class LookUpCriteria : CriteriaBase
   {
      public Nullable<int> Def_ID { get; set; }
      public string Lookup_Name { get; set; }
      public Nullable<int> Lookup_Data_ID { get; set; }
      public bool Mater { get; set; }
   }

   public class GlobalLookupService
   {

      #region Global Lookup Data
      public ServiceObjectResult LstLookUp(LookUpCriteria criteria)
      {
         var result = new ServiceObjectResult();
         result.Object = new List<Global_Lookup_Data>();
         using (var db = new SBS2DBContext())
         {
            var lookUp = db.Global_Lookup_Data
                    .Include(i => i.Global_Lookup_Def)
                    .Where(w => w.Record_Status == RecordStatus.Active);

            if (criteria.Def_ID.HasValue && criteria.Def_ID > 0)
               lookUp = lookUp.Where(w => w.Def_ID == criteria.Def_ID);

            if (criteria.Mater)
               lookUp = lookUp.Where(c => c.Company_ID == null);
            else
            {
               if (criteria.Company_ID.HasValue)
               {
                  lookUp = lookUp.Where(c => c.Company_ID == criteria.Company_ID | c.Company_ID == null);
               }
            }

            if (!string.IsNullOrEmpty(criteria.Lookup_Name))
               lookUp = lookUp.Where(w => w.Name == criteria.Lookup_Name);
            if (criteria.Lookup_Data_ID.HasValue && criteria.Lookup_Data_ID.Value > 0)
               lookUp = lookUp.Where(w => w.Lookup_Data_ID == criteria.Lookup_Data_ID);

            var obj = new List<Global_Lookup_Data>();
            obj = lookUp.OrderBy(o => o.Name).ToList();
            result.Object = obj;
            return result;
         }
      }

      public Global_Lookup_Data GetLookUp(Nullable<int> pDataID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == pDataID && w.Record_Status != RecordStatus.Delete).FirstOrDefault();
         }
      }

      public List<Global_Lookup_Data> LstLookUpData(String pDefName, String pStatus)
      {
          using (var db = new SBS2DBContext())
          {
              var get = db.Global_Lookup_Data
                  .Include(i => i.Global_Lookup_Def)
                  .Where(w => w.Global_Lookup_Def.Name == pDefName && w.Record_Status == pStatus);
                  get = get.OrderBy(o => o.Name);              
                return get.ToList();
          }
      }

      public List<Default_Expense_Type> LstDefaultExpenseType(int pDataID)
      {
          using (var db = new SBS2DBContext())
          {
              var get = db.Default_Expense_Type
                  .Include(i => i.Global_Lookup_Data)
                  .Where(w => w.Expense_Category_ID == pDataID);
              get = get.OrderBy(o => o.Expense_Type_Name);
              return get.ToList();
          }
      }

      public ServiceResult InsertLookUp(Global_Lookup_Data pDataID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Global_Lookup_Data.Add(pDataID);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Lookup_Data };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Lookup_Data };
         }
      }

      public ServiceResult UpdateLookUp(Global_Lookup_Data pDataID)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == pDataID.Lookup_Data_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Lookup_Data + " " + Resource.Not_Found_Msg, Field = Resource.Lookup_Data };

               db.Entry(current).CurrentValues.SetValues(pDataID);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Lookup_Data };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Lookup_Data };
         }
      }
      #endregion

      #region Global Lookup Def
      public Global_Lookup_Def GetLookupDef(Nullable<int> pDefID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Global_Lookup_Def.Where(i => i.Def_ID == pDefID).FirstOrDefault();
         }
      }

      public Global_Lookup_Def GetLookupDef(String pDefName)
      {
          using (var db = new SBS2DBContext())
          {
              return db.Global_Lookup_Def.Where(i => i.Name == pDefName).FirstOrDefault();
          }
      }
      #endregion



      //public ServiceResult UpdateLookupStatus(int[] pDataIDs, string pStatus, string pUpdateBy)
      //{
      //   try
      //   {
      //      var currentdate = StoredProcedure.GetCurrentDate();
      //      using (var db = new SBS2DBContext())
      //      {
      //         var current = db.Global_Lookup_Data.Where(w => pDataIDs.Contains(w.Lookup_Data_ID));
      //         foreach (var lookup in current)
      //         {
      //            if (lookup != null)
      //            {
      //               lookup.Update_On = currentdate;
      //               lookup.Update_By = pUpdateBy;
      //               lookup.Record_Status = pStatus;
      //            }
      //         }

      //         db.SaveChanges();
      //         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Lookup_Data };
      //      }
      //   }
      //   catch
      //   {

      //   }
      //   return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Lookup_Data };
      //}
      //public List<Global_Lookup_Def> LstLookupDef(Nullable<DateTime> pUpdateOn = null)
      //{

      //   using (var db = new SBS2DBContext())
      //   {
      //      var defs = db.Global_Lookup_Def.Select(s => s);
      //      if (pUpdateOn.HasValue)
      //      {
      //         defs = defs.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));
      //      }
      //      return defs.ToList();
      //   }
      //}

      //public List<Global_Lookup_Data> LstLookUpData(Nullable<DateTime> pUpdateOn = null)
      //{
      //   using (var db = new SBS2DBContext())
      //   {
      //      var defs = db.Global_Lookup_Data.Select(s => s);
      //      if (pUpdateOn.HasValue)
      //      {
      //         defs = defs.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));
      //      }
      //      return defs.ToList();
      //   }
      //}

      ////Added by sun 13-10-2015
      ////Update Status function delete
      //public ServiceResult UpdateDeleteAllLookupStatus(int[] pDataIDs, string pStatus, string pUpdateBy)
      //{
      //   try
      //   {
      //      var currentdate = StoredProcedure.GetCurrentDate();
      //      using (var db = new SBS2DBContext())
      //      {
      //         var current = db.Global_Lookup_Data.Where(w => pDataIDs.Contains(w.Lookup_Data_ID));
      //         foreach (var lookup in current)
      //         {
      //            if (lookup != null)
      //            {
      //               lookup.Update_On = currentdate;
      //               lookup.Update_By = pUpdateBy;
      //               lookup.Record_Status = pStatus;
      //            }
      //         }

      //         db.SaveChanges();
      //         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data };
      //      }
      //   }
      //   catch
      //   {

      //   }
      //   return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Lookup_Data };
      //}

      //public ServiceResult UpdateDeleteLookupStatus(Nullable<int> pDataID, string pStatus, string pUpdateBy)
      //{
      //   try
      //   {
      //      var currentdate = StoredProcedure.GetCurrentDate();
      //      using (var db = new SBS2DBContext())
      //      {
      //         var conToDel = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == pDataID).FirstOrDefault();
      //         if (conToDel == null)
      //            return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Lookup_Data + " " + Resource.Not_Found_Msg, Field = Resource.Lookup_Data };

      //         conToDel.Record_Status = pStatus;
      //         conToDel.Update_By = pUpdateBy;
      //         conToDel.Update_On = currentdate;

      //         db.SaveChanges();
      //         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data };
      //      }
      //   }
      //   catch
      //   {

      //   }
      //   return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Lookup_Data };
      //}

      //public ServiceResult DeleteAll(int[] pDataD)
      //{
      //   try
      //   {
      //      using (var db = new SBS2DBContext())
      //      {
      //         var current = db.Global_Lookup_Data.Where(w => pDataD.Contains(w.Lookup_Data_ID));
      //         if (current != null)
      //         {
      //            db.Global_Lookup_Data.RemoveRange(current);
      //            db.SaveChanges();
      //         }
      //         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data };
      //      }
      //   }
      //   catch
      //   {

      //   }
      //   return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Lookup_Data };
      //}

      //public ServiceResult DeleteDataLookup(Nullable<int> pDataID)
      //{
      //   try
      //   {
      //      using (var db = new SBS2DBContext())
      //      {
      //         var conToDel = db.Global_Lookup_Data.Where(w => w.Lookup_Data_ID == pDataID).FirstOrDefault();
      //         db.Global_Lookup_Data.Remove(conToDel);
      //         db.SaveChanges();
      //         return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data };
      //      }
      //   }
      //   catch
      //   {

      //   }
      //   return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Lookup_Data };
      //}

      //public List<Global_Lookup_Data> LstLookUpDetail(Nullable<int> pDefID, Nullable<int> pCompanyID, Nullable<int> pPageID)
      //{
      //    using (var db = new SBS2DBContext())
      //    {
      //        var get = db.Global_Lookup_Data
      //            .Include(i => i.Global_Lookup_Def)
      //            .Where(w => w.Def_ID == pDefID && w.Record_Status != RecordStatus.Delete);

      //        if (pPageID != 1)
      //        {
      //            if (pCompanyID.HasValue)
      //            {
      //                get = get.Where(c => c.Company_ID == pCompanyID | c.Company_ID == null);
      //            }
      //            get = get.OrderBy(o => o.Def_ID);
      //            return get.ToList();
      //        }

      //        get = get.Where(c => c.Company_ID == null);
      //        return get.ToList();
      //    }
      //}

   }

}




