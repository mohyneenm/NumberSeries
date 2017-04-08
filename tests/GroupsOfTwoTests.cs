using FluentAssertions;
using NumberSeries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberSeriesTests
{
    public class GroupsOfTwoTests
    {
        //var GetNextNumber = Setup(null, 0, null, 0);    // stub
        public static Func<List<double>, int, NPATResult> Setup(List<double> values1, double result1, List<double> values2, double result2)
        {
            Func<List<double>, int, NPATResult> GetNextNumber = (list, rec) =>
            {
                var result = new NPATResult();
                if (values1 == null && values2 == null)
                {
                    return null;
                }
                else if (list.SequenceEqual(values1))
                {
                    result.Values.Add(result1);
                    Console.WriteLine("returning 4");
                    return result;
                }
                else if (list.SequenceEqual(values2))
                {
                    result.Values.Add(result2);
                    Console.WriteLine("returning 9");
                    return result;
                }
                return null;
            };

            return GetNextNumber;
        }

        [TestCase(new[] { 1, 1, 4, 8, 14, 42, 51, 204d }, ExpectedResult = 216)]            // *n,+3n
        [TestCase(new[] { 3, 3, 1, 2, -2, -6, -12, -48d }, ExpectedResult = -56)]           // *n,-2n
        [TestCase(new[] { 3, 9, 4, 16, 6, 36, 21, 441d }, ExpectedResult = 421)]            // n^2,-5n
        [TestCase(new[] { 9, 4, 16, 6, 36, 21, 441d }, ExpectedResult = 421)]               // n^2,-5n
        //[TestCase(new[] { 48, 24, 35, 7, 16, 8, 75, 15, 80d }, ExpectedResult = 40)]        // n/2,n/5; 2grp+interleaving
        public double? GroupsOfTwo_Secondary2GroupsVariableFactors(double[] input)
        {
            var res = GroupsOfTwo.Evaluate(input.ToList(), 0);

            return res.Values[0];
        }

        [TestCase(new[] { 507, 169, 248, 62, 36, 12, 168, 42, 168d }, ExpectedResult = 56)] // /3,/4
        public double? GroupsOfTwo_2(double[] input)
        {
            var res = GroupsOfTwo.Evaluate(input.ToList(), 0);

            return Math.Round(res.Values[0].Value, 4);
        }

        [TestCase(new[] { 2, 7, 6, 3, 8, 1, 5d }, ExpectedResult = 4)]              // +3
        [TestCase(new[] { 2, 2, 3, 3, 5, 5, 6d }, ExpectedResult = 6)]              // +0
        [TestCase(new[] { 2, 5, 3, 6, 5, 8, 6d }, ExpectedResult = 9)]              // +3
        [TestCase(new[] { 2, -1, 3, 0, 5, 2, 6d }, ExpectedResult = 3)]             // -3
        [TestCase(new[] { -2, -5, -3, -6, 1, -2, -7d }, ExpectedResult = -10)]      // -3
        [TestCase(new[] { 22, 21, 34, 33, 45d }, ExpectedResult = 44)]              // -1
        [TestCase(new[] { 2, 6, 3, 9, 5, 15, 6d }, ExpectedResult = 18)]            // *3
        [TestCase(new[] { 12, 48, 96, 384, 768d }, ExpectedResult = 3072)]          // *4
        [TestCase(new[] { 6, 2, 9, 3, 15, 5, 18d }, ExpectedResult = 6)]            // /3
        [TestCase(new[] { -6, -4, 9, 6, -15, -10, -12d }, ExpectedResult = -8)]     // /1.5
        [TestCase(new[] { 7, 9.2, 3, 5.2, 5.2, 7.4, 9.5d }, ExpectedResult = 11.7)] // +2.2
        [TestCase(new[] { 3, 4.5, 2, 3, 7.5, 11.25, 6.2d }, ExpectedResult = 9.3)]  // *1.5
        [TestCase(new[] { 5, 25, 4, 16, 3, 9, 2d }, ExpectedResult = 4)]            // ^2
        [TestCase(new[] { 4, 64, 1, 1, 3, 27, 2d }, ExpectedResult = 8)]            // ^3
        [TestCase(new[] { 0, 1, 1, 1, 1, 2, 1, 3, 3, 2, 1d }, ExpectedResult = 5)]  // sum: 1, 2, 3, 4
        //[TestCase(new[] { 2, 1, 2, 2, 3, 9, 2, 8, 1, 1, 3d }, ExpectedResult = 243)]// ^0, ^1, ^2, ^3, ^4 => TODO
        //[TestCase(new[] { 5, 25, 4, 16, 3, 9, 2, 4d }, ExpectedResult = 1)]            // ^2: TODO: interleaving; leading items form a series
        public double? GroupsOfTwo_Primary2Groups(double[] input)           
        {
            var res = GroupsOfTwo.Evaluate(input.ToList(), 0);

            return Math.Round(res.Values[0].Value, 4);
        }

        [TestCase(new[] { 2, 5, 6, 9, 10, 13, 14, 17d }, ExpectedResult = 18)]      // +3,+1
        [TestCase(new[] { 12, 48, 96, 384, 768, 3072d }, ExpectedResult = 6144)]    // *4,*2
        [TestCase(new[] { 2, 4, 12, 14, 42, 44d }, ExpectedResult = 132)]           // +2,*3
        [TestCase(new[] { 2, 6, 3, 9, 4.5, 13.5d }, ExpectedResult = 6.75)]         // *3,/2
        public double? GroupsOfTwo_Secondary2GroupsConstFactors(double[] input)
        {
            var res = GroupsOfTwo.Evaluate(input.ToList(), 0);

            return Math.Round(res.Values[0].Value, 4);
        }
    }
}
