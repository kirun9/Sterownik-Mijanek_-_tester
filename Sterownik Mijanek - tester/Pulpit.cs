namespace Sterownik_Mijanek___tester;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using static Sterownik_Mijanek___tester.ArduinoCode;

public partial class Pulpit : Control
{
    private JunctionState Junction1State = JunctionState.Left;
    private JunctionState Junction2State = JunctionState.Left;

    public bool DetNext { get; set; }
    public bool DetNext2 { get; set; }
    public bool Det_1_1 { get; set; }
    public bool Det_2_1 { get; set; }
    public bool Det_3_1 { get; set; }
    public bool Det_1_2 { get; set; }
    public bool Det_2_2 { get; set; }
    public bool Det_3_2 { get; set; }

    public bool DetOut { get; set; }
    public bool Junction1L { get; set; }
    public bool Junction1R { get; set; }
    public bool Junction2L { get; set; }
    public bool Junction2R { get; set; }
    public bool Track1 { get; set; }
    public bool Track2 { get; set; }
    public bool Track3 { get; set; }

    public bool Track1Selected => sterownikMijanek.actualSelectedTrack == 1;
    public bool Track2Selected => sterownikMijanek.actualSelectedTrack == 2;
    public bool Track3Selected => sterownikMijanek.actualSelectedTrack == 3;

    public bool Track1Enabled => sterownikMijanek.track1Enable;
    public bool Track2Enabled => sterownikMijanek.track2Enable;
    public bool Track3Enabled => sterownikMijanek.track3Enable;


    public bool TrackInDet { get; set; }

    private SterownikMijanekCode2 sterownikMijanek;

    public Pulpit() : this(null) { }

    public Pulpit(IContainer container)
    {
        container.Add(this);
        this.DoubleBuffered = true;
        InitializeComponent();
        sterownikMijanek = new SterownikMijanekCode2();
        sterownikMijanek.DigitalUpdate += DigitalPinUpdate;
        sterownikMijanek.DataShiftedOut += DataShiftedOut;
        sterownikMijanek.OnLoopRun += (_, _) => InvokeInvalidate();
        sterownikMijanek.StartExecution();
    }

    public void InvokeInvalidate()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(Invalidate));
        }
        else
        {
            Invalidate();
        }
    }

    public void SetData(Expression<Func<bool>> property, bool value)
    {
        if (InvokeRequired)
        {
            this.Invoke(new Action<Expression<Func<bool>>, bool>(SetData), property, value);
            return;
        }

        var body = (MemberExpression) property.Body;
        var propertyInfo = (PropertyInfo) body.Member;
        propertyInfo.SetValue(this, value);
    }

    private void DataShiftedOut(object? _, int value)
    {
        bool IsBitSet(int pos) => ((value >> (pos)) & 1) == 1;

        DetOut = !IsBitSet(SterownikMijanekCode2.DetOut); //false - LOW - Detected |||| Note: Inversion - because it is on board
        Junction1L = IsBitSet(SterownikMijanekCode2.Junction1L);
        Junction1R = IsBitSet(SterownikMijanekCode2.Junction1R);
        Junction2L = IsBitSet(SterownikMijanekCode2.Junction2L);
        Junction2R = IsBitSet(SterownikMijanekCode2.Junction2R);
        Track1 = IsBitSet(SterownikMijanekCode2.Track1);
        Track2 = IsBitSet(SterownikMijanekCode2.Track2);
        Track3 = IsBitSet(SterownikMijanekCode2.Track3);

        if (Junction1L)
            Junction1State = JunctionState.Left;
        else if (Junction1R)
            Junction1State = JunctionState.Right;
        if (Junction2L)
            Junction2State = JunctionState.Left;
        else if (Junction2R)
            Junction2State = JunctionState.Right;

        InvokeInvalidate();
    }

    private void DigitalPinUpdate(object? sender, EventArgs e)
    {
        if (sender is DigitalPin pin)
        {
            switch (pin.Number)
            {
                case SterownikMijanekCode2.Det_1:
                    SetData(() => Det_1_1, pin.Value == ArduinoCode.LOW);
                    break;
                case SterownikMijanekCode2.Det_2:
                    SetData(() => Det_2_1, pin.Value == ArduinoCode.LOW);
                    break;
                case SterownikMijanekCode2.Det_3:
                    SetData(() => Det_3_1, pin.Value == ArduinoCode.LOW);
                    break;
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButtons.Left)
            return;

        switch (e.Location)
        {
            // Track - entry
            case { X: >= 945 and <= 1095, Y: >= 130 and <= 150 }:
                Debug.WriteLine("Clicked on track - entry");
                TrackInDet = !TrackInDet;
                break;
            // Junctions
            case { X: >= 410 and <= 460, Y: >= 80 and <= 120 }:
            case { X: >= 840 and <= 880, Y: >= 120 and <= 160 }:
                Debug.WriteLine("Clicked on Junction1");
                if (Junction1State is JunctionState.Left)
                {
                    SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction1RButton, LOW);
                    Thread thread = new Thread(() =>
                    {
                        Thread.Sleep(2000);
                        SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction1RButton, HIGH);
                    });
                    thread.Start();
                }
                else if (Junction1State is JunctionState.Right)
                {
                    SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction1LButton, LOW);
                    Thread thread = new Thread(() =>
                    {
                        Thread.Sleep(2000);
                        SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction1LButton, HIGH);
                    });
                    thread.Start();
                }
                break;
            case { X: >= 380 and <= 420, Y: >= 40 and <= 80 }:
            case { X: >= 810 and <= 850, Y: >= 80 and <= 120 }:
                Debug.WriteLine("Clicked on Junction2");
                if (Junction2State is JunctionState.Left)
                {
                    SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction2RButton, LOW);
                    Thread thread = new Thread(() =>
                    {
                        Thread.Sleep(1000);
                        SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction2RButton, HIGH);
                    });
                    thread.Start();
                }
                else if (Junction2State is JunctionState.Right)
                {
                    SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction2LButton, LOW);
                    Thread thread = new Thread(() => {
                        Thread.Sleep(1000);
                        SterownikMijanekCode2.SETPIN(SterownikMijanekCode2.Junction2LButton, HIGH);
                    });
                    thread.Start();
                }
                break;
            case { X: >= 600 and <= 797, Y: >= 50 and <= 70 } when Track3Enabled:
                Debug.WriteLine("Clicked on Track 3_2");
                Det_3_2 = !Det_3_2;
                SETPIN(SterownikMijanekCode2.Det_3, (Det_3_2 || Det_3_1) ? ArduinoCode.LOW : ArduinoCode.HIGH);
                break;
            case { X: >= 600 and <= 797, Y: >= 90 and <= 110 } when Track2Enabled:
                Debug.WriteLine("Clicked on Track 2_2");
                Det_2_2 = !Det_2_2;
                SETPIN(SterownikMijanekCode2.Det_2, (Det_2_2 || Det_2_1) ? ArduinoCode.LOW : ArduinoCode.HIGH);
                break;
            case { X: >= 600 and <= 797, Y: >= 130 and <= 150 } when Track1Enabled:
                Debug.WriteLine("Clicked on Track 1_2");
                Det_1_2 = !Det_1_2;
                SETPIN(SterownikMijanekCode2.Det_1, (Det_1_2 || Det_1_1) ? ArduinoCode.LOW : ArduinoCode.HIGH);
                break;
            case { X: >= 505 and <= 597, Y: >= 50 and <= 70 } when Track3Enabled:
                Debug.WriteLine("Clicked on Track 3_1");
                Det_3_1 = !Det_3_1;
                SETPIN(SterownikMijanekCode2.Det_3, (Det_3_2 || Det_3_1) ? LOW : HIGH);
                break;
            case { X: >= 505 and <= 597, Y: >= 90 and <= 110 } when Track2Enabled:
                Debug.WriteLine("Clicked on Track 2_1");
                Det_2_1 = !Det_2_1;
                SETPIN(SterownikMijanekCode2.Det_2, (Det_2_2 || Det_2_1) ? LOW : HIGH);
                break;
            case { X: >= 505 and <= 597, Y: >= 130 and <= 150 } when Track1Enabled:
                Debug.WriteLine("Clicked on Track 1_1");
                Det_1_1 = !Det_1_1;
                SETPIN(SterownikMijanekCode2.Det_1, (Det_1_2 || Det_1_1) ? LOW : HIGH);
                break;
            case { X: >= 230 and <= 377, Y: >= 50 and <= 70 }:
                Debug.WriteLine("Clicked on Track Next1");
                DetNext = !DetNext;
                SETPIN(SterownikMijanekCode2.Det_Next, !DetNext);
                break;
            case { X: >= 165 and <= 227, Y: >= 50 and <= 70 }:
                Debug.WriteLine("Clicked on Track Next2");
                DetNext2 = !DetNext2;
                SETPIN(SterownikMijanekCode2.Det_Next_2, !DetNext2);
                break;
            case { X: >= 480 and <= 500, Y: >= 130 and <= 150 }:
                Debug.WriteLine("Clicked on Signal Track 1");
                SETPIN(SterownikMijanekCode2.Track1Button, Track1);
                break;
            case { X: >= 480 and <= 500, Y: >= 90 and <= 110 }:
                Debug.WriteLine("Clicked on Signal Track 1");
                SETPIN(SterownikMijanekCode2.Track2Button, Track2);
                break;
            case { X: >= 480 and <= 500, Y: >= 50 and <= 70 }:
                Debug.WriteLine("Clicked on Signal Track 1");
                SETPIN(SterownikMijanekCode2.Track3Button, Track3);
                break;


            case { X: >= 10 and <= 20, Y: >= 10 and <= 20 }:
                Debug.WriteLine("Clicked on Dummy Field");
                SETPIN(SterownikMijanekCode2.SW1_1, !sterownikMijanek.track1Enable);
                SETPIN(SterownikMijanekCode2.SW1_3, !sterownikMijanek.track3Enable);
                break;
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        using Pen pen = new Pen(Color.Gray, 3);
        using SolidBrush brush = new SolidBrush(Color.White);

        g.FillTriangle(brush, new Rectangle(60, 50, 20, 20), TriangleDirection.Left);

        pen.Color = Color.Green;
        g.DrawRectangle(pen, new Rectangle(10, 10, 10, 10));

        pen.Color = DetNext2 ? Color.Red : Color.Gray;
        g.DrawLine(pen, 85, 60, 135, 60);
        pen.Color = Color.White;
        g.DrawOpenTriangleWithDot(pen, new Rectangle(140, 50, 20, 20), TriangleDirection.Left);

        pen.Color = DetNext2 ? Color.Red : Color.Gray;
        g.DrawLine(pen, 165, 60, 227, 60);

        pen.Color = DetNext ? Color.Red : Color.Gray;
        g.DrawLine(pen, 230, 60, 377, 60);

        //głowica wyjazdowa
        pen.Color = Track1Enabled || Track2Enabled || Track3Enabled ? (Track1Selected || Track2Selected || Track3Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 380, 60, 390, 60);
        pen.Color = Track1Enabled || Track2Enabled ? (Track1Selected || Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 410, 60, 420, 60);
        pen.Color = Track1Enabled || Track2Enabled ? (Track1Selected || Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 400, 70, 400, 90);

        pen.Color = Color.Gray;
        switch (Junction2State)
        {
            case JunctionState.Right:
                pen.Color = Track3Enabled ? (Track3Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 390, 60, 410, 60);
                break;
            case JunctionState.Left:
                pen.Color = Track1Enabled || Track2Enabled ? (Track1Selected || Track2Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 390, 60, 400, 70);
                break;
        }
        pen.Color = Track1Enabled || Track2Enabled ? (Track1Selected || Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 400, 90, 410, 100);

        pen.Color = Track1Enabled || Track2Enabled ? (Track1Selected || Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 410, 100, 420, 100);
        pen.Color = Track2Enabled ? (Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 440, 100, 450, 100);
        pen.Color = Track1Enabled ? (Track1Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 430, 110, 430, 130);
        switch (Junction1State)
        {
            case JunctionState.Right:
                pen.Color = Track2Enabled ? (Track2Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 420, 100, 440, 100);
                break;
            case JunctionState.Left:
                pen.Color = Track1Enabled ? (Track1Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 420, 100, 430, 110);
                break;
        }
        pen.Color = Track1Enabled ? (Track1Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 430, 130, 440, 140);

        // tory odstawcze
        pen.Color = Track3Enabled ? (Track3Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 410, 60, 475, 60);
        pen.Color = Track2Enabled ? (Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 450, 100, 475, 100);
        pen.Color = Track1Enabled ? (Track1Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 440, 140, 475, 140);

        brush.Color = Track3 ? Color.Green : Color.Gray;
        g.FillTriangle(brush, new Rectangle(480, 50, 20, 20), TriangleDirection.Left);
        brush.Color = Track2 ? Color.Green : Color.Gray;
        g.FillTriangle(brush, new Rectangle(480, 90, 20, 20), TriangleDirection.Left);
        brush.Color = Track1 ? Color.Green : Color.Gray;
        g.FillTriangle(brush, new Rectangle(480, 130, 20, 20), TriangleDirection.Left);

        pen.Color = Track3Enabled ? (Det_3_1 ? Color.Red : (Track3Selected ? Color.Green : Color.Gray)) : Color.White;
        g.DrawLine(pen, 505, 60, 597, 60);
        pen.Color = Track2Enabled ? (Det_2_1 ? Color.Red : (Track2Selected ? Color.Green : Color.Gray)) : Color.White;
        g.DrawLine(pen, 505, 100, 597, 100);
        pen.Color = Track1Enabled ? (Det_1_1 ? Color.Red : (Track1Selected ? Color.Green : Color.Gray)) : Color.White;
        g.DrawLine(pen, 505, 140, 597, 140);

        pen.Color = Track3Enabled ? (Det_3_2 ? Color.Red : (Track3Selected ? Color.Green : Color.Gray)) : Color.White;
        g.DrawLine(pen, 600, 60, 797, 60);
        pen.Color = Track2Enabled ? (Det_2_2 ? Color.Red : (Track2Selected ? Color.Green : Color.Gray)) : Color.White;
        g.DrawLine(pen, 600, 100, 797, 100);
        pen.Color = Track1Enabled ? (Det_1_2 ? Color.Red : (Track1Selected ? Color.Green : Color.Gray)) : Color.White;
        g.DrawLine(pen, 600, 140, 797, 140);

        pen.Color = Color.Gray;
        pen.Color = Track3Enabled ? (Track3Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 800, 60, 820, 60);
        pen.Color = Track2Enabled ? (Track2Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 800, 100, 820, 100);
        pen.Color = Track1Enabled ? (Track1Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 800, 140, 850, 140);

        pen.Color = Track3Enabled ? (Track3Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 820, 60, 830, 70);
        g.DrawLine(pen, 830, 70, 830, 90);
        pen.Color = Track2Enabled || Track3Enabled ? (Track2Selected || Track3Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 840, 100, 850, 100);
        g.DrawLine(pen, 850, 100, 860, 110);
        g.DrawLine(pen, 860, 110, 860, 130);
        switch (Junction2State)
        {
            case JunctionState.Right:
                pen.Color = Track3Enabled ? (Track3Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 830, 90, 840, 100);
                break;
            case JunctionState.Left:
                pen.Color = Track2Enabled ? (Track2Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 820, 100, 840, 100);
                break;
        }

        switch (Junction1State)
        {
            case JunctionState.Right:
                pen.Color = Track2Enabled || Track3Enabled ? (Track2Selected || Track3Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 860, 130, 870, 140);
                break;
            case JunctionState.Left:
                pen.Color = Track1Enabled ? (Track1Selected ? Color.Green : Color.Gray) : Color.White;
                g.DrawLine(pen, 850, 140, 870, 140);
                break;
        }
        pen.Color = Track1Enabled || Track2Enabled || Track3Enabled ? (Track1Selected || Track2Selected || Track3Selected ? Color.Green : Color.Gray) : Color.White;
        g.DrawLine(pen, 870, 140, 915, 140);

        brush.Color = !DetOut ? Color.Green : Color.Gray;
        g.FillTriangle(brush, new Rectangle(920, 130, 20, 20), TriangleDirection.Left);

        pen.Color = TrackInDet ? Color.Red : Color.Gray;
        g.DrawLine(pen, 945, 140, 1095, 140);

        brush.Color = Color.White;
        g.FillTriangle(brush, new Rectangle(1100, 130, 20, 20), TriangleDirection.Left);

        pen.Color = Color.Yellow;
        using Font font = new Font("Arial", 10);
        var textSize = g.MeasureString(sterownikMijanek.status.ToString(), font);
        g.DrawString(sterownikMijanek.status.ToString(), new Font("Arial", 10), pen.Brush, new PointF(550 - (textSize.Width / 2), 20 - (textSize.Height / 2)));

        //
        var textBox = g.MeasureString(SterownikMijanekCode2.Status.WAITING_FOR_CLEAR_TRACK.ToString(), font);
        pen.Width = 1f;
        pen.Color = Color.White;
        g.DrawRectangle(pen, new Rectangle(550 - (int)(textBox.Width / 2) - 3, 20 - (int)(textBox.Height / 2) - 3, (int) textBox.Width - 6, (int) textBox.Height + 6));
        //
        string s = $"Serial Data:\n" +
            $"DetOut: {DetOut.ToHighLow()}\n" +
            $"Junction1L: {Junction1L.ToHighLow()}\n" +
            $"Junction1R: {Junction1R.ToHighLow()}\n" +
            $"Junction2L: {Junction2L.ToHighLow()}\n" +
            $"Junction2R: {Junction2R.ToHighLow()}\n" +
            $"Track1: {Track1.ToHighLow()}\n" +
            $"Track2: {Track2.ToHighLow()}\n" +
            $"Track3: {Track3.ToHighLow()}\n";
        g.DrawString(s, font, brush, new PointF(30, 180));

        s = $"Sterownik:\n" +
            $"actualSelectedTrack: {sterownikMijanek.actualSelectedTrack}\n" +
            $"Junction 1 Lock: {sterownikMijanek.j1Lock}\n" +
            $"Junction 2 Lock: {sterownikMijanek.j2Lock}\n" +
            $"Detected: {sterownikMijanek.detected.ToHighLow()}\n" +
            $"Det_1: {sterownikMijanek.det_1.ToHighLow()}\n" +
            $"Det_2: {sterownikMijanek.det_2.ToHighLow()}\n" +
            $"Det_3: {sterownikMijanek.det_3.ToHighLow()}\n" +
            $"DetNext: {sterownikMijanek.detNext.ToHighLow()}\n" +
            $"DetNext2: {sterownikMijanek.detNext2.ToHighLow()}\n";
        g.DrawString(s, font, brush, new PointF(200, 180));

        s = $"Interfejs:\n" +
            $"Junction 1 State: {Junction1State}\n" +
            $"Junction 2 State: {Junction2State}\n" +
            $"DetOut: {(!DetOut).ToHighLow()}";
        g.DrawString(s, font, brush, new PointF(400, 180));
    }
}
