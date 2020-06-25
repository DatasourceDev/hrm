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
using SBSResourceAPI;



namespace SBSModel.Models
{
    public class SupportService
    {
        public ServiceResult InsertScreenCaptureLog(Screen_Capture_Log imgLog)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                
                    db.Screen_Capture_Log.Add(imgLog);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Screen_Capture_Log};
                }
            }
            catch
            {
                //Log
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Screen_Capture_Log };
            }
        }
    }
}
