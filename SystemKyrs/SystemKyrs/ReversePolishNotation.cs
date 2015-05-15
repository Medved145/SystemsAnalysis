using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemKyrs
{
    class ReversePolishNotation
    {
        // здесь задаются регулярные выражения для разбора строки;
        private Regex rxPattern = new Regex(@"\!|\(|\)|\&|\+|\!|([0-1])|([a-zA-z][a-zA-z0-9_]*)"); // регэксп для всех операторов и операндов;
        private Regex rxIdentifiers = new Regex(@"[a-zA-z][a-zA-z0-9_]*"); // регэксп для операндов: A, B, X, X1, X100 etc;
        private Regex rxNumbers = new Regex(@"[0-1]"); // регэксп для 0 и 1;
        private Regex rxBrackets = new Regex(@"\(|\)"); // регэксп для круглых скобок;
        private Regex rxOperations = new Regex(@"\!|\&|\+|"); // регэксп для операций: !, &, +; 
        private List<string> identifiers = new List<string>();  // лист из уникальных идентификаторов;
        private List<string> neededIDs = new List<string>(); // лист только из истинных идентификаторов;
        private List<string> expression = new List<string>(); // лист для хранения ОПН;
        private Stack stOperations = new Stack(); // стек для хранения операций;

        private string str; // строковое представление ОПН;

        // констуктор
        public ReversePolishNotation()
        {
        }

        // свойство;
        // отдает: строку в ОПН;
        // получает: строку, которую переводит в ОПН;
        public string Expression
        {
            get
            {
                return str; // возврат строки;
            }
            set
            {
                string[] operators = { "!", "(", ")", "&", "+" }; // массив из всех операторов, чем меньше индекс — тем выше приоритет;

                MatchCollection tokens = rxPattern.Matches(value); // разбиваем на токены по главному регэкспу;
                foreach (Match token in tokens)
                {
                    Match currentToken = rxIdentifiers.Match(token.Value);
                    if (currentToken.Success) // если идентификатор, то сохраняем его;
                    {
                        if (!expression.Contains(currentToken.Value)) // если такого еще не было, то сохраняем так же как уникальный;
                        {
                            identifiers.Add(currentToken.Value);
                        }
                        expression.Add(currentToken.Value);

                        continue;
                    }

                    currentToken = rxNumbers.Match(token.Value);
                    if (currentToken.Success) // если токен 0 или 1, то сохраняем его;
                    {
                        expression.Add(currentToken.Value);
                        continue;
                    }

                    currentToken = rxBrackets.Match(token.Value);
                    if (currentToken.Success) // если токен скобка..
                    {
                        if (currentToken.Value == "(") // ..открывающая? кладем в стек;
                        {
                            stOperations.Push(currentToken.Value);
                            continue;
                        }

                        string op;
                        while ((op = stOperations.Pop().ToString()) != "(") // ..закрывающая? выталкиваем из стека все в лист-выражение, пока не встретим открывающую;
                        {
                            expression.Add(op);
                        }
                        continue;
                    }

                    currentToken = rxOperations.Match(token.Value);
                    if (currentToken.Success) // если операция..
                    {
                        try
                        {   // ..выталкиваем из стека, пока не встретим открывающую скобку, операцию с приоритетом ниже или конец стека..
                            while ((Array.IndexOf(operators, currentToken.Value) >= Array.IndexOf(operators, stOperations.Peek())) && stOperations.Peek().ToString() != "(")
                            {
                                expression.Add(stOperations.Pop().ToString());
                            }
                        }
                        catch (Exception error) // обрабатываем возможные ошибки..
                        {
                            if (stOperations.Count != 0)  // ..причем возникшие не из-за того, что стек пуст;
                            {
                                Console.WriteLine("Houston, we've had a problem:\n   {0}\n", error.ToString());
                                Console.ReadKey();
                                return;
                            }
                        }
                        stOperations.Push(currentToken.Value); // ..и кладем текущий в стек;
                    }
                }

                while (stOperations.Count != 0) // пока стек не пуст..
                {
                    expression.Add(stOperations.Pop().ToString()); // ..выталкиваем из него все;
                }

                str = "";
                foreach (string element in expression) // сливаем в одну строку;
                {
                    str += element + ' ';
                }
            }
        }

        // метод для ввода значений операндов;
        public void InputValues()
        {
            if (identifiers.Count > 0)
            {
                Console.WriteLine("\nGood boy. Now say me, [t]rue or [f]alse these identifiers:");

                string[] trues = { "t", "true", "1" }; // различные представления истинного значения';
                string[] falses = { "f", "false", "0" }; // разлличные представления ложного значения;

                foreach (string ID in identifiers)
                {
                Start:
                    Console.Write("\t{0}: ", ID);
                    string value = Console.ReadLine().ToLower().Trim(); // ввод значения очередного идентификатора;
                    if (trues.Contains(value))
                    {
                        neededIDs.Add(ID); // если он истинн, то мы его сохраняем;
                    }
                    else if (falses.Contains(value))
                    {
                        continue; // если нет, выходим из текущей итерации;
                    }
                    else
                    {
                        Console.WriteLine("What the hell are you doing? Try again."); // в противном случае, просим еще раз ввести;
                        goto Start;
                    }
                }
            }
        }

        // метод для вывода результата
        public string Result()
        {
            bool b1, b2;
            foreach (string element in expression)
            {

                try
                {
                    if (rxIdentifiers.Match(element).Success) // присваиваем идентификатору его значение;
                    {
                        stOperations.Push(neededIDs.Contains(element) ? "true" : "false");
                    }
                    else if (rxNumbers.Match(element).Success) // интерпетируем 0 и 1;
                    {
                        stOperations.Push((element == "0") ? "false" : "true");
                    }
                    else if (element == "!") // инвертируем значение, если встретилось отрицание;
                    {
                        b1 = Convert.ToBoolean(stOperations.Pop());
                        stOperations.Push(b1 ? "false" : "true");
                    }
                    else if (element == "&") // логическое умножение;
                    {
                        b1 = Convert.ToBoolean(stOperations.Pop());
                        b2 = Convert.ToBoolean(stOperations.Pop());
                        stOperations.Push((b1 && b2) ? "true" : "false");
                    }
                    else if (element == "+") // логическое сложение;
                    {
                        b1 = Convert.ToBoolean(stOperations.Pop());
                        b2 = Convert.ToBoolean(stOperations.Pop());
                        stOperations.Push((b1 || b2) ? "true" : "false");
                    }
                }

                catch (Exception error) // обработка ошибок;
                {
                    if (stOperations.Count != 0)
                    {
                        return "Houston, we've had a problem:\n   {0}\n" + error.ToString();
                    }
                    else
                    {
                        return "What the hell, Houston? Stack is empty!";
                    }
                }
            }

            try
            {
                return stOperations.Pop().ToString(); // возвращаем результат;
            }
            catch (Exception error) // отлавливаем ошибки;
            {
                if (stOperations.Count != 0)
                {
                    return "Houston, we've had a problem:\n   {0}\n" + error.ToString();
                }
                else
                {
                    return "What the hell, Houston? Stack is empty!";
                }
            }
        }
    }
}
