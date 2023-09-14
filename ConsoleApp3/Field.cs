using System.Text;

namespace ConsoleApp3
{
    public class Field
    {
        public Field(int[][] initField)
        {
            Array = Helper.Copy(initField);
        }


        


        public static bool CompareArrays(int[][] array1, int[][] array2)
        {
            // Проверяем, если массивы имеют разную длину (разное количество строк)
            if (array1.Length != array2.Length)
            {
                return false;
            }

            // Проверяем, если каждая строка имеет разную длину (разное количество столбцов)
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i].Length != array2[i].Length)
                {
                    return false;
                }
            }

            // Сравниваем каждый элемент в массивах
            for (int i = 0; i < array1.Length; i++)
            {
                for (int j = 0; j < array1[i].Length; j++)
                {
                    if (array1[i][j] != array2[i][j])
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

        public int[][] Array { get; set; }

        public Field MoveRow(int columnId)
        {
            var newField = Helper.Copy(Array);
            int rowCount = newField[0].Length;

            for (int i = rowCount - 1; i > 0; i--)
            {
                (newField[columnId][i], newField[columnId][i - 1]) =
                (newField[columnId][i - 1], newField[columnId][i]);
            }

            return new Field(newField);
        }

        public Field MoveCol(int rowId)
        {
            var newField = Helper.Copy(Array);
            int columnCount = newField.Length;

            for (int i = columnCount - 1; i > 0; i--)
            {
                (newField[i][rowId], newField[i - 1][rowId]) =
                (newField[i - 1][rowId], newField[i][rowId]);
            }

            return new Field(newField);
        }




    }
}