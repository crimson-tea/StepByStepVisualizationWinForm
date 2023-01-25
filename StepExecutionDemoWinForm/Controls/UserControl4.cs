using AnimationWinForm.Control4;
using StepExecutionWinForm;
using System.Diagnostics;

namespace AnimationWinForm;

public partial class UserControl4 : UserControl
{
    public UserControl4()
    {
        InitializeComponent();
        RedoUndo = new RedoUndo<Operation>(ExecuteRedo, ExecuteUndo, SetProgress);
    }

    List<List<Label>> Cells { get; } = new();

    private MazeGenerator.Cell[][] _maze;
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

                //label.Font = new Font(label.Font.FontFamily.Name, 7);
                //label.Text = i.ToString();

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

    readonly Model _model = new Model();

    private int[][] _costs;

    RedoUndo<Operation> RedoUndo { get; }

    IEnumerator<Operation> _enumerator;
    IEnumerator<Operation> Enumerator => _enumerator ??= _model.Dfs(_maze, _startX, _startY);

    private bool _processing = false;
    private void AutoButton_Click(object sender, EventArgs e)
    {
        var b = (Button)sender;

        if (_processing)
        {
            _processing = false;
            return;
        }

        _processing = true;
        b.Text = "Stop";

        while (_processing && RedoUndo.Redo())
        {
            Application.DoEvents();
        }

        int count = 0;
        while (_processing && Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            RedoUndo.Execute(op);

            count++;
            if (count % 10 == 0)
            {
                Application.DoEvents();
            }
        }

        _processing = false;
        b.Text = "Auto";
    }

    private void NextButton_Click(object sender, EventArgs e)
    {
        if (RedoUndo.Redo())
        {
            return;
        }

        if (Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            RedoUndo.Execute(op);
        }
    }

    private void PrevButton_Click(object sender, EventArgs e)
    {
        if (RedoUndo.Undo())
        {
            return;
        }
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
                //  _costs[curY][curX] = cost;
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

            try
            {
                var (nx, ny) = s_vector.First(p => _costs[p.y + y][p.x + x] == cost);
                x += nx;
                y += ny;
            }
            catch (Exception)
            {
                throw;
            }

        }

        Cells[y][x].BackColor = isUndo ? Color.DarkGray : Color.Green;
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
                    //Cells[preY][preX].BackColor = Color.DarkGray;
                    Cells[preY][preX].BorderStyle = BorderStyle.FixedSingle;
                }

                break;
            //case OperationType.ReWrite:
            //    Numbers[target].BackColor = Color.LightGreen;
            //    Numbers[target].BorderStyle = BorderStyle.FixedSingle;

            //    Numbers[prev].BorderStyle = BorderStyle.None;
            //    break;
            default:
                break;
        }
    }

    void SetProgress(int currentStep) => StepLabel.Text = currentStep.ToString();

    enum SeiveType { DfsBetter, DfsWorth, Bfs, Dijkstra, AStar }
    private SeiveType _seive = SeiveType.DfsBetter;
    int SeiveCount => Enum.GetNames<SeiveType>().Length;

    private void SwitchSeiveButton_Click(object sender, EventArgs e)
    {
        _seive = (SeiveType)(((int)_seive + 1) % SeiveCount);

        (_enumerator, SearchTypeLabel.Text) = _seive switch
        {
            SeiveType.DfsBetter => (_model.Dfs(_maze, _startX, _startY), "DFS (better)"),
            SeiveType.DfsWorth => (_model.Dfs(_maze, _startX, _startY, true), "DFS (worth)"),
            SeiveType.Bfs => (_model.Bfs(_maze, _startX, _startY), "BFS"),
            SeiveType.Dijkstra => (_model.Dijkstra(_maze, _startX, _startY), "Dijkstra"),
            SeiveType.AStar => (_model.AStar(_maze, _startX, _startY), "A*"),
            _ => throw new ArgumentException()
        };

        RedoUndo.Reset();
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
