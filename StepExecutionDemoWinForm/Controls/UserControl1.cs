using AnimationWinForm.Control1;

namespace AnimationWinForm;

public partial class UserControl1 : UserControl
{
    public UserControl1()
    {
        InitializeComponent();
        RedoUndo = new RedoUndo<Operation>(ExecuteRedo, ExecuteUndo);
    }

    private Model Model { get; set; } = new();

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

        var enumerator = Model.Move(100);

        while (enumerator.MoveNext() && _isProcessing)
        {
            var op = enumerator.Current;
            RedoUndo.Execute(op);

            await Task.Delay(100);
        }

        StartButton.Text = "Start";
        _isProcessing = false;
    }

    RedoUndo<Operation> RedoUndo { get; }

    IEnumerator<Operation>? _enumerator;
    IEnumerator<Operation>? Enumerator => _enumerator ??= Model.Move(10);

    private void NextButton_Click(object sender, EventArgs e)
    {
        if (RedoUndo.Redo())
        {
            PreviousButton.Enabled = RedoUndo.CanUndo;
            return;
        }

        if (Enumerator?.MoveNext() is true)
        {
            var op = Enumerator.Current;
            RedoUndo.Execute(op);
            PreviousButton.Enabled = RedoUndo.CanUndo;
        }
        else
        {
            _enumerator = null;
        }
    }

    private void PreviousButton_Click(object sender, EventArgs e)
    {
        if (RedoUndo.Undo())
        {
            PreviousButton.Enabled = RedoUndo.CanUndo;
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
