using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class HR_FileExport_History
    {
        public HR_FileExport_History()
        {
            this.HR_FileExport_History_Detail = new List<HR_FileExport_History_Detail>();
            this.HR_FileExport_IR8S_Detail = new List<HR_FileExport_IR8S_Detail>();
        }

        public int Generated_ID { get; set; }
        public Nullable<int> Process_Year { get; set; }
        public Nullable<int> Process_Month { get; set; }
        public string File_Type { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<System.DateTime> Generated_Date { get; set; }
        public string File_LocalPath { get; set; }
        public int RFF_Code { get; set; }
        public string File_Name { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<HR_FileExport_History_Detail> HR_FileExport_History_Detail { get; set; }
        public virtual ICollection<HR_FileExport_IR8S_Detail> HR_FileExport_IR8S_Detail { get; set; }
    }
}
