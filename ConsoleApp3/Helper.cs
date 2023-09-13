namespace ConsoleApp3
{
    public static class Helper
    {

        public static int[][] Copy(int[][] array)
        {
            return array.Select(a => a.ToArray()).ToArray();
        } 
    }
}