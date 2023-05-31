using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Provides functionality to generate sequential numbers.
    /// </summary>
    public static class NumberAssign
    {
        private static int _nextNumber = 0;

        /// <summary>
        /// Generates the next number in the sequence.
        /// </summary>
        /// <returns>The generated number.</returns>
        public static int GenNextNum()
        {
            _nextNumber = (_nextNumber + 1);
            return _nextNumber;
        }
    }
}