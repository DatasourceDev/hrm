using System;
using System.Collections.Generic;

namespace SBSModel.Models
{
    public partial class Screen_Capture_Image
    {
        public System.Guid SC_Image_ID { get; set; }
        public Nullable<int> SC_Log_ID { get; set; }
        public byte[] Image { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_On { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_On { get; set; }
        public string File_Name { get; set; }
        public virtual Screen_Capture_Log Screen_Capture_Log { get; set; }
    }
}
