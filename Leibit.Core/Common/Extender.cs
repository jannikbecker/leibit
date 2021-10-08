using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Leibit.Core.Common
{
    public static class Extender
    {

        private class PassengerTrains
        {
            public List<string> TrainTypes { get; set; }
        }

        #region - Needs -
        private static List<string> m_PassengerTrainTypes;
        #endregion

        #region - Properties -
        public static List<string> PassengerTrainTypes
        {
            get
            {
                if (m_PassengerTrainTypes == null)
                    m_PassengerTrainTypes = __LoadPassengerTrainTypes();

                return m_PassengerTrainTypes;
            }
        }
        #endregion

        #region - String extender -

        public static bool IsNullOrEmpty(this string input)
        {
            return String.IsNullOrEmpty(input);
        }

        public static bool IsNotNullOrEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }

        public static bool IsNullOrWhiteSpace(this string input)
        {
            return String.IsNullOrWhiteSpace(input);
        }

        public static bool IsNotNullOrWhiteSpace(this string input)
        {
            return !String.IsNullOrWhiteSpace(input);
        }

        public static bool IsPassengerTrain(this string trainType)
        {
            if (trainType.IsNullOrWhiteSpace())
                return false;

            return PassengerTrainTypes.Contains(trainType.ToUpper());
        }

        #endregion

        #region - List extender -

        public static void AddIfNotNull<T>(this ICollection<T> list, T item)
        {
            if (item != null)
                list.Add(item);
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            items.ForEach(x => list.Add(x));
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null)
                return;

            foreach (T element in source)
                action(element);
        }

        #endregion

        #region - Dictionary extender -

        public static void AddIfNotNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (key != null && value != null)
                dictionary.Add(key, value);
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            var Result = new Dictionary<TKey, TValue>();

            foreach (var Pair in original)
                Result.Add(Pair.Key, Pair.Value);

            return Result;
        }

        #endregion

        #region [CloneObject]
        public static T CloneObject<T>(this T Source)
        {
            var Ser = new DataContractSerializer(typeof(T));

            using (var Stream = new MemoryStream())
            {
                Ser.WriteObject(Stream, Source);
                Stream.Seek(0, SeekOrigin.Begin);
                return (T)Ser.ReadObject(Stream);
            }
        }
        #endregion

        #region [__LoadPassengerTrainTypes]
        private static List<string> __LoadPassengerTrainTypes()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Data.passenger_trains.json"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var obj = JsonConvert.DeserializeObject<PassengerTrains>(reader.ReadToEnd());
                        return obj.TrainTypes;
                    }
                }
            }

            return null;
        }
        #endregion

    }
}
