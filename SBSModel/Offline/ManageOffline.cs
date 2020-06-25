using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Offline
{
    public class ManageOffline
    {
        #region Default
        private LocalService lcService = new LocalService();
        private ServerService svService = new ServerService();

        private List<TableShcema> svTable = new List<TableShcema>();

        private Nullable<int> comID;
        private bool newdb = false;

        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ManageOffline(Nullable<int> pComID)
        {
            comID = pComID;

        }
        #endregion

        #region Transaction
        public List<TableShcema> DoOffline(Nullable<bool> offlineManual = null)
        {
            log4net.Config.XmlConfigurator.Configure();
            log = log4net.LogManager.GetLogger(typeof(ManageOffline));

            if (!InternetUtil.CheckForServerConnection())
            {
                log.Info(DateTime.Now + ": cannot connect server.");
                return null;
            }
            log.Info("========================================================================");
            log.Info(DateTime.Now + ": start do offline");
            newdb = false;
            log.Info(DateTime.Now + ": start manage database schema.");
            if (!lcService.DatabaseIsExist("SBS2DB"))
            {
                newdb = true;
                svTable = svService.LstTableShcema();
                var result = lcService.CreateNewTable(svTable);
                if (result == false)
                    return null;
            }
            else
            {
                svTable = svService.LstTableShcema(MigrateTable.Tables);
                //svTable = svService.LstTableShcema();
                //lcService.DeleteTable(svTable);
                //newdb = true;
            }
            log.Info(DateTime.Now + ": end manage database schema.");
            log.Info(DateTime.Now + ": start send data to server.");
            var sendresult = SendDataToServer();
            log.Info(DateTime.Now + ": end send data to server.");
            if (sendresult == true)
            {
                log.Info(DateTime.Now + ": start migrate data to local database.");
                MigrateDatatoLocal();
                log.Info(DateTime.Now + ": end migrate data to local database.");
            }
            log.Info(DateTime.Now + ": end do offline.");
            log.Info("========================================================================");
            return svTable;
        }

        private bool MigrateDatatoLocal()
        {
            bool result = false;
            foreach (var tb in svTable)
            {

                var tbName = tb.Table_Name;
                //if (tbName == "User_Authentication")
                //    log.Info("User_Authentication");
                //if (tbName == "User_Profile")
                //      log.Info("User_Profile");

                if (MigrateTable.Tables.Contains(tbName) && !MigrateTable.ProductMasterTable.Contains(tbName) && tbName != "Global_Lookup_Data" && tbName != "Tag")
                {
                    var scri = new GetScriptCriteria() { Table = tb, Company_ID = comID, Where_Update_On = true, Update_On = lcService.GetUpdateOn(tbName) };
                    var sqlQuery = GetScript(scri);
                    var data = svService.LstData(sqlQuery);
                    if (data != null && data.Rows.Count > 0)
                    {
                        log.Info(DateTime.Now + ":Migrated Table " + tbName + " row count " + data.Rows.Count);
                        var mcri = new MigrateCriteria()
                        {
                            Table = tb,
                            Data = data,
                            Is_NewDB = newdb,
                            Company_ID = comID
                        };

                        result = lcService.MigrateDataToLocal(mcri);
                        if (result == false)
                            break;


                        if (tbName == "Product")
                        {
                            var tcri = new MigrateCriteria()
                            {
                                Table = svTable.Where(w => w.Table_Name == "Tag").FirstOrDefault(),
                                Data = svService.LstData(GetScript(new GetScriptCriteria() { Table = svTable.Where(w => w.Table_Name == "Tag").FirstOrDefault(), Company_ID = comID, Update_On = lcService.GetUpdateOn("Tag") })),
                                Is_NewDB = newdb,
                                Company_ID = comID
                            };

                            result = lcService.MigrateDataToLocal(tcri);
                            if (result == false)
                                break;

                            foreach (DataRow prow in mcri.Data.Rows)
                            {
                                var productID = NumUtil.ParseInteger(prow["Product_ID"].ToString());

                                var acri = CreateProductMigrateCriteria("Product_Attribute", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(acri);
                                if (result == false)
                                    break;

                                foreach (DataRow attrrow in acri.Data.Rows)
                                {
                                    var attrID = NumUtil.ParseInteger(attrrow["Attribute_ID"].ToString());
                                    var vcri = CreateProductMigrateCriteria("Product_Attribute_Value", "Attribute_ID", attrID);
                                    result = lcService.MigrateDataToLocal(vcri);
                                    if (result == false)
                                        break;

                                    var mapscri = new GetScriptCriteria()
                                    {
                                        Table = svTable.Where(w => w.Table_Name == "Product_Attribute_Map").FirstOrDefault(),
                                        Company_ID = comID,
                                        Where_Update_On = true,
                                        Update_On = lcService.GetUpdateOn("Product_Attribute_Map"),
                                        Key = "Attr1",
                                        Key2 = "Attr2",
                                        Key3 = "Attr3",
                                        Key4 = "Attr4",
                                        Key5 = "Attr5",
                                        Key_ID = attrID
                                    };

                                    var mapcri = new MigrateCriteria()
                                    {
                                        Table = svTable.Where(w => w.Table_Name == "Product_Attribute_Map").FirstOrDefault(),
                                        Data = svService.LstData(GetScript(mapscri)),
                                        Have_Del_Data = true,
                                        Del_Key = "Attr1",
                                        Del_Key2 = "Attr2",
                                        Del_Key3 = "Attr3",
                                        Del_Key4 = "Attr4",
                                        Del_Key5 = "Attr5",
                                        Del_Value = attrID,
                                        Is_NewDB = newdb,
                                        Company_ID = comID
                                    };

                                    result = lcService.MigrateDataToLocal(mapcri);
                                    if (result == false)
                                        break;
                                }

                                var tagcri = CreateProductMigrateCriteria("Product_Tag", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(tagcri);
                                if (result == false)
                                    break;

                                var pricecri = CreateProductMigrateCriteria("Product_Price", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(pricecri);
                                if (result == false)
                                    break;


                                foreach (DataRow pricerow in pricecri.Data.Rows)
                                {
                                    var priceID = NumUtil.ParseInteger(pricerow["Product_Price_ID"].ToString());

                                    var mpricecri = CreateProductMigrateCriteria("Product_Attribute_Map_Price", "Product_Price_ID", priceID);
                                    result = lcService.MigrateDataToLocal(mpricecri);
                                    if (result == false)
                                        break;
                                }

                                var imgcri = CreateProductMigrateCriteria("Product_Image", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(imgcri);
                                if (result == false)
                                    break;

                                var kcri = CreateProductMigrateCriteria("Kit", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(kcri);
                                if (result == false)
                                    break;

                                var bcri = CreateProductMigrateCriteria("Bom", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(bcri);
                                if (result == false)
                                    break;

                                var ucri = CreateProductMigrateCriteria("Unit_Of_Measurement", "Product_ID", productID);
                                result = lcService.MigrateDataToLocal(ucri);
                                if (result == false)
                                    break;
                            }

                        }
                        else if (tbName == "Global_Lookup_Def")
                        {
                            foreach (DataRow drow in mcri.Data.Rows)
                            {
                                var defID = NumUtil.ParseInteger(drow["Def_ID"].ToString());

                                scri = new GetScriptCriteria()
                                {
                                    Table = svTable.Where(w => w.Table_Name == "Global_Lookup_Data").FirstOrDefault(),
                                    Company_ID = comID,
                                    Key = "Def_ID",
                                    Key_ID = defID,
                                    Update_On = lcService.GetUpdateOn("Global_Lookup_Data"),
                                };

                                mcri = new MigrateCriteria()
                                {
                                    Table = svTable.Where(w => w.Table_Name == "Global_Lookup_Data").FirstOrDefault(),
                                    Data = svService.LstData(GetScript(scri)),
                                    Have_Del_Data = true,
                                    Del_Key = "Def_ID",
                                    Del_Value = defID,
                                    Is_NewDB = newdb,
                                    Company_ID = comID
                                };
                                result = lcService.MigrateDataToLocal(mcri);
                                if (result == false)
                                    break;
                            }

                        }
                    }


                }
            }


            if (result)
                log.Info("Completed");
            else
                log.Info("Error");

            return result;
        }

        public bool SendDataToServer()
        {
            if (!InternetUtil.CheckForServerConnection())
            {
                log.Info(DateTime.Now + ": cannot connect server.");
                return false;
            }
            if (svTable == null || svTable.Count == 0)
            {
                var svService = new ServerService();
                svTable = svService.LstTableShcema(MigrateTable.TransactionTable);
            }

            var result = SendDataToServer("POS_Terminal");
            if (result == false)
                return false;

            result = SendDataToServer("POS_Shift");
            if (result == false)
                return false;

            result = SendDataToServer("POS_Receipt");
            if (result == false)
                return false;

            result = SendDataToServer("POS_Receipt_Payment");
            if (result == false)
                return false;

            result = SendDataToServer("POS_Products_Rcp");
            if (result == false)
                return false;

            result = SendDataToServer("Inventory_Transaction");
            if (result == false)
                return false;

            return true;
        }

        private bool SendDataToServer(string pTableName, DataTable data = null)
        {
            var table = svTable.Where(w => w.Table_Name == pTableName).FirstOrDefault();
            var scri = new GetScriptCriteria() { Table = table, Company_ID = comID, Where_Is_Uploaded = true, Where_Is_Latest = true, Where_Do_Not_Upload = true };
            var sqlQuery = GetScript(scri);
            data = lcService.LstData(sqlQuery);

            if (data != null && data.Rows.Count > 0)
            {
                // send data to server
                var cri = new MigrateCriteria()
                {
                    Table = table,
                    Data = data,
                    Company_ID = comID
                };

                log.Info(DateTime.Now + ": Start send " + pTableName + " to server.");
                var result = svService.SendDatatoServer(cri);
                if (result == false)
                {
                    log.Error(DateTime.Now + ": cannot send " + pTableName + " to server.");
                    return false;
                }
                log.Info(DateTime.Now + ": sent " + pTableName + " to server.");

                log.Info(DateTime.Now + ": Start update " + pTableName + " to local database.");
                result = lcService.UpdateLocal(cri);
                if (result == false)
                {
                    log.Error(DateTime.Now + ": cannot update " + pTableName + " to local database.");
                    return false;
                }
                log.Info(DateTime.Now + ": updated " + pTableName + " to server.");
            }

            return true;

        }
        #endregion

        #region Other
        private MigrateCriteria CreateProductMigrateCriteria(string tablename, string key, Nullable<int> keyID)
        {
            var scri = new GetScriptCriteria()
            {
                Table = svTable.Where(w => w.Table_Name == tablename).FirstOrDefault(),
                Company_ID = comID,
                Where_Update_On = true,
                Key = key,
                Key_ID = keyID,
                Update_On = lcService.GetUpdateOn(tablename),
            };

            var mcri = new MigrateCriteria()
            {
                Table = svTable.Where(w => w.Table_Name == tablename).FirstOrDefault(),
                Data = svService.LstData(GetScript(scri)),
                Have_Del_Data = true,
                Del_Key = key,
                Del_Value = keyID,
                Is_NewDB = newdb,
                Company_ID = comID
            };

            return mcri;
        }

        private string GetScript(GetScriptCriteria cri, bool firstlvl = true)
        {
            Nullable<DateTime> updateon = null;
            if (cri.Update_On.HasValue)
                updateon = cri.Update_On;

            var sqlQuery = new StringBuilder();
            var whereQuery = new StringBuilder();
            sqlQuery.AppendLine("select ");
            var i = 0;
            var havecomcol = false;
            var haveupdateon = false;
            var havesysoffline = false;
            var haveisUploaded = false;
            var haveisLatest = false;
            var donotupload = false;

            if (cri.Table.Cols.Where(w => w.colname == "Company_ID").FirstOrDefault() != null)
                havecomcol = true;

            if (cri.Where_Update_On)
            {
                if (cri.Table.Cols.Where(w => w.colname == "Update_On").FirstOrDefault() != null)
                    haveupdateon = true;
            }
            if (cri.Where_Is_Uploaded)
            {
                if (cri.Table.Cols.Where(w => w.colname == "Is_Uploaded").FirstOrDefault() != null)
                    haveisUploaded = true;
            }

            if (cri.Where_Is_Latest)
            {
                if (cri.Table.Cols.Where(w => w.colname == "Is_Latest").FirstOrDefault() != null)
                    haveisLatest = true;
            }

            if (cri.Where_Do_Not_Upload)
            {
                if (cri.Table.Cols.Where(w => w.colname == "Do_Not_Upload").FirstOrDefault() != null)
                    donotupload = true;
            }

            if (cri.Table.Cols.Where(w => w.colname == "Syn_Offline").FirstOrDefault() != null)
                havesysoffline = true;

            var wheredcom = havecomcol;
            foreach (var col in cri.Table.Cols)
            {
                if (cri.getfkscript)
                {
                    if (!string.IsNullOrEmpty(col.key) && col.key.ToLower().Contains("pk_"))
                        sqlQuery.AppendLine("[" + col.colname + "]");
                }
                else
                {
                    sqlQuery.AppendLine("[" + col.colname + "]");

                    if (i < cri.Table.Cols.Count - 1)
                        sqlQuery.Append(", ");
                }
                if (havecomcol == false && (wheredcom == false | firstlvl))
                {
                    if (!string.IsNullOrEmpty(col.fktable) && col.fktable != "Global_Lookup_Data")
                    {
                        var fk = svTable.Where(w => w.Table_Name == col.fktable).FirstOrDefault();
                        if (fk != null)
                        {
                            var scri = new GetScriptCriteria();
                            scri.Table = fk;
                            scri.Company_ID = cri.Company_ID;
                            scri.getfkscript = true;
                            scri.Update_On = lcService.GetUpdateOn(fk.Table_Name);

                            var fkscript = GetScript(scri, false);
                            if (!string.IsNullOrEmpty(fkscript))
                            {
                                whereQuery.AppendLine("and [" + col.colname + "] in (");
                                whereQuery.AppendLine(fkscript);
                                whereQuery.AppendLine(") ");
                                wheredcom = true;
                            }
                        }
                    }

                }
                i++;
            }
            sqlQuery.AppendLine("from [" + cri.Table.Table_Name + "] ");
            sqlQuery.AppendLine("where 1 = 1 ");

            if (haveupdateon && updateon.HasValue)
            {
                var date = updateon.Value;
                var datestr = date.Year + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00") + " " + date.Hour.ToString("00") + ": " + date.Minute.ToString("00") + ": " + date.Second.ToString("00");
                sqlQuery.AppendLine("and convert(varchar(30),Update_On,120 )  > '" + datestr + "'"); //yyyy-mm-dd hh:mi:ss
            }

            if (havesysoffline)
                sqlQuery.AppendLine("and Syn_Offline = 1");

            if (haveisUploaded && haveisLatest)
                sqlQuery.AppendLine("and (Is_Uploaded = 0 or Is_Uploaded is null or Is_Latest = 0 or Is_Latest is null )");
            else if (haveisUploaded)
                sqlQuery.AppendLine("and (Is_Uploaded = 0 or Is_Uploaded is null  )");
            else if (haveisLatest)
                sqlQuery.AppendLine("and (Is_Latest = 0 or Is_Latest is null  )");


            if (donotupload)
                sqlQuery.AppendLine("and (Do_Not_Upload is null or Do_Not_Upload = 0)");

            if (!string.IsNullOrEmpty(cri.Key) && cri.Key_ID.HasValue)
            {
                sqlQuery.AppendLine("and (" + cri.Key + " = " + cri.Key_ID);
                if (!string.IsNullOrEmpty(cri.Key2))
                    sqlQuery.AppendLine("or " + cri.Key2 + " = " + cri.Key_ID);
                if (!string.IsNullOrEmpty(cri.Key3))
                    sqlQuery.AppendLine("or " + cri.Key3 + " = " + cri.Key_ID);
                if (!string.IsNullOrEmpty(cri.Key4))
                    sqlQuery.AppendLine("or " + cri.Key4 + " = " + cri.Key_ID);
                if (!string.IsNullOrEmpty(cri.Key5))
                    sqlQuery.AppendLine("or " + cri.Key5 + " = " + cri.Key_ID);

                sqlQuery.AppendLine(")");
            }

            if (havecomcol)
            {
                if (cri.Table.Table_Name == "Global_Lookup_Data" | cri.Table.Table_Name == "Donation_Formula")
                    sqlQuery.AppendLine("and (Company_ID = " + cri.Company_ID + " or Company_ID is null)");
                else
                    sqlQuery.AppendLine("and Company_ID = " + cri.Company_ID);
            }
            else
            {
                if (cri.getfkscript)
                {
                    if (!string.IsNullOrEmpty(whereQuery.ToString()))
                        sqlQuery.AppendLine(whereQuery.ToString());
                    else
                        sqlQuery = new StringBuilder();
                }
                else
                {
                    if (!string.IsNullOrEmpty(whereQuery.ToString()))
                        sqlQuery.AppendLine(whereQuery.ToString());
                }
            }
            return sqlQuery.ToString();
        }

        public string GenerateAllColumnString(TableShcema table, List<DBColumn> pks = null, bool tolocal = true)
        {
            //return [column1],[column2],[column3] ,...,[columnN]

            if (pks == null)
                pks = new List<DBColumn>();

            var colStr = new StringBuilder();
            var i = 0;
            foreach (var col in table.Cols)
            {
                if (tolocal)
                {
                    if (!string.IsNullOrEmpty(col.key) && col.key.ToLower().Contains("pk_"))
                        pks.Add(col);

                    colStr.Append("[" + col.colname + "]");

                    if (i < table.Cols.Count - 1)
                        colStr.Append(", ");
                }
                else
                {
                    if (!string.IsNullOrEmpty(col.key) && col.key.ToLower().Contains("pk_"))
                    {
                        pks.Add(col);
                    }
                    else
                    {
                        colStr.Append("[" + col.colname + "]");

                        if (i < table.Cols.Count - 1)
                            colStr.Append(", ");
                    }
                }

                i++;
            }
            return colStr.ToString();
        }

        public string ConvertDBValue(object data)
        {
            try
            {
                var datatype = data.GetType().ToString();
                if (datatype == "System.Int32")
                {
                    return data.ToString();
                }
                else if (datatype == "System.Boolean")
                {

                    if (data.ToString().ToLower() == "true")
                        return "1";
                    else
                        return "0";

                }
                else if (datatype == "System.DateTime")
                {
                    if (data is DBNull || string.IsNullOrEmpty(data.ToString()))
                        return "null";
                    else
                    {
                        try
                        {
                            var datetimestr = ((DateTime)data).ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US", false));
                            var datestr = DateUtil.ToInternalDate(datetimestr);
                            return "'" + datestr + "'";
                        }
                        catch (Exception ex)
                        {
                            log.Error(DateTime.Now, ex);
                            return "null";
                        }
                    }
                   
                }
                else if (datatype == "System.Byte[]")
                {
                    var bt = "0x" + BitConverter.ToString((byte[])data).Replace("-", "");
                    return "convert(varbinary(max), " + bt + ")";
                }
                else if (datatype == "System.DBNull")
                {
                    return "null";
                }
                else if (datatype == "System.Decimal")
                {
                    return data.ToString();
                }
                else
                {
                    return "'" + data.ToString().Replace("'", "''") + "'";
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
