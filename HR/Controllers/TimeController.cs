//using CivinTecAccessManager;
//using SBSModel.Common;
//using SBSModel.Models;
//using SBSResourceAPI;
//using SBSTimeModel.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;
//using HR.Models;
////using ZKAccessManager;
//namespace HR.Controllers
//{
//   [Authorize]
//   public class TimeController : ControllerBase
//   {
//      //
//      // GET: /Time/
//      [HttpGet]
//      [AllowAuthorized]
//      public ActionResult Configuration(ServiceResult result, TimeConfViewModel model)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         //Validate Page Right
//         var rightResult = base.validatePageRight(Operation.A);
//         if (rightResult.action != null) return rightResult.action;
//         model.rights = rightResult.rights;
//         model.result = result;

//         var tmServ = new TimeService();
//         var cbServ = new ComboService();
//         var tmcbServ = new TimeComboService();

//         if (model.operation == Operation.D | model.operation == Operation.R)
//            return Configuration(model);

//         model.cBranchlist = cbServ.LstBranch(userlogin.Company_ID);
//         model.deviceList = tmServ.LstTimeDevice(userlogin.Company_ID);
//         model.cBrandlist = tmcbServ.LstBrandName();
//         model.Brand_Name = model.Brand_Name;

//         if (!model.Search_Device_ID.HasValue & model.deviceList.Count() > 0)
//            model.Search_Device_ID = model.deviceList[0].Device_ID;

//         model.cDevicelist = tmcbServ.LstTimeDevice(userlogin.Company_ID, false);
//         model.cEmplist = cbServ.LstEmployee(userlogin.Company_ID, true);

//         var dvMng = new CivinTecAccessManager.clsAccessManager();
//         if (!model.Search_Device_ID.HasValue)
//         {
//            if (model.cDevicelist.Count > 0)
//               model.Search_Device_ID = NumUtil.ParseInteger(model.cDevicelist[0].Value);

//         }
//         var dv = tmServ.GetTimeDevice(model.Search_Device_ID);
//         if (dv != null)
//         {
//            if (dv.Brand_Name == "Utouch")
//            {
//               if (!string.IsNullOrEmpty(dv.IP_Address))
//               {
//                  var dvresult = dvMng.getConnection(dv.IP_Address);
//                  if (!dvresult)
//                     model.result = new ServiceResult() { Code = ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT, Msg = new Error().getError(ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT) };
//               }
//               model.cEmpDevicelist = dvMng.getUsers();
//            }
//            else
//            {
//               var ulist = tmServ.GetZKUsers(dv.Device_ID);
//               List<User> lst = new List<User>();
//               for (int j = 0; j < ulist.Count(); j++)
//               {
//                  User u = new User();
//                  u.Pin = ulist[j].User_Pin;
//                  u.FirstName = ulist[j].User_Name;
//                  u.Level = Convert.ToByte(ulist[j].User_Level);
//                  u.MiddleName = "";
//                  u.LastName = "";
//                  lst.Add(u);
//               }
//               model.cEmpDevicelist = lst;
//            }

//         }

//         //model.cEmpDevicelist = dvMng.getUsers();
//         //if (model.cEmpDevicelist == null || model.cEmpDevicelist.Count == 0)
//         //{
//         //    model.cEmpDevicelist = new List<User>();
//         //    for (var j = 0; j < 10; j++)
//         //    {
//         //        model.cEmpDevicelist.Add(new User() { Pin = j, FirstName = "Name-" + j.ToString() });
//         //    }
//         //}

//         var mlist = new List<TimeConfMapViewModel>();
//         var i = 0;
//         foreach (var row in tmServ.LstTimeDeviceMap(model.Search_Device_ID))
//         {
//            mlist.Add(new TimeConfMapViewModel()
//            {
//               Index = i,
//               Row_Type = RowType.EDIT,
//               Device_Employee_Pin = row.Device_Employee_Pin,
//               Employee_Map_ID = row.Map_ID,
//               Device_ID = row.Device_ID,
//               Employee_Profile_ID = row.Employee_Profile_ID,
//               Employee_Email = row.Employee_Email
//            });
//            i++;
//         }
//         model.Map_Rows = mlist.ToArray();
//         model.Map_Device_ID = model.Search_Device_ID;
//         return View(model);
//      }

//      [HttpPost]
//      [ValidateAntiForgeryToken]
//      public ActionResult Configuration(TimeConfViewModel model)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         var tmcbServ = new TimeComboService();
//         var tmServ = new TimeService();
//         var cbServ = new ComboService();
//         var dvMng = new CivinTecAccessManager.clsAccessManager();
//         //var ZKdvMng = new ZKAccessManager.clsZKAccessManager();

//         var currentdate = StoredProcedure.GetCurrentDate();
//         if (model.tabAction == "device")
//         {
//            if (model.operation == Operation.C | model.operation == Operation.U)
//            {
//               if (!model.Device_Branch_ID.HasValue)
//                  ModelState.AddModelError("Device_Branch_ID", Resource.Message_Is_Required);
//               if (string.IsNullOrEmpty(model.Device_No))
//                  ModelState.AddModelError("Device_No", Resource.Message_Is_Required);

//               if (ModelState.IsValid)
//               {
//                  var device = new Time_Device();
//                  if (model.operation == Operation.U)
//                     device = tmServ.GetTimeDevice(model.Device_ID);

//                  device.Branch_ID = model.Device_Branch_ID;

//                  if (model.Device_Branch_ID.HasValue)
//                  {
//                     var branch = new BranchService().GetBranch(model.Device_Branch_ID);
//                     if (branch != null)
//                        device.Branch_Name = branch.Branch_Name;
//                  }
//                  device.Brand_Name = model.Brand_Name;
//                  device.Company_ID = userlogin.Company_ID;
//                  device.Device_No = model.Device_No;
//                  device.IP_Address = model.Device_IP;
//                  device.Password = model.Device_Password;
//                  device.Port = model.Device_Port;
//                  device.User_Name = model.Device_User_Name;
//                  device.Update_By = userlogin.User_Authentication.Email_Address;
//                  device.Update_On = currentdate;
//                  device.Record_Status = RecordStatus.Active;
//                  if (model.operation == Operation.C)
//                  {
//                     device.Min_Transaction_Id = 0;
//                     device.Max_Transaction_Id = 0;
//                     device.Create_By = userlogin.User_Authentication.Email_Address;
//                     device.Create_On = currentdate;
//                     model.result = tmServ.InsertTimeDevice(device);
//                     if (model.result.Code == ERROR_CODE.SUCCESS)
//                        return RedirectToAction("Configuration", new RouteValueDictionary(model.result));
//                  }
//                  else if (model.operation == Operation.U)
//                  {
//                     model.result = tmServ.UpdateTimeDevice(device);
//                     if (model.result.Code == ERROR_CODE.SUCCESS)
//                        return RedirectToAction("Configuration", new RouteValueDictionary(model.result));
//                  }
//               }
//            }
//            else if (model.operation == Operation.D | model.operation == Operation.R)
//            {
//               if (!model.Device_ID.HasValue)
//                  ModelState.AddModelError("Device", Resource.Message_Is_Required);

//               if (ModelState.IsValid)
//               {
//                  if (model.operation == Operation.D)
//                     model.result = tmServ.DeleteTimeDevice(model.Device_ID);
//                  else if (model.operation == Operation.R)
//                     model.result = tmServ.ResetTimeDevice(model.Device_ID);

//                  if (model.result.Code == ERROR_CODE.SUCCESS)
//                     return RedirectToAction("Configuration", new RouteValueDictionary(model.result));
//               }
//            }

//         }
//         else if (model.tabAction == "map")
//         {
//            if (model.Map_Rows != null)
//            {
//               for (int i = 0; i < model.Map_Rows.Count(); i++)
//               {
//                  var row = model.Map_Rows[i];
//                  if (row.Row_Type == RowType.DELETE | !row.Employee_Profile_ID.HasValue)
//                  {
//                     DeleteModelStateError("Map_Rows[" + i + "]");
//                     continue;
//                  }

//                  if (!row.Device_ID.HasValue)
//                     ModelState.AddModelError("Map_Rows[" + i + "].Device_ID", Resource.Message_Is_Required);

//                  /*check dup emp code*/
//                  var dup1 = model.Map_Rows.Where(w => w.Device_Employee_Pin == row.Device_Employee_Pin & w.Index != i & w.Row_Type != RowType.DELETE).FirstOrDefault();
//                  if (dup1 != null)
//                     ModelState.AddModelError("Map_Rows[" + i + "].Device_Employee_Pin", Resource.Message_Is_Duplicated);

//                  /*check dup emp*/
//                  var dup2 = model.Map_Rows.Where(w => w.Employee_Profile_ID == row.Employee_Profile_ID & w.Index != i & w.Row_Type != RowType.DELETE).FirstOrDefault();
//                  if (dup2 != null)
//                     ModelState.AddModelError("Map_Rows[" + i + "].Employee_Profile_ID", Resource.Message_Is_Duplicated);
//               }
//               var err = GetErrorModelState();
//               if (ModelState.IsValid)
//               {
//                  model.Map_Rows = model.Map_Rows.Where(w => w.Employee_Profile_ID.HasValue).ToArray();
//                  var maps = new List<Time_Device_Map>();
//                  foreach (var row in model.Map_Rows)
//                  {
//                     if (row.Row_Type == RowType.DELETE)
//                        continue;

//                     var map = new Time_Device_Map();
//                     map.Device_Employee_Name = row.Device_Employee_Name;
//                     map.Device_Employee_Pin = row.Device_Employee_Pin;
//                     map.Employee_Profile_ID = row.Employee_Profile_ID;
//                     map.Map_ID = row.Employee_Map_ID.HasValue ? row.Employee_Map_ID.Value : 0;
//                     map.Device_ID = row.Device_ID;
//                     map.Update_By = userlogin.User_Authentication.Email_Address;
//                     map.Update_On = currentdate;
//                     if (row.Row_Type == RowType.ADD)
//                     {
//                        map.Record_Status = RecordStatus.Active;
//                        map.Create_By = userlogin.User_Authentication.Email_Address;
//                        map.Create_On = currentdate;
//                     }

//                     maps.Add(map);
//                  }
//                  if (model.Map_Device_ID.HasValue)
//                  {
//                     model.result = tmServ.SaveTimeDeviceMap(userlogin.Company_ID, model.Map_Device_ID, maps);
//                     if (model.result.Code == ERROR_CODE.SUCCESS)
//                     {
//                        var result = new AppRouteValueDictionary(model);
//                        result.Add("Search_Device_ID", model.Map_Device_ID);
//                        return RedirectToAction("Configuration", result);
//                     }
//                  }
//               }
//            }
//         }


//         var rightResult = base.validatePageRight(Operation.A);
//         if (rightResult.action != null)
//            return rightResult.action;
//         model.rights = rightResult.rights;

//         model.cBranchlist = cbServ.LstBranch(userlogin.Company_ID);
//         model.deviceList = tmServ.LstTimeDevice(userlogin.Company_ID);

//         var dv = tmServ.GetTimeDevice(model.Map_Device_ID);
//         if (dv != null)
//         {
//            if (!string.IsNullOrEmpty(dv.IP_Address))
//            {
//               var dvresult = dvMng.getConnection(dv.IP_Address);
//               if (!dvresult)
//                  model.result = new ServiceResult() { Code = ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT, Msg = new Error().getError(ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT) };
//            }
//         }

//         model.cDevicelist = tmcbServ.LstTimeDevice(userlogin.Company_ID, false);
//         model.cEmplist = cbServ.LstEmployee(userlogin.Company_ID, true);
//         model.cEmpDevicelist = dvMng.getUsers();
//         model.cBrandlist = tmcbServ.LstBrandName();
//         // model.cZKEmpDevicelist = ZKdvMng.getAllUsers();
//         return View(model);
//      }

//      public ActionResult ConfAddMap(int pIndex, int pDID)
//      {
//         var model = new TimeConfMapViewModel();
//         var cbServ = new ComboService();
//         var tmServ = new TimeService();
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         model.Index = pIndex;
//         model.Row_Type = RowType.ADD;
//         model.Device_ID = pDID;
//         model.cEmplist = cbServ.LstEmployee(userlogin.Company_ID);
//         if (model.cEmplist.Count() > 0)
//         {
//            model.Employee_Profile_ID = NumUtil.ParseInteger(model.cEmplist[0].Value);
//            model.Employee_Email = model.cEmplist[0].Desc;
//         }
//         var dvMng = new CivinTecAccessManager.clsAccessManager();
//         //var ZKdvMng = new ZKAccessManager.clsZKAccessManager();
//         var dv = tmServ.GetTimeDevice(pDID);
//         if (dv != null)
//         {
//            if (dv.Brand_Name == "Utouch")
//            {
//               dvMng.getConnection(dv.IP_Address);
//               model.cEmpDevicelist = dvMng.getUsers();
//            }
//            else
//            {
//               var ulist = tmServ.GetZKUsers(pDID);
//               List<User> lst = new List<User>();
//               for (int i = 0; i < ulist.Count(); i++)
//               {
//                  User u = new User();
//                  u.Pin = ulist[i].User_Pin;
//                  u.FirstName = ulist[i].User_Name;
//                  u.Level = Convert.ToByte(ulist[i].User_Level);
//                  u.MiddleName = "";
//                  u.LastName = "";
//                  lst.Add(u);
//               }
//               model.cEmpDevicelist = lst;
//            }
//         }
//         return PartialView("_ConfMapRow", model);
//      }

//      [HttpGet]
//      [AllowAuthorized]
//      public ActionResult Arrangement(ServiceResult result, TimeArrangementViewModel model)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         //Validate Page Right
//         var rightResult = base.validatePageRight(Operation.A);
//         if (rightResult.action != null) return rightResult.action;
//         model.rights = rightResult.rights;
//         model.result = result;

//         var tmServ = new TimeService();
//         var cbServ = new ComboService();
//         var currentdate = StoredProcedure.GetCurrentDate();

//         model.cBranchlist = cbServ.LstBranch(userlogin.Company_ID, true);
//         model.cDepartmentlist = cbServ.LstDepartment(userlogin.Company_ID, true);
//         model.cEmplist = cbServ.LstEmployeeList(userlogin.Company_ID, model.search_Department_ID);

//         if (string.IsNullOrEmpty(model.search_Effective_Date))
//            model.search_Effective_Date = DateUtil.ToDisplayDate(currentdate);

//         model.argList = tmServ.LstTimeArrangement(userlogin.Company_ID, DateUtil.ToDate(model.search_Effective_Date));


//         ModelState.Clear();

//         return View(model);
//      }

//      [HttpPost]
//      [AllowAuthorized]
//      public ActionResult Arrangement(TimeArrangementViewModel model)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         var tmServ = new TimeService();
//         var cbServ = new ComboService();
//         var currentdate = StoredProcedure.GetCurrentDate();

//         model.search_Effective_Date = model.display_Effective_Date;
//         if (string.IsNullOrEmpty(model.display_Effective_Date))
//            model.search_Effective_Date = DateUtil.ToDisplayDate(currentdate);

//         if (model.operation == Operation.C | model.operation == Operation.U)
//         {
//            if (model.Repeat.HasValue && model.Repeat.Value)
//               if (model.Day_Of_Weeks == null || model.Day_Of_Weeks.Length == 0)
//                  ModelState.AddModelError("Day_Of_Week", Resource.Message_Is_Required);

//            if (!string.IsNullOrEmpty(model.Time_From) && !string.IsNullOrEmpty(model.Time_To))
//            {
//               var tfrom = DateUtil.ToTime(model.Time_From);
//               var tTo = DateUtil.ToTime(model.Time_To);
//               if (tfrom == null)
//                  ModelState.AddModelError("Time_From", Resource.Message_Is_Invalid);

//               if (tTo == null)
//                  ModelState.AddModelError("Time_To", Resource.Message_Is_Invalid);

//               if (tfrom != null && tTo != null && tfrom > tTo)
//               {
//                  ModelState.AddModelError("Time_From", Resource.Message_Is_Invalid);
//                  ModelState.AddModelError("Time_To", Resource.Message_Is_Invalid);
//               }

//            }
//            if (!string.IsNullOrEmpty(model.Effective_Date))
//            {
//               var eff = DateUtil.ToDate(model.Effective_Date);
//               if (eff == null)
//                  ModelState.AddModelError("Effective_Date", Resource.Message_Is_Invalid);
//            }

//            if (ModelState.IsValid)
//            {

//               var arg = new Time_Arrangement();
//               if (model.operation == Operation.U)
//                  arg = tmServ.GetTimeArrangement(model.Arrangement_ID);

//               if (arg == null)
//                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employee_Arrangement);

//               arg.Branch_ID = model.Branch_ID;
//               if (model.Repeat.HasValue && model.Repeat.Value && model.Day_Of_Weeks != null)
//               {
//                  var dayOfweeks = "";
//                  foreach (var d in model.Day_Of_Weeks)
//                  {
//                     dayOfweeks += d;
//                     dayOfweeks += "|";
//                  }
//                  arg.Day_Of_Week = dayOfweeks;
//               }
//               arg.Effective_Date = DateUtil.ToDate(model.Effective_Date);
//               arg.Employee_Profile_ID = model.Employee_Profile_ID;
//               arg.Remark = model.Remark;
//               arg.Repeat = model.Repeat;
//               arg.Time_From = DateUtil.ToTime(model.Time_From);
//               arg.Time_To = DateUtil.ToTime(model.Time_To);
//               arg.Update_By = userlogin.User_Authentication.Email_Address;
//               arg.Update_On = currentdate;
//               arg.Company_ID = userlogin.Company_ID;
//               model.result = tmServ.DupTimeArrangement(arg);
//               if (model.result.Code == ERROR_CODE.SUCCESS)
//               {
//                  if (model.operation == Operation.C)
//                  {
//                     arg.Create_By = userlogin.User_Authentication.Email_Address;
//                     arg.Create_On = currentdate;
//                     model.result = tmServ.InsertTimeArrangement(arg);
//                     if (model.result.Code == ERROR_CODE.SUCCESS)
//                     {
//                        var route = new RouteValueDictionary(model.result);
//                        route.Add("search_Effective_Date", model.search_Effective_Date);
//                        return RedirectToAction("Arrangement", route);
//                     }

//                  }
//                  else if (model.operation == Operation.U)
//                  {
//                     model.result = tmServ.UpdateTimeArrangement(arg);
//                     if (model.result.Code == ERROR_CODE.SUCCESS)
//                     {
//                        var route = new RouteValueDictionary(model.result);
//                        route.Add("search_Effective_Date", model.search_Effective_Date);
//                        return RedirectToAction("Arrangement", route);
//                     }
//                  }
//               }
//               else
//               {
//                  ModelState.AddModelError("Effective_Date", Resource.Message_Is_Duplicated);
//                  ModelState.AddModelError("Time_From", Resource.Message_Is_Duplicated);
//                  ModelState.AddModelError("Time_To", Resource.Message_Is_Duplicated);
//               }
//            }

//         }
//         else if (model.operation == Operation.D)
//         {
//            var arg = tmServ.GetTimeArrangement(model.Arrangement_ID);
//            if (arg == null)
//               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employee_Arrangement);

//            model.result = tmServ.DeleteTimeArrangement(arg);
//            if (model.result.Code == ERROR_CODE.SUCCESS)
//            {
//               var route = new RouteValueDictionary(model.result);
//               route.Add("search_Effective_Date", model.Effective_Date);
//               return RedirectToAction("Arrangement", route);
//            }
//         }


//         var rightResult = base.validatePageRight(Operation.A);
//         if (rightResult.action != null) return rightResult.action;
//         model.rights = rightResult.rights;

//         model.cBranchlist = cbServ.LstBranch(userlogin.Company_ID, true);
//         model.cDepartmentlist = cbServ.LstDepartment(userlogin.Company_ID, true);
//         model.cEmplist = cbServ.LstEmployeeList(userlogin.Company_ID, model.search_Department_ID);

//         model.argList = tmServ.LstTimeArrangement(userlogin.Company_ID, DateUtil.ToDate(model.search_Effective_Date));

//         if (string.IsNullOrEmpty(model.Effective_Date))
//            model.Effective_Date = DateUtil.ToDisplayDate(currentdate);

//         return View(model);
//      }

//      [HttpGet]
//      [AllowAuthorized]
//      public ActionResult TransactionReport(ServiceResult result, TimeTransactionViewModel model)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         var rightResult = base.validatePageRight(Operation.A);
//         if (rightResult.action != null) return rightResult.action;
//         model.rights = rightResult.rights;
//         model.result = result;

//         var tmServ = new TimeService(userlogin);
//         var cbServ = new ComboService();
//         var currentdate = StoredProcedure.GetCurrentDate();
//         var tmcbServ = new TimeComboService();
//         var wkServ = new WorkingDaysService();

//         model.workdays = wkServ.GetWorkingDay(userlogin.Company_ID);
//         model.cBranchlist = cbServ.LstBranch(userlogin.Company_ID, true);
//         model.cDepartmentlist = cbServ.LstDepartment(userlogin.Company_ID, true);
//         model.cEmplist = cbServ.LstEmployeeList(userlogin.Company_ID, model.search_Department_ID, true);
//         model.cDevicelist = tmcbServ.LstTimeDevice(userlogin.Company_ID, false);

//         if (string.IsNullOrEmpty(model.search_From))
//            model.search_From = DateUtil.ToDisplayDate(currentdate);
//         if (string.IsNullOrEmpty(model.search_To))
//            model.search_To = DateUtil.ToDisplayDate(currentdate);

//         var msg = new StringBuilder();
//         var code = 1;
//         var dvMng = new clsAccessManager();
//         if (!model.search_Device_ID.HasValue)
//         {
//            if (model.cDevicelist.Count > 0)
//               model.search_Device_ID = NumUtil.ParseInteger(model.cDevicelist[0].Value);
//         }
//         var dvresult = false;
//         var dv = tmServ.GetTimeDevice(model.search_Device_ID);
//         if (dv != null)
//         {
//            model.Brand_Name = dv.Brand_Name;
//            if (dv.Brand_Name == "Utouch")
//            {
//               if (!string.IsNullOrEmpty(dv.IP_Address))
//               {
//                  dvresult = dvMng.getConnection(dv.IP_Address);
//                  if (dvresult)
//                  {
//                     code = ERROR_CODE.SUCCESS_CONNECT;
//                     msg.AppendLine("(" + dv.IP_Address + ") " + new Success().getSuccess(code));
//                  }
//                  else
//                  {
//                     code = ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT;
//                     msg.AppendLine(new Error().getError(code));
//                  }
//               }
//               else
//               {
//                  code = ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT;
//                  msg.AppendLine(new Error().getError(code));
//               }
//            }
//            else
//            {
//               code = 111;
//            }
//            model.Branch_Name = dv.Branch_Name;
//         }
//         else
//         {
//            code = ERROR_CODE.ERROR_DEVICE_SETUP_NOT_FOUND;
//            msg.AppendLine(new Error().getError(code));
//         }

//         if (model.do_migrate & dv != null && dvresult)
//         {
//            msg.AppendLine(Resource.Message_Start_Refresh_Data);
//            msg.AppendLine(Resource.Message_Start_Validate_Device_Data);
//            if (dv.Max_Transaction_Id.HasValue)
//            {
//               msg.AppendLine(Resource.Message_The_Last_Updated_Transacion_Id_Is + " " + dv.Max_Transaction_Id.Value.ToString() + ".");
//               var trans = dvMng.getTransactions(dv.Max_Transaction_Id.Value);

//               msg.AppendLine(Resource.Message_The_Transaction_Record_Successfully_Transferred + " (" + trans.Count().ToString() + ").");
//               if (trans.Count > 0)
//               {
//                  msg.AppendLine(Resource.Message_Start_Transfer_Device_Data);
//                  model.result = tmServ.UpdateTimeTransaction(userlogin.Company_ID, dv.Device_ID, trans);

//                  if (code >= 0)
//                     code = model.result.Code;

//                  msg.AppendLine(model.result.Msg);

//                  msg.AppendLine(Resource.Message_End_Transfer_Data);
//               }
//            }
//            else
//            {
//               code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
//               msg.AppendLine(Resource.Message_The_Max_Transacion_Not_Found);
//            }
//            msg.AppendLine(Resource.Message_End_Refresh_Data);
//         }
//         model.result = new ServiceResult() { Code = code, Msg = msg.ToString(), Field = Resource.Device };

//         var cri = new TimeTransactionCriteria();
//         cri.Company_ID = userlogin.Company_ID;
//         cri.Employee_Profile_ID = model.search_Employee_Profile_ID;
//         cri.Branch_ID = model.search_Branch_ID;
//         cri.Department_ID = model.search_Department_ID;
//         cri.From = DateUtil.ToDate(model.search_From);
//         cri.To = DateUtil.ToDate(model.search_To);
//         cri.Device_ID = model.search_Device_ID;

//         model.transList = tmServ.LstTimeTransaction(cri);
//         return View(model);
//      }

//      [HttpGet]
//      [AllowAuthorized]
//      public ActionResult SummaryReport(ServiceResult result, TimeTransactionViewModel model)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         var rightResult = base.validatePageRight(Operation.A);
//         if (rightResult.action != null) return rightResult.action;
//         model.rights = rightResult.rights;
//         model.result = result;

//         var tmServ = new TimeService(userlogin);
//         var cbServ = new ComboService();
//         var currentdate = StoredProcedure.GetCurrentDate();
//         var tmcbServ = new TimeComboService();
//         var wkServ = new WorkingDaysService();

//         model.workdays = wkServ.GetWorkingDay(userlogin.Company_ID);
//         model.cBranchlist = cbServ.LstBranch(userlogin.Company_ID, true);
//         model.cDepartmentlist = cbServ.LstDepartment(userlogin.Company_ID, true);
//         model.cEmplist = cbServ.LstEmployeeList(userlogin.Company_ID, model.search_Department_ID, true);
//         model.cDevicelist = tmcbServ.LstTimeDevice(userlogin.Company_ID, false);

//         if (string.IsNullOrEmpty(model.search_From))
//            model.search_From = DateUtil.ToDisplayDate(currentdate);
//         if (string.IsNullOrEmpty(model.search_To))
//            model.search_To = DateUtil.ToDisplayDate(currentdate);

//         var msg = new StringBuilder();
//         var code = 1;
//         var dvMng = new clsAccessManager();
//         if (!model.search_Device_ID.HasValue)
//         {
//            if (model.cDevicelist.Count > 0)
//               model.search_Device_ID = NumUtil.ParseInteger(model.cDevicelist[0].Value);
//         }
//         var dvresult = false;
//         var dv = tmServ.GetTimeDevice(model.search_Device_ID);
//         if (dv != null)
//         {
//            if (dv.Brand_Name == "Utouch")
//            {
//               if (!string.IsNullOrEmpty(dv.IP_Address))
//               {
//                  dvresult = dvMng.getConnection(dv.IP_Address);
//                  if (dvresult)
//                  {
//                     code = ERROR_CODE.SUCCESS_CONNECT;
//                     msg.AppendLine("(" + dv.IP_Address + ") " + new Success().getSuccess(code));
//                  }
//                  else
//                  {
//                     code = ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT;
//                     msg.AppendLine(new Error().getError(code));
//                  }
//               }
//               else
//               {
//                  code = ERROR_CODE.ERROR_DEVICE_CANNOT_CONNECT;
//                  msg.AppendLine(new Error().getError(code));
//               }
//            }
//            else
//            {
//               code = 111;
//            }

//            model.Branch_Name = dv.Branch_Name;
//         }
//         else
//         {
//            code = ERROR_CODE.ERROR_DEVICE_SETUP_NOT_FOUND;
//            msg.AppendLine(new Error().getError(code));
//         }

//         if (model.do_migrate & dv != null && dvresult)
//         {
//            msg.AppendLine(Resource.Message_Start_Refresh_Data);
//            msg.AppendLine(Resource.Message_Start_Validate_Device_Data);
//            if (dv.Max_Transaction_Id.HasValue)
//            {
//               msg.AppendLine(Resource.Message_The_Last_Updated_Transacion_Id_Is + " " + dv.Max_Transaction_Id.Value.ToString() + ".");
//               var trans = dvMng.getTransactions(dv.Max_Transaction_Id.Value);

//               msg.AppendLine(Resource.Message_The_Transaction_Record_Successfully_Transferred + " (" + trans.Count().ToString() + ").");
//               if (trans.Count > 0)
//               {
//                  msg.AppendLine(Resource.Message_Start_Transfer_Device_Data);
//                  model.result = tmServ.UpdateTimeTransaction(userlogin.Company_ID, dv.Device_ID, trans);

//                  if (code >= 0)
//                     code = model.result.Code;

//                  msg.AppendLine(model.result.Msg);

//                  msg.AppendLine(Resource.Message_End_Transfer_Data);
//               }
//            }
//            else
//            {
//               code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
//               msg.AppendLine(Resource.Message_The_Max_Transacion_Not_Found);
//            }
//            msg.AppendLine(Resource.Message_End_Refresh_Data);
//         }
//         model.result = new ServiceResult() { Code = code, Msg = msg.ToString(), Field = Resource.Device };

//         var cri = new TimeTransactionCriteria();
//         cri.Company_ID = userlogin.Company_ID;
//         cri.Employee_Profile_ID = model.search_Employee_Profile_ID;
//         cri.Branch_ID = model.search_Branch_ID;
//         cri.Department_ID = model.search_Department_ID;
//         cri.From = DateUtil.ToDate(model.search_From);
//         cri.To = DateUtil.ToDate(model.search_To);
//         cri.Device_ID = model.search_Device_ID;

//         model.transList = tmServ.Sum_LstTimeTransaction(cri);
//         return View(model);
//      }

//      [HttpGet]
//      public ActionResult DetailTransactionByEmp(int DeviceID, int EmpProfileID, String FromDate, String ToDate)
//      {
//         var userlogin = UserUtil.getUser(HttpContext);
//         if (userlogin == null)
//            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

//         TimeTransactionViewModel model = new TimeTransactionViewModel();
//         var cri = new TimeTransactionCriteria();
//         cri.Company_ID = userlogin.Company_ID;
//         cri.Employee_Profile_ID = EmpProfileID;
//         cri.Branch_ID = null;
//         cri.Department_ID = null;
//         cri.From = DateUtil.ToDate(FromDate);
//         cri.To = DateUtil.ToDate(ToDate);
//         cri.Device_ID = DeviceID;

//         var tmServ = new TimeService(userlogin);
//         model.transList = tmServ.LstTimeTransaction(cri);
//         return View(model);
//      }
//   }
//}