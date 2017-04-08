using NumberSeries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberSeriesTests
{
    class ProductsTests
    {
        [TestCase(new[] { 2, 6, 18, 54d }, 0, ExpectedResult = 162)]            // *3
        [TestCase(new[] { 3, 3.6, 4.32, 5.184d }, 0, ExpectedResult = 6.2208)]  // *1.2
        [TestCase(new[] { 100, 20, 4, 0.8d }, 0, ExpectedResult = 0.16)]        // *0.5
        public double? Products_ConstMultipleShouldReturnNextNumber(double[] input, int recLevel)
        {
            var res = Products.Evaluate(input.ToList(), recLevel);

            return Math.Round(res.Values[0].Value, 4);
        }

        [TestCase(new[] { 2, 4, 12, 60, 420d }, 0, ExpectedResult = 4620)]      // primes
        [TestCase(new[] { 2, -4, 12, -60, 420d }, 0, ExpectedResult = -4620)]   // primes*(-1)
        [TestCase(new[] { 2, 2, 8, 72, 1152d }, 0, ExpectedResult = 28800)]     // squares
        [TestCase(new[] { 1, 7, 42, 210, 840d }, 0, ExpectedResult = 2520)]     // natural desc
        public double? Products_MultiplesFormStandardSeriesShouldReturnNextNumber(double[] input, int recLevel)
        {
            var res = Products.Evaluate(input.ToList(), recLevel);

            return Math.Round(res.Values[0].Value, 4);
        }
    }
}
