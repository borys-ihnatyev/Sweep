﻿namespace System.Collections.Generic
{
    public partial class CircleLinkedList<T>
    {
        public sealed class Node<TNode>
        {
            internal Node<TNode> next;
            internal Node<TNode> prev;
            internal CircleLinkedList<TNode> list;

            public Node(T value)
            {
                Value = value;
                list = null;
            }

            internal void Clear()
            {
                next = null;
                prev = null;
                list = null;
            }

            public CircleLinkedList<TNode> List
            {
                get { return list; }
            }

            public T Value { get; set; }

            public Node<TNode> Next
            {
                get { return next; }
            }

            public Node<TNode> Previous
            {
                get { return prev; }
            }
        }
    }
}
