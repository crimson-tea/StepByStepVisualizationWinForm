using StepByStepVisualizationWinForm.Control4;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace StepByStepVisualizationWinForm.Controls;

public partial class UserControl4 : UserControl, IRedoUndo<Operation>
{
    public UserControl4()
    {
        InitializeComponent();
        _redoUndo = new RedoUndo<Operation>(this);

        _costs = new int[HEIGHT][];
        for (int i = 0; i < _costs.Length; i++)
        {
            _costs[i] = new int[WIDTH];
        }

        _maze = MazeGenerator.GenerateMaze(WIDTH, HEIGHT);
        _costs = InitCosts(WIDTH, IMAGE_HEIGHT);

        mazePictureBox.BackgroundImageLayout = ImageLayout.None;
        mazePictureBox.BackgroundImage = InitMaze(IMAGE_WIDTH, IMAGE_HEIGHT);
    }

    private readonly MazeGenerator.Cell[][] _maze;
    private readonly int _startX = 1;
    private readonly int _startY = 1;

    private const int WIDTH = 39;
    private const int HEIGHT = 39;
    private const int IMAGE_HEIGHT = HEIGHT * CELL_SIZE;
    private const int IMAGE_WIDTH = WIDTH * CELL_SIZE;
    private const int CELL_SIZE = 5; // 正方形

    private static int[][] InitCosts(int width, int height)
    {
        int[][] costs = new int[height][];
        for (int i = 0; i < costs.Length; i++)
        {
            costs[i] = new int[width];
        }

        for (int i = 0; i < costs.Length; i++)
        {
            for (int k = 0; k < costs[i].Length; k++)
            {
                costs[i][k] = int.MaxValue;
            }
        }
        return costs;
    }

    private readonly Model _model = new();

    private int[][] _costs;

    private readonly RedoUndo<Operation> _redoUndo;

    private IEnumerator<Operation>? _enumerator;

    private IEnumerator<Operation> Enumerator => _enumerator ??= _model.DfsBetter(_maze, _startX, _startY);

    private bool _isProcessing = false;

    private void AutoButton_Click(object sender, EventArgs e)
    {
        var AutoButton = (Button)sender;

        if (_isProcessing)
        {
            _isProcessing = false;
            return;
        }

        _isProcessing = true;
        AutoButton.Text = "Stop";

        while (_isProcessing && _redoUndo.Redo())
        {
            Application.DoEvents();
        }

        int count = 0;
        while (_isProcessing && Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            _redoUndo.Execute(op);

            count++;
            if (count % 10 == 0)
            {
                Application.DoEvents();
            }
        }

        _isProcessing = false;
        AutoButton.Text = "Auto";
    }

    private void NextButton_Click(object sender, EventArgs e)
    {
        if (_isProcessing) { return; }
        if (_redoUndo.Redo())
        {
            return;
        }

        if (Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            _redoUndo.Execute(op);
        }
    }

    private void PrevButton_Click(object sender, EventArgs e)
    {
        if (_isProcessing) { return; }
        if (_redoUndo.Undo())
        {
            return;
        }
    }

    private readonly static (int x, int y)[] s_vector = new (int x, int y)[]
    {
        (0, 1),
        (1, 0),
        (0, -1),
        (-1, 0),
    };

    private Image PathImage(int width, int height, int x, int y)
    {
        var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(image);

        int cost = _costs[y][x];
        while (x != 1 || y != 1)
        {
            g.DrawPath(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE));

            mazePictureBox.Image = image;
            Application.DoEvents();

            cost--;

            var (nx, ny) = s_vector.First(p => _costs[p.y + y][p.x + x] == cost);
            x += nx;
            y += ny;
        }

        g.DrawPath(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE));
        return image;
    }

    private void ExecuteRedo(Operation op)
    {
        var (type, current, prev, cost) = op;
        var (curX, curY) = current;
        var (preX, preY) = prev;

        if (type == OperationType.Open)
        {
            _costs[curY][curX] = cost;
        }

        var image = InitMaze(IMAGE_WIDTH, IMAGE_HEIGHT);

        mazePictureBox.BackgroundImage?.Dispose();
        mazePictureBox.BackgroundImage = type switch
        {
            OperationType.None => null,
            OperationType.Complete => DrawHistory(image, _costs),
            OperationType.Open => DrawHistory(image, _costs),
            _ => throw new ArgumentException(nameof(op.OperationType)),
        };

        mazePictureBox.Image?.Dispose();
        mazePictureBox.Image = type switch
        {
            OperationType.None => null,
            OperationType.Complete => PathImage(IMAGE_WIDTH, IMAGE_HEIGHT, preX, preY),
            OperationType.Open => CurrentBoarderImage(IMAGE_WIDTH, IMAGE_HEIGHT, new Rectangle(curX * CELL_SIZE, curY * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            _ => throw new ArgumentException(nameof(op.OperationType)),
        };
    }

    private void ExecuteUndo(Operation op)
    {
        var (type, current, prev, _) = op;
        var (curX, curY) = current;
        var (preX, preY) = prev;

        Debug.WriteLine(op);

        if (type == OperationType.Open)
        {
            _costs[curY][curX] = int.MaxValue;
        }

        var image = InitMaze(IMAGE_WIDTH, IMAGE_HEIGHT);

        mazePictureBox.BackgroundImage?.Dispose();
        mazePictureBox.BackgroundImage = type switch
        {
            OperationType.None => null,
            OperationType.Complete => DrawHistory(image, _costs),
            OperationType.Open => DrawHistory(image, _costs),
            _ => throw new ArgumentException(nameof(op.OperationType)),
        };

        mazePictureBox.Image?.Dispose();
        mazePictureBox.Image = (type, preX, preY) switch
        {
            (OperationType.None, _, _) => null,
            (OperationType.Complete, _, _) => CurrentBoarderImage(IMAGE_WIDTH, IMAGE_HEIGHT, new Rectangle(preX * CELL_SIZE, preY * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            (OperationType.Open, 0, 0) => null,
            (OperationType.Open, _, _) => CurrentBoarderImage(IMAGE_WIDTH, IMAGE_HEIGHT, new Rectangle(preX * CELL_SIZE, preY * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            _ => throw new ArgumentException(nameof(op.OperationType)),
        };
    }

    private static Bitmap DrawHistory(Bitmap image, int[][] costs)
    {
        using Graphics g = Graphics.FromImage(image);
        for (int y = 0; y < costs.Length; y++)
        {
            for (int x = 0; x < costs[y].Length; x++)
            {
                if (costs[y][x] != int.MaxValue)
                {
                    g.DrawMarked(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE));
                }
            }
        }
        return image;
    }

    private static Image CurrentBoarderImage(int width, int height, Rectangle rectangle)
    {
        var image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(image);
        g.DrawBorder(rectangle);
        return image;
    }

    private void SetProgress(int currentStep) => StepLabel.Text = currentStep.ToString();

    private enum SearchAlgolithmType { DfsBetter, DfsWorth, Bfs, Dijkstra, AStar, PerfectAStar }

    private SearchAlgolithmType _searchAlgolithm = SearchAlgolithmType.DfsBetter;

    static int AlgolithmCount => Enum.GetNames<SearchAlgolithmType>().Length;

    private void SwitchSieveButton_Click(object sender, EventArgs e)
    {
        _searchAlgolithm = (SearchAlgolithmType)(((int)_searchAlgolithm + 1) % AlgolithmCount);

        (_enumerator, SearchTypeLabel.Text) = _searchAlgolithm switch
        {
            SearchAlgolithmType.DfsBetter => (_model.DfsBetter(_maze, _startX, _startY), "DFS (better)"),
            SearchAlgolithmType.DfsWorth => (_model.DfsBetter(_maze, _startX, _startY, true), "DFS (worth)"),
            SearchAlgolithmType.Bfs => (_model.Bfs(_maze, _startX, _startY), "BFS"),
            SearchAlgolithmType.Dijkstra => (_model.Dijkstra(_maze, _startX, _startY), "Dijkstra"),
            SearchAlgolithmType.AStar => (_model.AStar(_maze, _startX, _startY), "A*"),
            SearchAlgolithmType.PerfectAStar => (_model.AStarWithPerfectHeuristic(_maze, _startX, _startY), "A* (Perfect)"),
            _ => throw new ArgumentException(nameof(_searchAlgolithm))
        };

        _redoUndo.Reset();
        _costs = InitCosts(WIDTH, HEIGHT);
        mazePictureBox.BackgroundImage?.Dispose();
        mazePictureBox.BackgroundImage = InitMaze(IMAGE_WIDTH, IMAGE_HEIGHT);
        mazePictureBox.Image?.Dispose();
        mazePictureBox.Image = null;
    }


    private Bitmap InitMaze(int imageWidth, int imageHeight)
    {
        var image = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
        using Graphics g = Graphics.FromImage(image);
        g.FillRectangle(Brushes.White, new Rectangle(Point.Empty, image.Size));

        // こうも書けるがわかりにくい。
        // _ = _maze.Select((row, y) => row.Select((c, x) => (c, x)).Aggregate(g, (g, t) => DrawMaze(g, t.c, t.x, y))).Last();

        for (int y = 0; y < _maze.Length; y++)
            for (int i = 0; i < _maze[y].Length; i++)
                _ = DrawMaze(g, _maze[y][i], i, y);

        return image;

        static Graphics DrawMaze(Graphics g, MazeGenerator.Cell cell, int x, int y) => cell switch
        {
            MazeGenerator.Cell.Wall => g.DrawWall(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            MazeGenerator.Cell.Road => g.DrawRoad(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            MazeGenerator.Cell.Start => g.DrawStart(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            MazeGenerator.Cell.Goal => g.DrawGoal(new Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE)),
            _ => throw new ArgumentException("Cell が未知の状態です。", nameof(cell))
        };
    }

    void IRedoUndo<Operation>.ExecuteRedo(Operation operation) => ExecuteRedo(operation);
    void IRedoUndo<Operation>.ExecuteUndo(Operation operation) => ExecuteUndo(operation);
    void IRedoUndo<Operation>.SetProgress(int step) => SetProgress(step);
}

public static class GraphicsExtensions
{
    public static Graphics DrawWall(this Graphics g, Rectangle rectangle)
    {
        g.FillRectangle(Brushes.Black, rectangle);
        return g;
    }

    public static Graphics DrawRoad(this Graphics g, Rectangle rectangle)
    {
        g.FillRectangle(SystemBrushes.Control, rectangle);
        return g;
    }

    public static Graphics DrawMarked(this Graphics g, Rectangle rectangle)
    {
        g.FillRectangle(Brushes.Gray, rectangle);
        return g;
    }

    public static Graphics DrawStart(this Graphics g, Rectangle rectangle)
    {
        g.FillRectangle(Brushes.Red, rectangle);
        return g;
    }

    public static Graphics DrawGoal(this Graphics g, Rectangle rectangle)
    {
        g.FillRectangle(Brushes.Green, rectangle);
        return g;
    }

    public static Graphics DrawPath(this Graphics g, Rectangle rectangle)
    {
        g.FillRectangle(Brushes.Green, rectangle);
        return g;
    }

    public static Graphics DrawBorder(this Graphics g, Rectangle rectangle)
    {
        var pen = new Pen(Brushes.DarkGray, 2);
        g.DrawRectangle(pen, rectangle);
        return g;
    }
}
