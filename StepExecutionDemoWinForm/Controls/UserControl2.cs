using AnimationWinForm.Control2;
using System.Diagnostics;

namespace AnimationWinForm;

public partial class UserControl2 : UserControl
{
    public UserControl2()
    {
        InitializeComponent();
        RedoUndo = new RedoUndo<BSOperation>(ExecuteRedo, ExecuteUndo);
    }

    public Image CreateBarImage(Size size, Brush brush)
    {
        var (width, height) = size;
        Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using Graphics g = Graphics.FromImage(bitmap);
        g.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        return bitmap;
    }

    List<PictureBox> Bars { get; } = new List<PictureBox>();
    List<int> Values { get; } = new List<int>();

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
            Values.Add(value);
            picture.Location = new Point(i * scale, 0);
            picture.Image = CreateBarImage(picture.Size, DefaultBrush);
            Bars.Add(picture);
            value += rand.Next(2);
        }

        var maxHeight = Bars.Last().Height + 10;
        Bars.ForEach(picture =>
        {
            picture.Location = new Point(picture.Location.X, maxHeight - picture.Height);
            Debug.WriteLine($"height: {picture.Height} loc: {picture.Location}");
        });

        SuspendLayout();
        Controls.AddRange(Bars.ToArray());
        ResumeLayout();
    }

    readonly BSModel _model = new BSModel();
    RedoUndo<BSOperation> RedoUndo { get; }

    IEnumerator<BSOperation> Enumerator { get; set; }
    public Brush DefaultBrush { get; } = Brushes.White;
    public Brush OutOfRangeBrush { get; } = Brushes.Gray;

    private void NextButton_Click(object sender, EventArgs e)
    {
        const int TARGET = 26;
        if (RedoUndo.Redo())
        {
            return;
        }

        Enumerator ??= _model.BinarySearch(Values.ToArray(), TARGET);

        if (Enumerator.MoveNext())
        {
            var op = Enumerator.Current;
            RedoUndo.Execute(op);
        }
    }

    private void PrevButton_Click(object sender, EventArgs e)
    {
        if (RedoUndo.Undo())
        {
            return;
        }
    }

    private void ExecuteRedo(BSOperation op) => Execute(op);

    private void ExecuteUndo(BSOperation op) => Execute(op, true);

    private void Execute(BSOperation current, bool isUndo = false)
    {
        var (type, fIndex, tIndex) = current;

        Debug.WriteLine(current);

        switch (type)
        {
            case BSOperationType.None:
                break;
            case BSOperationType.Complete:
                ResultLabel.Text = $"Result: {tIndex}";
                break;
            case BSOperationType.MoveLeft:
                SetColor(Brushes.Red);
                break;
            case BSOperationType.MoveRight:
                SetColor(Brushes.Green);
                break;
            default:
                break;
        }

        void SetColor(Brush brush)
        {
            var fillBrush = isUndo ? DefaultBrush : OutOfRangeBrush;

            int start = Math.Max(0, Math.Min(fIndex, tIndex));
            int end = Math.Min(Bars.Count - 1, Math.Max(fIndex, tIndex));

            SuspendLayout();
            for (int i = start; i <= end; i++)
            {
                var bar = Bars[i];
                bar.Image = CreateBarImage(bar.Size, fillBrush);
            }
            ResumeLayout();

            if (isUndo)
            {
                if (0 <= fIndex && fIndex < Bars.Count)
                {
                    var from = Bars[fIndex];
                    from.Image = CreateBarImage(from.Size, brush);
                }
            }
            else
            {
                if (0 <= tIndex && tIndex < Bars.Count)
                {
                    var to = Bars[tIndex];
                    to.Image = CreateBarImage(to.Size, brush);
                }
            }
        }
    }
}

public static class Extensions
{
    public static void Deconstruct(this Size size, out int width, out int height) => (width, height) = (size.Width, size.Height);
}
