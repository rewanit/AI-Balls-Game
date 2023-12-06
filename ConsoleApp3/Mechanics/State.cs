using System.Diagnostics;
using System.Text;

namespace ConsoleApp3.Mechanics
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
            return GetHashCode() == obj.GetHashCode();
        }

        public State? ParentState { get; set; }
        public MoveAction? Action { get; set; }
        public int? ActionId { get; set; }
        public Field Field { get; set; }

        public List<State> ChildStates { get; set; } = new List<State>();

        public List<State> GetPath()
        {
            var state = this;
            var list = new List<State>();

            while (state != null)
            {
                list.Add(state);
                state = state?.ParentState;
            }

            return list;

        }

        public HashSet<State> GenReversedStates(HashSet<State> existedStates)
        {
            var GenStates = new HashSet<State>();
            for (int i = 0; i < Field.Array.GetLength(0); i++)
            {
                var newField = Field.MoveColReverse(i);
                var newState = new State(newField, this, MoveAction.Column, i);

                if (!existedStates.Contains(newState))
                {
                    GenStates.Add(newState);
                    ChildStates.Add(newState);
                }
            }
            for (int i = 0; i < Field.Array.GetLength(1); i++)
            {
                var newField = Field.MoveRowReverse(i);
                var newState = new State(newField, this, MoveAction.Row, i);
                if (!existedStates.Contains(newState))
                {
                    GenStates.Add(newState);
                    ChildStates.Add(newState);
                }
            }
            return GenStates;
        }

        public HashSet<State> GenStates(HashSet<State> existedStates)
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

        


        public static void FindLr2(State TargetState, State StartState)
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

        public string Print()
        {
            var array = Field.Array;
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

                    sb.Append(array[i, j]);

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