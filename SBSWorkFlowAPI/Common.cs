using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace SBSWorkFlowAPI {
    static class Common {

        public static Tdest DeserializeObject<Tdest>(string json) {
            
            using (var sr = new StringReader(json)){
                using (var jr = new JsonTextReader(sr)) {
                    var js = new JsonSerializer();
                    var u = js.Deserialize<Tdest>(jr);

                    if (u != null)
                        return u;
                    else
                        return default(Tdest);
                }
            }

        }

        private static List<Tdest> DeserializeList<Tdest, T>(string json) {
            var results = JObject.Parse(json);
            var list = new List<Tdest>();

            using (var sr = new StringReader(json)) {
                using (var jr = new JsonTextReader(sr)) {
                    var js = new JsonSerializer();
                    var u = js.Deserialize<T>(jr);

                    //if (u != null) {
                    //    list.Add(u);
                    //}
                }
            }


            return list;
        }

        private static void createObject<T>(ref T dest, JToken token) {
            dest = Activator.CreateInstance<T>();
            Type t = dest.GetType();

            if (token.HasValues) {

                foreach (var prop in t.GetProperties()) {
                    switch (prop.PropertyType.Name.ToLower()) {
                        case "int32":
                            prop.SetValue(dest, (int)token[prop.Name]);
                            break;
                        case "double":
                            prop.SetValue(dest, (double)token[prop.Name]);
                            break;
                        case "decimal":
                            prop.SetValue(dest, (decimal)token[prop.Name]);
                            break;
                        case "string":
                            prop.SetValue(dest, (string)token[prop.Name]);
                            break;
                        case "guid":
                            if ((string)token[prop.Name] != null)
                                prop.SetValue(dest, new Guid((string)token[prop.Name]));
                            else
                                prop.SetValue(dest, null);
                            break;
                        case "boolean":
                            prop.SetValue(dest, (bool)token[prop.Name]);
                            break;
                        case "byte[]":
                            if (token[prop.Name] != null)
                                prop.SetValue(dest, token[prop.Name].ToString().Trim('[', ']').Split(',')
                                    .Select(x => byte.Parse(x))
                                    .ToArray());
                            break;
                        case "timespan":
                            if (token[prop.Name].HasValues)
                                prop.SetValue(dest, TimeSpan.Parse((string)token[prop.Name]));
                            else
                                prop.SetValue(dest, null);
                            break;
                        //case "product_profile_photo":
                        //    Product_Profile_Photo px = null;
                        //    createObject<Product_Profile_Photo>(ref px, token["Product_Profile_Photo"]);
                        //    if (px != null)
                        //        prop.SetValue(dest, px);
                        //    break;

                        case "nullable`1":

                            if (prop.PropertyType.FullName.Contains("Int32")) {
                                prop.SetValue(dest, (int?)token[prop.Name]);
                            } else if (prop.PropertyType.FullName.Contains("Double")) {
                                prop.SetValue(dest, (double?)token[prop.Name]);
                            } else if (prop.PropertyType.FullName.Contains("Decimal")) {
                                prop.SetValue(dest, (decimal?)token[prop.Name]);
                            } else if (prop.PropertyType.FullName.Contains("Boolean")) {
                                prop.SetValue(dest, (bool?)token[prop.Name]);
                            } else if (prop.PropertyType.FullName.Contains("Guid")) {
                                if ((string)token[prop.Name] != null)
                                    prop.SetValue(dest, new Guid((string)token[prop.Name]));
                                else
                                    prop.SetValue(dest, null);
                            } else if (prop.PropertyType.FullName.Contains("TimeSpan")) {
                                if (token[prop.Name].HasValues)
                                    prop.SetValue(dest, TimeSpan.Parse((string)token[prop.Name]));
                                else
                                    prop.SetValue(dest, null);
                            }

                            break;
                    }
                }
            }

        }

        public static DateTime GetRuntime() {
            return DateTime.Now;
        }
    }


}
