using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace SBSModel.Common
{
   public class StrUtil
   {
      async public Task<string> RandomizeString(int length)
      {

         var random = new Random();
         await Task.Delay(500);
         string chars = await Task.FromResult<string>(shuffle("ABCDE09FGHIJ18KLMNO27PQRST36UVWXY45Z" + (DateTime.Now.Ticks).ToString()));

         return new string(Enumerable.Repeat(chars, length)
           .Select(s => s[random.Next(s.Length)]).ToArray());
      }

      private string shuffle(string str)
      {
         char[] array = str.ToCharArray();
         Random rng = new Random();
         int n = array.Length;
         while (n > 1)
         {
            n--;
            int k = rng.Next(n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
         }
         return new string(array);
      }

      public static bool IsValidEmail(string email)
      {
         try
         {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
         }
         catch
         {
            return false;
         }
      }

      public static string HtmlReplaceSymbol(string str)
      {
         try
         {
            //return System.Web.HttpUtility.UrlEncode(str);
            return str.Replace("\\", @"\u005c")
                      .Replace("\"", @"\u0022")
                      .Replace("'", @"\u0027")
                      .Replace("&", @"\u0026")
                      .Replace("<", @"\u003c")
                      .Replace(">", @"\u003e");
         }
         catch
         {
            return str;
         }
      }

      public static bool IsValidUserName(string str)
      {
         return Regex.IsMatch(str, "^[a-zA-Z0-9_]+$");
      }
   }
}