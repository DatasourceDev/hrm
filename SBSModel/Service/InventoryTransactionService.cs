using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    public class InventoryTransactionService
    {
        public List<Inventory_Transaction> ListInventoryTransaction(int pCompanyID, string pType, int pID)
        {
            using (var db = new SBS2DBContext())
            {
                var transaction = db.Inventory_Transaction
                    .Include(i => i.Product)
                    .Include(i => i.Withdraw)
                    .Where(w => w.Company_ID == pCompanyID && w.Transaction_Type == pType);

                if (pType == InventoryType.Receive)
                {
                    transaction = transaction.Where(w => w.Receive_ID == pID);
                }
                else if (pType == InventoryType.Withdraw)
                {
                    transaction = transaction.Where(w => w.Withdraw_ID == pID);
                }
                else if (pType == InventoryType.Return)
                {
                    transaction = transaction.Where(w => w.Return_ID == pID);
                }
                else if (pType == InventoryType.SaleOrderReturn)
                {
                    transaction = transaction.Where(w => w.SO_Return_ID == pID);
                }
                return transaction.ToList();
            }
        }

        public List<Inventory_Transaction> ListInventoryWithdrawTransaction(int pCompanyID, int pSaleOrderID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Inventory_Transaction
                    .Include(i => i.Product)
                    .Include(i => i.Withdraw)
                    .Where(w => w.Company_ID == pCompanyID && w.Transaction_Type == InventoryType.Withdraw & w.Withdraw.Sale_Order_ID == pSaleOrderID).ToList();
            }
        }

        public ServiceResult Delivery(int pSaleOrderID, int[] pTransactionID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var withdraws = db.Inventory_Transaction.Where(w => w.Withdraw.Sale_Order_ID == pSaleOrderID & w.Withdraw_ID != null & w.Transaction_Type == InventoryType.Withdraw);
                    foreach (var w in withdraws)
                    {
                        if (pTransactionID != null)
                        {
                            if (pTransactionID.Contains(w.Transaction_ID))
                            {
                                w.Delivery = true;
                            }
                            else
                            {
                                w.Delivery = false;
                            }
                        }
                        else
                        {
                            w.Delivery = false;
                        }
                    }

                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = "Delivery" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_506_SAVE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_506_SAVE_ERROR), Field = "Delivery" };
            }
        }


                
        public ServiceResult InsertTransaction(List<Inventory_Transaction> pTrans)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    foreach (var tran in pTrans)
                    {
                        db.Inventory_Transaction.Add(tran);
                    }
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Transaction" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Transaction" };
            }
        }
    }
  

  
}