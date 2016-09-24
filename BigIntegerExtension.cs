//.Net 4.0 

using System;
using System.Numerics; //need to referece the System.Numerics assembly

namespace publicKeyAssignment6
{
    //found at http://stackoverflow.com/questions/3432412/calculate-square-root-of-a-biginteger-system-numerics-biginteger
    static class BigIntegerExtension
    {
        public static BigInteger iSqrt(this BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }
    }
}
