using StepByStepVisualizationWinForm.Control1;

namespace StepByStepVisualizationWinForm.Controls;

public partial class UserControl1 : UserControl, IRedoUndo<Operation>
{
    private readonly RedoUndo<Operation> _redoUndo;

    public UserControl1()
    {
        InitializeComponent();
        State = State.InitialState;
        _redoUndo = new RedoUndo<Operation>(this);
    }

    private void ZeroButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Append, Number.Zero));
    private void OneButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Append, Number.One));
    private void DeleteButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Delete, Model.LastAppend(_state)));

    private void PreviousButton_Click(object sender, EventArgs e) => _redoUndo.Undo();
    private void NextButton_Click(object sender, EventArgs e) => _redoUndo.Redo();

    private State _state;
    internal State State
    {
        get => _state;
        set => (_state, textBox1.Text, textBox2.Text, DeleteButton.Enabled, ZeroButton.Enabled)
            = (value, value.Text, value.Value.ToString(), Model.CanDelete(value), Model.CanAppendZero(value));
    }

    void IRedoUndo<Operation>.ExecuteRedo(Operation operation) => State = Model.ChangeState(_state, operation, true);
    void IRedoUndo<Operation>.ExecuteUndo(Operation operation) => State = Model.ChangeState(_state, operation, false);
    void IRedoUndo<Operation>.SetProgress(int step) { }
}
