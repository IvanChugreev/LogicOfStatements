using System;

namespace LogicOfStatements
{
    internal class TreeNode<T> : ICloneable, IEquatable<TreeNode<T>>
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (value == null) throw new ArgumentNullException();
                else _value = value;
            }
        }
        public TreeNode<T> LeftNode { get; set; }
        public TreeNode<T> RightNode { get; set; }
        public TreeNode(T value, TreeNode<T> leftNode) : this(value, leftNode, null) { }
        public TreeNode(T value) => Value = value;
        public TreeNode(T value, TreeNode<T> leftNode, TreeNode<T> rightNode)
        {
            Value = value;
            LeftNode = leftNode;
            RightNode = rightNode;
        }
        public object Clone() => new TreeNode<T>(_value, (TreeNode<T>)LeftNode?.Clone(), (TreeNode<T>)RightNode?.Clone());
        public bool Equals(TreeNode<T> other) => 
            other != null && Value.Equals(other.Value) && 
            ((LeftNode?.Equals(other.LeftNode) ?? other.LeftNode == null) && (RightNode?.Equals(other.RightNode) ?? other.RightNode == null) || 
            (LeftNode?.Equals(other.RightNode) ?? other.RightNode == null) && (RightNode?.Equals(other.LeftNode) ?? other.LeftNode == null));
    }
}
