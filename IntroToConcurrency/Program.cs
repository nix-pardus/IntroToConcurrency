using System.Diagnostics;

namespace IntroToConcurrency;

internal class Program
{
    static async Task Main(string[] args)
    {
        var directory = Environment.CurrentDirectory + "/Texts";

        //Синхронный блок кода
        Stopwatch sw2 = new Stopwatch();
        sw2.Start();
        var count = GetSpacesCountInDirectory(directory);
        sw2.Stop();
        Console.WriteLine($"{$"Sync: {sw2.Elapsed.Seconds}.{sw2.Elapsed.Milliseconds} с", 20} {$"{count} пробелов", 20}");

        //Асинхронный блок кода
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var countAsync = await GetSpacesCountInDirectoryAsync(directory);
        sw.Stop();
        Console.WriteLine($"{$"Async: {sw.Elapsed.Seconds}.{sw.Elapsed.Milliseconds} с", 20} {$"{countAsync} пробелов", 20}");


        Console.ReadKey();
    }


    /// <summary>
    /// Асинхронный метод получения количества пробелов во всех файлах директории
    /// </summary>
    /// <param name="directory">Путь к папке</param>
    /// <returns></returns>
    static async Task<int> GetSpacesCountInDirectoryAsync(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Console.WriteLine("Указанная папка не существует.");
            return 0;
        }
        var files = Directory.GetFiles(directory);
        var tasks = files.Select(GetSpacesCountInFileAsync);

        using var cts = new CancellationTokenSource();

        //Просто анимация загрузки
        var t = Task.Run(async () =>
        {
            Console.Write(" ");
            while (!cts.Token.IsCancellationRequested)
            {
                Console.Write("\b-");
                await Task.Delay(100, cts.Token);
                Console.Write("\b\\");
                await Task.Delay(100, cts.Token);
                Console.Write("\b|");
                await Task.Delay(100, cts.Token);
                Console.Write("\b/");
                await Task.Delay(100, cts.Token);
            }
        }, cts.Token);

        int[] results = await Task.WhenAll(tasks);

        cts.Cancel();
        Console.Write("\b");
        
        return results.Sum();
    }


    /// <summary>
    /// Асинхронный метод подсчета количества пробелов в файле
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <returns></returns>
    static async Task<int> GetSpacesCountInFileAsync(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);

            
            string text = await reader.ReadToEndAsync();
            
            return text.Count(c => c == ' ');
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
            return 0;
        }
    }


    /// <summary>
    /// Метод подсчета количества пробелов в файле (синхонный)
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <returns></returns>
    static int GetSpacesCountInFile(string filePath)
    {
        try
        {
            using var reader = new StreamReader(filePath);
            string text = reader.ReadToEnd();
            return text.Count(c => c == ' ');
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
            return 0;
        }
    }


    /// <summary>
    /// Метод получения количества пробелов во всех файлах директории (синхронный)
    /// </summary>
    /// <param name="directory">Путь к папке</param>
    /// <returns></returns>
    static int GetSpacesCountInDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Console.WriteLine("Указанная папка не существует.");
            return 0;
        }
        var files = Directory.GetFiles(directory);
        var results = files.Select(GetSpacesCountInFile);
        return results.Sum();
    }

}
