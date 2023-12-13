using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class SolutionInfo
    {
        public string MethodName { get; set; }
        public List<string> Steps { get; } = new List<string>();
        public int OpenedNodesCount { get; set; }
        public int ClosedNodesCount { get; set; }
        public int MaxOpenedNodesCount { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public bool IsSolutionFound { get; set; }
        public int Iterations { get; set; }
        public int StepsCount => Steps.Count; 


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
            Console.WriteLine($"Совершенных шагов: {StepsCount}");
            Console.WriteLine("Время: " + TimeElapsed.ToString(@"mm\:ss\.ffffff"));
        }
    }
}
