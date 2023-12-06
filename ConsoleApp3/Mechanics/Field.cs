using System;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp3.Mechanics
{
    public class Field
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

        public Field(int[,] initField)
        {
            Array = CloneArray(initField);
        }

        private int? Hash;

        private int ComputeHash()
        {
            Hash = GetArrayString().GetHashCode();
            return Hash.Value;
        }

        private string GetArrayString()
        {
            var hash = new StringBuilder();

            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    hash.Append(Array[i, j]);
                }
            }
            return hash.ToString();

        }
        
      
        public override int GetHashCode()
        {
            return Hash ?? ComputeHash();
        }





        public int[,] Array { get; private set; }

        public Field MoveRow(int columnId)
        {

            var newField = CloneArray(Array);
            int rowCount = newField.GetLength(0);

            for (int i = rowCount - 1; i > 0; i--)
            {
                (newField[columnId, i], newField[columnId, i - 1]) =
                (newField[columnId, i - 1], newField[columnId, i]);
            }

            return new Field(newField);
        }

        public Field MoveCol(int rowId)
        {
            var newField = CloneArray(Array);
            int columnCount = newField.GetLength(0);

            for (int i = columnCount - 1; i > 0; i--)
            {
                (newField[i, rowId], newField[i - 1, rowId]) =
                (newField[i - 1, rowId], newField[i, rowId]);
            }

            return new Field(newField);
        }

        public Field MoveRowReverse(int RowId)
        {

            var newField = CloneArray(Array);
            int rowCount = newField.GetLength(0);

            for (int i = 0; i < rowCount - 1; i++)
            {
                (newField[RowId, i], newField[RowId, i + 1]) =
                (newField[RowId, i + 1], newField[RowId, i]);
            }

            return new Field(newField);
        }

        public Field MoveColReverse(int ColId)
        {
            var newField = CloneArray(Array);
            int columnCount = newField.GetLength(0);

            for (int i = 0; i < columnCount - 1; i++)
            {
                (newField[i, ColId], newField[i + 1, ColId]) =
                (newField[i + 1, ColId], newField[i, ColId]);
            }

            return new Field(newField);
        }

        private int[,] CloneArray(int[,] array)
        {
            int[,] newArray = array.Clone() as int[,];
            return newArray;
        }
    }
}