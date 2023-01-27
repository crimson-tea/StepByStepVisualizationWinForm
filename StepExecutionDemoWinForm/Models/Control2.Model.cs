namespace AnimationWinForm.Control2;

internal class Model
{
    public IEnumerator<Operation> BinarySearch(int[] values, int target)
    {
        int l = -1;
        int r = values.Length;

        while (Math.Abs(l - r) > 1)
        {
            int mid = (l + r) / 2;
            if (values[mid] < target)
            {
                yield return new Operation(OperationType.MoveLeft, l, mid);
                l = mid;
            }
            else
            {
                yield return new Operation(OperationType.MoveRight, r, mid);
                r = mid;
            }
        }

        yield return new Operation(OperationType.Complete, r, r);
    }
}

internal enum OperationType { None, Complete, MoveLeft, MoveRight }

internal class Operation
{
    public OperationType Type { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public Operation(OperationType type, int from, int to)
    {
        Type = type;
        From = from;
        To = to;
    }

    public void Deconstruct(out OperationType type, out int from, out int to) => (type, from, to) = (Type, From, To);
    public override string ToString()
    {
        return $"{Type} from: {From} to: {To}";
    }
}
