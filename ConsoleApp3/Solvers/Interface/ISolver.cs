using ConsoleApp3.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Solvers.Interface
{
    internal interface ISolver
    {
        bool Find(State startState, State TargetState);

        void Print();


    }
}
