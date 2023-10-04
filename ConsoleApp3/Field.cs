using System;
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

        private int? Hash;


        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes); // .NET 5 +

                // Convert the byte array to hexadecimal string prior to .NET 5
                // StringBuilder sb = new System.Text.StringBuilder();
                // for (int i = 0; i < hashBytes.Length; i++)
                // {
                //     sb.Append(hashBytes[i].ToString("X2"));
                // }
                // return sb.ToString();
            }
        }

        private int ComputeHash()
        {



            Hash = GetArrayString().GetHashCode();
            return Hash.Value;
        }

        public string GetArrayString()
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
        static int ComputeArrayHashCode(int[,] array)
        {
            int hash = 17; // Начальное значение хэш-кода

            // Проходим по всем элементам массива и добавляем их в хэш-код
            foreach (int element in array)
            {
                hash = hash * 31 + element.GetHashCode();
            }

            return hash;
        }

        static int ComputeArrayHashCodeWithOrder(int[,] array)
        {
            int hash = 17; // Начальное значение хэш-кода

            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);

            // Проходим по всем элементам массива с учетом порядка
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    hash = hash * 31 + array[row, col].GetHashCode();
                }
            }

            return hash;
        }

        public string GetMD5()
        {
            return CreateMD5(GetArrayString());
        }
        public string Md5 = "";
        public override int GetHashCode()
        {
            return Hash ?? ComputeHash();
        }

       

      

        public int[,] Array { get; set; }

        public Field MoveRow(int columnId)
        {

            var newField = Helper.Copy(Array);
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
            var newField = Helper.Copy(Array);
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

            var newField = Helper.Copy(Array);
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
            var newField = Helper.Copy(Array);
            int columnCount = newField.GetLength(0);

            for (int i = 0; i < columnCount - 1; i++)
            {
                (newField[i, ColId], newField[i + 1, ColId]) =
                (newField[i + 1, ColId], newField[i, ColId]);
            }

            return new Field(newField);
        }

    }
}