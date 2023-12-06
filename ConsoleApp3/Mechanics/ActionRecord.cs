using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp3.Mechanics.State;

namespace ConsoleApp3.Mechanics
{
    internal class ActionRecord
    {
        public MoveAction ColRow { get; set; }
        public int Id { get; set; }
    }
}
