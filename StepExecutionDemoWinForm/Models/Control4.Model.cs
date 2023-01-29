using StepExecutionWinForm;

namespace AnimationWinForm.Control4;

internal class Model
{
    public IEnumerator<Operation> DfsBetter(MazeGenerator.Cell[][] cells, int startX, int startY, bool reverse = false)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        Stack<(int x, int y, int length)> stack = new();
        stack.Push((startX, startY, 0));

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (stack.Count > 0)
        {
            var (x, y, length) = stack.Pop();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, length);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            var directions = reverse ? MazeGenerator.Directions.Reverse() : MazeGenerator.Directions;
            foreach (var dir in directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int nextX = x + vx;
                int nextY = y + vy;

                if (cells[nextY][nextX] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                stack.Push((nextX, nextY, length + 1));
            }
        }

        yield return new Operation(OperationType.Complete, Previous: prev);
    }

    public IEnumerator<Operation> DfsWorst(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        List<Operation> list = new List<Operation>();
        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        Dfs(startX, startY, 0);

        foreach (var item in list)
        {
            yield return item;
        }

        yield return new Operation(OperationType.Complete, Previous: prev);

        void Dfs(int x, int y, int pathLength)
        {
            if (visited[y][x])
            {
                return;
            }

            visited[y][x] = true;
            list.Add(new Operation(OperationType.Open, new Point(x, y), prev, pathLength));

            prev = new Point(x, y);

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int nextX = x + vx;
                int nextY = y + vy;

                if (cells[nextY][nextX] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                Dfs(nextX, nextY, pathLength + 1);
            }
        }
    }

    public IEnumerator<Operation> Bfs(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        Queue<(int x, int y, int length)> queue = new();
        queue.Enqueue((startX, startY, 0));

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (queue.Count > 0)
        {
            var (x, y, length) = queue.Dequeue();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, length);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int nextX = x + vx;
                int nextY = y + vy;

                if (cells[nextY][nextX] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                queue.Enqueue((nextX, nextY, length + 1));
            }
        }

        yield return new Operation(OperationType.Complete, Previous: prev);
    }

    public IEnumerator<Operation> Dijkstra(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        const int INF = int.MaxValue >> 2;

        int height = cells.Length;
        int width = cells[0].Length;

        int[][] costs = new int[height][];
        for (int i = 0; i < costs.Length; i++)
        {
            costs[i] = new int[width];
            for (int k = 0; k < costs[i].Length; k++)
            {
                costs[i][k] = INF;
            }
        }

        costs[1][1] = 0;

        PriorityQueue<(int x, int y, int cost), int> queue = new();
        queue.Enqueue((startX, startY, 0), 0);

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (queue.Count > 0)
        {
            var (x, y, cost) = queue.Dequeue();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            costs[y][x] = cost;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, cost);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int nextX = x + vx;
                int nextY = y + vy;

                if (cells[nextY][nextX] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                if (visited[nextY][nextX])
                {
                    continue;
                }

                if (costs[nextY][nextX] <= costs[y][x] + 1)
                {
                    // 各辺の長さが1なのでここには来ない。
                    throw new NotImplementedException();
                }

                queue.Enqueue((nextX, nextY, cost + 1), cost + 1);
            }
        }

        yield return new Operation(OperationType.Complete, Previous: prev);
    }

    public IEnumerator<Operation> AStar(MazeGenerator.Cell[][] cells, int startX, int startY)
    {
        int height = cells.Length;
        int width = cells[0].Length;

        int goalX = width - 2;
        int goalY = height - 2;

        PriorityQueue<(int x, int y, int cost), int> queue = new();
        queue.Enqueue((startX, startY, 0), 0);

        bool[][] visited = new bool[cells.Length][];
        for (int i = 0; i < visited.Length; i++)
        {
            visited[i] = new bool[cells[i].Length];
        }

        Point prev = Point.Empty;

        while (queue.Count > 0)
        {
            var (x, y, cost) = queue.Dequeue();

            if (visited[y][x])
            {
                continue;
            }

            visited[y][x] = true;
            yield return new Operation(OperationType.Open, new Point(x, y), prev, cost);
            prev = new Point(x, y);

            if (cells[y][x] == MazeGenerator.Cell.Goal)
            {
                break;
            }

            foreach (var dir in MazeGenerator.Directions)
            {
                var (vx, vy) = MazeGenerator.GetVector(dir);

                int nextX = x + vx;
                int nextY = y + vy;

                if (cells[nextY][nextX] == MazeGenerator.Cell.Wall)
                {
                    continue;
                }

                queue.Enqueue((nextX, nextY, cost + 1), cost + 1 + CalcHeulisticCost(nextX, nextY));
            }
        }

        yield return new Operation(OperationType.Complete, Previous: prev);

        int CalcHeulisticCost(int x, int y)
        {
            return Math.Abs(goalX - x) + Math.Abs(goalY - y);
        }
    }
}

internal enum OperationType
{
    None,
    Complete,
    Open,
}

internal record Operation(OperationType OperationType, Point Current = default, Point Previous = default, int PathLength = 0);

public static class PointExtensions
{
    public static void Deconstruct(this Point point, out int x, out int y) => (x, y) = (point.X, point.Y);
}
