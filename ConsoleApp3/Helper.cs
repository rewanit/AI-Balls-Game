namespace ConsoleApp3
{
    public static class Helper
    {
        public static int[,] Copy(int[,] array)
        {
            int[,] newArray = array.Clone() as int[,];
            return newArray;
        }
    }
}