using System;
using System.Collections.Generic;
using System.Linq;

namespace Ardalis.GuardClauses
{
    public static class GuardClauseExtensions
    {
        public static void NullOrEmptyCollection<T>(this IGuardClause guardClause, IEnumerable<T> input, string parameterName)
        {
            if (input is null)
                throw new ArgumentNullException(parameterName, "Should not have been null!");

            if (!input.Any())
                throw new ArgumentException("Should not be empty", parameterName);
        }
    }
}