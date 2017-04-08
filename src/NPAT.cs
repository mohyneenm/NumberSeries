using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    // RULE 1: A single pattern should find at least 3 values (ref RULE 4)
    // Reduce the constraint to 2 values if another pattern is found in the series (eg splitting the series into 2, one pattern finds 3 values, another finds 2)
    // RULE 2: How do we determine complexity of a pattern: common standard series, +1, -1, *2, /2, ^2, recursive level, or something else
    // A pattern (eg. diffs) with all the same numbers (3,3,3,...) is simpler than a pattern such as (7,9,13,...), but the 2nd pattern may be discovered before the 1st.
    // 2, 5, 3, 6, 5, 8, 6, x
    // So, it means we need to check multiple patterns before settling on a single one.
    // RULE 3: What shoud the depth of recursion be? Keep it 2 for now.
    // RULE 4: Should the rules be same for all levels of recursion? Heuristially, it is likely to find a simple standard series in a level 1 or 2 in a recursive call.
    // RULE 5: Assume series starts at the 1st number for now. We can generalize this later.
    // RULE 6: Should we consider a subset/subsequence of a standard series? For now, let's consider both.
    // RULE 7: Are the basic opearations (+,-,*,/) in one direction enough to find a pattern. No! Because we are also matching standard series, so, we need to match 1,1/2,1/3... or 1/2,1/3,1/5,1/7...
    // which becomes tricky unless we simply reverse the division. 

    // Types of patterns:
    // standard, diffs, diffs of diffs, interleaving, padded
    // 2grp-primary, 2grp-secondary, 2grp-interleaving, 2grp-interleaving 
    // 3grp-primary, 3grp-secondary, 3grp-interleaving, 3grp-interleaving 
    // 4grp-primary, 4grp-secondary

        // TODO: Power series
        // Multithreading

    public class NPAT
    {
        public static string Matches { get; set; }
        private static int maxRecursiveDepth = 2;
        
        public static NPATResult GetNextNumber(List<double> input, int recLevel = 0)
        {
            //input = new List<double> { 4, 5, 9, 18, 34 };                                 // 59, diffs
            //input = new List<double> { 3, 4, 12, 76, 588 };                               // 4684, needs fine tuning ***
            //input = new List<double> { 4, 20, 35, 49, 62, 74 };                           // 85, diffs; // n, ([0]@c-0), ([1]@c-1), ([2]@c-2)
            //input = new List<double> { 100, 98, 95, 90, 83 };                             // 72, decreasing gaps
            //input = new List<double> { 3, 7, 10, 8, 4, 12, 0, 5, 5, 3, 2 };               // 5, groups of three   // {n,m,[0]@[1]}
            //input = new List<double> { 5, 10, 9, 3, 6, 5, 4, 8, 7, 7 };                   // 14, groups of three  // {n,[0]@1c1,[1]@2c2}
            //input = new List<double> { 32, 0.2, 160, 15, 0.5, 30, 28, 0.7 };              // 40, groups of three
            //input = new List<double> { 18, 24, 5, 21, 27, 8, 17, 23, 4, 24, 30 };         // 11, groups of three
            //input = new List<double> { 9, 12, 4, 8, 12, 15, 5, 9, 3, 6, 2, 6, 6, 9 };     // 3, groups of four    // {n,[0]@c,[1]@c,[2]@c}
            //input = new List<double> { -5, 1, -11, 23, 29, 17, -20, -14 };                // -26, groups of three
            //input = new List<double> { 33, 8, 41, 46, 21, 67, 72, 47, 119, 124 };         // 99, groups of three
            //input = new List<double> { 4, 3, 144, 1.5, 2, 9, 5, .3, 2.25, 3.2, 2 };       // 40.96, groups of three    // {n,m,([0]@[1])@c}
            //input = new List<double> { 2, 6, 5, 14, 11, 26, 17 };                         // 39, primes and multiples ***
            //input = new List<double> { 2, 9, 5, 49, 11, 169, 17 };                        // 36, primes and squares ***
            //input = new List<double> { 7, 11, 13, 17, 19, 23, 29 };                       // 31, primes
            //input = new List<double> { 28, 14, 15, 22.5, 24.5, 61.25, 64.25 };            // 224.875, groups of two, div  // {n,[0]*0.5}, {n,[0]*1.5}
            //input = new List<double> { 1+2, 4*2, 9+3, 16*3, 25+4, 36*4, 49+5 };           // 320; 1, 4, 9, 16, 25, 36, 49, 64 padded lists
            //input = new List<double> { 1 * 1, 4 * 2, 9 * 3, 16 * 4, 25 * 5, 36 * 6 };     // 343; 1, 4, 9, 16, 25, 36, 49, 64
            //input = new List<double> { 5, 7, 10, 13, 18, 21, 26, 29, 34, 41 };            // 44, padded primes //2+3, 3+4, 5+5, 7+6, 11+7, 13+8, 17+9, 19+10, 23+11, 29+12
            //input = new List<double> { 2*3, 3*4, 5*5, 7*6, 11*7, 13*8 };                  // 153, (padded primes) 2, 3, 10, 15, 26
            //input = new List<double> { 2, 3, 10, 15, 26 };                                // 35, padded squares
            //input = new List<double> { 2, 10, 30, 68, 130 };                              // 222, padded cubes
            //input = new List<double> { 9, 3, 6, 21, 9, 12, 33, 15, 18 };                  // 45, 21, 24 three series
            //input = new List<double> { 13, 24, 36, 23, 34, 56, 33, 44, 96, 43, 54, 156 }; // 53, 64, 236 three series
            //input = new List<double> { -3, 6, 1, -2, 4, -1, -1, 2, -3 }; // 0, 0, -5 three series
            //input = new List<double> { 1, 2, 3, 5, 7, 10, 13, 17 }; // 21, 26 two series
            //input = new List<double> { 2, 13, 2, 4, 8, 3, 8, 3, 5, 16 }; // -2, (7|8), 32 three series
            //input = new List<double> { 96, 64, 128, 192, 128, 256, 288, 192 };            // 384, groups of three, div/div
            //input = new List<double> { 0, 1, 2, 0.5, 2, 3, 5, 1, 2, 0, 2, 1, 4, 2, 3 };   // 2, groups of four Add,Div
            //input = new List<double> { 0, 1, 2, 2, 2, 3, 5, 11, 2, 0, 2, 2, 4, 2, 3 };    // 11, groups of four Mul,Add
            //input = new List<double> { 2, 3, 2, 4, 3, 1, 1, 2, 5, 2, 3, 7, 4, 2, 3 };     // 5, groups of four Mul,Sub
            //input = new List<double> { 2.2, 5.5, 8, 36, 40.5, 263.25 };                   // groups of three intervleaved *** 
            //input = new List<double> { 1, 1, 3, 2, 1, 5, 3, 4, 10, 4, 2 };                // 10, incremental padding, groups of three
            //input = new List<double> { 69, 55, 26, 13 };                                  // 4, product of digits with padding
            //input = new List<double> { 8, 16, 3, 27, 4, 8, 7, 19, 2, 0, 7 };              // 9, groups of four
            //input = new List<double> { 3, 5, 2, 100, 4, 7, 3, 196, 1, 6, 5 };             // 144, groups of four
            //input = new List<double> { 5, 1, 2, 8, 8, 1, 0, 9, 9, 2, 3 };                 // 14, groups of four
            //input = new List<double> { 3, 2, 1, 4, 5, 4, 5, 1, 0, 6, 3 };                 // 2,7 groups of four interleaved ***
            //input = new List<double> { 8, 0.5, 16, 4, 15, 0.3, 50, 4.5, 6, 0.2, 30 };     // 1.2, groups of four 8/0.5 = 16	8*0.5 = 4
            //input = new List<double> { 2, 7, -35, 7, 12, -60, 5, 10 };                    // -50, groups of three, Sub,Div
            //input = new List<double> { 6, 3, 3, 9, 12, 5, 7, 35, 8, 6, 2 };               // 12, groups of four, 3+3 = 6, 	3*3 = 9
            //input = new List<double> { 1 1 2 4 5 25 6 36 9 81 7 };                       // NOTE: matches 4group and 2group 1+1+2=4 vs 2^2=4;5^=25...
            //input = new List<double> { 9/8.0, 4/3.0, 17/8.0, 10/3.0, 7/3.0, 42/8.0, 10/3.0, 29/4.0, 13/3.0, 7/2.0, 16/3.0 };               // 11/2; +1,+2 2series
            //input = new List<double> { 3, 6, 2, 4, 12, 3, 5, 20 };                        // 4; 3grp; 6/2=3; 12/3=4; 20/x=5


            // don't want to go down a rabbit hole
            if (recLevel > maxRecursiveDepth)
                return null;

            // since we are doing ^ and logdiv it is possible to get values that are too large or NaN; ignore those results
            if (input == null || input.Any(x => double.IsNaN(x) || x > double.MaxValue || x < double.MinValue))
                return null;

            Helpers.PrintMethod("GetNextNumber", input, recLevel);

            NPATResult result = null;
            int numOfItems = input.Count();
            bool usePredefinedLists = false;

            //if (usePredefinedLists && NPATpredefined.FindSeries(input))
                //return null;

            // heuristically, for a secondary series there is a higher chance for it to be a standard series
            if(recLevel > 0 && numOfItems >= 3)
                result = StandardSeries.Evaluate(input, recLevel, false); // check subsequence

            if (result == null && recLevel <= maxRecursiveDepth)
                result = Diffs.Evaluate(input, recLevel);

            if (recLevel <= maxRecursiveDepth && (result == null || result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 3)
                result = Products.Evaluate(input, recLevel);

            if ((result == null || result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 6)
                result = GroupsOfThree.Evaluate(input, recLevel);

            if((result == null || result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 8)
                result = EvaluateGroupsOfFour_TwoAndThreeOps(input);

            if (recLevel <= maxRecursiveDepth && (result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 6)
                result = EvaluateInterleavingLists(input, recLevel);

            //if (result == null || result?.Values?.Count == 0 || result?.Values?[0] == null)
              //  result = PaddedSeries.Evaluate(input, recLevel);

            if (recLevel <= maxRecursiveDepth && (result == null || result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 5) // 4608, 2304, 576, 288, 72
                result = GroupsOfTwo.Evaluate(input, recLevel);

            if ((result == null || result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 4)
                result = CheckMultiples(input);

            // final check, match any standard series as a subset
            if ((result == null || result?.Values?.Count == 0 || result?.Values?[0] == null) && numOfItems >= 5)
                result = StandardSeries.Evaluate(input, recLevel, true);  // check subset


            if(result!=null)
                result.InputSeries = input;
            return result;
        }

        // Checks if all items are divisible by some integer number
        // {7, 21, 49, 77} => all multiples of 7
        private static NPATResult CheckMultiples(List<double> input)
        {
            NPATResult result = new NPATResult();
            //if(!Enumerable.Range(1, 10).SelectMany(n => input, (n, i) => i / n).Except(allSeries[Series.Natural]).Any())
            for (var i=3; i<=10; i++)
            {
                var isMultiple = !input.Select(n => n / i).Except(StandardSeries.allSeries[Series.Natural]).Any();
                if(isMultiple)
                {
                    result.MatchType += "Multiples;";
                    result.Values.Add(i);
                    result.Comment = $"List Divides by {i}";
                    return result;
                }
            }
            return null;
        }


        private static List<int> FindPerfectCubeBases(List<double> input)
        {
            return input.Select(x => (int)Math.Round(Math.Pow(x, (1.0 / 3)))).ToList();
        }

        private static NPATResult EvaluateGroupsOfFour_TwoAndThreeOps(List<double> input)
        {
            List<List<double>> groups = Helpers.GetGroups(input, 4);
            var crossjoin = MathUtils.operations.SelectMany(o => MathUtils.operations, (o1, o2) => o1 + "," + o2);      // { "Add,Mul", "Mul,Sub", "Add,Div", "Sub,Mul", etc...}
            var twoOps = crossjoin.ToList();                                                        // { "Add,Mul", "Mul,Sub", "Add,Div", "Sub,Mul", etc...}
            var threeOps = crossjoin.SelectMany(o => MathUtils.operations, (o1, o2) => o1 + "," + o2);        // { "Add,Mul,Add", "Mul,Sub,Sub", "Add,Div,Mul", "Sub,Mul,Div", etc...}
            double? next = null;
            NPATResult result = new NPATResult();
            result.MatchType += "4Group";

            var tmp0 = new List<double>();
            var tmp1 = new List<double>();
            var tmp2 = new List<double>();
            string[] ops = new string[0];

            // Check 1: two operations with one match
            next = GroupsOfFourTwoOpsOneMatch(input, groups, twoOps);
            if (next != null)
            {
                result.Values.Add(next);
                Helpers.PrintInput(input, "4grp", twoOps, tmp0);
                return result;
            }
            // Check 2: two operations with two matches
            next = GroupsOfFourTwoOpsTwoMatches(input, groups, twoOps);
            if (next != null)
            {
                result.Values.Add(next);
                Helpers.PrintInput(input, "4grp", twoOps, tmp0);
                return result;
            }

            // Check 3: three operations
            foreach (var tripleOperation in threeOps)
            {
                // fill tmp lists per group based on pair of operation
                foreach (var list in groups)
                {
                    if (list.Count() == 4)
                    {
                        double first = list[0];
                        double second = list[1];
                        double third = list[2];
                        double fourth = list[3];
                        ops = tripleOperation.Split(',');

                        tmp0.Add(MathUtils.Evaluate(ops[0], first, second));
                        tmp1.Add(MathUtils.Evaluate(ops[1], second, third));
                        tmp2.Add(MathUtils.Evaluate(ops[2], third, fourth));
                    }
                }

                // evaluate next value
                if (tmp0.Distinct().Count() == 1 && tmp1.Distinct().Count() == 1 && tmp2.Distinct().Count() == 1)
                {
                    if (groups.Last().Count() == 1)
                        next = MathUtils.Evaluate(ops[0], groups.Last()[0], tmp0.First());
                    else if (groups.Last().Count() == 2)
                        next = MathUtils.Evaluate(ops[1], groups.Last()[1], tmp1.First());
                    else if (groups.Last().Count() == 3)
                        next = MathUtils.Evaluate(ops[2], groups.Last()[2], tmp2.First());

                    result.Values.Add(next);
                    Helpers.PrintInput(input, "4grp", ops.ToList(), new List<double>() { tmp0.First(), tmp1.First(), tmp2.First() });
                    return result;
                }

                tmp0.Clear();
                tmp1.Clear();
                tmp2.Clear();
            }

            return null;
        }
        private static double? GroupsOfFourTwoOpsOneMatch(List<double> input, List<List<double>> groups, List<string> twoOps)
        {
            var tmp = new List<double>();
            double? next = null;
            string[] ops = new string[0];

            // check each pair of operation
            foreach (var twoOp in twoOps)
            {
                ops = twoOp.Split(',');

                // fill tmp lists per group based on pair of operation
                foreach (var group in groups)
                {
                    if (group.Count() == 4)
                    {
                        double first = group[0];
                        double second = group[1];
                        double third = group[2];
                        double fourth = group[3];

                        var tmpresult = MathUtils.Evaluate(ops[0], first, second);
                        tmp.Add(MathUtils.Evaluate(ops[1], tmpresult, third));  // (0, 1, 2, 0.5) => (0 + 1)/2 = 0.5
                    }
                }

                // evaluate next value
                var inAllGroups = false;
                var powerMatch = -1;
                for (var i = 0; i < groups.Count(); i++)
                {
                    var group = groups[i];
                    if (group.Count() == 4)
                    {
                        powerMatch = -1;

                        // Todo: the logic here is flawed, fix it; We need all groups to match the same condition, and then set inAllGroups = true;
                        // Here multiple groups can match multiple conditions and set inAllGroups to true, which is wrong.
                        if (group[3] == tmp[i])
                            powerMatch = 1;
                        else if (System.Math.Pow(tmp[i], 2) == group[3])
                            powerMatch = 2;
                        else if (System.Math.Pow(tmp[i], 3) == group[3])
                            powerMatch = 3;

                        inAllGroups = powerMatch > 0;

                        if (!inAllGroups) break;
                    }
                }
                if(inAllGroups && groups.Last().Count() == 3)
                {
                    var tmpresult = MathUtils.Evaluate(ops[0], groups.Last()[0], groups.Last()[1]);
                    next = System.Math.Pow(MathUtils.Evaluate(ops[1], tmpresult, groups.Last()[2]), powerMatch);
                    return next;
                }

                tmp.Clear();
            }

            return next;
        }
        private static double? GroupsOfFourTwoOpsTwoMatches(List<double> input, List<List<double>> groups, List<string> twoOps)
        {
            var tmp0 = new List<double>();
            var tmp1 = new List<double>();
            var tmp2 = new List<double>();
            var tmp3 = new List<double>();
            var tmp4 = new List<double>();
            double? next = null;
            string[] ops = new string[0];

            // check each pair of operation
            foreach (var twoOp in twoOps)
            {
                ops = twoOp.Split(','); // eg, Add,MulEvaluateOperation

                // ignore same ops, eg, Add,Add, 'cos we will apply the ops to the same args, hence it's a duplicate
                //if (ops.Length == 2 && ops[0] == ops[1])
                    //continue;

                // Fill tmp lists based on pair of operation
                foreach (var group in groups)
                {
                    if (group.Count() == 4)
                    {
                        double first = group[0];
                        double second = group[1];
                        double third = group[2];
                        double fourth = group[3];

                        tmp0.Add(MathUtils.Evaluate(ops[0], first, second)); // [14, 6, 5, 4] => 14+6
                        tmp1.Add(MathUtils.Evaluate(ops[1], third, fourth)); // [14, 6, 5, 4] => 5*4
                        tmp2.Add(MathUtils.Evaluate(ops[1], first, second)); // [8, 0.5, 16, 4] => 8/0.5=16; 8*0.5=4

                        tmp3.Add(MathUtils.Evaluate(ops[0], second, third)); // [6, 3, 3, 9] => 3+3=6
                        tmp4.Add(MathUtils.Evaluate(ops[1], second, third)); // [6, 3, 3, 9] => 3*3=9
                    }
                }

                // Match 1: check tmp lists to find a pattern and calculate next value (Todo: you can keep a flag above for SequenceEqual instead of checking it here)
                if (tmp0.SequenceEqual(tmp1) && groups.Last().Count() == 3)
                {
                    next = MathUtils.EvaluateInverseOp(ops[1], tmp0.Last(), groups.Last()[2]); // [14, 6, 5, x]
                    return next;
                }

                // Match 2: check if results of the two ops are within the same group
                var isMatch = true;
                for (var i = 0; i < groups.Count(); i++)
                {
                    isMatch = true;
                    var group = groups[i];
                    if (group.Count() == 4)
                    {
                        if(tmp0[i] != group[2] || tmp2[i] != group[3])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
                if (isMatch)
                {
                    if(groups.Last().Count() == 2)
                        next = MathUtils.Evaluate(ops[0], groups.Last()[0], groups.Last()[1]); // [14, 6, x, y] => x=14+6
                    else if(groups.Last().Count() == 3)
                        next = MathUtils.Evaluate(ops[1], groups.Last()[0], groups.Last()[1]); // [14, 6, 20, y] => y=14*6

                    return next;
                }

                // Match 3: check if results of the two ops are within the same group
                isMatch = true;
                for (var i = 0; i < groups.Count(); i++)
                {
                    isMatch = true;
                    var group = groups[i];
                    if (group.Count() == 4)
                    {
                        if (tmp3[i] != group[0] || tmp4[i] != group[3])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
                if (isMatch)
                {
                    if (groups.Last().Count() == 2)
                        next = MathUtils.EvaluateInverseOp(ops[0], groups.Last()[0], groups.Last()[1]); // [6, 3, x, y] => 3+x=6
                    else if (groups.Last().Count() == 3)
                        next = MathUtils.Evaluate(ops[1], groups.Last()[1], groups.Last()[2]); // [6, 3, 3, y] => 3*3=y

                    return next;
                }

                tmp0.Clear();
                tmp1.Clear();
                tmp2.Clear();
                tmp3.Clear();
                tmp4.Clear();
            }

            return next;
        }

        private static NPATResult EvaluateInterleavingLists(List<double> input, int recLevel)
        {
            recLevel++;
            Helpers.PrintMethod("Interleaving", input, recLevel);

            List<List<double>> twoLists = Helpers.TwoLists(input);
            List<List<double>> threeLists = Helpers.ThreeLists(input);
            List<List<double>> groupsOfThree = Helpers.GetGroups(input, 3);
            var listResults = new List<double?>();
            NPATResult result = new NPATResult() { };

            // if all lists are 2 items only, it's not enough
            if (twoLists.All(list => list.Count <= 2))
                return null;

            result.InputSeries = input;

            // CHECK: 2 lists
            foreach (var list in twoLists)
            {
                var r = GetNextNumber(list, recLevel);
                if (r != null && r.Values?.Count > 0 && r.Values[0] != null)
                {
                    listResults.Add(r.Values[0].Value);
                    result.SeriesNames.Add(r.SeriesName);
                }
            }

            if (listResults.Count() == 2)
            {
                result.Series1 = twoLists[0];
                result.Series2 = twoLists[1];
                result.MatchType += "Interleaving;";

                result.Values.Add(listResults[input.Count() % 2]);    // the mode 2 will pick the right list to return the next number from
                Helpers.PrintInput(input, "Interleaving - 2series", "", listResults[input.Count() % 2]);
                return result;
            }

            // CHECK: 3grp, 2 lists
            if (listResults.Count() != 2)   // 2 lists didn't find anything
            {
                // let's check 2 interleaving lists with groups of 3 in each
                // (1, 3, 3), (2, 8, 10), (3, 4, 12), (4, 8, x)                 => 1*3=3, 2+8=10, 3*4=12, 4+8=?
                // (6, 18, 21), (8, 11, 33), (2, 6, 9), (5, 8, 24), (7, 21, x)  => (6*3=18, 18+3=21), (8+3=11, 11*3=33)
                // we want to make sure that both interleaving lists return some result (even if different)
                var r1 = GroupsOfThree.Evaluate(groupsOfThree.Where((l, i) => i % 2 == 0).SelectMany(x => x).ToList(), recLevel); // (1, 3, 3), (3, 4, 12)
                NPATResult r2 = null;
                if ((r1 != null && r1.Comment == "CompleteSeries") || (r1?.Values?.Count > 0 && r1?.Values?[0] != null))
                    r2 = GroupsOfThree.Evaluate(groupsOfThree.Where((l, i) => i % 2 != 0).SelectMany(x => x).ToList(), recLevel); // (2, 8, 10), (4, 8, x) 
                if ((r2 != null && r2.Comment == "CompleteSeries") || (r2?.Values?.Count > 0 && r2?.Values?[0] != null))
                {
                    var res = r1.Comment == "CompleteSeries" ? r2 : r1;
                    Helpers.PrintInput(input, "Interleaving - 3grp", "", "");
                    return res;
                }
            }

            // CHECK: 3 lists
            if (listResults.Count() != 2)   // 2 lists didn't find anything
            {
                listResults.Clear();
                result.SeriesNames.Clear();

                // if all lists are 2 items only, it's not enough
                if (threeLists.All(list => list.Count <= 2))
                    return null;


                foreach (var list in threeLists)
                {
                    var r = GetNextNumber(list, recLevel);
                    if (r != null && r.Values?.Count > 0 && r.Values[0] != null)
                    {
                        listResults.Add(r.Values[0].Value);
                        result.SeriesNames.Add(r.SeriesName);
                    }
                }
                if (listResults.Count() != 3)
                {
                    listResults.Clear();
                    result.SeriesNames.Clear();
                }
                else
                {
                    result.Series1 = threeLists[0];
                    result.Series2 = threeLists[1];
                    result.Series3 = threeLists[2];

                    // 1, 9, 0, 2, 7, 1, 3, 5, 2, 4, 3  => 1 2 3 4,     9 7 5 3,    0 1 2 (3)
                    result.Values.Add(listResults[input.Count() % 3]);    // the mode 3 will pick the right list to return the next number from
                    Helpers.PrintInput(input, "Interleaving - 3series", "", listResults[input.Count() % 3]);
                    return result;
                }
            }

            return null;
        }

        private static NPATResult ProductOfDigitsGetNext(List<double> input)
        {
            NPATResult result = new NPATResult();
            bool allProds = true;
            var tmpList = input.Select(x=>Math.Abs(x)).ToList();

            if (input?.Count > 0 && input[0] < 10)
                return null;

            for (var padding = -5; padding <= 5; padding++)
            {
                allProds = true;
                for (var i = 0; i < tmpList.Count; i++)
                {
                    var prod = GetProductOfDigits((int)tmpList[i]);

                    if ((i + 1) < tmpList.Count && (prod + padding) != tmpList[i + 1])
                    {
                        allProds = false;
                        break;
                    }
                }

                if (allProds)
                {
                    result.Values.Add(GetProductOfDigits((int)tmpList.Last()) + padding);
                    break;
                }
            }

            return result;
        }
        private static double? GetProductOfDigits(int num)
        {
            // Get the product of digits
            double prod = 1;
            if (num == 0)
                prod = 0;

            while (num != 0)
            {
                prod *= num % 10;
                num /= 10;
            }
            
            return prod;
        }
    }

    public enum PatternComplexityOrder
    {
        SameNumbers,
        ConstantDiff,
        NaturalNumbers,
        Exponentials,
        Primes,
        StandardSeries,
        PaddedExponentials,
        Other
    }
}
