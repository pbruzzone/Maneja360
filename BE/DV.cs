using System;
using System.Linq;

namespace BE
{
    public class DV : IEquatable<DV>
    {
        public string TableName { get;  }
        public long[][] Hash { get; }

        public DV(string tableName, long[][] hash)
        {
            TableName = tableName;
            Hash = hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DV)obj);
        }

        public bool Equals(DV other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return TableName == other.TableName && AreJaggedArraysEqual(Hash, other.Hash);
        }

        private static bool AreJaggedArraysEqual(long[][] array1, long[][] array2)
        {
            if (array1.Length != array2.Length) return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].SequenceEqual(array2[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TableName != null ? TableName.GetHashCode() : 0) * 397) ^ (Hash != null ? Hash.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DV left, DV right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DV left, DV right)
        {
            return !(left == right);
        }
    }
}