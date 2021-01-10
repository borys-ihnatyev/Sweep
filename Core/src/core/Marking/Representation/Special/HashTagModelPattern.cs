﻿using System.Collections.Generic;
using Sweep.Core.Marking.Representation;

namespace Sweep.Core.Processing.AutoFolder
{
    public class HashTagModelPattern
    {
        public HashTagModelPattern(HashTagModel model, int priority)
        {
            Model = model;
            Priority = priority;
        }

        public HashTagModel Model { get; private set; }

        public int Priority { get; private set; }

        public class Comparer : HashTagModel.Comparer, IComparer<HashTagModelPattern>
        {
            public Comparer() : base(true)
            {
            }

            public int Compare(HashTagModelPattern x, HashTagModelPattern y)
            {
                if (x != null && y == null)
                    return Higher;

                if (x == null && y != null)
                    return Lower;

                if (x.Priority > y.Priority)
                    return Higher;

                if (x.Priority < y.Priority)
                    return Lower;

                return Compare(x.Model, y.Model);
            }
        }
    }
}
