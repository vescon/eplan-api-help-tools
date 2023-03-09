using System;
using System.Collections.Generic;

namespace PlaceholderConfigLoader.Extensions
{
    /// <summary>Structure of to elements, which belong together</summary>
    public class Pair<TFirst, TSecond>
    {
        /// <summary>The first element in the pair.</summary>
        public TFirst First { get; }

        /// <summary>The second element in the pair.</summary>
        public TSecond Second { get; }

        /// <summary>
        /// Creates an instance of a pair, which contains to elements.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        /// <summary>Casts the elements in the pair to the given types.</summary>
        /// <returns>Returns a new pair which contains the casted elements.</returns>
        public Pair<TFirstBase, TSecondBase> Cast<TFirstBase, TSecondBase>() where TFirstBase : class where TSecondBase : class
        {
            TFirstBase first = (object) First as TFirstBase;
            if (EqualityComparer<TFirstBase>.Default.Equals(first, default (TFirstBase)))
                throw new InvalidCastException();

            TSecondBase second = (object) Second as TSecondBase;
            if (EqualityComparer<TSecondBase>.Default.Equals(second, default (TSecondBase)))
                throw new InvalidCastException();

            return new Pair<TFirstBase, TSecondBase>(first, second);
        }
    }
}