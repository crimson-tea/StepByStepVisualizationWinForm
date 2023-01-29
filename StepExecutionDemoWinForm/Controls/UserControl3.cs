using AnimationWinForm.Control3;
using StepExecutionDemoWinForm;
using System.Diagnostics;

namespace AnimationWinForm;

public partial class UserControl3 : UserControl, IRedoUndo<Operation>
{
    public UserControl3()
    {
        InitializeComponent();
        _redoUndo = new RedoUndo<Operation>(this);
    }

    private readonly List<Label> _numbers = new();
    /// <summary>
    /// 篩われた回数をカウント
    /// </summary>
    private readonly List<int> _sieveCount = new();
    private readonly int _length = 100;

    private void UserControl2_Load(object sender, EventArgs e)
    {
        const int width = 25;
        const int height = 20;
        const int col = 10;

        for (int i = 0; i < _length; i++)
        {
            var label = new Label();
            label.TextAlign = ContentAlignment.MiddleRight;
            label.Text = i.ToString();
            label.Size = new Size(width, height);
            label.Location = new Point((i % col) * width, i / col * height);
            _numbers.Add(label);
            _sieveCount.Add(0);

            Debug.WriteLine(label.Location);
        }

        SuspendLayout();
        Controls.AddRange(_numbers.ToArray());
        ResumeLayout();
    }

    readonly Model _model = new Model();

    private readonly RedoUndo<Operation> _redoUndo;

    private IEnumerator<Operation> _enumerator;
    private IEnumerator<Operation> Enumerator => _enumerator ??= _model.SieveOfEratosthenes(_length);

    private bool _processing = false;

    private async void AutoButton_Click(object sender, EventArgs e)
    {
        var b = (Button)sender;

        if (_processing)
        {
            _processing = false;
            return;
        }

        _processing = true;
        b.Text = "Stop";

        while (_processing && _redoUndo.Redo())
        {
            await Task.Delay(100);
        }

        while (_processing && Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            _redoUndo.Execute(op);
            await Task.Delay(100);
        }

        _processing = false;
        b.Text = "Auto";
    }

    private void NextButton_Click(object sender, EventArgs e)
    {
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
        if (_redoUndo.Undo())
        {
            return;
        }
    }

    private void ExecuteRedo(Operation op)
    {
        var (type, target, prev) = op;

        Debug.WriteLine(op);

        switch (type)
        {
            case OperationType.None:
                break;
            case OperationType.Complete:
                _numbers[prev].BorderStyle = BorderStyle.None;
                break;
            case OperationType.MarkNonPrime:
                _numbers[target].BackColor = Color.DarkGray;
                _numbers[target].BorderStyle = BorderStyle.FixedSingle;

                if (prev >= 0)
                {
                    _numbers[prev].BorderStyle = BorderStyle.None;
                }

                _sieveCount[target]++;

                break;
            case OperationType.MarkPrime:
                _numbers[target].BackColor = Color.LightGreen;
                _numbers[target].BorderStyle = BorderStyle.FixedSingle;

                _numbers[prev].BorderStyle = BorderStyle.None;
                break;
            default:
                break;
        }
    }

    private void ExecuteUndo(Operation op)
    {
        var (type, target, prev) = op;

        Debug.WriteLine(op);

        switch (type)
        {
            case OperationType.None:
                break;
            case OperationType.Complete:
                _numbers[prev].BorderStyle = BorderStyle.FixedSingle;

                break;
            case OperationType.MarkNonPrime:
                _sieveCount[target]--;

                if (_sieveCount[target] == 0)
                {
                    _numbers[target].BackColor = SystemColors.Control;
                }

                _numbers[target].BorderStyle = BorderStyle.None;
                if (prev >= 0)
                {
                    _numbers[prev].BorderStyle = BorderStyle.FixedSingle;
                }

                break;
            case OperationType.MarkPrime:
                _numbers[target].BackColor = SystemColors.Control;
                _numbers[prev].BorderStyle = BorderStyle.FixedSingle;
                _numbers[target].BorderStyle = BorderStyle.None;
                break;
            default:
                break;
        }
    }

    private void SetProgress(int currentStep) => StepLabel.Text = currentStep.ToString();

    enum SieveType { Eratosthenes, Atkin }
    private SieveType _sieve = SieveType.Eratosthenes;
    int SieveCountOfType => Enum.GetNames<SieveType>().Length;

    private void SwitchSieveButton_Click(object sender, EventArgs e)
    {
        _sieve = (SieveType)(((int)_sieve + 1) % SieveCountOfType);

        (_enumerator, SieveTypeLabel.Text) = _sieve switch
        {
            SieveType.Eratosthenes => (_model.SieveOfEratosthenes(100), "Eratosthenes"),
            SieveType.Atkin => (_model.SieveOfAtkin(100), "Atkin"),
            _ => throw new ArgumentException()
        };

        _redoUndo.Reset();
        RefreshNumbers();
    }

    private void RefreshNumbers()
    {
        foreach (var number in _numbers)
        {
            number.BackColor = SystemColors.Control;
            number.BorderStyle = BorderStyle.None;
        }

        for (int i = 0; i < _sieveCount.Count; i++)
        {
            _sieveCount[i] = 0;
        }
    }

    void IRedoUndo<Operation>.ExecuteRedo(Operation operation) => ExecuteRedo(operation);
    void IRedoUndo<Operation>.ExecuteUndo(Operation operation) => ExecuteUndo(operation);
    void IRedoUndo<Operation>.SetProgress(int step) => SetProgress(step);
}
