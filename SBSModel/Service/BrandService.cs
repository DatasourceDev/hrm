using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace SBSModel.Models
{
    public class BrandService
    {
        public List<Brand> LstBrand(Nullable<int> pCompanyId,  Nullable<DateTime> pUpdateOn = null)
        {

            using (var db = new SBS2DBContext())
            {
                var Brands = db.Brands.Where(w => w.Company_ID == pCompanyId);
                if (pUpdateOn.HasValue)
                {
                    Brands = Brands.Where(w => EntityFunctions.CreateDateTime(w.Update_On.Value.Year, w.Update_On.Value.Month, w.Update_On.Value.Day, w.Update_On.Value.Hour, w.Update_On.Value.Minute, w.Update_On.Value.Second) > EntityFunctions.CreateDateTime(pUpdateOn.Value.Year, pUpdateOn.Value.Month, pUpdateOn.Value.Day, pUpdateOn.Value.Hour, pUpdateOn.Value.Minute, pUpdateOn.Value.Second));
                }
                return Brands.ToList();
            }
        }
        public Brand GetBrand(Nullable<int> pBrandID)
        {
            using (var db = new SBS2DBContext())
            {
                return db.Brands.Where(w => w.Brand_ID == pBrandID).FirstOrDefault();
            }
        }
        public ServiceResult UpdateBrand(Brand pBrand)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBrand != null && pBrand.Brand_ID > 0 && pBrand.Brand_ID > 0)
                    {
                        var current = (from a in db.Brands
                                       where a.Brand_ID == pBrand.Brand_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            db.Entry(current).CurrentValues.SetValues(pBrand);
                            db.SaveChanges();
                        }

                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = "Brand" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = "Brand" };
            }

        }
        public ServiceResult InsertBrand(Brand pBrand)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pBrand != null)
                    {
                        //Insert                        
                        db.Brands.Add(pBrand);
                        db.SaveChanges();
                    }
                    return new ServiceResult { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = "Brand" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = "Brand" };
            }

        }
        public ServiceResult DeleteBrand(Nullable<int> pBrandID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {

                    var current = (from a in db.Brands where a.Brand_ID == pBrandID select a).FirstOrDefault();
                    if (current != null)
                    {
                        db.Brands.Remove(current);
                        db.SaveChanges();
                    }


                    return new ServiceResult { Code = ERROR_CODE.SUCCESS_DELETE, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = "Brand" };
                }
            }
            catch
            {
                return new ServiceResult() { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = "Brand" };
            }

        }


   

    }
}
