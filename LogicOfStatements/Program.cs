using System;

namespace LogicOfStatements
{
    internal class Program
    {
        static readonly string[] menu =
        {
            "1. Проверить является ли строка формулой логики высказывания.",
            "2. Проверить эквивалентность двух формул.",
            "3. Найти эквивалентную формулу в ДНФ, КНФ.",
            "4. Выход."
        };
        static readonly History historyTask1 = new History("HistoryTask1");
        static readonly History historyTask2 = new History("HistoryTask2");
        static readonly History historyTask3 = new History("HistoryTask3");
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(string.Join("\r\n", menu));
                Console.Write("Выберите пункт меню: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": Task1(); break;
                    case "2": Task2(); break;
                    case "3": Task3(); break;
                    case "4": return;
                    default: Console.WriteLine("Неверный ввод, выберете от 1 до 4."); break;
                }
                Console.WriteLine("\r\n\r\nНажмите любую кнопку...");
                Console.ReadKey();
            }
        }
        static void Task1()
        {
            Console.Write("Введите выражение: ");
            Formula formula = new Formula(Console.ReadLine().Replace(" ", ""));
            string output = formula.IsFormula ? "Является формулой" : "Не является формулой";
            Console.WriteLine();
            Console.WriteLine(output);
            historyTask1.Add(string.Join("\r\n", new string[] { formula.StringFormula, output }));
        }
        static void Task2()
        {
            Console.Write("Введите первую формулу: ");
            Formula formula1 = new Formula(Console.ReadLine().Replace(" ", ""));
            Console.Write("Введите вторую формулу: ");
            Formula  formula2 = new Formula(Console.ReadLine().Replace(" ", ""));
            string output;
            if (formula1.IsFormula && formula2.IsFormula)
                output = formula1.IsEquivalent(formula2) ? "Формулы эквивалентны" : "Формулы не эквивалентны";
            else
                output = (formula1.IsFormula ? "" : $"{formula1.StringFormula} не является формулой\r\n") + 
                    (formula2.IsFormula ? "" : $"{formula2.StringFormula} не является формулой");
            Console.WriteLine();
            Console.WriteLine(output);
            historyTask2.Add(string.Join("\r\n", new string[] { formula1.StringFormula, formula2.StringFormula, output }));
        }
        static void Task3()
        {
            Console.Write("Введите формулу: ");
            Formula formula = new Formula(Console.ReadLine().Replace(" ", ""));
            string output;
            if (formula.IsFormula)
                output = "ДНФ: " + Formula.ToString(Formula.CreateDNF(formula.TreeFormula)) +
                    "\r\nКНФ: " + Formula.ToString(Formula.CreateCNF(formula.TreeFormula));
            else
                output = $"{formula.StringFormula} не является формулой";
            Console.WriteLine();
            Console.WriteLine(output);
            historyTask3.Add(string.Join("\r\n", new string[] { formula.StringFormula, output }));
        }
    }
}
