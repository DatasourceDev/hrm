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

namespace SBSModel.Models
{
    #region Time
    public class TimeArrangementService
    {      
        #region Time Employee Arrangement
        public Time_Arrangement GetTimeArrangement(int? pArgID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Time_Arrangement
                    .Where(w => w.Arrangement_ID == pArgID)
                    .FirstOrDefault();
            }

        }

        public List<Time_Arrangement> LstTimeArrangement(int? pComID, Nullable<DateTime> pEffdate = null)
        {
            using (var db = new SBS2DBContext())
            {
                var args = db.Time_Arrangement
                    .Include(i => i.Employee_Profile)
                    .Where(w => w.Employee_Profile.User_Profile.Company_ID == pComID);

                if (pEffdate.HasValue)
                {
                    args = args.Where(w => (w.Repeat == true ?
                        (EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) <= pEffdate) :
                        EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) == pEffdate));
                    //EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) 
                }
                return args.OrderByDescending(o => o.Effective_Date).ToList();
            }
        }

        public ServiceResult DupTimeArrangement(Time_Arrangement pArg)
        {
            var msg = new StringBuilder();
            using (var db = new SBS2DBContext())
            {
                var args = db.Time_Arrangement.Where(w => (w.Repeat == true ?
                     (EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) <= pArg.Effective_Date) :
                     EntityFunctions.CreateDateTime(w.Effective_Date.Value.Year, w.Effective_Date.Value.Month, w.Effective_Date.Value.Day, 0, 0, 0) == pArg.Effective_Date));

                args = db.Time_Arrangement.Where(w => w.Employee_Profile_ID == pArg.Employee_Profile_ID & w.Branch_ID == pArg.Branch_ID);
                if (pArg.Arrangement_ID > 0)
                    args = args.Where(w => w.Arrangement_ID != pArg.Arrangement_ID);

                var tfrom = pArg.Time_From.Value;
                var tTo = pArg.Time_To.Value;

                var dup = false;
                foreach (var arg in args)
                {
                    var doCheckTime = false;
                    var doCheckDayOfweek = false;
                    var aEff = DateUtil.ToDate(arg.Effective_Date.Value.Day, arg.Effective_Date.Value.Month, arg.Effective_Date.Value.Year);
                    var eff = pArg.Effective_Date.Value;

                    var aRepeat = arg.Repeat.HasValue ? arg.Repeat.Value : false;
                    var repeat = pArg.Repeat.HasValue ? pArg.Repeat.Value : false;

                    if (aRepeat & repeat)
                    {
                        if (aEff == eff)
                            doCheckDayOfweek = true;
                    }
                    else if (aRepeat & !repeat)
                    {
                        if (aEff <= eff)
                            doCheckDayOfweek = true;
                    }
                    else if (!aRepeat & repeat)
                    {
                        if (aEff >= eff)
                            doCheckDayOfweek = true;
                    }
                    else if (!aRepeat & !repeat)
                    {
                        if (aEff == eff)
                            doCheckTime = true;
                    }

                    if (doCheckDayOfweek)
                    {
                        if (aRepeat & repeat)
                        {
                            if (!string.IsNullOrEmpty(arg.Day_Of_Week) && !string.IsNullOrEmpty(pArg.Day_Of_Week))
                            {
                                var aDayOfweeks = arg.Day_Of_Week.Split('|');
                                var dayOfweeks = pArg.Day_Of_Week.Split('|');
                                for (var i = 0; i < aDayOfweeks.Length; i++)
                                {
                                    var dw = aDayOfweeks[i];
                                    if (!string.IsNullOrEmpty(dw))
                                    {
                                        if (dayOfweeks.Contains(dw))
                                        {
                                            doCheckTime = true;
                                            break;
                                        }
                                        else if (dayOfweeks.Contains("-1"))
                                        {
                                            doCheckTime = true;
                                            break;
                                        }
                                    }
                                    else if (dw == "-1")
                                    {
                                        doCheckTime = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (aRepeat & !repeat)
                        {
                            if (!string.IsNullOrEmpty(arg.Day_Of_Week))
                            {
                                var dw = ((int)pArg.Effective_Date.Value.DayOfWeek).ToString();
                                var aDayOfweeks = arg.Day_Of_Week.Split('|');
                                if (aDayOfweeks.Contains(dw))
                                    doCheckTime = true;
                                else if (aDayOfweeks.Contains("-1"))
                                    doCheckTime = true;
                            }
                        }
                        else if (!aRepeat & repeat)
                        {
                            if (!string.IsNullOrEmpty(pArg.Day_Of_Week))
                            {
                                var adw = ((int)arg.Effective_Date.Value.DayOfWeek).ToString();
                                var dayOfweeks = pArg.Day_Of_Week.Split('|');
                                if (dayOfweeks.Contains(adw))
                                    doCheckTime = true;
                                else if (dayOfweeks.Contains("-1"))
                                    doCheckTime = true;
                            }
                        }
                    }

                    if (doCheckTime)
                    {
                        var atfrom = arg.Time_From.Value;
                        var atTo = arg.Time_To.Value;

                        if (tfrom >= atfrom & tfrom <= atTo)
                            dup = true;
                        else if (tTo >= atfrom & tTo <= atTo)
                            dup = true;
                        else if (atfrom >= tfrom & atfrom <= tTo)
                            dup = true;
                        else if (atTo >= tfrom & atTo <= tTo)
                            dup = true;

                        if (dup)
                        {
                            if (string.IsNullOrEmpty(msg.ToString()))
                                msg.AppendLine("The date/time field contains duplicate values.");

                            if (arg.Repeat.HasValue && arg.Repeat.Value)
                            {
                                var display = new StringBuilder();
                                var aDayOfweeks = arg.Day_Of_Week.Split('|');
                                if (aDayOfweeks.Length > 0)
                                {
                                    display.Append(Resource.Repeat);
                                    display.Append(" ");
                                    var i = 0;
                                    foreach (var dw in aDayOfweeks)
                                    {
                                        if (!string.IsNullOrEmpty(dw))
                                        {
                                            display.Append(DateUtil.GetFullDayOfweek(NumUtil.ParseInteger(dw)));
                                            if (i != aDayOfweeks.Length - 2)
                                                display.Append(", ");
                                        }
                                        i++;
                                    }
                                }
                                msg.AppendLine(DateUtil.ToDisplayDate(arg.Effective_Date) + " " + display.ToString() + " (" + DateUtil.ToDisplayTime(atfrom) + " - " + DateUtil.ToDisplayTime(atTo) + ")");
                            }
                            else
                                msg.AppendLine(DateUtil.ToDisplayDate(arg.Effective_Date) + "(" + DateUtil.ToDisplayTime(atfrom) + " - " + DateUtil.ToDisplayTime(atTo) + ")");
                        }

                    }
                }

                if (dup)
                    return new ServiceResult { Code = ERROR_CODE.ERROR_510_DATA_DUPLICATE, Msg = msg.ToString(), Field = Resource.Employee_Arrangement };

                return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg_Code = ERROR_CODE.SUCCESS, Field = Resource.Employee_Arrangement };
            }
        }

        public ServiceResult InsertTimeArrangement(Time_Arrangement pArg)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pArg).State = EntityState.Added;
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_CREATE,
                        Field = Resource.Employee_Arrangement
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Field = Resource.Employee_Arrangement
                };
            }
        }

        public ServiceResult UpdateTimeArrangement(Time_Arrangement pArg)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var curArg = db.Time_Arrangement.Where(w => w.Arrangement_ID == pArg.Arrangement_ID).FirstOrDefault();
                    db.Entry(curArg).CurrentValues.SetValues(pArg);
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_EDIT,
                        Field = Resource.Employee_Arrangement
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Field = Resource.Employee_Arrangement
                };
            }
        }

        public ServiceResult DeleteTimeArrangement(Time_Arrangement pArg)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    db.Entry(pArg).State = EntityState.Deleted;
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg_Code = ERROR_CODE.SUCCESS_DELETE,
                        Field = Resource.Employee_Arrangement
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Field = Resource.Employee_Arrangement
                };
            }
        }


        #endregion
    }
    #endregion

    
}
