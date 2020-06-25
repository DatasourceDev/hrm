using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Offline
{
    public class ServerService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ServerService()
        {
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(typeof(ServerService));
        }

        #region Default
        public int GetCompanyID(string pEmail)
        {
            using (var db = new ServerContext())
            {
                var d = db.Database.SqlQuery<int>("select Company_ID from User_Authentication where Email_Address = '" + pEmail + "'").ToList();
                if (d.Count > 0)
                {
                    return d[0];
                }
            }
            return 0;
        }
        public DateTime GetCurrentDate()
        {
            using (var db = new ServerContext())
            {
                var d = db.Database.SqlQuery<DateTime>("select distinct getdate() from sys.databases").ToList();
                if (d.Count > 0)
                {
                    return d[0];
                }
            }
            return new DateTime();
        }

        public string GetDatabaseName()
        {
            var name = "";
            using (var db = new ServerContext())
            {
                name = db.Database.Connection.DataSource + " (" + db.Database.Connection.Database + ")";
            }
            return name;
        }

        #endregion


        #region Get & List Data

        public int getCompanyIDByEmail(String pEmail)
        {
            using (var db = new ServerContext())
            {
                var d = db.Database.SqlQuery<int>("select p.Company_ID from User_Profile p inner join User_Authentication a on a.User_Authentication_ID = p.User_Authentication_ID  where a.Email_Address = '" + pEmail + "'").ToList();
                if (d.Count > 0)
                    return d[0];
            }
            return -1;
        }

        //public int GetCompanyID(string pCompanyName)
        //{

        //    using (var db = new ServerContext())
        //    {
        //        var tables = new List<TableShcema>();
        //        var d = db.Database.SqlQuery<int>("select [Company_ID] from Company_Details where [Name] = '" + pCompanyName + "'").ToList();
        //        if (d.Count > 0)
        //            return d[0];
        //    }
        //    return -1;
        //}

        public DataTable GetReceiptConfig(Nullable<int> pCompanyID)
        {
            using (var db = new ServerContext())
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);
                var sqlQuery = "select [Ref_Count], [Prefix], [Date_Format], [Num_Lenght], [Suffix] from POS_Receipt_Configuration where [Company_ID] = '" + pCompanyID + "'";
                using (var cmd = factory.CreateCommand())
                {
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = db.Database.Connection;
                    using (var adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;

                        var data = new DataTable();
                        adapter.Fill(data);

                        return data;
                    }
                }
            }
        }

        private List<TableShcema> GetTableShcema(List<TableShcema> pTables, string pTbName, List<DBColumn> pAllColsSchema)
        {
            if (pTables.Where(w => w.Table_Name == pTbName).FirstOrDefault() == null)
            {

                var tb = new TableShcema();
                tb.Table_Name = pTbName;
                tb.Cols = pAllColsSchema.Where(w => w.tablename == pTbName).ToList();

                var cons = new List<DBConstraint>();
                var script = new StringBuilder();
                script.AppendLine("create table [" + pTbName + "]");
                script.AppendLine("(");

                var pk = "";
                foreach (var col in tb.Cols)
                {
                    if (!string.IsNullOrEmpty(col.fktable) && !string.IsNullOrEmpty(col.fkcolname))
                    {
                        if (pTables.Where(w => w.Table_Name == col.fktable).FirstOrDefault() == null)
                        {
                            if (col.fktable != pTbName)
                            {
                                pTables = GetTableShcema(pTables, col.fktable, pAllColsSchema);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(col.key))
                    {
                        if (col.key.ToLower().Contains("pk_"))
                        {
                            if (col.type == "uniqueidentifier")
                            {
                                script.AppendLine("[" + col.colname + "]" + " uniqueidentifier ROWGUIDCOL not null UNIQUE,");
                            }
                            else
                            {
                                if (col.type == "varchar" | col.type == "nvarchar")
                                {
                                    script.AppendLine("[" + col.colname + "]" + " " + col.type + "(" + col.max_length + ") not null,");
                                }
                                else
                                {
                                    script.AppendLine("[" + col.colname + "]" + " int identity(1,1) not null,");
                                    tb.Have_Identity = true;
                                }

                            }
                            pk = "[" + col.colname + "]";

                            if (cons.Where(w => w.Constraint_Name == col.key).FirstOrDefault() == null)
                            {
                                var con = new DBConstraint();
                                con.Constraint_Name = col.key;
                                con.Constraint_Type = "pk";
                                con.Cols = new List<string>();
                                con.Cols.Add("[" + col.colname + "]");
                                cons.Add(con);
                            }
                            else
                            {
                                var con = cons.Where(w => w.Constraint_Name == col.key).FirstOrDefault();
                                if (con != null)
                                    con.Cols.Add("[" + col.colname + "]");
                            }
                        }
                        else
                        {
                            if (col.type == "varchar" | col.type == "nvarchar")
                            {
                                script.AppendLine("[" + col.colname + "]" + " " + col.type + "(" + col.max_length + ") null,");
                            }
                            else
                            {
                                script.AppendLine("[" + col.colname + "]" + " " + col.type + " null,");

                            }
                            if (cons.Where(w => w.Constraint_Name == col.key).FirstOrDefault() == null)
                            {
                                var con = new DBConstraint();
                                con.Constraint_Name = col.key;
                                con.Constraint_Type = "fk";
                                con.Cols = new List<string>();
                                con.Cols.Add(col.colname);

                                con.fkcolname = new List<string>();
                                con.fkcolname.Add(col.fkcolname);

                                con.fktable = new List<string>();
                                con.fktable.Add(col.fktable);

                                cons.Add(con);
                            }
                            else
                            {
                                var con = cons.Where(w => w.Constraint_Name == col.key).FirstOrDefault();
                                if (con != null)
                                {
                                    con.Cols.Add(col.colname);

                                    con.fkcolname = new List<string>();
                                    con.fkcolname.Add(col.fkcolname);

                                    con.fktable = new List<string>();
                                    con.fktable.Add(col.fktable);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (col.type == "decimal" | col.type == "numeric")
                        {
                            script.AppendLine("[" + col.colname + "]" + " " + "decimal (18,2)" + " null,");
                        }
                        else if (col.type == "varchar" | col.type == "nvarchar")
                        {
                            if (col.max_length == -1)
                            {
                                script.AppendLine("[" + col.colname + "]" + " " + col.type + "(max) null,");
                            }
                            else
                            {
                                script.AppendLine("[" + col.colname + "]" + " " + col.type + "(" + col.max_length + ") null,");
                            }

                        }
                        else if (col.type == "varbinary")
                        {
                            script.AppendLine("[" + col.colname + "]" + " " + col.type + "(max) FILESTREAM null,");
                        }
                        else
                        {
                            script.AppendLine("[" + col.colname + "]" + " " + col.type + " null,");
                        }
                    }

                }
                foreach (var con in cons)
                {
                    if (con.Constraint_Type == "pk")
                    {
                        var colsname = "";
                        var i = 0;
                        foreach (var col in con.Cols)
                        {
                            colsname += col;
                            if (i < con.Cols.Count - 1)
                                colsname += ",";
                            i++;
                        }
                        script.AppendLine("constraint [" + con.Constraint_Name + "] Primary KEY (" + colsname + "),");
                    }
                    else
                    {
                        script.AppendLine("constraint [" + con.Constraint_Name + "] FOREIGN KEY ([" + con.Cols[0] + "]) REFERENCES [" + con.fktable[0] + "] (" + con.fkcolname[0] + "),");
                    }
                }
                script.AppendLine(")");

                if (string.IsNullOrEmpty(pk))
                {
                    Console.WriteLine("Debug");
                }
                tb.Script_Create = script.ToString();
                pTables.Add(tb);
            }

            return pTables;
        }

        public List<TableShcema> LstTableShcema(string[] pTable = null, string pTableName = "")
        {
            using (var db = new ServerContext())
            {
                var sqlQuery = new StringBuilder();
                sqlQuery.AppendLine("select [Name] from sys.tables where name not in ('sysdiagrams','__MigrationHistory')");
                var tables = new List<TableShcema>();

                if (!string.IsNullOrEmpty(pTableName))
                {
                    sqlQuery.AppendLine("and Name = '" + pTableName + "'");
                }
                if (pTable != null)
                {
                    sqlQuery.AppendLine("and Name in (");
                    var i = 0;
                    foreach (var tb in pTable)
                    {
                        sqlQuery.Append("'" + tb + "'");

                        if (i < pTable.Length - 1)
                            sqlQuery.Append(",");

                        i++;
                    }
                    sqlQuery.AppendLine(")");
                }
                sqlQuery.AppendLine(" order by create_date");

                var tbnames = db.Database.SqlQuery<string>(sqlQuery.ToString()).ToList();
                if (tbnames.Count > 0)
                {
                    var allcolschema = LstColumns(tbnames);

                    foreach (var tbname in tbnames)
                    {
                        //if (tbname == "Company_Details")
                        //    Console.WriteLine("Create Shcema " + tbname);

                        tables = GetTableShcema(tables, tbname, allcolschema);
                    }
                    return tables;
                }
            }
            return new List<TableShcema>();
        }

        public List<DBColumn> LstColumns(List<string> pTbName)
        {
            using (var db = new ServerContext())
            {
                var sql = new StringBuilder();
                sql.AppendLine("select t.[name] tablename, c.[name] colname,  tp.[name] type, c.max_length");
                sql.AppendLine(", ( case when pk.pk is not null then  pk.pk  else cons.[name] end)  as [key]");
                sql.AppendLine(", cons.fktable, cons.fkcolname");
                sql.AppendLine(" from sys.tables t inner join sys.columns c on c.[object_id] = t.[object_id]");
                sql.AppendLine(" inner join sys.types tp on c.system_type_id = tp.system_type_id");
                sql.AppendLine(" left join ( select con.colid, o.[parent_object_id], o.[name], fk.[name] as fktable, pkc.[name] as fkcolname from sys.sysconstraints con inner join sys.objects o on con.constid = o.[object_id] left join sys.sysforeignkeys f on f.constid = con.constid left join sys.objects fk on f.rkeyid = fk.[object_id] left join sys.columns pkc on pkc.[object_id] = fk.[object_id] and pkc.column_id = 1) cons on cons.parent_object_id = t.[object_id] and c.column_id =  cons.colid left join ( select i.name pk, i.object_id  , icol.column_id from sys.indexes i inner join sys.index_columns icol on icol.object_id = i.object_id and i.index_id = icol.index_id where i.is_primary_key =1 ) pk on pk.[object_id] = t.[object_id] and pk.column_id = c.column_id");
                sql.AppendLine(" where 1 = 1 and (");
                var i = 0;
                foreach (var tbname in pTbName)
                {
                    sql.Append(" t.name <> '" + tbname + "' ");
                    if (i < pTbName.Count - 1)
                        sql.Append(" or");
                    i++;
                }
                sql.Append(" )");
                sql.AppendLine(" and tp.[name] <> 'sysname'");
                sql.AppendLine(" order by t.name, c.column_id");
                var tbcols = db.Database.SqlQuery<DBColumn>(sql.ToString()).ToList();
                if (tbcols.Count > 0)
                {
                    return tbcols;
                }
            }
            return new List<DBColumn>();
        }

        public List<DBColumn> LstColumns(string pTbName)
        {
            using (var db = new ServerContext())
            {

                var d = db.Database.SqlQuery<DBColumn>("select t.[name] tablename, c.[name] colname,  tp.[name] type, c.max_length, ( case when pk.pk is not null then  pk.pk  else cons.[name] end)  as [key], cons.fktable, cons.fkcolname from sys.tables t inner join sys.columns c on c.[object_id] = t.[object_id] inner join sys.types tp on c.system_type_id = tp.system_type_id left join ( select con.colid, o.[parent_object_id], o.[name], fk.[name] as fktable, pkc.[name] as fkcolname from sys.sysconstraints con inner join sys.objects o on con.constid = o.[object_id] left join sys.sysforeignkeys f on f.constid = con.constid left join sys.objects fk on f.rkeyid = fk.[object_id] left join sys.columns pkc on pkc.[object_id] = fk.[object_id] and pkc.column_id = 1) cons on cons.parent_object_id = t.[object_id] and c.column_id =  cons.colid left join ( select i.name pk, i.object_id  , icol.column_id from sys.indexes i inner join sys.index_columns icol on icol.object_id = i.object_id and i.index_id = icol.index_id where i.is_primary_key =1 ) pk on pk.[object_id] = t.[object_id] and pk.column_id = c.column_id where t.name = '" + pTbName + "' and tp.[name] <> 'sysname' order by c.column_id ").ToList();
                if (d.Count > 0)
                {
                    return d;
                }
            }
            return new List<DBColumn>();
        }

        public DataTable LstData(string sqlQuery)
        {
            try
            {
                using (var db = new ServerContext())
                {
                    try
                    {
                        DbProviderFactory factory = DbProviderFactories.GetFactory(db.Database.Connection);

                        using (var cmd = factory.CreateCommand())
                        {
                            cmd.CommandText = sqlQuery;
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = db.Database.Connection;
                            using (var adapter = factory.CreateDataAdapter())
                            {
                                adapter.SelectCommand = cmd;

                                var data = new DataTable();
                                adapter.Fill(data);
                                return data;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(DateTime.Now, ex);

                        throw;
                    }


                }
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now, ex);
            }
            return null;
        }

        #endregion

        #region Transaction

        public bool SendDatatoServer(MigrateCriteria cri)
        {
            var mgOffline = new ManageOffline(cri.Company_ID);
            var sql = new StringBuilder();
            var updateRefcnt = false;
            try
            {
                using (var db = new ServerContext())
                {

                    var colStr = new StringBuilder();
                    var pks = new List<DBColumn>();

                    colStr.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));

                    if (cri.Data != null)
                    {
                        var insertQuery = new StringBuilder();
                        var insertcnt = 0;

                        var updateQuery = new StringBuilder();
                        var updatecnt = 0;

                        //Get receipt configuration from server
                        var rcpConfigdata = GetReceiptConfig(cri.Company_ID);
                        DataRow rcpConfig = null;
                        if (rcpConfigdata != null && rcpConfigdata.Rows.Count > 0)
                            rcpConfig = rcpConfigdata.Rows[0];

                        var currentdate = GetCurrentDate();
                        int no = NumUtil.ParseInteger(rcpConfig["Ref_Count"].ToString());

                        foreach (DataRow row in cri.Data.Rows)
                        {
                            if (!cri.Data.Columns.Contains("Is_Uploaded") | !cri.Data.Columns.Contains("Is_Latest"))
                                continue;

                            if (cri.Data.Columns.Contains("Do_Not_Upload"))
                            {
                                var donotupload = Convert.ToBoolean(row["Do_Not_Upload"] is DBNull ? false : row["Do_Not_Upload"]);
                                if (donotupload)
                                    continue;
                            }

                            var isupload = Convert.ToBoolean(row["Is_Uploaded"] is DBNull ? false : row["Is_Uploaded"]);
                            if (!isupload)
                            {
                                //insert
                                insertQuery.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                insertQuery.AppendLine(colStr.ToString());
                                insertQuery.AppendLine(") values (");

                                var j = 0;
                                var colDataStr = new StringBuilder();
                                foreach (DataColumn col in cri.Data.Columns)
                                {
                                    if (col.ColumnName == "Is_Uploaded" | col.ColumnName == "Is_Latest")
                                    {
                                        row[col.ColumnName] = 1;
                                        colDataStr.AppendLine("1");
                                        if (j < cri.Data.Columns.Count - 1)
                                            colDataStr.Append(", ");
                                        j++;
                                        continue;
                                    }
                                    else if (col.ColumnName == "Receipt_No")
                                    {
                                        var rcpNo = rcpConfig["Prefix"] + currentdate.Date.ToString(rcpConfig["Date_Format"].ToString()) + no.ToString().PadLeft(NumUtil.ParseInteger(rcpConfig["Num_Lenght"].ToString()), '0') + rcpConfig["Suffix"];
                                        row[col.ColumnName] = rcpNo;
                                        colDataStr.AppendLine("'" + rcpNo + "'");
                                        no++;
                                        updateRefcnt = true;

                                        if (j < cri.Data.Columns.Count - 1)
                                            colDataStr.Append(", ");
                                        j++;
                                        continue;
                                    }
                                    else if (pks.Select(s => s.colname).Contains(col.ColumnName))
                                    {
                                        j++;
                                        continue;
                                    }
                                    var datatype = row[col.ColumnName].GetType().ToString();
                                    colDataStr.AppendLine(mgOffline.ConvertDBValue(row[col.ColumnName]));

                                    if (j < cri.Data.Columns.Count - 1)
                                        colDataStr.Append(", ");
                                    j++;
                                }
                                insertQuery.AppendLine(colDataStr.ToString());
                                insertQuery.AppendLine(");");
                                insertcnt++;
                            }
                            else
                            {
                                //update
                                updateQuery.AppendLine(" update [" + cri.Table.Table_Name + "] set");

                                var j = 0;
                                foreach (DataColumn col in cri.Data.Columns)
                                {
                                    if (col.ColumnName == "Is_Uploaded" | col.ColumnName == "Is_Latest")
                                    {
                                        row[col.ColumnName] = 1;

                                        updateQuery.Append(col.ColumnName + " = ");
                                        updateQuery.Append("1");

                                        if (j < cri.Data.Columns.Count - 1)
                                            updateQuery.Append(", ");
                                        j++;
                                        continue;
                                    }
                                    else if (pks.Select(s => s.colname).Contains(col.ColumnName))
                                    {
                                        j++;
                                        continue;
                                    }
                                    else
                                    {
                                        updateQuery.Append(col.ColumnName + " = ");
                                        updateQuery.Append(mgOffline.ConvertDBValue(row[col.ColumnName]));
                                        if (j < cri.Data.Columns.Count - 1)
                                            updateQuery.Append(", ");
                                        j++;
                                    }

                                }


                                foreach (var pk in pks)
                                {
                                    var pkname = pk.colname;
                                    if (pk.type == "int")
                                        updateQuery.AppendLine(" where " + pkname + " = " + row[pkname] + ";");
                                    else
                                        updateQuery.AppendLine(" where " + pkname + " = '" + row[pkname] + "' ;");
                                }

                                updatecnt++;
                            }
                        }


                        if (insertcnt > 0)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine(insertQuery.ToString());
                            if (updateRefcnt)
                                sql.AppendLine("update POS_Receipt_Configuration set Ref_Count = " + no + ";");

                            sql.AppendLine("Select SCOPE_IDENTITY();");
                            var lastID = db.Database.SqlQuery<decimal>(sql.ToString()).FirstOrDefault();
                            db.SaveChanges();
                            if (lastID > 0)
                            {
                                for (var j = cri.Data.Rows.Count - 1; j >= 0; j--)
                                {
                                    foreach (var pk in pks)
                                    {
                                        log.Info(DateTime.Now + ": Sent " + cri.Table.Table_Name + "." + pk.colname + " (" + lastID + ") to server.");
                                        var key = pk.colname;
                                        cri.Data.Rows[j][key] = lastID;
                                        lastID -= 1;
                                        break;
                                    }
                                }

                            }
                        }

                        if (updatecnt > 0)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine(updateQuery.ToString());
                            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql.ToString());
                            db.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now, ex);
                Console.WriteLine(sql);
                return false;
            }
        }
        #endregion

        #region Other
        public bool ExecuteSqlCommand(string sql)
        {
            using (var db = new ServerContext())
            {
                try
                {
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    log.Error(DateTime.Now, ex);
                    return false;
                }
            }

        }
        #endregion
    }
}
