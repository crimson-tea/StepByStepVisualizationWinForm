using System.Numerics;

namespace StepByStepVisualizationWinForm.Control1;

internal record struct State(string Text, BigInteger Value)
{
    public static State InitialState => new State() { Text = "", Value = 0 };
}

internal class Model
{
    public static State ChangeState(State state, Operation operation, bool toNextState) => (operation.OperationType, toNextState) switch
    {
        (OperationType.Append, true) => Append(state.Text, state.Value, operation.Number),
        (OperationType.Append, false) => Delete(state.Text, state.Value),
        (OperationType.Delete, true) => Delete(state.Text, state.Value),
        (OperationType.Delete, false) => Append(state.Text, state.Value, operation.Number),
        _ => throw new ArgumentException(nameof(operation.OperationType))
    };

    public static State Append(string text, BigInteger value, Number number) => new($"{text}{(int)number}", (value << 1) + (int)number);
    public static State Delete(string text, BigInteger value) => new(text[..^1], value >> 1);

    internal static bool CanDelete(State state) => string.IsNullOrWhiteSpace(state.Text) is false;
    internal static bool CanAppendZero(State state) => string.IsNullOrWhiteSpace(state.Text) is false;

    internal static Number LastAppend(State state) => (state.Value & 1) == 0 ? Number.Zero : Number.One;

}

internal enum OperationType
{
    Append,
    Delete,
}

internal enum Number
{
    Zero,
    One,
}

internal record struct Operation(OperationType OperationType, Number Number);
