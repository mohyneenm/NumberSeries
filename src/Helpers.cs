using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    class Helpers
    {
        /// <summary>
        /// Checks if search list is a subsequence in source list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static bool ListContains<T>(List<T> source, List<T> search)
        {
            if (search.Count > source.Count)
                return false;

            return Enumerable.Range(0, source.Count - search.Count + 1)
                .Select(a => source.Skip(a).Take(search.Count))
                .Any(a => a.SequenceEqual(search));
        }

        // Creates groups of two, three, four... based on the groupcount
        public static List<List<double>> GetGroups(List<double> input, int groupcount, bool canOverlap = false, int startIndex = 0)
        {
            if (!canOverlap)
            {
                // non-overlapping groups
                // {1, 2, 3, 4, 5, 6, 7} => {1,2} {3,4} {5,6} {7}
                return input.Skip(startIndex).Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / groupcount)
                    .Select(g => g.Select(x => x.Value).ToList())
                    .ToList();
            }
            else
            {
                // overlapping groups
                // {6, 18, 21, 8, 11, 33, 2, 6, 9} => (6, 18, 21), (21, 8, 11), (11, 33, 2), (2, 6, 9)
                var finalList = new List<List<double>>();
                var tmpList = new List<double>();
                var counter = 0;
                for (var i = startIndex; i < input.Count; i++)
                {
                    if (counter < groupcount)
                    {
                        tmpList.Add(input[i]);
                        counter++;
                    }
                    else
                    {
                        finalList.Add(tmpList);
                        tmpList = new List<double>();
                        i--;
                        tmpList.Add(input[i]);
                        counter = 1;
                    }
                }
                if (tmpList.Count > 0)
                    finalList.Add(tmpList);

                return finalList;
            }
        }

        // Breaks up a list into two lists.
        // {1, 6, 3, 4, 7, 5} => {1, 3, 7}, {6, 4, 5} 
        public static List<List<double>> TwoLists(List<double> input)
        {
            var list1 = input.Where((x, i) => i % 2 == 0).ToList();
            var list2 = input.Where((x, i) => i % 2 == 1).ToList();
            var lists = new List<List<double>>();
            lists.Add(list1);
            lists.Add(list2);

            return lists;
        }
        // Breaks up a list into three lists.
        // {1, 6, 3, 4, 7, 5, 2, 8} => {1, 4, 2}, {6, 7, 8}, {3, 5}
        public static List<List<double>> ThreeLists(List<double> input)
        {
            var list1 = input.Where((x, i) => i % 3 == 0).ToList();
            var list2 = input.Where((x, i) => i % 3 == 1).ToList();
            var list3 = input.Where((x, i) => i % 3 == 2).ToList();
            var lists = new List<List<double>>();
            lists.Add(list1);
            lists.Add(list2);
            lists.Add(list3);

            return lists;
        }

        public static List<double> FormatInput(string input)
        {
            var separator = ' ';
            var isFraction = false;
            if (input.Contains(','))
                separator = ',';
            if (input.Contains('/'))
                isFraction = true;

            var terms = input.Split(separator).ToList();
            terms.ForEach(t => t.Trim());

            List<double> result = new List<double>();
            if (isFraction)
            {
                foreach (var t in terms)
                {
                    var parts = t.Split('/');
                    var dividend = double.Parse(parts[0]);
                    var divisor = parts.Length == 2 ? double.Parse(parts[1]) : 1;
                    result.Add(dividend / divisor);
                }
            }
            else
            {
                foreach (var t in terms)
                    result.Add(double.Parse(t));
            }

            return result;
        }
        public static void PrintInput(List<double> input, string methodName, object ops, object res)
        {
            //return;
            Console.WriteLine($"Match: {string.Join(", ", input)}");

            if (res != null && ops != null)
            {
                string opsList = (ops is IList<string>) ? string.Join(",", (IList<string>)ops) : (string)ops;
                string resList = (res is IList<double>) ? string.Join(",", (IList<double>)res) : res.ToString();
                Console.WriteLine($"{methodName}, '{opsList}', {resList}");
            }
            else
            {
                Console.WriteLine($"{methodName}");
            }
        }
        public static void PrintMethod(string methodName, List<double> input, int recLevel)
        {
            string spaces = CalculateSpaces(recLevel);
            Console.WriteLine($"{spaces}>{methodName}: {string.Join(", ", input)}");
        }

        private static string CalculateSpaces(int recLevel)
        {
            string spaces = "";
            for (var i = 0; i < recLevel; i++)
                spaces += " ";

            return spaces;
        }
    }
}
