using AnimationWinForm.Control4;
using StepExecutionWinForm;
using System.Diagnostics;

namespace AnimationWinForm;

public partial class UserControl4 : UserControl
{
    class RedoUndo : RedoUndo<Operation>
    {
        private readonly UserControl4 _control;
        public RedoUndo(UserControl4 control)
        {
            _control = control;
        }

        protected override void RedoAction(Operation operation) => _control.ExecuteRedo(operation);
        protected override void UndoAction(Operation operation) => _control.ExecuteUndo(operation);
        protected override void SetProgress(int steps) => _control.SetProgress(steps);
    }

    public UserControl4()
    {
        InitializeComponent();
        _redoUndo = new RedoUndo(this);
    }

    List<List<Label>> Cells { get; } = new();

    private MazeGenerator.Cell[][]? _maze;
    private int _startX;
    private int _startY;

    private void UserControl4_Load(object sender, EventArgs e)
    {
        const int width = 41;
        const int height = 41;

        for (int i = 0; i < height; i++)
        {
            Cells.Add(new List<Label>());
        }

        _costs = new int[height][];
        for (int i = 0; i < _costs.Length; i++)
        {
            _costs[i] = new int[width];
        }

        var labelSize = new Size(5, 5);
        _maze = MazeGenerator.GenerateMaze(width, height);

        for (int i = 0; i < _maze.Length; i++)
        {
            for (int k = 0; k < _maze[i].Length; k++)
            {
                var label = new Label();
                label.TextAlign = ContentAlignment.MiddleCenter;

                label.Size = labelSize;
                label.Location = new Point(k * labelSize.Width, i * labelSize.Height);
                Cells[i].Add(label);

                if (_maze[i][k] == MazeGenerator.Cell.Start)
                {
                    _startX = k;
                    _startY = i;
                }

                // Debug.WriteLine(label.Location);
            }
        }

        InitMaze();

        SuspendLayout();
        Controls.AddRange(Cells.SelectMany(x => x).ToArray());
        ResumeLayout();
    }

    private readonly Model _model = new Model();

    private int[][] _costs;

    private readonly RedoUndo<Operation> _redoUndo;

    private IEnumerator<Operation> _enumerator;
    private IEnumerator<Operation> Enumerator => _enumerator ??= _model.DfsBetter(_maze, _startX, _startY);

    private bool _isProcessing = false;
    private void AutoButton_Click(object sender, EventArgs e)
    {
        var button = (Button)sender;

        if (_isProcessing)
        {
            _isProcessing = false;
            return;
        }

        _isProcessing = true;
        button.Text = "Stop";

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
        button.Text = "Auto";
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

    private static (int x, int y)[] s_vector = new (int x, int y)[]
    {
        (0, 1),
        (1, 0),
        (0, -1),
        (-1, 0),
    };

    private void DrawPath(int x, int y, bool isUndo = false)
    {
        int cost = _costs[y][x];
        while (x != 1 || y != 1)
        {
            Cells[y][x].BackColor = isUndo ? Color.DarkGray : Color.Green;
            Application.DoEvents();
            cost--;

            var (nx, ny) = s_vector.First(p => _costs[p.y + y][p.x + x] == cost);
            x += nx;
            y += ny;
        }

        Cells[y][x].BackColor = isUndo ? Color.DarkGray : Color.Green;
    }
    private void ExecuteRedo(Operation op)
    {
        var (type, current, prev, cost) = op;
        var (curX, curY) = current;
        var (preX, preY) = prev;

        // Debug.WriteLine(op);

        switch (type)
        {
            case OperationType.None:
                break;
            case OperationType.Complete:
                Cells[preY][preX].BorderStyle = BorderStyle.None;
                _costs[curY][curX] = cost;
                DrawPath(preX, preY);
                break;
            case OperationType.Open:
                Cells[curY][curX].BackColor = Color.DarkGray;
                Cells[curY][curX].BorderStyle = BorderStyle.FixedSingle;
                _costs[curY][curX] = cost;

                if (prev != Point.Empty)
                {
                    Cells[preY][preX].BorderStyle = BorderStyle.None;
                }

                break;
            default:
                break;
        }
    }
    private void ExecuteUndo(Operation op)
    {
        var (type, current, prev, _) = op;
        var (curX, curY) = current;
        var (preX, preY) = prev;

        Debug.WriteLine(op);

        switch (type)
        {
            case OperationType.None:
                break;
            case OperationType.Complete:
                Cells[preY][preX].BorderStyle = BorderStyle.FixedSingle;
                DrawPath(preX, preY, true);
                break;
            case OperationType.Open:
                Cells[curY][curX].BackColor = SystemColors.Control;
                Cells[curY][curX].BorderStyle = BorderStyle.None;
                _costs[curY][curX] = int.MaxValue;

                if (prev != Point.Empty)
                {
                    Cells[preY][preX].BorderStyle = BorderStyle.FixedSingle;
                }
                break;
            default:
                break;
        }
    }

    private void SetProgress(int currentStep) => StepLabel.Text = currentStep.ToString();

    enum SearchAlgolithmType { DfsBetter, DfsWorth, Bfs, Dijkstra, AStar, PerfectAStar }
    private SearchAlgolithmType _searchAlgolithm = SearchAlgolithmType.DfsBetter;
    int AlgolithmCount => Enum.GetNames<SearchAlgolithmType>().Length;

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
            _ => throw new ArgumentException()
        };

        _redoUndo.Reset();
        RefreshNumbers();
    }

    private void InitMaze()
    {
        for (int i = 0; i < _maze.Length; i++)
        {
            for (int k = 0; k < _maze[i].Length; k++)
            {
                var label = Cells[i][k];
                label.BorderStyle = BorderStyle.None;

                (label.Text, label.BackColor) = _maze[i][k] switch
                {
                    MazeGenerator.Cell.Wall => ("", Color.Black),
                    MazeGenerator.Cell.Road => ("", SystemColors.Control),
                    MazeGenerator.Cell.Start => ("S", Color.Red),
                    MazeGenerator.Cell.Goal => ("G", Color.Green),
                    _ => throw new ArgumentException()
                };

                if (_maze[i][k] == MazeGenerator.Cell.Start)
                {
                    _startX = k;
                    _startY = i;
                }

                // Debug.WriteLine(label.Location);
            }
        }

        for (int i = 0; i < _costs.Length; i++)
        {
            for (int k = 0; k < _costs[i].Length; k++)
            {
                _costs[i][k] = int.MaxValue;
            }
        }
    }

    private void RefreshNumbers()
    {
        InitMaze();
    }
}
