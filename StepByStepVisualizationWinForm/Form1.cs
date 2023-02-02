using System.Diagnostics;
using StepByStepVisualizationWinForm.Controls;

namespace StepByStepVisualizationWinForm;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        _currentControl = new UserControl1();
        _currentControl.Location = new Point(9, 9);
        Controls.Add(_currentControl);
        _controlMode = ControlMode.RedoUndo;
        Text = "RedoUndoDemo";
    }

    private enum ControlMode { RedoUndo, BinarySearch, Sieve, Graph }

    static int ModeCount => Enum.GetValues<ControlMode>().Length;

    private ControlMode _controlMode;
    private UserControl _currentControl;

    private void SwitchButton_Click(object sender, EventArgs e)
    {
        _controlMode = (ControlMode)(((int)_controlMode + 1) % ModeCount);

        Controls.Remove(_currentControl);
        // Dispose‚ð–Y‚ê‚¸‚É
        _currentControl.Dispose();

        (_currentControl, string text, Text) = _controlMode switch
        {
            ControlMode.RedoUndo => (new UserControl1() as UserControl, "Switch2", "RedoUndoDemo"),
            ControlMode.BinarySearch => (new UserControl2(), "Switch3", "BinarySearchStepExecution"),
            ControlMode.Sieve => (new UserControl3(), "Switch4", "SeiveStepExecution"),
            ControlMode.Graph => (new UserControl4(), "Switch1", "SolveMazeStepExecution"),
            _ => throw new ArgumentException()
        };
        _currentControl.Location = new Point(9, 9);
        Controls.Add(_currentControl);

        SwitchButton.Text = text;
    }

    private void Form1_ResizeEnd(object sender, EventArgs e)
    {
        Debug.WriteLine(Size.ToString());
    }
}
