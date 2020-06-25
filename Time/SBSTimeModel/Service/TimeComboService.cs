using SBSModel.Common;
using SBSModel.Models;
using SBSTimeModel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    public class TimeComboService
    {
        public List<ComboViewModel> LstTimeDevice(Nullable<int> pCompanyID = null, bool hasBlank = true)
        {
            using (var db = new SBS2TimeDBContext())
            {
                var clist = new List<ComboViewModel>();
                if (hasBlank)
                    clist.Insert(0, new ComboViewModel() { Value = null, Text = "-" });

                var divs = db.Time_Device.Where(w => w.Company_ID == pCompanyID & w.Record_Status == RecordStatus.Active)
                    .Select(s => new ComboViewModel()
                    {
                        Value = SqlFunctions.StringConvert((double)s.Device_ID).Trim(),
                        Text = s.Device_No
                    });
                if (divs.Count() > 0)
                    clist.AddRange(divs);

                return clist;
            }
        }
        public List<ComboViewModel> LstBrandName()
        {
            var clist = new List<ComboViewModel>();

            clist.Add(new ComboViewModel { Value = "Utouch", Text = "Utouch" });
            clist.Add(new ComboViewModel { Value = "ZKAccess", Text = "ZKAccess" });
            clist.Add(new ComboViewModel { Value = "RFID", Text = "RFID" });
            return clist;
        }
    }
}
