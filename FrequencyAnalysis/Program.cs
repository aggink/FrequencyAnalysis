using System.Collections.Concurrent;
using System.Diagnostics;

namespace FrequencyAnalysisText;

public class Program
{
    private static Stopwatch _timer = new();
    private static string? _path;

    //(3 идущих подряд буквы слова)
    public static void Main()
    {
        ReadPathConsole();

        _timer.Start();

        var triplets = ExecuteAnalysis();
        var result = triplets.OrderByDescending(x => x.Value).Take(10);

        _timer.Stop();

        foreach (var value in result)
            Console.WriteLine($"{value.Key} - {value.Value}");
        Console.WriteLine($"Время: {_timer.ElapsedMilliseconds} мс");
    }

    public static ConcurrentDictionary<string, int> ExecuteAnalysis()
    {
        ConcurrentDictionary<string, int> triplets = new();

        Parallel.ForEach(ReadTextFile(_path!), line =>
        {
            List<char> triplet = new();
            foreach (char c in line)
            {
                if (!char.IsLetter(c))
                {
                    triplet.Clear();
                    continue;
                }

                triplet.Add(c);

                if (triplet.Count == 3)
                {
                    triplets.AddOrUpdate(new string(triplet.ToArray()), 1, (triplet, count) => count = count + 1);
                    triplet.RemoveAt(0);
                }
            }
        });

        return triplets;
    }

    public static string[] ReadTextFile(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException();
        return File.ReadAllLines(path);
    }

    public static void ReadPathConsole()
    {
        while (true)
        {
            Console.Write("Please, enter the path to the file:");
            _path = Console.ReadLine();
            if (!File.Exists(_path))
                Console.WriteLine("The file was not found at the given path.");
            else
            {
                Console.WriteLine("Thank you. The result will be soon!\n");
                break;
            }
        }
    }
}