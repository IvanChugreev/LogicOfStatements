using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace LogicOfStatements
{
    internal partial class Formula
    {
        public abstract class ElementFormula
        {
            public abstract bool CalculatingValue(TreeNode<ElementFormula> node);
            public abstract TreeNode<ElementFormula> CreateNodeTree(Func<TreeNode<ElementFormula>> recursionCreateTree);
            public abstract TreeNode<ElementFormula> CreateNegationNormalForm(TreeNode<ElementFormula> nodeTree);
            public abstract TreeNode<ElementFormula> CreateDisjunctiveNormalForm(TreeNode<ElementFormula> nodeTreeInNNF);
            public abstract TreeNode<ElementFormula> CreateConjunctionNormalForm(TreeNode<ElementFormula> nodeTreeInNNF);
            public abstract string ToString(TreeNode<ElementFormula> nodeTree);
        }
        private class Variable : ElementFormula, IEquatable<Variable>
        {
            private static readonly Dictionary<string, Variable> s_allNames = new Dictionary<string, Variable>();
            public string Name { get; private set; }
            public bool Value { get; set; }
            private Variable(string name) => Name = name;
            public static Variable GetVariable(string name)
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentException("Имя не может быть пустой строкой или равным null");
                else if (s_allNames.ContainsKey(name)) return s_allNames[name];
                else
                {
                    Variable newVariable = new Variable(name);
                    s_allNames.Add(name, newVariable);
                    return newVariable;
                }
            }
            public static bool IsVariable(string inputString)
            {
                return !char.IsDigit(inputString[0]) && IsStringConsistOnlyOfLettersOrDigit();

                bool IsStringConsistOnlyOfLettersOrDigit()
                {
                    for (int i = 0; i < inputString.Length; i++)
                        if (!char.IsLetterOrDigit(inputString[i]))
                            return false;
                    return true;
                }
            }
            public bool Equals(Variable other)
            {
                if (other == null) throw new ArgumentNullException();
                return Name == other.Name;
            }
            public override bool Equals(object obj) => obj is Variable variable && Equals(variable);
            public override int GetHashCode() => Name.GetHashCode();
            public override bool CalculatingValue(TreeNode<ElementFormula> node) => Value;
            public override TreeNode<ElementFormula> CreateNodeTree(Func<TreeNode<ElementFormula>> recursionCreateTree)
                => new TreeNode<ElementFormula>(this);
            public override TreeNode<ElementFormula> CreateNegationNormalForm(TreeNode<ElementFormula> nodeTree) => nodeTree;
            public override TreeNode<ElementFormula> CreateDisjunctiveNormalForm(TreeNode<ElementFormula> nodeTreeInNNF) => nodeTreeInNNF;
            public override TreeNode<ElementFormula> CreateConjunctionNormalForm(TreeNode<ElementFormula> nodeTreeInNNF) => nodeTreeInNNF;
            public override string ToString(TreeNode<ElementFormula> nodeTree) => ToString();
            public override string ToString() => Name.ToString();
        }
        private abstract class Operation : ElementFormula
        {
            public static bool IsUnary(char symbol) => symbol == '-';
            public static bool IsBinary(char symbol) => symbol == '+' || symbol == '*' || symbol == '>';
            public static Operation ChangeOperation(Operation operation) => (operation == Dis) ? (Operation)Con : Dis;
            public static Operation DefineOperation(char inputSymbol)
            {
                switch (inputSymbol)
                {
                    case '-': return Neg;
                    case '+': return Dis;
                    case '*': return Con;
                    case '>': return Imp;
                    default: throw new ArgumentException($"{inputSymbol} не является операцией");
                }
            }
            public static bool IsElementaryOperation(TreeNode<ElementFormula> treeNode, Operation operation)
            {
                return IsElementary(treeNode);

                bool IsElementary(TreeNode<ElementFormula> _treeNode)
                {
                    if (_treeNode.Value == Neg || _treeNode.Value is Variable) return true;
                    else if (_treeNode.Value != operation) return false;
                    else return IsElementary(_treeNode.LeftNode) && IsElementary(_treeNode.RightNode);
                }
            }
            protected static TreeNode<ElementFormula> CreateOperationNormalForm(TreeNode<ElementFormula> nodeTreeInNNF, Operation operationFrom)
            {
                Operation operationInto = ChangeOperation(operationFrom);
                return CreateOperationNormalForm(nodeTreeInNNF);

                TreeNode<ElementFormula> CreateOperationNormalForm(TreeNode<ElementFormula> _nodeTreeInNNF)
                {
                    if (_nodeTreeInNNF.LeftNode.Value != operationInto && _nodeTreeInNNF.RightNode.Value != operationInto)
                    {
                        if (!IsElementaryOperation(_nodeTreeInNNF.LeftNode, operationFrom)) _nodeTreeInNNF.LeftNode = CreateOperationNormalForm(_nodeTreeInNNF.LeftNode);
                        if (!IsElementaryOperation(_nodeTreeInNNF.RightNode, operationFrom)) _nodeTreeInNNF.RightNode = CreateOperationNormalForm(_nodeTreeInNNF.RightNode);
                    }
                    return ApplicationAssociativityProperty(_nodeTreeInNNF);

                    TreeNode<ElementFormula> ApplicationAssociativityProperty(TreeNode<ElementFormula> treeNode)
                    {
                        _nodeTreeInNNF.Value = operationInto;
                        TreeNode<ElementFormula> temp = _nodeTreeInNNF.RightNode;
                        if (_nodeTreeInNNF.LeftNode.Value != operationInto)
                        {
                            temp = _nodeTreeInNNF.LeftNode;
                            _nodeTreeInNNF.LeftNode = _nodeTreeInNNF.RightNode;
                            _nodeTreeInNNF.RightNode = temp;
                        }
                        _nodeTreeInNNF.RightNode = new TreeNode<ElementFormula>(operationFrom, _nodeTreeInNNF.LeftNode.RightNode, temp);
                        _nodeTreeInNNF.LeftNode = new TreeNode<ElementFormula>(operationFrom, _nodeTreeInNNF.LeftNode.LeftNode, temp);
                        return _nodeTreeInNNF;
                    }
                }
            }
            public override TreeNode<ElementFormula> CreateNegationNormalForm(TreeNode<ElementFormula> nodeTree)
            {
                nodeTree.LeftNode = nodeTree.LeftNode?.Value.CreateNegationNormalForm(nodeTree.LeftNode);
                nodeTree.RightNode = nodeTree.RightNode?.Value.CreateNegationNormalForm(nodeTree.RightNode);
                return nodeTree;
            }
            public override TreeNode<ElementFormula> CreateDisjunctiveNormalForm(TreeNode<ElementFormula> nodeTreeInNNF)
            {
                nodeTreeInNNF.LeftNode = nodeTreeInNNF.LeftNode?.Value.CreateDisjunctiveNormalForm(nodeTreeInNNF.LeftNode);
                nodeTreeInNNF.RightNode = nodeTreeInNNF.RightNode?.Value.CreateDisjunctiveNormalForm(nodeTreeInNNF.RightNode);
                return nodeTreeInNNF;
            }
            public override TreeNode<ElementFormula> CreateConjunctionNormalForm(TreeNode<ElementFormula> nodeTreeInNNF)
            {
                nodeTreeInNNF.LeftNode = nodeTreeInNNF.LeftNode?.Value.CreateConjunctionNormalForm(nodeTreeInNNF.LeftNode);
                nodeTreeInNNF.RightNode = nodeTreeInNNF.RightNode?.Value.CreateConjunctionNormalForm(nodeTreeInNNF.RightNode);
                return nodeTreeInNNF;
            }
        }
    }
}