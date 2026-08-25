using System;
using UnityEngine;

namespace Module5.Player
{
    [ConditionOperator("Equals", typeof(bool))]
    public class BoolEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "bool";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToBoolean(sourceValue) == Convert.ToBoolean(expectedValue);
    }

    [ConditionOperator("Equals", true, typeof(UnityEngine.Object))]
    public class ObjectEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "object_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => EvaluateObjectEquals(sourceValue, expectedValue);

        public static bool EvaluateObjectEquals(object sourceValue, object expectedValue)
        {
            if (sourceValue == null && expectedValue == null) return true;

            var uSource = sourceValue as UnityEngine.Object;
            var uExpected = expectedValue as UnityEngine.Object;

            bool isSourceNull = sourceValue == null || (uSource != null && uSource == null);
            bool isExpectedNull = expectedValue == null || (uExpected != null && uExpected == null);

            if (isSourceNull && isExpectedNull) return true;
            if (isSourceNull || isExpectedNull) return false;

            if (uSource != null && uExpected != null)
            {
                if (uSource == uExpected) return true;

                GameObject goSource = uSource is GameObject g1 ? g1 : (uSource is Component c1 ? c1.gameObject : null);
                GameObject goExpected = uExpected is GameObject g2 ? g2 : (uExpected is Component c2 ? c2.gameObject : null);

                if (goSource != null && goExpected != null && goSource == goExpected)
                {
                    if (uSource is GameObject || uExpected is GameObject)
                    {
                        return true;
                    }
                }
                return false;
            }

            return Equals(sourceValue, expectedValue);
        }
    }

    [ConditionOperator("Not Equals", true, typeof(UnityEngine.Object))]
    public class ObjectNotEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "object_not_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => !ObjectEqualsOperatorEvaluator.EvaluateObjectEquals(sourceValue, expectedValue);
    }

    [ConditionOperator("Equals", typeof(string))]
    public class StringEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "string_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => string.Equals(sourceValue?.ToString(), expectedValue?.ToString());
    }

    [ConditionOperator("Not Equals", typeof(string))]
    public class StringNotEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "string_not_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => !string.Equals(sourceValue?.ToString(), expectedValue?.ToString());
    }

    [ConditionOperator("Equals", typeof(float), typeof(int), typeof(double))]
    public class NumericEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "numeric_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDouble(sourceValue) == Convert.ToDouble(expectedValue);
    }

    [ConditionOperator("Not Equals", typeof(float), typeof(int), typeof(double))]
    public class NumericNotEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "numeric_not_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDouble(sourceValue) != Convert.ToDouble(expectedValue);
    }

    [ConditionOperator("Greater Than", typeof(float), typeof(int), typeof(double))]
    public class NumericGreaterThanOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "numeric_greater_than";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDouble(sourceValue) > Convert.ToDouble(expectedValue);
    }

    [ConditionOperator("Greater Than or Equals", typeof(float), typeof(int), typeof(double))]
    public class NumericGreaterThanEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "numeric_greater_than_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDouble(sourceValue) >= Convert.ToDouble(expectedValue);
    }

    [ConditionOperator("Less Than", typeof(float), typeof(int), typeof(double))]
    public class NumericLessThanOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "numeric_less_than";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDouble(sourceValue) < Convert.ToDouble(expectedValue);
    }

    [ConditionOperator("Less Than or Equals", typeof(float), typeof(int), typeof(double))]
    public class NumericLessThanEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "numeric_less_than_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDouble(sourceValue) <= Convert.ToDouble(expectedValue);
    }

    [ConditionOperator("Equals", typeof(DateTime))]
    public class DateEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_equal";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) == Convert.ToDateTime(expectedValue);
    }

    [ConditionOperator("Not Equals", typeof(DateTime))]
    public class DateNotEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_not_equal";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) != Convert.ToDateTime(expectedValue);
    }

    [ConditionOperator("Greater Than", typeof(DateTime))]
    public class DateGreaterThanOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_greater_than";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) > Convert.ToDateTime(expectedValue);
    }

    [ConditionOperator("Greater Than or Equals", typeof(DateTime))]
    public class DateGreaterThanEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_greater_than_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) >= Convert.ToDateTime(expectedValue);
    }

    [ConditionOperator("Less Than", typeof(DateTime))]
    public class DateLessThanOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_less_than";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) < Convert.ToDateTime(expectedValue);
    }

    [ConditionOperator("Less Than or Equals", typeof(DateTime))]
    public class DateLessThanEqualsOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_less_than_equals";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) <= Convert.ToDateTime(expectedValue);
    }

    [ConditionOperator("Equals Today", typeof(DateTime))]
    public class DateEqualsNowOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_equal_now";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue).Date == DateTime.Now.Date;
    }

    [ConditionOperator("Not Equals Today", typeof(DateTime))]
    public class DateNotEqualsNowOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_not_equal_now";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue).Date != DateTime.Now.Date;
    }

    [ConditionOperator("After Now", typeof(DateTime))]
    public class DateGreaterThanNowOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_greater_now";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) > DateTime.Now;
    }

    [ConditionOperator("Before Now", typeof(DateTime))]
    public class DateLessThanNowOperatorEvaluator : IConditionOperatorEvaluator
    {
        public const string Key = "date_less_than_now";
        public string OperatorKey => Key;
        public bool Evaluate(object sourceValue, object expectedValue) => Convert.ToDateTime(sourceValue) < DateTime.Now;
    }
}
