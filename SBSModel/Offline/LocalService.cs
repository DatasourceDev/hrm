
using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SBSModel.Offline
{
    public class LocalService
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LocalService()
        {
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(typeof(LocalService));
        }
        #region Default

        public string GetDatabaseName()
        {
            var name = "";
            using (var db = new SBS2DBContext())
            {
                name = db.Database.Connection.DataSource + " (" + db.Database.Connection.Database + ")";
            }
            return name;
        }

        public bool DatabaseIsExist(string pDBName)
        {

            using (var db = new LocalInitialContext())
            {
                var d = db.Database.SqlQuery<string>("select name from master.sys.databases where name = N'" + pDBName + "'").ToList();
                if (d.Count > 0)
                {
                    return true;
                }
                var script = new StringBuilder();
                script.AppendLine("CREATE DATABASE " + pDBName + " ");
                script.AppendLine("EXEC sp_configure filestream_access_level, 2 RECONFIGURE ");
                script.AppendLine("alter database " + pDBName + " add FILEGROUP [SBS2_Data_Files] contains filestream ");
                script.AppendLine("alter database " + pDBName + " add FILE ( Name = '" + pDBName + "_file', FILENAME = 'C:\\SBS2\\SBS2_Files' ) TO FILEGROUP [SBS2_Data_Files] ");

                db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, script.ToString());
                db.SaveChanges();
            }
            return false;
        }

        public bool TableIsExist(string pTableName)
        {

            using (var db = new LocalInitialContext())
            {
                var d = db.Database.SqlQuery<string>("select name from sys.tables where name = N'" + pTableName + "'").ToList();
                if (d.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Get & List Data
        public Nullable<DateTime> GetUpdateOn(string pTableName)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var d = db.Database.SqlQuery<DateTime>("select max(update_on) from " + pTableName).ToList();
                    if (d.Count > 0)
                    {
                        return d[0];
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        public List<string> LstTableName()
        {
            using (var db = new SBS2DBContext())
            {

                var d = db.Database.SqlQuery<string>("select [Name] from sys.pTables where name not in ( 'sysdiagrams') order by create_date desc ").ToList();
                if (d.Count > 0)
                {
                    return d;
                }
            }
            return new List<string>();
        }

        public List<DBColumn> LstColumns(string pTable)
        {
            using (var db = new SBS2DBContext())
            {

                var d = db.Database.SqlQuery<DBColumn>("select c.[name] colname,  tp.[name] type, c.max_length, ( case when pk.pk is not null then  pk.pk  else cons.[name] end)  as [key], cons.fktable, cons.fkcolname from sys.pTables t inner join sys.columns c on c.[object_id] = t.[object_id] inner join sys.types tp on c.system_type_id = tp.system_type_id left join ( select con.colid, o.[parent_object_id], o.[name], fk.[name] as fktable, pkc.[name] as fkcolname from sys.sysconstraints con inner join sys.objects o on con.constid = o.[object_id] left join sys.sysforeignkeys f on f.constid = con.constid left join sys.objects fk on f.rkeyid = fk.[object_id] left join sys.columns pkc on pkc.[object_id] = fk.[object_id] and pkc.column_id = 1) cons on cons.parent_object_id = t.[object_id] and c.column_id =  cons.colid left join ( select i.name pk, i.object_id  , icol.column_id from sys.indexes i inner join sys.index_columns icol on icol.object_id = i.object_id and i.index_id = icol.index_id where i.is_primary_key =1 ) pk on pk.[object_id] = t.[object_id] and pk.column_id = c.column_id where t.name = '" + pTable + "' and tp.[name] <> 'sysname' order by c.column_id ").ToList();
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
                using (var db = new SBS2DBContext())
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
        public bool CreateNewTable(List<TableShcema> pTables)
        {
            try
            {

                using (var db = new SBS2DBContext())
                {
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "create table tblog (Table_Name varchar(300) null, Description varchar(max) null);");
                    foreach (var tb in pTables)
                    {
                        try
                        {
                            //var d = db.Database.SqlQuery<string>("select name from sys.tables where [name] = '" + tb.Table_Name + "';").ToList();
                            //if (d.Count > 0)
                            //{
                            //    continue;
                            //}

                            if (!string.IsNullOrEmpty(tb.Script_Create))
                            {
                                db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, tb.Script_Create);
                                Console.WriteLine("Create table " + tb.Table_Name);
                            }

                        }
                        catch (Exception ex)
                        {
                            log.Error(DateTime.Now, ex);
                            return false;
                        }
                    }


                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTable(List<TableShcema> pTables)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    for (int i = pTables.Count() - 1; i >= 0; i--)
                    {
                        try
                        {
                            //if (pTables[i].Table_Name == "Company")
                            //    Console.WriteLine("Break Migrate Data " + pTables[i].Table_Name);

                            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "delete from [" + pTables[i].Table_Name + "]");

                        }
                        catch (Exception ex)
                        {
                            log.Error(DateTime.Now, ex);
                            //return false;
                        }
                    }



                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool MigrateDataToLocal(MigrateCriteria cri)
        {
            var mgOffline = new ManageOffline(cri.Company_ID);
            var sql = new StringBuilder();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (cri.Have_Del_Data && !string.IsNullOrEmpty(cri.Del_Key) && cri.Del_Value.HasValue)
                    {
                        var delsql = new StringBuilder();
                        delsql.Append("Delete from " + cri.Table.Table_Name + " where " + cri.Del_Key + " = " + cri.Del_Value);
                        if (!string.IsNullOrEmpty(cri.Del_Key2))
                            delsql.Append(" or " + cri.Del_Key2 + " = " + cri.Del_Value);

                        if (!string.IsNullOrEmpty(cri.Del_Key3))
                            delsql.Append(" or " + cri.Del_Key3 + " = " + cri.Del_Value);

                        if (!string.IsNullOrEmpty(cri.Del_Key4))
                            delsql.Append(" or " + cri.Del_Key4 + " = " + cri.Del_Value);

                        if (!string.IsNullOrEmpty(cri.Del_Key5))
                            delsql.Append(" or " + cri.Del_Key5 + " = " + cri.Del_Value);

                        db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, delsql.ToString());
                        db.SaveChanges();
                    }

                    var colStr = new StringBuilder();
                    var pks = new List<DBColumn>();

                    colStr.Append(mgOffline.GenerateAllColumnString(cri.Table, pks));

                    if (cri.Data != null)
                    {
                        var sqlQuery = new StringBuilder();

                        foreach (DataRow row in cri.Data.Rows)
                        {
                           

                            var j = 0;
                            //update
                            sqlQuery.Append(" update [" + cri.Table.Table_Name + "] set ");

                            foreach (DataColumn col in cri.Data.Columns)
                            {
                                var ispk = false;
                                foreach (var pk in pks)
                                {
                                    var pkname = pk.colname;
                                    if (pkname == col.ColumnName)
                                    {
                                        ispk = true;
                                        break;
                                    }
                                }

                                if (ispk)
                                {
                                    j++; continue;
                                }

                                sqlQuery.Append(col.ColumnName + " = ");

                                var datatype = row[col.ColumnName].GetType().ToString();
                                if (datatype == "System.Int32")
                                {
                                    sqlQuery.Append(row[col.ColumnName].ToString());

                                }
                                else if (datatype == "System.Boolean")
                                {
                                    if (row[col.ColumnName].ToString().ToLower() == "true")
                                    {
                                        sqlQuery.Append("1");
                                    }
                                    else
                                    {
                                        sqlQuery.Append("0");
                                    }
                                }
                                else if (datatype == "System.DateTime")
                                {
                                    

                                    var date = "";
                                    if (row[col.ColumnName] is DBNull || string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                        date = "null";
                                    else
                                    {
                                        try
                                        {
                                            var datetimestr = ((DateTime)row[col.ColumnName]).ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US", false));

                                            var datestr = DateUtil.ToInternalDate(datetimestr);
                                            date = "'" + datestr + "'";
                                            if (col.ColumnName == "Start_Date")
                                                sql.AppendLine(" insert into tblog (Table_Name, Description) values('" + cri.Table.Table_Name + "','C" + datestr + "');");
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Error(DateTime.Now, ex);
                                            date = "null";
                                        }
                                    }
                                    sqlQuery.Append(date);
                                }
                                else if (datatype == "System.DBNull")
                                {
                                    sqlQuery.Append("null");
                                }
                                else if (datatype == "System.Byte[]")
                                {
                                    var bt = "0x" + BitConverter.ToString((byte[])row[col.ColumnName]).Replace("-", "");
                                    sqlQuery.AppendLine("convert(varbinary(max), " + bt + ")");
                                }
                                else if (datatype == "System.Decimal")
                                {
                                    sqlQuery.Append(row[col.ColumnName].ToString());
                                }
                                else
                                {
                                    sqlQuery.Append("'" + row[col.ColumnName].ToString().Replace("'", "''") + "'");
                                }

                                if (j < cri.Data.Columns.Count - 1)
                                    sqlQuery.Append(", ");

                                j++;
                            }

                            foreach (var pk in pks)
                            {
                                var pkname = pk.colname;
                                sqlQuery.AppendLine();
                                if (pk.type == "int")
                                    sqlQuery.AppendLine(" where " + pkname + " = " + row[pkname] + ";");
                                else
                                    sqlQuery.AppendLine(" where " + pkname + " = '" + row[pkname] + "' ;");
                            }
                            sqlQuery.AppendLine(" if @@rowcount = 0 ");
                            sqlQuery.AppendLine(" begin ");

                            //insert
                            sqlQuery.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                            sqlQuery.AppendLine(colStr.ToString());
                            sqlQuery.AppendLine(") values (");

                            j = 0;
                            var colDataStr = new StringBuilder();
                            foreach (DataColumn col in cri.Data.Columns)
                            {
                                colDataStr.Append(mgOffline.ConvertDBValue(row[col.ColumnName]));

                                if (j < cri.Data.Columns.Count - 1)
                                    colDataStr.Append(", ");

                                j++;
                            }
                            sqlQuery.Append(colDataStr.ToString());
                            sqlQuery.AppendLine(");");

                            sqlQuery.AppendLine(" end ");
                        }


                        sql.AppendLine(" begin tran ");
                        if (cri.Table.Have_Identity)
                            sql.AppendLine(" SET IDENTITY_INSERT [" + cri.Table.Table_Name + "] ON;");

                        sql.AppendLine(sqlQuery.ToString());
                        if (cri.Table.Have_Identity)
                            sql.AppendLine(" SET IDENTITY_INSERT [" + cri.Table.Table_Name + "] OFF;");

                        sql.AppendLine(" commit tran ");

                        if (cri.Table.Table_Name == "Subscription")
                        {
                            sql.AppendLine(" insert into tblog (Table_Name, Description) values('" + cri.Table.Table_Name + "','" + sqlQuery.ToString().Replace("'", "''") + "');");
                        }
                        db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql.ToString());
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now, ex);
                //log.Error(DateTime.Now + Environment.NewLine + sql.ToString());
                return false;
            }
        }

        public bool UpdateLocal(MigrateCriteria cri)
        {
            //update id or flat at local database when sent data to server successfully.
            var mgOffline = new ManageOffline(cri.Company_ID);
            using (var db = new SBS2DBContext())
            {
                var sql = new StringBuilder();
                try
                {
                    if (cri.Data != null && cri.Data.Rows.Count > 0)
                    {
                        foreach (DataRow row in cri.Data.Rows)
                        {
                            if (cri.Table.Table_Name == "POS_Terminal")
                            {
                                // do check pos terminal if 
                                var posterminal = db.Database.SqlQuery<int>("select Terminal_ID  from POS_Terminal where Terminal_Local_ID = '" + row["Terminal_Local_ID"] + "';").FirstOrDefault();
                                if (posterminal <= 0)
                                    continue;
                                if (posterminal == NumUtil.ParseInteger(row["Terminal_ID"].ToString()))
                                {
                                    sql.AppendLine("update POS_Terminal set Is_Uploaded = 1 , Is_Latest = 1 where [Terminal_ID]  = '" + row["Terminal_ID"] + "';");
                                    continue;
                                }

                                // check dupid
                                var dupID = db.Database.SqlQuery<int>("select [Terminal_ID]  from POS_Terminal where Terminal_ID = '" + row["Terminal_ID"] + "';").FirstOrDefault();
                                if (dupID > 0)
                                {
                                    // move to new row
                                    var pks = new List<DBColumn>();
                                    pks.Add(cri.Table.Cols.Where(w => w.colname == "Terminal_ID").FirstOrDefault());
                                    sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(")  ");
                                    sql.AppendLine("select ");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(" from [" + cri.Table.Table_Name + "] ");
                                    sql.AppendLine(" where [POS_Terminal] = '" + row["Terminal_ID"] + "';");
                                    sql.AppendLine("delete from POS_Terminal where  [Terminal_ID] = " + row["Terminal_ID"] + ";");
                                }

                                sql.Append(GenerateInsertString(cri, row));
                                sql.AppendLine("update POS_Shift set Terminal_ID = " + row["Terminal_ID"] + " where Terminal_Local_ID = '" + row["Terminal_Local_ID"] + "';");
                                sql.AppendLine("delete from POS_Terminal where Terminal_Local_ID = '" + row["Terminal_Local_ID"] + "' and Terminal_ID <> " + row["Terminal_ID"] + ";");

                                log.Info(DateTime.Now + ": Updated " + cri.Table.Table_Name + ".Terminal_ID (" + row["Terminal_ID"] + ")  to local database.");
                            }
                            else if (cri.Table.Table_Name == "POS_Shift")
                            {
                                // do check pos terminal if 
                                var posshift = db.Database.SqlQuery<int>("select Shift_ID  from POS_Shift where Shift_Local_ID = '" + row["Shift_Local_ID"] + "';").FirstOrDefault();
                                if (posshift <= 0)
                                    continue;
                                if (posshift == NumUtil.ParseInteger(row["Shift_ID"].ToString()))
                                {
                                    sql.AppendLine("update POS_Shift set Is_Uploaded = 1, Is_Latest = 1 where [Shift_ID]  = '" + row["Shift_ID"] + "';");
                                    continue;
                                }

                                // check dupid
                                var dupID = db.Database.SqlQuery<string>("select [Shift_Local_ID]  from POS_Shift where Shift_ID = '" + row["Shift_ID"] + "';").FirstOrDefault();
                                if (!string.IsNullOrEmpty(dupID))
                                {
                                    // move to new row
                                    var pks = new List<DBColumn>();
                                    pks.Add(cri.Table.Cols.Where(w => w.colname == "Shift_ID").FirstOrDefault());
                                    sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(")  ");
                                    sql.AppendLine("select ");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(" from [" + cri.Table.Table_Name + "] ");
                                    sql.AppendLine(" where [POS_Shift] = '" + row["Shift_ID"] + "';");

                                    sql.AppendLine("update POS_Receipt set Shift_ID = (select Shift_ID from POS_Shift where Shift_Local_ID ='" + dupID + "' and Shift_ID <> " + row["Shift_ID"] + ") where Shift_Local_ID  = '" + dupID + "';");
                                    sql.AppendLine("delete from POS_Shift where  [Shift_ID] = " + row["Shift_ID"] + ";");
                                }

                                sql.Append(GenerateInsertString(cri, row));
                                sql.AppendLine("update POS_Receipt set Shift_ID = " + row["Shift_ID"] + " where Shift_Local_ID  = '" + row["Shift_Local_ID"] + "';");
                                sql.AppendLine("delete from POS_Shift where Shift_Local_ID = '" + row["Shift_Local_ID"] + "' and Shift_ID <> " + row["Shift_ID"] + ";");

                                log.Info(DateTime.Now + ": Updated " + cri.Table.Table_Name + ".Shift_ID (" + row["Shift_ID"] + ")  to local database.");
                            }
                            else if (cri.Table.Table_Name == "POS_Receipt")
                            {
                                var rcp = db.Database.SqlQuery<int>("select Receipt_ID  from POS_Receipt where Receipt_Local_ID = '" + row["Receipt_Local_ID"] + "';").FirstOrDefault();
                                if (rcp <= 0)
                                    continue;

                                if (rcp == NumUtil.ParseInteger(row["Receipt_ID"].ToString()))
                                {
                                    sql.AppendLine("update POS_Receipt set Is_Uploaded = 1, Is_Latest = 1, Receipt_No ='" + row["Receipt_No"] + "' where [Receipt_ID]  = '" + row["Receipt_ID"] + "';");
                                    continue;
                                }

                                // check dupid
                                var dupID = db.Database.SqlQuery<string>("select [Receipt_Local_ID]  from POS_Receipt where Receipt_ID = '" + row["Receipt_ID"] + "';").FirstOrDefault();
                                if (!string.IsNullOrEmpty(dupID))
                                {
                                    // move to new row
                                    var pks = new List<DBColumn>();
                                    pks.Add(cri.Table.Cols.Where(w => w.colname == "Receipt_ID").FirstOrDefault());
                                    sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(")  ");
                                    sql.AppendLine("select ");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(" from [" + cri.Table.Table_Name + "] ");
                                    sql.AppendLine(" where [Receipt_ID] = '" + row["Receipt_ID"] + "';");

                                    sql.AppendLine("update POS_Receipt_Payment set Receipt_ID = (select Receipt_ID from POS_Receipt where Receipt_Local_ID ='" + dupID + "' and Receipt_ID <> " + row["Receipt_ID"] + ") where Receipt_Local_ID  = '" + dupID + "';");
                                    sql.AppendLine("update POS_Products_Rcp set Receipt_ID = (select Receipt_ID from POS_Receipt where  Receipt_Local_ID ='" + dupID + "' and Receipt_ID <> " + row["Receipt_ID"] + ") where Receipt_Local_ID  = '" + dupID + "';");
                                    sql.AppendLine("update Inventory_Transaction set Receipt_ID = (select Receipt_ID from POS_Receipt where  Receipt_Local_ID ='" + dupID + "' and Receipt_ID <> " + row["Receipt_ID"] + ") where Receipt_Local_ID  = '" + dupID + "';");

                                    sql.AppendLine("delete from POS_Receipt where  [Receipt_ID] = " + row["Receipt_ID"] + ";");
                                }

                                sql.Append(GenerateInsertString(cri, row));
                                sql.AppendLine("update POS_Receipt_Payment set Receipt_ID = " + row["Receipt_ID"] + " where Receipt_Local_ID  = '" + row["Receipt_Local_ID"] + "';");
                                sql.AppendLine("update POS_Products_Rcp set Receipt_ID = " + row["Receipt_ID"] + " where Receipt_Local_ID  = '" + row["Receipt_Local_ID"] + "';");
                                sql.AppendLine("update Inventory_Transaction set Receipt_ID = " + row["Receipt_ID"] + " where Receipt_Local_ID  = '" + row["Receipt_Local_ID"] + "';");
                                sql.AppendLine("delete from POS_Receipt where Receipt_Local_ID = '" + row["Receipt_Local_ID"] + "' and Receipt_ID <> " + row["Receipt_ID"] + ";");

                                log.Info(DateTime.Now + ": Updated " + cri.Table.Table_Name + ".Receipt_ID (" + row["Receipt_ID"] + ")  to local database.");
                            }
                            else if (cri.Table.Table_Name == "POS_Receipt_Payment")
                            {
                                var rcpID = db.Database.SqlQuery<int>("select Receipt_ID  from POS_Receipt where Receipt_Local_ID = '" + row["Receipt_Local_ID"] + "';").FirstOrDefault();
                                if (rcpID <= 0)
                                    continue;
                                var paymentID = db.Database.SqlQuery<int>("select Receipt_Payment_ID  from POS_Receipt_Payment where Receipt_Payment_Local_ID = '" + row["Receipt_Payment_Local_ID"] + "';").FirstOrDefault();
                                if (paymentID <= 0)
                                    continue;
                                if (paymentID == NumUtil.ParseInteger(row["Receipt_Payment_ID"].ToString()))
                                {
                                    sql.AppendLine("update POS_Receipt_Payment set Is_Uploaded = 1, Is_Latest = 1, Receipt_ID = " + rcpID + " where [Receipt_Payment_ID]  = '" + row["Receipt_Payment_ID"] + "';");
                                    continue;
                                }

                                row["Receipt_ID"] = rcpID;

                                // check dupid
                                var dupID = db.Database.SqlQuery<string>("select [Receipt_Payment_Local_ID]  from POS_Receipt_Payment where Receipt_Payment_ID = '" + row["Receipt_Payment_ID"] + "';").FirstOrDefault();
                                if (!string.IsNullOrEmpty(dupID))
                                {
                                    // move to new row
                                    var pks = new List<DBColumn>();
                                    pks.Add(cri.Table.Cols.Where(w => w.colname == "Receipt_Payment_ID").FirstOrDefault());
                                    sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(")  ");
                                    sql.AppendLine("select ");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(" from [" + cri.Table.Table_Name + "] ");
                                    sql.AppendLine(" where [Receipt_Payment_ID] = '" + row["Receipt_Payment_ID"] + "';");

                                    sql.AppendLine("delete from POS_Receipt_Payment where  [Receipt_Payment_ID] = " + row["Receipt_Payment_ID"] + ";");
                                }

                                sql.Append(GenerateInsertString(cri, row));
                                sql.AppendLine("delete from POS_Receipt_Payment where Receipt_Payment_ID = '" + paymentID + "' and Receipt_Payment_ID <> " + row["Receipt_Payment_ID"] + ";");

                                log.Info(DateTime.Now + ": Updated " + cri.Table.Table_Name + ".Receipt_Payment_ID (" + row["Receipt_Payment_ID"] + ")  to local database.");
                            }
                            else if (cri.Table.Table_Name == "POS_Products_Rcp")
                            {
                                var rcpID = db.Database.SqlQuery<int>("select Receipt_ID  from POS_Receipt where Receipt_Local_ID = '" + row["Receipt_Local_ID"] + "';").FirstOrDefault();
                                if (rcpID <= 0)
                                    continue;
                                var productID = db.Database.SqlQuery<int>("select [ID]  from POS_Products_Rcp where Receipt_Product_Local_ID = '" + row["Receipt_Product_Local_ID"] + "';").FirstOrDefault();
                                if (productID <= 0)
                                    continue;
                                if (productID == NumUtil.ParseInteger(row["ID"].ToString()))
                                {
                                    sql.AppendLine("update POS_Products_Rcp set Is_Uploaded = 1, Is_Latest = 1, Receipt_ID = " + rcpID + " where [ID]  = '" + row["ID"] + "';");
                                    continue;
                                }

                                row["Receipt_ID"] = rcpID;


                                // check dupid
                                var dupID = db.Database.SqlQuery<int>("select [ID]  from POS_Products_Rcp where ID = '" + row["ID"] + "';").FirstOrDefault();
                                if (dupID > 0)
                                {
                                    // move to new row
                                    var pks = new List<DBColumn>();
                                    pks.Add(cri.Table.Cols.Where(w => w.colname == "ID").FirstOrDefault());
                                    sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(")  ");
                                    sql.AppendLine("select ");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(" from [" + cri.Table.Table_Name + "] ");
                                    sql.AppendLine(" where [ID] = '" + row["ID"] + "';");
                                    sql.AppendLine("delete from POS_Products_Rcp where  [ID] = " + row["ID"] + ";");
                                }
                                sql.Append(GenerateInsertString(cri, row));
                                sql.AppendLine("delete from POS_Products_Rcp where [ID] = '" + productID + "' and [ID] <> " + row["ID"] + ";");

                                log.Info(DateTime.Now + ": Updated " + cri.Table.Table_Name + ".ID (" + row["ID"] + ")  to local database.");
                            }
                            else if (cri.Table.Table_Name == "Inventory_Transaction")
                            {
                                var rcpID = db.Database.SqlQuery<int>("select Receipt_ID  from POS_Receipt where Receipt_Local_ID = '" + row["Receipt_Local_ID"] + "';").FirstOrDefault();
                                if (rcpID <= 0)
                                    continue;
                                var tranID = db.Database.SqlQuery<int>("select [Transaction_ID]  from Inventory_Transaction where Transaction_Local_ID = '" + row["Transaction_Local_ID"] + "';").FirstOrDefault();
                                if (tranID <= 0)
                                    continue;
                                if (tranID == NumUtil.ParseInteger(row["Transaction_ID"].ToString()))
                                {
                                    sql.AppendLine("update Inventory_Transaction set Is_Uploaded = 1, Is_Latest = 1, Receipt_ID = " + rcpID + " where [Transaction_ID]  = '" + row["Transaction_ID"] + "';");
                                    continue;
                                }
                                row["Receipt_ID"] = rcpID;

                                // check dupid
                                var dupID = db.Database.SqlQuery<int>("select [Transaction_ID]  from Inventory_Transaction where Transaction_ID = '" + row["Transaction_ID"] + "';").FirstOrDefault();
                                if (dupID > 0)
                                {
                                    // move to new row
                                    var pks = new List<DBColumn>();
                                    pks.Add(cri.Table.Cols.Where(w => w.colname == "Transaction_ID").FirstOrDefault());
                                    sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(")  ");
                                    sql.AppendLine("select ");
                                    sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table, pks, false));
                                    sql.AppendLine(" from [" + cri.Table.Table_Name + "] ");
                                    sql.AppendLine(" where [Transaction_ID] = '" + row["Transaction_ID"] + "';");
                                    sql.AppendLine("delete from Inventory_Transaction where  [Transaction_ID] = " + row["Transaction_ID"] + ";");
                                }


                                sql.Append(GenerateInsertString(cri, row));
                                sql.AppendLine("delete from Inventory_Transaction where Transaction_ID = '" + tranID + "' and [Transaction_ID] <> " + row["Transaction_ID"] + ";");

                                log.Info(DateTime.Now + ": Updated " + cri.Table.Table_Name + ".Transaction_ID (" + row["Transaction_ID"] + ")  to local database.");
                            }
                        }

                        var sqlstr = sql.ToString();
                        if (!string.IsNullOrEmpty(sqlstr))
                        {
                            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql.ToString());
                            db.SaveChanges();
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(DateTime.Now, ex);
                    return false;
                }
            }
            return false;
        }

        #endregion

        #region Other
        private string GenerateInsertString(MigrateCriteria cri, DataRow row)
        {
            var mgOffline = new ManageOffline(cri.Company_ID);
            var sql = new StringBuilder();
            sql.AppendLine(" SET IDENTITY_INSERT [" + cri.Table.Table_Name + "] ON;");
            sql.AppendLine(" insert into [" + cri.Table.Table_Name + "] (");
            sql.AppendLine(mgOffline.GenerateAllColumnString(cri.Table));
            sql.AppendLine(") values (");

            var j = 0;
            var colDataStr = new StringBuilder();
            foreach (DataColumn col in cri.Data.Columns)
            {
                colDataStr.Append(mgOffline.ConvertDBValue(row[col.ColumnName]));

                if (j < cri.Data.Columns.Count - 1)
                    colDataStr.Append(", ");

                j++;
            }
            sql.AppendLine(colDataStr.ToString());
            sql.AppendLine(");");
            sql.AppendLine(" SET IDENTITY_INSERT [" + cri.Table.Table_Name + "] OFF;");

            return sql.ToString();

        }

        #endregion

    }
}
