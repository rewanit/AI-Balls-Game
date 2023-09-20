using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Text;

namespace ConsoleApp3
{
    public class State
    {
        
        public enum MoveAction
        {
            Row,
            Column
        }
       
        public State(Field parentField, State parentState = null, MoveAction? action = null, int? actionId = null)
        {
            Field = parentField;
            ParentState = parentState;
            Action = action;
            ActionId = actionId;
        }

        public override int GetHashCode()
        {
            return Field.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public State? ParentState { get; set; }
        public MoveAction? Action { get; set; }
        public int? ActionId { get; set; }
        public Field Field { get; set; }


        public List<State> ChildStates { get; set; } = new List<State>();


        private HashSet<State> GenStates(HashSet<State> existedStates)
        {
            var GenStates = new HashSet<State>();
            for (int i = 0; i < Field.Array.GetLength(0); i++)
            {
                var newField = Field.MoveCol(i);
                var newState = new State(newField, this, MoveAction.Column, i);
                if (!existedStates.Contains(newState))
                {
                    GenStates.Add(newState);
                    ChildStates.Add(newState);
                }
            }
            for (int i = 0; i < Field.Array.GetLength(1); i++)
            {
                var newField = Field.MoveRow(i);
                var newState = new State(newField, this, MoveAction.Row, i);
                if (!existedStates.Contains(newState))
                {
                    GenStates.Add(newState);
                    ChildStates.Add(newState);
                }
            }
            return GenStates;

        }

        
        static  public void Find(State TargetState,State StartState)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var existedStates = new HashSet<State>();
            var pool = new HashSet<State>
            {
                StartState
            };
            State? result = null;
            while(pool.Count>0)
            {
                HashSet<State> newPool = new HashSet<State>();
                if (pool.Contains(TargetState))
                {
                    pool.TryGetValue(TargetState,out result);
                    break;
                }
                else
                {
                    existedStates.UnionWith(pool);
                    newPool = pool.AsParallel().Select( (x) =>
                    {
                        var genStates = x.GenStates(existedStates);
                        
                        return genStates;
                    }).SelectMany(x=>x).ToHashSet();
                }
                pool = newPool;
            }
            stopwatch.Stop();
            if (result is null)
            {
                Console.WriteLine("Решений нет");
            }
            else
            {
                Console.WriteLine("Последовательное решение");

                var listToPrint = new List<string>();

                while (result?.ParentState != null)
                {
                    listToPrint.Add(result.Print());
                    result = result.ParentState;
                }

                listToPrint.Reverse();
                listToPrint.Insert(0, StartState.Print());
                var step = 0;
                foreach (var item in listToPrint)
                {
                    Console.WriteLine("Step: "+step++);
                    Console.WriteLine(item);
                }
                Console.WriteLine("Открытых на последнем этапе:" + pool.Count);
                Console.WriteLine("Закрытых:" + existedStates.Count);
                Console.WriteLine("Время: " + new TimeSpan(stopwatch.ElapsedTicks).ToString(@"mm\:ss\.ffffff"));
            }

            

        }

        public string Print()
        {
            var array = this.Field.Array;
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (Action.HasValue)
                        if (Action.Value == MoveAction.Row && i == ActionId || Action.Value == MoveAction.Column && j == ActionId)
                        {
                            sb.Append("[");
                        }

                    sb.Append(array[i,j]);

                    if (Action.HasValue)
                        if (Action.Value == MoveAction.Row && i == ActionId || Action.Value == MoveAction.Column && j == ActionId)
                        {
                            sb.Append("]");
                        }
                   

                    if (j < columns - 1)
                    {
                        sb.Append("\t"); // Разделитель между элементами в строке
                    }
                }
               
                sb.AppendLine(); // Переход на следующую строку
            }
            sb.AppendLine("------------------------------------------------");
            return sb.ToString();
        }
    }
}