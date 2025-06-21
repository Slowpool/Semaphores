namespace Semaphores;

public abstract class MockThread(int number)
{
    protected int number = number;
    public void Imitate()
    {
        Prepare();
        while (true)
        {
            ImitateIteration();
        }
    }
    public abstract void ImitateIteration();
    public virtual void Prepare()
    {

    }
}