using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberSeries
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var input = Console.ReadLine();
                var list = Helpers.FormatInput(input);
                var r = NPAT.GetNextNumber(list);
                if (r != null)
                    r.Print();
            }/**/
        }

    }
}
