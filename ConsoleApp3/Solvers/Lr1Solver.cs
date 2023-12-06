using ConsoleApp3.Mechanics;
using ConsoleApp3.Solvers.Base;
using ConsoleApp3.Solvers.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Solvers
{
    internal class Lr1Solver : BaseSolver, ISolver
    {
        public bool Find(State TargetState, State StartState)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var existedStates = new HashSet<State>();
            var pool = new HashSet<State>
            {
                StartState
            };
            State? result = null;
            while (pool.Count > 0)
            {
                HashSet<State> newPool = new HashSet<State>();
                if (pool.Contains(TargetState))
                {
                    pool.TryGetValue(TargetState, out result);
                    break;
                }
                else
                {
                    //existedStates.EnsureCapacity(existedStates.Count * 8);
                    pool.AsParallel().ForAll((x) =>
                    {
                        existedStates.Add(x);
                    });
                    newPool = pool.AsParallel().Select((x) =>
                    {
                        var genStates = x.GenStates(existedStates);

                        return genStates;
                    }).SelectMany(x => x).ToHashSet();
                }
                pool = newPool;
            }
            stopwatch.Stop();
       
        }
    }
}
