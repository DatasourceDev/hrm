using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Data.Entity;

namespace SBSModel.Models
{
    public class VendorService
    {

        public List<Vendor> getVendors(Nullable<int> Company_ID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Vendors.Where(w => w.Company_ID == Company_ID).ToList();
            }

        }

        public Vendor getVendor(Nullable<int> pVID)
        {
            Vendor vend = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    vend = db.Vendors
                        .Include(i => i.Purchase_Order)
                        .Include(i => i.Purchase_Order.Select(s => s.PO_Payment))
                        .Where(w => w.Vendor_ID == pVID).FirstOrDefault();
                }
            }
            catch
            {
            }

            return vend;
        }

        public Vendor getVendorByName(string Vendor_Name, Nullable<int> Vendor_ID = null)
        {
            Vendor vend = null;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var vens = db.Vendors
                        .Include(i => i.Purchase_Order);

                    if (Vendor_ID.HasValue)
                    {
                        vens = vens.Where(w => w.Name == Vendor_Name && w.Vendor_ID != Vendor_ID);
                    }
                    else
                    {
                        vens = vens.Where(w => w.Name == Vendor_Name);
                    }

                    vend = vens.FirstOrDefault();
                }
            }
            catch
            {
            }

            return vend;
        }

        public int insertVendor(Vendor vend)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vend).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateVendor(Vendor vend)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vend).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteVendor(Vendor vend)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(vend).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public bool VendorExists(string name, int Company_ID, ref int Vendor_ID)
        {
            bool result = false;
            try
            {
                using (var db = new SBS2DBContext())
                {
                    var vendor = db.Vendors.SingleOrDefault(x => x.Name.ToLower() == name.ToLower() && x.Company_ID == Company_ID);

                    if (vendor != null)
                    {
                        Vendor_ID = vendor.Vendor_ID;
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

    }



}

