using StepByStepVisualizationWinForm.Control1;

namespace StepByStepVisualizationWinForm.Controls;

public partial class UserControl1 : UserControl, IRedoUndo<Operation>
{
    private readonly RedoUndo<Operation> _redoUndo;

    public UserControl1()
    {
        InitializeComponent();
        Model = Model.InitialState;
        _redoUndo = new RedoUndo<Operation>(this);
    }

    private void ZeroButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Append, Number.Zero));
    private void OneButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Append, Number.One));
    private void DeleteButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Delete, BinaryToDecimalConverter.LastAppend(_model)));

    private void PreviousButton_Click(object sender, EventArgs e) => _redoUndo.Undo();
    private void NextButton_Click(object sender, EventArgs e) => _redoUndo.Redo();

    private Model _model;
    internal Model Model
    {
        get => _model;
        set => (_model, textBox1.Text, textBox2.Text, DeleteButton.Enabled, ZeroButton.Enabled)
            = (value, value.Text, value.Value.ToString(), BinaryToDecimalConverter.CanDelete(value), BinaryToDecimalConverter.CanAppendZero(value));
    }

    void IRedoUndo<Operation>.ExecuteRedo(Operation operation) => Model = BinaryToDecimalConverter.ChangeState(_model, operation, true);
    void IRedoUndo<Operation>.ExecuteUndo(Operation operation) => Model = BinaryToDecimalConverter.ChangeState(_model, operation, false);
    void IRedoUndo<Operation>.SetProgress(int step) { }
}
