using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SBSResourceAPI;

namespace HR.Models
{
    //Added by sun 
    public class ExchangeViewModel : ModelBase
    {
        public List<ComboViewModel> StatusComboList { get; set; }
        public List<ComboViewModel> Currency_List { get; set; }
        public List<ComboViewModel> Month_List { get; set; }
        public List<DateTime> Date_List { get; set; }

        public Nullable<int> Exchange_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Company_Currency_ID { get; set; }
        public string Company_Currency_Code { get; set; }
        public string Company_Currency_Name { get; set; }

        [LocalizedRequired]
        [LocalizedDisplayName("Year", typeof(Resource))]
        public Nullable<int> Fiscal_Year { get; set; }

        public String tabAction { get; set; }
        public String Record_Status { get; set; }
        public List<ExchangeCurrencyViewModel> LstExchangeCurrency { get; set; }
        public ExchangeCurrencyViewModel[] ExchangeCurrency_Rows { get; set; }
    }
    public class ExchangeCurrencyViewModel : ModelBase
    {
        public Nullable<int> Foreign_ID { get; set; }
        public Nullable<int> Exchange_Currency_ID { get; set; }
        public Nullable<int> Exchange_ID { get; set; }
        public string Exchange_Period { get; set; }
        public List<ComboViewModel> Currency_List { get; set; }
        public Nullable<int> Month_ID { get; set; }
        public string Month_Name { get; set; }
        public string Top_Name { get; set; }
        public Nullable<int> Index { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public string Company_Currency_Name { get; set; }
        public string Row_Type { get; set; }
        public ExchangeRateViewModel[] ExchangeRate_Rows { get; set; }
    }

    public class ExchangeRateViewModel : ModelBase
    {
        public List<ComboViewModel> Month_List;
        public List<DateTime> Date_List;
        public Nullable<int> Index { get; set; }
        public Nullable<int> Month_ID { get; set; }
        public string Row_Type { get; set; }
        public string Exchange_Date { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> Exchange_Rate_ID { get; set; }
        public Nullable<int> Exchange_Currency_ID { get; set; }
        public string Exchange_Period { get; set; }
        public Nullable<int> Exchange_Month { get; set; }
        public string Exchange_Month_Text { get; set; }
        public Nullable<int> Currency_ID { get; set; }
        public string Currency_Code { get; set; }
        public string Currency_Name { get; set; }
    }

}