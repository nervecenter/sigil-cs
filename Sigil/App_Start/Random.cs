using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil
{
    /// <summary>
    /// Random Number Generator for the entire project ------ Code taken from http://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number/
    /// </summary>
    public class RNG
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        /// <summary>
        /// Gets you a random integer
        /// </summary>
        /// <param name="min">Inclusive minimum number</param>
        /// <param name="max">Exclusive maximum number</param>
        /// <returns></returns>
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    }
}