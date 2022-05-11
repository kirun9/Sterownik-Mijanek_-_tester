namespace Sterownik_Mijanek___tester;

public static class GraphicExtensions
{
    public static void DrawTriangle(this Graphics g, Pen pen, Point p1, Point p2, Point p3)
    {
        g.DrawLine(pen, p1, p2);
        g.DrawLine(pen, p2, p3);
        g.DrawLine(pen, p3, p1);
    }

    public static void FillTriangle(this Graphics g, Brush brush, Point p1, Point p2, Point p3)
    {
        var points = new Point[] { p1, p2, p3 };
        g.FillPolygon(brush, points);
    }

    public static void DrawDoubleTriangle(this Graphics g, Pen pen, Rectangle rect, TriangleDirection direction)
    {
        var p1 = new Point(0, 0);
        var p2 = new Point(0, 0);
        var p3 = new Point(0, 0);
        var p4 = new Point(0, 0);
        var p5 = new Point(0, 0);
        var p6 = new Point(0, 0);

        switch (direction)
        {
            case TriangleDirection.Up:
                p1 = new Point(rect.Left, rect.Bottom);
                p2 = new Point(rect.Left + rect.Width / 2, rect.Bottom - rect.Height / 2);
                p3 = new Point(rect.Right, rect.Bottom);
                p4 = new Point(rect.Left, rect.Bottom - rect.Height / 2);
                p5 = new Point(rect.Left + rect.Width / 2, rect.Top);
                p6 = new Point(rect.Right, rect.Bottom - rect.Height / 2);
                break;
            case TriangleDirection.Left:
                p1 = new Point(rect.Right, rect.Top);
                p2 = new Point(rect.Right - rect.Width / 2, rect.Top + rect.Height / 2);
                p3 = new Point(rect.Right, rect.Bottom);
                p4 = new Point(rect.Right - rect.Width / 2, rect.Top);
                p5 = new Point(rect.Left, rect.Top + rect.Height / 2);
                p6 = new Point(rect.Right - rect.Width / 2, rect.Bottom);
                break;
            case TriangleDirection.Right:
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                p3 = new Point(rect.Left, rect.Bottom);
                p4 = new Point(rect.Left + rect.Width / 2, rect.Top);
                p5 = new Point(rect.Right, rect.Top + rect.Height / 2);
                p6 = new Point(rect.Left + rect.Width / 2, rect.Bottom);
                break;
            case TriangleDirection.Down:
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                p3 = new Point(rect.Right, rect.Top);
                p4 = new Point(rect.Left, rect.Top + rect.Height / 2);
                p5 = new Point(rect.Left + rect.Width / 2, rect.Bottom);
                p6 = new Point(rect.Right, rect.Top + rect.Height / 2);
                break;
        }
        Pen p = pen.Clone() as Pen;
        p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        p.StartCap = System.Drawing.Drawing2D.LineCap.Round;

        g.DrawLine(p, p1, p2);
        g.DrawLine(p, p2, p3);
        g.DrawLine(p, p4, p5);
        g.DrawLine(p, p5, p6);
    }

    //draw triangle in rectangle with direction
    public static void DrawTriangle(this Graphics g, Pen pen, Rectangle rect, TriangleDirection direction)
    {
        var p1 = new Point(rect.Left, rect.Top);
        var p2 = new Point(rect.Right, rect.Top);
        var p3 = new Point(rect.Left + rect.Width / 2, rect.Bottom);

        switch (direction)
        {
            case TriangleDirection.Up:
                p1 = new Point(rect.Left, rect.Bottom);
                p2 = new Point(rect.Right, rect.Bottom);
                p3 = new Point(rect.Left + rect.Width / 2, rect.Top);
                break;
            case TriangleDirection.Down:
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Right, rect.Top);
                p3 = new Point(rect.Left + rect.Width / 2, rect.Bottom);
                break;
            case TriangleDirection.Left:
                p1 = new Point(rect.Right, rect.Top);
                p2 = new Point(rect.Right, rect.Bottom);
                p3 = new Point(rect.Left, rect.Top + rect.Height / 2);
                break;
            case TriangleDirection.Right:
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Left, rect.Bottom);
                p3 = new Point(rect.Right, rect.Top + rect.Height / 2);
                break;
        }

        g.DrawTriangle(pen, p1, p2, p3);
    }

    public static void DrawTriangle(this Graphics g, Pen pen, Point p1, Point p2, TriangleDirection direction)
    {
        var rect = new Rectangle(p1, new Size(p2.X - p1.X, p2.Y - p1.Y));
        g.DrawTriangle(pen, rect, direction);
    }

    public static void FillTriangle(this Graphics g, Brush brush, Rectangle rect, TriangleDirection direction)
    {
        var p1 = new Point(rect.Left, rect.Top);
        var p2 = new Point(rect.Right, rect.Top);
        var p3 = new Point(rect.Left + rect.Width / 2, rect.Bottom);

        switch (direction)
        {
            case TriangleDirection.Up:
                p1 = new Point(rect.Left, rect.Bottom);
                p2 = new Point(rect.Right, rect.Bottom);
                p3 = new Point(rect.Left + rect.Width / 2, rect.Top);
                break;
            case TriangleDirection.Down:
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Right, rect.Top);
                p3 = new Point(rect.Left + rect.Width / 2, rect.Bottom);
                break;
            case TriangleDirection.Left:
                p1 = new Point(rect.Right, rect.Top);
                p2 = new Point(rect.Right, rect.Bottom);
                p3 = new Point(rect.Left, rect.Top + rect.Height / 2);
                break;
            case TriangleDirection.Right:
                p1 = new Point(rect.Left, rect.Top);
                p2 = new Point(rect.Left, rect.Bottom);
                p3 = new Point(rect.Right, rect.Top + rect.Height / 2);
                break;
        }

        g.FillTriangle(brush, p1, p2, p3);
    }

    public static void FillTriangle(this Graphics g, Brush brush, Point p1, Point p2, TriangleDirection direction)
    {
        var rect = new Rectangle(p1, new Size(p2.X - p1.X, p2.Y - p1.Y));
        g.FillTriangle(brush, rect, direction);
    }

    public static void DrawOpenTriangleWithDot(this Graphics g, Pen pen, Rectangle rect, TriangleDirection direction)
    {
        var p1 = new Point(0, 0);
        var p2 = new Point(0, 0);
        var p3 = new Point(0, 0);
        Point center = Point.Empty;
        Point center2 = Point.Empty;
        switch (direction)
        {
            case TriangleDirection.Up:
                p1 = new Point(rect.Left, rect.Bottom - rect.Height / 3);
                p2 = new Point(rect.Left + rect.Width / 2, rect.Bottom - rect.Height / 2 - rect.Height / 3);
                p3 = new Point(rect.Right, rect.Bottom - rect.Height / 3);
                center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 + rect.Height / 3);
                center2 = new Point(center.X, center.Y - 1);
                break;
            case TriangleDirection.Down:
                p1 = new Point(rect.Left, rect.Top + rect.Height / 3);
                p2 = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 + rect.Height / 3);
                p3 = new Point(rect.Right, rect.Top + rect.Height / 3);
                center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 - rect.Height / 3);
                center2 = new Point(center.X, center.Y + 1);
                break;
            case TriangleDirection.Left:
                p1 = new Point(rect.Right - rect.Width / 3, rect.Top);
                p2 = new Point(rect.Right - rect.Width / 2 - rect.Width / 3, rect.Top + rect.Height / 2);
                p3 = new Point(rect.Right - rect.Width / 3, rect.Bottom);
                center = new Point(rect.Left + rect.Width / 2 + rect.Width / 3, rect.Top + rect.Height / 2);
                center2 = new Point(center.X - 1, center.Y);
                break;
            case TriangleDirection.Right:
                p1 = new Point(rect.Left + rect.Width / 3, rect.Top);
                p2 = new Point(rect.Left + rect.Width / 2 + rect.Height / 3, rect.Top + rect.Height / 2);
                p3 = new Point(rect.Left + rect.Width / 3, rect.Bottom);
                center = new Point(rect.Left + rect.Width / 2 - rect.Height / 3, rect.Top + rect.Height / 2);
                center2 = new Point(center.X + 1, center.Y);
                break;
        }

        using Pen p = pen.Clone() as Pen;
        p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        p.StartCap = System.Drawing.Drawing2D.LineCap.Round;

        g.DrawLine(p, p1, p2);
        g.DrawLine(p, p2, p3);
        g.DrawLine(p, center, center2);
    }
}
