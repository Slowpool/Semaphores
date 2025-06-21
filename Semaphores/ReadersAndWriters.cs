using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public const int NUMBER_OF_READERS = 20;
    // public static Semaphore writingToken = new(1, int.MaxValue);
    // public static Semaphore readers = new();
    public static Semaphore readingTokens = new(NUMBER_OF_READERS, int.MaxValue);
    public static string Database = "";

    public static void ReadersAndWriters()
    {
        Thread[] readerThreads = new Thread[NUMBER_OF_READERS];
        for (int i = 0; i < readerThreads.Length; i++)
        {
            int ownI = i;
            readerThreads[i] = new Thread(() =>
            {
                Reader reader = new(ownI);
                reader.Imitate();
            });
        }

        Thread[] writerThreads = new Thread[1];
        for (int i = 0; i < writerThreads.Length; i++)
        {
            int ownI = i;
            writerThreads[i] = new Thread(() =>
            {
                Writer writer = new(ownI);
                writer.Imitate();
            });
        }

        foreach (var reader in readerThreads)
        {
            reader.Start();
        }

        foreach (var writer in writerThreads)
        {
            writer.Start();
        }
    }
    public class Reader(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.5)
                {
                    readingTokens.WaitOne();
                    Console.WriteLine($"reader {number} reads: {Database}");
                    readingTokens.Release();
                }
                Thread.Sleep(1000);
            }
        }
    }
    public class Writer(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.7)
                {
                    mutex.WaitOne();
                    // block all readers
                    for (int i = 0; i < NUMBER_OF_READERS; i++)
                    {
                        readingTokens.WaitOne();
                        Console.WriteLine($"{i}th reader was locked");
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine($"writer {number} writes...");
                    Database += number;

                    readingTokens.Release(NUMBER_OF_READERS);
                    mutex.Release();
                }
                Thread.Sleep(1000);
            }
        }
    }
}
