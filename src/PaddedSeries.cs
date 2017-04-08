using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    class PaddedSeries
    {
        public static NPATResult Evaluate(List<double> input, int recLevel)
        {
            double? next = null;
            NPATResult result = new NPATResult();
            bool found = false;
            var signMultiplier = 1;

            if (input.Count == 0 || input.Count <= 2)
                return null;

            // keep track of the sign of the input series, 'cos we only deal with +ve standard series
            if (input.All(x => x <= 0))
                signMultiplier = -1;
            if (input.Any(x => x < 0) && signMultiplier == 1)
                return null;  // we don't deal with random -ve signs in a standard series {1, 2, 3, -4, 5, 6, -7}, we only deal with all +ve or all -ve.

            foreach (var kvp in StandardSeries.allSeries)
            {
                found = false;

                var series = kvp.Value;

                int j = 0;
                for (var v = 0; v <= 10; v++)
                {
                    // +v
                    List<double> newList = series.Select((x, i) => x + v).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: {v}; ";
                        found = true;
                        break;
                    }
                    // -v
                    newList = series.Select((x, i) => x - v).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: -{v}; ";
                        found = true;
                        break;
                    }

                    // +v, +(v+1), +(v+2), +(v+3), ...
                    newList = series.Select((x, i) => x + (v + i)).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: {v}+n; ";
                        found = true;
                        break;
                    }
                    // *v, *(v+1), *(v+2), *(v+3), ...
                    newList = series.Select((x, i) => x * (v + i)).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: *{v}+n; ";
                        found = true;
                        break;
                    }

                    // +v, -v                    
                    newList = series.Select((x, i) => i % 2 == 0 ? x + v : x - v).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: +{v},-{v}; ";
                        found = true;
                        break;
                    }
                    // -v, +v
                    newList = series.Select((x, i) => i % 2 != 0 ? x + v : x - v).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: -{v},+{v}; ";
                        found = true;
                        break;
                    }

                    // +v, *v
                    newList = series.Select((x, i) => i % 2 == 0 ? x + v : x * v).ToList();
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        result.MatchType += $"Padding: +{v},*{v}; ";
                        found = true;
                        break;
                    }

                    // +v, +(v+c)
                    // 11, 8, 17, 14, 23, 24, 31, 32    (ans: 41)
                    // 11, 13, 17, 19, 23, 29, 31, 37... primes
                    // prime+0, prime-5, prime+0, prime-5
                    for (var c = -10; c <= 10; c++)
                    {
                        newList = series.Select((x, i) => i % 2 == 0 ? x + v : x + (v + c)).ToList();
                        if (Helpers.ListContains(newList, input))
                        {
                            next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                            found = true;
                            result.MatchType += $"Padding: {v + c}; ";
                            break;
                        }
                    }
                    if (found)
                        break;

                    // *v, *(v+c)
                    // 2, 6, 5, 14, 11, 26, 17... (ans: 38)
                    // prime*1, prime*2, prime*1, prime*2
                    for (var c = -10; c <= 10; c++)
                    {
                        newList = series.Select((x, i) => i % 2 == 0 ? x * v : x * (v + c)).ToList();
                        if (Helpers.ListContains(newList, input))
                        {
                            next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                            found = true;
                            result.MatchType += $"Padding: *{v + c}; ";
                            break;
                        }
                    }
                    if (found)
                        break;

                    // +v, *v, +(v+1), *(v+1), +(v+2), *(v+2), ...
                    // 1,   4,      9,      16,     25,     36,   49,   64      squares
                    // 3,   8,      12,     48,     29                          series
                    // +2,  *2,     +3,     *3,     +4,     *4,   +5,   *5      padding
                    // 3,   1,      13,     11,     31,     29                  series
                    // +2,  -3,     +4,     -5,     +6,     -7                  TODO: padding, not working
                    newList = new List<double>();
                    for (var i = 0; i < series.Count(); i++)
                    {
                        if (i % 2 == 0)
                            newList.Add(series[i] + (v + j));
                        else
                            newList.Add(series[i] * (v + j++));
                    }
                    j = 0;
                    if (Helpers.ListContains(newList, input))
                    {
                        next = newList[newList.IndexOf(input.Last()) + 1] * signMultiplier;
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    result.MatchType += $"Padded {kvp.Key}";
                    break;
                }
            }

            if (next != null)
            {
                result.Values.Add(next);
                result.PatternComplexityOrder = PatternComplexityOrder.PaddedExponentials;
                result.RecursiveDepth = recLevel;
                return result;
            }
            return null;
        }
    }
}
