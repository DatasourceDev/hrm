using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class CPF_Formula
    {
        public CPF_Formula()
        {
            this.Selected_CPF_Formula = new List<Selected_CPF_Formula>();
        }

        public int CPF_Formula_ID { get; set; }
        public string Formula { get; set; }
        public string Formula_Name { get; set; }
        public string Formula_Desc { get; set; }
        public Nullable<int> Year { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string Record_Status { get; set; }
        public virtual ICollection<Selected_CPF_Formula> Selected_CPF_Formula { get; set; }
    }
}
