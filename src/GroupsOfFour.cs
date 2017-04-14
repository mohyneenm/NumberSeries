using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    public class GroupsOfFour
    {
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
                if (inAllGroups && groups.Last().Count() == 3)
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
                        if (tmp0[i] != group[2] || tmp2[i] != group[3])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
                if (isMatch)
                {
                    if (groups.Last().Count() == 2)
                        next = MathUtils.Evaluate(ops[0], groups.Last()[0], groups.Last()[1]); // [14, 6, x, y] => x=14+6
                    else if (groups.Last().Count() == 3)
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
    }
}
