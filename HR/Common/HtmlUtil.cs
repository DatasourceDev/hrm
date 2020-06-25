using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;


namespace SBSModel.Common
{
   public class HtmlUtil
   {
      public static MvcHtmlString InitJS()
      {
         StringBuilder builder = new StringBuilder();
         using (StringWriter writer = new StringWriter(builder))
         {
            using (HtmlTextWriter html = new HtmlTextWriter(writer))
            {
               GenScript(html, "/Scripts/jquery-2.1.4.min.js");
               GenScript(html, "/Scripts/jquery-ui-1.11.4.min.js");
               GenScript(html, "/Scripts/jquery.validate.min.js");
               GenScript(html, "/Scripts/modernizr-2.6.2.js");
               GenScript(html, "/Scripts/respond.min.js");
               GenScript(html, "/Scripts/app-validate.js");
               GenScript(html, "/Scripts/app-control.js");
               GenScript(html, "/assets/js/vendor/bootstrap/bootstrap.min.js");
               GenScript(html, "/assets/js/vendor/bootstrap/bootstrap-dropdown-multilevel.js");
               GenScript(html, "/assets/js/vendor/mmenu/js/jquery.mmenu.min.js");
               GenScript(html, "/assets/js/vendor/sparkline/jquery.sparkline.min.js");
               GenScript(html, "/assets/js/vendor/nicescroll/jquery.nicescroll.js");
               GenScript(html, "/assets/js/vendor/animate-numbers/jquery.animateNumbers.js");
               GenScript(html, "/assets/js/vendor/chosen/chosen.jquery.min.js");
               GenScript(html, "/assets/js/vendor/blockui/jquery.blockUI.js");
               GenScript(html, "/assets/js/minimal.min.js");
               GenScript(html, "/assets/js/vendor/datatables/jquery.dataTables.min.js");
               GenScript(html, "/assets/js/vendor/datatables/ColReorderWithResize.js");
               GenScript(html, "/assets/js/vendor/datatables/colvis/dataTables.colVis.min.js");
               GenScript(html, "/assets/js/vendor/datatables/tabletools/ZeroClipboard.js");
               GenScript(html, "/assets/js/vendor/datatables/tabletools/dataTables.tableTools.min.js");
               GenScript(html, "/assets/js/vendor/datatables/dataTables.bootstrap.js");
               GenScript(html, "/assets/js/vendor/momentjs/moment-with-langs.min.js");
               GenScript(html, "/assets/js/vendor/datepicker/bootstrap-datetimepicker.min.js");
               GenScript(html, "/assets/ckeditor/ckeditor.js");
               GenScript(html, "/assets/js/vendor/modals/classie.js");
               GenScript(html, "/assets/js/vendor/modals/modalEffects.js");
               GenScript(html, "/assets/js/vendor/modals/cssParser.js");
               GenScript(html, "/Scripts/html2canvas.js");
               GenScript(html, "/Scripts/bootstrap-tour-0.11.0/js/bootstrap-tour.js");
               GenScript(html, "/Scripts/bootstrap-tour-0.11.0/js/bootstrap-tour.min.js");

            }
         }
         return MvcHtmlString.Create(builder.ToString());
      }

      public static void GenScript(HtmlTextWriter html, string jsfile)
      {
         //html.AddAttribute(HtmlTextWriterAttribute.Src, AppSetting.SERVER_NAME + AppSetting.SBSTmpAPI + jsfile);
         html.AddAttribute(HtmlTextWriterAttribute.Src, VirtualPathUtility.ToAbsolute("~" + jsfile) + "?v=0");
         html.RenderBeginTag(HtmlTextWriterTag.Script);
         html.RenderEndTag();
      }


      public static MvcHtmlString InitCSS()
      {
         StringBuilder builder = new StringBuilder();
         using (StringWriter writer = new StringWriter(builder))
         {
            using (HtmlTextWriter html = new HtmlTextWriter(writer))
            {
               GenCss(html, "/assets/css/app.css");
               GenCss(html, "/assets/css/vendor/bootstrap/bootstrap.css");
               GenCss(html, "/assets/css/vendor/animate/animate.min.css");
               GenCss(html, "/assets/css/vendor/bootstrap-checkbox.css");
               GenCss(html, "/assets/css/vendor/bootstrap/bootstrap-dropdown-multilevel.css");
               GenCss(html, "/assets/css/minimal.css");
               GenCss(html, "/assets/js/vendor/chosen/css/chosen.min.css");
               GenCss(html, "/assets/js/vendor/chosen/css/chosen-bootstrap.css");
               GenCss(html, "/assets/js/vendor/datatables/css/dataTables.bootstrap.css");
               GenCss(html, "/assets/js/vendor/datatables/css/ColVis.css");
               GenCss(html, "/assets/js/vendor/datatables/css/TableTools.css");
               GenCss(html, "/assets/js/vendor/datepicker/css/bootstrap-datetimepicker.css");
               GenCss(html, "/assets/js/vendor/modals/css/component.css");
               GenCss(html, "/assets/font-awesome/css/font-awesome.min.css");
               GenCss(html, "/Scripts/bootstrap-tour-0.11.0/css/bootstrap-tour.css");
               GenCss(html, "/Scripts/bootstrap-tour-0.11.0/css/bootstrap-tour.min.css");

            }
         }
         return MvcHtmlString.Create(builder.ToString());
      }

      public static void GenCss(HtmlTextWriter html, string cssfile, bool media = false)
      {
         if (media)
            html.AddAttribute("media", "all");

         html.AddAttribute(HtmlTextWriterAttribute.Href, VirtualPathUtility.ToAbsolute("~" + cssfile) + "?v=0");
         //html.AddAttribute(HtmlTextWriterAttribute.Href, AppSetting.SERVER_NAME + AppSetting.SBSTmpAPI + cssfile);
         html.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
         html.RenderBeginTag(HtmlTextWriterTag.Link);
         html.RenderEndTag();
      }



      public static MvcHtmlString InitFrontJS()
      {
         StringBuilder builder = new StringBuilder();
         using (StringWriter writer = new StringWriter(builder))
         {
            using (HtmlTextWriter html = new HtmlTextWriter(writer))
            {
               GenScript(html, "/Scripts/jquery-2.1.4.min.js");
               GenScript(html, "/Scripts/jquery-ui-1.11.4.min.js");
               GenScript(html, "/Scripts/app-validate.js");
               //GenScript(html, "/Scripts/app-control.js");
               GenScript(html, "/Scripts/lightbox/lightbox.js");

               GenScript(html, "/assets/js/vendor/chosen/chosen.jquery.min.js");
               GenScript(html, "/assets/js/vendor/datatables/jquery.dataTables.min.js");
               GenScript(html, "/assets/js/vendor/datatables/ColReorderWithResize.js");
               GenScript(html, "/assets/js/vendor/datatables/colvis/dataTables.colVis.min.js");
               GenScript(html, "/assets/js/vendor/datatables/tabletools/ZeroClipboard.js");
               GenScript(html, "/assets/js/vendor/datatables/tabletools/dataTables.tableTools.min.js");
               GenScript(html, "/assets/js/vendor/datatables/dataTables.bootstrap.js");
               GenScript(html, "/assets/js/vendor/datatables/dataTables.bootstrap.js");

               GenScript(html, "/frontpage/assets/js/vendor/skrollr/skrollr.min.js");
               GenScript(html, "/frontpage/assets/js/vendor/modernizr-2.6.2-respond-1.1.0.min.js");
               GenScript(html, "/frontpage/assets/js/minimal.min.js");

               //GenScript(html, "/frontpage/assets/js/vendor/text-rotator/jquery.simple-text-rotator.js");
               //GenScript(html, "/frontpage/assets/js/vendor/owl-carousel/owl.carousel.min.js");
               //GenScript(html, "/frontpage/assets/js/vendor/nivo-lightbox/nivo-lightbox.min.js");
               //GenScript(html, "/frontpage/assets/js/vendor/tweet-js/jquery.tweet.min.js");
               //GenScript(html, "/frontpage/assets/js/vendor/vticker/jquery.vticker.min.js");
               //GenScript(html, "/frontpage/assets/js/vendor/jflickrfeed/jflickrfeed.min.js");
               //GenScript(html, "/frontpage/assets/js/vendor/appear/jquery.appear.js");
               //GenScript(html, "/frontpage/assets/js/vendor/parallax/jquery.parallax-1.1.3.js");
            }
         }
         return MvcHtmlString.Create(builder.ToString());
      }

      public static MvcHtmlString InitFrontCSS()
      {
         StringBuilder builder = new StringBuilder();
         using (StringWriter writer = new StringWriter(builder))
         {
            using (HtmlTextWriter html = new HtmlTextWriter(writer))
            {
               GenCss(html, "/assets/font-awesome/css/font-awesome.min.css");
               GenCss(html, "/assets/js/vendor/datatables/css/dataTables.bootstrap.css");
               GenCss(html, "/assets/js/vendor/datatables/css/ColVis.css");
               GenCss(html, "/assets/js/vendor/datatables/css/TableTools.css");
               GenCss(html, "/assets/js/vendor/chosen/css/chosen.min.css");
               GenCss(html, "/assets/js/vendor/chosen/css/chosen-bootstrap.css");

               GenCss(html, "/frontpage/assets/css/vendor/bootstrap/bootstrap.min.css");    
               GenCss(html, "/frontpage/assets/css/vendor/animate/animate.css");
               GenCss(html, "/frontpage/assets/css/vendor/bootstrap-checkbox.css");
               GenCss(html, "/frontpage/assets/js/vendor/text-rotator/css/simpletextrotator.css");
               GenCss(html, "/frontpage/assets/js/vendor/owl-carousel/css/owl.carousel.css");
               GenCss(html, "/frontpage/assets/js/vendor/owl-carousel/css/owl.theme.css");
               GenCss(html, "/frontpage/assets/js/vendor/owl-carousel/css/owl.transitions.css");
               GenCss(html, "/frontpage/assets/js/vendor/nivo-lightbox/css/nivo-lightbox.css");
               GenCss(html, "/frontpage/assets/js/vendor/nivo-lightbox/css/themes/default/default.css");

               //CUSTOM STYLE
               GenCss(html, "/frontpage/assets/css/minimal.css");

               GenCss(html, "/frontpage/assets/css/css/lightbox.min.css");
               GenCss(html, "/frontpage/assets/css/css/slick.css");
               GenCss(html, "/frontpage/assets/css/css/slick-theme.css");


            }
         }
         return MvcHtmlString.Create(builder.ToString());
      }
   }
}
