using Azure.Core;

namespace EDSimulator
{
    public static class Extensions
    {
        public static T GetRandomValue<T>(this Type t) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException("Random enum type is not an enum");

            var random = new Random();

            var values = Enum.GetValues(typeof(T));

            if (values == null || values.Length == 0)
                throw new InvalidOperationException("Cannot get random value from enum as no values exist.");

#pragma warning disable CS8605 // Unboxing a possibly null value.
            return (T)values.GetValue(random.Next(values.Length));
#pragma warning restore CS8605 // Unboxing a possibly null value.
        }

        public static KeyValuePair<TKey, TValue> GetRandomEntry<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : class
            where TValue : class
        {
            var random = new Random();

            var entry = dictionary.ElementAt(random.Next(0, dictionary.Count));

            return entry;
        }
    }
}