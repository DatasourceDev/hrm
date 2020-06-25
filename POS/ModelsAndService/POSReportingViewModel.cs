using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SBSModel.Models;

namespace POS.Models {
    public class CategoryReportViewModel : ModelBase {
        public int Category_ID { get; set; }
        public string Category_Name { get; set; }
        public decimal[] Quarter_Sales { get; set; }
    }

    public class ProductQuarterSales : ModelBase {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; }
        public int Quarter_ID { get; set; }
        public string Quarter { get; set; }
        public decimal Total_Sales { get; set; }
    }

    public class MonthlySales : ModelBase {
        public int Year { get; set; }
        public string[] Months { get; set; }
        public decimal[] Monthly_Sales { get; set; }
    }

}