using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authentication.Common
{
    public class NumUtil
    {
        public static decimal ParseDecimal(string s, decimal def = 0)
        {
            decimal d = 0;
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    return Convert.ToDecimal(s.Trim().Replace(",",""));
                }
                catch
                {
                    return d;
                }
            }
            return d;
        }

        public static int ParseInteger(string s, int def = 0)
        {
            int d = 0;
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    return Convert.ToInt32(s.Trim().Replace(",", ""));
                }
                catch
                {
                    return d;
                }
            }
            return d;
        }
    }
}