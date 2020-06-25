using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Offline
{
    public class MigrateCriteria
    {
        public TableShcema Table { get; set; }
        public DataTable Data { get; set; }
        public bool Have_Del_Data { get; set; }
        public string Del_Key { get; set; }
        public string Del_Key2 { get; set; }
        public string Del_Key3 { get; set; }
        public string Del_Key4 { get; set; }
        public string Del_Key5 { get; set; }
        public Nullable<int> Del_Value { get; set; }
        public bool Is_NewDB { get; set; }
        public Nullable<int> Company_ID { get; set; }

    }

    public class SendReceiptCriteria
    {
        public TableShcema Table { get; set; }
        public DataTable Receipt_Data { get; set; }
        public DataTable Receipt_Payment_Data { get; set; }
        public DataTable Receipt_Product_Data { get; set; }
        public DataTable Transaction_Data { get; set; }
        public Nullable<int> Company_ID { get; set; }

    }

    public class GetScriptCriteria
    {
        public TableShcema Table { get; set; }
        public string Key { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string Key4 { get; set; }
        public string Key5 { get; set; }
        public Nullable<int> Key_ID { get; set; }
        public bool Where_Update_On { get; set; }
        public Nullable<DateTime> Update_On { get; set; }
        public bool getfkscript { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public bool Where_Is_Uploaded { get; set; }
        public bool Where_Is_Latest { get; set; }
        public bool Where_Do_Not_Upload { get; set; }
    }

    public class TableShcema
    {
        public string Table_Name { get; set; }
        public string Script_Create { get; set; }
        public List<DBColumn> Cols { get; set; }
        public bool Have_Identity { get; set; }
    }

    public class DBConstraint
    {
        public string Constraint_Name { get; set; }
        public string Constraint_Type { get; set; }
        public List<string> Cols { get; set; }
        public List<string> fktable { get; set; }
        public List<string> fkcolname { get; set; }
    }

    public class DBColumn
    {
        public string tablename { get; set; }
        public string colname { get; set; }
        public string type { get; set; }
        public Int16 max_length { get; set; }
        public string key { get; set; }
        public string fktable { get; set; }
        public string fkcolname { get; set; }
    }
}
