using Microsoft.VisualBasic;
using System.Reflection;

namespace ConsoleApp3
{



    internal class Program
    {
        static int[,] GenRandom(int seed,int size=4)
        {
            var rand  = new Random(seed);
            int[,] array = new int[size, size];
            var pool = Enumerable.Range(1,size).SelectMany(x=>Enumerable.Repeat(x,size)).OrderBy(x=>rand.Next()).GetEnumerator();
            for (int i = 0; i < size; i++)
            {
                for (int l = 0; l < size; l++)
                {
                    pool.MoveNext();
                    array[i,l] = pool.Current;
                }
            }

            return array;

        }

        static int[,] GenSolvable(int[,] initArray,int seed, int steps)
        {
            var rand = new Random(seed);

            var field = new Field(initArray);
            for (int i = 0; i < steps; i++)
            {

                var RowCol = rand.Next(1, 2);
                var id = rand.Next(0, initArray.GetLength(0)-1);

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



        static void Main(string[] args)
        {

            while (true)
            {


            int[,] ArrayToFind =
            {
                 { 1, 2, 3  ,4 },
                { 1, 2, 3   ,4 },
                { 1, 2, 3   ,4 },
                { 1, 2, 3   ,4}
            };

            int[,] InitArray =
            {

                { 1, 2, 3,4 },
                { 1, 2, 3,4 },
                { 1, 2, 3,4 },
                { 1, 2, 3,4 }
            };

            if (true)
            {
                var size = 4;
                var seed = Random.Shared.Next();
                InitArray = GenRandom(seed, size);

                ArrayToFind = GenSolvable(InitArray, seed,100000);

                //ArrayToFind = GenRandom(seed, size);


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

            State.Find(TargetState, InitState );

            Console.ReadLine();
            }
        }
    }
}