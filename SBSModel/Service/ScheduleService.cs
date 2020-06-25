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

namespace SBSModel.Models
{
    public class Payroll_Company_Non_Process
    {
        public int Company_ID { get; set; }
        public string Company_Name { get; set; }

        public List<Employee_Non_Process> Emps_Non_Process { get; set; }
        public List<HR_Group> HRs { get; set; }
    }

    public class Employee_Non_Process
    {
        public string Process_Status { get; set; }
        public int Process_Month { get; set; }
        public int Process_Year { get; set; }

        /* Employee*/
        public string User_Name { get; set; }
        public string Email_Address { get; set; }
        public int Profile_ID { get; set; }
        public int Employee_Profile_ID { get; set; }
    }

    public class HR_Group
    {
        /* HR Payroll Email Address*/
        public int HR_ID { get; set; }
        public string HR_Email { get; set; }
        public string HR_Name { get; set; }
        public List<int> Emps { get; set; }
    }


    public class ScheduleService
    {
        public List<Payroll_Company_Non_Process> LstPayrollNonProcess()
        {
            var noncoms = new List<Payroll_Company_Non_Process>();
            using (var db = new SBS2DBContext())
            {
                var curmonth = 0;
                var curyear = 0;
                var d = db.Database.SqlQuery<DateTime>("select distinct getdate() from Company").ToList();
                if (d.Count > 0)
                {
                    DateTime currentdate = d[0];
                    curmonth = currentdate.Month;
                    curyear = currentdate.Year;
                }
                else
                    return noncoms;


                var coms = db.Companies.Where(w => w.Company_Details.Where(w1 => w1.Company_Status == RecordStatus.Active).FirstOrDefault() != null);
                foreach (var com in coms)
                {
                    var company = com.Company_Details.FirstOrDefault();
                    var users = com.User_Profile;

                    var noncom = new Payroll_Company_Non_Process();
                    noncom.Emps_Non_Process = new List<Employee_Non_Process>();
                    noncom.HRs = new List<HR_Group>();
                    noncom.Company_ID = com.Company_ID;
                    noncom.Company_Name = company.Name;

                    foreach (var user in users)
                    {
                        if (user.User_Status == RecordStatus.Active && user.User_Authentication.Activated == true)
                        {
                            var emp = user.Employee_Profile.FirstOrDefault();
                            if (emp != null)
                            {
                                var firsthist = emp.Employment_History.OrderBy(o => o.Effective_Date).FirstOrDefault();
                                if (firsthist != null && firsthist.Effective_Date.HasValue)
                                {
                                    var hireddate = firsthist.Effective_Date.Value;
                                    //  for(var y = hireddate.Year; y <= curyear; y++)
                                    for (var y = curyear; y <= curyear; y++)
                                    {
                                        for (var m = 1; m <= 12; m++)
                                        {
                                            var doadd = false;
                                            if (y == hireddate.Year)
                                            {
                                                if (m >= hireddate.Month & m < curmonth)
                                                {
                                                    // add
                                                    if (emp.PRMs.Where(w => w.Process_Month == m & w.Process_Year == y).FirstOrDefault() == null)
                                                        doadd = true;

                                                }
                                            }
                                            else if (y == curyear)
                                            {
                                                if (m < curmonth)
                                                {
                                                    // add
                                                    if (emp.PRMs.Where(w => w.Process_Month == m & w.Process_Year == y).FirstOrDefault() == null)
                                                        doadd = true;
                                                }
                                            }
                                            else
                                            {
                                                // add
                                                if (emp.PRMs.Where(w => w.Process_Month == m & w.Process_Year == y).FirstOrDefault() == null)
                                                    doadd = true;
                                            }

                                            if (doadd)
                                            {
                                                var nonprm = new Employee_Non_Process();
                                                nonprm.Process_Month = m;
                                                nonprm.Process_Year = y;
                                                nonprm.User_Name = AppConst.GetUserName(user);
                                                nonprm.Email_Address = user.User_Authentication.Email_Address;
                                                nonprm.Employee_Profile_ID = emp.Employee_Profile_ID;
                                                nonprm.Profile_ID = user.Profile_ID;
                                                noncom.Emps_Non_Process.Add(nonprm);

                                                foreach (var prel in  emp.PRELs)
                                                {
                                                    foreach (var pral in prel.PRG.PRALs)
                                                    {
                                                        var hr = noncom.HRs.Where(w => w.HR_ID == pral.Employee_Profile_ID).FirstOrDefault();
                                                        if (hr == null)
                                                        {
                                                            hr = new HR_Group();
                                                            hr.HR_ID = pral.Employee_Profile_ID;
                                                            hr.HR_Name = AppConst.GetUserName(pral.Employee_Profile.User_Profile);
                                                            hr.HR_Email = pral.Employee_Profile.User_Profile.User_Authentication.Email_Address;
                                                            hr.Emps = new List<int>();
                                                            hr.Emps.Add(emp.Employee_Profile_ID);
                                                            noncom.HRs.Add(hr);
                                                        }
                                                        else
                                                            hr.Emps.Add(emp.Employee_Profile_ID);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }

                    if (noncom.Emps_Non_Process.Count > 0)
                        noncoms.Add(noncom);
                }
                return noncoms;
            }
        }


    }
}
