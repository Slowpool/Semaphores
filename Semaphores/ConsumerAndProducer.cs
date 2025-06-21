using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;
public partial class Program
{
    public static readonly Stack<string> Buffer = new Stack<string>();
    private static Semaphore Items = new(0, int.MaxValue);
    private const int NUMBER_OF_CONSUMERS = 3;
    private const int NUMBER_OF_PRODUCERS = 3;
    private const int BUFFER_MAX = 3;
    private static Semaphore BufferMax = new(BUFFER_MAX, int.MaxValue);
    public static void ProducerAndConsumer()
    {
        Thread[] consumers = new Thread[NUMBER_OF_CONSUMERS];
        for (int i = 0; i < consumers.Length; i++)
        {
            int ownI = i;
            consumers[i] = new Thread(() =>
            {
                Consumer consumer = new(ownI);
                consumer.HandleEvent();
            });
        }

        Thread[] producers = new Thread[NUMBER_OF_PRODUCERS];
        for (int i = 0; i < producers.Length; i++)
        {
            string ownEvent = $"EVENT FROM {i}";
            producers[i] = new Thread(() =>
            {
                Producer producer = new(ownEvent);
                producer.GenerateEventSometimes();
            });
        }

        foreach (var consumer in consumers)
        {
            consumer.Start();
        }

        foreach (var producer in producers)
        {
            producer.Start();
        }
    }
    public class Producer(string @event)
    {
        public void GenerateEvent()
        {
            // Console.WriteLine($"Producer generated event: \"{@event}\"");
            Console.WriteLine(@event);

            BufferMax.WaitOne();

            mutex.WaitOne();
            Buffer.Push(@event);
            mutex.Release();
            Items.Release();
        }

        public void GenerateEventSometimes()
        {
            while (true)
            {
                if (Random.Shared.NextSingle() > 0.5F)
                    this.GenerateEvent();
                // Thread.Sleep(Random.Shared.Next(5) * 1000);
                Thread.Sleep(1000);
            }
        }
    }
    public class Consumer(int number)
    {
        public void HandleEvent()
        {
            while (true)
            {
                Items.WaitOne();
                mutex.WaitOne();
                string @event = Buffer.Pop();
                Console.WriteLine($"Consumer {number} got new event: \"{@event}\"");
                mutex.Release();
                BufferMax.Release();
                int i = 0;
                while (i < 5)
                {
                    Console.WriteLine($"Consumer {number} handles {@event}...");
                    Console.WriteLine($"Buffer size: {Buffer.Count}");
                    Thread.Sleep(1000);
                }
                // Console.WriteLine("/a-list-of-news?tags=/");
            }
        }
    }
}
