using System;

namespace LogicOfStatements
{
    internal partial class Formula
    {
        private class Negation : Operation
        {
            public override bool CalculatingValue(TreeNode<ElementFormula> node)
                => !node.LeftNode.Value.CalculatingValue(node.LeftNode);
            public override TreeNode<ElementFormula> CreateNodeTree(Func<TreeNode<ElementFormula>> recursionCreatingTree)
                => new TreeNode<ElementFormula>(Neg, recursionCreatingTree());
            public override TreeNode<ElementFormula> CreateNegationNormalForm(TreeNode<ElementFormula> nodeTree)
            {
                if (nodeTree.LeftNode.Value == Neg)
                    nodeTree = nodeTree.LeftNode.LeftNode;
                else if (nodeTree.LeftNode.Value == Dis || nodeTree.LeftNode.Value == Con)
                {
                    nodeTree.Value = ChangeOperation((Operation)nodeTree.LeftNode.Value);
                    nodeTree.RightNode = new TreeNode<ElementFormula>(Neg, nodeTree.LeftNode.RightNode);
                    nodeTree.LeftNode = new TreeNode<ElementFormula>(Neg, nodeTree.LeftNode.LeftNode);
                }
                else if (nodeTree.LeftNode.Value == Imp)
                {
                    nodeTree.Value = Con;
                    nodeTree.RightNode = new TreeNode<ElementFormula>(Neg, nodeTree.LeftNode.RightNode);
                    nodeTree.LeftNode = nodeTree.LeftNode.LeftNode;
                }
                return base.CreateNegationNormalForm(nodeTree);
            }
            public override string ToString(TreeNode<ElementFormula> nodeTree) => $"-{nodeTree.LeftNode.Value.ToString(nodeTree.LeftNode)}";
        }
        private class Disjunction : Operation
        {
            public override bool CalculatingValue(TreeNode<ElementFormula> node)
                => node.LeftNode.Value.CalculatingValue(node.LeftNode) || node.RightNode.Value.CalculatingValue(node.RightNode);
            public override TreeNode<ElementFormula> CreateNodeTree(Func<TreeNode<ElementFormula>> recursionCreatingTree)
                => new TreeNode<ElementFormula>(Dis, recursionCreatingTree(), recursionCreatingTree());
            public override TreeNode<ElementFormula> CreateConjunctionNormalForm(TreeNode<ElementFormula> nodeTreeInNNF)
                => IsElementaryOperation(nodeTreeInNNF, Dis) ? nodeTreeInNNF : base.CreateConjunctionNormalForm(CreateOperationNormalForm(nodeTreeInNNF, Dis));
            public override string ToString(TreeNode<ElementFormula> nodeTree)
                => $"({nodeTree.LeftNode.Value.ToString(nodeTree.LeftNode)}+{nodeTree.RightNode.Value.ToString(nodeTree.RightNode)})";
        }
        private class Conjunction : Operation
        {
            public override bool CalculatingValue(TreeNode<ElementFormula> node)
                => node.LeftNode.Value.CalculatingValue(node.LeftNode) && node.RightNode.Value.CalculatingValue(node.RightNode);
            public override TreeNode<ElementFormula> CreateNodeTree(Func<TreeNode<ElementFormula>> recursionCreatingTree)
                => new TreeNode<ElementFormula>(Con, recursionCreatingTree(), recursionCreatingTree());
            public override TreeNode<ElementFormula> CreateDisjunctiveNormalForm(TreeNode<ElementFormula> nodeTreeInNNF)
                => IsElementaryOperation(nodeTreeInNNF, Con) ? nodeTreeInNNF : base.CreateDisjunctiveNormalForm(CreateOperationNormalForm(nodeTreeInNNF, Con));
            public override string ToString(TreeNode<ElementFormula> nodeTree)
                => $"({nodeTree.LeftNode.Value.ToString(nodeTree.LeftNode)}*{nodeTree.RightNode.Value.ToString(nodeTree.RightNode)})";
        }
        private class Implication : Operation
        {
            public override bool CalculatingValue(TreeNode<ElementFormula> node)
                => !node.LeftNode.Value.CalculatingValue(node.LeftNode) || node.RightNode.Value.CalculatingValue(node.RightNode);
            public override TreeNode<ElementFormula> CreateNodeTree(Func<TreeNode<ElementFormula>> recursionCreatingTree)
                => new TreeNode<ElementFormula>(Imp, recursionCreatingTree(), recursionCreatingTree());
            public override TreeNode<ElementFormula> CreateNegationNormalForm(TreeNode<ElementFormula> nodeTree)
            {
                nodeTree.Value = Dis;
                nodeTree.LeftNode = new TreeNode<ElementFormula>(Neg, nodeTree.LeftNode);
                return base.CreateNegationNormalForm(nodeTree);
            }
            public override TreeNode<ElementFormula> CreateDisjunctiveNormalForm(TreeNode<ElementFormula> nodeTreeInNNF)
                => throw new ArgumentException("Формула не находится в форме с тесными отрицаниями");
            public override TreeNode<ElementFormula> CreateConjunctionNormalForm(TreeNode<ElementFormula> nodeTreeInNNF)
                => throw new ArgumentException("Формула не находится в форме с тесными отрицаниями");
            public override string ToString(TreeNode<ElementFormula> nodeTree)
                => $"({nodeTree.LeftNode.Value.ToString(nodeTree.LeftNode)}>{nodeTree.RightNode.Value.ToString(nodeTree.RightNode)})";
        }
    }
}