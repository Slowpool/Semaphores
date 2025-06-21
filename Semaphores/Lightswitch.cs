using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static Lightswitch lightswitch = new();
    public static Semaphore turnstile = new(1, int.MaxValue);
    public static Semaphore writerToken = new(1, int.MaxValue);
    public static void ReadersAndWriters3()
    {
        Thread[] readerThreads = new Thread[NUMBER_OF_READERS];
        for (int i = 0; i < readerThreads.Length; i++)
        {
            int ownI = i;
            readerThreads[i] = new Thread(() =>
            {
                Reader3 reader = new(ownI);
                reader.Imitate();
            });
        }

        Thread[] writerThreads = new Thread[2];
        for (int i = 0; i < writerThreads.Length; i++)
        {
            int ownI = i;
            writerThreads[i] = new Thread(() =>
            {
                Writer3 writer = new(ownI);
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
    public class Reader3(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.5)
                {
                    Console.WriteLine($"reader {number} waits for reading...");
                    turnstile.WaitOne();
                    turnstile.Release();
                    Console.WriteLine($"reader {number} passed the turnstile...");

                    lightswitch.Lock(roomEmpty);
                    Console.WriteLine($"reader {number} reads: {Database}");
                    lightswitch.Unlock(roomEmpty);
                }
                Thread.Sleep(1000);
            }
        }
    }
    public class Writer3(int number)
    {
        public void Imitate()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.7)
                {
                    turnstile.WaitOne();
                    Console.WriteLine("turnstile is closed");

                    Thread.Sleep(3000);
                    roomEmpty.WaitOne();

                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine($"writer {number} writes...");
                        Thread.Sleep(1000);
                    }
                    Database += number;

                    roomEmpty.Release();
                    turnstile.Release();
                }
                Thread.Sleep(100);
            }
        }
    }
    public class Lightswitch()
    {
        private int counter = 0;
        public Semaphore mutex = new(1, int.MaxValue);
        public void Lock(Semaphore roomEmpty)
        {
            mutex.WaitOne();
            counter += 1;
            if (counter == 1)
                roomEmpty.WaitOne();
            mutex.Release();
        }
        public void Lock(Semaphore roomEmpty, string roomName)
        {
            mutex.WaitOne();
            counter += 1;
            if (counter == 1)
            {
                roomEmpty.WaitOne();
                // Console.WriteLine($"{roomName} light is on");
            }
            else {
                // Console.WriteLine($"{roomName} light is already on");
            }
            mutex.Release();
        }
        public void Unlock(Semaphore roomEmpty)
        {
            mutex.WaitOne();
            counter -= 1;
            if (counter == 0)
                roomEmpty.Release();
            mutex.Release();
        }
        public void Unlock(Semaphore roomEmpty, string roomName)
        {
            mutex.WaitOne();
            counter -= 1;
            if (counter == 0)
            {
                // Console.WriteLine($"{roomName} light is gonna be off");
                roomEmpty.Release();
                // Console.WriteLine($"{roomName} light is off");
            }
            else {
                // Console.WriteLine($"{roomName} is already on");
            }
            mutex.Release();
        }
    }
}
