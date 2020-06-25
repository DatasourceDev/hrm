using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using SBSResourceAPI;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SBSModel.Common
{
    public class LocalizedRequired : RequiredAttribute
    {
        public LocalizedRequired([CallerMemberName] string displayNameKey = null, Type resourceType = null)
        {
            if (resourceType == null)
                resourceType = typeof(SBSResourceAPI.Resource);

            this.ErrorMessageResourceName = "Message_Is_Required";
            this.ErrorMessageResourceType = resourceType;
        }
    }

    public class LocalizedValidationEmail : RegularExpressionAttribute
    {
        public LocalizedValidationEmail([CallerMemberName] string displayNameKey = null, Type resourceType = null)
            : base(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}")
        {
            if (resourceType == null)
                resourceType = typeof(SBSResourceAPI.Resource);

            this.ErrorMessageResourceName = "Message_Is_Invalid";
            this.ErrorMessageResourceType = resourceType;
        }
    }
    //Added by Moet on 3/Aug/2016
    public class LocalizedValidationUserName : RegularExpressionAttribute
    {
        public LocalizedValidationUserName([CallerMemberName] string displayNameKey = null, Type resourceType = null)
            : base(@"^[a-zA-Z0-9-_]+$")
        {
            if (resourceType == null)
                resourceType = typeof(SBSResourceAPI.Resource);

            this.ErrorMessageResourceName = "Message_Is_Invalid";
            this.ErrorMessageResourceType = resourceType;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class LocalizedValidDate : ValidationAttribute
    {
        public LocalizedValidDate()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                Type resourceType = typeof(SBSResourceAPI.Resource);
                if (value != null)
                {
                    DateTime date;
                    if (value is string)
                    {
                        var tempdate = DateUtil.ToDate(value.ToString(), "/");
                        if (tempdate == null )
                            return new ValidationResult(Resource.Message_Is_Invalid);

                        if (!DateTime.TryParse(tempdate.Value.ToString(), out date))
                        {
                            //this.ErrorMessageResourceName = "Message_Is_Invalid";
                            //this.ErrorMessageResourceType = resourceType;
                            return new ValidationResult(Resource.Message_Is_Invalid);
                        }
                    }
                    else
                        date = (DateTime)value;
                }
                return null;
            }
            catch
            {
                return new ValidationResult(Resource.Message_Is_Invalid); 
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizedValidDecimal : ValidationAttribute
    {
        public LocalizedValidDecimal()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value != null)
                {
                    Decimal num;
                    if (value is string)
                    {
                        if (!decimal.TryParse((string)value, out num))
                            return new ValidationResult(Resource.Message_Is_Invalid);
                    }
                    else
                        num = (Decimal)value;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizedValidInt : ValidationAttribute
    {
        public LocalizedValidInt()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value != null)
                {
                    int num;
                    if (value is string)
                    {
                        if (!int.TryParse((string)value, out num))
                            return new ValidationResult(Resource.Message_Is_Invalid);
                    }
                    else
                        num = (int)value;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class LocalizedValidMaxLength : ValidationAttribute
    {

        private readonly int maxLimit;
        public LocalizedValidMaxLength(int pmaxLimit)
            : base()
        {
            maxLimit = pmaxLimit;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value != null)
                {
                    int userValue = value.ToString().Count();
                    if (userValue <= maxLimit)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult(Resource.Message_Is_Maxmum_Length + " (" + maxLimit.ToString() + ") ");
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


    }

    public class LocalizedValidationPhone : RegularExpressionAttribute
    {
        public LocalizedValidationPhone([CallerMemberName] string displayNameKey = null, Type resourceType = null)
            : base(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")
        {
            if (resourceType == null)
                resourceType = typeof(SBSResourceAPI.Resource);

            this.ErrorMessageResourceName = "Message_Is_Invalid";
            this.ErrorMessageResourceType = resourceType;
        }
    }

    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly PropertyInfo nameProperty;


        public LocalizedDisplayNameAttribute([CallerMemberName] string displayNameKey = null, Type resourceType = null)
            : base(displayNameKey)
        {
            if (resourceType == null)
                resourceType = typeof(SBSResourceAPI.Resource);


            var q = (from a in resourceType.GetRuntimeProperties() where a.Name.Equals(displayNameKey) select a);
            if (q.ToList().Count > 0)
            {
                nameProperty = q.ToList().FirstOrDefault();
            }

        }

        public LocalizedDisplayNameAttribute(string displayNameKey)
            : base(displayNameKey)
        {
            Type resourceType = typeof(SBSResourceAPI.Resource);

            var q = (from a in resourceType.GetRuntimeProperties() where a.Name.Equals(displayNameKey) select a);
            if (q.ToList().Count > 0)
            {
                nameProperty = q.ToList().FirstOrDefault();
            }
        }


        public override string DisplayName
        {
            get
            {
                if (nameProperty == null)
                {
                    return base.DisplayName;
                }
                return (string)nameProperty.GetValue(nameProperty.DeclaringType, null);
            }
        }
    }

    //public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    //{
    //    private readonly PropertyInfo nameProperty;
    //    private readonly string displayName;



    //    public LocalizedDisplayNameAttribute(string dname)
    //        : base(dname)
    //    {

    //        displayName = dname;
    //    }

    //    public override string DisplayName
    //    {
    //        get
    //        {
    //            if (nameProperty == null)
    //            {
    //                if (!string.IsNullOrEmpty(displayName))
    //                {
    //                    return displayName;
    //                }
    //                return base.DisplayName;
    //            }
    //            return (string)nameProperty.GetValue(nameProperty.DeclaringType, null);
    //        }
    //    }
    //}

}