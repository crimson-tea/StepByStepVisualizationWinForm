using StepExecutionDemoWinForm;

namespace AnimationWinForm;

/// <summary>
/// RedoUndoを実装します。
/// </summary>
internal abstract class RedoUndoBase<TOperation>
{
    private Stack<TOperation> _undo;
    private Stack<TOperation> _redo;
    protected abstract void RedoAction(TOperation operation);
    protected abstract void UndoAction(TOperation operation);
    protected abstract void SetProgress(int steps);

    /// <param name="initCapacity">RedoとUndoに使われるスタックの大きさを指定します。RedoとUndoそれぞれこのキャパシティで初期化されます。</param>
    public RedoUndoBase(int initCapacity = 65535)
    {
        _redo = new Stack<TOperation>(initCapacity);
        _undo = new Stack<TOperation>(initCapacity);
    }

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;
    public int Steps => _undo.Count;

    public void Execute(TOperation op)
    {
        _redo.Clear();
        _undo.Push(op);
        SetProgress(_undo.Count);
        RedoAction(op);
    }

    /// <summary>
    /// Redoを試します。
    /// </summary>
    /// <returns>true: 成功 false: 失敗</returns>
    public bool Redo()
    {
        if (_redo.Count == 0)
        {
            return false;
        }

        var op = _redo.Pop();
        _undo.Push(op);
        SetProgress(_undo.Count);
        RedoAction(op);

        return true;
    }

    /// <summary>
    /// Undoを試します。
    /// </summary>
    /// <returns>true: 成功 false: 失敗</returns>
    public bool Undo()
    {
        if (_undo.Count == 0)
        {
            return false;
        }

        var op = _undo.Pop();
        _redo.Push(op);
        SetProgress(_undo.Count);
        UndoAction(op);

        return true;
    }

    /// <summary>
    /// RedoUndoをリセットします。
    /// </summary>
    internal void Reset()
    {
        _redo.Clear();
        _undo.Clear();
        SetProgress(_undo.Count);
    }
}

class RedoUndo<TOperation> : RedoUndoBase<TOperation>
{
    private readonly IRedoUndo<TOperation> _control;

    public RedoUndo(IRedoUndo<TOperation> control)
    {
        _control = control;
    }

    protected override void RedoAction(TOperation operation) => _control.ExecuteRedo(operation);
    protected override void UndoAction(TOperation operation) => _control.ExecuteUndo(operation);
    protected override void SetProgress(int steps) => _control.SetProgress(steps);
}
