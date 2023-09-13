using Microsoft.VisualBasic;
using System.Reflection;

namespace ConsoleApp3
{



    internal class Program
    {
        static void Main(string[] args)
        {



            int[][] ArrayToFind =
            {
                new int[] { 1, 2, 3, 4},
                new int[] { 1, 2, 3, 4},
                new int[] { 1, 2, 3, 4},
                new int[] { 1, 2, 3, 4}
            };


            int[][] InitArray =
            {
                new int[] { 1, 2, 3, 4},
                new int[] { 1, 2, 3, 4},
                new int[] { 1, 2, 3, 4},
                new int[] { 2, 1, 3, 4}
            };


            var TargetField = new Field(ArrayToFind);
            var InitField = new Field(InitArray);
            var InitState = new State(InitField);

            State.Find(TargetField, new List<State> { InitState });






            Console.ReadLine();
        }
    }
}