using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    public class GroupsOfThree
    {
        public static NPATResult Evaluate(List<double> input, int recLevel, Func<List<double>, int, NPATResult> getNext = null, List<string> ops = null)
        {
            getNext = getNext ?? NPAT.GetNextNumber;

            if (input.Count < 6)
                return null;

            List<List<double>> primaryGroupsOfThree = Helpers.GetGroups(input, 3);
            List<List<double>> secondaryGroupsOfThree = input.Count > 6 ? Helpers.GetGroups(input, 3, true) : null;   // it doesn't make sense to do overlapping 3grp with less than 6 items
            var result = EvaluteGroupsOfThree_OneAndTwoOps(primaryGroupsOfThree, "3group", recLevel, getNext, ops);

            if (secondaryGroupsOfThree != null &&
               (result?.Values?.Count == 0 || (result?.Values?.Count > 0 && result?.Values?[0] == null)))
                result = EvaluteGroupsOfThree_OneAndTwoOps(secondaryGroupsOfThree, "3group, overlapping", recLevel, getNext, ops);

            return result;
        }

        /*private static NPATResult EvaluteGroupsOfThree_OneAndTwoOps(List<List<double>> groupsOfThree, string groupType, int recLevel, Func<List<double>, int, NPATResult> GetNextNumber = null, List<string> operations = null)
        {
            GetNextNumber = GetNextNumber ?? NPAT.GetNextNumber;
            operations = operations ?? MathUtils.operations;
            // NOTE: we are only doing one cross-join to get (a@b)(b@c), and not two to get (a@b)(b@c)(c@a), 'cos heuristically this pattern will be very rare.
            var crossjoin = operations.SelectMany(o => operations, (o1, o2) => o1 + "," + o2);  // { "Add,Mul", "Mul,Sub", "Add,Div", "Sub,Mul", etc...}
            double? next = null;
            NPATResult result = new NPATResult();
            result.MatchType += groupType;

            var firstAndSecondResults = new List<double>();
            var secondAndThirdResults = new List<double>();
            var thirdAndFirstResults = new List<double>();
            string[] ops = new string[0];
            var firstAndSecondOperation = "";
            var secondAndThirdOperation = "";
            var thirdAndFirstOperation = "";
            var listOperations = crossjoin.ToList();

            // {2,3,11} {4,1,9} {3,4,17}
            // a*b-c=-5
            // {4,3,7} {6,2,8} {0.5,24,-14}
            // a*b=12, b+c=10

            // check each pair of operation
            foreach (var pairOperation in listOperations)
            {
                ops = pairOperation.Split(','); // eg Add,Mul
                firstAndSecondOperation = ops[0];
                secondAndThirdOperation = ops[1];

                // fill tmp lists per group based on a pair of operations {2,3,5}=>{2+3,3*5}; {5,6,3}=>{5+6,6*3}, etc, same pair applied to all groups
                ApplyOperationPairToAllGroups(groupsOfThree, firstAndSecondResults, secondAndThirdResults, firstAndSecondOperation, secondAndThirdOperation);

                // Check 1: see if the results of the operation form a pattern

                NPATResult recur1 = GetNextNumber(firstAndSecondResults, recLevel + 1);
                NPATResult recur2 = GetNextNumber(secondAndThirdResults, recLevel + 1);
                NPATResult recur3 = GetNextNumber(thirdAndFirstResults, recLevel + 1);
                NPATResult recur4 = GetNextNumber(CreateNewList(firstAndSecondResults, groupsOfThree, 2), recLevel + 1);
                NPATResult recur5 = GetNextNumber(CreateNewList(secondAndThirdResults, groupsOfThree, 0), recLevel + 1);
                NPATResult recur6 = GetNextNumber(CreateNewList(thirdAndFirstResults, groupsOfThree, 1), recLevel + 1);


                //if ((firstAndSecondResults.Count > 1 && secondAndThirdResults.Count > 1) &&
                //  (firstAndSecondResults.Distinct().Count() == 1 && secondAndThirdResults.Distinct().Count() == 1))
                if (recur1?.Values.Count > 0 && recur2?.Values.Count > 0)
                {
                    // match found, now evaluate the next value
                    if (groupsOfThree.Last().Count == 1)
                    {
                        next = MathUtils.Evaluate(firstAndSecondOperation, groupsOfThree.Last()[0], recur1.Values[0].Value);
                        Helpers.PrintInput(groupsOfThree.SelectMany(x => x).ToList(), "3grp", firstAndSecondOperation, recur1.Values[0].Value);
                    }
                    else if (groupsOfThree.Last().Count == 2)
                    {
                        next = MathUtils.Evaluate(secondAndThirdOperation, groupsOfThree.Last()[1], recur2.Values[0].Value);
                        Helpers.PrintInput(groupsOfThree.SelectMany(x => x).ToList(), "3grp", secondAndThirdOperation, recur2.Values[0].Value);
                    }

                    if (next != null)
                    {
                        var recur = groupsOfThree.Last().Count == 1 ? recur1 : recur2;
                        result = new NPATResult() { PatternComplexityOrder = recur.PatternComplexityOrder, RecursiveDepth = recur.RecursiveDepth };
                        result.Values.Add(next);
                        return result;  // TODO: we want to evaluate all operations instead of returning from here, 'cos we don't know which one is the simplest
                    }
                    else
                    {
                        if (groupsOfThree.Last().Count == 3)
                        {
                            result.IsCompleteSeries = true;  // this is for the 2lists,3grp case where one list will be a complete series
                            return result;
                        }
                    }
                }
                // Do you need all 3 items in the group to take part? 
                else
                {
                    // Check 2: see if each group contains its operation result (this reduces the problem to one op)
                    // We can create new lists by injecting the 3rd or 1st items into the results from recur1 and recur2, then
                    // check if they form a pattern.
                    // TODO: Only 1st and 3rd items are checked for matches, middle item is not checked yet.
                    var second = false;
                    //next = GroupContainsGetNext(groupsOfThree, firstAndSecondResults, firstAndSecondOperation, 2);
                    //recur4 = GetNextNumber(CreateNewList(firstAndSecondResults, groupsOfThree, 2), recLevel + 1);
                    recur5 = null;
                    if (recur4 == null)
                    {
                        recur5 = GetNextNumber(CreateNewList(secondAndThirdResults, groupsOfThree, 0), recLevel + 1);
                        second = true;
                    }

                    if (recur4?.Values.Count > 0 || recur5?.Values.Count > 0)
                    {
                        var recur = recur4 ?? recur5;
                        if (groupsOfThree.Last().Count == 1)
                        {
                            next = recur.Values[0].Value;
                        }
                        if (groupsOfThree.Last().Count == 2)
                        {
                            next = recur.Values[0].Value;
                        }

                        result.Values.Add(next);
                        Helpers.PrintInput(groupsOfThree.SelectMany(x => x).ToList(), "3grp - grp contains", second ? secondAndThirdOperation : firstAndSecondOperation,
                            second ? secondAndThirdResults.First() : firstAndSecondResults.First());

                        return result;
                    }
                    if (groupsOfThree.Last().Count == 3)
                    {
                        result.IsCompleteSeries = true;  // this is for the 2lists,3grp case where one list will be a complete series
                        return result;
                    }
                }

                firstAndSecondResults.Clear();
                secondAndThirdResults.Clear();
            }

            return null;
        }*/
        private static NPATResult EvaluteGroupsOfThree_OneAndTwoOps(List<List<double>> groupsOfThree, string groupType, int recLevel, Func<List<double>, int, NPATResult> GetNextNumber = null, List<string> operations = null)
        {
            GetNextNumber = GetNextNumber ?? NPAT.GetNextNumber;
            operations = operations ?? MathUtils.operations;
            var crossjoin = operations.SelectMany(o => operations, (o1, o2) => o1 + "," + o2);  // { "Add,Mul", "Mul,Sub", "Add,Div", "Sub,Mul", etc...}
            double? next = null;
            NPATResult result = new NPATResult();
            NPATResult recur = null;
            List<NPATResult> results = new List<NPATResult>();
            result.MatchType += groupType;

            var firstAndSecondResults = new List<double>();
            var secondAndThirdResults = new List<double>();
            var thirdAndFirstResults = new List<double>();
            string[] ops = new string[0];
            var listOperations = MathUtils.operations;
            List<double> intermediateList = null;   // list formed by combining results with another item in the group, or by taking the leftover items from the groups (leftover items are those that didn't take part in the operation)

            // check each pair of operation
            foreach (var operation in listOperations)
            {
                // fill tmp lists per group based on a pair of operations {2,3,5}=>{2+3,3*5}; {5,6,3}=>{5+6,6*3}, etc, same pair applied to all groups
                ApplyOperationToAllGroups(groupsOfThree, firstAndSecondResults, secondAndThirdResults, thirdAndFirstResults, operation);

                // Check 1: see if the results of the operation form a pattern
                NPATResult recur1 = GetNextNumber(firstAndSecondResults, recLevel + 1);
                intermediateList = CreateNewList(groupsOfThree, 2);
                NPATResult recur1Extra = GetNextNumber(intermediateList, recLevel + 1);

                NPATResult recur2 = GetNextNumber(secondAndThirdResults, recLevel + 1);
                intermediateList = CreateNewList(groupsOfThree, 0);
                NPATResult recur2Extra = GetNextNumber(intermediateList, recLevel + 1);

                NPATResult recur3 = GetNextNumber(thirdAndFirstResults, recLevel + 1);
                intermediateList = CreateNewList(groupsOfThree, 1);
                NPATResult recur3Extra = GetNextNumber(intermediateList, recLevel + 1);

                intermediateList = CreateNewList(firstAndSecondResults, groupsOfThree, 2);
                NPATResult recur4 = GetNextNumber(intermediateList, recLevel + 1);

                intermediateList = CreateNewList(secondAndThirdResults, groupsOfThree, 0);
                NPATResult recur5 = GetNextNumber(intermediateList, recLevel + 1);

                intermediateList = CreateNewList(thirdAndFirstResults, groupsOfThree, 1);
                NPATResult recur6 = GetNextNumber(intermediateList, recLevel + 1);

                var debug = "";
                // match found, now evaluate the result
                if (recur1?.Values.Count > 0 && groupsOfThree.Last().Count == 1 && recur1Extra?.Values.Count > 0)        // (a,b)=r => {(8,2),5,(6,4),5,(12,-2),5,(5,x)} => a+b=10
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[0], operation, out next, recur1.Values[0].Value);
                    recur = recur1;
                    debug = $"recur1: {operation}";
                }
                else if (recur2?.Values.Count > 0 && groupsOfThree.Last().Count == 2 && recur2Extra?.Values.Count > 0)   // (b,c)=r => {1,(8,2),2,(6,4),3,(12,-2),4,(5,x)} => b+c=10
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[1], operation, out next, recur2.Values[0].Value);
                    recur = recur2;
                    debug = $"recur2: {operation}";
                }
                else if (recur3?.Values.Count > 0 && groupsOfThree.Last().Count == 2 && recur3Extra?.Values.Count > 0)   // (a,c)=r => {(8),3,(2),(6),3,(4),(12),3,(-2),(5),3,(x)} => a+c=10
                {
                    MathUtils.SolveEquation(out next, operation, groupsOfThree.Last()[0], recur3.Values[0].Value);
                    recur = recur3;
                    debug = $"recur3: {operation}";
                }
                else if (recur4?.Values.Count > 0 && groupsOfThree.Last().Count == 2)   // {2,2,3,6,1,1,7,1,0,5,2,x} => a*b+c=7 => (a,b),c = r,c => {(4,3),(6,1),(7,0),(10,x)}
                {
                    next = recur4.Values[0].Value;
                    recur = recur4;
                    debug = $"recur4: {operation}";
                }
                else if (recur5?.Values.Count > 0 && groupsOfThree.Last().Count == 2)   // {2,2,3,2,6,1,1,1,7,5,2,x} => a+b*c=8 => a,(b,c) = r,a => {(6,2),(6,2),(7,1),(5,y)}
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[1], operation, out next, recur5.Values[0].Value);
                    recur = recur5;
                    debug = $"recur5: {operation}";
                }
                else if (recur6?.Values.Count > 0 && groupsOfThree.Last().Count == 2)   // {3,2,2,6,2,1,7,1,1,5,-2,x} => a*c+b=8 => (a),b,(c) = r,b => {(6,2),(6,2),(7,1),(5,y)} 
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[0], operation, out next, recur6.Values[0].Value);
                    recur = recur6;
                    debug = $"recur6: {operation}";
                }

                // add result to list
                if (next != null)
                {
                        
                    result = new NPATResult() { PatternComplexityOrder = recur.PatternComplexityOrder, RecursiveDepth = recur.RecursiveDepth };
                    result.Values.Add(next);
                    result.Comment = $"{debug}";
                    results.Add(result);
                }
                else
                {
                    if (groupsOfThree.Last().Count == 3)
                    {
                        result.IsCompleteSeries = true;  // this is for the 2lists,3grp case where one list will be a complete series
                        return result;
                    }
                }

                firstAndSecondResults.Clear();
                secondAndThirdResults.Clear();
                thirdAndFirstResults.Clear();
            }

            if (results.Count > 0)
            {
                var resultsOrdered = results.Where(r=>r?.Values.Count > 0).OrderBy(r => 2 * r.RecursiveDepth + r.PatternComplexityOrder); // recursive depth is given lower precedence by giving it extra weight (*2)
                return resultsOrdered.FirstOrDefault();
            }
            return null;
        }

        private static List<double> CreateNewList(List<List<double>> groupsOfThree, int groupIndex)
        {
            List<double> list = new List<double>();

            foreach(var group in groupsOfThree)
            {
                if(groupIndex < group.Count)
                    list.Add(group[groupIndex]);
            }

            return list;
        }

        // Create a new list from the results and the group items that were not in the result: {a,b,c} => {r,c}, {r,a}, {r,b}, assuming a@b=r, b@c=r and a@c=r respectively
        private static List<double> CreateNewList(List<double> results, List<List<double>> groupsOfThree, int groupIndex)
        {
            var newList = new List<double>();

            if (results.Count == groupsOfThree.Count || results.Count == groupsOfThree.Count-1) // last group can have 1 or 2 items, so results can be one less item
            {
                for(var i=0; i < results.Count; i++)
                {
                    var group = groupsOfThree[i];
                    if (group.Count == 3)
                    {
                        newList.Add(results[i]);
                        newList.Add(group[groupIndex]);
                    }
                    else if (group.Count == 2)  // the last group
                    {
                        newList.Add(results[i]);
                    }
                }

                // {3,2,2,1,6,1,0,1,7,5,2,x} => secondAndThird or thirdAndFirst
                if (results.Count < groupsOfThree.Count && groupsOfThree.Last().Count == 2 && (groupIndex == 0 || groupIndex == 1))
                {
                    // there is no result for secondAndThird or thirdAndFirst, so we only add the item from the group
                    newList.Add(groupsOfThree.Last()[groupIndex]);
                }
            }

            return newList;
        }

        private static void ApplyOperationToAllGroups(List<List<double>> groupsOfThree, List<double> firstAndSecondResults, List<double> secondAndThirdResults, List<double> thirdAndFirstResults, string operation)
        {
            foreach (var group in groupsOfThree)
            {
                if (group.Count == 2 || group.Count == 3)
                {
                    double first = group[0];
                    double second = group[1];
                    double third = group.Count == 3 ? group[2] : 0;

                    firstAndSecondResults.Add(MathUtils.Evaluate(operation, first, second));
                    if (group.Count == 3)
                    {
                        secondAndThirdResults.Add(MathUtils.Evaluate(operation, second, third));
                        thirdAndFirstResults.Add(MathUtils.Evaluate(operation, third, first));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="results"></param>
        /// <param name="op"></param>
        /// <param name="indexToCheck">This is the index in each group, for eg, 0,1,2 for 3grp</param>
        /// <returns></returns>
        private static double? GroupContainsGetNext(List<List<double>> groups, List<double> results, string op, int indexToCheck)
        {
            bool inAllGroups = false;
            double? next = null;
            if (indexToCheck > 2) return null;

            for (var i = 0; i < groups.Count(); i++)
            {
                var group = groups[i];
                if (group.Count() == 3)
                {
                    //inGroup = group.Contains(tmp[i]);
                    inAllGroups = group[indexToCheck] == results[i];
                    if (!inAllGroups) break;
                }
            }
            if (inAllGroups)
            {
                if (groups.Last().Count() == 2)
                {
                    // TODO: fix for Mul (32, 0.2, 160), (15, 0.5, 30), (28, 0.7, x)
                    next = indexToCheck == 0 && op == "/"
                        ? MathUtils.Evaluate(op, groups.Last()[1], groups.Last()[0])     // (3, 6 / 2), (5, 20 / x)
                        : MathUtils.Evaluate(op, groups.Last()[0], groups.Last()[1]);    // (6 / 3, 2), (20 / 5, x)
                    return next;
                }
            }

            // squared
            if (!inAllGroups)
            {
                for (var i = 0; i < groups.Count(); i++)
                {
                    var group = groups[i];
                    if (group.Count() == 3)
                    {
                        //inGroup = group.Contains(Math.Pow(tmp[i], 2));
                        inAllGroups = group[indexToCheck] == System.Math.Pow(results[i], 2);
                        if (!inAllGroups) break;
                    }
                }
            }
            if (inAllGroups)
            {
                if (groups.Last().Count() == 2)
                {
                    next = System.Math.Pow(MathUtils.Evaluate(op, groups.Last()[0], groups.Last()[1]), 2);
                    return next;
                }
            }

            // cubed
            if (!inAllGroups)
            {
                for (var i = 0; i < groups.Count(); i++)
                {
                    var group = groups[i];
                    if (group.Count() == 3)
                    {
                        //inGroup = group.Contains(Math.Pow(tmp[i], 3));
                        inAllGroups = group[indexToCheck] == System.Math.Pow(results[i], 3);
                        if (!inAllGroups) break;
                    }
                }
            }
            if (inAllGroups)
            {
                if (groups.Last().Count() == 2)
                {
                    next = System.Math.Pow(MathUtils.Evaluate(op, groups.Last()[0], groups.Last()[1]), 3);
                    return next;
                }
            }

            // padded
            if (!inAllGroups && results.Count > 1)  // we want to check more than 1 group to be confident
            {
                next = GroupContainsPadded(groups, results, op, "+", false, indexToCheck);
                if (next == null)
                    next = GroupContainsPadded(groups, results, op, "+", true, indexToCheck);
                if (next == null)
                    next = GroupContainsPadded(groups, results, op, "*", false, indexToCheck);
                if (next == null)
                    next = GroupContainsPadded(groups, results, op, "*", true, indexToCheck);

                if (next != null)
                    return next;
            }

            return next;
        }

        private static double? GroupContainsPadded(List<List<double>> groups, List<double> tmp, string op, string opVariations, bool incrementalPadding, int indexToCheck)
        {
            bool inAllGroups = false;
            double? next = null;
            int j = 0;

            // simple 2-padding:    {2, 3, 7} {3, 4, 9}             => (2+3)+2 = 7; (3+4)+2 = 9
            // incremental padding: {2, 3, 5} {3, 4, 8} {6, 5, 13}  => (2+3)+0 = 5; (3+4)+1 = 8; (6+5)+2 = 13
            for (var v = -5; v <= 5; v++)
            {
                for (var i = 0; i < groups.Count(); i++)
                {
                    var group = groups[i];
                    if (group.Count() == 3)
                    {
                        if (opVariations == "+")
                            inAllGroups = group[indexToCheck] == tmp[i] + (v + j);
                        else if (opVariations == "*")
                            inAllGroups = group[indexToCheck] == tmp[i] * (v + j);

                        if (incrementalPadding)
                            j++;

                        if (!inAllGroups) break;
                    }
                }

                if (inAllGroups)
                {
                    if (groups.Last().Count() == 2)
                    {
                        if (opVariations == "+")
                            next = MathUtils.Evaluate(op, groups.Last()[0], groups.Last()[1]) + (v + j);   // Todo: check order for op == "/"
                        else if (opVariations == "*")
                            next = MathUtils.Evaluate(op, groups.Last()[0], groups.Last()[1]) * (v + j);   // Todo: check order for op == "/"

                        return next;
                    }
                }
            }

            return next;
        }
    }
}
