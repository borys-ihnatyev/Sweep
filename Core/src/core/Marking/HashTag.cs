using System;
using System.Collections.Generic;

namespace Sweep.Core.Marking
{
    public partial class HashTag : IHashTag
    {
        public const string Hash = "#";

        public const string Meta = "@";

        private readonly string tagValue;
        private readonly string tagMetaValue;

        public HashTag(string tagValue, string tagMetaValue)
        {
            this.tagValue = tagValue.Replace(" ", string.Empty).ToLower();
            this.tagMetaValue = tagMetaValue.Replace(" ", string.Empty).ToLower();
        }

        public HashTag(string tagValue) : this(tagValue, string.Empty)
        {
        }

        public HashTag() : this(string.Empty, string.Empty)
        {
        }

        public virtual string TagValue
        {
            get { return tagValue; }
        }

        public string TagMetaValue
        {
            get { return tagMetaValue; }
        }

        public bool IsEmpty()
        {
            return TagValue == string.Empty;
        }

        public override string ToString()
        {
            var tag = Hash + TagValue;
            if (!String.IsNullOrWhiteSpace(TagMetaValue))
                tag += Meta + TagMetaValue;
            return tag;
        }

        public static implicit operator string(HashTag hashTag)
        {
            return hashTag.ToString();
        }

        protected virtual bool Equals(HashTag other)
        {
            return string.Equals(tagValue, other.tagValue) && string.Equals(tagMetaValue, other.tagMetaValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HashTag) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((tagValue != null ? tagValue.GetHashCode() : 0)*397) ^
                       (tagMetaValue != null ? tagMetaValue.GetHashCode() : 0);
            }
        }

        public class Comparer : IComparer<HashTag>, IComparer<KeyHashTag>
        {
            protected readonly int Higher;

            protected readonly int Lower;

            protected readonly int Equal;

            public Comparer(bool isDesc = false)
            {
                if (isDesc)
                {
                    Higher = -1;
                    Lower = 1;
                    Equal = 0;
                }
                else
                {
                    Higher = 1;
                    Lower = -1;
                    Equal = 0;
                }
            }

            public int Compare(HashTag x, HashTag y)
            {
                if (Equals(x, y)) return Equal;

                if (x == null) return Higher;
                if (y == null) return Lower;

                if (x.Equals(y)) return Equal;

                if (string.IsNullOrWhiteSpace(x.TagValue)) return Higher;
                if (string.IsNullOrWhiteSpace(y.TagValue)) return Lower;

                var xKeyHashTag = x as KeyHashTag;
                var yKeyHashTag = y as KeyHashTag;

                if (xKeyHashTag != null || yKeyHashTag != null)
                    return Compare(xKeyHashTag, yKeyHashTag);

                var compare = string.Compare(x, y, true);

                if (compare == 1)
                    return Higher;

                return compare == -1 ? Lower : Equal;
            }

            public int Compare(KeyHashTag x, KeyHashTag y)
            {
                if (Equals(x, y)) return Equal;

                if (x == null) return Higher;
                if (y == null) return Lower;

                if (x.Key == y.Key) return Equal;

                foreach (var key in CircleOfFifths.AllKeys)
                    if (key == x.Key)
                        return Lower;
                    else if (key == y.Key)
                        return Higher;

                throw new InvalidOperationException("Must allways return in code above");
            }
        }
    }
}
