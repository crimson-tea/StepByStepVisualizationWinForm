namespace AnimationWinForm.Control1;

internal class Model
{
    private Random _rand = new Random(1000000001);
    public IEnumerator<Operation> Move(int n)
    {
        int prev = 0;
        for (int i = 1; i <= n; i++)
        {
            int next = _rand.Next(100);
            yield return new Operation(OperationType.Move, prev, next);
            prev = next;
        }
    }
}

internal enum OperationType { None, Move }

internal record Operation(OperationType OperationType, int From , int To);
