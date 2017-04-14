using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    class CommonStandardSeries
    {
        public static List<double> Natural = Enumerable.Range(-100, 500).Select(x => x * 1.0).ToList();    // natural numbers
        public static List<double> NaturalDesc = Enumerable.Range(-100, 500).Select(x => x * 1.0).Reverse().ToList();    // natural numbers descending
        //public static List<double> Even = Enumerable.Range(0, 200).Select(x => x * 1.0).Where((x, i) => i % 2 == 0).ToList();  // even
        //public static List<double> Odd = Enumerable.Range(0, 200).Select(x => x * 1.0).Where((x, i) => i % 2 != 0).ToList();  // odd
        public static List<double> Alternating = Enumerable.Repeat(1, 100).Select((x, i) => x * Math.Pow(-1, i)).ToList(); // alternating: -1, 2, -3, 4, ...
        public static List<double> Exponentials = Enumerable.Range(1, 100).Where(x => (x & (x - 1)) == 0).Select(x => x * 1.0).ToList();   // exponentials
        public static List<double> Squares = Enumerable.Range(1, 100).Select(x => Math.Pow(x, 2) * 1.0).ToList(); // squares
        public static List<double> Cubes = Enumerable.Range(1, 100).Select(x => Math.Pow(x, 3) * 1.0).ToList();  // cubes
        public static List<double> Factorials = new List<double> { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800 };    // factorials
        public static List<double> Fibonacci = new List<double> { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765 };  // fibonacci
        public static List<double> Primes = Enumerable.Range(2, 1000).Where(number => Enumerable.Range(2, (int)Math.Sqrt(number) - 1).All(divisor => number % divisor != 0)).Select(i => (double)i).ToList();  // primes
        public static List<double> MaxCircleDivisionsByChord = new List<double>() { 1, 2, 4, 8, 16, 31, 57, 99, 163, 256, 386, 562, 794, 1093, 1471, 1941, 2517 };  // (n^4 - 6*n^3 + 23*n^2 - 18*n + 24)/24 => Maximum number of regions obtained by joining n points around a circle by straight lines. 
        // Triangular Series is obtained by Diffs of Diffs =>  0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55
        // Pentagonal Series => 1, 5, 12, 22, 35, 51, 70, 92, 117, 145, 176
        // Hexagonal Series => 1, 6, 15, 28, 45, 66, 91, 120, 153, 190, 231
        // Magic Square Series => 15, 34, 65, 111, 175, 260
        // Catalan Series => 1, 1, 2, 5, 14, 42, 132, 429, 1430, 4862
    }
    public enum Series
    {
        Natural,
        NaturalDesc,
        //Even,
        //Odd,
        Exponentials,
        Primes,
        Fibonacci,
        Factorials,
        Squares,
        Cubes,
        Alternating,
        MaxCircleDivisionsByChord,
        None
    }

    public class StandardSeries
    {
        public static Dictionary<Series, List<double>> allSeries = new Dictionary<Series, List<double>>()
                                                                    {{ Series.Natural, CommonStandardSeries.Natural },
                                                                    { Series.NaturalDesc, CommonStandardSeries.NaturalDesc },
                                                                    { Series.Exponentials, CommonStandardSeries.Exponentials },
                                                                    //{ Series.Even, CommonStandardSeries.Even },
                                                                    //{ Series.Odd, CommonStandardSeries.Odd },
                                                                    { Series.Squares, CommonStandardSeries.Squares },
                                                                    { Series.Cubes, CommonStandardSeries.Cubes },
                                                                    { Series.Primes, CommonStandardSeries.Primes },
                                                                    { Series.Fibonacci, CommonStandardSeries.Fibonacci },
                                                                    { Series.Factorials, CommonStandardSeries.Factorials },
                                                                    { Series.Alternating, CommonStandardSeries.Alternating },
                                                                    { Series.MaxCircleDivisionsByChord, CommonStandardSeries.MaxCircleDivisionsByChord }};

        public static NPATResult Evaluate(List<double> input, int recLevel, bool checkSubset = false)
        {
            NPATResult result = new NPATResult();
            if ((recLevel == 0 && input.Count <= 2) || input.Count == 0 || input.Count == 1)
                return null;

            // eg. 3,3,3... if all are the same then this is not a standard series nor a subset of one.
            if (input.Distinct().Count() == 1)
                return null;

            foreach (var kvp in allSeries)
            {
                var series = kvp.Value;
                var name = kvp.Key;
                var signMultiplier = 1;

                // keep track of the sign of the input series, 'cos we only deal with +ve standard series (TODO: this is not true, we have an alternating series too)
                if (input.All(x => x <= 0))
                    signMultiplier = -1;
                if (input.Any(x => x < 0) && signMultiplier == 1)
                    return null;  // we don't deal with random -ve signs in a standard series {1, 2, 3, -4, 5, 6, -7}, we only deal with all +ve or all -ve.

                // take the absolute values of the series and check against a standard series
                var tmpInput = input.Select(x => Math.Abs(x)).ToList();
                if (Helpers.ListContains(series, tmpInput))        // subsequence
                {
                    result.Values.Add(series[series.IndexOf(tmpInput.Last()) + 1] * signMultiplier);
                    result.SeriesName = name;
                    result.RecursiveDepth = recLevel;
                    result.PatternComplexityOrder = (name == Series.Natural) ? PatternComplexityOrder.NaturalNumbers : PatternComplexityOrder.StandardSeries;
                    return result;
                }
                else if (checkSubset && name != Series.Natural && name != Series.NaturalDesc
                        && !tmpInput.Except(series).Any())   // subset
                {
                    var newList = tmpInput.Select(i => (double?)i).ToList();
                    result.Values.AddRange(newList);
                    result.SeriesName = name;
                    result.StandardSeries = series.Take(20).ToList(); // it's a subset, so we can't predict what the next number will be
                    result.IsSubset = true;
                    result.RecursiveDepth = recLevel;
                    result.PatternComplexityOrder = (name == Series.Natural) ? PatternComplexityOrder.NaturalNumbers : PatternComplexityOrder.StandardSeries;
                    return result;
                }
            }

            return null;
        }

        private static double? SquaresGetNext(List<double> input)
        {
            List<double> series = Enumerable.Range(1, 200).Select(x => Math.Pow(x, 2)).ToList();   // { 1, 4, 9, 16, 25, 36, ... }

            List<double> squares1 = series.Where((x, i) => i % 2 != 0).ToList();    // 3, 7, 13, 19
            List<double> squares2 = series.Where((x, i) => i % 2 == 0).ToList();     // 2, 5, 11, 17
            List<double> squares3 = series.Where((x, i) => i % 3 == 0).ToList();    // 2, 7, 17, 29

            //bool isSubset = !input.Except(series).Any();            // checks if input is a subset of series
            if (Helpers.ListContains(series, input))
                series = series;
            else if (Helpers.ListContains(squares1, input))
                series = squares1;
            else if (Helpers.ListContains(squares2, input))
                series = squares2;
            else if (Helpers.ListContains(squares3, input))
                series = squares3;
            else
                series = null;

            if (series != null)
                return series[series.IndexOf(input.Last()) + 1];    // returns the next number in the input
            else
                return null;
        }
        private static double? CubesGetNext(List<double> input)
        {
            List<double> series = Enumerable.Range(1, 200).Select(x => Math.Pow(x, 3)).ToList();   // { 1, 4, 9, 16, 25, 36, ... }
            bool isSubset = !input.Except(series).Any();            // checks if input is a subset of series

            if (isSubset)
                return series[series.IndexOf(input.Last()) + 1];    // returns the next number in the input
            else
                return null;
        }
        private static double? FactorialsGetNext(List<double> input)
        {
            List<double> series = new List<double> { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800 };  // factorials
            bool isSubset = !input.Except(series).Any();            // checks if input is a subset of series

            if (isSubset)
                return series[series.IndexOf(input.Last()) + 1];    // returns the next number in the input
            else
                return null;
        }
        private static double? FibonacciGetNext(List<double> input)
        {
            List<double> series = new List<double> { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765 };  // fibonacci
            bool isSubset = !input.Except(series).Any();            // checks if input is a subset of series

            if (isSubset)
                return series[series.IndexOf(input.Last()) + 1];    // returns the next number in the input
            else
                return null;
        }
        private static double? ExponentialsGetNext(List<int> input)
        {
            List<int> series = Enumerable.Range(1, 1000000).Where(x => (x & (x - 1)) == 0).ToList();   // { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, ... }
            bool isSubset = !input.Except(series).Any();            // checks if input is a subset of series

            if (isSubset)
                return series[series.IndexOf(input.Last()) + 1];    // returns the next number in the input
            else
                return null;
        }
        private static double? PrimesGetNext(List<double> input)
        {
            List<double> series = Enumerable.Range(2, 1000)
                                            .Where(number => Enumerable.Range(2, (int)Math.Sqrt(number) - 1)
                                            .All(divisor => number % divisor != 0))
                                            .Select(i => (double)i).ToList();    // primes { 2, 3, 5, 7, 11, 13, 17, ... }

            List<double> primes1 = series.Where((x, i) => i % 2 != 0).ToList();    // 3, 7, 13, 19
            List<double> primes2 = series.Where((x, i) => i % 2 == 0).ToList();     // 2, 5, 11, 17
                                                                                    //List<double> primes3 = series.Where((x, i) => i % 3 == 0).ToList();    // 2, 7, 17, 29

            if (Helpers.ListContains(series, input))
                series = series;
            else if (Helpers.ListContains(primes1, input))
                series = primes1;
            else if (Helpers.ListContains(primes2, input))
                series = primes2;
            //else if (ListContains(primes3, input))
            //series = primes3;
            else
                series = null;

            int multiplier = input.Last() < 0 ? -1 : 1;
            //bool isSubset = !input.Select(x => Math.Abs(x)).Except(series).Any();            // checks if input is a subset of series
            if (series != null)
                return multiplier * series[series.IndexOf(System.Math.Abs(input.Last())) + 1];    // returns the next number in the input
            else
                return null;
        }
    }
}
