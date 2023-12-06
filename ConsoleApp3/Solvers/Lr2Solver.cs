using ConsoleApp3.Mechanics;
using ConsoleApp3.Solvers.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Solvers
{
    internal class Lr2Solver : ISolver
    {
        public bool Find(State TargetState, State StartState)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var existedStates = new HashSet<State>();
            var existedEndStates = new HashSet<State>();
            var pool = new HashSet<State>()
            {
                StartState
            };
            var endPool = new HashSet<State>()
            {
                TargetState
            };
            State? result = null;
            State? endResult = null;
            while (pool.Count > 0 || endPool.Count > 0)
            {
                HashSet<State> newPool = new HashSet<State>();
                HashSet<State> newEndPool = new HashSet<State>();
                var intersect = new HashSet<State>(existedStates);
                intersect.UnionWith(pool);
                var endIntersect = new HashSet<State>(existedEndStates);
                endIntersect.UnionWith(endPool);
                intersect.IntersectWith(endIntersect);
                if (intersect.Count() > 0)
                {
                    var stateHash = intersect.First();
                    pool.TryGetValue(stateHash, out result);
                    endPool.TryGetValue(stateHash, out endResult);
                    break;
                }




                    existedStates.UnionWith(pool);
                    newPool = pool.AsParallel().Select((x) =>
                    {
                        var genStates = x.GenStates(existedStates);

                        return genStates;
                    }).SelectMany(x => x).ToHashSet();



                    existedEndStates.UnionWith(endPool);

                    newEndPool = endPool.AsParallel().Select((x) =>
                    {
                        var genStates = x.GenReversedStates(existedEndStates);

                        return genStates;
                    }).SelectMany(x => x).ToHashSet();
                pool = newPool;
                endPool = newEndPool;

            }
            stopwatch.Stop();
            if (result is null)
            {
                Console.WriteLine("Решений нет");
            }
            else
            {


                var ValidateList = new List<ActionRecord>();
                Console.WriteLine("Последовательное решение");

                var listToPrint = new List<string>();

                while (result != null)
                {
                    listToPrint.Insert(0, result.Print());
                    result = result?.ParentState;
                }

                listToPrint.Add("+++ Стык +++");


                while (endResult != null)
                {
                    listToPrint.Add(endResult.Print());
                    endResult = endResult?.ParentState;
                }

                var step = 0;
                foreach (var item in listToPrint)
                {
                    Console.WriteLine("Step: " + step++);
                    Console.WriteLine(item);
                }

            }
            Console.WriteLine("Открытых на последнем этапе:" + (pool.Count + endPool.Count));
            Console.WriteLine("Закрытых:" + (existedStates.Count + existedEndStates.Count));
            Console.WriteLine("Время: " + new TimeSpan(stopwatch.ElapsedTicks).ToString(@"mm\:ss\.ffffff"));
        }
    }
}
