using StepByStepVisualizationWinForm.Control1;

namespace StepByStepVisualizationWinForm.Controls;

public partial class UserControl1 : UserControl, IRedoUndo<Operation>
{
    private readonly RedoUndo<Operation> _redoUndo;

    public UserControl1()
    {
        InitializeComponent();
        _redoUndo = new RedoUndo<Operation>(this);
    }

    private void ZeroButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Append, '0'));

    private void OneButton_Click(object sender, EventArgs e) => _redoUndo.Execute(new Operation(OperationType.Append, '1'));

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_model.Text))
        {
            return;
        }
        _redoUndo.Execute(new Operation(OperationType.Delete, _model.Text[^1]));
    }

    private void PreviousButton_Click(object sender, EventArgs e) => _redoUndo.Undo();

    private void NextButton_Click(object sender, EventArgs e) => _redoUndo.Redo();

    private Model _model = Model.InitialState;
    internal Model Model
    {
        get => _model;
        set => SetModel(value);
    }

    private void SetModel(Model model)
    {
        _model = model;
        (textBox1.Text, textBox2.Text) = (_model.Text, _model.Value.ToString());
    }

    void IRedoUndo<Operation>.ExecuteRedo(Operation operation) => Model = BinaryToDecimalConverter.ChangeState(_model, operation, true);
    void IRedoUndo<Operation>.ExecuteUndo(Operation operation) => Model = BinaryToDecimalConverter.ChangeState(_model, operation, false);
    void IRedoUndo<Operation>.SetProgress(int step) { }
}
