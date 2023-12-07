using System.Diagnostics;
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

        private HashSet<State> GenReversedStates(HashSet<State> existedStates)
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

        public static void Find(State TargetState, State StartState)
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
                    Console.WriteLine("Step: " + step++);
                    Console.WriteLine(item);
                }
              
            }
            Console.WriteLine("Открытых на последнем этапе:" + pool.Count);
            Console.WriteLine("Закрытых:" + existedStates.Count);
            Console.WriteLine("Время: " + new TimeSpan(stopwatch.ElapsedTicks).ToString(@"mm\:ss\.ffffff"));
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
            while (pool.Count > 0 || endPool.Count >0)
            {
                HashSet<State> newPool = new HashSet<State>();
                HashSet<State> newEndPool = new HashSet<State>();
                var intersect = new HashSet<State>(existedStates);
                intersect.UnionWith(pool);
                var endIntersect = new HashSet<State>(existedEndStates);
                endIntersect.UnionWith(endPool);
                intersect.IntersectWith(endIntersect);
                if (intersect.Count()>0)
                {
                    var stateHash = intersect.First();
                    pool.TryGetValue(stateHash, out result);
                    endPool.TryGetValue(stateHash, out endResult);
                    break;
                }


                {
                   //existedStates.EnsureCapacity(existedStates.Count * 8);


                    existedStates.UnionWith(pool);
                    //pool.AsParallel().ForAll((x) =>
                    //{
                    //    existedStates.Add(x);
                    //});
                    newPool = pool.AsParallel().Select((x) =>
                    {
                        var genStates = x.GenStates(existedStates);

                        return genStates;
                    }).SelectMany(x => x).ToHashSet();



                    //existedEndStates.EnsureCapacity(existedStates.Count * 8);
                    existedEndStates.UnionWith(endPool);

                    //endPool.AsParallel().ForAll((x) =>
                    //{
                    //    existedEndStates.Add(x);
                    //});
                    newEndPool = endPool.AsParallel().Select((x) =>
                    {
                        var genStates = x.GenReversedStates(existedEndStates);

                        return genStates;
                    }).SelectMany(x => x).ToHashSet();
                }
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
            Console.WriteLine("Открытых на последнем этапе:" + (pool.Count+endPool.Count));
            Console.WriteLine("Закрытых:" + (existedStates.Count+existedEndStates.Count));
            Console.WriteLine("Время: " + new TimeSpan(stopwatch.ElapsedTicks).ToString(@"mm\:ss\.ffffff"));
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

        internal static void FindLr3_1(State targetState, State initState)
        {
            throw new NotImplementedException();
        }

        private static bool ContainsState(PriorityQueue<State,int> queue, State state)
        {
            foreach (var item in queue.)
            {
                if (item.Equals(state))
                {
                    return true;
                }
            }
            return false;
        }

        public static void AStarSearch(State TargetState, State StartState)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var openSet = new PriorityQueue<State,int>(); // Очередь с приоритетом
            var closedSet = new HashSet<State>();
            var cameFrom = new Dictionary<State, State>();
            var gScore = new Dictionary<State, int>();
            var fScore = new Dictionary<State, int>();

            gScore[StartState] = 0;
            fScore[StartState] = CalculateHeuristic(StartState, TargetState); // Используйте вашу эвристическую функцию

            openSet.Enqueue(StartState, fScore[StartState]);

            State result = null;

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current == TargetState)
                {
                    result = current;
                    break;
                }

                closedSet.Add(current);

                foreach (var neighbor in current.GenStates(closedSet))
                {
                    var tentativeGScore = gScore[current] + 1; // Расстояние между узлами всегда 1 (в вашем случае может отличаться)

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + CalculateHeuristic(neighbor, TargetState);



                        if (!ContainsState(openSet, neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }
            }



            stopwatch.Stop();

            if (result is null)
            {
                Console.WriteLine("Решений нет");
            }
            else
            {
                Console.WriteLine("A* решение");

                var path = ReconstructPath(result, cameFrom, StartState);
                for (int i = 0; i < path.Count; i++)
                {
                    Console.WriteLine($"Step: {i}");
                    Console.WriteLine(path[i].Print());
                }
            }

            Console.WriteLine("Открытых на последнем этапе:" + openSet.Count);
            Console.WriteLine("Закрытых:" + closedSet.Count);
            Console.WriteLine("Время: " + new TimeSpan(stopwatch.ElapsedTicks).ToString(@"mm\:ss\.ffffff"));
        }

        private static List<State> ReconstructPath(State current, Dictionary<State, State> cameFrom, State start)
        {
            var path = new List<State> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }

            return path;
        }

        private static int CalculateHeuristic(State current, State target)
        {
            // Реализуйте свою эвристическую функцию оценки состояния
            // Это может включать в себя расстояние Манхэттен, евклидово расстояние и т.д.
            // Пример: return Math.Abs(current.X - target.X) + Math.Abs(current.Y - target.Y);
            return 0;
        }

        internal static void FindLr3_2(State targetState, State initState)
        {
            throw new NotImplementedException();
        }
    }
}