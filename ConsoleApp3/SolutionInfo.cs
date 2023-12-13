using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class SolutionInfo
    {
        public List<string> Steps { get; } = new List<string>();
        public int OpenedNodesCount { get; set; }
        public int ClosedNodesCount { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public bool IsSolutionFound { get; set; }

        public void AddStep(string step)
        {
            Steps.Add(step);
        }

        public void PrintSteps()
        {
            Console.WriteLine("Последовательное решение");

            foreach (var step in Steps)
            {
                Console.WriteLine(step);
            }
        }

        public void PrintStatistic()
        {
            

            Console.WriteLine("Открытых на последнем этапе: " + OpenedNodesCount);
            Console.WriteLine("Закрытых: " + ClosedNodesCount);
            Console.WriteLine("Время: " + TimeElapsed.ToString(@"mm\:ss\.ffffff"));
        }
    }
}
