using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace System
{
    public static class StringExtentions
    {
        public static bool ContainsIgnoreCase(this string str, StringComparison comp, string toCheck)
        {
            try
            {
                return str?.IndexOf(toCheck, comp) >= 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsIgnoreCase(this string str, string toCheck)
        {
            try
            {
                return str?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsIgnoreCase(this string str, StringComparison comp, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.ContainsIgnoreCase(comp, item))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsIgnoreCase(this string str, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.ContainsIgnoreCase(StringComparison.OrdinalIgnoreCase, item))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool Contains(this string str, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.Contains(item))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool EqualsIgnoreCase(this string str, string toCheck)
        {
            try
            {
                return str.Equals(toCheck, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBase64(this string base64String)
        {
            if (base64String.IsNullOrEmpty() || base64String.Length % 4 != 0
               || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool StartsWith(this string str, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.StartsWith(item))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool StartsWithIgnoreCase(this string str, string toCheck)
        {
            try
            {
                if (str.StartsWith(toCheck, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool StartsWithIgnoreCase(this string str, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.StartsWith(item, StringComparison.OrdinalIgnoreCase))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool EndsWith(this string str, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.EndsWith(item))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool EndsWithIgnoreCase(this string str, string toCheck)
        {
            try
            {
                if (str.EndsWith(toCheck, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool EndsWithIgnoreCase(this string str, params string[] toCheck)
        {
            try
            {
                foreach (var item in toCheck)
                    if (str.EndsWith(item, StringComparison.OrdinalIgnoreCase))
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether this string is null or an System.String.Empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// indicates whether this string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Guard.Against.Null(str, nameof(str));

            if (str.EndsWith(c.ToString(), comparisonType))
                return str;

            return str + c;
        }

        /// <summary>
        /// Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Guard.Against.Null(str, nameof(str));

            if (str.StartsWith(c.ToString(), comparisonType))
                return str;

            return c + str;
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
        public static string Left(this string str, int len)
        {
            Guard.Against.Null(str, nameof(str));

            if (str.Length < len)
                throw new ArgumentException("len argument can not be bigger than given string's length!");

            return str[..len];
        }

        /// <summary>
        /// Converts line endings in the string to <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string NormalizeLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Gets index of nth occurrence of a char in a string.
        /// </summary>
        /// <param name="str">source string to be searched</param>
        /// <param name="c">Char to search in <paramref name="str"/></param>
        /// <param name="n">Count of the occurrence</param>
        public static int NthIndexOf(this string str, char c, int n)
        {
            Guard.Against.Null(str, nameof(str));

            int count = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != c)
                    continue;

                if ((++count) == n)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Removes first occurrence of the given postfixes from end of the given string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="postFixes">one or more postfix.</param>
        /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            return str.RemovePostFix(StringComparison.Ordinal, postFixes);
        }

        /// <summary>
        /// Removes first occurrence of the given postfixes from end of the given string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="comparisonType">String comparison type</param>
        /// <param name="postFixes">one or more postfix.</param>
        /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
        public static string RemovePostFix(this string str, StringComparison comparisonType, params string[] postFixes)
        {
            if (str.IsNullOrEmpty())
                return str;

            if (postFixes.IsNullOrEmpty())
                return str;

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix, comparisonType))
                    return str.Left(str.Length - postFix.Length);
            }

            return str;
        }

        /// <summary>
        /// Removes first occurrence of the given prefixes from beginning of the given string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="preFixes">one or more prefix.</param>
        /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            return str.RemovePreFix(StringComparison.Ordinal, preFixes);
        }

        /// <summary>
        /// Removes first occurrence of the given prefixes from beginning of the given string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="comparisonType">String comparison type</param>
        /// <param name="preFixes">one or more prefix.</param>
        /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
        public static string RemovePreFix(this string str, StringComparison comparisonType, params string[] preFixes)
        {
            if (str.IsNullOrEmpty())
                return str;

            if (preFixes.IsNullOrEmpty())
                return str;

            foreach (var preFix in preFixes)
            {
                if (str.StartsWith(preFix, comparisonType))
                    return str.Right(str.Length - preFix.Length);
            }

            return str;
        }

        public static string ReplaceFirst(this string str, string search, string replace, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Guard.Against.Null(str, nameof(str));

            var pos = str.IndexOf(search, comparisonType);
            if (pos < 0)
            {
                return str;
            }

            return $"{str[0..pos]}{replace}{str[(pos + search.Length)..]}";
        }

        /// <summary>
        /// Gets a substring of a string from end of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
        public static string Right(this string str, int len)
        {
            Guard.Against.Null(str, nameof(str));
            Guard.Against.OutOfRange(len, nameof(len), 0, str.Length);
  
            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str.Split(new[] { separator }, options);
        }

        /// <summary>
        /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string[] SplitToLines(this string str)
        {
            return str.Split(Environment.NewLine);
        }

        /// <summary>
        /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return str.Split(Environment.NewLine, options);
        }

        /// <summary>
        /// Converts PascalCase string to camelCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
        /// <param name="handleAbbreviations">set true to if you want to convert 'XYZ' to 'xyz'.</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, bool useCurrentCulture = false, bool handleAbbreviations = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            if (str.Length == 1)
                return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();

            if (handleAbbreviations && IsAllUpperCase(str))
                return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();

            return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str[1..];
        }

        /// <summary>
        /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
        public static string ToSentenceCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return useCurrentCulture
                ? Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]))
                : Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
        }

        /// <summary>
        /// Converts given PascalCase/camelCase string to kebab-case.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
        public static string ToKebabCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            str = str.ToCamelCase();

            return useCurrentCulture
                ? Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLower(m.Value[1]))
                : Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLowerInvariant(m.Value[1]));
        }

        /// <summary>
        /// Converts given PascalCase/camelCase string to snake case.
        /// Example: "ThisIsSampleSentence" is converted to "this_is_a_sample_sentence".
        /// https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NameTranslation/NpgsqlSnakeCaseNameTranslator.cs#L51
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns></returns>
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var builder = new StringBuilder(str.Length + Math.Min(2, str.Length / 5));
            var previousCategory = default(UnicodeCategory?);

            for (var currentIndex = 0; currentIndex < str.Length; currentIndex++)
            {
                var currentChar = str[currentIndex];
                if (currentChar == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                var currentCategory = char.GetUnicodeCategory(currentChar);
                switch (currentCategory)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (previousCategory == UnicodeCategory.SpaceSeparator ||
                            previousCategory == UnicodeCategory.LowercaseLetter ||
                            previousCategory != UnicodeCategory.DecimalDigitNumber &&
                            previousCategory != null &&
                            currentIndex > 0 &&
                            currentIndex + 1 < str.Length &&
                            char.IsLower(str[currentIndex + 1]))
                        {
                            builder.Append('_');
                        }

                        currentChar = char.ToLower(currentChar);
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                        {
                            builder.Append('_');
                        }
                        break;

                    default:
                        if (previousCategory != null)
                        {
                            previousCategory = UnicodeCategory.SpaceSeparator;
                        }
                        continue;
                }

                builder.Append(currentChar);
                previousCategory = currentCategory;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            Guard.Against.Null(value, nameof(value));
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            Guard.Against.Null(value, nameof(value));
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToMd5(this string str)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(str);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            if (str.Length == 1)
                return useCurrentCulture ? str.ToUpper() : str.ToUpperInvariant();

            return (useCurrentCulture ? char.ToUpper(str[0]) : char.ToUpperInvariant(str[0])) + str[1..];
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
                return null;

            if (str.Length <= maxLength)
                return str;

            return str.Left(maxLength);
        }

        /// <summary>
        /// Gets a substring of a string from Ending of the string if it exceeds maximum length.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string TruncateFromBeginning(this string str, int maxLength)
        {
            if (str == null)
                return null;

            if (str.Length <= maxLength)
                return str;

            return str.Right(maxLength);
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// It adds a "..." postfix to end of the string if it's truncated.
        /// Returning string can not be longer than maxLength.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string TruncateWithPostfix(this string str, int maxLength)
        {
            return TruncateWithPostfix(str, maxLength, "...");
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// It adds given <paramref name="postfix"/> to end of the string if it's truncated.
        /// Returning string can not be longer than maxLength.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
        {
            if (str == null)
                return null;

            if (str == string.Empty || maxLength == 0)
                return string.Empty;

            if (str.Length <= maxLength)
                return str;

            if (maxLength <= postfix.Length)
                return postfix.Left(maxLength);

            return str.Left(maxLength - postfix.Length) + postfix;
        }

        /// <summary>
        /// Converts given string to a byte array using <see cref="Encoding.UTF8"/> encoding.
        /// </summary>
        public static byte[] GetBytes(this string str)
        {
            return str.GetBytes(Encoding.UTF8);
        }

        /// <summary>
        /// Converts given string to a byte array using the given <paramref name="encoding"/>
        /// </summary>
        public static byte[] GetBytes([NotNull] this string str, [NotNull] Encoding encoding)
        {
            Guard.Against.Null(str, nameof(str));
            Guard.Against.Null(encoding, nameof(encoding));

            return encoding.GetBytes(str);
        }

        private static bool IsAllUpperCase(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input[1..]
            };
    }
}