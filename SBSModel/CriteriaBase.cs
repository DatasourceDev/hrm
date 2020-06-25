using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel
{
    public class CriteriaBase
    {
        public int Start_Index { get; set; }
        public int Page_Size { get; set; }
        public Nullable<int> Top { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public Nullable<int> User_Authentication_ID { get; set; }
        public string Sort_By { get; set; }
        public bool hasBlank { get; set; }
        public string Record_Status { get; set; }
        public Nullable<DateTime> Update_On { get; set; }
        public string Text_Search { get; set; }

        public int End_Index { get; set; }
        public int Page_No { get; set; }
        public int Record_Count { get; set; }

        private bool _queryChild = true;
        public bool QueryChild {
            get { 
                return _queryChild;
            }
            set { 
                _queryChild = value; 
            }
        }

        public object Clone()
        {
            var obj =this.MemberwiseClone();
            return obj;
        }


    }
}
