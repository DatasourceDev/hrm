using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;

namespace HR.Controllers
{
   public class UploadController : ControllerBase
   {

      public ActionResult UserProfilePhotoUpload(Nullable<int> pProfileID)
      {
         if (pProfileID.HasValue)
         {
            HttpPostedFileBase file = Request.Files[0];
            var photo = new User_Profile_Photo();
            int fileSizeInBytes = file.ContentLength;
            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            photo.Photo = data;

            UserService userservice = new UserService();
            userservice.updateUserProfilePhoto(pProfileID.Value, data);

            var base64 = Convert.ToBase64String(data);
            var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
            return Json(imgSrc, JsonRequestBehavior.AllowGet);
         }

         return Json("", JsonRequestBehavior.AllowGet);
      }

      //Added by sun 30-06-2016
      public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
      {
         using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(byteArrayIn))
         {
            return System.Drawing.Image.FromStream(mStream);
         }
      }

      public ActionResult UploadCompanyLogo(Nullable<int> pCompanyID)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var currentdate = StoredProcedure.GetCurrentDate();

         HttpPostedFileBase file = Request.Files[0];
         //var photo = new Product_Profile_Photo();

         int fileSizeInBytes = file.ContentLength;
         MemoryStream target = new MemoryStream();
         file.InputStream.CopyTo(target);
         byte[] data = target.ToArray();

         System.Drawing.Image image = byteArrayToImage(data);
         int thumbnailSize = 150;
         int newWidth = 0;
         int newHeight = 0;
         if (image.Width > image.Height)
         {
            newWidth = thumbnailSize;
            newHeight = (int)(image.Height * thumbnailSize / image.Width);
         }
         else
         {
            newWidth = (int)(image.Width * thumbnailSize / image.Height);
            newHeight = thumbnailSize;
         }

         var thumbnailBitmap = new System.Drawing.Bitmap(newWidth, newHeight);

         var thumbnailGraph = System.Drawing.Graphics.FromImage(thumbnailBitmap);
         thumbnailGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
         thumbnailGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
         thumbnailGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

         var imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
         thumbnailGraph.DrawImage(image, imageRectangle);

         //thumbnailBitmap.Save(outputPath & ".jpg", image.RawFormat);
         System.IO.MemoryStream ms = new System.IO.MemoryStream();
         thumbnailBitmap.Save(ms, image.RawFormat);

         data = ms.ToArray();

         CompanyService comService = new CompanyService();
         if (pCompanyID.HasValue && pCompanyID.Value > 0)
         {
            Company_Logo logo = new Company_Logo();
            logo = comService.GetLogo(pCompanyID);
            if (logo != null)
            {
               logo.Logo = data;
               logo.Update_By = userlogin.User_Authentication.Email_Address;
               logo.Update_On = currentdate;
               comService.UpdateCompanyLogo(logo);
            }
            else
            {

               logo = new Company_Logo(); 
               logo.Company_ID = pCompanyID.Value;
               logo.Logo = data;
               logo.Create_By = userlogin.User_Authentication.Email_Address;
               logo.Create_On = currentdate;
               logo.Update_By = userlogin.User_Authentication.Email_Address;
               logo.Update_On = currentdate;
               comService.InsertCompanyLogo(logo);
            }
         }
         var base64 = Convert.ToBase64String(data);
         var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
         return Json(new { img = imgSrc, imgByte = data }, JsonRequestBehavior.AllowGet);
      }
   }
}