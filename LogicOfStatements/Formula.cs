using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicOfStatements
{
    internal partial class Formula
    {
        private static readonly Negation Neg = new Negation();
        private static readonly Disjunction Dis = new Disjunction();
        private static readonly Conjunction Con = new Conjunction();
        private static readonly Implication Imp = new Implication();

        private string _stringFormula;
        public string StringFormula
        {
            get => _stringFormula;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                else
                {
                    _stringFormula = value;
                    IsFormula = TryCreateNPN(_stringFormula, out _NPN);
                    TreeFormula = CreateTreeFormula(_NPN);
                }
            }
        }
        // NPN - normal polish notation
        private List<ElementFormula> _NPN;
        public bool IsFormula { get; private set; }
        public TreeNode<ElementFormula> TreeFormula { get; private set; }
        public Formula(string inputString) => StringFormula = inputString;
        public static bool TryCreateNPN(string inputString, out List<ElementFormula> normalPolishNotatin)
        {
            List<ElementFormula> NPN = new List<ElementFormula>();
            bool result = _tryCreateNPN(inputString);
            normalPolishNotatin = result ? NPN : new List<ElementFormula>();
            return result;

            bool _tryCreateNPN(string _inputString)
            {
                if (string.IsNullOrEmpty(_inputString)) return false;
                else if (Variable.IsVariable(_inputString)) { NPN.Add(Variable.GetVariable(_inputString)); return true; }
                else if (Operation.IsUnary(_inputString[0])) { NPN.Add(Neg); return _tryCreateNPN(_inputString.Substring(1)); }
                else return IsBinaryOperationFormula(_inputString);

                bool IsBinaryOperationFormula(string stringFormula)
                {
                    int indexOperation;
                    if (stringFormula[0] != '(' || stringFormula[stringFormula.Length - 1] != ')'
                        || (indexOperation = FindIndexLastOperationInOrder(stringFormula)) == -1)
                        return false;
                    NPN.Add(Operation.DefineOperation(stringFormula[indexOperation]));
                    string leftOperand = stringFormula.Substring(1, indexOperation - 1),
                        rightOperand = stringFormula.Substring(indexOperation + 1, stringFormula.Length - indexOperation - 2);
                    return _tryCreateNPN(leftOperand) && _tryCreateNPN(rightOperand);

                    int FindIndexLastOperationInOrder(string inputStringFormula)
                    {
                        int openigBracketsCounter = 0, closingBracketsCounter = 0;
                        for (int i = 1; i < inputStringFormula.Length - 1; i++)
                            if (inputStringFormula[i] == '(') openigBracketsCounter++;
                            else if (inputStringFormula[i] == ')') closingBracketsCounter++;
                            else if (openigBracketsCounter == closingBracketsCounter && Operation.IsBinary(inputStringFormula[i])) return i;
                        return -1;
                    }
                }
            }
        }
        public static TreeNode<ElementFormula> CreateTreeFormula(IEnumerable<ElementFormula> NPN)
        {
            IEnumerator<ElementFormula> enumeratorNPN = NPN.GetEnumerator();
            return RecursionCreatingTree();

            TreeNode<ElementFormula> RecursionCreatingTree()
            {
                if (!enumeratorNPN.MoveNext()) return null;
                return enumeratorNPN.Current.CreateNodeTree(RecursionCreatingTree);
            }
        }
        public bool IsEquivalent(Formula otherFormula)
        {
            if (!IsFormula || !otherFormula.IsFormula)
                throw new ArgumentException($"{StringFormula} и/или {otherFormula.StringFormula} не являются формулами");
            Variable[] allVariable = GetVariablesUsed(_NPN).Union(GetVariablesUsed(otherFormula._NPN)).ToArray();
            bool[] valuesOfVariables = new bool[allVariable.Length];
            int CountOfUpdatedVariables = 0;
            foreach (Variable variable in allVariable) variable.Value = false;
            return CheckAllInterpretation();

            IEnumerable<Variable> GetVariablesUsed(IEnumerable<ElementFormula> NPN)
            {
                HashSet<Variable> result = new HashSet<Variable>();
                foreach (ElementFormula item in NPN) if (item is Variable variable) result.Add(variable);
                return result;
            }
            bool CheckAllInterpretation()
            {
                do
                {
                    for (int i = 0; i < CountOfUpdatedVariables; i++) allVariable[i].Value = valuesOfVariables[i];
                    if (TreeFormula.Value.CalculatingValue(TreeFormula) != otherFormula.TreeFormula.Value.CalculatingValue(otherFormula.TreeFormula))
                        return false;
                } while ((CountOfUpdatedVariables = IncrementBinaryNumber(valuesOfVariables)) != 0);
                return true;

                int IncrementBinaryNumber(bool[] boolArray)
                {
                    if (boolArray == null || boolArray.Length == 0) throw new ArgumentException("Массив не должен быть пустым или равным null");
                    for (int i = 0; i < boolArray.Length; i++)
                    {
                        boolArray[i] = !boolArray[i];
                        if (boolArray[i])
                            return i + 1;
                    }
                    return 0;
                }
            }
        }
        // NNF - negation normal form
        public static TreeNode<ElementFormula> CreateNNF(TreeNode<ElementFormula> treeFormula)
            => treeFormula.Value.CreateNegationNormalForm((TreeNode<ElementFormula>)treeFormula.Clone());
        // DNF - disjunctive normal form
        public static TreeNode<ElementFormula> CreateDNF(TreeNode<ElementFormula> treeFormula)
        {
            TreeNode<ElementFormula> treeFormulaInNNF = CreateNNF(treeFormula);
            return treeFormulaInNNF.Value.CreateDisjunctiveNormalForm(treeFormulaInNNF);
        }
        // CNF - conjunction normal form
        public static TreeNode<ElementFormula> CreateCNF(TreeNode<ElementFormula> treeFormula)
        {
            TreeNode<ElementFormula> treeFormulaInNNF = CreateNNF(treeFormula);
            return treeFormulaInNNF.Value.CreateConjunctionNormalForm(treeFormulaInNNF);
        }
        public static string ToString(TreeNode<ElementFormula> treeNode) => treeNode.Value.ToString(treeNode);
    }
}
