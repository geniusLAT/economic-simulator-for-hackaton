using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Utilities;

public class TableDrawer
{
    private readonly List<List<string>> _list;

    public TableDrawer() 
    {
        _list = new List<List<string>>();
    }

    public void AddLine(List<string> line)
    {
        _list.Add(line);
    }

    public string Draw(bool separator=false)
    {
        var min = _list[0].Count;
        Console.WriteLine($"first line contains {min} elements");
        Console.WriteLine($"there are {_list.Count} lines\n\n");

        int[] maxCharsInColumns = new int[min];
        for (int elementInLineIndex = 0; elementInLineIndex < min; elementInLineIndex++)
        {
            int maxCharsInColumn = 0;
            Console.Write($" Elements #{elementInLineIndex} of each line \n");
            for (int rowIndex = 0; rowIndex < _list.Count; rowIndex++)
            {
                Console.Write($" {rowIndex} ");
                Console.Write($" {_list[rowIndex].Count} ");
                Console.Write($" {_list[rowIndex][elementInLineIndex]} \n");
                if (maxCharsInColumn < _list[rowIndex][elementInLineIndex].Length)
                {
                    maxCharsInColumn = _list[rowIndex][elementInLineIndex].Length;
                }
            }
            Console.Write($"maxCharsInColumn = {maxCharsInColumn} \n\n");
            maxCharsInColumns[elementInLineIndex] = maxCharsInColumn;
        }
        Console.Write($" \n\n\n");
        StringBuilder sb = new();

        for (int rowIndex = 0; rowIndex < _list.Count; rowIndex++)
        {
            sb.Append('|');
            for (int elementInLineIndex = 0; elementInLineIndex < min; elementInLineIndex++)
            {
                Console.Write($"\n {rowIndex} ");
                Console.Write($" {_list[rowIndex].Count} ");
                Console.Write($" {_list[rowIndex][elementInLineIndex]} ");

                sb.Append(_list[rowIndex][elementInLineIndex]);
                var extraSpaces = maxCharsInColumns[elementInLineIndex] - _list[rowIndex][elementInLineIndex].Length;
                Console.Write($" maxCharsInColumns[{elementInLineIndex}]  = {maxCharsInColumns[elementInLineIndex]} ");
                Console.Write($" _list[rowIndex][elementInLineIndex].Length  = {_list[rowIndex][elementInLineIndex].Length} ");
                Console.Write($" extraSpaces  = {extraSpaces} ");
                sb.Append(new string(' ', extraSpaces));
                sb.Append('|');
            }
            sb.Append('\n');

            if(separator && rowIndex == 0)
            {
                sb.Append('|');
                for (int elementInLineIndex = 0; elementInLineIndex < min; elementInLineIndex++)
                {
                    sb.Append(new string('-', maxCharsInColumns[elementInLineIndex]));
                    sb.Append('|');
                }
                    sb.Append('\n');
            }
        }
        Console.Write($" \n\n");
        return sb.ToString();
    }
}
