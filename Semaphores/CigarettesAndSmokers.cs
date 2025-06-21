using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semaphores.Program;

namespace Semaphores;

public partial class Program
{
    public static void CigarettesAndSmokers()
    {
        Thread[] threads = [
            new Thread(() =>
            {
                AgentA thread = new(0);
                thread.Imitate();
            }),
            new Thread(() =>
            {
                AgentB thread = new(0);
                thread.Imitate();
            }),
            new Thread(() =>
            {
                AgentC thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                SmokerWithTobacco thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                SmokerWithPaper thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                SmokerWithMatches thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                PaperPusher thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                MatchPusher thread = new(0);
                thread.Imitate();
            }),
            new Thread(() => {
                TobaccoPusher thread = new(0);
                thread.Imitate();
            }),
        ];

        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public abstract class Agent(int number) : MockThread(number)
    {
        public readonly string[] INGREDIENTS_TO_PROVIDE = ["tobacco", "paper", "matches"];
        public static Semaphore agentSem = new(1, int.MaxValue);
        public static Semaphore tobaccoOnTable = new(0, int.MaxValue);
        public static Semaphore paperOnTable = new(0, int.MaxValue);
        public static Semaphore matchOnTable = new(0, int.MaxValue);

        // public override void Prepare()
        // {
        //     for (int i = 0; i < THREADS_NUMBER; i++)
        //     {

        //     }
        // }

        // public override void ImitateIteration()
        // {
        //     Thread.Sleep(100);
        //     if (Random.Shared.NextDouble() > 0.4)
        //     {
        //         Provide2Elements();
        //     }
        // }

        // public void Provide2Elements()
        // {
        //     #region
        //     // int ingredientToNotProvide = Random.Shared.Next(3);
        //     // List<string> ingredients = new(2);
        //     // for (int i = 0; i < 3; i++)
        //     // {
        //     //     if (i != ingredientToNotProvide)
        //     //     {
        //     //         ingredients.Append(INGREDIENTS_TO_PROVIDE[i]);
        //     //     }
        //     // }
        //     #endregion
        //     int ingredientToNotProvide = Random.Shared.Next(3);
        //     for (int i = 0; i < 3; i++)
        //     {
        //         if (i != ingredientToNotProvide)
        //         {
        //             switch (INGREDIENTS_TO_PROVIDE[i])
        //             {
        //                 case "tobacco":
        //                     tobacco.Release();
        //                     break;
        //                 case "paper":
        //                     paper.Release();
        //                     break;
        //                 case "match":
        //                     match.Release();
        //                     break;
        //             }
        //         }
        //     }
        // }
    }

    public class AgentA(int number) : Agent(number)
    {
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            agentSem.WaitOne();
            Console.WriteLine("tobacco and paper supply");
            tobaccoOnTable.Release();
            paperOnTable.Release();
        }
    }

    public class AgentB(int number) : Agent(number)
    {
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            agentSem.WaitOne();
            Console.WriteLine("paper and matches supply");
            paperOnTable.Release();
            matchOnTable.Release();
        }
    }

    public class AgentC(int number) : Agent(number)
    {
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            agentSem.WaitOne();
            Console.WriteLine("tobacco and matches supply");
            tobaccoOnTable.Release();
            matchOnTable.Release();
        }
    }

    public class SmokerWithTobacco(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            sem.WaitOne();

            // Console.WriteLine("Smoker with tobacco gonna take paper");
            // Agent.paperOnTable.WaitOne();
            // Console.WriteLine("Smoker with tobacco took paper");

            // Console.WriteLine("Smoker with tobacco gonna take match");
            // Agent.matchOnTable.WaitOne();
            // Console.WriteLine("Smoker with tobacco took match");

            Console.WriteLine("Smoker with tobacco made cigarette");
            Agent.agentSem.Release();
        }
    }
    public class SmokerWithPaper(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            sem.WaitOne();

            // Console.WriteLine("Smoker with paper gonna take match");
            // Agent.matchOnTable.WaitOne();
            // Console.WriteLine("Smoker with paper took match");

            // Console.WriteLine("Smoker with paper gonna take tobacco");
            // Agent.tobaccoOnTable.WaitOne();
            // Console.WriteLine("Smoker with paper took tobacco");

            Console.WriteLine("Smoker with paper made cigarette");
            Agent.agentSem.Release();
        }
    }

    public class SmokerWithMatches(int number) : MockThread(number)
    {
        public static Semaphore sem = new(0, int.MaxValue);
        public override void ImitateIteration()
        {
            Thread.Sleep(200);
            sem.WaitOne();

            // Console.WriteLine("Smoker with matches gonna take tobacco");
            // Agent.tobaccoOnTable.WaitOne();
            // Console.WriteLine("Smoker with matches took tobacco");

            // Console.WriteLine("Smoker with matches gonna take paper");
            // Agent.paperOnTable.WaitOne();
            // Console.WriteLine("Smoker with matches took paper");

            Console.WriteLine("Smoker with matches made cigarette");
            Agent.agentSem.Release();
        }
    }
}

public class PaperPusher(int number) : MockThread(number)
{
    public static bool isPaper = false;
    public override void ImitateIteration()
    {
        Agent.paperOnTable.WaitOne();

        mutex.WaitOne();
        if (MatchPusher.isMatch)
        {
            MatchPusher.isMatch = false;
            SmokerWithTobacco.sem.Release();
        }
        else if (TobaccoPusher.isTobacco)
        {
            TobaccoPusher.isTobacco = false;
            SmokerWithMatches.sem.Release();
        }
        else
        {
            isPaper = true;
        }
        mutex.Release();
    }
}

public class MatchPusher(int number) : MockThread(number)
{
    public static bool isMatch = false;
    public override void ImitateIteration()
    {
        Agent.matchOnTable.WaitOne();

        mutex.WaitOne();
        if (PaperPusher.isPaper)
        {
            PaperPusher.isPaper = false;
            SmokerWithTobacco.sem.Release();
        }
        else if (TobaccoPusher.isTobacco)
        {
            TobaccoPusher.isTobacco = false;
            SmokerWithPaper.sem.Release();
        }
        else
        {
            isMatch = true;
        }

        mutex.Release();
    }
}

public class TobaccoPusher(int number) : MockThread(number)
{
    public static bool isTobacco = false;
    public override void ImitateIteration()
    {
        Agent.tobaccoOnTable.WaitOne();

        mutex.WaitOne();
        if (PaperPusher.isPaper)
        {
            PaperPusher.isPaper = false;
            SmokerWithMatches.sem.Release();
        }
        else if (MatchPusher.isMatch)
        {
            MatchPusher.isMatch = false;
            SmokerWithPaper.sem.Release();
        }
        else
        {
            isTobacco = true;
        }
        mutex.Release();
    }
}
