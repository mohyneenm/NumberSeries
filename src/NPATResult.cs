using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    public class NPATResult
    {
        public List<double> InputSeries { get; set; } = new List<double>();
        public List<double?> Values { get; set; } = new List<double?>();
        public Series SeriesName { get; set; } = Series.None;
        public List<double> StandardSeries { get; set; } = new List<double>();
        public List<double> Series1 { get; set; } = new List<double>();
        public List<double> Series2 { get; set; } = new List<double>();
        public List<double> Series3 { get; set; } = new List<double>();
        public List<Series> SeriesNames { get; set; } = new List<Series>();
        public List<double> DiffSeries { get; set; } = new List<double>();
        public bool IsSubset { get; set; }
        public string Comment { get; set; }
        public string MatchType { get; set; }
        public bool IsCompleteSeries { get; set; }
        public PatternComplexityOrder PatternComplexityOrder { get; set; }
        public int RecursiveDepth { get; set; }

        public void Print()
        {
            var ori = string.Join(", ", InputSeries);
            var results = string.Join(", ", Values);
            var diff = string.Join(", ", DiffSeries);
            var series1 = string.Join(", ", Series1);
            var series2 = string.Join(", ", Series2);
            var series3 = string.Join(", ", Series3);
            var seriesNames = string.Join(", ", SeriesNames);
            var standardSeries = string.Join(", ", StandardSeries);

            if (Values?.Count == 1 && Values[0] == null)
            {
                Console.WriteLine("Result could not be evaluated.");
            }
            else
            {
                if (DiffSeries.Count > 0)
                    Console.WriteLine("\n Results: " + "(diff: " + results + ")");
                else if (!string.IsNullOrWhiteSpace(Comment))
                    Console.WriteLine($"\n Results: {Comment}");
                else
                    Console.WriteLine($"\n Results: {results}");

                Console.WriteLine($"\n Original: {ori}");
                Console.WriteLine($" DiffSeries: {diff}");
                Console.WriteLine(" Series name: " + SeriesName.ToString());
                Console.WriteLine(" Series values: " + standardSeries);
                Console.WriteLine($" IsSubset: {IsSubset}");
                Console.WriteLine($"\n Series1: {series1}");
                Console.WriteLine($" Series2: {series2}");
                Console.WriteLine($" Series3: {series3}");
                Console.WriteLine($" Interleaved series names: {seriesNames}");
                Console.WriteLine($" Match type: {MatchType}");
                Console.WriteLine($" Total Matches: {NPAT.Matches}");
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("\n");
            }
        }
    }
}
