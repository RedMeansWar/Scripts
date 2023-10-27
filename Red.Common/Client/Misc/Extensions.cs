using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace Red.Common.Client.Misc
{
    // Forked From Albo1125 Fire Script (https://github.com/Albo1125/FireScript/blob/master/FireScript/Vector3Extension.cs)
    public static class Extensions
    {
        private static Random random = new Random(Environment.TickCount);
        public static Vector3 Around(this Vector3 start, float radius)
        {
            // Random direction.
            Vector3 direction = RandomXY();
            Vector3 around = start + (direction * radius);
            return around;
        }

        public static Vector3 Around(this Vector3 start, float MinDistance, float MaxDistance)
        {
            return start.Around(GetRandomFloat(MinDistance, MaxDistance));
        }
        public static float GetRandomFloat(float minimum, float maximum)
        {

            return (float)random.NextDouble() * (maximum - minimum) + minimum;
        }
        public static float DistanceTo(this Vector3 start, Vector3 end)
        {
            return (end - start).Length();
        }

        public static Vector3 RandomXY()
        {
            Vector3 vector3 = new Vector3();
            vector3.X = (float)(random.NextDouble() - 0.5);
            vector3.Y = (float)(random.NextDouble() - 0.5);
            vector3.Z = 0.0f;
            vector3.Normalize();
            return vector3;
        }

        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.TryGetValue(key, out object value) && value is T t)
            {
                return t;
            }

            return defaultVal;
        }
    }

    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public TupleList(TupleList<T1, T2> tupleList)
        {
            foreach (Tuple<T1, T2> tuple in tupleList)
            {
                Add(tuple);
            }
        }

        public TupleList() { }
        public void Add(T1 item, T2 item2)
        {
            Add(new Tuple<T1, T2>(item, item2));
        }
    }

    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2, T3> tuplelist)
        {
            foreach (Tuple<T1, T2, T3> tuple in tuplelist)
            {
                this.Add(tuple);
            }
        }
        public void Add(T1 item, T2 item2, T3 item3)
        {
            Add(new Tuple<T1, T2, T3>(item, item2, item3));
        }

    }
    public class TupleList<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>>
    {
        public TupleList() { }
        public TupleList(TupleList<T1, T2, T3, T4> tuplelist)
        {
            foreach (Tuple<T1, T2, T3, T4> tuple in tuplelist)
            {
                Add(tuple);
            }
        }
        public void Add(T1 item, T2 item2, T3 item3, T4 item4)
        {
            Add(new Tuple<T1, T2, T3, T4>(item, item2, item3, item4));
        }
    }
}