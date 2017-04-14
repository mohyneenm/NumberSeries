using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace NumberSeries
{
    public class MathUtils
    {
        public const int roundingPrecision = 10;
        public static readonly List<string> operations = new List<string>() { "+", "*", "-", "/", @"\", "^" };  // "\" is the reciprocal of "/", as in, given a,b, it returns b/a

        public static List<double> Apply(string op, List<List<double>> groups)
        {
            var tmp0 = new List<double>();
            foreach (var group in groups)
            {
                if (group.Count() == 2)
                {
                    double first = group[0];
                    double second = group[1];

                    if (op == "^" && ((first != 2 && second >= 10) || (first == 2 && second >= 20)))
                    {
                        return null;
                    }
                    tmp0.Add(Evaluate(op, first, second));
                }
            }

            // Deal with the following cases for logdiv
            // some of the groups = [1,1]
            // tmp0 = [3, 3, 3, NaN, 3, 3, NaN]
            // we need to convert tmp0 into: [3, 3, 3, 3, 3, 3, 3]
            if (op == "logdiv1" || op == "logdiv2")
            {
                for (var i = 0; i < tmp0.Count; i++)
                {
                    if (double.IsNaN(tmp0[i]))
                    {
                        var tmpDistinct = tmp0.Distinct().ToList();
                        if (tmpDistinct.Count() == 2 && groups[i].First() == groups[i].Last() && groups[i].First() == 1)
                            tmp0[i] = double.IsNaN(tmpDistinct[0]) ? tmpDistinct[1] : tmpDistinct[0];
                    }
                }
            }

            return tmp0;
        }
        public static double Evaluate(string op, double a, double b)
        {
            double result = int.MinValue;

            if (op == "+")
                result = Add(a, b);
            else if (op == "*")
                result = Mul(a, b);
            else if (op == "-")
                result = Sub(a, b);
            else if (op == "/")
                result = Div(a, b);
            else if (op == @"\")
                result = Div(b, a);
            else if (op == "logdiv1")
                result = LogDiv(a, b);
            else if (op == "logdiv2")
                result = LogDiv(b, a);
            else if (op == "^")
                result = Math.Pow(a, b);

            return result;
        }
        public static double EvaluateInverseOp(string op, double a, double b)
        {
            double result = int.MinValue;

            if (op == "+")
                result = Sub(a, b);
            else if (op == "*")
                result = Div(a, b);
            else if (op == "-")
                result = Add(a, b);
            else if (op == "/")
                result = Mul(a, b);
            else if (op == @"\")
                result = Mul(b, a);
            else if (op == "^")
                result = LogDiv(b, a);

            return result;
        }
        private static double Add(double a, double b)
        {
            return Math.Round(a + b, roundingPrecision);
        }
        private static double Mul(double a, double b)
        {
            return Math.Round(a * b, roundingPrecision);
        }
        private static double Sub(double a, double b)
        {
            return Math.Round(a - b, roundingPrecision);
        }
        private static double Div(double a, double b)
        {
            // double supports infinity, so this will not throw for b=0
            return Math.Round(a / b, roundingPrecision);   // Don't use (b/a) for heuristic reasons, 'cos it confuses other direct callers.
        }
        private static double LogDiv(double a, double b)
        {
            // this helps in determining if one number is a power of the other (eg: 2^x=8 => x=log8/log2 = 3)
            //if (a != b && b == 1)
            //return 0;
            return Math.Round(Math.Log10(a) / Math.Log10(b), roundingPrecision); // Don't use (b/a) for heuristic reasons, 'cos it confuses other direct callers.
        }

        /// <summary>
        /// Solves equation of the form: x @ y = z, where @ is any basic operation
        /// The reason we are using 'out' to return the result instead of simply returning it is for readability,
        /// you can read the function signature like an equation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="operation"></param>
        /// <param name="z"></param>
        /// <returns>y</returns>
        public static void SolveEquation(double x, string operation, out double? y, double z)
        {
            #region Use Cases
            // x+y = z => y = 0+(z - x)
            // x-y = z => y = 0-(z - x)
            // x*y = z => y = z/x
            // x/y = z => y = x/z 
            // y/x = z => y = x*z 
            // x^y = z => y = (log z)/(log x) 
            // logdiv1(x,y) = z => (log y)/(log x) = z
            #endregion

            y = null;

            if (operation == "+" || operation == "-")
                y = Evaluate(operation, 0, Evaluate("-", z, x));
            else if (operation == "*")
                y = Evaluate("/", z, x);
            else if (operation == "/")
                y = Evaluate("/", x, z);
            else if (operation == @"\")
                y = Evaluate("*", x, z);
            else if (operation == "^")
                y = Math.Log10(z) / Math.Log10(x);
            else if (operation == "logdiv1")
                y = Math.Pow(10, Evaluate("/", Math.Log10(x), z));
            else if (operation == "logdiv2")
                y = Evaluate("^", x, z);
        }

        // The reason we are using 'out' to return the result instead of simply returning it is for readability,
        // you can read the function signature like an equation.
        public static void SolveEquation(out double? y, string operation, double x, double z)
        {
            y = null;
            if (operation == "^")
                y = Math.Pow(10,(Math.Log10(z) / x));
        }

        public static bool FractionalPartTooLong(double value)
        {
            // Math.Truncate(123.6666 * 10000)=1236666 == 123.6666 * 10000
            // Math.Truncate(123.666666 * 10000)=1236666 != 123.666666 * 10000
            if (Math.Truncate(value * 10000) != value * 10000)  // we don't want to deal with long fractional parts at this point
                return true;
            else
                return false;
        }
    }
}
