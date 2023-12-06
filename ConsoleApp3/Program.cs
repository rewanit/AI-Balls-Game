using ConsoleApp3.Mechanics;

namespace ConsoleApp3
{
    internal class Program
    {
        

        private static void Main(string[] args)
        {
            while (true)
            {
                GC.Collect();

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



                
                //Console.WriteLine("Lr1");
                //State.Find(TargetState, InitState);
                Console.WriteLine("Lr2");
                State.FindLr2(TargetState, InitState);

                Console.ReadLine();
            }
        }
    }
}