using StepByStepVisualizationWinForm.Control2;
using System.Diagnostics;

namespace StepByStepVisualizationWinForm;

public partial class UserControl2 : UserControl, IRedoUndo<Operation>
{
    public UserControl2()
    {
        InitializeComponent();
        _redoUndo = new RedoUndo<Operation>(this);
    }

    public Image CreateBarImage(Size size, Brush brush)
    {
        var (width, height) = size;
        Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using Graphics g = Graphics.FromImage(bitmap);
        g.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        return bitmap;
    }

    private readonly List<(PictureBox bar, int value)> Bars = new();

    private void UserControl2_Load(object sender, EventArgs e)
    {
        const int scale = 3;
        const int length = 128;

        Random rand = new Random(100001);
        int value = 1;
        for (int i = 1; i <= length; i++)
        {
            var picture = new PictureBox();
            picture.Size = new Size(scale, value * scale);
            picture.Location = new Point(i * scale, 0);
            picture.Image = CreateBarImage(picture.Size, _defaultBrush);
            Bars.Add((picture, value));
            value += rand.Next(2);
        }

        var pictures = Bars.Select(x => x.bar).ToArray();
        var maxHeight = pictures.Last().Height + 10;

        foreach (var picture in pictures)
        {
            picture.Location = new Point(picture.Location.X, maxHeight - picture.Height);
            Debug.WriteLine($"height: {picture.Height} loc: {picture.Location}");
        }

        SuspendLayout();
        Controls.AddRange(pictures);
        ResumeLayout();
    }

    private readonly Model _model = new Model();
    private readonly RedoUndo<Operation> _redoUndo;

    private IEnumerator<Operation> _enumerator;
    private IEnumerator<Operation> Enumerator => _enumerator ??= _model.BinarySearch(Bars.Select(x => x.value).ToArray(), TARGET);

    private readonly Brush _defaultBrush = Brushes.White;
    private readonly Brush _outOfRangeBrush = Brushes.Gray;

    private const int TARGET = 26;

    private void NextButton_Click(object sender, EventArgs e)
    {
        if (_redoUndo.Redo())
        {
            return;
        }

        if (Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            _redoUndo.Execute(op);
        }
    }

    private void PrevButton_Click(object sender, EventArgs e)
    {
        if (_redoUndo.Undo())
        {
            return;
        }
    }

    private void ExecuteRedo(Operation op) => Execute(op);

    private void ExecuteUndo(Operation op) => Execute(op, true);

    private void Execute(Operation current, bool isUndo = false)
    {
        var (type, fIndex, tIndex) = current;

        Debug.WriteLine(current);

        switch (type)
        {
            case OperationType.None:
                break;
            case OperationType.Complete:
                ResultLabel.Text = $"Result: {tIndex}";
                break;
            case OperationType.MoveLeft:
                SetColor(Brushes.Red);
                break;
            case OperationType.MoveRight:
                SetColor(Brushes.Green);
                break;
            default:
                break;
        }

        void SetColor(Brush brush)
        {
            var fillBrush = isUndo ? _defaultBrush : _outOfRangeBrush;

            int start = Math.Max(0, Math.Min(fIndex, tIndex));
            int end = Math.Min(Bars.Count - 1, Math.Max(fIndex, tIndex));

            SuspendLayout();
            for (int i = start; i <= end; i++)
            {
                var bar = Bars[i].bar;
                bar.Image = CreateBarImage(bar.Size, fillBrush);
            }
            ResumeLayout();

            if (isUndo)
            {
                if (0 <= fIndex && fIndex < Bars.Count)
                {
                    var from = Bars[fIndex].bar;
                    from.Image = CreateBarImage(from.Size, brush);
                }
            }
            else
            {
                if (0 <= tIndex && tIndex < Bars.Count)
                {
                    var to = Bars[tIndex].bar;
                    to.Image = CreateBarImage(to.Size, brush);
                }
            }
        }
    }

    void IRedoUndo<Operation>.ExecuteRedo(Operation operation) => ExecuteRedo(operation);
    void IRedoUndo<Operation>.ExecuteUndo(Operation operation) => ExecuteUndo(operation);
    void IRedoUndo<Operation>.SetProgress(int step) { }
}

public static class Extensions
{
    public static void Deconstruct(this Size size, out int width, out int height) => (width, height) = (size.Width, size.Height);
}
