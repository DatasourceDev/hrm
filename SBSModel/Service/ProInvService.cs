using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;
using System.Data.Entity;
using SBSModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    public class InventoryConfigurationService
    {
        //Inventory_Location
        public List<Inventory_Location> getInventoryLocations(int Company_ID)
        {
            List<Inventory_Location> location = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    location = db.Inventory_Location.Where(w => w.Company_ID == Company_ID).ToList();
                }
            }
            catch
            {

            }

            return location;
        }

        public Inventory_Location getInventoryLocation(int Inventory_Location_ID)
        {
            Inventory_Location location = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    location = db.Inventory_Location.Where(w => w.Inventory_Location_ID == Inventory_Location_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return location;
        }

        public int insertInventoryLocation(Inventory_Location location)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(location).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateInventoryLocation(Inventory_Location location)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(location).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteInventoryLocation(Inventory_Location location)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(location).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public bool inventoryLocationExists(string locationName, int company_ID, ref int location_ID) {
            bool result = false;
            try {
                using (var db = new SBS2DBContext()) {
                    var location = db.Inventory_Location.SingleOrDefault(x => x.Name == locationName && x.Company_ID == company_ID);

                    if (location != null) {
                        location_ID = location.Inventory_Location_ID;
                        result = true;
                    } else {
                        result = false;
                    }
                }
            } catch {
                result = false;
            }

            return result;
        }

        //Inventory_Preferences
        public Inventory_Preferences getInventoryPreferences(int Company_ID)
        {
            Inventory_Preferences pref = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    pref = db.Inventory_Preferences.Where(w => w.Company_ID == Company_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return pref;
        }

        public int insertInventoryPreferences(Inventory_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateInventoryPreferences(Inventory_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Product_Preferences

        public Product_Preferences getProductPreferences(int Company_ID)
        {
            Product_Preferences pref = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    pref = db.Product_Preferences.Where(w => w.Company_ID == Company_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return pref;
        }

        public int insertProductPreferences(Product_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateProductPreferences(Product_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }


        //Pricing_Preferences

        public Pricing_Preferences getPricingPreferences(int Company_ID)
        {
            Pricing_Preferences pref = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    pref = db.Pricing_Preferences.Where(w => w.Company_ID == Company_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return pref;
        }

        public int insertPricingPreferences(Pricing_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updatePricingPreferences(Pricing_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Tax_Preferences

        public Tax_Preferences getTaxPreferences(int Company_ID)
        {
            Tax_Preferences pref = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    pref = db.Tax_Preferences
                        .Include(i=>i.Tax_Scheme)
                        .Where(w => w.Company_ID == Company_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return pref;
        }

        public int insertTaxPreferences(Tax_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateTaxPreferences(Tax_Preferences pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Tax_Scheme

        public List<Tax_Scheme> getTaxSchemes(Nullable<int> Company_ID)
        {
            List<Tax_Scheme> pref = new List<Tax_Scheme>();

            try
            {
                using (var db = new SBS2DBContext())
                {
                    pref = db.Tax_Scheme.Where(w => w.Company_ID == Company_ID).ToList();
                }
            }
            catch
            {

            }

            return pref;
        }

        public Tax_Scheme getTaxScheme(int Tax_Scheme_ID)
        {
            Tax_Scheme pref = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    pref = db.Tax_Scheme.Where(w => w.Tax_Scheme_ID == Tax_Scheme_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return pref;
        }

        public int insertTaxScheme(Tax_Scheme pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateTaxScheme(Tax_Scheme pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteTaxScheme(Tax_Scheme pref)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(pref).State = EntityState.Deleted;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        //Location_Authorize

        public List<Location_Authorize> getLocationAuthorizes(int Company_ID)
        {
            List<Location_Authorize> loc = new List<Location_Authorize>();

            try
            {
                using (var db = new SBS2DBContext())
                {
                    loc = db.Location_Authorize
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.User_Profile)
                        .Where(w => w.Company_ID == Company_ID).ToList();
                }
            }
            catch
            {

            }

            return loc;
        }

        public Location_Authorize getLocationAuthorize(int Location_Authorize_ID)
        {
            Location_Authorize loc = null;

            try
            {
                using (var db = new SBS2DBContext())
                {
                    loc = db.Location_Authorize
                        .Include(i => i.Inventory_Location)
                        .Include(i => i.User_Profile)
                        .Where(w => w.Location_Authorize_ID == Location_Authorize_ID).FirstOrDefault();
                }
            }
            catch
            {

            }

            return loc;
        }

        public int insertLocationAuthorize(Location_Authorize loc)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(loc).State = EntityState.Added;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int updateLocationAuthorize(Location_Authorize loc)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(loc).State = EntityState.Modified;
                    db.SaveChanges();
                    return ERROR_CODE.SUCCESS;
                }
            }
            catch
            {
                return ERROR_CODE.ERROR_500_DB;
            }
        }

        public int deleteLocationAuthorize(Location_Authorize loc)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    db.Entry(loc).State = EntityState.Deleted;
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
}
