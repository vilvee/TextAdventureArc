using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Provides a random number generator.
    /// </summary>
    public class RandomNumberGenerator
    {
        private static Random _generator = new();

        /// <summary>
        /// Generates a random number between the specified minimum and maximum values (inclusive).
        /// </summary>
        /// <param name="minimumValue">The minimum value of the range.</param>
        /// <param name="maximumValue">The maximum value of the range.</param>
        /// <returns>A random number between the specified minimum and maximum values.</returns>
        public static int NumberBetween(int minimumValue, int maximumValue)
        {
            return _generator.Next(minimumValue, maximumValue + 1);
        }
    }
}