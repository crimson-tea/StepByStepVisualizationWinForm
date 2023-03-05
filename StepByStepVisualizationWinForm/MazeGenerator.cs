using System.Diagnostics;

namespace StepByStepVisualizationWinForm
{
    internal class MazeGenerator
    {
        public enum Direction { Up, Left, Down, Right }
        public enum Cell { Wall, Road, Start, Goal }

        private static readonly Random s_rand = new Random(100001);

        public static readonly Direction[] Directions = new Direction[] {
            Direction.Up,
            Direction.Left,
            Direction.Down,
            Direction.Right,
        };

        public static Cell[][] GenerateMaze(int width, int height)
        {
            Cell[][] field = new Cell[height][];
            for (int i = 0; i < field.Length; i++)
            {
                field[i] = new Cell[width];
            }

            FillAround(Cell.Road);

            int x = GetRandomOdd(width);
            int y = GetRandomOdd(height);

            Dfs(x, y);

            FillAround(Cell.Wall);

            field[1][1] = Cell.Start;
            field[^2][^2] = Cell.Goal;

            return field;

            void FillAround(Cell value)
            {
                for (int i = 0; i < field.Length; i++)
                {
                    for (int k = 0; k < field[i].Length; k++)
                    {
                        if (i == 0 || i == field.Length - 1
                            || k == 0 || k == field[i].Length - 1)
                        {
                            field[i][k] = value;
                        }
                    }
                }
            }

            void Dfs(int x, int y)
            {
                if (x < 0 || width <= x)
                {
                    return;
                }

                if (y < 0 || height <= y)
                {
                    return;
                }

                field[y][x] = Cell.Road;

                List<Direction> list = Directions.ToList();

                list.Shuffle();
                while (list.Count != 0)
                {
                    var dir = list[^1];
                    list.RemoveAt(list.Count - 1);

                    var (vx, vy) = GetVector(dir);

                    int x1 = x + vx;
                    int y1 = y + vy;
                    int x2 = x + vx * 2;
                    int y2 = y + vy * 2;

                    if (x2 < 0 || width <= x2 || y2 < 0 || height <= y2)
                    {
                        continue;
                    }

                    if (field[y2][x2] == Cell.Wall)
                    {
                        field[y][x] = field[y1][x1] = Cell.Road;
                        // DebugShow(field);
                        Dfs(x2, y2);
                    }
                }
            }

            int GetRandomOdd(int n)
            {
                int v = s_rand.Next(n / 2);
                return v * 2 + 1;
            }
        }

        public static (int x, int y) GetVector(Direction direction) => direction switch
        {
            Direction.Up => (0, -1),
            Direction.Right => (1, 0),
            Direction.Down => (0, 1),
            Direction.Left => (-1, 0),
            _ => throw new ArgumentException()
        };

        private static void DebugShow(Cell[][] field)
        {
            foreach (var row in field)
            {
                Debug.WriteLine(new string(row.Select(x => x == Cell.Road ? '.' : '#').ToArray()));
            }
        }
    }

    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int dest = Random.Shared.Next(i);
                (list[i], list[dest]) = (list[dest], list[i]);
            }
        }
    }
}
