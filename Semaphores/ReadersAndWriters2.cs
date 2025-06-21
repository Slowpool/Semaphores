using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static Semaphore roomEmpty = new(1, int.MaxValue);
    public static int readers = 0;

    public static void ReadersAndWriters2()
    {
        Thread[] readerThreads = new Thread[NUMBER_OF_READERS];
        for (int i = 0; i < readerThreads.Length; i++)
        {
            int ownI = i;
            readerThreads[i] = new Thread(() =>
            {
                Reader2 reader = new(ownI);
                reader.Imitate();
            });
        }

        Thread[] writerThreads = new Thread[1];
        for (int i = 0; i < writerThreads.Length; i++)
        {
            int ownI = i;
            writerThreads[i] = new Thread(() =>
            {
                Writer2 writer = new(ownI);
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
    public class Reader2(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.5)
                {
                    mutex.WaitOne();
                    readers += 1;
                    if (readers == 1)
                        roomEmpty.WaitOne();
                    mutex.Release();

                    Console.WriteLine($"reader {number} reads: {Database}");

                    mutex.WaitOne();
                    readers -= 1;
                    if (readers == 0)
                        roomEmpty.Release();
                    mutex.Release();
                }
                Thread.Sleep(1000);
            }
        }
    }
    public class Writer2(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.7)
                {
                    roomEmpty.WaitOne();
                    
                    Console.WriteLine($"writer {number} writes...");
                    Database += number;

                    roomEmpty.Release();
                }
                Thread.Sleep(1000);
            }
        }
    }
}
