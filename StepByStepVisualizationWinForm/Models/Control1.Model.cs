using System.Numerics;

namespace StepByStepVisualizationWinForm.Control1;

internal record struct Model(string Text, BigInteger Value)
{
    public static Model InitialState => new Model() { Text = "", Value = 0 };
}

internal class BinaryToDecimalConverter
{
    public static Model ChangeState(Model model, Operation operation, bool toNextState) => (operation.OperationType, toNextState) switch
    {
        (OperationType.Append, true) => Append(model.Text, model.Value, operation.Number),
        (OperationType.Append, false) => Delete(model.Text, model.Value),
        (OperationType.Delete, true) => Delete(model.Text, model.Value),
        (OperationType.Delete, false) => Append(model.Text, model.Value, operation.Number),
        _ => throw new ArgumentException(nameof(operation.OperationType))
    };

    public static Model Append(string text, BigInteger value, Number number) => new($"{text}{(int)number}", (value << 1) + (int)number);
    public static Model Delete(string text, BigInteger value) => new(text[..^1], value >> 1);

    internal static bool CanDelete(Model model) => string.IsNullOrWhiteSpace(model.Text) is false;
    internal static bool CanAppendZero(Model model) => string.IsNullOrWhiteSpace(model.Text) is false;

    internal static Number LastAppend(Model model) => (model.Value & 1) == 0 ? Number.Zero : Number.One;

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
