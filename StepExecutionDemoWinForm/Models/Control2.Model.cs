namespace AnimationWinForm.Control2;

internal class BSModel
{
    public IEnumerator<BSOperation> BinarySearch(int[] values, int target)
    {
        int l = -1;
        int r = values.Length;

        while (Math.Abs(l - r) > 1)
        {
            int mid = (l + r) / 2;
            if (values[mid] < target)
            {
                yield return new BSOperation(BSOperationType.MoveLeft, l, mid);
                l = mid;
            }
            else
            {
                yield return new BSOperation(BSOperationType.MoveRight, r, mid);
                r = mid;
            }
        }

        yield return new BSOperation(BSOperationType.Complete, r, r);
    }
}

internal enum BSOperationType { None, Complete, MoveLeft, MoveRight }

internal class BSOperation
{
    public BSOperationType Type { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public BSOperation(BSOperationType type, int from, int to)
    {
        Type = type;
        From = from;
        To = to;
    }

    public void Deconstruct(out BSOperationType type, out int from, out int to) => (type, from, to) = (Type, From, To);
    public override string ToString()
    {
        return $"{Type} from: {From} to: {To}";
    }
}
