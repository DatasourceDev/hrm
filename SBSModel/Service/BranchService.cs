using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using SBSResourceAPI;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SBSModel.Models
{
    public class BranchService
    {
        public List<Branch> LstBranch(Nullable<int> pCompanyId, Nullable<DateTime> pUpdateOn = null)
        {

            using (var db = new SBS2DBContext())
            {
                var branches = db.Branches.Where(w => w.Company_ID == pCompanyId)
                                          .Where(w => w.Company_ID == pCompanyId)
                                          .Where(w => w.Record_Status != RecordStatus.Delete);

                if (pUpdateOn.HasValue)
                {
                    branches = branches.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));
                }
                return branches.ToList();
            }
        }

        public Branch GetBranch(Nullable<int> pBranchID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Branches.Where(w => w.Branch_ID == pBranchID).FirstOrDefault();
            }
        }

        public Branch GetBranch(int pCompanyId,  string pBranchName)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Branches.Where(w => w.Company_ID == pCompanyId && w.Branch_Name == pBranchName).FirstOrDefault();
            }
        }

        public ServiceResult UpdateBranch(Branch pBranch)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBranch != null && pBranch.Branch_ID > 0)
                    {
                        var current = (from a in db.Branches
                                       where a.Branch_ID == pBranch.Branch_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            db.Entry(current).CurrentValues.SetValues(pBranch);
                            //if(current.Inventory_Location.Count() == 0)
                            //{
                            //    var pLocation = new Inventory_Location();
                            //    pLocation.Company_ID = pBranch.Company_ID;
                            //    pLocation.Name = pBranch.Branch_Name;
                            //    pLocation.Description = pBranch.Branch_Desc;
                            //    pLocation.Create_On = pBranch.Create_On;
                            //    pLocation.Create_By = pBranch.Create_By;
                            //    pLocation.Branch_ID = current.Branch_ID;
                            //    pLocation.Record_Status = pBranch.Record_Status;
                            //    db.Inventory_Location.Add(pLocation);
                            //}
                            //else
                            //    current.Inventory_Location.FirstOrDefault().Record_Status = pBranch.Record_Status;

                            db.SaveChanges();
                        }
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Branch };
            }

        }

        public ServiceResult UpdateMultipleBranch(int[] pBranchesID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.Branches.Where(w => pBranchesID.Contains(w.Branch_ID));
                    if (current != null)
                    {
                        foreach (var b in current)
                        {
                            b.Update_On = currentdate;
                            b.Update_By = pUpdateBy;
                            b.Record_Status = pStatus;

                            //if (b.Inventory_Location.Count() > 0)
                            //    b.Inventory_Location.FirstOrDefault().Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Branch };
            }

        }
        /// <summary>
        /// Inserts Branch data into database.
        /// </summary>
        /// <param name="pBranch">Branch model, fields and values.</param>
        /// <param name="pCreateLocation">Indicates if Inventory Location should be created and saved at the same time. Defauts to true.</param>
        /// <returns></returns>
        public ServiceResult InsertBranch(Branch pBranch, bool pCreateLocation=true)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBranch != null)
                    {
                        //Insert    
                 
                        db.Branches.Add(pBranch);
                        db.SaveChanges();
                        db.Entry(pBranch).GetDatabaseValues();

                        if (pCreateLocation) {
                            InsertInvLoc(pBranch);
                        }

                    }                    
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Branch };
            }

        }

        public bool InsertInvLoc(Branch pBranch)
        {
            
            HttpWebResponse response = null;
            StreamReader readStream = null;
            //return Redirect("http://localhost:49489/Approval/ProcessWorkflow?pID=" + HttpUtility.UrlEncode(pID) + "&appID=" + HttpUtility.UrlEncode(appID) + "&empID=" + HttpUtility.UrlEncode(empID) + "&profileID=" + HttpUtility.UrlEncode(profileID) + "&reqID=" + HttpUtility.UrlEncode(reqID) + "&status=" + HttpUtility.UrlEncode(status) + "&Remark=" + HttpUtility.UrlEncode(Remark) + "&code=" + HttpUtility.UrlEncode(code) + "&cancelStatus=" + HttpUtility.UrlEncode(cancelStatus));
            //var request = (HttpWebRequest)WebRequest.Create(string.Format(AppSetting.SERVER_NAME + ModuleDomain.Time + "/WServ/InsertInvLoc?pComID=" + pBranch.Company_ID + "&pBID=" + pBranch.Branch_ID + "&pBName=" + pBranch.Branch_Name + "&pBDesc=" + pBranch.Branch_Desc + "&pRcStatus=" + pBranch.Record_Status + "&pCreBy=" + pBranch.Create_By));

            var request = (HttpWebRequest)WebRequest.Create(string.Format("http://localhost:49489/WServ/InsertInvLoc?pComID=" + pBranch.Company_ID + "&pBID=" + pBranch.Branch_ID + "&pBName=" + pBranch.Branch_Name + "&pBDesc=" + pBranch.Branch_Desc + "&pRcStatus=" + pBranch.Record_Status + "&pCreBy=" + pBranch.Create_By));
            request.Method = "Get";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                readStream = new StreamReader(receiveStream, Encoding.UTF8);
                var rawJson = readStream.ReadToEnd();
                var json = JObject.Parse(rawJson);
                if(json != null && json.Count>0)
                    return json["result"].ToObject<bool>();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (readStream != null)
                    readStream.Close();
                if (response != null)
                    response.Close();
            }
            return true;
        }

        public ServiceResult DeleteBranch(Nullable<int> pBranchID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    var current = (from a in db.Branches where a.Branch_ID == pBranchID select a).FirstOrDefault();
                    if (current != null)
                    {
                        db.Branches.Remove(current);
                        db.SaveChanges();
                    }


                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Branch };
            }

        }

        public bool chkBranchUsed(Nullable<int> pBranchID)
        {
            var chkProblem = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var branch = (from a in db.Employment_History where a.Branch_ID == pBranchID select a).ToList();

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

        public bool chkBranchinEmpePtnUsed(Nullable<int> pBranchID)
        {
            var chkProblem = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var branch = (from a in db.Employee_No_Pattern where a.Branch_ID == pBranchID select a).ToList();

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
    
        public ServiceResult MultipleDeleteBranch(int[] branches)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = db.Branches.Where(w => branches.Contains(w.Branch_ID));
                    if (current != null)
                    {
                        foreach (var b in current)
                        {
                            db.Branches.Remove(b);
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Branch };
            }

        }

        public ServiceResult UpdateDeleteBranchStatus(Nullable<int> pBranchID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {

                    var current = (from a in db.Branches where a.Branch_ID == pBranchID select a).FirstOrDefault();
                    if (current != null)
                    {
                        current.Record_Status = pStatus;
                        current.Update_By = pUpdateBy;
                        current.Update_On = currentdate;

                        //if (current.Inventory_Location.Count() > 0)
                        //    current.Inventory_Location.FirstOrDefault().Record_Status = pStatus;
                        db.SaveChanges();
                    }

                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Branch };
            }

        }

        public ServiceResult UpdateMultipleDeleteBranchStatus(int[] pBranchesID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var current = db.Branches.Where(w => pBranchesID.Contains(w.Branch_ID));
                    if (current != null)
                    {
                        foreach (var b in current)
                        {
                            b.Update_On = currentdate;
                            b.Update_By = pUpdateBy;
                            b.Record_Status = pStatus;

                            //if (b.Inventory_Location.Count() > 0)
                            //    b.Inventory_Location.FirstOrDefault().Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Branch };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Branch };
            }

        }
    }
}
