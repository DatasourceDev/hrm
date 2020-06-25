using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBSModel.Common
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
               return Convert.ToDecimal(s.Trim().Replace(",", ""));
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

      public static int ParseInteger(double dd, int def = 0)
      {
         int d = 0;
         try
         {
            return Convert.ToInt32(dd);
         }
         catch
         {
            return d;
         }
      }
      public static string FormatCurrency(Nullable<double> val, int digit = 2)
      {
         try
         {
            if (val.HasValue)
            {
               if (GetDoubleLength(val.Value) > 0)
               {
                  if (digit > 0)
                     return val.Value.ToString("n" + digit.ToString());
                  else
                     return val.Value.ToString("n2");
               }
               else
               {
                  if (digit > 0)
                     return val.Value.ToString("n" + digit.ToString());
                  else
                     return val.Value.ToString("n0");
               }

            }
            if (digit > 0)
               return "0.00";
            else
               return "0";
         }
         catch
         {
            if (digit > 0)
               return "0.00";
            else
               return "0";
         }
      }
      public static string FormatCurrencyExcel(Nullable<decimal> val, int digit = 0)
      {
         try
         {
            if (val.HasValue)
            {
               if (val.Value > 0)
                     return val.Value.ToString("n2");
            }
            return "";
         }
         catch
         {
            return "";
         }
      }
      public static string FormatPercenExcel(Nullable<decimal> val, int digit = 0)
      {
         try
         {
            if (val.HasValue)
            {
               if (val.Value > 0)
                  return val.Value.ToString("n2") + "%";
            }
            return "";
         }
         catch
         {
            return "";
         }
      }

      public static decimal Round(decimal? value)
      {
         if (value.HasValue)
         {
            return (decimal)Math.Round(value.Value, 2, MidpointRounding.AwayFromZero);
         }
         return 0;
      }
      public static string Comma(Nullable<decimal> val)
      {
         try
         {
            if (val.HasValue)
            {
               if (GetDecimalLength(val.Value) > 0)
                  return val.Value.ToString("n2");
               else
                  return val.Value.ToString("n0");
            }
            return "";
              
         }
         catch
         {
            return "";
         }
      }

      public static string FormatCurrency(Nullable<decimal> val, int digit = 0)
      {
         try
         {
            if (val.HasValue)
            {
               if (GetDecimalLength(val.Value) > 0)
               {
                  if (digit > 0)
                     return val.Value.ToString("n" + digit.ToString());
                  else
                     return val.Value.ToString("n2");
               }

               else
               {
                  if (digit > 0)
                     return val.Value.ToString("n" + digit.ToString());
                  else
                     return val.Value.ToString("n0");
               }
            }
            if (digit > 0)
               return "0.00";
            else
               return "0";
         }
         catch
         {
            if (digit > 0)
               return "0.00";
            else
               return "0";
         }
      }
      public static string FormatCurrency(Nullable<int> val)
      {
         try
         {
            if (val.HasValue)
               return val.Value.ToString("n0");

            return "0";
         }
         catch
         {
            return "0";
         }
      }

      public static int GetDoubleLength(double dValue)
      {

         var tempValue = dValue.ToString();
         int decimalLength = 0;
         if (tempValue.Contains('.') || tempValue.Contains(','))
         {
            char[] separator = new char[] { '.', ',' };
            string[] tempstring = tempValue.Split(separator);

            if (ParseInteger(tempstring[1]) > 0)
               decimalLength = tempstring[1].Length;
         }
         return decimalLength;
      }
      public static int GetDecimalLength(decimal dValue)
      {

         var tempValue = dValue.ToString();
         int decimalLength = 0;
         if (tempValue.Contains('.') || tempValue.Contains(','))
         {
            char[] separator = new char[] { '.', ',' };
            string[] tempstring = tempValue.Split(separator);

            if (ParseInteger(tempstring[1]) > 0)
               decimalLength = tempstring[1].Length;
         }
         return decimalLength;
      }

      public static int GetDecimalLength(string dValue)
      {
         var tempValue = dValue;
         int decimalLength = 0;
         if (tempValue.Contains('.') || tempValue.Contains(','))
         {
            char[] separator = new char[] { '.', ',' };
            string[] tempstring = tempValue.Split(separator);

            if (ParseInteger(tempstring[1]) > 0)
               decimalLength = tempstring[1].Length;
         }
         return decimalLength;
      }
   }
}