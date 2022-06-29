using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FrequencyAnalysisText;

public class Record
{
    public string Triplet = null!;
    public long Count;
}

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
        var result = triplets.OrderByDescending(x => x.Count).Take(10);

        _timer.Stop();

        foreach (var value in result)
            Console.WriteLine($"{value.Triplet} - {value.Count}");
        Console.WriteLine($"Время: {_timer.ElapsedMilliseconds} мс");
    }

    public static ConcurrentBag<Record> ExecuteAnalysis()
    {
        // Заменяем все символы кроме букв пробелами и все переводим в нижний регистор
        var text = Regex.Replace(ReadTextFile(_path!), @"[^A-zА-я]+", " ").ToLower();

        // Создаем список слов
        var words = text.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Where(word => word.Length >= 3)
            .ToList();

        // Разбиваем слова на триплеты и подсчитываем их количество
        ConcurrentBag<Record> triplets = new();
        if (!words.Any()) return triplets;

        Parallel.For(0, words.Count, i =>
        {
            for (int j = 0; j <= words[i].Length - 3; j++)
            {
                var newTriplet = words[i].Substring(j, 3);

                var result = triplets.FirstOrDefault(x => x.Triplet == newTriplet);
                if (result == null)
                    triplets.Add(new Record { Triplet = newTriplet, Count = 1 });
                else
                    result.Count += 1;
            }
        });

        return triplets;
    }

    public static string ReadTextFile(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException();
        return File.ReadAllText(path);
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