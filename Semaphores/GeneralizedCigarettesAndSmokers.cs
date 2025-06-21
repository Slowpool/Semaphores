using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public const int QUOTA = 2;
    public static string CurrentState
    {
        get
        {
            mutex.WaitOne();
            var result = "\r\n\tpaper\tmatches\ttobacco";
            result += String.Format("\r\ntable\t{0}\t{1}\t{2}", GPaperPusher.numberOfPaper, GMatchPusher.numberOfMatch, GTobaccoPusher.numberOfTobacco);
            result += String.Format("\r\nmade by\t{0}\t{1}\t{2}", GSmokerWithPaper.cigarettesMade, GSmokerWithMatches.cigarettesMade, GSmokerWithTobacco.cigarettesMade);
            mutex.Release();
            return result;
        }
    }

    public static void GeneralizedCigarettesAndSmokers()
    {
        Thread[] threads = [
            new Thread(() =>
            {
                GAgentA thread = new(0);
                thread.Imitate();
            }),
            new Thread(() =>
            {
                GAgentB thread = new(0);
                thread.Imitate();
            }),
            new Thread(() =>
            {
                GAgentC thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                GSmokerWithTobacco thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                GSmokerWithPaper thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                GSmokerWithMatches thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                GPaperPusher thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                GMatchPusher thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                GTobaccoPusher thread = new(0);
                thread.Imitate();
            }),
        ];

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public abstract class GAgent(int number) : MockThread(number)
    {
        public static Semaphore tobaccoOnTable = new(0, int.MaxValue);
        public static Semaphore paperOnTable = new(0, int.MaxValue);
        public static Semaphore matchOnTable = new(0, int.MaxValue);
    }

    public class GAgentA(int number) : GAgent(number)
    {
        public static int quota = QUOTA;
        public override void ImitateIteration()
        {
            mutex.WaitOne();
            if (quota-- == 0)
            {
                mutex.Release();
                Console.WriteLine("Agent A finished");
                Thread.Sleep(int.MaxValue);
            }
            else
            {
                mutex.Release();
                Thread.Sleep(200);
                tobaccoOnTable.Release();
                Console.WriteLine(CurrentState + "\r\n+tobacco ");
                paperOnTable.Release();
                Console.WriteLine(CurrentState + "\r\n+paper ");
            }
        }
    }

    public class GAgentB(int number) : GAgent(number)
    {
        public static int quota = QUOTA;
        public override void ImitateIteration()
        {
            mutex.WaitOne();
            if (quota-- == 0)
            {
                mutex.Release();
                Console.WriteLine("Agent B finished");
                Thread.Sleep(int.MaxValue);
            }
            else
            {
                mutex.Release();
                Thread.Sleep(200);
                paperOnTable.Release();
                Console.WriteLine(CurrentState + "\r\n+paper ");
                matchOnTable.Release();
                Console.WriteLine(CurrentState + "\r\n+match ");
            }
        }
    }

    public class GAgentC(int number) : GAgent(number)
    {
        public static int quota = QUOTA;
        public override void ImitateIteration()
        {
            mutex.WaitOne();
            if (quota-- == 0)
            {
                mutex.Release();
                Console.WriteLine("Agent C finished");
                Thread.Sleep(int.MaxValue);
            }
            else
            {
                mutex.Release();
                Thread.Sleep(200);
                tobaccoOnTable.Release();
                Console.WriteLine(CurrentState + "\r\n+tobacco ");
                matchOnTable.Release();
                Console.WriteLine(CurrentState + "\r\n+match ");
            }
        }
    }

    public class GSmokerWithTobacco(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public static int cigarettesMade = 0;
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            sem.WaitOne();
            
            mutex.WaitOne();
            cigarettesMade++;
            mutex.Release();
            
            Console.WriteLine(CurrentState + "\r\nSmoker with tobacco made cigarette.");
        }
    }
    public class GSmokerWithPaper(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public static int cigarettesMade = 0;
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            sem.WaitOne();

            mutex.WaitOne();
            cigarettesMade++;
            mutex.Release();
            
            Console.WriteLine(CurrentState + "\r\nSmoker with paper made cigarette");
        }
    }

    public class GSmokerWithMatches(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public static int cigarettesMade = 0;
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            sem.WaitOne();

            mutex.WaitOne();
            cigarettesMade++;
            mutex.Release();

            Console.WriteLine(CurrentState + "\r\nSmoker with matches made cigarette.");
        }
    }
}

public class GPaperPusher(int number) : MockThread(number)
{
    public static int numberOfPaper = 0;
    public override void ImitateIteration()
    {
        GAgent.paperOnTable.WaitOne();

        mutex.WaitOne();
        if (GMatchPusher.numberOfMatch > 0)
        {
            GMatchPusher.numberOfMatch--;
            GSmokerWithTobacco.sem.Release();
        }
        else if (GTobaccoPusher.numberOfTobacco > 0)
        {
            GTobaccoPusher.numberOfTobacco--;
            GSmokerWithMatches.sem.Release();
        }
        else
        {
            numberOfPaper++;
        }
        mutex.Release();
    }
}

public class GMatchPusher(int number) : MockThread(number)
{
    public static int numberOfMatch = 0;
    public override void ImitateIteration()
    {
        GAgent.matchOnTable.WaitOne();

        mutex.WaitOne();
        if (GTobaccoPusher.numberOfTobacco > 0)
        {
            GTobaccoPusher.numberOfTobacco--;
            GSmokerWithPaper.sem.Release();
        }
        else if (GPaperPusher.numberOfPaper > 0)
        {
            GPaperPusher.numberOfPaper--;
            GSmokerWithTobacco.sem.Release();
        }
        else
        {
            numberOfMatch++;
        }
        mutex.Release();
    }
}

public class GTobaccoPusher(int number) : MockThread(number)
{
    public static int numberOfTobacco = 0;
    public override void ImitateIteration()
    {
        GAgent.tobaccoOnTable.WaitOne();
        mutex.WaitOne();
        if (GPaperPusher.numberOfPaper > 0)
        {
            GPaperPusher.numberOfPaper--;
            GSmokerWithMatches.sem.Release();
        }
        else if (GMatchPusher.numberOfMatch > 0)
        {
            GMatchPusher.numberOfMatch--;
            GSmokerWithPaper.sem.Release();
        }
        else
        {
            numberOfTobacco++;
        }
        mutex.Release();
    }
}
