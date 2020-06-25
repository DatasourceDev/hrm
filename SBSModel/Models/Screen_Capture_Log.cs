using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Screen_Capture_Log
    {
        public Screen_Capture_Log()
        {
            this.Screen_Capture_Image = new List<Screen_Capture_Image>();
        }

        public int SC_Log_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }
        public string Description { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public virtual ICollection<Screen_Capture_Image> Screen_Capture_Image { get; set; }
        public virtual User_Profile User_Profile { get; set; }
    }
}
