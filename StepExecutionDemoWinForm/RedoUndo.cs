using AnimationWinForm.Control3;
using System.Diagnostics;

namespace AnimationWinForm;

internal class RedoUndo<TOperation> where TOperation : class
{
    private Stack<TOperation> _undo;
    private Stack<TOperation> _redo;
    private Action<Operation>? executeRedo;
    private Action<Operation>? executeUndo;
    private Action<int>? _setProgress;

    public RedoUndo(Action<TOperation> redo, Action<TOperation> undo, Action<int> setProgress = null, int initCapacity = 65535)
    {
        _undo = new Stack<TOperation>(initCapacity);
        _redo = new Stack<TOperation>(initCapacity);
        UndoAction = undo;
        RedoAction = redo;
        _setProgress = setProgress;
        InitCapacity = initCapacity;
    }

    public Action<TOperation> UndoAction { get; }
    public Action<TOperation> RedoAction { get; }
    public int InitCapacity { get; }
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public int Step => _undo.Count;

    public void Execute(TOperation op)
    {
        _redo.Clear();
        _undo.Push(op);
        RedoAction(op);
        _setProgress?.Invoke(_undo.Count);
    }

    /// <summary>
    /// 
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
        RedoAction(op);

        _setProgress?.Invoke(_undo.Count);
        Debug.WriteLine(op.ToString());
        return true;
    }

    /// <summary>
    /// 
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
        UndoAction(op);

        _setProgress?.Invoke(_undo.Count);
        Debug.WriteLine(op.ToString());
        return true;
    }

    internal void Reset()
    {
        _redo.Clear();
        _undo.Clear();
    }
}
