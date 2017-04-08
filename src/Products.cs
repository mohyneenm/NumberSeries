using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    public class Products
    {
        public static NPATResult Evaluate(List<double> input, int recLevel)
        {
            NPATResult result = new NPATResult();
            if (input.Count < 3)
                return null;

            Helpers.PrintMethod("Prods", input, recLevel);

            // calculates prods
            // input: 2,6,18,54 => output: 3,3,3            (not grouped)
            var tmp = new List<double>();
            for (var i = 0; i < input.Count(); i++)
            {
                if ((i + 1) < input.Count())
                {
                    if (input[i] == 0)
                        return null;    // we are using division to determine multiples (see below), so no point continuing if divByZero

                    var value = input[i + 1] / input[i];

                    if (MathUtils.FractionalPartTooLong(value))
                        return null;

                    tmp.Add(value);
                }
            }

            // Check if each number is a constant multiple of previous
            if (tmp.Distinct().Count() == 1)
            {
                result.MatchType += "Products;";
                result.Values.Add(input.Last() * tmp[0]);
                Helpers.PrintInput(input, "prods", "*", tmp[0]);
                result.RecursiveDepth = recLevel;
                result.PatternComplexityOrder = PatternComplexityOrder.SameNumbers;
                return result;
            }

            // Check if the factors form a series themselves
            // {2,4,12,60,420} => {2,3,5,7} => primes
            var tmpResult = NPAT.GetNextNumber(tmp, ++recLevel);
            if (tmpResult != null && tmpResult.Values.Count() > 0)
            {
                result.InputSeries = input;
                result.SeriesName = tmpResult.SeriesName;
                result.MatchType += $"Products; {result.SeriesName};";
                result.Values.Add(input.Last() * tmpResult.Values[0]);
                result.PatternComplexityOrder = tmpResult.PatternComplexityOrder;
                result.RecursiveDepth = tmpResult.RecursiveDepth;
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
