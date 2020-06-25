using POS.Models;
using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POS.Controllers {
    public class POSReportingController : ControllerBase {

        public ActionResult SalesDashboard() {
            var userlogin = UserSession.getUser(HttpContext);

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            return View();
        }

        #region JSON Functions
        public JsonResult GetCategorySalesPerQuarter(string pYear) {
            var userlogin = UserSession.getUser(HttpContext);
            var iService = new InventoryService();
            var pService = new POSService();
            int year = 1900;

            List<CategoryReportViewModel> catSalesPerQuarter = new List<CategoryReportViewModel>();

            List<Product_Category> categories = iService.LstCategory(getCategoryCriteria());

            var posCri = new POSReciptCriteria() {
                Company_ID = userlogin.Company_ID
            };

            if (!string.IsNullOrEmpty(pYear)){
                year = Convert.ToInt32(pYear);
            }

            List<POS_Receipt> rcps = pService.LstPOSReceipt(posCri);
            CategoryReportViewModel catSales;

            if (categories != null) {
                foreach (var cat in categories) {
                    catSales = new CategoryReportViewModel();

                    catSales.Category_ID = cat.Product_Category_ID;
                    catSales.Category_Name = cat.Category_Name;
                    catSales.Quarter_Sales = new decimal[4];

                    for (int idx = 0; idx <= 3; idx++) {
                        catSales.Quarter_Sales[idx] = getCategorySalesPerQtr(year, idx + 1, cat.Product_Category_ID, rcps);
                    }

                    catSalesPerQuarter.Add(catSales);
                }

                //uncategorized items
                catSales = new CategoryReportViewModel();
                catSales.Category_ID = 0;
                catSales.Category_Name = "Uncategorized";
                catSales.Quarter_Sales = new decimal[4];

                for (int idx = 0; idx <= 3; idx++) {
                    catSales.Quarter_Sales[idx] = getCategorySalesPerQtr(year, idx + 1, 0, rcps);
                }

                catSalesPerQuarter.Add(catSales);
            }

            return Json(catSalesPerQuarter, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductSalesByQuarter(int pQuarter, int pYear) {
            var userlogin = UserSession.getUser(HttpContext);
            var iService = new InventoryService();
            var pService = new POSService();

            List<ProductQuarterSales> productSalesByQuarter = new List<ProductQuarterSales>();

            List<Product> products = iService.LstProducts(userlogin.Company_ID, null);

            var posCri = new POSReciptCriteria() {
                Company_ID = userlogin.Company_ID
            };

            List<POS_Receipt> rcps = pService.LstPOSReceipt(posCri);
            ProductQuarterSales productSales;

            if (products != null) {
                foreach (var prod in products) {
                    productSales = new ProductQuarterSales();

                    productSales.Product_ID = prod.Product_ID;
                    productSales.Product_Name = prod.Product_Name;
                    productSales.Quarter_ID = pQuarter;
                    productSales.Quarter = "Q" + pQuarter;
                    productSales.Total_Sales = getProductSalesPerQtr(pQuarter, pYear, prod.Product_ID, rcps);

                    productSalesByQuarter.Add(productSales);
                }

                //uncategorized items
                productSales = new ProductQuarterSales();

                productSales.Product_ID = 0;
                productSales.Product_Name = "Open Products";
                productSales.Quarter_ID = pQuarter;
                productSales.Quarter = "Q" + pQuarter;
                productSales.Total_Sales = getProductSalesPerQtr(pQuarter, pYear, 0, rcps);

                productSalesByQuarter.Add(productSales);
            }
            return Json(productSalesByQuarter, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetYearToDateSales(int pYear) {
            var userlogin = UserSession.getUser(HttpContext);
            var iService = new InventoryService();
            var pService = new POSService();

            var posCri = new POSReciptCriteria() {
                Company_ID = userlogin.Company_ID
            };
            List<POS_Receipt> rcps = pService.LstPOSReceipt(posCri);
            List<MonthlySales> monthlySales = new List<MonthlySales>();

            string[] months = getMonths(pYear);

            if (months != null) {

                MonthlySales monthSale = new MonthlySales();
                monthSale.Year = pYear;
                monthSale.Months = months;
                monthSale.Monthly_Sales = new decimal[months.Length];

                var dtf = CultureInfo.CurrentCulture.DateTimeFormat;

                for (var idx = 0; idx < months.Length; idx++) {
                    monthSale.Monthly_Sales[idx] = getMonthSale(pYear, idx + 1, rcps);
                }
                monthlySales.Add(monthSale);
            }

            return Json(monthlySales, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Functions
        private CategoryCriteria getCategoryCriteria() {
            var userlogin = UserSession.getUser(HttpContext);
            var pService = new POSService();

            var ccri = new CategoryCriteria() {
                Company_ID = userlogin.Company_ID,
                Branch_ID = pService.GetTerminal(userlogin.Profile_ID).Branch_ID,
                hasBlank = false,
                Record_Status = RecordStatus.Active
            };

            return ccri;
        }

        private ProductCriteria getProductCriteria() {
            var userlogin = UserSession.getUser(HttpContext);
            var pService = new POSService();

            var ccri = new ProductCriteria() {
                Company_ID = userlogin.Company_ID,
                Branch_ID = pService.GetTerminal(userlogin.Profile_ID).Branch_ID,
                hasBlank = false,
                Record_Status = RecordStatus.Active
            };

            return ccri;
        }

        private decimal getCategorySalesPerQtr(int year, int quarter, int categoryId, List<POS_Receipt> rcps) {
            decimal totalSales = 0;
            DateTime? firstDayOfQuarter = null;
            DateTime? lastDayOfQuarter = null;

            if (rcps != null) {
                switch (quarter) {
                    case 1:
                        firstDayOfQuarter = new DateTime(year, 1, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 3);
                        break;
                    case 2:
                        firstDayOfQuarter = new DateTime(year, 4, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 6);
                        break;
                    case 3:
                        firstDayOfQuarter = new DateTime(year, 7, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 9);
                        break;
                    case 4:
                        firstDayOfQuarter = new DateTime(year, 10, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 12);
                        break;
                }

                if (categoryId > 0) {
                    totalSales = (decimal)rcps.Where(x => x.Status != "Void" && x.Status != "Hold"
                        && (x.Receipt_Date.Value >= firstDayOfQuarter && x.Receipt_Date.Value <= lastDayOfQuarter)).SelectMany(x => x.POS_Products_Rcp.Where(
                        y => y.Product_ID != null && y.Product.Product_Category != null && y.Product.Product_Category.Product_Category_ID == categoryId)).Sum(z => z.Price * z.Qty);
                } else {
                    totalSales = (decimal)rcps.Where(x => x.Status != "Void" && x.Status != "Hold"
                            && (x.Receipt_Date.Value >= firstDayOfQuarter && x.Receipt_Date.Value <= lastDayOfQuarter)).SelectMany(x => x.POS_Products_Rcp.Where(
                            y => y.Product_ID == null || y.Product.Product_Category == null)).Sum(z => z.Price * z.Qty);
                }
            }

            return totalSales;
        }

        private decimal getProductSalesPerQtr(int quarter, int year, int productId, List<POS_Receipt> rcps) {
            decimal totalSales = 0;
            DateTime? firstDayOfQuarter = null;
            DateTime? lastDayOfQuarter = null;

            if (rcps != null) {
                switch (quarter) {
                    case 1:
                        firstDayOfQuarter = new DateTime(year, 1, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 3);
                        break;
                    case 2:
                        firstDayOfQuarter = new DateTime(year, 4, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 6);
                        break;
                    case 3:
                        firstDayOfQuarter = new DateTime(year, 7, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 9);
                        break;
                    case 4:
                        firstDayOfQuarter = new DateTime(year, 10, 1);
                        lastDayOfQuarter = getLastDayOfMonth(year, 12);
                        break;
                }

                if (productId > 0) {
                    totalSales = (decimal)rcps.Where(x => x.Status != "Void" && x.Status != "Hold"
                        && (x.Receipt_Date.Value >= firstDayOfQuarter.Value && x.Receipt_Date.Value <= lastDayOfQuarter.Value)).SelectMany(x => x.POS_Products_Rcp.Where(
                        y => y.Product_ID == productId)).Sum(z => z.Price * z.Qty);
                } else {
                    totalSales = (decimal)rcps.Where(x => x.Status != "Void" && x.Status != "Hold"
                            && (x.Receipt_Date.Value >= firstDayOfQuarter.Value && x.Receipt_Date.Value <= lastDayOfQuarter.Value)).SelectMany(x => x.POS_Products_Rcp.Where(
                            y => y.Product_ID == null)).Sum(z => z.Price * z.Qty);
                }
            }

            return totalSales;
        }

        private decimal getMonthSale(int year, int month, List<POS_Receipt> rcps) {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = getLastDayOfMonth(year, month);
            decimal totalSales = 0;

            totalSales = (decimal)rcps.Where(x => x.Status != "Void" && x.Status != "Hold"
                                && (x.Receipt_Date.Value >= firstDayOfMonth && x.Receipt_Date.Value <= lastDayOfMonth)).Sum(z => z.Total_Amount);

            return totalSales;
        }

        private string[] getMonths(int pYear) {
            string[] months;

            if (pYear < DateTime.Today.Year) {
                months = new string[12];

            } else {
                months = new string[DateTime.Today.Month];
            }

            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            for (var idx = 0; idx < months.Length; idx++) {
                months[idx] = dtf.GetMonthName(idx + 1);
            }

            return months;
        }

        private DateTime getLastDayOfMonth(int year, int month) {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            return firstDayOfMonth.AddMonths(1).AddDays(-1);
        }

        #endregion


    }
}