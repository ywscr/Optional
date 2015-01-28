﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Optional
{
    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be wrapped.</typeparam>
    public struct Option<T>
    {
        private bool hasValue;
        private T value;

        /// <summary>
        /// Checks if a value is present.
        /// </summary>
        public bool HasValue { get { return hasValue; } }
        internal T Value { get { return value; } }

        internal Option(T value, bool hasValue)
        {
            this.value = value;
            this.hasValue = hasValue;
        }

        /// <summary>
        /// Determines whether two Option&lt;T&gt; instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the instances are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Option<T>)
            {
                var other = (Option<T>)obj;

                if (!hasValue && !other.hasValue)
                {
                    return true;
                }
                else if (hasValue && other.hasValue)
                {
                    if (value == null && other.value == null)
                    {
                        return true;
                    }
                    else if (value != null && other.value != null)
                    {
                        return value.Equals(other.value);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a hash code the current Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            if (hasValue)
            {
                if (value == null)
                {
                    return 1;
                }

                return value.GetHashCode();
            }

            return 0;
        }

        /// <summary>
        /// Returns a string that represents the current Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>A string that represents the current instance.</returns>
        public override string ToString()
        {
            if (hasValue)
            {
                if (value == null)
                {
                    return "Some(null)";
                }

                return string.Format("Some({0})", value);
            }

            return "None";
        }

        /// <summary>
        /// Returns the existing value if present, and otherwise an alternative value.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public T ValueOr(T alternative)
        {
            if (HasValue)
            {
                return Value;
            }

            return alternative;
        }

        /// <summary>
        /// Uses an alternative value, if no existing value is present.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new Option&lt;T&gt; instance, containing either the existing or alternative value.</returns>
        public Option<T> Or(T alternative)
        {
            if (HasValue)
            {
                return this;
            }

            return alternative.Some();
        }

        public Option<T, THint> WithHint<THint>(THint hint)
        {
            return Match(
                some: value => Option.Some<T, THint>(value),
                none: () => Option.None<T, THint>(hint)
            );
        }

        /// <summary>
        /// Evaluates a specified function, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the value is present.</param>
        /// <param name="none">The function to evaluate if the value is missing.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (HasValue)
            {
                return some(Value);
            }

            return none();
        }

        /// <summary>
        /// Evaluates a specified action, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        /// <param name="none">The action to evaluate if the value is missing.</param>
        public void Match(Action<T> some, Action none)
        {
            if (HasValue)
            {
                some(Value);
            }
            else
            {
                none();
            }
        }

        /// <summary>
        /// Transforms the inner value in an Option&lt;T&gt; instance.
        /// If the instance is empty, an empty instance is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed Option&lt;T&gt; instance.</returns>
        public Option<TResult> Map<TResult>(Func<T, TResult> mapping)
        {
            return Match(
                some: value => mapping(value).Some(),
                none: () => Option.None<TResult>()
            );
        }

        /// <summary>
        /// Transforms the inner value in an Option&lt;T&gt; instance
        /// into another Option&lt;T&gt; instance. The result is flattened, 
        /// and if either is empty, an empty instance is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed Option&lt;T&gt; instance.</returns>
        public Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> mapping)
        {
            return Match(
                some: value => mapping(value),
                none: () => Option.None<TResult>()
            );
        }

        /// <summary>
        /// Empties an Option&lt;T&gt; instance, if a specified predicate
        /// is not satisfied.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The filtered Option&lt;T&gt; instance.</returns>
        public Option<T> Filter(Func<T, bool> predicate)
        {
            var original = this;
            return Match(
                some: value => predicate(value) ? original : Option.None<T>(),
                none: () => original
            );
        }
    }

    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the value to be wrapped.</typeparam>
    public struct Option<T, THint>
    {
        private bool hasValue;
        private T value;
        private THint hint;

        /// <summary>
        /// Checks if a value is present.
        /// </summary>
        public bool HasValue { get { return hasValue; } }
        internal T Value { get { return value; } }
        internal THint Hint { get { return hint; } }

        internal Option(T value, THint hint, bool hasValue)
        {
            this.value = value;
            this.hasValue = hasValue;
            this.hint = hint;
        }

        /// <summary>
        /// Determines whether two Option&lt;T&gt; instances are equal.
        /// </summary>
        /// <param name="obj">The instance to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the instances are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Option<T, THint>)
            {
                var other = (Option<T, THint>)obj;

                if (!hasValue && !other.hasValue)
                {
                    return hint.Equals(other.hint);
                }
                else if (hasValue && other.hasValue)
                {
                    if (value == null && other.value == null)
                    {
                        return true;
                    }
                    else if (value != null && other.value != null)
                    {
                        return value.Equals(other.value);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a hash code the current Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            if (hasValue)
            {
                if (value == null)
                {
                    return 1;
                }

                return value.GetHashCode();
            }

            if (hint == null)
            {
                return 0;
            }

            return hint.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>A string that represents the current instance.</returns>
        public override string ToString()
        {
            if (hasValue)
            {
                if (value == null)
                {
                    return "Some(null)";
                }

                return string.Format("Some({0})", value);
            }

            if (hint == null)
            {
                return "None(null)";
            }

            return string.Format("None({0})", hint);
        }

        /// <summary>
        /// Returns the existing value if present, and otherwise an alternative value.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public T ValueOr(T alternative)
        {
            if (HasValue)
            {
                return Value;
            }

            return alternative;
        }

        /// <summary>
        /// Uses an alternative value, if no existing value is present.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new Option&lt;T&gt; instance, containing either the existing or alternative value.</returns>
        public Option<T, THint> Or(T alternative)
        {
            if (HasValue)
            {
                return this;
            }

            return alternative.Some<T, THint>();
        }

        public Option<T> WithoutHint()
        {
            return Match(
                some: value => value.Some(),
                none: _ => Option.None<T>()
            );
        }

        /// <summary>
        /// Evaluates a specified function, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The function to evaluate if the value is present.</param>
        /// <param name="none">The function to evaluate if the value is missing.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<T, TResult> some, Func<THint, TResult> none)
        {
            if (HasValue)
            {
                return some(Value);
            }

            return none(Hint);
        }

        /// <summary>
        /// Evaluates a specified action, based on whether a value is present or not.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        /// <param name="none">The action to evaluate if the value is missing.</param>
        public void Match(Action<T> some, Action<THint> none)
        {
            if (HasValue)
            {
                some(Value);
            }
            else
            {
                none(Hint);
            }
        }

        /// <summary>
        /// Transforms the inner value in an Option&lt;T&gt; instance.
        /// If the instance is empty, an empty instance is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed Option&lt;T&gt; instance.</returns>
        public Option<TResult, THint> Map<TResult>(Func<T, TResult> mapping)
        {
            return Match(
                some: value => Option.Some<TResult, THint>(mapping(value)),
                none: hint => Option.None<TResult, THint>(hint)
            );
        }

        public Option<T, THintResult> MapHint<THintResult>(Func<THint, THintResult> mapping)
        {
            return Match(
                some: value => Option.Some<T, THintResult>(value),
                none: hint => Option.None<T, THintResult>(mapping(hint))
            );
        }

        /// <summary>
        /// Transforms the inner value in an Option&lt;T&gt; instance
        /// into another Option&lt;T&gt; instance. The result is flattened, 
        /// and if either is empty, an empty instance is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed Option&lt;T&gt; instance.</returns>
        public Option<TResult, THint> FlatMap<TResult>(Func<T, Option<TResult, THint>> mapping)
        {
            return Match(
                some: value => mapping(value),
                none: hint => Option.None<TResult, THint>(hint)
            );
        }

        /// <summary>
        /// Empties an Option&lt;T&gt; instance, if a specified predicate
        /// is not satisfied.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The filtered Option&lt;T&gt; instance.</returns>
        public Option<T, THint> Filter(Func<T, bool> predicate, THint hint)
        {
            var original = this;
            return Match(
                some: value => predicate(value) ? original : Option.None<T, THint>(hint),
                none: _ => original
            );
        }
    }

    /// <summary>
    /// Provides a set of functions for creating optional values.
    /// </summary>
    public static class Option
    {
        /// <summary>
        /// Wraps an existing value in an Option&lt;T&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An Option&lt;T&gt; instance containing the specified value.</returns>
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value, true);
        }

        /// <summary>
        /// Creates an empty Option&lt;T&gt; instance.
        /// </summary>
        /// <returns>An empty instance of Option&lt;T&gt;.</returns>
        public static Option<T> None<T>()
        {
            return new Option<T>(default(T), false);
        }

        public static Option<T, THint> Some<T, THint>(T value)
        {
            return new Option<T, THint>(value, default(THint), true);
        }

        public static Option<T, THint> None<T, THint>(THint hint)
        {
            return new Option<T, THint>(default(T), hint, false);
        }
    }
}
