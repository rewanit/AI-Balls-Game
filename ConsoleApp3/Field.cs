using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp3
{
    public class Field
    {
        public Field(int[,] initField)
        {
            Array = Helper.Copy(initField);
        }

        private int? Hash ;


        private int ComputeHash()
        {
            var hash = new StringBuilder();
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    hash.Append(Array[i, j]);
                }
            }
            Hash = hash.ToString().GetHashCode();

            return Hash.Value;
        }
        public override int GetHashCode()
        {
            return Hash ?? ComputeHash();
        }


        public static bool CompareArrays(int[,] array1, int[,] array2)
        {
            
            // Сравниваем каждый элемент в массивах
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    if (array1[i,j] != array2[i,j])   
                    {
                        return false;
                    }
                }
            }

            // Если все элементы совпадают, возвращаем true
            return true;
        }



        public bool Equals(Field obj)
        {
            //Console.WriteLine(Print()); 
            //Console.ReadLine();
            var t = CompareArrays(this.Array, obj.Array);
           
            return t;
        }

        public int[,] Array { get; set; }

        public Field MoveRow(int columnId)
        {
            var newField = Helper.Copy(Array);
            int rowCount = newField.GetLength(0);

            for (int i = rowCount - 1; i > 0; i--)
            {
                (newField[columnId,i], newField[columnId,i - 1]) =
                (newField[columnId,i - 1], newField[columnId,i]);
            }

            return new Field(newField);
        }

        public Field MoveCol(int rowId)
        {
            var newField = Helper.Copy(Array);
            int columnCount = newField.GetLength(0);

            for (int i = columnCount - 1; i > 0; i--)
            {
                (newField[i,rowId], newField[i - 1,rowId]) =
                (newField[i - 1,rowId], newField[i,rowId]);
            }

            return new Field(newField);
        }




    }
}