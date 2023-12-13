using System.Collections;
using System.Formats.Asn1;
using System.Globalization;
using System.Threading;
using ConsoleTables;
using CsvHelper;
using CsvHelper.Configuration;

namespace ConsoleApp3
{
    internal class Program
    {
        private static int[,] GenRandom(int seed, int size = 4)
        {
            var rand = new Random(seed);
            int[,] array = new int[size, size];
            var pool = Enumerable.Range(1, size).SelectMany(x => Enumerable.Repeat(x, size)).OrderBy(x => rand.Next()).GetEnumerator();
            for (int i = 0; i < size; i++)
            {
                for (int l = 0; l < size; l++)
                {
                    pool.MoveNext();
                    array[i, l] = pool.Current;
                }
            }

            return array;
        }


        private static int[,] GenSimpleRandom(int seed, int size = 4)
        {
            var rand = new Random(seed);
            int[,] array = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int l = 0; l < size; l++)
                {
                    array[i, l] = 0;
                }
            }
            array[0, 0] = 1;

            return array;
        }

        private static int[,] GenSolvable(int[,] initArray, int seed, int steps)
        {
            var rand = new Random(seed);

            var field = new Field(initArray);
            for (int i = 0; i < steps; i++)
            {
                var RowCol = rand.Next(1, 3);
                var id = rand.Next(0, initArray.GetLength(0));

                var newField = (RowCol, id) switch
                {
                    (1, _) => field.MoveRow(id),
                    (2, _) => field.MoveCol(id),
                    _ => throw new Exception()
                };
                field = newField;
            }

            return field.Array;
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("1- обычный\n2- в файл");
            if (Console.ReadLine().Trim() == "1")
            {
                while (true)
                {

                


                int[,] InitArray =
                {
                    { 1, 2, 3, 4 },
                    { 1, 2, 3, 4 },
                    { 1, 2, 3, 4 },
                    { 1, 2, 3, 4 }
                };
                int[,] ArrayToFind =
                {
                    { 1, 2, 3, 4 },
                    { 1, 2, 3, 4 },
                    { 1, 2, 3, 4 },
                    { 1, 2, 3, 4 }
                };



                if (true)
                {
                    var size = 4;
                    var seed = Random.Shared.Next();
                    InitArray = GenRandom(seed, size);

                    ArrayToFind = GenSolvable(InitArray, seed, 50);



                    //ArrayToFind = GenRandom(seed+1, size);
                }

                //var findOk = ArrayToFind.SelectMany(x => x.Select(l => l)).GroupBy(x => x).All(x => x.Count() == 4);
                //var initOk = InitArray.SelectMany(x => x.Select(l => l)).GroupBy(x => x).All(x => x.Count() == 4);

                //if ( !(findOk && initOk))
                //{
                //    Console.WriteLine("не вверный ввод");
                //    return;
                //}

                var TargetField = new Field(ArrayToFind);
                var TargetState = new State(TargetField);
                var InitField = new Field(InitArray);
                var InitState = new State(InitField);

                Console.WriteLine("Задано:");
                Console.WriteLine(InitState.Print());
                Console.WriteLine("Найти:");
                Console.WriteLine(TargetState.Print());

                RunAndPrintStats(TargetState, InitState);
                    Console.ReadLine();
                }
            }
            else
            {
                RunMultipleTests(100);
            }
            Console.ReadLine();

        }

        private static async Task<SolutionInfo> RunSearchAsync(Func<State, State, SolutionInfo> searchMethod, State targetState, State initState, TimeSpan timeout)
        {
            var searchTask = Task.Run(() => searchMethod(targetState, initState));
            var timeoutTask = Task.Delay(timeout);

            // Wait for either the search task to complete or the timeout task to complete
            var completedTask = await Task.WhenAny(searchTask, timeoutTask);

            // If the search task completed, return its result; otherwise, return a timeout result
            return completedTask == searchTask ? searchTask.Result : CreateTimeoutResult(timeout);
        }

        private static SolutionInfo CreateTimeoutResult(TimeSpan timeSpan)
        {
            return new SolutionInfo
            {
                IsSolutionFound = false,
                TimeElapsed = timeSpan // Set timeout duration
            };
        }


        public static async Task RunAndPrintStats(State targetState, State initState)
        {

            TimeSpan timeout = TimeSpan.FromMinutes(5);
            var table = new ConsoleTable("Method", "Opened Nodes", "Closed Nodes", "Max Opened Nodes", "StepsCount", "Iterations", "Time Elapsed");
            // Lr1
           // var lr1Task = RunSearchAsync(State.Find, targetState, initState, timeout);

            // Lr2
            var lr2Task = RunSearchAsync(State.FindLr2, targetState, initState, timeout);

            // Lr3_1
            var lr3_1Task = RunSearchAsync(State.FindLr3_1, targetState, initState, timeout);

            // Lr3_2
            var lr3_2Task = RunSearchAsync(State.FindLr3_2, targetState, initState, timeout);

            // Wait for all tasks to complete
            await Task.WhenAll( lr2Task, lr3_1Task, lr3_2Task);

            // Print results
           // PrintStats("Lr1", lr1Task.Result,table);
            PrintStats("Lr2", lr2Task.Result, table);
            PrintStats("Lr3_1", lr3_1Task.Result, table);
            PrintStats("Lr3_2", lr3_2Task.Result, table);

            Console.WriteLine(table.ToString());


        }

        private static void PrintStats(string methodName, SolutionInfo solutionInfo,ConsoleTable table)
        {

            table.AddRow(methodName, solutionInfo.OpenedNodesCount, solutionInfo.ClosedNodesCount, solutionInfo.MaxOpenedNodesCount, solutionInfo.StepsCount, solutionInfo.Iterations, solutionInfo.TimeElapsed);

        }


        public static async void RunMultipleTests(int numberOfTests)
        {
            TimeSpan timeout = TimeSpan.FromMinutes(10);
            for (int i = 1; i <= numberOfTests; i++)
            {
                Console.WriteLine($"Running Test {i}");

                // Generate random initial and target states
                var InitArray = GenRandom(Random.Shared.Next(),4);
                var ArrayToFind = GenSolvable(InitArray, Random.Shared.Next(),Random.Shared.Next(1,20));


                var TargetField = new Field(ArrayToFind);
                var targetState = new State(TargetField);
                var InitField = new Field(InitArray);
                var initState = new State(InitField);

                // Run search methods
                var lr1Task = RunSearchAsync(State.Find, targetState, initState, timeout);

                // Lr2
                var lr2Task = RunSearchAsync(State.FindLr2, targetState, initState, timeout);

                // Lr3_1
                var lr3_1Task = RunSearchAsync(State.FindLr3_1, targetState, initState, timeout);

                // Lr3_2
                var lr3_2Task = RunSearchAsync(State.FindLr3_2, targetState, initState, timeout);

                // Wait for all tasks to complete
                await Task.WhenAll(lr1Task, lr2Task, lr3_1Task, lr3_2Task);

                // Save results to files
                SaveResultsToFile(initState, targetState, new SolutionInfo[] { lr1Task.Result, lr2Task.Result, lr3_1Task.Result, lr3_2Task.Result });


                Console.WriteLine($"Test {i} completed.\n");
            }
        }

        private static void SaveResultsToFile(State initState, State targetState, SolutionInfo[] solutionInfos)
        {
            string fileName = $"TestResults_{DateTime.Now:yyyyMMddHHmmssfff}.csv";

            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteField("Initial State:");
                csv.NextRecord();
                csv.WriteField(initState.Print());
                csv.NextRecord();

                csv.WriteField("Target State:");
                csv.NextRecord();
                csv.WriteField(targetState.Print());
                csv.NextRecord();

                csv.WriteField("Results Table:");
                csv.NextRecord();
                csv.WriteField("Method");
                csv.WriteField("Opened Nodes");
                csv.WriteField("Closed Nodes");
                csv.WriteField("Max Opened Nodes");
                csv.WriteField("StepsCount");
                csv.WriteField("Iterations");
                csv.WriteField("Time Elapsed");
                csv.NextRecord();

                foreach (var solutionInfo in solutionInfos)
                {
                    csv.WriteField(solutionInfo.MethodName);
                    csv.WriteField(solutionInfo.OpenedNodesCount);
                    csv.WriteField(solutionInfo.ClosedNodesCount);
                    csv.WriteField(solutionInfo.MaxOpenedNodesCount);
                    csv.WriteField(solutionInfo.StepsCount);
                    csv.WriteField(solutionInfo.Iterations);
                    csv.WriteField(solutionInfo.TimeElapsed);
                    csv.NextRecord();
                }
            }

            Console.WriteLine($"Results saved to file: {fileName}\n");
        }
    }
}