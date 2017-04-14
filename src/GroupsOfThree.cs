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
            NPATResult result = new NPATResult();
            NPATResult recursiveResult = null;
            List<NPATResult> results = new List<NPATResult>();
            result.MatchType += groupType;

            var firstAndSecondResults = new List<double>();
            var secondAndThirdResults = new List<double>();
            var thirdAndFirstResults = new List<double>();
            var listOfOpResults = new List<List<double>>();
            listOfOpResults.Add(firstAndSecondResults);
            listOfOpResults.Add(secondAndThirdResults);
            listOfOpResults.Add(thirdAndFirstResults);

            List<double> intermediateList1 = null;   // list formed by combining results with another item in the group, or by taking the leftover items from the groups (leftover items are those that didn't take part in the operation)
            List<double> intermediateList2 = null;
            NPATResult[,] resultset = new NPATResult[6,3];
            /*
             [[r1, null, null],     => firstAndSecondResults + c
              [r2, null, null],     => secondAndThirdResults + a
              [r3, null, null],     => thirdAndFirstResults + b
              [r4, c1, null],       => firstAndSecondResults, c
              [r5, c2, null],       => secondAndThirdResults, a
              [r6, c3, null]]       => thirdAndFirstResults, b
             */

            // check each operation
            foreach (var operation in operations)
            {
                // fill tmp lists per group based on an operation => {(1, 2, 3), (4, 5, 6), 7}: {1*3, 4*6, 7}
                ApplyOperationToAllGroups(operation, groupsOfThree, firstAndSecondResults, secondAndThirdResults, thirdAndFirstResults);

                // 1 result is calculated by forming a new list made from the results and the remaining items:
                // 2, 2, 3, 6, 1, 1, 7, 1, 0, 5, 2 => a*b+c=7 => (4, 3), (6, 1), (7, 0), (10, x)
                for (var i = 0; i < 3; i++)
                {
                    var idx = (i + 2) % 3;
                    intermediateList1 = CreateNewList(listOfOpResults[i], groupsOfThree, idx);
                    resultset[i, 0] = GetNextNumber(intermediateList1, recLevel + 1);
                }

                // 2 SEPARATE results are calculated, one using the results from any two items, another using the remaining items:
                // 1, 8, 2, 2, 6, 4, 3, 12, -2, 4, 3 => b+c=10, a=1,2,3...
                for (var i = 0; i < 3; i++)
                {
                    var idx = (i + 2) % 3;
                    resultset[i+3, 0] = GetNextNumber(listOfOpResults[i], recLevel + 1);
                    intermediateList2 = CreateNewList(groupsOfThree, idx);
                    resultset[i+3, 1] = GetNextNumber(intermediateList2, recLevel + 1);
                }

                //  2, 2, 3, 2, 6, 1, 1, 1, 7, 5, 2 => 6 2 6 2 7 1 5 r
                // match found, now evaluate the result and add a new result entry to the resultset
                EvaluateNext(groupsOfThree, resultset, operation);
                AddResultsToList(results, resultset);

                #region OLD
                // add result to list ()
                /*if (nextNumber != null)
                {

                    result = new NPATResult() { PatternComplexityOrder = recursiveResult.PatternComplexityOrder, RecursiveDepth = recursiveResult.RecursiveDepth };
                    result.Values.Add(nextNumber);
                    results.Add(result);
                }
                else
                {
                    if (groupsOfThree.Last().Count == 3)
                    {
                        result.IsCompleteSeries = true;  // this is for the 2lists,3grp case where one list will be a complete series
                        return result;
                    }
                }*/ 
                #endregion

                firstAndSecondResults.Clear();
                secondAndThirdResults.Clear();
                thirdAndFirstResults.Clear();
            }

            if (results.Count > 0)
            {
                var resultsOrdered = results.Where(r => r?.Values.Count > 0 && !double.IsInfinity(r.Values[0].Value))
                                            .OrderBy(r => 2 * r.RecursiveDepth + r.PatternComplexityOrder); // recursive depth is given lower precedence by giving it extra weight (*2)

                return resultsOrdered.FirstOrDefault();
            }

            return null;
        }

        private static void AddResultsToList(List<NPATResult> results, NPATResult[,] resultset)
        {
            for (var i = 0; i < 6; i++)
            {
                if (resultset[i, 2] != null)
                {
                    var r = resultset[i, 2];
                    r.PatternComplexityOrder = resultset[i, 0].PatternComplexityOrder;  // ERROR: null ref!!!
                    r.RecursiveDepth = resultset[i, 0].RecursiveDepth;
                    results.Add(r);
                }
            }
        }

        private static void EvaluateNext(List<List<double>> groupsOfThree, NPATResult[,] resultset, string operation)
        {
            double? nextNumber = null;
            int rowZero = 0, rowOne = 1, rowTwo = 2, rowThree = 3, rowFour = 4, rowFive = 5;

            // TODO: IMPORTANT: remove all 'else' parts, these should all be 'if' only. We need to preserve all results and find the best match at the end
            // Add the results in the array itself
            if (groupsOfThree.Last().Count == 1)
            {
                if (resultset[rowThree, 0]?.Values.Count > 0 && resultset[rowThree, 1]?.Values.Count > 0)        // (a,b)=r => {(8,2),5,(6,4),5,(12,-2),5,(5,x)} => a+b=10, c=5
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[0], operation, out nextNumber, resultset[rowThree, 0].Values[0].Value);
                    resultset[rowThree, 2] = new NPATResult() { Values = new List<double?>() { nextNumber } };
                }
            }
            else if (groupsOfThree.Last().Count == 2)
            {
                if (resultset[rowFour, 0]?.Values.Count > 0 && resultset[rowFour, 1]?.Values.Count > 0)   // (b,c)=r => {1,(8,2),2,(6,4),3,(12,-2),4,(5,x)} => b+c=10, a=1,2,3...
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[1], operation, out nextNumber, resultset[rowFour, 0].Values[0].Value);
                    resultset[rowFour, 2] = new NPATResult() { Values=new List<double?>() { nextNumber }};
                }
                else if (resultset[rowFive, 0]?.Values.Count > 0 && resultset[rowFive, 1]?.Values.Count > 0)   // (a,c)=r => {(8),3,(2),(6),3,(4),(12),3,(-2),(5),3,(x)} => a+c=10
                {
                    MathUtils.SolveEquation(out nextNumber, operation, groupsOfThree.Last()[0], resultset[rowFive, 0].Values[0].Value);
                    resultset[rowFive, 2] = new NPATResult() { Values = new List<double?>() { nextNumber } };
                }
                else if (resultset[rowZero, 0]?.Values.Count > 0)   // {2,2,3,6,1,1,7,1,0,5,2,x} => a*b+c=7 => (a,b),c = r,c => {(4,3),(6,1),(7,0),(10,x)}
                {
                    nextNumber = resultset[rowZero, 0].Values[0].Value;
                    resultset[rowZero, 2] = new NPATResult() { Values = new List<double?>() { nextNumber } };
                }
                else if (resultset[rowOne, 0]?.Values.Count > 0)   // {2,2,3,2,6,1,1,1,7,5,2,x} => a+b*c=8 => a,(b,c) = r,a => {(6,2),(6,2),(7,1),(5,y)}
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[1], operation, out nextNumber, resultset[rowOne, 0].Values[0].Value);
                    resultset[rowOne, 2] = new NPATResult() { Values = new List<double?>() { nextNumber } };
                }
                else if (resultset[rowTwo, 0]?.Values.Count > 0)   // {3,2,2,6,2,1,7,1,1,5,-2,x} => a*c+b=8 => (a),b,(c) = r,b => {(6,2),(6,2),(7,1),(5,y)} 
                {
                    MathUtils.SolveEquation(groupsOfThree.Last()[0], operation, out nextNumber, resultset[rowTwo, 0].Values[0].Value);
                    resultset[rowTwo, 2] = new NPATResult() { Values = new List<double?>() { nextNumber } };
                }
            }
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

        private static void ApplyOperationToAllGroups(string operation, List<List<double>> groupsOfThree, List<double> firstAndSecondResults, List<double> secondAndThirdResults, List<double> thirdAndFirstResults)
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
