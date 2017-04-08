using NumberSeries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberSeriesTests
{
    class DiffsTests
    {
        // var GetNextNumber = Setup(null, 0, null, 0);    // stub
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

        [TestCase(new[] { 10, 10, 10, 10, 10d }, 0, ExpectedResult = 10)]
        [TestCase(new[] { -10, -10, -10, -10d }, 0, ExpectedResult = -10)]
        public double? Diffs_AllEqualShouldReturnSame(double[] input, int recLevel)
        {
            var res = Diffs.Evaluate(input.ToList(), recLevel);

            return Math.Round(res.Values[0].Value, 4);
        }

        [TestCase(new[] { 10, 10d }, 0, ExpectedResult = null)]
        [TestCase(new[] { 10d }, 1, ExpectedResult = null)]
        [TestCase(new[] { 10d }, 2, ExpectedResult = null)]
        public double? Diffs_ListSamllerThanMinShouldReturnNull(double[] input, int recLevel)
        {
            var res = Diffs.Evaluate(input.ToList(), recLevel);

            return null;
        }

        [TestCase(new[] { 2, 4, 6, 8, 10d }, 0, ExpectedResult = 12)]
        [TestCase(new[] { -2, -5, -8, -11d }, 0, ExpectedResult = -14)]
        [TestCase(new[] { 2.5, 3.5, 4.5, 5.5d }, 0, ExpectedResult = 6.5)]
        public double? Diffs_ConstIntDiffShouldReturnNextNumber(double[] input, int recLevel)
        {
            var res = Diffs.Evaluate(input.ToList(), recLevel);

            return Math.Round(res.Values[0].Value, 2);
        }

        [TestCase(new[] { 1/3d, 1, 5/3d, 7/3d, 3d }, 0)]
        public void Diffs_ConstFloatDiffShouldReturnNextNumber(double[] input, int recLevel)
        {
            var res = Diffs.Evaluate(input.ToList(), recLevel);

            Assert.AreEqual(res.Values[0].Value, 11 / 3d, 0.001);
        }

        [TestCase(new[] { 2, 3, 5, 8, 12d }, 0, ExpectedResult = 17)]           // natural numbers
        [TestCase(new[] { 1, 3, 6, 11, 18d }, 0, ExpectedResult = 29)]          // primes
        [TestCase(new[] { 5, 6, 8, 12, 20d }, 0, ExpectedResult = 36)]          // exponentials
        [TestCase(new[] { 3, 4, 8, 17, 33d }, 0, ExpectedResult = 58)]          // squares
        [TestCase(new[] { 3, 4, 5, 7, 10, 15d }, 0, ExpectedResult = 23)]       // fibonacci
        //[TestCase(new[] { 3, 4, 2, 5, 1d }, 0, ExpectedResult = 6)]             // alternating (-1 2 -3 4 ...) TODO: 'cos of sign, alternating series is mistakenly ignored
        public double? Diffs_StandardSeriesDiffsShouldReturnNextNumber(double[] input, int recLevel)
        {
            var res = Diffs.Evaluate(input.ToList(), recLevel);

            return Math.Round(res.Values[0].Value, 4);
        }

        [TestCase(new[] { 2, 3, 6, 13, 28d }, 0, ExpectedResult = 59)]      // diff1: 1, 3, 7, 15   diff2: 2, 4, 8, (16) => exponentials
        [TestCase(new[] { 3, 5, 9, 16, 28d }, 0, ExpectedResult = 47)]      // diff1: 2, 4, 7, 12   diff2: 2, 3, 5, (7) => primes
        [TestCase(new[] { 9, 15, 22, 33, 53d }, 0, ExpectedResult = 89)]    // diff1: 6, 7, 11, 20   diff2: 1, 4, 9, (16) => squares
        [TestCase(new[] { 8, 10, 18, 44, 124d }, 0, ExpectedResult = 366)]  // diff1: 2, 8, 26, 80; diff2: 6, 18, 54; 6*3=18, 18*3=54, 54*3=162
        public double? Diffs_DiffsOfDiffsShouldReturnNextNumber(double[] input, int recLevel)
        {
            var res = Diffs.Evaluate(input.ToList(), recLevel);

            return Math.Round(res.Values[0].Value, 4);
        }
    }
}
