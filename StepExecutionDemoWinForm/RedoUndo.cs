using System.Diagnostics;

namespace AnimationWinForm;

internal class RedoUndo<TOperation> where TOperation : class
{
    private Stack<TOperation> _undo;
    private Stack<TOperation> _redo;
    private Action<TOperation>? _executeRedo;
    private Action<TOperation>? _executeUndo;
    private Action<int>? _setProgress;

    /// <summary>
    /// RedoUndoを実装します。
    /// </summary>
    /// <param name="redo">Redoで行われるアクションを指定します。</param>
    /// <param name="undo">Undoで行われるアクションを指定します。</param>
    /// <param name="setProgress">ステップ数を受け取りたい場合にアクションを指定します。</param>
    /// <param name="initCapacity">RedoとUndoに使われるスタックの大きさを指定します。RedoとUndoそれぞれこのキャパシティで初期化されます。</param>
    public RedoUndo(Action<TOperation> redo, Action<TOperation> undo, Action<int> setProgress = null, int initCapacity = 65535)
    {
        _redo = new Stack<TOperation>(initCapacity);
        _undo = new Stack<TOperation>(initCapacity);
        _executeRedo = redo;
        _executeUndo = undo;
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
        _executeRedo?.Invoke(op);
        _setProgress?.Invoke(_undo.Count);
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
        _executeRedo?.Invoke(op);

        _setProgress?.Invoke(_undo.Count);
        Debug.WriteLine(op.ToString());
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
        _executeUndo?.Invoke(op);

        _setProgress?.Invoke(_undo.Count);
        Debug.WriteLine(op.ToString());
        return true;
    }

    /// <summary>
    /// RedoUndoをリセットします。
    /// </summary>
    internal void Reset()
    {
        _redo.Clear();
        _undo.Clear();
    }
}
