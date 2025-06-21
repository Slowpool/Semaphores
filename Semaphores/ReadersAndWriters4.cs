using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static Lightswitch readSwitch = new();
    public static Lightswitch writeSwitch = new();
    public static Semaphore noReaders = new(1, int.MaxValue);
    public static Semaphore noWriters = new(1, int.MaxValue);

    public static void ReadersAndWriters4()
    {
        Thread[] readerThreads = new Thread[3];
        for (int i = 0; i < readerThreads.Length; i++)
        {
            int ownI = i;
            readerThreads[i] = new Thread(() =>
            {
                Reader4 reader = new(ownI);
                reader.Imitate();
            });
        }

        Thread[] writerThreads = new Thread[2];
        for (int i = 0; i < writerThreads.Length; i++)
        {
            int ownI = i;
            writerThreads[i] = new Thread(() =>
            {
                Writer4 writer = new(ownI);
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
    public class Reader4(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.5)
                {
                    Console.WriteLine($"reader {number} waits...");
                    noWriters.WaitOne();
                    Console.WriteLine($"reader {number} don't see writers...");
                    readSwitch.Lock(noReaders, $"readers by {number}");
                    noWriters.Release();
                    Console.WriteLine($"reader {number} locked...");

                    Console.WriteLine($"reader {number} reads: {Database}");

                    readSwitch.Unlock(noReaders, $"readers by {number}");
                    Console.WriteLine($"reader {number} unlocked...");
                }
                Thread.Sleep(1000);
            }
        }
    }
    public class Writer4(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.7)
                {
                    Console.WriteLine($"writer {number} waits...");
                    writeSwitch.Lock(noWriters, $"writers by {number}");
                    Console.WriteLine($"writer {number} locked...");
                    
                    noReaders.WaitOne();
                    Console.WriteLine($"writer {number} don't see readers...");

                    // mutex.WaitOne();

                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine($"writer {number} writes...");
                        Thread.Sleep(1000);
                    }
                    Database += number;

                    noReaders.Release();
                    // mutex.Release();

                    writeSwitch.Unlock(noWriters, $"writers by {number}");
                    Console.WriteLine($"writer {number} unlocked...");
                }
                Thread.Sleep(1000);
            }
        }
    }
}
