using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Common
{
    public class ObjectUtil
    {
        public static Object BindDefault(object obj, bool allownull = false)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                var value = prop.GetValue(obj, null);
                var type = prop.PropertyType;
                var name = prop.Name;
                if (type.FullName.Contains("Int32"))
                {
                    if (allownull)
                    {
                        if (type.FullName.Contains("Nullable"))
                        {
                            if (value != null && (int)value == 0) prop.SetValue(obj, null, null);
                        }
                    }
                    else
                    {
                        if (value == null) prop.SetValue(obj, 0, null);
                    }
                }
                else if (type.FullName.Contains("Decimal"))
                {
                    if (allownull)
                    {
                        if (type.FullName.Contains("Nullable"))
                        {
                            if (value != null && (decimal)value == 0) prop.SetValue(obj, null, null);
                        }
                    }
                    else
                    {
                        if (value == null) prop.SetValue(obj, 0M, null);
                    }

                }
                else if (type.Name == "String")
                {
                    if (value == null) prop.SetValue(obj, "", null);
                    if (value != null && value.ToString() == "(null)") prop.SetValue(obj, "", null);
                }
                else if (type.FullName.Contains("Boolean"))
                {
                    if (value == null) prop.SetValue(obj, false, null);
                }
              
            }

            return obj;
        }


    }
}
