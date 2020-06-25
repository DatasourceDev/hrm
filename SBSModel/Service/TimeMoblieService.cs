using SBSModel.Common;
using SBSResourceAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBSWorkFlowAPI.Constants;
namespace SBSModel.Models
{
   public class TimeMoblieService
   {
      #region Time Mobile
      public Time_Mobile_Map GetMapping(string pUUID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Time_Mobile_Map.Where(w => w.UUID == pUUID).FirstOrDefault();
         }
      }


      public ServiceResult SaveMapping(Time_Mobile_Map pMap)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var curTD = db.Time_Mobile_Map.Where(w => w.UUID == pMap.UUID).FirstOrDefault();
               if (curTD != null)
               {
                  db.Entry(curTD).CurrentValues.SetValues(pMap);
                  db.SaveChanges();

                  return new ServiceResult
                  {
                     Code = ERROR_CODE.SUCCESS,
                     Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                     Field = Resource.Register
                  };
               }
               else
               {
                  db.Time_Mobile_Map.Add(pMap);
                  db.SaveChanges();
                  return new ServiceResult
                  {
                     Code = ERROR_CODE.SUCCESS,
                     Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                     Field = Resource.Register
                  };
               }
            }
         }
         catch
         {
            return new ServiceResult
            {
               Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
               Field = Resource.Register
            };
         }
      }

      public ServiceResult ClockIn(Time_Mobile_Trans pTran)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Time_Mobile_Trans.Add(pTran);
               db.SaveChanges();
               return new ServiceResult
               {
                  Code = ERROR_CODE.SUCCESS,
                  Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                  Field = Resource.Clock_In
               };
            }
         }
         catch
         {
            return new ServiceResult
            {
               Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
               Field = Resource.Clock_In
            };
         }
      }
      #endregion
   }
}
