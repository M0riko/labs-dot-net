using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        List<string> history = new List<string>();
        bool isRunning = true;

        while (isRunning)
        {
            ShowMenu();
            int choice = ReadMenuChoice();

            switch (choice)
            {
                case 1:
                    HandleCalculation(history);
                    break;
                case 2:
                    ShowHistory(history);
                    break;
                case 0:
                    isRunning = false;
                    Console.WriteLine("Роботу програми завершено.");
                    break;
            }
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("\n===== КАЛЬКУЛЯТОР ВИРАЗІВ =====");
        Console.WriteLine("1. Обчислити вираз");
        Console.WriteLine("2. Показати історію обчислень");
        Console.WriteLine("0. Вихід");
    }

    static int ReadMenuChoice()
    {
        while (true)
        {
            Console.Write("Ваш вибір: ");
            string? input = Console.ReadLine();

            if (input == null)
                return 0;

            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 2)
                return choice;

            Console.WriteLine("Помилка: введіть 0, 1 або 2.");
        }
    }

    static void HandleCalculation(List<string> history)
    {
        Console.Write("Введіть вираз (наприклад, 12 + 7): ");
        string input = Console.ReadLine() ?? "";

        if (!TryParseExpression(input, out double first, out char operation,
                out double second, out string error))
        {
            Console.WriteLine($"Помилка: {error}");
            return;
        }

        if (!TryCalculate(first, operation, second, out double result))
        {
            Console.WriteLine("Помилка: ділення на нуль заборонено.");
            return;
        }

        if (!double.IsFinite(result))
        {
            Console.WriteLine("Помилка: результат виходить за межі типу double.");
            return;
        }

        string record = $"{FormatNumber(first)} {operation} " +
                        $"{FormatNumber(second)} = {FormatNumber(result)}";
        Console.WriteLine($"Результат: {record}");
        history.Add(record);
    }

    static bool TryParseExpression(string input, out double first,
        out char operation, out double second, out string error)
    {
        first = 0;
        operation = '\0';
        second = 0;
        error = "";

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
        {
            error = "формат виразу має бути «число оператор число», наприклад 12 + 7.";
            return false;
        }

        if (!TryParseNumber(parts[0], out first) ||
            !TryParseNumber(parts[2], out second))
        {
            error = "операнди мають бути числами.";
            return false;
        }

        if (parts[1].Length != 1 || !"+-*/".Contains(parts[1]))
        {
            error = "непідтримуваний оператор. Доступні +, -, *, /.";
            return false;
        }

        operation = parts[1][0];
        return true;
    }

    static bool TryParseNumber(string input, out double number)
    {
        string normalized = input.Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float,
                   CultureInfo.InvariantCulture, out number) && double.IsFinite(number);
    }

    static bool TryCalculate(double first, char operation, double second,
        out double result)
    {
        result = 0;
        switch (operation)
        {
            case '+':
                result = first + second;
                return true;
            case '-':
                result = first - second;
                return true;
            case '*':
                result = first * second;
                return true;
            case '/':
                if (second == 0)
                    return false;
                result = first / second;
                return true;
            default:
                return false;
        }
    }

    static string FormatNumber(double number)
    {
        return number.ToString("G", CultureInfo.InvariantCulture);
    }

    static void ShowHistory(List<string> history)
    {
        Console.WriteLine("\n===== ІСТОРІЯ ОБЧИСЛЕНЬ =====");
        if (history.Count == 0)
        {
            Console.WriteLine("Історія порожня. Обчислень ще не було.");
            return;
        }

        for (int i = 0; i < history.Count; i++)
            Console.WriteLine($"{i + 1}. {history[i]}");
    }
}
