using System.Text;
using System.Collections.Generic;

namespace Simulation.Utilities;

public static class RomanNumeralConverter
{
    private static readonly Dictionary<int, string> RomanMap = new Dictionary<int, string>
    {
        { 1000, "M" }, { 900, "CM" }, { 500, "D" }, { 400, "CD" }, { 100, "C" },
        { 90, "XC" }, { 50, "L" }, { 40, "XL" }, { 10, "X" },
        { 9, "IX" }, { 5, "V" }, { 4, "IV" }, { 1, "I" }
    };

    /// <summary>
    /// Преобразует целое арабское число в его римское представление.
    /// </summary>
    /// <param name="number">Целое число от 1 до 3999.</param>
    /// <returns>Строка с римским числом или пустая строка, если число некорректно.</returns>
    public static string ToRoman(int number)
    {
        if (number < 1 || number > 3999)
        {
            return string.Empty;
        }
        var romanResult = new StringBuilder();

        foreach (var pair in RomanMap)
        {
            while (number >= pair.Key)
            {
                romanResult.Append(pair.Value);
                number -= pair.Key;
            }
        }

        return romanResult.ToString();
    }
}