using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class RandomValue
    {
        // generate random number that doesn't contains two conservtive ones
        public static int generate()
        {
            Random rnd = new Random();
            int value;
            do
            {
                value = 1013 + rnd.Next(9898 - 1013);
            } while ((value & 1) == 1 ||
                    value % 10 == value % 100 / 10 ||
                    value % 100 / 10 == value % 1000 / 100 ||
                    value % 1000 / 100 == value % 10000 / 1000);
            return value;
        }


        // Get random fibonacci number between min and max
        public int GetRandom(int min, int max)
        {
            int a = 1;
            int b = 1;
            Random random = new Random();
            List<int> nums = new List<int>();

            while (b <= max)
            {
                if (b > min)
                {
                    nums.Add(b);
                }

                int temp = b;
                b += a;
                a = temp;
            }

            return nums[random.Next() % nums.Count];
        }


        // return a random odd number between min and maxc
        public int GetRandomOdd(int min, int max)
        {
            Random r = new Random();
            int n;
            while ((n = r.Next(min, max)) % 2 == 0) ;

            return n;
        }
    }
}
