using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class MySolution
    {
        // Tap equilibrium
        public int solution(int[] A)
        {
            int rightSum = 0;
            int leftSum = 0;
            int diff = int.MaxValue;

            for (int i = 0; i < A.Length; i++)
                rightSum += A[i];

            for (int i = 1; i < A.Length; i++)
            {
                int current = A[i - 1];
                leftSum += current;
                rightSum -= current;
                diff = Math.Min(diff, Math.Abs(leftSum - rightSum));
            }

            return diff;
        }
    }
}
