using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace SBSModel.Models
{
    public class ModuleService
    {

        public int SaveModule(List<Subscription> pSubscriptionList)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pSubscriptionList != null && pSubscriptionList.Count > 0)
                    {
                        foreach (var row in pSubscriptionList)
                        {
                            var current = (from a in db.Subscriptions
                                           where a.Company_ID == row.Company_ID.Value & a.Module_Detail_ID == row.Module_Detail_ID
                                           select a).FirstOrDefault();
                            if (current != null)
                            {
                                //Update
                                current.Start_Date = row.Start_Date;
                                current.Subscription_Period = row.Subscription_Period;
                            }
                            else
                            {
                                //insert
                                db.Subscriptions.Add(row);
                            }
                        }
                        db.SaveChanges();

                    }
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }


        }

        public void LstModuleDetail(int pCompanyID,
            ref int[] pModuleDetailID,
            ref string[] pStartDate,
            ref int[] pSubscriptionPeriod,
            ref string[] pModuleDetailName, ref 
            string[] pModuleName,
            ref int[] pSubscriptionID,
            ref string[] pStatus,
            ref string[] pModuleDetailDesc,
            ref string[] pModuleDesc)
        {
            using (var db = new SBS2DBContext())
            {
                var q = (from a in db.SBS_Module_Detail orderby a.Module_ID, a.Module_Detail_ID select a);

                List<int> Mudule_Detail_ID = new List<int>();
                List<int> Subscription_Period = new List<int>();
                List<String> Start_Date = new List<String>();
                List<String> Module_Detail_Name = new List<String>();
                List<String> Module_Name = new List<String>();
                List<int> Subscription_ID = new List<int>();
                List<String> Status = new List<String>();
                List<String> Module_Detail_Desc = new List<String>();
                List<String> Module_Desc = new List<String>();

                foreach (var drow in (from a in db.SBS_Module_Detail orderby a.Module_ID, a.Module_Detail_ID select a).ToList())
                {

                    DateTime currentdate = StoredProcedure.GetCurrentDate();

                    Module_Name.Add(drow.SBS_Module.Module_Name);
                    Module_Detail_Name.Add(drow.Module_Detail_Name);
                    Mudule_Detail_ID.Add(drow.Module_Detail_ID);

                    Module_Desc.Add(drow.SBS_Module.Module_Description);
                    Module_Detail_Desc.Add(drow.Module_Detail_Description);

                    bool haveSub = false;
                    foreach (var srow in (from a in db.Subscriptions where a.Company_ID == pCompanyID & a.Module_Detail_ID == drow.Module_Detail_ID orderby a.Subscription_ID select a).ToList())
                    {
                        haveSub = true;
                        Subscription_Period.Add(srow.Subscription_Period.Value);
                        if (srow.Start_Date.HasValue)
                        {
                            DateTime endate = new DateTime();
                            if (srow.Subscription_Period == NumUtil.ParseInteger(Duration._15Days))
                            {
                                endate = srow.Start_Date.Value.AddDays(15); ;
                            }
                            else if (srow.Subscription_Period == NumUtil.ParseInteger(Duration._3Months))
                            {
                                endate = srow.Start_Date.Value.AddMonths(3);

                            }
                            else if (srow.Subscription_Period == NumUtil.ParseInteger(Duration._6Months))
                            {
                                endate = srow.Start_Date.Value.AddMonths(6);
                            }
                            else if (srow.Subscription_Period == NumUtil.ParseInteger(Duration._1Year))
                            {
                                endate = srow.Start_Date.Value.AddYears(1);
                            }
                            else if (srow.Subscription_Period == NumUtil.ParseInteger(Duration._2Years))
                            {
                                endate = srow.Start_Date.Value.AddYears(2);
                            }
                            if (currentdate >= srow.Start_Date.Value & currentdate < endate)
                            {
                                Status.Add("A");
                            }
                            else
                            {
                                Status.Add("C");
                            }
                            Start_Date.Add(srow.Start_Date.Value.ToString("dd/MM/yyyy"));

                        }
                        else
                        {
                            Start_Date.Add(null);
                        }
                        Subscription_ID.Add(srow.Subscription_ID);
                        break;
                    }
                    if (!haveSub)
                    {
                        Subscription_Period.Add(0);
                        Start_Date.Add(null);
                        Subscription_ID.Add(0);
                        Status.Add("C");
                    }
                }

                pModuleDetailID = Mudule_Detail_ID.ToArray();
                pSubscriptionPeriod = Subscription_Period.ToArray();
                pStartDate = Start_Date.ToArray();
                pModuleDetailName = Module_Detail_Name.ToArray();
                pModuleName = Module_Name.ToArray();
                pSubscriptionID = Subscription_ID.ToArray();
                pStatus = Status.ToArray();
                pModuleDesc = Module_Desc.ToArray();
                pModuleDetailDesc = Module_Detail_Desc.ToArray();

            }

        }

    }
}
