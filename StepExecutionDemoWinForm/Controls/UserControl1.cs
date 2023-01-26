using AnimationWinForm.Control1;

namespace AnimationWinForm;

public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();
        _redoUndo = new RedoUndo<Operation>(ExecuteRedo, ExecuteUndo);
    }

    private readonly Model _model = new();

    private bool _isProcessing = false;

    private async void StartButton_Click(object sender, EventArgs e)
    {
        if (_isProcessing)
        {
            _isProcessing = false;
            return;
        }
        if (StartButton.Text == "Start")
        {
            StartButton.Text = "Stop";
        }
        else if (StartButton.Text == "Stop")
        {
            StartButton.Text = "Start";
            return;
        }

        _isProcessing = true;

        var enumerator = _model.Move(100);

        while (enumerator.MoveNext() && _isProcessing)
        {
            var op = enumerator.Current;
            _redoUndo.Execute(op);

            await Task.Delay(100);
        }

        StartButton.Text = "Start";
        _isProcessing = false;
    }

    private readonly RedoUndo<Operation> _redoUndo;

    IEnumerator<Operation>? _enumerator;
    IEnumerator<Operation>? Enumerator => _enumerator ??= _model.Move(10);

    private void NextButton_Click(object sender, EventArgs e)
    {
        if (_redoUndo.Redo())
        {
            PreviousButton.Enabled = _redoUndo.CanUndo;
            return;
        }

        if (Enumerator?.MoveNext() is true)
        {
            var op = Enumerator.Current;
            _redoUndo.Execute(op);
            PreviousButton.Enabled = _redoUndo.CanUndo;
        }
        else
        {
            _enumerator = null;
        }
    }

    private void PreviousButton_Click(object sender, EventArgs e)
    {
        if (_redoUndo.Undo())
        {
            PreviousButton.Enabled = _redoUndo.CanUndo;
            return;
        }
    }

    private void ExecuteRedo(Operation op) => Execute(op.OperationType, op.To);
    private void ExecuteUndo(Operation op) => Execute(op.OperationType, op.From, true);

    private void Execute(OperationType operationType, int x, bool isUndo = false)
    {
        switch (operationType)
        {
            case OperationType.None:
                break;
            case OperationType.Move:
                var loc = label1.Location;
                loc.X = x;
                label1.Location = loc;
                label1.Text = $"pos: {x}";
                break;
            default:
                break;
        }
    }
}
