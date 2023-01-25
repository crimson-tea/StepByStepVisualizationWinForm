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

internal class Operation
{
    public OperationType OperationType;
    public int From;
    public int To;
    public Operation(OperationType operationType, int from, int to)
    {
        OperationType = operationType;
        From = from;
        To = to;
    }

    public void Deconstruct(out OperationType operationType, out int from, out int to) => (operationType, from, to) = (OperationType, From, To);
    public override string ToString()
    {
        return $"{OperationType} x: {From}";
    }
}
