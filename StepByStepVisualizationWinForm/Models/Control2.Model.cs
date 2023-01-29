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

internal record Operation(OperationType OperationType, int From , int To);
