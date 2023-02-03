using System.Numerics;

namespace StepByStepVisualizationWinForm.Control1;

internal record struct Model(string Text, BigInteger Value)
{
    public static Model InitialState => new Model() { Text = "", Value = 0 };
}

internal class BinaryToDecimalConverter
{
    public static Model ChangeState(Model model, Operation operation, bool toNextState) => toNextState switch
    {
        true => NextState(model, operation),
        false => PrevState(model, operation),
    };

    public static Model NextState(Model model, Operation operation)
    {
        var (text, value) = model;
        var (op, number) = operation;

        var (nextText, nextValue) = op switch
        {
            OperationType.Append => ($"{text}{(int)number}", (value << 1) + (int)number),
            OperationType.Delete => (text[..^1], value >> 1),
            _ => throw new ArgumentException(nameof(operation)),
        };

        return new(nextText, nextValue);
    }

    public static Model PrevState(Model model, Operation operation)
    {
        var (text, value) = model;
        var (op, number) = operation;

        var (nextText, nextValue) = op switch
        {
            OperationType.Append => (text[..^1], value >> 1),
            OperationType.Delete => ($"{text}{(int)number}", (value << 1) + (int)number),
            _ => throw new ArgumentException(nameof(operation)),
        };

        return new(nextText, nextValue);
    }

    internal static bool CanDelete(Model model) => string.IsNullOrWhiteSpace(model.Text) is false;

    internal static AppendNumber LastAppend(Model model) => (model.Value & 1) == 0 ? AppendNumber.Zero : AppendNumber.One;
}

internal enum OperationType
{
    Append,
    Delete,
}

internal enum AppendNumber
{
    Zero,
    One,
}

internal record struct Operation(OperationType OperationType, AppendNumber Number);
