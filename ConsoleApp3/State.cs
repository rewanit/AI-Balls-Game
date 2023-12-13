﻿using System.Diagnostics;
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
            return this.Field.Md5 == ((State)obj).Field.Md5;
        }

        public State? ParentState { get; set; }
        public MoveAction? Action { get; set; }
        public int? ActionId { get; set; }
        public Field Field { get; set; }

        public int Depth { get; set; } = 0;



        public static int FindDistanceToClosestElement(int[,] array, int row, int col, int elem)
        {
            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);


            int targetElement = elem;

            int closestDistance = int.MaxValue;

            // Находим ближайший элемент, исключая сам элемент
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (array[i, j] == targetElement)
                    {
                        int distance = Math.Abs(row - i) + Math.Abs(col - j);
                        closestDistance = Math.Min(closestDistance, distance);
                    }
                }
            }

            return closestDistance == int.MaxValue ? -1 : closestDistance;
        }
        public static int FindDistanceToClosestElement2(int[,] array, int row, int col, int elem)
        {
            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);


            int targetElement = elem;

            int closestDistance = int.MaxValue;

            // Находим ближайший элемент, исключая сам элемент
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (array[i, j] == targetElement)
                    {
                        int distance = (int)Math.Sqrt(Math.Pow(row - i, 2) + Math.Pow(col - j, 2));
                        closestDistance = Math.Min(closestDistance, distance);
                    }
                }
            }

            return closestDistance == int.MaxValue ? -1 : closestDistance;
        }

        public int CalculateHeuristic(State target)
        {
            int heuristic = 0;


            var numRows = Field.Array.GetLength(0);
            var numCols = Field.Array.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int l = 0; l < numCols; l++)
                {
                    heuristic += FindDistanceToClosestElement(target.Field.Array, i, l, Field.Array[i, l]);
                }

            }

            return heuristic;
        }

        public int CalculateHeuristic2(State target)
        {
            int heuristic = 0;


            var numRows = Field.Array.GetLength(0);
            var numCols = Field.Array.GetLength(1);

            for (int i = 0; i < numRows; i++)
            {
                for (int l = 0; l < numCols; l++)
                {
                    heuristic += FindDistanceToClosestElement2(target.Field.Array, i, l, Field.Array[i, l]);
                }

            }

            return heuristic;
        }

        private int heuristic;


        public List<State> ChildStates { get; set; } = new List<State>();

        private HashSet<State> GenReversedStates(HashSet<State> existedStates)
        {
            var GenStates = new HashSet<State>();
            for (int i = 0; i < Field.Array.GetLength(0); i++)
            {
                var newField = Field.MoveColReverse(i);
                var newState = new State(newField, this, MoveAction.Column, i);
                newState.Depth = this.Depth + 1;

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
                    existedStates.EnsureCapacity(existedStates.Count * 8);
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
            var pool = new HashSet<State> { StartState };
            var endPool = new HashSet<State> { TargetState };
            State? result = null;
            State? endResult = null;

            while (pool.Count > 0 || endPool.Count > 0)
            {
                var newPool = new HashSet<State>();
                var newEndPool = new HashSet<State>();

                var intersect = pool.Intersect(endPool);
                if (intersect.Any())
                {
                    result = intersect.First();
                    endResult = result; // If there's an intersection, both pools should have the same result
                    break;
                }

                existedStates.UnionWith(pool);
                newPool = pool.SelectMany(x => x.GenStates(existedStates)).Where(s => !existedStates.Contains(s)).ToHashSet();

                existedEndStates.UnionWith(endPool);
                newEndPool = endPool.SelectMany(x => x.GenReversedStates(existedEndStates)).Where(s => !existedEndStates.Contains(s)).ToHashSet();

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
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var existedStates = new HashSet<State>();
            var pool = new PriorityQueue<State,int>(); // Замените на вашу реализацию PriorityQueue
            initState.heuristic = initState.CalculateHeuristic(targetState);
            pool.Enqueue(initState, 0); // Начальный узел с приоритетом 0
            State? result = null;

            while (pool.Count > 0)
            {
                var currentState = pool.Dequeue();
                if (currentState.Equals(targetState))
                {
                    result = currentState;
                    break;
                }

                existedStates.Add(currentState);

                var genStates = currentState.GenStates(existedStates);
                foreach (var newState in genStates)
                {
                    newState.heuristic = newState.Depth + newState.CalculateHeuristic(targetState);
                    var priority = newState.heuristic;
                    pool.Enqueue(newState, priority);
                }
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
                listToPrint.Insert(0, initState.Print());
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

        

        internal static void FindLr3_2(State targetState, State initState)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var existedStates = new HashSet<State>();
            var pool = new PriorityQueue<State, int>(); 
            initState.heuristic = initState.CalculateHeuristic2(targetState);
            pool.Enqueue(initState, 0); 
            State? result = null;

            while (pool.Count > 0)
            {
                var currentState = pool.Dequeue();
                if (currentState.Equals(targetState))
                {
                    result = currentState;
                    break;
                }

                existedStates.Add(currentState);

                var genStates = currentState.GenStates(existedStates);
                foreach (var newState in genStates)
                {
                    newState.heuristic = newState.Depth + newState.CalculateHeuristic2(targetState);
                    var priority = newState.heuristic;
                    pool.Enqueue(newState, priority);
                }
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
                listToPrint.Insert(0, initState.Print());
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
    }
}