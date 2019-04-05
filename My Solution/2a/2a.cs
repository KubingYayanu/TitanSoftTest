namespace ConsoleApplication1
{
    public class _2a
    {
        public static void Sort(int[] input)
        {
            bool[] temp = new bool[5000];
            for (int i = 0; i < input.Length; i++)
            {
                temp[input[i]] = true;
            }

            int index = 0;
            for (int i = 0; i < 5000; i++)
            {
                if (temp[i])
                    input[index++] = i;
            }
        }
    }
}