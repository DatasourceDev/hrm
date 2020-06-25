using SBSModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SBSModel.Models
{
    public class PurchaseOrderService
    {

        public List<Purchase_Order> getPurchaseOrders(int Company_ID)
        {
            List<Purchase_Order> po = new List<Purchase_Order>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    po = db.Purchase_Order
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.PO_Payment)
                        .Include(i => i.Purchase_Order_Approval)
                        .Include(i => i.Currency)
                        .Include(i => i.Vendor)
                        .Include(i => i.Purchase_Order_has_Vendor_Product)
                        .Include(i => i.User_Profile.User_Authentication)
                        .Where(w => w.Company_ID == Company_ID)
                        .OrderByDescending(o => o.Order_Date)
                        .ToList();
                }
            }
            catch
            {
            }
            return po;
        }

        public List<Purchase_Order> getPurchaseOrdersByProduct(int Product_ID)
        {
            List<Purchase_Order> po = new List<Purchase_Order>();
            try   
            {
                using (var db = new SBS2DBContext())
                {
                    po = db.Purchase_Order
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.PO_Payment)
                        .Include(i => i.Purchase_Order_Approval)
                        .Include(i => i.Currency)
                        .Include(i => i.Vendor)
                        .Include(i => i.Purchase_Order_has_Vendor_Product)
                        .Include(i => i.User_Profile.User_Authentication)
                        .Where(w => w.Purchase_Order_has_Vendor_Product.Where(ww => ww.Product_ID == Product_ID).FirstOrDefault() != null)
                        .ToList();
                }
            }
            catch
            {
            }
            return po;
        }

        public Purchase_Order getPurchaseOrder(int Purchase_Order_ID)
        {
            Purchase_Order po = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    po = db.Purchase_Order
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.PO_Payment)
                        .Include(i => i.Purchase_Order_Approval)
                        .Include(i => i.Currency)
                        .Include(i => i.Vendor)
                        .Include(i => i.Purchase_Order_has_Vendor_Product)
                        .Include(i => i.User_Profile.User_Authentication)
                        .Include(i => i.Returns)
                        .Include(i => i.Receives)
                        .Where(w => w.Purchase_Order_ID == Purchase_Order_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return po;
        }

        public Purchase_Order getPurchaseOrder(string Order_No)
        {
            Purchase_Order po = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    po = db.Purchase_Order
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.PO_Payment)
                        .Include(i => i.Purchase_Order_Approval)
                        .Include(i => i.Currency)
                        .Include(i => i.Vendor)
                        .Include(i => i.Purchase_Order_has_Vendor_Product)
                        .Include(i => i.User_Profile.User_Authentication)
                        .Include(i => i.Returns)
                        .Include(i => i.Receives)
                        .Where(w => w.Order_No == Order_No).FirstOrDefault();
                }
            }
            catch
            {
            }
            return po;
        }

        public Purchase_Order getPurchaseOrder(string Order_No, int Company_ID)
        {
            Purchase_Order po = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    po = db.Purchase_Order
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.PO_Payment)
                        .Include(i => i.Purchase_Order_Approval)
                        .Include(i => i.Currency)
                        .Include(i => i.Vendor)
                        .Include(i => i.Purchase_Order_has_Vendor_Product)
                        .Include(i => i.User_Profile.User_Authentication)
                        .Where(w => w.Order_No == Order_No & w.Company_ID == Company_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return po;
        }


        public int insertPurchaseOrder(Purchase_Order po)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(po).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updatePurchaseOrder(Purchase_Order po)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(po).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deletePurchaseOrder(Purchase_Order po)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    if (po.Purchase_Order_has_Vendor_Product != null && po.Purchase_Order_has_Vendor_Product.Count > 0)
                    {
                        foreach (Purchase_Order_has_Vendor_Product p in po.Purchase_Order_has_Vendor_Product)
                        {
                            deletePurchaseVendorProduct(p);
                        }
                    }

                    if (po.PO_Payment != null && po.PO_Payment.Count > 0)
                    {
                        foreach (PO_Payment p in po.PO_Payment)
                        {
                            deletePOPayment(p);
                        }
                    }

                    db.Entry(po).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public List<Product> getProducts(int Company_ID)
        {
            List<Product> product = new List<Product>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    product = db.Products.Where(w => w.Company_ID == Company_ID)
                        .Include(i => i.Product_Category)
                        .OrderBy(o => o.Product_Name)
                        .ToList();
                }
            }
            catch
            {
            }
            return product;
        }

        public Product getProduct(int Company_ID, int Product_ID)
        {
            Product product = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    product = db.Products.Where(w => w.Company_ID == Company_ID).Where(w => w.Product_ID == Product_ID)
                        .Include(i => i.Product_Category).FirstOrDefault();
                }
            }
            catch
            {
            }
            return product;
        }

        public Purchase_Order_has_Vendor_Product getPurchaseVendorProduct(int Purchase_Order_has_Vendor_Product_ID)
        {
            Purchase_Order_has_Vendor_Product p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Purchase_Order_has_Vendor_Product
                        .Include(i => i.Product)
                        .Where(w => w.Purchase_Order_has_Vendor_Product_ID == Purchase_Order_has_Vendor_Product_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertPurchaseVendorProduct(Purchase_Order_has_Vendor_Product p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updatePurchaseVendorProduct(Purchase_Order_has_Vendor_Product p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deletePurchaseVendorProduct(Purchase_Order_has_Vendor_Product p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public PO_Payment getPOPayment(int Payment_ID)
        {
            PO_Payment p = new PO_Payment();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.PO_Payment.Where(w => w.Payment_ID == Payment_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertPOPayment(PO_Payment p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updatePOPayment(PO_Payment p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deletePOPayment(PO_Payment p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }


        public Purchase_Order_Approval getPurchaseOrderApproval(int Purchase_Order_Approval_ID)
        {
            Purchase_Order_Approval p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Purchase_Order_Approval.Where(w => w.Purchase_Order_Approval_ID == Purchase_Order_Approval_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertPurchaseOrderApproval(Purchase_Order_Approval p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updatePurchaseOrderApproval(Purchase_Order_Approval p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deletePurchaseOrderApproval(Purchase_Order_Approval p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public Return getReturn(int Purchase_Order_ID)
        {
            Return p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Returns.Include(i => i.Inventory_Transaction).Where(w => w.Purchase_Order_ID == Purchase_Order_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertReturn(Return p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateReturn(Return p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteReturn(Return p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public Inventory_Transaction getInventoryTransaction(int Transaction_ID)
        {
            Inventory_Transaction p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Inventory_Transaction.Include(i => i.Product).Where(w => w.Transaction_ID == Transaction_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertInventoryTransaction(Inventory_Transaction p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateInventoryTransaction(Inventory_Transaction p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteInventoryTransaction(Inventory_Transaction p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public Receive getReceive(string Receive_No, int Company_ID)
        {
            Receive p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Receives.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Purchase_Order)
                        .Include(i => i.Purchase_Order.Purchase_Order_has_Vendor_Product)
                        .Where(w => w.Receive_No == Receive_No & w.Company_ID == Company_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public Receive getReceive(int Receive_ID)
        {
            Receive p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Receives.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Purchase_Order)
                        .Include(i => i.Purchase_Order.Purchase_Order_has_Vendor_Product)
                        .Where(w => w.Receive_ID == Receive_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public List<Receive> getReceives(int Purchase_Order_ID)
        {
            List<Receive> p = new List<Receive>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Receives.Include(i => i.Inventory_Transaction).Include(i => i.Purchase_Order).Where(w => w.Purchase_Order_ID == Purchase_Order_ID)
                        .OrderByDescending(o => o.Receive_Date).ToList();
                }
            }
            catch
            {
            }
            return p;
        }

        public List<Receive> getReceivesInCompany(int Company_ID)
        {
            List<Receive> p = new List<Receive>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Receives.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Purchase_Order)
                        .Include(i => i.Purchase_Order.Purchase_Order_has_Vendor_Product)
                        .Include(i => i.User_Profile)
                        .Where(w => w.Company_ID == Company_ID)
                        .OrderByDescending(o => o.Receive_Date).ToList();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertReceive(Receive p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateReceive(Receive p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteReceive(Receive p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    if (p.Inventory_Transaction != null && p.Inventory_Transaction.Count > 0)
                    {
                        foreach (Inventory_Transaction t in p.Inventory_Transaction.ToList())
                        {
                            db.Entry(t).State = EntityState.Deleted;
                        }
                    }
                    //db.Inventory_Transaction.RemoveRange(p.Inventory_Transaction);
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }


        public Withdraw getWithdraw(string Withdraw_No, int Company_ID)
        {
            Withdraw p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Withdraws.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Sale_Order)
                        .Include(i => i.Sale_Order.Sale_Order_has_Product)
                        .Include(i => i.User_Profile)
                        .Where(w => w.Company_ID == Company_ID)
                        .Where(w => w.Withdraw_No == Withdraw_No).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public Withdraw getWithdraw(int Withdraw_ID)
        {
            Withdraw p = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Withdraws.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Sale_Order)
                        .Include(i => i.Sale_Order.Sale_Order_has_Product)
                        .Include(i => i.User_Profile)
                        .Where(w => w.Withdraw_ID == Withdraw_ID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return p;
        }

        public List<Withdraw> getWithdraws(int Sale_Order_ID)
        {
            List<Withdraw> p = new List<Withdraw>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Withdraws.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Sale_Order)
                        .Include(i => i.Sale_Order.Sale_Order_has_Product)
                        .Include(i => i.User_Profile).Where(w => w.Sale_Order_ID == Sale_Order_ID)
                        .OrderByDescending(o => o.Withdraw_Date).ToList();
                }
            }
            catch
            {
            }
            return p;
        }

        public List<Withdraw> getWithdrawsInCompany(int Company_ID)
        {
            List<Withdraw> p = new List<Withdraw>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    p = db.Withdraws.Include(i => i.Inventory_Transaction)
                        .Include(i => i.Sale_Order)
                        .Include(i => i.Sale_Order.Sale_Order_has_Product)
                        .Include(i => i.User_Profile)
                        .Where(w => w.Company_ID == Company_ID)
                        .OrderByDescending(o => o.Withdraw_Date).ToList();
                }
            }
            catch
            {
            }
            return p;
        }

        public int insertWithdraw(Withdraw p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateWithdraw(Withdraw p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteWithdraw(Withdraw p)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    if (p.Inventory_Transaction != null && p.Inventory_Transaction.Count > 0)
                    {
                        foreach (Inventory_Transaction t in p.Inventory_Transaction.ToList())
                        {
                            db.Entry(t).State = EntityState.Deleted;
                        }
                    }
                    db.Entry(p).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }
    }

    public class Product_Transaction
    {
        public bool chk { get; set; }
        public int Transaction_ID { get; set; }
        public int Product_ID { get; set; }
        public string Product_Name { get; set; }
        public decimal oqty { get; set; }
        public decimal qty { get; set; }
        public string remark { get; set; }
        public int Location_ID { get; set; }
        public double Costing { get; set; }
        public string Serial_No { get; set; }
        public string Batch_No { get; set; }
        public string Expiry_Date { get; set; }
    }

    public class Purchase_Payment
    {
        public int Payment_ID { get; set; }
        public string Payment_Date { get; set; }
        public decimal Amount { get; set; }
        public string Cheque_No { get; set; }
        public Nullable<int> Bank_Payment { get; set; }
    }

   


}