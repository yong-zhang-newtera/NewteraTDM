using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class Numbers
    {
        public long Power(int x, int y)
        {
            int res = 1;

            while (y > 0)
            {
                if ((y % 2) != 0)
                {
                    res = res * x;
                }

                x = x * x; 
                y = y >> 1; // y is even
            }

            return res;
        }

        public void Bin(int n)
        {
            if (n > 1)
                Bin(n / 2);
            Console.Write(n % 2);
        }
    }
}
