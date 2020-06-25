using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HR.Common
{
    public class FormulaUtil
    {

        public static List<string> GetFormulaCase()
        {
            var formulacase = new List<string>();

            formulacase.Add("If1==1Then1Endif");
            formulacase.Add("If1==2Then0Elseif1==1Then1Endif");
            formulacase.Add("If1==2Then0Elseif1==2Then0Else1Endif");
            formulacase.Add("If1==2Then0Elseif1==2Then0Elseif1==2Then0Else1Endif");
            formulacase.Add("If1==1ThenIf1==1Then1EndifEndif");
            formulacase.Add("If1==1ThenIf1==2Then0Else1EndifEndif");
            formulacase.Add("If1==1ThenIf1==2Then0Elseif1==1Then1EndifEndif");
            formulacase.Add("If1==1ThenIf1==2Then0Elseif1==1Then1Else0EndifEndif");
            formulacase.Add("If1==1ThenIf1==2Then0Elseif1==2Then0Elseif1==1Then1Else0EndifEndif");
            formulacase.Add("If1==1ThenIf1==2Then0Elseif1==2Then0Elseif1==2Then0Else1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==1ThenIf1==2Then0Elseif1==1Then1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==1ThenIf1==2Then0Elseif1==1Then1Else0EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==1ThenIf1==2Then0Elseif1==2Then0Else1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==1ThenIf1==1Then1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==1ThenIf1==2Then0Else1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==2Then0ElseIf1==2Then0Elseif1==1Then1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==2Then0ElseIf1==2Then0Else1EndifEndif");
            formulacase.Add("If1==2Then0Elseif1==2Then0ElseIf1==2Then0Elseif1==2Then0Else1EndifEndif");
            formulacase.Add("If1==2ThenIf1==2Then0Elseif1==2ThenElse1EndifElseif1==2ThenIf1==2Then0Elseif1==2ThenElse1EndifElseIf1==2Then0Elseif1==2ThenElse1EndifEndif");
            formulacase.Add("If1==2ThenIf1==2Then0Elseif1==2Then0Else1EndifElseif1==1ThenIf1==2Then0Elseif1==1Then1Else0EndifElseIf1==2Then0Elseif1==2Then0Else1EndifEndif");
            return formulacase;

        }

        public static string Condition(string expr)
        {
            try
            {
                //begin with if
                if (!expr.Substring(0, 2).Equals("If"))
                {
                    return expr;
                }

                {
                    String s2 = ""; String s4 = ""; String s6 = ""; String s5 = "";
                    int bracketCount = 0;
                    string innerExp = "";
                    Nullable<double> result = null;

                    for (int i = 2; i < expr.Length; i++)
                    {
                        var str = expr.Substring(i, expr.Length - i);

                        String s = expr.Substring(i, 1);
                        if (i + 2 <= expr.Length) s2 = expr.Substring(i, 2);
                        if (i + 4 <= expr.Length) s4 = expr.Substring(i, 4);
                        if (i + 5 <= expr.Length) s5 = expr.Substring(i, 5);
                        if (i + 6 <= expr.Length) s6 = expr.Substring(i, 6);

                        if (s2.Equals("If") & !s6.Equals("Elseif") & !s5.Equals("Endif"))
                            bracketCount++;

                        if (bracketCount > 0)
                        {
                            if (s5.Equals("Endif"))
                            {
                                bracketCount--;
                                if (result != 0 & bracketCount == 0)
                                    return Condition(innerExp + "Endif");

                                innerExp += s;
                                continue;
                            }
                        }
                        else
                        {
                            if (result == null & s4.Equals("Then"))
                            {                              
                                result = Evaluate(innerExp);
                                innerExp = "";
                                i++; i++; i++;
                                continue;
                            }

                            if (result != null && result == 1)
                            {
                                if (s6.Equals("Elseif"))
                                {
                                    return innerExp;
                                }
                                else if (s4.Equals("Else") & !s6.Equals("Elseif"))
                                {
                                    return innerExp;
                                }
                                else if (s5.Equals("Endif"))
                                {
                                    return innerExp;
                                }
                            }
                            else
                            {
                                //condition is false

                                if (s6.Equals("Elseif"))
                                {
                                    innerExp = "";
                                    result = null;
                                    i = i + 5;
                                    continue;
                                }
                                else if (s4.Equals("Else") & !s6.Equals("Elseif"))
                                {
                                    innerExp = "";
                                    result = null;
                                    i = i + 3;
                                    continue;
                                }
                                else if (s5.Equals("Endif"))
                                {
                                    if (innerExp.Length > 0 && innerExp.Substring(0, 1) != "[")
                                    {
                                        return "";
                                    }
                                    return innerExp;
                                }
                            }
                        }

                        innerExp += s;
                    }
                }
            }
            catch
            {

            }
            return "";
        }

        public static double Evaluate(string expr)
        {
            try
            {
                expr = expr.Replace(" ", "");
                expr = expr.Replace("true", "1");
                expr = expr.Replace("false", "0");

                Stack<String> stack = new Stack<String>();

                string value = "";
                for (int i = 0; i < expr.Length; i++)
                {
                    String s = expr.Substring(i, 1);
                    // pick up any doublelogical operators first.
                    if (i < expr.Length - 1)
                    {
                        String op = expr.Substring(i, 2);
                        if (op == "<=" || op == ">=" || op == "==")
                        {
                            if (!string.IsNullOrEmpty(value))
                                stack.Push(value);
                            value = "";
                            stack.Push(op);
                            i++;
                            continue;
                        }
                    }

                    char chr = s.ToCharArray()[0];

                    if (!char.IsDigit(chr) && chr != '.' && value != "")
                    {
                        stack.Push(value);
                        value = "";
                    }
                    if (s.Equals("("))
                    {
                        string innerExp = "";
                        i++; //Fetch Next Character
                        int bracketCount = 0;
                        for (; i < expr.Length; i++)
                        {
                            s = expr.Substring(i, 1);

                            if (s.Equals("(")) bracketCount++;

                            if (s.Equals(")"))
                            {
                                if (bracketCount == 0) break;
                                bracketCount--;
                            }
                            innerExp += s;
                        }
                        stack.Push(Evaluate(innerExp).ToString());
                    }
                    else if (s.Equals("+") ||
                             s.Equals("-") ||
                             s.Equals("*") ||
                             s.Equals("/") ||
                             s.Equals("<") ||
                             s.Equals(">"))
                    {
                        if (s.Equals("-"))
                        {

                            if (stack.Count == 0)
                            {
                                value += s;
                            }
                            else
                            {
                                var spop = stack.Pop();
                                stack.Push(spop);
                                if (spop.Equals("=") ||
                                    spop.Equals("*") ||
                                    spop.Equals("/") ||
                                    spop.Equals("<") ||
                                    spop.Equals(">"))
                                {
                                    value += s;
                                }
                                else
                                {
                                    stack.Push(s);
                                }
                            }
                        }
                        else
                        {
                            stack.Push(s);
                        }
                    }
                    else if (char.IsDigit(chr) || chr == '.')
                    {
                        value += s;

                        if (value.Split('.').Length > 2)
                            throw new Exception("Invalid decimal.");

                        if (i == (expr.Length - 1))
                            stack.Push(value);

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(s))
                            stack.Push(s);
                        //throw new Exception("Invalid character.");
                    }

                }
                double result = 0;
                List<String> list = stack.ToList<String>();

                while (list.Contains("/"))
                {
                    for (int i = list.Count - 2; i >= 0; i--)
                    {
                        if (list[i] == "/")
                        {
                            list[i] = (Convert.ToDouble(list[i + 1]) / Convert.ToDouble(list[i - 1])).ToString();
                            list.RemoveAt(i + 1);
                            list.RemoveAt(i - 1);
                            i -= 2;
                        }
                    }
                }
                while (list.Contains("*") | list.Contains("x"))
                {

                    for (int i = list.Count - 2; i >= 0; i--)
                    {
                        if (list[i] == "*" | list.Contains("x"))
                        {
                            list[i] = (Convert.ToDouble(list[i + 1]) * Convert.ToDouble(list[i - 1])).ToString();
                            list.RemoveAt(i + 1);
                            list.RemoveAt(i - 1);
                            i -= 2;
                        }
                    }
                }
                while (list.Contains("-") | list.Contains("–") | list.Contains("+"))
                {
                    for (int i = list.Count - 2; i >= 0; i--)
                    {
                        if (list[i] == "+")
                        {
                            list[i] = (Convert.ToDouble(list[i + 1]) + Convert.ToDouble(list[i - 1])).ToString();
                            list.RemoveAt(i + 1);
                            list.RemoveAt(i - 1);
                            i -= 1;
                        }
                        else if (list[i] == "-" | list[i] == "–")
                        {
                            list[i] = (Convert.ToDouble(list[i + 1]) - Convert.ToDouble(list[i - 1])).ToString();
                            list.RemoveAt(i + 1);
                            list.RemoveAt(i - 1);
                            i -=1;
                        }
                    }
                }

                if (list.Contains("&") | list.Contains("|"))
                {
                    List<string> conditions = new List<string>();
                    var con = "";
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i] == "&" | list[i] == "|")
                        {
                            conditions.Add(con);
                            conditions.Add(list[i]);
                            con = "";
                        }
                        else
                        {
                            con = con + list[i];
                        }
                    }
                    if (!string.IsNullOrEmpty(con))
                    {
                        conditions.Add(con);
                    }

                    while (conditions.Contains("&") | conditions.Contains("|"))
                    {
                        for (int i = conditions.Count - 1; i >= 0; i--)
                        {
                            if (conditions[i] == "&")
                            {
                                if (Evaluate(conditions[i - 1]) == 1 & Evaluate(conditions[i + 1]) == 1)
                                {
                                    conditions[i] = "1";
                                }
                                else
                                {
                                    conditions[i] = "0";
                                }
                                conditions.RemoveAt(i + 1);
                                conditions.RemoveAt(i - 1);
                                i -= 2;

                            }
                            else if (conditions[i] == "|")
                            {
                                if (Evaluate(conditions[i - 1]) == 1 | Evaluate(conditions[i + 1]) == 1)
                                {
                                    conditions[i] = "1";
                                }
                                else
                                {
                                    conditions[i] = "0";
                                }
                                conditions.RemoveAt(i + 1);
                                conditions.RemoveAt(i - 1);
                                i -= 2;
                            }
                        }
                    }

                    list = conditions;

                }

                stack.Clear();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    stack.Push(list[i]);
                }
                while (stack.Count >= 3)
                {

                    string right = stack.Pop();
                    string op = stack.Pop();
                    string left = stack.Pop();

                    Nullable<double> dright = null;
                    Nullable<double> dleft = null;
                    try
                    {
                        dright = Convert.ToDouble(right);
                        dleft = Convert.ToDouble(left);
                    }
                    catch
                    {

                    }
                    if (dright != null & dleft != null)
                    {
                        if (op == "<") result = (dleft < dright) ? 1 : 0;
                        else if (op == ">") result = (dleft > dright) ? 1 : 0;
                        else if (op == "<=") result = (dleft <= dright) ? 1 : 0;
                        else if (op == ">=") result = (dleft >= dright) ? 1 : 0;
                        else if (op == "==") result = (dleft == dright) ? 1 : 0;
                    }
                    else
                    {
                        if (op == "==") result = (left == right) ? 1 : 0;
                    }


                    stack.Push(result.ToString());
                }
                return Convert.ToDouble(stack.Pop());
            }
            catch
            {
                return 0;
            }
        }
    }
}