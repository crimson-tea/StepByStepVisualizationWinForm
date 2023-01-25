using AnimationWinForm.Control3;
using System.Diagnostics;

namespace AnimationWinForm;

public partial class UserControl3 : UserControl
{
    public UserControl3()
    {
        InitializeComponent();
        RedoUndo = new RedoUndo<Operation>(ExecuteRedo, ExecuteUndo, SetProgress);
    }

    List<Label> Numbers { get; } = new();
    List<int> SeiveCount { get; } = new();

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
            Numbers.Add(label);
            SeiveCount.Add(0);

            Debug.WriteLine(label.Location);
        }

        SuspendLayout();
        Controls.AddRange(Numbers.ToArray());
        ResumeLayout();
    }

    readonly Model _model = new Model();
    RedoUndo<Operation> RedoUndo { get; }

    IEnumerator<Operation> _enumerator;
    IEnumerator<Operation> Enumerator => _enumerator ??= _model.SeiveOfEratosthenes(_length);

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

        while (_processing && RedoUndo.Redo())
        {
            await Task.Delay(100);
        }

        while (_processing && Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            RedoUndo.Execute(op);
            await Task.Delay(100);
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
        var (type, target, prev) = op;

        Debug.WriteLine(op);

        switch (type)
        {
            case OperationType.None:
                break;
            case OperationType.Complete:
                Numbers[prev].BorderStyle = BorderStyle.None;
                break;
            case OperationType.MarkNonPrime:
                Numbers[target].BackColor = Color.DarkGray;
                Numbers[target].BorderStyle = BorderStyle.FixedSingle;

                if (prev >= 0)
                {
                    Numbers[prev].BorderStyle = BorderStyle.None;
                }

                SeiveCount[target]++;

                break;
            case OperationType.MarkPrime:
                Numbers[target].BackColor = Color.LightGreen;
                Numbers[target].BorderStyle = BorderStyle.FixedSingle;

                Numbers[prev].BorderStyle = BorderStyle.None;
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
                Numbers[prev].BorderStyle = BorderStyle.FixedSingle;

                break;
            case OperationType.MarkNonPrime:
                SeiveCount[target]--;

                if (SeiveCount[target] == 0)
                {
                    Numbers[target].BackColor = SystemColors.Control;
                }

                Numbers[target].BorderStyle = BorderStyle.None;
                if (prev >= 0)
                {
                    Numbers[prev].BorderStyle = BorderStyle.FixedSingle;
                }

                break;
            case OperationType.MarkPrime:
                Numbers[target].BackColor = SystemColors.Control;
                Numbers[prev].BorderStyle = BorderStyle.FixedSingle;
                Numbers[target].BorderStyle = BorderStyle.None;
                break;
            default:
                break;
        }
    }

    void SetProgress(int currentStep) => StepLabel.Text = currentStep.ToString();

    enum SeiveType { Eratosthenes, Atkin }
    private SeiveType _seive = SeiveType.Eratosthenes;
    int SeiveCountOfType => Enum.GetNames<SeiveType>().Length;

    private void SwitchSeiveButton_Click(object sender, EventArgs e)
    {
        _seive = (SeiveType)(((int)_seive + 1) % SeiveCountOfType);

        (_enumerator, SeiveTypeLabel.Text) = _seive switch
        {
            SeiveType.Eratosthenes => (_model.SeiveOfEratosthenes(100), "Eratosthenes"),
            SeiveType.Atkin => (_model.SeiveOfAtkin(100), "Atkin"),
            _ => throw new ArgumentException()
        };

        RedoUndo.Reset();
        RefreshNumbers();
    }

    private void RefreshNumbers()
    {
        foreach (var number in Numbers)
        {
            number.BackColor = SystemColors.Control;
            number.BorderStyle = BorderStyle.None;
        }

        for (int i = 0; i < SeiveCount.Count; i++)
        {
            SeiveCount[i] = 0;
        }
    }
}
