using Ardalis.GuardClauses;

namespace System.Reflection
{
    public static class ObjectExtensions
    {
        public static object GetPropertyValue(this object target, string propertyName)
        {
            Guard.Against.Null(target, nameof(target));

            object propertyValue = target.GetType().GetProperty(propertyName).GetValue(target);
            return propertyValue;
        }


        public static void SetPropertyValue(this object target, object value, string propertyName)
        {
            Guard.Against.Null(target, nameof(target));

            target.GetType().GetProperty(propertyName).SetValue(target, value);
        }
    }
}