using System;
using System.Collections.Generic;
using System.Linq;
using NumberSeries;
using NUnit.Framework;

namespace NumberSeriesTests
{
    class GroupsOfThreeTests
    {
        [TestCase(new[] { 2, 1, 2, 3, 2, 9, 9, 0, 1, 1, 9d }, 0, ExpectedResult = 1)]           // a^b=c
        [TestCase(new[] { 2, 1, 3, 3, 2, 5, 9, 0, 9, 1, 9d }, 0, ExpectedResult = 10)]          // a+b=c
        [TestCase(new[] { 2, 1, 1, 5, 2, 3, 4, 6, -2, 7, 10d }, 0, ExpectedResult = -3)]        // a-b=c
        [TestCase(new[] { 3, 1, 2, 5, 4, 1, 9, 7, 2, 13, 9d }, 0, ExpectedResult = 4)]          // b+c=a
        [TestCase(new[] { 1, 3, 3, 3, 15, 5, 4, 28, 7, 8, 32d }, 0, ExpectedResult = 4)]        // b/a=c
        [TestCase(new[] { 2, 2, 3, 6, 1, 1, 7, 1, 0, 5, 2d }, 0, ExpectedResult = -3)]          // a*b+c=7
        [TestCase(new[] { 6, 3, 3, 12, 3, 1, 20, 5, 1, 14, 2d }, 0, ExpectedResult = -2)]       // a/b+c=7/**/
        [TestCase(new[] { 2, 2, 3, 2, 6, 1, 1, 1, 7, 5, 2d }, 0, ExpectedResult = 1.5)]         // a+b*c=8
        [TestCase(new[] { 3, 2, 2, 6, 2, 1, 7, 1, 1, 5, -2d }, 0, ExpectedResult = 2)]          // a*c+b=8
        [TestCase(new[] { 8, 2, 5, 6, 4, 5, 12, -2, 5, 5d }, 0, ExpectedResult = 5)]            // a+b=10, c=5
        [TestCase(new[] { 1, 8, 2, 2, 6, 4, 3, 12, -2, 4, 3d }, 0, ExpectedResult = 7)]         // b+c=10, a=1,2,3...
        [TestCase(new[] { 1, 8, 12, 2, 2, 3, 4, 2, 0.75, 4, 3d }, 0, ExpectedResult = 0.1875)]  // a^b*c=12
        [TestCase(new[] { 8, 1, 12, 2, 2, 3, 2, 4, 0.75, 3, 4d }, 0, ExpectedResult = 0.1875)]  // b^a*c=12
        [TestCase(new[] { 8, 12, 1, 2, 3, 2, 2, 0.75, 4, 3, 0.1875d }, 0, ExpectedResult = 4)]  // c^a*b=12
        [TestCase(new[] { 1, 1, 3, 2, 3, 7, 2, 9, 13, 3, 14, 19, 0, 23d }, 0, ExpectedResult = 29)]  // a+b=prime, c=prime/**/
        public double? GroupsOfThree_SimpleOperationShouldReturnNextNumber(double[] input, int recLevel)
        {
            var ops = new List<string>() { "^" };
            var res = GroupsOfThree.Evaluate(input.ToList(), recLevel, null);

            return Math.Round(res.Values[0].Value, 4);
        }
    }
}
