using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    public class Diffs
    {
        // Diffs
        public static NPATResult Evaluate(List<double> input, int recLevel)
        {
            var result = new NPATResult();

            if ((recLevel == 0 && input.Count <= 2) || (recLevel > 0 && input.Count <= 1))
                return null;

            // eg: 3, 3, 3, 3...
            if (input.Distinct().Count() == 1)
            {
                result.Values.Add(input.First());
                result.RecursiveDepth = recLevel;
                result.PatternComplexityOrder = PatternComplexityOrder.SameNumbers;
                return result;
            }

            var diffs = GetDiffs(input);
            Helpers.PrintMethod("Diffs", diffs, recLevel);

            // Check if 1st level diffs form a series
            if ((recLevel == 0 && diffs.Count >= 3) || (recLevel >= 1 && diffs.Count() >= 2))
            {
                var tmpResult = NPAT.GetNextNumber(diffs, ++recLevel);
                if (tmpResult != null && tmpResult.Values.Count() > 0)
                {
                    result.InputSeries = input;
                    result.SeriesName = tmpResult.SeriesName;
                    result.MatchType += $"{tmpResult.MatchType}";
                    result.Values.Add(input.Last() + tmpResult.Values[0]);
                    Helpers.PrintInput(input, $"diffs", "-", tmpResult.Values[0]);
                    result.RecursiveDepth = recLevel;
                    result.PatternComplexityOrder = PatternComplexityOrder.ConstantDiff;
                    return result;
                }
                else
                {
                    return null;
                }
            }
            // ???????
            // This will cause almost every recursive level ConstDiff if we allow just 2 items (every 2 items have a ConstDiff by definition)
            // DO NOT do this!
            /*else if(recLevel > 0 && diffs1.Count >= 1)  // 1, 1, 4, 8, 14, 42 => *1,*2,*3; +3,+6
            {
                result.RecursiveDepth = recLevel;
                result.PatternComplexityOrder = PatternComplexityOrder.ConstantDiff;
                result.Values.Add(input.Last() + diffs1.Last());
                return result;
            }*/

            return null;
        }

        private static List<double> GetDiffs(List<double> input, bool groupedByTwo = false)
        {
            var tmp = new List<double>();

            // calculates diffs
            // input: 6,8,3,7,9,3 => output: 2,-5,4,2,-6    (not grouped, diffs of all adjacent items)
            // input: 6,8,3,7,9,3 => output: 2,4,-6         (groupedByTwo)
            for (var i = 0; i < input.Count(); i++)
            {
                if ((i + 1) < input.Count())
                {
                    tmp.Add(Math.Round(input[i + 1] - input[i], MathUtils.roundingPrecision));
                    if (groupedByTwo)
                        i++;
                }
            }

            return tmp;
        }
    }
}
