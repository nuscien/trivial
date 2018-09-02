// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleCondition.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The simple condition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Trivial.Maths;

namespace Trivial.Data
{
    /// <summary>
    /// The interface for simple condition.
    /// </summary>
    public interface ISimpleCondition
    {
        /// <summary>
        /// Gets or sets he value for the comparing in the condition object.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets or sets he comparing operator.
        /// </summary>
        DbCompareOperator Operator { get; set; }
        
        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        DbValueType ValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the value of the condition is null.
        /// </summary>
        bool ValueIsNull { get; }
    }

    /// <summary>
    /// The base class for simple condition.
    /// </summary>
    public interface IClassSimpleCondition<T> where T : class
    {
        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Gets or sets he comparing operator.
        /// </summary>
        DbCompareOperator Operator { get; set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        DbValueType ValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the value of the condition is null.
        /// </summary>
        bool ValueIsNull { get; }
    }

    /// <summary>
    /// The base class for simple condition.
    /// </summary>
    public interface IStructSimpleCondition<T> where T : struct
    {
        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        T? Value { get; set; }

        /// <summary>
        /// Gets or sets he comparing operator.
        /// </summary>
        DbCompareOperator Operator { get; set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        DbValueType ValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the value of the condition is null.
        /// </summary>
        bool ValueIsNull { get; }
    }

    /// <summary>
    /// The base class for simple condition.
    /// </summary>
    public abstract class SimpleCondition : ISimpleCondition
    {
        /// <summary>
        /// A list about valid comparing operator for null or boolean value.
        /// </summary>
        private static List<DbCompareOperator> _validNullValueOp;

        /// <summary>
        /// A list about valid comparing operator for literal value.
        /// </summary>
        private static List<DbCompareOperator> _validLiteralOp;

        /// <summary>
        /// A list about valid comparing operator for comparable value.
        /// </summary>
        private static List<DbCompareOperator> _validComparableOp;

        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// Gets or sets the comparing operator.
        /// </summary>
        public DbCompareOperator Operator { get; set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public abstract DbValueType ValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the value of the condition is null.
        /// </summary>
        public abstract bool ValueIsNull { get; }

        /// <summary>
        /// Gets a left comparing operator from a simple interval.
        /// </summary>
        /// <typeparam name="T">The type of interval value.</typeparam>
        /// <param name="value">A simple interval instance.</param>
        /// <returns>A comparing operator.</returns>
        public static DbCompareOperator GetLeftOperator<T>(ISimpleInterval<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return value.LeftOpen ? DbCompareOperator.Greater : DbCompareOperator.GreaterOrEqual;
        }

        /// <summary>
        /// Gets a right comparing operator from a simple interval.
        /// </summary>
        /// <typeparam name="T">The type of interval value.</typeparam>
        /// <param name="value">A simple interval instance.</param>
        /// <returns>A comparing operator.</returns>
        public static DbCompareOperator GetRightOperator<T>(ISimpleInterval<T> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return value.RightOpen ? DbCompareOperator.Less : DbCompareOperator.LessOrEqual;
        }

        /// <summary>
        /// Gets the list about valid comparing operator for null or boolean value.
        /// </summary>
        public static ICollection<DbCompareOperator> GetBasicValidOperators()
        {
            return _validNullValueOp ?? (_validNullValueOp = new List<DbCompareOperator>
                                               {
                                                   DbCompareOperator.Equal,
                                                   DbCompareOperator.NotEqual
                                               });
        }

        /// <summary>
        /// Gets the list about valid comparing operator for literal value.
        /// </summary>
        public static ICollection<DbCompareOperator> GetLiteralValidOperators()
        {
            return _validLiteralOp ?? (_validLiteralOp = new List<DbCompareOperator>
                                               {
                                                   DbCompareOperator.Equal,
                                                   DbCompareOperator.NotEqual,
                                                   DbCompareOperator.Contains,
                                                   DbCompareOperator.EndsWith,
                                                   DbCompareOperator.StartsWith
                                               });
        }

        /// <summary>
        /// Gets the list about valid comparing operator for comparable value.
        /// </summary>
        public static ICollection<DbCompareOperator> GetComparableValidOperators()
        {
            return _validComparableOp ?? (_validComparableOp = new List<DbCompareOperator>
                                               {
                                                   DbCompareOperator.Equal,
                                                   DbCompareOperator.NotEqual,
                                                   DbCompareOperator.Greater,
                                                   DbCompareOperator.Less,
                                                   DbCompareOperator.GreaterOrEqual,
                                                   DbCompareOperator.LessOrEqual
                                               });
        }

        /// <summary>
        /// Converts operation string.
        /// </summary>
        /// <param name="op"></param>
        /// <returns>A string that represents the operation.</returns>
        public static string ToString(DbCompareOperator op)
        {
            switch (op)
            {
                case DbCompareOperator.Equal:
                    return BooleanOperatorUtility.EqualSign;
                case DbCompareOperator.Greater:
                    return BooleanOperatorUtility.GreaterSign;
                case DbCompareOperator.GreaterOrEqual:
                    return BooleanOperatorUtility.GreaterOrEqualSign;
                case DbCompareOperator.Less:
                    return BooleanOperatorUtility.LessSign;
                case DbCompareOperator.LessOrEqual:
                    return BooleanOperatorUtility.LessOrEqualSign;
                case DbCompareOperator.NotEqual:
                    return BooleanOperatorUtility.NotEqualSign;
                default:
                    return op.ToString();
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return SimpleCondition.ToString(Operator) + " " + (Value != null ? Value.ToString() : "null");
        }
    }

    /// <summary>
    /// The base generic class for simple condition with a reference value.
    /// </summary>
    /// <typeparam name="T">The type of the value. Should be a reference type.</typeparam>
    public abstract class ClassSimpleCondition<T> : ISimpleCondition, IClassSimpleCondition<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the ClassSimpleCondition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        protected ClassSimpleCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClassSimpleCondition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        protected ClassSimpleCondition(IClassSimpleCondition<T> copier)
        {
            if (copier == null) return;
            Value = copier.Value;
            Operator = copier.Operator;
        }

        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        object ISimpleCondition.Value
        {
            get { return Value; }
            set { Value = (T) value; }
        }

        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the comparing operator.
        /// </summary>
        public DbCompareOperator Operator { get; set; }

        /// <summary>
        /// Gets a value indicating whether the value of the condition is null.
        /// </summary>
        public bool ValueIsNull { get { return Value == null; } }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return SimpleCondition.ToString(Operator) + " " + (Value != null ? Value.ToString() : "null");
        }

        /// <summary>
        /// Checks if the simple condition with reference value is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <param name="handler">A checker handler.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        internal static bool IsValid(ClassSimpleCondition<T> value, Func<DbCompareOperator, bool> handler)
        {
            if (value == null) return false;
            return value.Value == null
                       ? SimpleCondition.GetBasicValidOperators().Contains(value.Operator)
                       : handler(value.Operator);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public abstract DbValueType ValueType { get; }
    }

    /// <summary>
    /// The base generic class for simple condition with a reference value.
    /// </summary>
    /// <typeparam name="T">The type of the value. Should be a struct type.</typeparam>
    public abstract class StructSimpleCondition<T> : ISimpleCondition, IStructSimpleCondition<T> where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the StructSimpleCondition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        protected StructSimpleCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructSimpleCondition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        protected StructSimpleCondition(IStructSimpleCondition<T> copier)
        {
            if (copier == null) return;
            Value = copier.Value;
            Operator = copier.Operator;
        }

        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        object ISimpleCondition.Value
        {
            get { return Value; }
            set { Value = (T) value; }
        }

        /// <summary>
        /// Gets or sets the value for the comparing in the condition object.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// Gets or sets the comparing operator.
        /// </summary>
        public DbCompareOperator Operator { get; set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public abstract DbValueType ValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the value of the condition is null.
        /// </summary>
        public bool ValueIsNull { get { return Value == null; } }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return SimpleCondition.ToString(Operator) + " " + (Value.HasValue ? Value.ToString() : "null");
        }

        /// <summary>
        /// Checks if the simple condition with struct value is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <param name="handler">A checker handler.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        internal static bool IsValid(StructSimpleCondition<T> value, Func<DbCompareOperator, bool> handler)
        {
            if (value == null) return false;
            return value.Value == null
                       ? SimpleCondition.GetBasicValidOperators().Contains(value.Operator)
                       : handler(value.Operator);
        }
    }

    /// <summary>
    /// The simple condition class for string.
    /// </summary>
    public class StringCondition : ClassSimpleCondition<string>
    {
        /// <summary>
        /// Initializes a new instance of the StringCondition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public StringCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the StringCondition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public StringCondition(IClassSimpleCondition<string> copier)
            : base(copier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StringCondition class.
        /// </summary>
        /// <param name="op">The operator in the condition.</param>
        /// <param name="value">The value for comparing in the condition.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public StringCondition(DbCompareOperator op, string value)
        {
            Value = value;
            Operator = op;
        }

        /// <summary>
        /// Creates a string condition as starting with a value.
        /// </summary>
        /// <param name="value">The value for comparing in the condition.</param>
        /// <returns>A StringCondition value.</returns>
        public static StringCondition CreateForStartingWith(string value)
        {
            return new StringCondition { Value = value, Operator = DbCompareOperator.StartsWith };
        }

        /// <summary>
        /// Creates a string condition as ending with a value.
        /// </summary>
        /// <param name="value">The value for comparing in the condition.</param>
        /// <returns>A StringCondition value.</returns>
        public static StringCondition CreateForEndingWith(string value)
        {
            return new StringCondition { Value = value, Operator = DbCompareOperator.EndsWith };
        }

        /// <summary>
        /// Creates a string condition as liking with a value.
        /// </summary>
        /// <param name="value">The value for comparing in the condition.</param>
        /// <returns>A StringCondition value.</returns>
        public static StringCondition CreateForLiking(string value)
        {
            return new StringCondition { Value = value, Operator = DbCompareOperator.Contains };
        }

        /// <summary>
        /// Creates a string condition as equaling with a value.
        /// </summary>
        /// <param name="value">The value for comparing in the condition.</param>
        /// <returns>A StringCondition value.</returns>
        public static StringCondition CreateForEqualing(string value)
        {
            return new StringCondition { Value = value, Operator = DbCompareOperator.Equal };
        }

        /// <summary>
        /// Creates a string condition as not equaling with a value.
        /// </summary>
        /// <param name="value">The value for comparing in the condition.</param>
        /// <returns>A StringCondition value.</returns>
        public static StringCondition CreateForNotEqualing(string value)
        {
            return new StringCondition { Value = value, Operator = DbCompareOperator.NotEqual };
        }

        /// <summary>
        /// Checks if a string condition is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        public static bool IsValid(StringCondition value)
        {
            return IsValid(value, SimpleCondition.GetLiteralValidOperators().Contains);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public override DbValueType ValueType
        {
            get { return DbValueType.LiteralString; }
        }
    }

    /// <summary>
    /// The simple condition class for integer.
    /// </summary>
    public class Int32Condition : StructSimpleCondition<int>
    {
        /// <summary>
        /// Initializes a new instance of the Int32Condition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32Condition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Int32Condition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32Condition(IStructSimpleCondition<int> copier)
            : base(copier)
        {
        }

        /// <summary>
        /// Get a condition from the given struct value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from left bound of a specific simple interval.</returns>
        public static Int32Condition CreateFromLeft(StructValueSimpleInterval<int> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new Int32Condition
            {
                Value = value.MinValue,
                Operator = SimpleCondition.GetLeftOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given struct value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from right bound of a specific simple interval.</returns>
        public static Int32Condition CreateFromRight(StructValueSimpleInterval<int> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new Int32Condition
            {
                Value = value.MaxValue,
                Operator = SimpleCondition.GetRightOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given nullable value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from left bound of a specific simple interval.</returns>
        public static Int32Condition CreateFromLeft(NullableValueSimpleInterval<int> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.LeftBounded) return null;
            return new Int32Condition
            {
                Value = value.MinValue,
                Operator = SimpleCondition.GetLeftOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given nullable value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from right bound of a specific simple interval.</returns>
        public static Int32Condition CreateFromRight(NullableValueSimpleInterval<int> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.RightBounded) return null;
            return new Int32Condition
            {
                Value = value.MaxValue,
                Operator = SimpleCondition.GetRightOperator(value)
            };
        }

        /// <summary>
        /// Checks if a integer condition is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        public static bool IsValid(Int32Condition value)
        {
            return IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public override DbValueType ValueType
        {
            get { return DbValueType.Integer; }
        }
    }

    /// <summary>
    /// The simple condition class for single float.
    /// </summary>
    public class SingleCondition : StructSimpleCondition<float>
    {
        /// <summary>
        /// Initializes a new instance of the SingleCondition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SingleCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SingleCondition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SingleCondition(IStructSimpleCondition<float> copier)
            : base(copier)
        {
        }

        /// <summary>
        /// Get a condition from the given struct value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from left bound of a specific simple interval.</returns>
        public static SingleCondition CreateFromLeft(StructValueSimpleInterval<float> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new SingleCondition
            {
                Value = value.MinValue,
                Operator = SimpleCondition.GetLeftOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given struct value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from right bound of a specific simple interval.</returns>
        public static SingleCondition CreateFromRight(StructValueSimpleInterval<float> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new SingleCondition
            {
                Value = value.MaxValue,
                Operator = SimpleCondition.GetRightOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given nullable value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from left bound of a specific simple interval.</returns>
        public static SingleCondition CreateFromLeft(NullableValueSimpleInterval<float> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.LeftBounded) return null;
            return new SingleCondition
            {
                Value = value.MinValue,
                Operator = SimpleCondition.GetLeftOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given nullable value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from right bound of a specific simple interval.</returns>
        public static SingleCondition CreateFromRight(NullableValueSimpleInterval<float> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.RightBounded) return null;
            return new SingleCondition
            {
                Value = value.MaxValue,
                Operator = SimpleCondition.GetRightOperator(value)
            };
        }

        /// <summary>
        /// Checks if a single float value condition is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        public static bool IsValid(SingleCondition value)
        {
            return IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public override DbValueType ValueType
        {
            get { return DbValueType.SingleDecimal; }
        }
    }

    /// <summary>
    /// The simple condition class for date time.
    /// </summary>
    public class DateTimeCondition : StructSimpleCondition<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeCondition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DateTimeCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DateTimeCondition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DateTimeCondition(IStructSimpleCondition<DateTime> copier)
            : base(copier)
        {
        }

        /// <summary>
        /// Get a condition from the given struct value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from left bound of a specific simple interval.</returns>
        public static DateTimeCondition CreateFromLeft(StructValueSimpleInterval<DateTime> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new DateTimeCondition
            {
                Value = value.MinValue,
                Operator = SimpleCondition.GetLeftOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given struct value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from right bound of a specific simple interval.</returns>
        public static DateTimeCondition CreateFromRight(StructValueSimpleInterval<DateTime> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new DateTimeCondition
            {
                Value = value.MaxValue,
                Operator = SimpleCondition.GetRightOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given nullable value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from left bound of a specific simple interval.</returns>
        public static DateTimeCondition CreateFromLeft(NullableValueSimpleInterval<DateTime> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.LeftBounded) return null;
            return new DateTimeCondition
            {
                Value = value.MinValue,
                Operator = SimpleCondition.GetLeftOperator(value)
            };
        }

        /// <summary>
        /// Get a condition from the given nullable value simple interval left bound.
        /// </summary>
        /// <param name="value">A simple interval.</param>
        /// <returns>A condition from right bound of a specific simple interval.</returns>
        public static DateTimeCondition CreateFromRight(NullableValueSimpleInterval<DateTime> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.RightBounded) return null;
            return new DateTimeCondition
            {
                Value = value.MaxValue,
                Operator = SimpleCondition.GetRightOperator(value)
            };
        }

        /// <summary>
        /// Checks if a date time condition is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        public static bool IsValid(DateTimeCondition value)
        {
            return IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public override DbValueType ValueType
        {
            get { return DbValueType.DateTimeGmt; }
        }
    }

    /// <summary>
    /// The simple condition class for boolean.
    /// </summary>
    public class BooleanCondition : StructSimpleCondition<bool>
    {
        /// <summary>
        /// Initializes a new instance of the BooleanCondition class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public BooleanCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BooleanCondition class.
        /// </summary>
        /// <param name="copier">An instance to copy.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public BooleanCondition(IStructSimpleCondition<bool> copier)
            : base(copier)
        {
        }

        /// <summary>
        /// Checks if a boolean value condition is valid.
        /// </summary>
        /// <param name="value">The condition object.</param>
        /// <returns>true if it is valid; otherwise, false.</returns>
        public static bool IsValid(BooleanCondition value)
        {
            return IsValid(value, SimpleCondition.GetBasicValidOperators().Contains);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public override DbValueType ValueType
        {
            get { return DbValueType.Boolean; }
        }
    }
}
