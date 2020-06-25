using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using HR.Common;
using System.ComponentModel.DataAnnotations;
using SBSModel.Common;

namespace HR
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
           
        }

        public class DecimalModelBinder : IModelBinder
        {
           public object BindModel(ControllerContext controllerContext,ModelBindingContext bindingContext)
           {
              ValueProviderResult valueResult = bindingContext.ValueProvider .GetValue(bindingContext.ModelName);
              if (valueResult == null)
                 return null;
              if (string.IsNullOrEmpty(valueResult.AttemptedValue))
                 return null;
              ModelState modelState = new ModelState { Value = valueResult };
              object actualValue = null;
              try
              {
                 actualValue = Convert.ToDecimal(valueResult.AttemptedValue,CultureInfo.CurrentCulture);
              }
              catch (FormatException e)
              {
                 modelState.Errors.Add(e);
              }

              bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
              return actualValue;
           }
        }
      
        
        //public class DecimalModelBinder : IModelBinder
        //{
        //    public object BindModel(ControllerContext controllerContext,ModelBindingContext bindingContext)
        //    {
        //        ValueProviderResult valueResult = bindingContext.ValueProvider
        //            .GetValue(bindingContext.ModelName);
        //        ModelState modelState = new ModelState { Value = valueResult };
        //        object actualValue = null;
        //        try
        //        {
        //            actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
        //                CultureInfo.CurrentCulture);
        //        }
        //        catch (FormatException e)
        //        {
        //            modelState.Errors.Add(e);
        //        }

        //        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
        //        return actualValue;
        //    }
        //}
    }
}
