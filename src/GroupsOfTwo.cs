using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    public static class GroupsOfTwo
    {
        // We calculate two different groups, each with different result constraints (ref RULE 1), and check the results
        public static NPATResult Evaluate(List<double> input, int recLevel, Func<List<double>, int, NPATResult> getNext = null)
        {
            List<List<double>> groups1 = Helpers.GetGroups(input, 2);
            List<List<double>> groups2 = Helpers.GetGroups(input, 2, false, 1); // start grouping from index 1; secondary grouping
            NPATResult result2 = null;
            NPATResult result = new NPATResult();
            getNext = getNext ?? NPAT.GetNextNumber;

            // Primary grouping:
            // 1, 1, 4, 8, 14, 42 => (1,1) (4,8) (14,42)
            // If there are odd number of items and we match 3 values, then just return the result. (eg: 2, 6, 3, 9, 5, 15, 6, x)
            var result1 = EvaluateGroupsOfTwoUtil(groups1, recLevel, 3, getNext);
            if (result1 == null)
            {
                result1 = EvaluateGroupsOfTwoUtil(groups1, recLevel, 2, getNext);
            }
            else if (!result1.IsCompleteSeries && result1?.Values.Count > 0 && input.Count() % 2 != 0)
            {
                return result1;
            }

            if (result1 == null)
                return null;

            // Secondary grouping:
            // 1, 1, 4, 8, 14, 42 => (1,4) (8,14) (42,x)
            // If we are here then it means there are even number of items, so we need to check the secondary grouping.
            if (result1 != null)
            {
                result2 = EvaluateGroupsOfTwoUtil(groups2, recLevel, 3, getNext);
                if (result2 == null)
                    result2 = EvaluateGroupsOfTwoUtil(groups2, recLevel, 2);

                if (result1?.Values.Count > 0 && input.Count() % 2 != 0)
                    return result1;
                else if (result2?.Values.Count > 0 && input.Count() % 2 == 0)
                    return result2;
                else
                    return null;
            }

            return null;
        }

        private static NPATResult EvaluateGroupsOfTwoUtil(List<List<double>> groups, int recLevel, int resultContstraint, Func<List<double>, int, NPATResult> GetNextNumber = null)
        {
            double? next = null;
            IEnumerable<string> ops = MathUtils.operations.Union(new List<string>() { "logdiv1", "logdiv2" }); // to determine if one number is a power of the other
            GetNextNumber = GetNextNumber ?? NPAT.GetNextNumber;
            List<NPATResult> results = new List<NPATResult>();
            var newRecLevel = recLevel + 1;

            // Iterate the operations: +, *, -, /, ^
            foreach (var op in ops)
            {
                // apply each operation to all groups and store the results in a list
                var operationResults = MathUtils.Apply(op, groups);  // result for each op: '+', '-', '*', '/', '^'       (eg, a+b, a*b, a-b, a/b)

                // since we are doing ^ and logdiv it is possible to get values that are too large or NaN; ignore those results
                if (operationResults == null || operationResults.Any(x => double.IsNaN(x)))
                    continue;

                // Check 1: see if the results of the operation form a pattern
                var recur = GetNextNumber(operationResults, newRecLevel);
                if (recur?.Values.Count > 0)
                {
                    var result = new NPATResult() { PatternComplexityOrder = recur.PatternComplexityOrder, RecursiveDepth = recur.RecursiveDepth };

                    if (groups.Last().Count() == 1)
                    {
                        MathUtils.SolveEquation(groups.Last()[0], op, out next, recur.Values[0].Value);

                        result.Values.Add(Math.Round(next.Value, MathUtils.roundingPrecission));
                        Helpers.PrintInput(groups.SelectMany(x => x).ToList(), "2grp", new List<string>() { op }, operationResults);
                        //Matches += $"2grp:'{op}'";
                        results.Add(result);
                    }
                    else
                    {
                        result.IsCompleteSeries = true;
                        results.Add(result);
                    }
                }

                // Check 2: see if squares match
                /*if (op == "^2" && tmp0.Count > 0)
                {
                    var sqMatch = true;
                    for (var i = 0; i < groups.Count(); i++)
                    {
                        var group = groups[i];
                        if (group.Count == 2)
                        {
                            sqMatch = sqMatch && (tmp0[i] == group[1]);
                        }
                    }
                    if (sqMatch) // all 2-groups have (n,n^2) relation
                    {
                        Matches += "(n,n^2)";
                        if (groups.Last().Count() == 1) // last group is not full, so find the missing item
                        {
                            next = Math.Pow(groups.Last()[0], 2);
                            r.Values.Add(next);
                            r.MatchType += "squares;";
                            return r;
                        }
                        else // since the last group is full (2 items), we need to find relations between groups to figure out the 1st item of next group
                        {
                            // TODO {1, 1, 4, 8, 14, 42} => (1,1), (1,4), (4,8), (8,14), (14,42)
                            //List<double> overlappingGroups = GetGroups(input, 2, true).Where((l, i) => i % 2 != 0).SelectMany(x => x).ToList(); // take the odd indexed groups in overlapping groups, and flatten into a single list
                            //var res = GetNextNumber(overlappingGroups);
                        }
                    }
                }*/

                // Check 3: see if cubes match
                /*if (op == "^3" && tmp0.Count > 0)
                {
                    var cbMatch = true;
                    for (var i = 0; i < groups.Count(); i++)
                    {
                        var group = groups[i];
                        if (group.Count == 2)
                        {
                            cbMatch = cbMatch && (tmp0[i] == group[1]);
                        }
                    }
                    if (cbMatch)    // all 2-groups have (n,n^3) relation
                    {
                        Matches += "(n,n^3)";
                        if (groups.Last().Count() == 1){
                            next = Math.Pow(groups.Last()[0], 3);
                            r.Values.Add(next);
                            r.MatchType += "cubes;";
                            return r;
                        }
                    }
                }*/
            }

            if (results.Count > 0)
            {
                var resultsOrdered = results.Where(r => r?.Values.Count > 0 || r.IsCompleteSeries).OrderBy(r => 2 * r.RecursiveDepth + r.PatternComplexityOrder); // recursive depth is given lower precedence by giving it extra weight (*2)
                return resultsOrdered.FirstOrDefault();
            }
            return null;
        }
    }
}
