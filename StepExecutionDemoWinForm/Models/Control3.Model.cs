namespace AnimationWinForm.Control3;

internal class Model
{
    public IEnumerator<Operation> SeiveOfEratosthenes(int count)
    {
        bool[] isPrime = Enumerable.Repeat(true, count).ToArray();

        isPrime[0] = false;
        yield return new Operation(OperationType.MarkNonPrime, 0, -1);
        isPrime[1] = false;
        yield return new Operation(OperationType.MarkNonPrime, 1, 0);

        int prev = 1;
        for (int i = 2; i < isPrime.Length; i++)
        {
            if (isPrime[i])
            {
                yield return new Operation(OperationType.MarkPrime, i, prev);
                prev = i;

                for (int k = i * 2; k < isPrime.Length; k += i)
                {
                    isPrime[k] = false;
                    yield return new Operation(OperationType.MarkNonPrime, k, prev);
                    prev = k;
                }
            }
        }

        yield return new Operation(OperationType.Complete, -1, prev);
    }

    public IEnumerator<Operation> SeiveOfAtkin(int n)
    {
        List<int> primes = new List<int>();
        int[] minFactor = Enumerable.Repeat(-1, n).ToArray();

        yield return new Operation(OperationType.MarkNonPrime, 0, -1);
        yield return new Operation(OperationType.MarkNonPrime, 1, 0);

        int prev = 1;
        for (int i = 2; i < minFactor.Length; i++)
        {
            if (minFactor[i] == -1)
            {
                minFactor[i] = i;
                primes.Add(i);
                yield return new Operation(OperationType.MarkPrime, i, prev);
                prev = i;
            }

            foreach (var prime in primes)
            {
                if (prime * i >= n || prime > minFactor[i])
                {
                    break;
                }
                // Debug.Assert(prime * i < 100);
                minFactor[prime * i] = prime;
                yield return new Operation(OperationType.MarkNonPrime, prime * i, prev);
                prev = prime * i;
            }
        }

        yield return new Operation(OperationType.Complete, -1, prev);
    }
}

internal enum OperationType
{
    None, Complete,
    MarkNonPrime, MarkPrime
}

internal class Operation
{
    public OperationType Type { get; set; }
    public int Target { get; set; }
    public int Prev { get; set; }
    public Operation(OperationType type, int target, int prev)
    {
        Type = type;
        Target = target;
        Prev = prev;
    }

    public void Deconstruct(out OperationType type, out int target, out int prev) => (type, target, prev) = (Type, Target, Prev);
    public override string ToString()
    {
        return $"{Type} target: {Target} prev: {Prev}";
    }
}
