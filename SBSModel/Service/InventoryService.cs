using System;
using System.Collections.Generic;
using System.Linq;
using SBSModel.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;

namespace SBSModel.Models
{

    public class TaxCriteria : CriteriaBase
    {
        public int Tax_ID { get; set; }
        public bool Include_Service_Charge { get; set; }
        public Nullable<decimal> Service_Charge_Percen { get; set; }
        public bool Include_Surcharge { get; set; }
        public bool Include_GST { get; set; }

    }


    public class PromotionCriteria : CriteriaBase
    {
        public Nullable<int> Branch_ID { get; set; }
        public string Promotion_Name { get; set; }
        public Nullable<DateTime> Start_Date { get; set; }
        public Nullable<DateTime> End_Date { get; set; }
    }

    public class ProductCriteria : CriteriaBase
    {
        public Nullable<int> Category_ID { get; set; }
        public bool Display_Photo { get; set; }
        public bool Random { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> User_Authentication_ID { get; set; }
        public string Product_Name { get; set; }
        public string Product_Code { get; set; }
        public Nullable<int> Category_1 { get; set; }
        public Nullable<int> Category_2 { get; set; }
        public Nullable<int> Category_3 { get; set; }
        public int[] User_Select { get; set; }
    }

    public class CategoryCriteria : CriteriaBase
    {
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> Category_ID { get; set; }
        public string Category_Name { get; set; }
        public string Category_Parent { get; set; }

        public Nullable<int> Category_LV { get; set; }

    }

    public class InventoryService
    {
        public Product_Image GetProductMainImage(Nullable<int> pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Product_Image.Where(w => w.Product_ID == pProductID & w.Is_Main == true).FirstOrDefault();
            }
        }

        public Product_Table GetProductTable(Nullable<int> pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Product_Table.Where(w => w.Company_ID == pCompanyID).SingleOrDefault();
            }
        }

        public ServiceResult InsertProductTable(Product_Table pTable)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Product_Table.Add(pTable);
                    db.SaveChanges();
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE),
                        Field = "Table"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR),
                    Field = "Table"
                };
            }
        }

        public ServiceResult UpdateProductTable(Product_Table pTable)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current =
                        (from a in db.Product_Table where a.Product_Table_ID == pTable.Product_Table_ID select a)
                            .FirstOrDefault();
                    if (current != null)
                    {
                        db.Entry(current).CurrentValues.SetValues(pTable);
                        db.SaveChanges();
                    }
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT),
                        Field = "Table"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR),
                    Field = "Table"
                };
            }
        }

        public List<Inventory_Location> LstInventoryLocation(Nullable<int> pCompanyID, bool hasBlank = true)
        {
            using (var db = new SBS2DBContext())
            {

                var InventoryLocation = db.Inventory_Location.Where(w => w.Company_ID == pCompanyID).ToList();

                var lst = InventoryLocation.ToList();
                lst.Insert(0,
                      new Inventory_Location()
                      {
                          Inventory_Location_ID = 0,
                          Name = "-",
                          Location_Level = 1
                      });
                return lst;
            }
        }

        //Added by Nay on 16-Dec-2015
        public List<Product_RelatedProduct> LstRelatedProduct(Nullable<int> pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                var pList = db.Product_RelatedProduct.Where(p => p.Based_Product_ID == pProductID || p.Related_Product_ID == pProductID).ToList();
                if (pList.Count > 0)
                    return pList;
                else
                    return null;
            }
        }
        //Added by Nay on 16-Dec-2015
        public List<Product> LstProducts(Nullable<int> CompID, Nullable<int> productID)
        {
            using(var db = new SBS2DBContext())
            {
                var pList = db.Products.Where(w => w.Company_ID == CompID && w.Record_Status == "Active" && w.Product_ID != productID).ToList();
                if (pList.Count > 0)
                    return pList;
                else
                    return null;
            }
        }

        //Added by Nay on 17-Dec-2015 
        public Product_Category GetProductofCategoryL1(Nullable<int> pProductID)
        {
            using(var db = new SBS2DBContext())
            {
                var pCategory = (from p in db.Products
                     join c in db.Product_Category
                     on p.Product_Category_L1 equals c.Product_Category_ID
                     where p.Product_ID == pProductID
                     select c).SingleOrDefault();

                if (pCategory != null)
                    return pCategory;
                else
                    return null;
            }
        }
        //Added by Nay on 16-Dec-2015
        public List<Product> LstProductsNotRelated(Nullable<int> CategoryID, Nullable<int> ProductID, Nullable<int> companyID, int[] pList)
        {
            using(var db = new SBS2DBContext())
            {
                var products = db.Products.Where(w => w.Product_Category_L1 == CategoryID && w.Company_ID == companyID && w.Record_Status == "Active");
                
                if(ProductID != null)
                {
                    products = products.Where(w => w.Product_ID != ProductID);
                }
                if(pList != null)
                {
                    foreach(var p in pList)
                    {
                        products = products.Where(w => w.Product_ID != p);
                    }
                }
                return products.ToList();
            }
        }
        //Added by Nay on 16-Dec-2015
        public bool InsertRelatedProduct(Product_RelatedProduct rProducts)
        {
            bool returnVal = true;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    //var chkRelatedProduct = (from p in db.Product_RelatedProducts 
                    //                             where p.Based_Product_ID == rProducts.Based_Product_ID 
                    //                             && p.Related_Product_ID == rProducts.Related_Product_ID.Value 
                    //                             select p).SingleOrDefault();
                    //if (chkRelatedProduct != null)
                    //{
                    //    //db.Entry(chkRelatedProduct).CurrentValues.SetValues(rProducts);
                    //    db.Product_RelatedProducts.Remove(chkRelatedProduct);
                    //    db.SaveChanges();
                    //}
                    db.Product_RelatedProduct.Add(rProducts);
                    db.SaveChanges();
                    returnVal = true;
                }
            }
            catch
            {
                returnVal = false;
            }
            return returnVal;
        }
        
        //Added by Nay on 17-Dec-2015
        public bool RemoveExistRelatedProduct(Nullable<int> basedProductID)
        {
            bool returnVal = true;
            try
            {
                using(var db = new SBS2DBContext())
                {
                    var removeProducts = (from p in db.Product_RelatedProduct
                                          where p.Based_Product_ID == basedProductID
                                          select p).ToList();

                    if(removeProducts != null)
                    {
                        foreach(var product in removeProducts)
                        {
                            db.Product_RelatedProduct.Remove(product);
                            db.SaveChanges();
                        }                        
                    }
                }
                returnVal = true;
            }
            catch
            {
                returnVal = false;
            }
            return returnVal;
        }
        //---------------------Category----------------------------------------------------------------------
        public List<Product_Category> LstCategory(int pCompany_ID, bool parentOnly = false)
        {
            using (var db = new SBS2DBContext())
            {
                var categories = db.Product_Category.Where(w => w.Company_ID == pCompany_ID);

                if (parentOnly)
                {
                    categories = categories.Where(w => w.Category_Parent_ID == null || w.Category_Parent_ID == 0).OrderBy(c => c.Category_Name);
                }

                var lst = categories.ToList();

                var level = 1;
                if (lst.Count() > 0 && lst[0].Category_Level.HasValue) level = lst[0].Category_Level.Value;
                lst.Insert(0,
                      new Product_Category()
                      {
                          Product_Category_ID = 0,
                          Category_Name = "-",
                          Category_Level = level
                      });
                return lst;
            }
        }

        public List<Product_Category> LstCategory(CategoryCriteria criteria)
        {
            using (var db = new SBS2DBContext())
            {
                var cats = db.Product_Category
                    .Include(i => i.Product_Category2) //Product Category Parent 
                    .Where(w => w.Company_ID == criteria.Company_ID);

                if (!string.IsNullOrEmpty(criteria.Category_Name))
                {
                    cats = cats.Where(w => w.Category_Name.Contains(criteria.Category_Name));

                    if (!string.IsNullOrEmpty(criteria.Category_Parent))
                    {
                        cats = cats.Where(w => w.Product_Category2.Category_Name.Contains(criteria.Category_Parent));
                    }
                }
                if (criteria.Branch_ID.HasValue)
                {

                }
                //Added by sun 13-11-2015
                if (criteria.Category_LV.HasValue && criteria.Category_LV > 0)
                {
                    cats = cats.Where(w => w.Category_Level == criteria.Category_LV.Value);
                }

                if (!string.IsNullOrEmpty(criteria.Record_Status)) 
                {
                    cats = cats.Where(w => w.Record_Status == criteria.Record_Status);
                } else 
                {
                    cats =  cats.Where(w=>w.Record_Status != RecordStatus.Delete);
                }

                if (criteria.Update_On.HasValue)
                {
                    cats = cats.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(criteria.Update_On.Value.Year, criteria.Update_On.Value.Month, criteria.Update_On.Value.Day, criteria.Update_On.Value.Hour, criteria.Update_On.Value.Minute, criteria.Update_On.Value.Second));
                }
                cats = cats.OrderBy(o => o.Category_Name);

                if (criteria.hasBlank)
                {
                    var lst = cats.ToList();
                    lst.Insert(0,
                        new Product_Category()
                        {
                            Category_Name = "All",
                            Company_ID = criteria.Company_ID,
                            Product_Category_ID = 0
                        });
                    return lst;
                }

                return cats.ToList();
            }
        }

        public Product_Category GetCategory(Nullable<int> pCategoryID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Product_Category.Where(w => w.Product_Category_ID == pCategoryID).SingleOrDefault();
            }
        }

        public ServiceResult InsertCategory(Product_Category pCategory)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pCategory.Category_Parent_ID == 0) { pCategory.Category_Parent_ID = null; }

                    db.Product_Category.Add(pCategory);
                    db.SaveChanges();
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE),
                        Field = "Category"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR),
                    Field = "Category"
                };
            }
        }

        public ServiceResult UpdateCategory(Product_Category pCategory)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current =
                        (from a in db.Product_Category
                         where a.Product_Category_ID == pCategory.Product_Category_ID
                         select a).FirstOrDefault();
                    if (current != null)
                    {
                        //Added by sun 12-11-2015
                        pCategory.Create_On = current.Create_On;
                        pCategory.Create_By = current.Create_By;

                        db.Entry(current).CurrentValues.SetValues(pCategory);
                        db.SaveChanges();
                    }
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT),
                        Field = "Category"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR),
                    Field = "Category"
                };
            }
        }

        public ServiceResult DeleteCategory(int pCategoryID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current =
                        (from a in db.Product_Category where a.Product_Category_ID == pCategoryID select a)
                            .FirstOrDefault();
                    if (current != null)
                    {
                        db.Product_Category.Remove(current);
                        db.SaveChanges();
                    }
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE),
                        Field = "Category"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR),
                    Field = "Category"
                };
            }
        }

        private bool productCategoryExists(string categoryName, int company_ID, ref int category_ID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var location =
                        db.Product_Category.SingleOrDefault(
                            x => x.Category_Name.ToLower() == categoryName.ToLower() && x.Company_ID == company_ID);

                    if (location != null)
                    {
                        category_ID = location.Product_Category_ID;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        //---------------------Promotion--------------------------------------//
        public Promotion GetPromotion(int pCompanyID, Nullable<int> pPromotionID)
        {
            Promotion promo = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    promo = db.Promotions
                   .Where(w => w.Company_ID == pCompanyID)
                   .Include(i => i.Promotion_Branch)
                   .Include(i => i.Promotion_Product)
                   .Include(i => i.Promotion_Spacial)
                   .Include(i => i.Promotion_Branch)
                   .Include(i => i.Promotion_Branch.Select(s => s.Branch))
                   .Where(w => w.Promotion_ID == pPromotionID).FirstOrDefault();
                }
            }
            catch
            {
            }
            return promo;
        }

        public ServiceResult InsertPromotion(Promotion pPromotion)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Promotions.Add(pPromotion);
                    db.SaveChanges();
                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Promotion" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Promotion" };
            }
        }

        public ServiceObjectResult LstPromotion(PromotionCriteria criteria)
        {
            var result = new ServiceObjectResult();
            result.Object = new List<Promotion>();
            using (var db = new SBS2DBContext())
            {
                var pro = db.Promotions
                    .Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete)
                    .Include(i => i.Promotion_Product)
                    .Include(i => i.Promotion_Spacial)
                    .Include(i => i.Promotion_Branch)
                    .Include(i => i.Promotion_Branch.Select(s => s.Branch));

                if (criteria.Record_Status != null)
                {
                    pro = pro.Where(w => w.Record_Status == criteria.Record_Status);
                }
                if (criteria.Promotion_Name != null)
                {
                    pro = pro.Where(w => w.Promotion_Name == criteria.Promotion_Name);
                }
                if (criteria.Start_Date.HasValue)
                {
                    // pro = pro.Where(w => w.Start_Date == DateUtil.ToDate(pStartDate)).ToList();
                    pro = pro.Where(w => EntityFunctions.CreateDateTime(w.Start_Date.Value.Year, w.Start_Date.Value.Month, w.Start_Date.Value.Day, null, null, null) ==
                        EntityFunctions.CreateDateTime(criteria.Start_Date.Value.Year, criteria.Start_Date.Value.Month, criteria.Start_Date.Value.Day, null, null, null));
                }
                if (criteria.End_Date.HasValue)
                {
                    // pro = pro.Where(w => w.End_Date == DateUtil.ToDate(pEndDate));
                    pro = pro.Where(w => EntityFunctions.CreateDateTime(w.End_Date.Value.Year, w.End_Date.Value.Month, w.End_Date.Value.Day, 0, 0, 0) ==
                        EntityFunctions.CreateDateTime(criteria.End_Date.Value.Year, criteria.End_Date.Value.Month, criteria.End_Date.Value.Day, 0, 0, 0));
                }
                if (criteria.Branch_ID.HasValue && criteria.Branch_ID > 0)
                {
                    pro = pro.Where(we => we.Promotion_Branch.Where(w => w.Branch_ID == criteria.Branch_ID).FirstOrDefault() != null);

                }
                var obj = new List<Promotion>();
                obj = pro.ToList();
                result.Object = obj;
                return result;
            }
        }

        public ServiceResult UpdatePromotion(Promotion pPromotion)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    List<Promotion_Product> currPromoPro = new List<Promotion_Product>();
                    List<Promotion_Spacial> currPromoSpa = new List<Promotion_Spacial>();
                    var PromoProRemove = new List<Promotion_Product>();
                    var PromoSpaRemove = new List<Promotion_Spacial>();

                    if (pPromotion.Promotion_ID == 0)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = "Promotion" + " not found.", Field = "Promotion" };

                    var currpromotion = db.Promotions.Where(w => w.Promotion_ID == pPromotion.Promotion_ID).FirstOrDefault();
                    if (currpromotion == null)
                        return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = "Promotion" + " not found.", Field = "Promotion" };

                    var PromotionBranch = db.Promotion_Branch.Where(w => w.Promotion_ID == pPromotion.Promotion_ID);
                    db.Promotion_Branch.RemoveRange(PromotionBranch);
                    if (pPromotion.Promotion_Branch.Count > 0)
                    {
                        foreach (var brow in pPromotion.Promotion_Branch)
                        {
                            var pb = new Promotion_Branch()
                            {
                                Branch_ID = brow.Branch_ID,
                                Promotion_ID = brow.Promotion_ID
                            };
                            db.Promotion_Branch.Add(pb);
                        }
                    }

                    currPromoPro = (from a in db.Promotion_Product where a.Promotion_ID == pPromotion.Promotion_ID select a).ToList();
                    foreach (var row in currPromoPro)
                    {
                        if (pPromotion.Promotion_Product == null || !pPromotion.Promotion_Product.Select(s => s.Promotion_Product_ID).Contains(row.Promotion_Product_ID))
                        {
                            PromoProRemove.Add(row);
                            var premove = db.Promotion_Product.Where(w => w.Promotion_Product_ID == row.Promotion_Product_ID);
                            db.Promotion_Product.RemoveRange(premove);
                        }
                    }
                    if (PromoProRemove.Count > 0)
                    {
                        db.Promotion_Product.RemoveRange(PromoProRemove);
                    }
                    if (pPromotion.Promotion_Product.Count != 0)
                    {
                        foreach (var row in pPromotion.Promotion_Product)
                        {
                            if (row.Promotion_Product_ID == 0 || !currPromoPro.Select(s => s.Promotion_Product_ID).Contains(row.Promotion_Product_ID))
                            {
                                db.Promotion_Product.Add(row);
                            }
                            else
                            {
                                var currPro = db.Promotion_Product.Where(w => w.Promotion_Product_ID == row.Promotion_Product_ID).FirstOrDefault();
                                if (currPro != null)
                                {
                                    currPro.Promotion_ID = row.Promotion_ID;
                                    currPro.Product_ID = row.Product_ID;
                                    currPro.Discount_Amount = row.Discount_Amount;
                                    currPro.Discount_Type = row.Discount_Type;
                                    currPro.Currency_ID = row.Currency_ID;
                                    db.Entry(currPro).State = EntityState.Modified;
                                }
                            }
                        }
                    }

                    currPromoSpa = (from a in db.Promotion_Spacial where a.Promotion_ID == pPromotion.Promotion_ID select a).ToList();
                    foreach (var row in currPromoSpa)
                    {
                        if (pPromotion.Promotion_Spacial == null || !pPromotion.Promotion_Spacial.Select(s => s.Promotion_Spacial_ID).Contains(row.Promotion_Spacial_ID))
                        {
                            PromoSpaRemove.Add(row);
                            var sremove = db.Promotion_Spacial.Where(w => w.Promotion_Spacial_ID == row.Promotion_Spacial_ID);
                            db.Promotion_Spacial.RemoveRange(sremove);
                        }
                    }
                    if (PromoSpaRemove.Count > 0)
                    {
                        db.Promotion_Spacial.RemoveRange(PromoSpaRemove);
                    }
                    if (pPromotion.Promotion_Spacial.Count != 0)
                    {
                        foreach (var row in pPromotion.Promotion_Spacial)
                        {
                            if (row.Promotion_Spacial_ID == 0 || !currPromoSpa.Select(s => s.Promotion_Spacial_ID).Contains(row.Promotion_Spacial_ID))
                            {
                                db.Promotion_Spacial.Add(row);
                            }
                            else
                            {
                                var currSpa = db.Promotion_Spacial.Where(w => w.Promotion_Spacial_ID == row.Promotion_Spacial_ID).FirstOrDefault();
                                if (currSpa != null)
                                {
                                    currSpa.Promotion_ID = row.Promotion_ID;
                                    currSpa.Product_ID = row.Product_ID;
                                    currSpa.Purchase_Type = row.Purchase_Type;
                                    currSpa.Min = row.Min;
                                    currSpa.Max = row.Max;
                                    currSpa.Discount = row.Discount;
                                    currSpa.Qty_Discount_Apply = row.Qty_Discount_Apply;
                                    currSpa.Discount_Type = row.Discount_Type;
                                    db.Entry(currSpa).State = EntityState.Modified;
                                }
                            }
                        }
                    }

                    db.Entry(currpromotion).CurrentValues.SetValues(pPromotion);
                    db.SaveChanges();

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Promotion" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Promotion" };
            }
        }

        public ServiceResult PromoStatusDelete(Nullable<int> pPromoID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var promotion = db.Promotions.Where(w => w.Promotion_ID == pPromoID).FirstOrDefault();

                    if (promotion != null)
                    {
                        promotion.Record_Status = pStatus;
                        promotion.Update_By = pUpdateBy;
                        promotion.Update_On = currentdate;
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Promotion" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Promotion" };
            }
        }

        public ServiceResult ProStatusDelete(Nullable<int> pProdID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var pro = db.Products.Where(w => w.Product_ID == pProdID).FirstOrDefault();

                    if (pro != null)
                    {
                        pro.Record_Status = pStatus;
                        pro.Update_By = pUpdateBy;
                        pro.Update_On = currentdate;
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Product" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Product" };
            }
        }

        public ServiceResult CatStatusDelete(Nullable<int> pCatID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var cat = db.Product_Category.Where(w => w.Product_Category_ID == pCatID).FirstOrDefault();

                    if (cat != null)
                    {
                        cat.Record_Status = pStatus;
                        cat.Update_By = pUpdateBy;
                        cat.Update_On = currentdate;
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Category" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Product" };
            }
        }

        //-----------------------------------list inven -------------------------------//
        public ServiceResult MultiePromoStatusDelete(int[] pPromID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var promotion = db.Promotions.Where(w => pPromID.Contains(w.Promotion_ID));
                    if (promotion != null)
                    {
                        foreach (var promo in promotion)
                        {
                            promo.Update_On = currentdate;
                            promo.Update_By = pUpdateBy;
                            promo.Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Promotion" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Promotion" };
            }
        }

        public ServiceResult MultiePromoStatus(int[] pPromID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var promotion = db.Promotions.Where(w => pPromID.Contains(w.Promotion_ID));
                    if (promotion != null)
                    {
                        foreach (var promo in promotion)
                        {
                            promo.Update_On = currentdate;
                            promo.Update_By = pUpdateBy;
                            promo.Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Promotion" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Promotion" };
            }

        }

        public ServiceResult MultieProStatusDelete(int[] pProID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var product = db.Products.Where(w => pProID.Contains(w.Product_ID));
                    if (product != null)
                    {
                        foreach (var pro in product)
                        {
                            pro.Update_On = currentdate;
                            pro.Update_By = pUpdateBy;
                            pro.Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Product" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Product" };
            }
        }

        public ServiceResult MultieProStatus(int[] pProID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var product = db.Products.Where(w => pProID.Contains(w.Product_ID));
                    if (product != null)
                    {
                        foreach (var pro in product)
                        {
                            pro.Update_On = currentdate;
                            pro.Update_By = pUpdateBy;
                            pro.Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Product" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Product" };
            }

        }

        public ServiceResult MultieCateStatusDelete(int[] pCatID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var Category = db.Product_Category.Where(w => pCatID.Contains(w.Product_Category_ID));
                    if (Category != null)
                    {
                        foreach (var cat in Category)
                        {
                            cat.Update_On = currentdate;
                            cat.Update_By = pUpdateBy;
                            cat.Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }

                    return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Category" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Category" };
            }
        }

        public ServiceResult MultieCateStatus(int[] pCatID, string pStatus, string pUpdateBy)
        {
            try
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                using (var db = new SBS2DBContext())
                {
                    var category = db.Product_Category.Where(w => pCatID.Contains(w.Product_Category_ID));
                    if (category != null)
                    {
                        foreach (var cat in category)
                        {
                            cat.Update_On = currentdate;
                            cat.Update_By = pUpdateBy;
                            cat.Record_Status = pStatus;
                        }
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Category" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Category" };
            }

        }

        //Check data use in promotion_product 
        public bool chkPromoProUsed(int pComID, Nullable<int> pProdID)
        {
            var chkProblem = false;
            var i = 0;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var Pp = (from a in db.Promotion_Product where a.Product_ID == pProdID select a).ToList();

                    if (Pp.Count > 0)
                    {
                        foreach (var row in Pp)
                        {
                            var promotion = GetPromotion(pComID, row.Promotion_ID.Value).Record_Status != RecordStatus.Delete ;
                            if (promotion)
                            {
                                i++;
                            }
                        }
                    }

                    if (i > 0)
                        chkProblem = true;
                }
                return chkProblem;
            }
            catch
            {
                return true;
            }
        }

        ////Check data use in product 
        //public bool chkProUsed(Nullable<int> pCatID)
        //{
        //    var chkProblem = false;
        //    try
        //    {
        //        using (var db = new SBS2DBContext())
        //        {

        //            var pro = (from a in db.Products where a.Product_Category_L1 == pCatID || a.Product_Category_L2 == pCatID || a.Product_Category_L3 == pCatID  select a).ToList();
        //            pro = pro.Where(w => w.Record_Status != RecordStatus.Delete).ToList();

        //            if (pro.Count > 0)

        //                chkProblem = true;
        //        }
        //        return chkProblem;
        //    }
        //    catch
        //    {
        //        return true;
        //    }
        //}

       

        //---------------------Product----------------------------------------------------------------------



        //public List<Product> LstProductSearch(int pCompany_ID, Nullable<int> pl1, Nullable<int> pl2, Nullable<int> pl3, Nullable<int> pBranchID, string pSearch, int[] pUSel)
        //{
        //    using (var db = new SBS2DBContext())
        //    {
        //        var prod = db.Products
        //            .Where(w => w.Company_ID == pCompany_ID)
        //            .Include(i => i.Product_Category)
        //            .Include(i => i.Unit_Of_Measurement)
        //            .OrderBy(o => o.Product_Category.Category_Name)
        //            .ThenBy(o => o.Product_Code)
        //            .ThenBy(o => o.Product_Name).ToList();

        //        if (pUSel != null && pUSel.Length > 0)
        //        {
        //            prod = prod.Where(w => !pUSel.Contains(w.Product_ID)).ToList();
        //        }

        //        if (pBranchID.HasValue)
        //        {
        //            prod = prod.Where(w => w.Branch_ID == pBranchID).ToList();
        //        }

        //        if (pl1.HasValue && pl1 > 0)
        //        {
        //            prod = prod.Where(w => w.Product_Category_L1 == pl1).ToList();
        //        }
        //        if (pl2.HasValue && pl2 > 0)
        //        {
        //            prod = prod.Where(w => w.Product_Category_L2 == pl2).ToList();
        //        }
        //        if (pl3.HasValue && pl3 > 0)
        //        {
        //            prod = prod.Where(w => w.Product_Category_L3 == pl3).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(pSearch))
        //        {
        //            prod = prod.Where(w => w.Product_Name == pSearch).ToList();
        //        }

        //        return prod.ToList();
        //    }
        //}


        //public ServiceObjectResult LstProduct(ProductCriteria criteria)
        //{
        //public List<Product> LstProduct(ProductCriteria pcriteria)
        //{
        //    using (var db = new SBS2DBContext())
        //    {

        //        var prod = db.Products
        //               .Where(w => w.Company_ID == pcriteria.Company_ID)
        //               .Include(i => i.Product_Category)
        //               .Include(i => i.Unit_Of_Measurement)
        //               .OrderBy(o => o.Product_Category.Category_Name)
        //               .ThenBy(o => o.Product_Code)
        //               .ThenBy(o => o.Product_Name).ToList();

        //        if (pcriteria.User_Select != null && pcriteria.User_Select.Length > 0)
        //        {
        //            prod = prod.Where(w => !pcriteria.User_Select.Contains(w.Product_ID)).ToList();
        //        }
        //        if (pcriteria.Branch_ID.HasValue)
        //        {
        //            prod = prod.Where(w => w.Branch_ID == pcriteria.Branch_ID).ToList();
        //        }
        //        if (pcriteria.Category_1.HasValue && pcriteria.Category_1 > 0)
        //        {
        //            prod = prod.Where(w => w.Product_Category_L1 == pcriteria.Category_1).ToList();
        //        }
        //        if (pcriteria.Category_2.HasValue && pcriteria.Category_2 > 0)
        //        {
        //            prod = prod.Where(w => w.Product_Category_L2 == pcriteria.Category_2).ToList();
        //        }

        //        if (pcriteria.Category_3.HasValue && pcriteria.Category_3 > 0)
        //        {
        //            prod = prod.Where(w => w.Product_Category_L3 == pcriteria.Category_3).ToList();
        //        }

        //        if (!string.IsNullOrEmpty(pcriteria.Product_Name))
        //        {
        //            prod = prod.Where(w => w.Product_Name == pcriteria.Product_Name).ToList();
        //        }
        //        return prod.ToList();
        //    }
        //}

        //public List<Product> LstProduct(int pCompany_ID)
        //{
        //    using (var db = new SBS2DBContext())
        //    {
        //        return db.Products
        //            .Where(w => w.Company_ID == pCompany_ID)
        //            .Include(i => i.Product_Category)
        //            .Include(i => i.Unit_Of_Measurement)
        //            .OrderBy(o => o.Product_Category.Category_Name)
        //            .ThenBy(o => o.Product_Code)
        //            .ThenBy(o => o.Product_Name)
        //            .ToList();
        //    }
        //}


        public List<Quantity_On_Hand> LstQuantityOnHand(List<Inventory_Transaction> transactionList)
        {
            var qtyOnHands = new List<Quantity_On_Hand>();
            if (transactionList != null)
            {
                foreach (var row in transactionList)
                {
                    if (row.Location_ID.HasValue)
                    {
                        var currentQtyOnHand =
                            qtyOnHands.Where(w => w.Location_ID == row.Location_ID.Value).FirstOrDefault();
                        if (qtyOnHands.Where(w => w.Location_ID == row.Location_ID.Value).FirstOrDefault() == null)
                        {
                            var qtyOnHand = new Quantity_On_Hand();
                            if (row.Inventory_Location != null)
                            {
                                qtyOnHand.Location_ID = row.Inventory_Location.Inventory_Location_ID;
                                qtyOnHand.Location = row.Inventory_Location;
                            }

                            if (row.Transaction_Type == InventoryType.Receive)
                            {
                                qtyOnHand.Quantity = row.Qty.HasValue ? row.Qty.Value : 0;
                            }
                            else if (row.Transaction_Type == InventoryType.Withdraw |
                                     row.Transaction_Type == InventoryType.Return |
                                     row.Transaction_Type == InventoryType.Sale)
                            {
                                qtyOnHand.Quantity = -1 * (row.Qty.HasValue ? row.Qty.Value : 0);
                            }

                            qtyOnHands.Add(qtyOnHand);
                        }
                        else
                        {
                            if (row.Transaction_Type == InventoryType.Receive)
                            {
                                currentQtyOnHand.Quantity = currentQtyOnHand.Quantity +
                                                            (row.Qty.HasValue ? row.Qty.Value : 0);
                            }
                            else if (row.Transaction_Type == InventoryType.Withdraw |
                                     row.Transaction_Type == InventoryType.Return |
                                     row.Transaction_Type == InventoryType.Sale)
                            {
                                currentQtyOnHand.Quantity = currentQtyOnHand.Quantity -
                                                            (row.Qty.HasValue ? row.Qty.Value : 0);
                            }
                        }
                    }
                    else
                    {
                        var currentQtyOnHand = qtyOnHands.Where(w => w.Location_ID == null).FirstOrDefault();
                        if (currentQtyOnHand == null)
                        {
                            currentQtyOnHand = new Quantity_On_Hand() { Quantity = 0 };
                            if (row.Transaction_Type == InventoryType.Receive)
                            {
                                currentQtyOnHand.Quantity = row.Qty.HasValue ? row.Qty.Value : 0;
                            }
                            else if (row.Transaction_Type == InventoryType.Withdraw |
                                     row.Transaction_Type == InventoryType.Return |
                                     row.Transaction_Type == InventoryType.Sale)
                            {
                                currentQtyOnHand.Quantity = -1 * (row.Qty.HasValue ? row.Qty.Value : 0);
                            }
                            qtyOnHands.Add(currentQtyOnHand);
                        }
                        else
                        {
                            if (row.Transaction_Type == InventoryType.Receive)
                            {
                                currentQtyOnHand.Quantity = currentQtyOnHand.Quantity +
                                                            (row.Qty.HasValue ? row.Qty.Value : 0);
                            }
                            else if (row.Transaction_Type == InventoryType.Withdraw |
                                     row.Transaction_Type == InventoryType.Return |
                                     row.Transaction_Type == InventoryType.Sale)
                            {
                                currentQtyOnHand.Quantity = currentQtyOnHand.Quantity -
                                                            (row.Qty.HasValue ? row.Qty.Value : 0);
                            }
                        }

                    }
                }
            }
            return qtyOnHands;

        }

        public Product GetProduct(Nullable<int> pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Products
                    //.Include(i => i.Extra_Product_Info)
                    .Include(i => i.Product_Image)
                    .Include(i => i.Kits1)
                    .Include(i => i.Boms1)
                    .Include(i => i.Measurements)
                    .Include(i => i.Unit_Of_Measurement)
                    .Include(i => i.Product_Color)
                    .Include(i => i.Product_Size)
                    .Include(i => i.Product_Price)
                    .Include(i => i.Product_Category)
                    .Include("Product_Tag.Tag")
                    .Include("Product_Attribute.Product_Attribute_Value")
                    .Include("Product_Image.Product_Image_Attribute.Product_Attribute_Map.Product_Attribute_Map_Price")
                    .FirstOrDefault(w => w.Product_ID == pProductID);
            }
        }

        public bool VerifyProductCode(string productCode, int? productId)
        {
            using (var db = new SBS2DBContext())
            {
                if (productId.HasValue && productId.Value > 0)
                {
                    return !db.Products.Any(a => a.Product_Code == productCode && a.Product_ID != productId.Value);
                }
                return !db.Products.Any(a => a.Product_Code == productCode);
            }
        }

        public ServiceResult InsertProduct(Product pProduct, 
            Measurement pMeasurement,
            Kit[] pKit, string[] pKitRowsType,
            Bom[] pBom, string[] pBomRowsType,
            Dictionary<string, string[]> attributes,
            List<AttributeMapPriceViewModel> attributeMapPrices,
            AttributeMapImageViewModel[] mapImages,
            string[] productTagNames, 
            int[] relatedProducts)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pMeasurement != null)
                    {
                        pProduct.Measurements.Add(pMeasurement);
                    }

                    if (pProduct.Assembly_Type == ProductInfoType.Kit)
                    {
                        if (pKit != null && pKitRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pKit)
                            {
                                if (pKitRowsType[i] == RowType.ADD | pKitRowsType[i] == RowType.EDIT)
                                {
                                    pProduct.Kits1.Add(row);
                                }
                                i++;
                            }
                        }
                    }
                    else if (pProduct.Assembly_Type == ProductInfoType.Bom)
                    {

                        if (pBom != null && pBomRowsType != null)
                        {
                            var i = 0;
                            foreach (var row in pBom)
                            {
                                if (pBomRowsType[i] == RowType.ADD | pBomRowsType[i] == RowType.EDIT)
                                {
                                    pProduct.Boms1.Add(row);
                                }
                                i++;
                            }
                        }
                    }

                    if (productTagNames != null)
                    {
                        var foundTags = db.Tags.Where(w => productTagNames.Contains(w.Tag_Name)).ToList();
                        var foundNames = foundTags.Select(s => s.Tag_Name).ToArray();
                        var nameNotMaps = foundNames.Where(productTagNames.Contains);

                        foreach (var tag in nameNotMaps.Select(notMap => foundTags.FirstOrDefault(f => f.Tag_Name == notMap)))
                            pProduct.Product_Tag.Add(new Product_Tag { Tag_ID = tag.Tag_ID });

                        var newTags = productTagNames.Except(foundNames);
                        foreach (var newTag in newTags)
                            pProduct.Product_Tag.Add(new Product_Tag
                            {
                                Tag = new Tag { Tag_Name = newTag }
                            });
                    }

                    InsertProductAttribute(ref pProduct, attributes);
                    db.Products.Add(pProduct);
                    db.SaveChanges();
                    attributeMapPrices = UpdateAttributeMapPriceIds(pProduct, attributeMapPrices);
                    SyncAttributeMapPrice(pProduct.Product_ID, attributeMapPrices, db);
                    if (pProduct.Product_Image != null)
                        CreateImageMapAttriburte(pProduct, pProduct.Product_Image.ToArray(), attributeMapPrices, mapImages, db);

                    if(relatedProducts != null)
                    {
                        //delete exists related products
                        RemoveExistRelatedProduct(pProduct.Product_ID);

                        foreach (var p in relatedProducts)
                        {
                            var rProduct = GetProduct(p);

                            var pRelated = new Product_RelatedProduct
                            {
                                Based_Product_ID = pProduct.Product_ID,
                                Related_Product_ID = rProduct.Product_ID,
                                Related_Product_Code = rProduct.Product_Code,
                                Related_Product_Name = rProduct.Product_Name,
                                Update_On = pProduct.Update_On.Value,
                                Update_By = pProduct.Update_By
                            };
                            InsertRelatedProduct(pRelated);
                        }
                    }
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE),
                        Field = "Product"
                    };
                }
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR),
                    Field = "Product"
                };
            }
        }

        private static List<AttributeMapPriceViewModel> UpdateAttributeMapPriceIds(Product pProduct, List<AttributeMapPriceViewModel> attributeMapPrices)
        {
            foreach (var attributeMapPrice in attributeMapPrices)
            {
                if (pProduct.Product_Price.Count > 0)
                    attributeMapPrice.PriceId0 = pProduct.Product_Price.ToList()[0].Product_Price_ID;
                if (pProduct.Product_Price.Count > 1)
                    attributeMapPrice.PriceId1 = pProduct.Product_Price.ToList()[1].Product_Price_ID;
                if (pProduct.Product_Price.Count > 2)
                    attributeMapPrice.PriceId2 = pProduct.Product_Price.ToList()[2].Product_Price_ID;
            }
            return attributeMapPrices;
        }

        private static void CreateImageMapAttriburte(Product pProduct, Product_Image[] pImage, List<AttributeMapPriceViewModel> attributeMapPrices,
            IReadOnlyList<AttributeMapImageViewModel> mapImages, SBS2DBContext db)
        {
            if (pImage == null) return;
            var allAttributeMaps = db.Product_Attribute_Map
                .Where(w => w.Product_Attribute_Value.Product_Attribute.Product_ID == pProduct.Product_ID)
                .ToList();
            for (var i = 0; i < pImage.Length; i++)
            {
                if (mapImages == null) continue;
                var mapItem = mapImages[i];
                foreach (var attrImg in mapItem.AttributeNames)
                {
                    var splitValues = attrImg.Replace(" ", "").Split('-');
                    if (splitValues.Length <= 0) continue;
                    var attrMap = allAttributeMaps.Where(w => w.Product_Attribute_Value.Attribute_Value == splitValues[0]);
                    attrMap = splitValues.Length > 1 ? attrMap.Where(w => w.Product_Attribute_Value1.Attribute_Value == splitValues[1]) : attrMap;
                    attrMap = splitValues.Length > 2 ? attrMap.Where(w => w.Product_Attribute_Value2.Attribute_Value == splitValues[2]) : attrMap;
                    attrMap = splitValues.Length > 3 ? attrMap.Where(w => w.Product_Attribute_Value3.Attribute_Value == splitValues[3]) : attrMap;
                    attrMap = splitValues.Length > 4 ? attrMap.Where(w => w.Product_Attribute_Value4.Attribute_Value == splitValues[4]) : attrMap;

                    var attrMapItem = attrMap.FirstOrDefault();
                    if (attrMapItem == null) continue;
                    var addImageMapAttr = new Product_Image_Attribute
                    {
                        Map_ID = attrMapItem.Map_ID,
                        Product_Image_ID = pImage[i].Product_Image_ID
                    };
                    db.Product_Image_Attribute.Add(addImageMapAttr);
                }
            }
            db.SaveChanges();
        }

        private static void InsertProductAttribute(ref Product pProduct, Dictionary<string, string[]> attributes)
        {
            if (!attributes.Any()) return;
            foreach (var attr in attributes.Select(attribute => new Product_Attribute
            {
                Attribute_Name = attribute.Key,
                Product_Attribute_Value = attribute.Value
                    .Select(vals => new Product_Attribute_Value { Attribute_Value = vals })
                    .ToList()
            }))
                pProduct.Product_Attribute.Add(attr);
        }

        public ServiceResult UpdateProduct(Product pProduct, 
            Measurement pMeasurement,
            Kit[] pKit, string[] pKitRowsType,
            Bom[] pBom, string[] pBomRowsType,
            Dictionary<string, string[]> attributes,
            List<AttributeMapPriceViewModel> attributeMapPrices,
            AttributeMapImageViewModel[] mapImages,
            string[] productTagNames, 
            int[] relatedProducts)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = db.Products
                        .Include("Product_Attribute.Product_Attribute_Value")
                        .Include("Product_Tag.Tag")
                        .Include("Product_Image.Product_Image_Attribute")
                        .Include("Product_Price.Product_Attribute_Map_Price")
                        .FirstOrDefault(f => f.Product_ID == pProduct.Product_ID);

                    if (current != null)
                    {
                        if (productTagNames != null)
                        {
                            var oldNames = current.Product_Tag.Select(s => s.Tag.Tag_Name).ToArray();
                            var foundTags = db.Tags.Where(w => productTagNames.Contains(w.Tag_Name)).ToList();
                            var foundNames = foundTags.Select(s => s.Tag_Name).ToArray();
                            var nameNotMaps = foundNames.Except(oldNames);
                            foreach (var tag in nameNotMaps.Select(notMap => foundTags.FirstOrDefault(f => f.Tag_Name == notMap)))
                                current.Product_Tag.Add(new Product_Tag
                                {
                                    Tag_ID = tag.Tag_ID
                                });

                            var newTags = productTagNames.Except(foundNames);
                            foreach (var newTag in newTags)
                                current.Product_Tag.Add(new Product_Tag
                                {
                                    Tag = new Tag { Tag_Name = newTag }
                                });

                            var removeTags = oldNames.Except(productTagNames);
                            foreach (var removed in removeTags.Select(removeTag =>
                                current.Product_Tag.FirstOrDefault(f =>
                                    f.Tag.Tag_Name == removeTag)))
                                current.Product_Tag.Remove(removed);
                        }

                        if (relatedProducts != null)
                        {
                            //delete exists related products
                            RemoveExistRelatedProduct(pProduct.Product_ID);

                            foreach (var p in relatedProducts)
                            {
                                var rProduct = GetProduct(p);

                                var pRelated = new Product_RelatedProduct
                                {
                                    Based_Product_ID = pProduct.Product_ID,
                                    Related_Product_ID = rProduct.Product_ID,
                                    Related_Product_Code = rProduct.Product_Code,
                                    Related_Product_Name = rProduct.Product_Name,
                                    Update_On = pProduct.Update_On.Value,
                                    Update_By = pProduct.Update_By
                                };
                                InsertRelatedProduct(pRelated);
                            }
                        }

                        if (pMeasurement != null)
                        {
                            pMeasurement.Update_By = pProduct.Update_By;
                            pMeasurement.Update_On = pProduct.Update_On;

                            if (pMeasurement.Measurement_ID > 0)
                            {
                                //update
                                var currentmeasurement =
                                    (from a in db.Measurements
                                     where a.Measurement_ID == pMeasurement.Measurement_ID
                                     select a).FirstOrDefault();

                                if (currentmeasurement != null)
                                {
                                    pMeasurement.Create_By = currentmeasurement.Create_By;
                                    pMeasurement.Create_On = currentmeasurement.Create_On;

                                    db.Entry(currentmeasurement).CurrentValues.SetValues(pMeasurement);
                                }
                            }
                            else
                            {
                                pMeasurement.Create_By = pProduct.Create_By;
                                pMeasurement.Create_On = pProduct.Create_On;
                                pMeasurement.Product_ID = pProduct.Product_ID;

                                db.Measurements.Add(pMeasurement);
                            }
                        }

                        if (pProduct.Unit_Of_Measurement != null)
                            foreach (var row in pProduct.Unit_Of_Measurement)
                            {

                                var currentUom = current.Unit_Of_Measurement.FirstOrDefault(f =>
                                    f.Unit_Of_Measurement_ID == row.Unit_Of_Measurement_ID);

                                row.Update_By = pProduct.Update_By;
                                row.Update_On = pProduct.Update_On;

                                if (currentUom != null)
                                {
                                    currentUom.Conversion_Factor = row.Conversion_Factor;
                                    currentUom.Is_Default = row.Is_Default;
                                    currentUom.Unit = row.Unit;
                                    currentUom.Unit_Code = row.Unit_Code;
                                    currentUom.Is_Stocking = row.Is_Stocking;
                                    currentUom.Update_By = row.Update_By;
                                    currentUom.Update_On = row.Update_On;

                                }
                                else
                                {
                                    row.Create_By = pProduct.Create_By;
                                    row.Create_On = pProduct.Create_On;

                                    current.Unit_Of_Measurement.Add(row);
                                }
                            }
                        RemoveProductUom(pProduct, ref current);

                        if (pProduct.Assembly_Type == ProductInfoType.None)
                        {
                            var kits = db.Kits.Where(w => w.Product_ID == pProduct.Product_ID);
                            db.Kits.RemoveRange(kits);

                            var boms = db.Boms.Where(w => w.Product_ID == pProduct.Product_ID);
                            db.Boms.RemoveRange(boms);
                        }
                        else if (pProduct.Assembly_Type == ProductInfoType.Kit)
                        {
                            var boms = db.Boms.Where(w => w.Product_ID == pProduct.Product_ID);
                            db.Boms.RemoveRange(boms);

                            if (pKit != null && pKitRowsType != null)
                            {
                                var i = 0;
                                foreach (var row in pKit)
                                {
                                    row.Product_ID = pProduct.Product_ID;
                                    row.Update_By = pProduct.Update_By;
                                    row.Update_On = pProduct.Update_On;

                                    if (pKitRowsType[i] == RowType.ADD)
                                    {
                                        row.Create_By = pProduct.Create_By;
                                        row.Create_On = pProduct.Create_On;

                                        db.Kits.Add(row);
                                    }
                                    else if (pKitRowsType[i] == RowType.EDIT)
                                    {
                                        var currentkit = db.Kits.FirstOrDefault(w => w.Kit_ID == row.Kit_ID);
                                        if (currentkit != null)
                                        {
                                            row.Create_By = currentkit.Create_By;
                                            row.Create_On = currentkit.Create_On;

                                            db.Entry(currentkit).CurrentValues.SetValues(row);
                                        }
                                    }
                                    else if (pKitRowsType[i] == RowType.DELETE)
                                    {
                                        var currentkit = db.Kits.FirstOrDefault(w => w.Kit_ID == row.Kit_ID);
                                        if (currentkit != null)
                                        {
                                            db.Kits.Remove(currentkit);
                                        }
                                    }
                                    i++;
                                }
                            }
                        }
                        else if (pProduct.Assembly_Type == ProductInfoType.Bom)
                        {
                            var kits = db.Kits.Where(w => w.Product_ID == pProduct.Product_ID);
                            db.Kits.RemoveRange(kits);

                            if (pBom != null && pBomRowsType != null)
                            {
                                var i = 0;
                                foreach (var row in pBom)
                                {
                                    row.Product_ID = pProduct.Product_ID;
                                    row.Update_By = pProduct.Update_By;
                                    row.Update_On = pProduct.Update_On;

                                    if (pBomRowsType[i] == RowType.ADD)
                                    {
                                        row.Create_By = pProduct.Create_By;
                                        row.Create_On = pProduct.Create_On;

                                        db.Boms.Add(row);
                                    }
                                    else if (pBomRowsType[i] == RowType.EDIT)
                                    {
                                        var currentbom = db.Boms.FirstOrDefault(w => w.Bom_ID == row.Bom_ID);
                                        if (currentbom != null)
                                        {
                                            row.Create_By = currentbom.Create_By;
                                            row.Create_On = currentbom.Create_On;

                                            db.Entry(currentbom).CurrentValues.SetValues(row);
                                        }
                                    }
                                    else if (pBomRowsType[i] == RowType.DELETE)
                                    {
                                        var currentbom = db.Boms.FirstOrDefault(w => w.Bom_ID == row.Bom_ID);
                                        if (currentbom != null)
                                        {
                                            db.Boms.Remove(currentbom);
                                        }
                                    }
                                    i++;
                                }
                            }
                        }

                        if (pProduct.Product_Image != null)
                        {
                            foreach (var row in pProduct.Product_Image)
                            {
                                var currentImage = current.Product_Image.FirstOrDefault(f => f.Product_Image_ID == row.Product_Image_ID);
                                if (currentImage != null)
                                {
                                    currentImage.Image_Name = row.Image_Name;
                                    currentImage.Is_Main = row.Is_Main;
                                    if (row.Image.Any()) currentImage.Image = row.Image;
                                }
                                else current.Product_Image.Add(row);
                            }
                            RemoveProductImages(pProduct, ref current);
                        }
                        else if (current.Product_Image.Count > 0)
                            RemoveProductImages(pProduct, ref current);

                        if (pProduct.Product_Price != null)
                        {
                            foreach (var price in pProduct.Product_Price)
                            {
                                var oldPrice = price.Product_Price_ID > 0
                                    ? current.Product_Price.FirstOrDefault(f => f.Product_Price_ID == price.Product_Price_ID)
                                    : current.Product_Price.FirstOrDefault(f => f.Price_Name == price.Price_Name);
                                if (oldPrice == null) current.Product_Price.Add(price);
                                else oldPrice.Price_Name = price.Price_Name;
                            }
                            RemoveProductPrices(pProduct, ref current);
                        }
                        else if (current.Product_Price.Count > 0)
                            RemoveProductPrices(pProduct, ref current);

                        var mapIdRemove = new List<int>();
                        var attrIdRemove = new List<int>();
                        var attrValRemove = new List<int>();
                        CreateOrUpdateAttributes(attributes, ref current, ref mapIdRemove, ref attrIdRemove, ref attrValRemove);

                        //Addded by sun 12-11-2015
                        pProduct.Create_By = current.Create_By;
                        pProduct.Create_On = current.Create_On;

                        db.Entry(current).CurrentValues.SetValues(pProduct);
                        db.SaveChanges();

                        RemoveProductAttributeMap(db, mapIdRemove);
                        RemoveProductAttributeValue(db, attrValRemove, ref mapIdRemove);
                        RemoveProductAttribute(db, attrIdRemove, ref attrValRemove);


                        current = db.Products
                        .Include("Product_Attribute.Product_Attribute_Value")
                        .Include("Product_Tag.Tag")
                        .Include("Product_Image.Product_Image_Attribute")
                        .Include("Product_Price.Product_Attribute_Map_Price")
                        .FirstOrDefault(f => f.Product_ID == pProduct.Product_ID);

                        attributeMapPrices = UpdateAttributeMapPriceIds(current, attributeMapPrices);
                        SyncAttributeMapPrice(pProduct.Product_ID, attributeMapPrices, db);

                        var currentImages = db.Product_Image
                            .Include("Product_Image_Attribute.Product_Attribute_Map.Product_Attribute_Value")
                            .Include("Product_Image_Attribute.Product_Attribute_Map.Product_Attribute_Value1")
                            .Include("Product_Image_Attribute.Product_Attribute_Map.Product_Attribute_Value2")
                            .Include("Product_Image_Attribute.Product_Attribute_Map.Product_Attribute_Value3")
                            .Include("Product_Image_Attribute.Product_Attribute_Map.Product_Attribute_Value4")
                            .Where(w => w.Product_ID == pProduct.Product_ID).ToList();

                        var allAttributeMaps = db.Product_Attribute_Map
                            .Where(w => w.Product_Attribute_Value.Product_Attribute.Product_ID == pProduct.Product_ID)
                            .ToList();

                        if (pProduct.Product_Image == null)
                            return new ServiceResult
                            {
                                Code = ERROR_CODE.SUCCESS,
                                Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT),
                                Field = "Product"
                            };
                        var productImage = pProduct.Product_Image.ToList();
                        for (var i = 0; i < pProduct.Product_Image.Count; i++)
                        {
                            var currentImage = currentImages.FirstOrDefault(f => f.Product_Image_ID == productImage[i].Product_Image_ID);
                            var imageAttributes = currentImage.Product_Image_Attribute.Where(w => w.Map_ID != null).ToList();
                            if (mapImages == null) continue;
                            var mapIds = new List<int>();
                            var mapItem = mapImages[i];
                            foreach (var attrImg in mapItem.AttributeNames)
                            {
                                var splitValues = attrImg.Replace(" ", "").Split('-');
                                if (splitValues.Length <= 0) continue;
                                var attrMap = allAttributeMaps.Where(w => w.Product_Attribute_Value.Attribute_Value == splitValues[0]);
                                attrMap = splitValues.Length > 1 ? attrMap.Where(w => w.Product_Attribute_Value1.Attribute_Value == splitValues[1]) : attrMap;
                                attrMap = splitValues.Length > 2 ? attrMap.Where(w => w.Product_Attribute_Value2.Attribute_Value == splitValues[2]) : attrMap;
                                attrMap = splitValues.Length > 3 ? attrMap.Where(w => w.Product_Attribute_Value3.Attribute_Value == splitValues[3]) : attrMap;
                                attrMap = splitValues.Length > 4 ? attrMap.Where(w => w.Product_Attribute_Value4.Attribute_Value == splitValues[4]) : attrMap;

                                var attrMapItem = attrMap.FirstOrDefault();
                                if (attrMapItem == null) continue;
                                var findMap = imageAttributes.FirstOrDefault(f => f.Map_ID == attrMapItem.Map_ID);
                                mapIds.Add(attrMapItem.Map_ID);
                                if (findMap != null) continue;
                                var addImageMapAttr = new Product_Image_Attribute
                                {
                                    Map_ID = attrMapItem.Map_ID,
                                    Product_Image_ID = currentImage.Product_Image_ID
                                };
                                db.Product_Image_Attribute.Add(addImageMapAttr);
                            }

                            var notExitsImgAttrs = imageAttributes.Where(w => w.Map_ID != null && !mapIds.Contains(w.Map_ID.Value)).ToList();
                            foreach (var removed in notExitsImgAttrs)
                                db.Product_Image_Attribute.Remove(removed);
                        }
                        db.SaveChanges();
                    }

                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT),
                        Field = "Product"
                    };
                }
            }
            catch (Exception e)
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_504_UPDATE_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR),
                    Field = "Product"
                };
            }
        }

        private static void RemoveProductAttribute(SBS2DBContext db, List<int> attrIdRemove, ref List<int> attrValueIds)
        {
            var attrRemoves = db.Product_Attribute
                .Include(i => i.Product_Attribute_Value)
                .Where(w => attrIdRemove.Contains(w.Attribute_ID))
                .ToList();
            foreach (var attrRemove in attrRemoves)
            {
                attrValueIds.AddRange(attrRemove.Product_Attribute_Value.Select(attrValue => attrValue.Attribute_Value_ID));
                db.Product_Attribute.Remove(attrRemove);
            }
            db.SaveChanges();
        }

        private static void RemoveProductAttributeValue(SBS2DBContext db, ICollection<int> attrValueIds, ref List<int> mapIdRemove)
        {
            var attrValueRemoves = db.Product_Attribute_Value
                .Include(i => i.Product_Attribute_Map)
                .Include(i => i.Product_Attribute_Map1)
                .Include(i => i.Product_Attribute_Map2)
                .Include(i => i.Product_Attribute_Map3)
                .Include(i => i.Product_Attribute_Map4)
                .Where(w => attrValueIds.Contains(w.Attribute_Value_ID)).ToList();
            foreach (var productAttributeValue in attrValueRemoves)
            {
                mapIdRemove.AddRange(productAttributeValue.Product_Attribute_Map
                    .Where(w => w.Attr1 != null)
                    .Select(s => s.Attr1 ?? 0));
                mapIdRemove.AddRange(productAttributeValue.Product_Attribute_Map
                    .Where(w => w.Attr2 != null)
                    .Select(s => s.Attr2 ?? 0));
                mapIdRemove.AddRange(productAttributeValue.Product_Attribute_Map
                    .Where(w => w.Attr3 != null)
                    .Select(s => s.Attr3 ?? 0));
                mapIdRemove.AddRange(productAttributeValue.Product_Attribute_Map
                    .Where(w => w.Attr4 != null)
                    .Select(s => s.Attr4 ?? 0));
                mapIdRemove.AddRange(productAttributeValue.Product_Attribute_Map
                    .Where(w => w.Attr5 != null)
                    .Select(s => s.Attr5 ?? 0));
                db.Product_Attribute_Value.Remove(productAttributeValue);
            }
            db.SaveChanges();
        }

        private static void RemoveProductAttributeMap(SBS2DBContext db, List<int> mapIdRemove)
        {
            var mapRemoves = db.Product_Attribute_Map
                .Include(i => i.Product_Attribute_Map_Price)
                .Include(i => i.Product_Image_Attribute)
                .Include(i => i.Product_Attribute_Value)
                .Include(i => i.Product_Attribute_Value1)
                .Include(i => i.Product_Attribute_Value2)
                .Include(i => i.Product_Attribute_Value3)
                .Include(i => i.Product_Attribute_Value4)
                .Where(w => mapIdRemove.Contains(w.Map_ID)).ToList();
            foreach (var mapRemove in mapRemoves)
                db.Product_Attribute_Map.Remove(mapRemove);
            db.SaveChanges();
        }

        private static void RemoveProductPrices(Product pProduct, ref Product current)
        {
            List<Product_Price> productPricesRemoves;
            if (pProduct.Product_Price == null)
                productPricesRemoves = current.Product_Price.ToList();
            else
            {
                var priceNames = pProduct.Product_Price.Select(s => s.Price_Name);
                productPricesRemoves = current.Product_Price.Where(w => !priceNames.Contains(w.Price_Name)).ToList();
            }

            foreach (var notExitsPrice in productPricesRemoves)
                current.Product_Price.Remove(notExitsPrice);

        }

        private static void RemoveProductImages(Product pProduct, ref Product current)
        {
            List<Product_Image> imageRemoves;
            if (pProduct.Product_Image == null)
                imageRemoves = current.Product_Image.ToList();
            else
            {
                var imageRemoveIds = pProduct.Product_Image.Select(s => s.Product_Image_ID);
                imageRemoves = current.Product_Image.Where(w => !imageRemoveIds.Contains(w.Product_Image_ID)).ToList();
            }

            foreach (var productImage in imageRemoves)
                current.Product_Image.Remove(productImage);

        }

        private static void RemoveProductUom(Product pProduct, ref Product current)
        {
            List<Unit_Of_Measurement> itemRemoves;
            if (pProduct.Unit_Of_Measurement == null)
                itemRemoves = current.Unit_Of_Measurement.ToList();
            else
            {
                var removeIds = pProduct.Unit_Of_Measurement.Select(s => s.Unit_Of_Measurement_ID);
                itemRemoves = current.Unit_Of_Measurement.Where(w =>
                    !removeIds.Contains(w.Unit_Of_Measurement_ID)).ToList();
            }
            foreach (var itemRemove in itemRemoves)
                current.Unit_Of_Measurement.Remove(itemRemove);
        }

        private static void CreateOrUpdateAttributes(Dictionary<string, string[]> attributes, ref Product current,
            ref List<int> mapIdRemove, ref List<int> attrIdRemoves, ref List<int> attrValRemoves)
        {
            if (!attributes.Any())
            {
                var delCurrentAttributes = current.Product_Attribute.ToList();

                foreach (var delCurrentAttribute in delCurrentAttributes)
                    current.Product_Attribute.Remove(delCurrentAttribute);
            }
            else
            {
                foreach (var attribute in attributes)
                {
                    var oldAttr = current.Product_Attribute.FirstOrDefault(f => f.Attribute_Name == attribute.Key);
                    if (oldAttr != null)
                    {
                        foreach (var oldValue in from vals in attribute.Value
                                                 let oldValue = oldAttr.Product_Attribute_Value
                                                     .FirstOrDefault(f => f.Attribute_Value == vals)
                                                 where oldValue == null
                                                 select new Product_Attribute_Value { Attribute_Value = vals })
                            oldAttr.Product_Attribute_Value.Add(oldValue);

                        var attributeValues = attribute.Value.Select(s => s);
                        var notExitsValue =
                            oldAttr.Product_Attribute_Value.Where(
                                w => !attributeValues.Contains(w.Attribute_Value)).ToList();
                        foreach (var delValue in notExitsValue)
                        {
                            if (delValue.Product_Attribute_Map != null)
                                mapIdRemove.AddRange(delValue.Product_Attribute_Map.Select(map => map.Map_ID));
                            if (delValue.Product_Attribute_Map1 != null)
                                mapIdRemove.AddRange(delValue.Product_Attribute_Map1.Select(map => map.Map_ID));
                            if (delValue.Product_Attribute_Map2 != null)
                                mapIdRemove.AddRange(delValue.Product_Attribute_Map2.Select(map => map.Map_ID));
                            if (delValue.Product_Attribute_Map3 != null)
                                mapIdRemove.AddRange(delValue.Product_Attribute_Map3.Select(map => map.Map_ID));
                            if (delValue.Product_Attribute_Map4 != null)
                                mapIdRemove.AddRange(delValue.Product_Attribute_Map4.Select(map => map.Map_ID));
                            oldAttr.Product_Attribute_Value.Remove(delValue);
                        }
                    }
                    else
                    {
                        var newAttr = new Product_Attribute
                        {
                            Attribute_Name = attribute.Key,
                            Product_Attribute_Value = attribute.Value
                                .Select(vals => new Product_Attribute_Value { Attribute_Value = vals })
                                .ToList()
                        };
                        current.Product_Attribute.Add(newAttr);
                    }
                }
                var attributeNames = attributes.Select(s => s.Key);
                var notExits = current.Product_Attribute.Where(w => !attributeNames.Contains(w.Attribute_Name)).ToList();

                foreach (var delAttr in notExits)
                {
                    attrIdRemoves.Add(delAttr.Attribute_ID);
                    attrValRemoves.AddRange(delAttr.Product_Attribute_Value.Select(s => s.Attribute_Value_ID));
                    foreach (var attrVal in delAttr.Product_Attribute_Value)
                    {
                        if (attrVal.Product_Attribute_Map != null) mapIdRemove.AddRange(attrVal.Product_Attribute_Map.Select(s => s.Map_ID));
                        if (attrVal.Product_Attribute_Map1 != null) mapIdRemove.AddRange(attrVal.Product_Attribute_Map1.Select(s => s.Map_ID));
                        if (attrVal.Product_Attribute_Map2 != null) mapIdRemove.AddRange(attrVal.Product_Attribute_Map2.Select(s => s.Map_ID));
                        if (attrVal.Product_Attribute_Map3 != null) mapIdRemove.AddRange(attrVal.Product_Attribute_Map3.Select(s => s.Map_ID));
                        if (attrVal.Product_Attribute_Map4 != null) mapIdRemove.AddRange(attrVal.Product_Attribute_Map4.Select(s => s.Map_ID));
                    }
                    current.Product_Attribute.Remove(delAttr);
                }
            }
        }

        public void CreateOrUpdateAttributes(Dictionary<string, string[]> attributes, int productId)
        {
            using (var db = new SBS2DBContext())
            {
                var current = db.Products
                    .FirstOrDefault(f => f.Product_ID == productId);
                if (current != null)
                {
                    var mapIdRemove = new List<int>();
                    var attrIdRemove = new List<int>();
                    var attrValRemove = new List<int>();

                    CreateOrUpdateAttributes(attributes, ref current, ref mapIdRemove, ref attrIdRemove, ref attrValRemove);
                    db.SaveChanges();
                }
            }
        }

        public void SyncAttributeMapPrice(int productId, List<AttributeMapPriceViewModel> attributeMapPrices,
            SBS2DBContext db)
        {
            var oldAttributeMaps = db.Product_Attribute_Map
                .Include("Product_Attribute_Map_Price")
                .Where(w => w.Product_Attribute_Value.Product_Attribute.Product_ID == productId)
                .ToList();

            var productAttributeValues = db.Product_Attribute_Value.Where(w =>
                w.Product_Attribute.Product_ID == productId).ToList();

            foreach (var attributeMapPrice in attributeMapPrices)
            {
                Product_Attribute_Map attributeMap;
                if (attributeMapPrice.AttributeMapId == null)
                {
                    attributeMap = ProductAttributeMap(productAttributeValues, attributeMapPrice);
                    db.Product_Attribute_Map.Add(attributeMap);
                }
                else
                {
                    attributeMap = oldAttributeMaps.FirstOrDefault(f =>
                        f.Map_ID == attributeMapPrice.AttributeMapId);
                    attributeMap = ProductAttributeMap(productAttributeValues, attributeMapPrice, attributeMap);

                    foreach (var mapPrice in attributeMap.Product_Attribute_Map_Price)
                        mapPrice.Price = mapPrice.Product_Price_ID == attributeMapPrice.PriceId0
                            ? mapPrice.Price = attributeMapPrice.Price0
                            : mapPrice.Product_Price_ID == attributeMapPrice.PriceId1
                                ? mapPrice.Price = attributeMapPrice.Price1
                                : mapPrice.Product_Price_ID == attributeMapPrice.PriceId2
                                    ? mapPrice.Price = attributeMapPrice.Price2
                                    : mapPrice.Price;

                }
            }
            db.SaveChanges();
        }

        public void SyncAttributeMapPrice(int productId, List<AttributeMapPriceViewModel> attributeMapPrices)
        {
            using (var db = new SBS2DBContext())
            {
                var oldAttributeMaps = db.Product_Attribute_Map
                    .Include("Product_Attribute_Map_Price")
                    .Where(w => w.Product_Attribute_Value.Product_Attribute.Product_ID == productId)
                    .ToList();
                var productAttributeValues = db.Product_Attribute_Value.Where(w =>
                    w.Product_Attribute.Product_ID == productId).ToList();

                foreach (var attributeMapPrice in attributeMapPrices)
                {
                    var oldAttributeMap =
                        oldAttributeMaps.FirstOrDefault(f => f.Map_ID == attributeMapPrice.AttributeMapId);
                    if (oldAttributeMap != null)
                        oldAttributeMap = ProductAttributeMap(productAttributeValues, attributeMapPrice, oldAttributeMap);
                    else
                    {
                        oldAttributeMap = ProductAttributeMap(productAttributeValues, attributeMapPrice);
                        db.Product_Attribute_Map.Add(oldAttributeMap);
                    }
                }
                db.SaveChanges();
            }
        }

        private static Product_Attribute_Map ProductAttributeMap(List<Product_Attribute_Value> productAttributeValues,
            AttributeMapPriceViewModel attributeMapPrice, Product_Attribute_Map attributeMap = null)
        {
            if (attributeMap == null) attributeMap = new Product_Attribute_Map();
            var attributeValue0 = productAttributeValues.FirstOrDefault(f =>
                f.Attribute_Value == attributeMapPrice.AttributeValue0);
            var attributeValue1 = productAttributeValues.FirstOrDefault(f =>
                f.Attribute_Value == attributeMapPrice.AttributeValue1);
            var attributeValue2 = productAttributeValues.FirstOrDefault(f =>
                f.Attribute_Value == attributeMapPrice.AttributeValue2);
            var attributeValue3 = productAttributeValues.FirstOrDefault(f =>
                f.Attribute_Value == attributeMapPrice.AttributeValue3);
            var attributeValue4 = productAttributeValues.FirstOrDefault(f =>
                f.Attribute_Value == attributeMapPrice.AttributeValue4);

            attributeMap.Attr1 = attributeValue0 != null ? (int?)attributeValue0.Attribute_Value_ID : null;
            attributeMap.Attr2 = attributeValue1 != null ? (int?)attributeValue1.Attribute_Value_ID : null;
            attributeMap.Attr3 = attributeValue2 != null ? (int?)attributeValue2.Attribute_Value_ID : null;
            attributeMap.Attr4 = attributeValue3 != null ? (int?)attributeValue3.Attribute_Value_ID : null;
            attributeMap.Attr5 = attributeValue4 != null ? (int?)attributeValue4.Attribute_Value_ID : null;

            attributeMap.Costing_Price = attributeMapPrice.CostingPrice;
            attributeMap.Selling_Price = attributeMapPrice.SellingPrice;
            attributeMap.Record_Status = attributeMapPrice.RecordStatus;

            if (attributeMapPrice.PriceId0 != null)
            {
                if (attributeMap.Product_Attribute_Map_Price.Count > 0)
                    attributeMap.Product_Attribute_Map_Price.ToList()[0].Price = attributeMapPrice.Price0;
                else
                    attributeMap.Product_Attribute_Map_Price.Add(new Product_Attribute_Map_Price
                    {
                        Product_Price_ID = attributeMapPrice.PriceId0,
                        Price = attributeMapPrice.Price0
                    });
            }

            if (attributeMapPrice.PriceId1 != null)
            {
                if (attributeMap.Product_Attribute_Map_Price.Count > 1)
                    attributeMap.Product_Attribute_Map_Price.ToList()[1].Price = attributeMapPrice.Price1;
                else
                    attributeMap.Product_Attribute_Map_Price.Add(new Product_Attribute_Map_Price
                    {
                        Product_Price_ID = attributeMapPrice.PriceId1,
                        Price = attributeMapPrice.Price1
                    });
            }

            if (attributeMapPrice.PriceId2 == null) return attributeMap;
            if (attributeMap.Product_Attribute_Map_Price.Count > 2)
                attributeMap.Product_Attribute_Map_Price.ToList()[2].Price = attributeMapPrice.Price2;
            else
                attributeMap.Product_Attribute_Map_Price.Add(new Product_Attribute_Map_Price
                {
                    Product_Price_ID = attributeMapPrice.PriceId2,
                    Price = attributeMapPrice.Price2
                });

            return attributeMap;
        }

        public ServiceResult DeleteProduct(int pProductID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var current = (from a in db.Products
                                        .Include(i => i.Product_Tag)
                                   where a.Product_ID == pProductID
                                   select a)
                                   .FirstOrDefault();
                    if (current == null)
                        return new ServiceResult
                        {
                            Code = ERROR_CODE.SUCCESS,
                            Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE),
                            Field = "Product"
                        };

                    if (current.Product_Image != null)
                        db.Product_Image.RemoveRange(current.Product_Image);

                    var currentmeasurement = (from a in db.Measurements where a.Product_ID == pProductID select a);
                    db.Measurements.RemoveRange(currentmeasurement);

                    var currentUom = (from a in db.Unit_Of_Measurement where a.Product_ID == pProductID select a);
                    db.Unit_Of_Measurement.RemoveRange(currentUom);

                    var kits = db.Kits.Where(w => w.Product_ID == pProductID);
                    db.Kits.RemoveRange(kits);

                    var boms = db.Boms.Where(w => w.Product_ID == pProductID);
                    db.Boms.RemoveRange(boms);

                    var colors = db.Product_Color.Where(w => w.Product_ID == pProductID);
                    db.Product_Color.RemoveRange(colors);

                    var sizes = db.Product_Size.Where(w => w.Product_ID == pProductID);
                    db.Product_Size.RemoveRange(sizes);

                    var images = db.Product_Image
                        .Include(i => i.Product_Image_Attribute)
                        .Where(w => w.Product_ID == pProductID);
                    db.Product_Image.RemoveRange(images);

                    var mapPrice = db.Product_Attribute_Map.Where(w =>
                        w.Product_Attribute_Map_Price.Any(a =>
                            a.Product_Price.Product_ID == pProductID));
                    db.Product_Attribute_Map.RemoveRange(mapPrice);

                    var prices = db.Product_Price
                        .Include(i => i.Product_Attribute_Map_Price)
                        .Where(w => w.Product_ID == pProductID);
                    db.Product_Price.RemoveRange(prices);

                    var attributes = db.Product_Attribute
                        .Include(i => i.Product_Attribute_Value)
                        .Where(w => w.Product_ID == pProductID);
                    db.Product_Attribute.RemoveRange(attributes);

                    db.Products.Remove(current);
                    db.SaveChanges();
                    return new ServiceResult
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE),
                        Field = "Product"
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        Console.WriteLine("Must delete products before deleting category");
                    }
                }
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR),
                    Field = "Product"
                };
            }
            catch
            {
                return new ServiceResult
                {
                    Code = ERROR_CODE.ERROR_505_DELETE_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR),
                    Field = "Product"
                };
            }
        }

        public Unit_Of_Measurement GetUOM(int pUomID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Unit_Of_Measurement.FirstOrDefault(w => w.Unit_Of_Measurement_ID == pUomID);
            }
        }

        public Unit_Of_Measurement GetDefaultUOM(int pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                return
                    db.Unit_Of_Measurement.Where(w => w.Product_ID == pProductID & w.Is_Default == true)
                        .FirstOrDefault();
            }
        }

        //public int SaveProductPhoto(Nullable<int> pProductID, byte[] file, Nullable<int> pIndex)
        //{
        //    try
        //    {
        //        using (var db = new SBS2DBContext())
        //        {
        //            var photo =
        //                db.Product_Image.FirstOrDefault(
        //                    w => w.Product_ID == pProductID && w.Photo_Index == pIndex);
        //            if (photo != null)
        //            {
        //                //UPDATE
        //                photo.Photo = file;
        //                db.Entry(photo).State = EntityState.Modified;

        //                if (pIndex == null || pIndex == 0)
        //                {
        //                    //main picture
        //                    //var product = db.Products.Where(w => w.Product_ID == pProductID).FirstOrDefault();
        //                    //if (product != null)
        //                    //{
        //                    //    product.Product_Profile_Photo_ID = photo.Product_Profile_Photo_ID;
        //                    //}
        //                }
        //            }
        //            else
        //            {
        //                System.Guid id = Guid.NewGuid();
        //                //Insert
        //                Product_Profile_Photo newphoto = new Product_Profile_Photo()
        //                {
        //                    Product_Profile_Photo_ID = id,
        //                    Photo = file,
        //                    Photo_Index = pIndex,
        //                    Product_ID = pProductID
        //                };

        //                db.Product_Profile_Photo.Add(newphoto);

        //                if (pIndex == null || pIndex == 0)
        //                {
        //                    //main picture
        //                    //var product = db.Products.Where(w => w.Product_ID == pProductID).FirstOrDefault();
        //                    //if (product != null)
        //                    //{
        //                    //    product.Product_Profile_Photo_ID = id;
        //                    //}
        //                }
        //            }

        //            db.SaveChanges();

        //            return 1;
        //        }
        //    }
        //    catch
        //    {
        //        //Log
        //        return -500;
        //    }
        //}

        //public int DeleteProductPhoto(Nullable<int> pProductID, Nullable<int> pIndex)
        //{
        //    try
        //    {
        //        using (var db = new SBS2DBContext())
        //        {
        //            var photo =
        //                db.Product_Profile_Photo.FirstOrDefault(
        //                    w => w.Product_ID == pProductID && w.Photo_Index == pIndex);
        //            if (photo != null)
        //            {
        //                //UPDATE
        //                db.Product_Profile_Photo.Remove(photo);
        //                db.SaveChanges();
        //            }
        //            return 1;
        //        }
        //    }
        //    catch
        //    {
        //        //Log
        //        return -500;
        //    }
        //}

        public Product_Image SaveProductImage(Product_Image entity, List<AttributeMapPriceViewModel> attributeMapPrices)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var productAttributeValues = db.Product_Attribute_Value.Where(w =>
                        w.Product_Attribute.Product_ID == entity.Product_ID).ToList();
                    foreach (var attributeMapPrice in attributeMapPrices)
                    {
                        Product_Attribute_Map attributeMap = ProductAttributeMap(productAttributeValues, attributeMapPrice);
                        entity.Product_Image_Attribute.Add(new Product_Image_Attribute
                        {
                            Map_ID = attributeMap.Map_ID
                        });
                    }
                    db.Product_Image.Add(entity);
                    db.SaveChanges();
                    return entity;
                }
            }
            catch
            {
                //Log
                return null;
            }
        }

        public Product_Image UpdateProductImage(Product_Image entity)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    return entity;
                }
            }
            catch
            {
                //Log
                return null;
            }
        }

        public bool UpdateProductImage(Product_Image[] entities)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    foreach (var image in entities)
                    {
                        db.Entry(image).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                //Log
                return false;
            }
        }

        public bool DeleteProductImage(Guid imageId)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    Product_Image image = db.Product_Image.Find(imageId);
                    if (image == null) return false;
                    db.Product_Image.Remove(image);
                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                //Log
                return false;
            }
        }

        public bool DeleteProductImage(Guid[] imageIds)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    List<Product_Image> images = db.Product_Image
                        .Where(w => imageIds.Contains(w.Product_Image_ID))
                        .ToList();
                    foreach (var image in images)
                    {
                        db.Product_Image.Remove(image);
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                //Log
                return false;
            }
        }

        public List<Product_Tag> LstProductTag(int pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                var Tags = db.Product_Tag.Where(a => a.Product_ID == pProductID).ToList();
                return Tags;
            }
        }

        public int LstTagList(string pTag_Name)
        {
            using (var db = new SBS2DBContext())
            {
                var lstTags = db.Tags.Where(w => w.Tag_Name == pTag_Name).ToList();

                if (lstTags.Count > 0)
                {
                    return lstTags[0].Tag_ID;
                }

                return 0;
            }
        }

        public int SaveTag(Tag entity)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Tags.Add(entity);
                    db.SaveChanges();

                    return LstTagList(entity.Tag_Name);
                }
            }
            catch
            {
                //Log
                return 0;
            }
        }

        public List<Product_Vendor> LstProductVendor(int pProductID)
        {
            var productvendors = new List<Product_Vendor>();
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var pohasvendor = db.Purchase_Order_has_Vendor_Product.Where(w => w.Product_ID == pProductID);

                    var pos = pohasvendor.GroupBy(g => g.Purchase_Order.Vendor_ID)
                        .Select(g => new
                        {
                            Product_Code = g.Max(s => s.Product.Product_Code),
                            Product_Name = g.Max(s => s.Product.Product_Name),
                            Product_ID = g.Max(s => s.Product.Product_ID),
                            Vendor_ID = g.Max(m => m.Purchase_Order.Vendor_ID),
                            Vendor_Name = g.Max(m => m.Purchase_Order.Vendor.Name)
                        });

                    foreach (var po in pos)
                    {
                        var productvendor = new Product_Vendor()
                        {
                            Product_Code = po.Product_Code,
                            Product_ID = po.Product_ID,
                            Product_Name = po.Product_Name,
                            Vendor_ID = po.Vendor_ID.Value,
                            Vendor_Name = po.Vendor_Name
                        };
                        var temp =
                            pohasvendor.Where(w => w.Purchase_Order.Vendor_ID == po.Vendor_ID)
                                .OrderByDescending(o => o.Update_On)
                                .FirstOrDefault();
                        if (temp != null)
                        {
                            productvendor.Vendor_Price = temp.Unit_Price.HasValue ? temp.Unit_Price.Value : 0;
                        }
                        productvendors.Add(productvendor);
                    }
                }
            }
            catch
            {

            }
            return productvendors;
        }

        public List<Product_Attribute> LstProductAttribute(int pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                var att = db.Product_Attribute.Where(a => a.Product_ID == pProductID).ToList();
                return att;
            }
        }

        public List<Inventory_Transaction> LstProductTransaction(int pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Inventory_Transaction
                    .Include(i => i.Withdraw)
                    .Include(i => i.Receive)
                    .Include(i => i.Return)
                    .Include(i => i.SO_Return)
                    .Include(i => i.Inventory_Location)
                    .Include(i => i.Receive.Purchase_Order)
                    .Include(i => i.Receive.Purchase_Order.Inventory_Location)
                    .Include(i => i.Withdraw.Sale_Order)
                    //  .Include(i => i.POS_Receipt)
                    .Where(w => w.Product_ID == pProductID)
                    .ToList();
            }
        }

        public List<Inventory_Transaction> LstProductTransactionByCompany(int pCompanyID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Inventory_Transaction
                    .Include(i => i.Withdraw)
                    .Include(i => i.Receive)
                    .Include(i => i.Return)
                    .Include(i => i.SO_Return)
                    .Include(i => i.Inventory_Location)
                    .Include(i => i.Receive.Purchase_Order)
                    .Include(i => i.Receive.Purchase_Order.Inventory_Location)
                    .Include(i => i.Withdraw.Sale_Order)
                    .Where(w => w.Company_ID == pCompanyID)
                    .OrderBy(o => o.Product_ID).ThenBy(o => o.Location_ID)
                    .ToList();
            }
        }

        public bool productExists(string pProductCode, int pCompanyID, ref int pProductID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var product =
                        db.Products.SingleOrDefault(
                            x => x.Product_Code.ToLower() == pProductCode.ToLower() && x.Company_ID == pCompanyID);

                    if (product != null)
                    {
                        pProductID = product.Product_ID;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        //public ServiceResult ImportProducts(List<Product_Details_> prods, int pCompanyID, User_Profile user,
        //    string domain)
        //{

        //    int categoryID = 0;
        //    int poPrefixConfigID = 0;
        //    int rcvPrefixConfigID = 0;

        //    Inventory_Prefix_Config poPrefixConfig = null;
        //    Inventory_Prefix_Config rcvPrefixConfig = null;

        //    VendorService vendService = new VendorService();
        //    PrefixConfigServie prefixService = new PrefixConfigServie();
        //    InventoryConfigurationService invConfig = new InventoryConfigurationService();

        //    int vendorID = 0;

        //    //Loop through unique vendor names from the excel file
        //    foreach (var vendorName in prods.Select(p => p.Vendor).Distinct())
        //    {
        //        //if vendor name is not empty, create PO, Inv receive transactions
        //        if (!string.IsNullOrEmpty(vendorName))
        //        {
        //            //create prefix for Purchase Order if not existing
        //            if (!prefixService.PrefixConfigExists(pCompanyID, InventoryType.PurchaseOrder, ref poPrefixConfigID))
        //            {

        //                poPrefixConfig = new Inventory_Prefix_Config()
        //                {
        //                    Company_ID = pCompanyID,
        //                    Inventory_Type = InventoryType.PurchaseOrder,
        //                    Number_Of_Digit = 5,
        //                    Ref_Count = 1,
        //                    Prefix_Ref_No = "PO"
        //                };

        //                prefixService.InsertPrefigConfig(poPrefixConfig);

        //                poPrefixConfigID = poPrefixConfig.Prefix_Config_ID;

        //            }
        //            else
        //            {
        //                poPrefixConfig = prefixService.GetPrefigConfig(pCompanyID, InventoryType.PurchaseOrder);
        //            }

        //            //create prefix for Receive Transaction if not existing
        //            if (!prefixService.PrefixConfigExists(pCompanyID, InventoryType.Receive, ref rcvPrefixConfigID))
        //            {

        //                rcvPrefixConfig = new Inventory_Prefix_Config()
        //                {
        //                    Company_ID = pCompanyID,
        //                    Inventory_Type = InventoryType.Receive,
        //                    Number_Of_Digit = 5,
        //                    Ref_Count = 1,
        //                    Prefix_Ref_No = "BCR"
        //                };

        //                prefixService.InsertPrefigConfig(rcvPrefixConfig);
        //                rcvPrefixConfigID = rcvPrefixConfig.Prefix_Config_ID;

        //            }
        //            else
        //            {
        //                rcvPrefixConfig = prefixService.GetPrefigConfig(pCompanyID, InventoryType.Receive);
        //            }

        //            //create vendor data if not existing
        //            if (!vendService.VendorExists(vendorName, pCompanyID, ref vendorID))
        //            {

        //                Vendor newVendor = new Vendor()
        //                {

        //                    Company_ID = pCompanyID,
        //                    Name = vendorName,
        //                    Create_By = user.User_Authentication.Email_Address,
        //                    Create_On = DateTime.Now,
        //                    Update_By = user.User_Authentication.Email_Address,
        //                    Update_On = DateTime.Now

        //                };

        //                vendService.insertVendor(newVendor);

        //                vendorID = newVendor.Vendor_ID;
        //            }

        //            foreach (var loc in prods.Where(p => p.Vendor == vendorName).Select(p => p.Location).Distinct())
        //            {
        //                int locationID = 0;

        //                Inventory_Location location = null;

        //                if (!string.IsNullOrEmpty(loc))
        //                {

        //                    if (!invConfig.inventoryLocationExists(loc, pCompanyID, ref locationID))
        //                    {

        //                        location = new Inventory_Location()
        //                        {
        //                            Name = loc,
        //                            Address = string.Empty,
        //                            Company_ID = pCompanyID,
        //                            Create_By = user.User_Authentication.Email_Address,
        //                            Create_On = DateTime.Now
        //                        };

        //                        invConfig.insertInventoryLocation(location);

        //                    }
        //                    else
        //                    {
        //                        location = invConfig.getInventoryLocation(locationID);
        //                    }
        //                }

        //                List<Purchase_Order_has_Vendor_Product> poProducts =
        //                    new List<Purchase_Order_has_Vendor_Product>();

        //                int uomID = 0;

        //                foreach (var prod in prods.Where(p => !string.IsNullOrEmpty(p.Vendor)
        //                                                      && p.Vendor.ToLower() == vendorName.ToLower() &&
        //                                                      p.Location == location.Name))
        //                {
        //                    //save products
        //                    Product uplProduct = null;
        //                    int productID = 0;

        //                    if (!productExists(prod.Code, pCompanyID, ref productID))
        //                    {
        //                        uplProduct = new Product()
        //                        {

        //                            Company_ID = pCompanyID,
        //                            Type = prod.Type,
        //                            Product_Code = prod.Code,
        //                            Product_Name = prod.Name,
        //                            Description = prod.Description,
        //                            Selling_Price = prod.Price,
        //                            Assembly_Type = "None",
        //                            Record_Status = RecordStatus.Active

        //                        };

        //                    }
        //                    else
        //                    {
        //                        uplProduct = GetProduct(productID);
        //                    }

        //                    //create product cate
        //                    //if (!productCategoryExists(prod.Category, pCompanyID, ref categoryID))
        //                    //{

        //                    //    Product_Category category = new Product_Category()
        //                    //    {
        //                    //        Category_Name = prod.Category,
        //                    //        Company_ID = pCompanyID,
        //                    //        Record_Status = RecordStatus.Active
        //                    //    };
        //                    //    //InsertCategory(category);
        //                    //    //uplProduct.Product_Category_ID = category.Product_Category_ID;
        //                    //    uplProduct.Product_Category = category;

        //                    //}
        //                    //else
        //                    //{
        //                    //    uplProduct.Product_Category_ID = categoryID;
        //                    //}


        //                    if (!string.IsNullOrEmpty(prod.Color))
        //                    {

        //                        if (!productColorExists(prod.Color, uplProduct.Product_ID))
        //                        {
        //                            Product_Color color = new Product_Color()
        //                            {
        //                                Product_ID = uplProduct.Product_ID,
        //                                Color_Code = null,
        //                                Color = prod.Color
        //                            };
        //                            //InsertProductColor(color);
        //                            uplProduct.Product_Color.Add(color);
        //                        }
        //                    }

        //                    if (!string.IsNullOrEmpty(prod.Size))
        //                    {

        //                        if (!productSizeExists(prod.Size, uplProduct.Product_ID))
        //                        {
        //                            Product_Size size = new Product_Size()
        //                            {
        //                                Product_ID = uplProduct.Product_ID,
        //                                Size = prod.Size
        //                            };

        //                            //InsertProductSize(size);
        //                            uplProduct.Product_Size.Add(size);
        //                        }
        //                    }

        //                    if (uplProduct.Product_ID <= 0)
        //                        InsertProduct(uplProduct, null, null, null, null, null,
        //                            null, null, null, null, null);

        //                    if (!string.IsNullOrEmpty(prod.UoM))
        //                    {

        //                        if (!productUoMExists(prod.UoM, uplProduct.Product_ID, ref uomID))
        //                        {
        //                            Unit_Of_Measurement uom = new Unit_Of_Measurement()
        //                            {
        //                                Product_ID = uplProduct.Product_ID,
        //                                Unit = prod.UoM,
        //                                Is_Default = true,
        //                                Conversion_Factor = 1
        //                            };

        //                            InsertProductUoM(uom);

        //                            uomID = uom.Unit_Of_Measurement_ID;
        //                        }
        //                    }

        //                    Product addPOProduct = GetProduct(uplProduct.Product_ID);
        //                    Unit_Of_Measurement addUOM = GetUOM(uomID);

        //                    //create po products with id and qty
        //                    Purchase_Order_has_Vendor_Product poProduct = new Purchase_Order_has_Vendor_Product()
        //                    {
        //                        Company_ID = pCompanyID,
        //                        Product_ID = addPOProduct.Product_ID,
        //                        Qty = prod.Qty,
        //                        Unit_Price = addPOProduct.Selling_Price,
        //                        UOM = addUOM.Unit_Of_Measurement_ID,
        //                        UOM_Conversion_Factor = addUOM.Conversion_Factor,
        //                        UOM_Qty = prod.Qty / addUOM.Conversion_Factor,
        //                        UOM_Desc = addUOM.Unit,
        //                        Create_By = user.User_Authentication.Email_Address,
        //                        Create_On = DateTime.Now,
        //                        Update_By = user.User_Authentication.Email_Address,
        //                        Update_On = DateTime.Now
        //                    };

        //                    poProducts.Add(poProduct);
        //                }

        //                //create Purchase Order
        //                Purchase_Order po = new Purchase_Order()
        //                {
        //                    Company_ID = pCompanyID,
        //                    Delivery_Date = DateTime.Now,
        //                    User_Profile_ID = user.Profile_ID,
        //                    Vendor_ID = vendorID,
        //                    Order_Date = DateTime.Now,
        //                    Date_Due = DateTime.Now,
        //                    Order_No = poPrefixConfig.Prefix_Ref_No + "-" +
        //                               DateTime.Today.Year.ToString().Substring(2, 2) +
        //                               poPrefixConfig.Ref_Count.ToString().PadLeft(5, '0'),
        //                    Total_Price = Convert.ToDouble(poProducts.Sum(p => p.Unit_Price * p.Qty)),
        //                    Is_Back_To_Back = false,
        //                    Overall_Status = "AA",
        //                    Create_By = user.User_Authentication.Email_Address,
        //                    Create_On = DateTime.Now,
        //                    Update_By = user.User_Authentication.Email_Address,
        //                    Update_On = DateTime.Now,
        //                    Inventory_Location_ID = location.Inventory_Location_ID,
        //                    Remarks = "Product Upload"
        //                };

        //                po.Purchase_Order_has_Vendor_Product = poProducts;

        //                //Update prefix count for PO
        //                poPrefixConfig.Ref_Count++;
        //                prefixService.UpdatePrefigConfig(poPrefixConfig);

        //                //create and save receive data
        //                Receive receiveItem = new Receive()
        //                {
        //                    Company_ID = pCompanyID,
        //                    //Purchase_Order_ID = po.Purchase_Order_ID,
        //                    Purchase_Order = po,
        //                    Receive_By = user.Profile_ID,
        //                    Receive_Date = DateTime.Now,
        //                    Receive_No = rcvPrefixConfig.Prefix_Ref_No + "-" +
        //                                 DateTime.Today.Year.ToString().Substring(2, 2) +
        //                                 rcvPrefixConfig.Ref_Count.ToString().PadLeft(5, '0')
        //                };

        //                foreach (var poProduct in poProducts)
        //                {

        //                    receiveItem.Inventory_Transaction.Add(new Inventory_Transaction()
        //                    {
        //                        Transaction_Type = InventoryType.Receive,
        //                        Company_ID = pCompanyID,
        //                        Product_ID = poProduct.Product_ID.Value,
        //                        Qty = poProduct.Qty,
        //                        Location_ID = location.Inventory_Location_ID
        //                    });

        //                }

        //                PurchaseOrderService poService = new PurchaseOrderService();
        //                poService.insertReceive(receiveItem);
        //                //Update prefix count for Receive Transaction
        //                rcvPrefixConfig.Ref_Count++;
        //                prefixService.UpdatePrefigConfig(rcvPrefixConfig);

        //            }

        //        }
        //        else
        //        {

        //            foreach (var prod in prods.Where(p => string.IsNullOrEmpty(p.Vendor)))
        //            {
        //                //save products
        //                Product uplProduct = null;
        //                int productID = 0;
        //                int uomID = 0;

        //                if (!productExists(prod.Code, pCompanyID, ref productID))
        //                {
        //                    uplProduct = new Product()
        //                    {

        //                        Company_ID = pCompanyID,
        //                        Type = prod.Type,
        //                        Product_Code = prod.Code,
        //                        Product_Name = prod.Name,
        //                        Description = prod.Description,
        //                        Selling_Price = prod.Price,
        //                        Assembly_Type = "None",
        //                        Record_Status = RecordStatus.Active

        //                    };

        //                }
        //                else
        //                {
        //                    uplProduct = GetProduct(productID);
        //                }

        //                if (!productCategoryExists(prod.Category, pCompanyID, ref categoryID))
        //                {

        //                    Product_Category category = new Product_Category()
        //                    {
        //                        Category_Name = prod.Category,
        //                        Company_ID = pCompanyID,
        //                        Record_Status = RecordStatus.Active
        //                    };

        //                    //InsertCategory(category);
        //                    //uplProduct.Product_Category_ID = category.Product_Category_ID;
        //                    uplProduct.Product_Category = category;

        //                }
        //                else
        //                {
        //                    uplProduct.Product_Category_ID = categoryID;
        //                }

        //                if (!string.IsNullOrEmpty(prod.Color))
        //                {

        //                    if (!productColorExists(prod.Color, uplProduct.Product_ID))
        //                    {
        //                        Product_Color color = new Product_Color()
        //                        {
        //                            Product_ID = uplProduct.Product_ID,
        //                            Color_Code = null,
        //                            Color = prod.Color
        //                        };

        //                        //InsertProductColor(color);
        //                        uplProduct.Product_Color.Add(color);
        //                    }

        //                }

        //                if (!string.IsNullOrEmpty(prod.Size))
        //                {

        //                    if (!productSizeExists(prod.Size, uplProduct.Product_ID))
        //                    {
        //                        Product_Size size = new Product_Size()
        //                        {
        //                            Product_ID = uplProduct.Product_ID,
        //                            Size = prod.Size
        //                        };

        //                        //InsertProductSize(size);
        //                        uplProduct.Product_Size.Add(size);
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(prod.UoM))
        //                {

        //                    if (!productUoMExists(prod.UoM, uplProduct.Product_ID, ref uomID))
        //                    {
        //                        Unit_Of_Measurement uom = new Unit_Of_Measurement()
        //                        {
        //                            Product_ID = uplProduct.Product_ID,
        //                            Unit = prod.UoM,
        //                            Is_Default = true,
        //                            Conversion_Factor = 1
        //                        };

        //                        //InsertProductUoM(uom);
        //                        uplProduct.Unit_Of_Measurement.Add(uom);
        //                    }
        //                }

        //                if (uplProduct.Product_ID <= 0)
        //                    InsertProduct(uplProduct, null, null, null, null,
        //                        null, null, null, null, null, null);
        //                else
        //                    UpdateProduct(uplProduct, null, null, null, null,
        //                        null, null, null, null, null, null);
        //            }
        //        }
        //    }

        //    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS) };
        //}

        //---------------------Size----------------------------------------------------------------------

        public bool productSizeExists(string size, int product_ID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var location =
                        db.Product_Size.SingleOrDefault(
                            x => x.Size.ToLower() == size.ToLower() && x.Product_ID == product_ID);

                    if (location != null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public ServiceResult InsertProductSize(Product_Size pSize)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Product_Size.Add(pSize);
                    db.SaveChanges();
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE),
                        Field = "Category"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR),
                    Field = "Category"
                };
            }
        }

        //---------------------Color----------------------------------------------------------------------
        public bool productColorExists(string color, int product_ID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var location =
                        db.Product_Color.SingleOrDefault(
                            x => x.Color.ToLower() == color.ToLower() && x.Product_ID == product_ID);

                    if (location != null)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public ServiceResult InsertProductColor(Product_Color pColor)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Product_Color.Add(pColor);
                    db.SaveChanges();
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE),
                        Field = "Category"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR),
                    Field = "Category"
                };
            }
        }

        //---------------------Unit of Measure-------------------------------------------------------------
        public bool productUoMExists(string unit, int product_ID, ref int uomID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var uom =
                        db.Unit_Of_Measurement.SingleOrDefault(
                            x => x.Unit.ToLower() == unit.ToLower() && x.Product_ID == product_ID);

                    if (uom != null)
                    {
                        uomID = uom.Unit_Of_Measurement_ID;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public ServiceResult InsertProductUoM(Unit_Of_Measurement pUnit)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Unit_Of_Measurement.Add(pUnit);
                    db.SaveChanges();
                    return new ServiceResult()
                    {
                        Code = ERROR_CODE.SUCCESS,
                        Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE),
                        Field = "Category"
                    };
                }
            }
            catch
            {
                return new ServiceResult()
                {
                    Code = ERROR_CODE.ERROR_503_INSERT_ERROR,
                    Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR),
                    Field = "Category"
                };
            }
        }

        public ServiceObjectResult LstProduct(ProductCriteria criteria)
        {
            var result = new ServiceObjectResult();
            result.Object = new List<Product>();

            using (var db = new SBS2DBContext())
            {
                var products = db.Products
                    .Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete)
                    .Include(i => i.Product_Category)
                    .Include(i => i.Product_Category1)
                    .Include(i => i.Product_Category2)
                    .Include(i => i.Product_Image)
                    .Include(i => i.Product_Attribute)
                    .Include(i => i.Product_Attribute.Select(s => s.Product_Attribute_Value))
                    .Include(i => i.Product_Attribute_Map)
                    .Include(i => i.Unit_Of_Measurement);

                if (criteria.Display_Photo)
                {
                    products = products.Include(i => i.Product_Image);
                }

                if (criteria.Category_ID.HasValue && criteria.Category_ID > 0)
                {
                    products = products.Where(w => w.Product_Category_L1 == criteria.Category_ID | w.Product_Category_L2 == criteria.Category_ID | w.Product_Category_L3 == criteria.Category_ID);
                }
                if (!string.IsNullOrEmpty(criteria.Product_Code))
                {
                    products = products.Where(w => w.Product_Code == criteria.Product_Code || w.Product_Name.Contains(criteria.Product_Code));
                }

                if (!string.IsNullOrEmpty(criteria.Product_Name))
                {
                    products = products.Where(w => w.Product_Name.Contains(criteria.Product_Name));
                }
                if (!string.IsNullOrEmpty(criteria.Record_Status))
                {
                    products = products.Where(w => w.Record_Status == criteria.Record_Status);
                }
                if (criteria.Category_1.HasValue && criteria.Category_1 > 0)
                {
                    products = products.Where(w => w.Product_Category_L1 == criteria.Category_1);
                }
                if (criteria.Category_2.HasValue && criteria.Category_2 > 0)
                {
                    products = products.Where(w => w.Product_Category_L2 == criteria.Category_2);
                }

                if (criteria.Category_3.HasValue && criteria.Category_3 > 0)
                {
                    products = products.Where(w => w.Product_Category_L3 == criteria.Category_3);
                }
                //I remember by sun
                if (criteria.User_Select != null && criteria.User_Select.Length > 0)
                {
                    products = products.Where(w => !criteria.User_Select.Contains(w.Product_ID));
                }

                if (criteria.Update_On.HasValue)
                {
                    products = products.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(criteria.Update_On.Value.Year, criteria.Update_On.Value.Month, criteria.Update_On.Value.Day, criteria.Update_On.Value.Hour, criteria.Update_On.Value.Minute, criteria.Update_On.Value.Second));
                }
                if (criteria.Branch_ID.HasValue)
                {
                    if (criteria.User_Authentication_ID.HasValue)
                    {
                        var isAdmin = false;
                        var roles =
                            db.User_Assign_Role.Where(w => w.User_Authentication_ID == criteria.User_Authentication_ID)
                                .Select(s => s.User_Role_ID);
                        if (roles != null)
                        {
                            var hasPageRole =
                                db.Page_Role.Where(
                                    w =>
                                        w.Page.Page_Url == "/POSConfig/ConfigurationAdmin" &&
                                        roles.Contains(w.User_Role_ID)).FirstOrDefault();
                            if (hasPageRole != null)
                            {
                                isAdmin = true;
                            }
                        }

                        if (isAdmin)
                        {
                            products = products.Where(w => w.Branch_ID == criteria.Branch_ID || w.Branch_ID == null);
                        }
                        else
                        {
                            products = products.Where(w => w.Branch_ID == criteria.Branch_ID);
                        }
                    }
                    else
                    {
                        products = products.Where(w => w.Branch_ID == criteria.Branch_ID);
                    }

                }

                if (!string.IsNullOrEmpty(criteria.Text_Search))
                {
                    int n; decimal sellPrice;
                    bool isNumeric = int.TryParse(criteria.Text_Search, out n);
                    if (isNumeric)
                    {
                        sellPrice = NumUtil.ParseDecimal(criteria.Text_Search);
                        products = products.Where(w => w.Product_Name.Contains(criteria.Text_Search) || w.Product_Code.Contains(criteria.Text_Search) || w.Selling_Price.Value == sellPrice);
                    }
                    else
                    {
                        products = products.Where(w => w.Product_Name.Contains(criteria.Text_Search) || w.Product_Code.Contains(criteria.Text_Search));
                    }
                }

                result.Record_Count = products.Count();
                //Added by Nay on 28-Jul-2015
                criteria.Top = products.Count();

                if (!string.IsNullOrEmpty(criteria.Sort_By))
                {
                    if (criteria.Sort_By == "code")
                    {
                        products = products.OrderBy(o => o.Product_Code);
                    }
                    else if (criteria.Sort_By == "price")
                    {
                        products = products.OrderBy(o => o.Selling_Price);
                    }
                    else if (criteria.Sort_By == "name")
                    {
                        products = products.OrderBy(o => o.Product_Name);
                    }
                }
                else
                {
                    if (criteria.Random)
                    {
                        products = products.OrderBy(o => Guid.NewGuid());
                    }
                    else
                    {
                        products = products.OrderByDescending(o => o.Product_ID);
                    }
                }


                if (criteria.Top.HasValue)
                {
                    products = products.Take(criteria.Top.Value);
                }
                else
                {
                    products = products.Skip(criteria.Start_Index).Take(criteria.Page_Size);

                }

                var obj = new List<Product>();
                obj = products.ToList();
                if (criteria.hasBlank)
                {
                    obj.Insert(0, new Product() { Company_ID = criteria.Company_ID, Product_ID = 0, Product_Name = "Other" });
                }

                result.Object = obj;
                result.Start_Index = criteria.Start_Index;
                result.Page_Size = criteria.Page_Size;

                return result;

            }
        }



        public _Attribute GetProductAndAttribute(Nullable<int> pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                var product = db.Products
                    .Where(w => w.Product_ID == pProductID)
                    .FirstOrDefault();
                if (product != null)
                {
                    var attr = new _Attribute();
                    attr.Product_ID = product.Product_ID;
                    attr.Product_Code = product.Product_Code;
                    attr.Product_Name = product.Product_Name;
                    attr.Selling_Price = product.Selling_Price;
                    attr.Product_Attribute = product.Product_Attribute.ToList();

                    var i = 0;
                    foreach (var a in product.Product_Attribute)
                    {
                        attr.Product_Attribute[i].Product_Attribute_Value = a.Product_Attribute_Value.ToList();
                        i++;
                    }
                    var image = product.Product_Image.Where(w => w.Is_Main == true).FirstOrDefault();
                    if (image != null)
                    {
                        attr.Image = image.Image;
                    }
                    //attr.Product_Image = product.Product_Image.ToList();
                    //attr.Product_Attribute_Map = product.Product_Attribute_Map.ToList();
                    //attr.Product_Attribute_Map_Price = product.Product_Attribute_Map_Price.ToList();
                    //attr.Product_Image_Attribute = product.Product_Image_Attribute.ToList();
                    //attr.Product_Price = product.Product_Price.ToList();
                    return attr;
                }
            }
            return null;
        }
        public List<Product_Attribute_Map> GetAttributeMap(int pProductID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Product_Attribute_Map
                    .Include(i => i.Product_Attribute_Value)
                    .Include(i => i.Product_Attribute_Value1)
                    .Include(i => i.Product_Attribute_Value2)
                    .Include(i => i.Product_Attribute_Value3)
                    .Include(i => i.Product_Attribute_Value4)
                    .Include(i => i.Product_Attribute_Map_Price)
                    .Where(w =>
                    (w.Product_Attribute_Value.Product_Attribute.Product_ID ?? 0) == pProductID
                    || (w.Product_Attribute_Value1.Product_Attribute.Product_ID ?? 0) == pProductID
                    || (w.Product_Attribute_Value2.Product_Attribute.Product_ID ?? 0) == pProductID
                    || (w.Product_Attribute_Value3.Product_Attribute.Product_ID ?? 0) == pProductID
                    || (w.Product_Attribute_Value4.Product_Attribute.Product_ID ?? 0) == pProductID)
                    .Distinct()
                    .ToList();
            }
        }

        public ProductImageDetail GetImageDetail(Guid imageId)
        {
            using (var db = new SBS2DBContext())
            {
                var image = db.Product_Image
                    .Include(i => i.Product_Image_Attribute)
                    .FirstOrDefault(f => f.Product_Image_ID == imageId);
                if (image == null) return null;

                var model = new ProductImageDetail();
                model.ProductImage = image;
                var attrMapImgs = image.Product_Image_Attribute.ToList();
                var attrNames = attrMapImgs.Select(s => CreateAttributeMapName(s.Product_Attribute_Map)).ToList();
                var selectedList = new List<bool>();
                //var attrMaps = db.Product_Attribute_Map.Where(w =>
                //    w.Product_Attribute_Value.Product_Attribute.Product_ID == image.Product_ID
                //    || w.Product_Attribute_Value1.Product_Attribute.Product_ID == image.Product_ID
                //    || w.Product_Attribute_Value2.Product_Attribute.Product_ID == image.Product_ID
                //    || w.Product_Attribute_Value3.Product_Attribute.Product_ID == image.Product_ID
                //    || w.Product_Attribute_Value4.Product_Attribute.Product_ID == image.Product_ID)
                //    .ToList();
                //if (attrMaps.Count > 0)
                //{
                //    for (int i = 0; i < attrMaps.Count; i++)
                //    {
                //        attrNames.Add(CreateAttributeMapName(attrMaps[i]));
                //        selectedList.Add(attrMapImgs.FirstOrDefault(f => f.Map_ID == attrMaps[i].Map_ID) != null);
                //    }
                //    model.AttributeMapNames = attrNames.ToArray();
                //    model.Selected = selectedList.ToArray();
                //}
                model.AttributeMapNames = attrNames.ToArray();
                return model;
            }

        }

        private static string CreateAttributeMapName(Product_Attribute_Map attrMap)
        {
            string attrMapName = string.Empty;
            if (attrMap.Product_Attribute_Value != null)
            {
                attrMapName = attrMap.Product_Attribute_Value.Attribute_Value;
            }

            if (attrMap.Product_Attribute_Value1 != null)
            {
                if (!string.IsNullOrEmpty(attrMapName)) attrMapName += " - ";
                attrMapName += attrMap.Product_Attribute_Value1.Attribute_Value;
            }

            if (attrMap.Product_Attribute_Value2 != null)
            {
                if (!string.IsNullOrEmpty(attrMapName)) attrMapName += " - ";
                attrMapName += attrMap.Product_Attribute_Value2.Attribute_Value;
            }

            if (attrMap.Product_Attribute_Value3 != null)
            {
                if (!string.IsNullOrEmpty(attrMapName)) attrMapName += " - ";
                attrMapName += attrMap.Product_Attribute_Value3.Attribute_Value;
            }

            if (attrMap.Product_Attribute_Value4 != null)
            {
                if (!string.IsNullOrEmpty(attrMapName)) attrMapName += " - ";
                attrMapName += attrMap.Product_Attribute_Value4.Attribute_Value;
            }
            return attrMapName;
        }
    }


    public class Quantity_On_Hand
    {
        public Nullable<int> Product_ID { get; set; }
        public decimal Quantity { get; set; }
        public Nullable<int> Location_ID { get; set; }
        public Inventory_Location Location { get; set; }

    }

    public class Product_Vendor
    {
        public int Vendor_ID { get; set; }
        public int Product_ID { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public string Vendor_Name { get; set; }
        public decimal Vendor_Price { get; set; }
    }

    public partial class Product_Details_
    {
        public Nullable<int> Company_ID { get; set; }
        public bool Validate { get; set; }
        public string ErrMsg { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string UoM { get; set; }
        public int Qty { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Vendor { get; set; }
    }

    public class AttributeMapImageViewModel
    {
        public int ImageIndex { get; set; }
        public string[] AttributeNames { get; set; }
        public int[] AttributeMapIds { get; set; }
        public bool[] Selected { get; set; }

    }

    public class AttributeMapPriceViewModel
    {
        public bool? RecordStatus { get; set; }
        public string AttributeValue0 { get; set; }
        public string AttributeValue1 { get; set; }
        public string AttributeValue2 { get; set; }
        public string AttributeValue3 { get; set; }
        public string AttributeValue4 { get; set; }
        public decimal? CostingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? Price0 { get; set; }
        public decimal? Price1 { get; set; }
        public decimal? Price2 { get; set; }
        public int? PriceId0 { get; set; }
        public int? PriceId1 { get; set; }
        public int? PriceId2 { get; set; }
        public int? AttributeMapId { get; set; }


    }

    public class ProductImageDetail
    {
        public Product_Image ProductImage { get; set; }
        public bool[] Selected { get; set; }
        public string[] AttributeMapNames { get; set; }
    }
}