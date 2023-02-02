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
            OperationType.Append => (text + number, (value << 1) + int.Parse(number.ToString())),
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
            OperationType.Delete => (text + number, (value << 1) + int.Parse(number.ToString())),
            _ => throw new ArgumentException(nameof(operation)),
        };

        return new(nextText, nextValue);
    }
}

internal enum OperationType
{
    Append,
    Delete,
}

internal record struct Operation(OperationType OperationType, char Char);

