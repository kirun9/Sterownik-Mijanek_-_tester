namespace Sterownik_Mijanek___tester;
using System;

internal class ArduinoCode
{
    public event EventHandler DigitalUpdate;
    public event EventHandler AnalogUpdate;
    public event EventHandler OnLoopRun;
    public event EventHandler<int> DataShiftedOut;

    internal class Pin<T>
    {
        public int Number { get; }
        public string Name { get; }
        public PinModeEnum Mode { get; set; }
        public T Value { get; set; }
        public string Comment { get; set; }

        public Pin(int pinNumber, string pinName)
        {
            Number = pinNumber;
            Name = pinName;
        }
    }

    internal class DigitalPin : Pin<int>
    {
        private static DigitalPin[] pins = { new DigitalPin(0, "D0"), new DigitalPin(1, "D1"), new DigitalPin(2, "D2"), new DigitalPin(3, "D3"), new DigitalPin(4, "D4"), new DigitalPin(5, "D5"), new DigitalPin(6, "D6"), new DigitalPin(7, "D7"), new DigitalPin(8, "D8"), new DigitalPin(9, "D9"), new DigitalPin(10, "D10"), new DigitalPin(11, "D11"), new DigitalPin(12, "D12"), new DigitalPin(13, "D13") };

        public static readonly DigitalPin DoNotUse = new DigitalPin(-1, "DoNotUse");

        public DigitalPin(int pinNumber, string pinName) : base(pinNumber, pinName)
        {
            if (Number is -1)
                return;
            if (pinNumber is < 0 or > 13)
                throw new Exception("Pin number must be between 0 and 13");
        }

        public DigitalPin this[int pinNumber]
        {
            get
            {
                if (pinNumber is >= 0 and <= 13)
                {
                    return pins[pinNumber];
                }
                throw new Exception("Pin number is out of range");
            }

            set
            {
                if (pinNumber is >= 0 and <= 13)
                {
                    pins[pinNumber] = value;
                }
                throw new Exception("Pin number is out of range");
            }
        }

        public DigitalPin this[DigitalPin pin]
        {
            get
            {
                return this[pin.Number];
            }

            set
            {
                pins[pin.Number] = value;
            }
        }

        public DigitalPin this[string pinName]
        {
            get
            {
                var pinIndex = Array.FindIndex(pins, p => p.Name == pinName);
                if (pinIndex == -1)
                    throw new Exception("Pin not found");
                return pins[pinIndex];
            }

            set
            {
                var pinIndex = Array.FindIndex(pins, p => p.Name == pinName);
                if (pinIndex == -1)
                    throw new Exception("Pin not found");
                pins[pinIndex] = value;
            }
        }
    }

    internal class AnalogPin : Pin<float>
    {
        private static AnalogPin[] pins = { new AnalogPin(14, "A0"), new AnalogPin(15, "A1"), new AnalogPin(16, "A2"), new AnalogPin(17, "A3"), new AnalogPin(18, "A4"), new AnalogPin(19, "A5"), new AnalogPin(20, "A6"), new AnalogPin(21, "A7") };

        public static readonly AnalogPin DoNotUse = new AnalogPin(-1, "DoNotUse");

        public AnalogPin(int pinNumber, string pinName) : base(pinNumber, pinName)
        {
            System.Diagnostics.Debug.WriteLine($"new AnalogPin({pinNumber}, {pinName})");
            if (pinNumber is -1)
                return;
            if (pinNumber is < 14 or > 21)
                throw new Exception("Pin number must be between 14 and 21");
        }

        public AnalogPin this[int pinNumber]
        {
            get
            {
                if (pinNumber is >= 14 and <= 21)
                {
                    return pins[pinNumber - 14];
                }
                throw new Exception("Pin number is out of range");
            }

            set
            {
                if (pinNumber is >= 14 and <= 21)
                {
                    pins[pinNumber - 14] = value;
                }
                throw new Exception("Pin number is out of range");
            }
        }

        public AnalogPin this[AnalogPin pin]
        {
            get
            {
                return this[pin.Number];
            }

            set
            {
                pins[pin.Number] = value;
            }
        }

        public AnalogPin this[string pinName]
        {
            get
            {
                var pinIndex = Array.FindIndex(pins, p => p.Name == pinName);
                if (pinIndex == -1)
                    throw new Exception("Pin not found");
                return pins[pinIndex];
            }

            set
            {
                var pinIndex = Array.FindIndex(pins, p => p.Name == pinName);
                if (pinIndex == -1)
                    throw new Exception("Pin not found");
                pins[pinIndex] = value;
            }
        }

        public int DigitalValue
        {
            get => Value >= (255f / 2f) ? 1 : 0;
        }
    }

    internal enum PinModeEnum { OUTPUT, INPUT, INPUT_PULLUP };

    public const PinModeEnum OUTPUT = PinModeEnum.OUTPUT;
    public const PinModeEnum INPUT = PinModeEnum.INPUT;
    public const PinModeEnum INPUT_PULLUP = PinModeEnum.INPUT_PULLUP;


    public const int MSBFIRST = 1;
    public const int LSBFIRST = 0;

    public const int LOW = 0;
    public const int HIGH = 1;

    public virtual bool DEBUG_MODE => false;

    public const int D0 = 0;
    public const int D1 = 1;
    public const int D2 = 2;
    public const int D3 = 3;
    public const int D4 = 4;
    public const int D5 = 5;
    public const int D6 = 6;
    public const int D7 = 7;
    public const int D8 = 8;
    public const int D9 = 9;
    public const int D10 = 10;
    public const int D11 = 11;
    public const int D12 = 12;
    public const int D13 = 13;
    public const int A0 = 14;
    public const int A1 = 15;
    public const int A2 = 16;
    public const int A3 = 17;
    public const int A4 = 18;
    public const int A5 = 19;
    public const int A6 = 20;
    public const int A7 = 21;

    #region Main

    private Thread thread;

    public ArduinoCode()
    {
        PreInit();
        thread = new Thread(() =>
        {
            setup();
            while (true)
            {
                loop();
                OnLoopRun?.Invoke(null, new EventArgs());
            }
        });
    }

    public void StartExecution()
    {
        thread.Start();
    }

    protected virtual void PreInit() { }

    protected virtual void setup() { }

    protected virtual void loop() { }

    #endregion

    public void __InternalUpdateListeners()
    {
        OnLoopRun?.Invoke(null, new EventArgs());
    }

    public static void SETPIN(int pin, bool value) => SETPIN(pin, value ? HIGH : LOW);

    public static void SETPIN(int pin, int value)
    {
        _ = pin switch
        {
            >= 0 and <= 13 => DigitalPin.DoNotUse[pin].Value = value,
            >= 14 and <= 21 => AnalogPin.DoNotUse[pin].Value = (int) value * 255,
            _ => throw new Exception("Pin number is out of range"),
        };
    }

    protected void pinMode(int pin, PinModeEnum mode)
    {
        if (DEBUG_MODE) Console.WriteLine($"pinMode({pin}, {mode});");
        _ = pin switch
        {
            >= 0 and <= 13 => DigitalPin.DoNotUse[pin].Mode = mode,
            >= 14 and <= 21 => AnalogPin.DoNotUse[pin].Mode = mode,
            _ => throw new Exception("Pin number is out of range")
        };
    }


    private void digitalWrite(int pin, bool value, bool notifyListeners) => digitalWrite(pin, value ? HIGH : LOW, notifyListeners);
    protected void digitalWrite(int pin, bool value) => digitalWrite(pin, value ? HIGH : LOW);
    protected void digitalWrite(int pin, int value) => digitalWrite(pin, value, true);
    private void digitalWrite(int pin, int value, bool notifyListeners)
    {
        if (DEBUG_MODE) Console.WriteLine($"digitalWrite({pin}, {value});");
        _ = pin switch
        {
            >= 0 and <= 13 => DigitalPin.DoNotUse[pin].Value = value,
            _ => throw new Exception("Pin number is out of range")
        };
        if (notifyListeners) DigitalUpdate?.Invoke(DigitalPin.DoNotUse[pin], new EventArgs());
    }

    protected int digitalRead(int pin)
    {
        if (DEBUG_MODE)
            Console.WriteLine($"digitalRead({pin});");
        return _ = pin switch
        {
            >= 0 and <= 13 => DigitalPin.DoNotUse[pin].Value,
            >= 14 and <= 20 => AnalogPin.DoNotUse[pin].DigitalValue,
            _ => throw new Exception("Pin number is out of range")
        };
    }
    protected void analogWrite(int pin, float value) => analogWrite(pin, value, true);
    private void analogWrite(int pin, float value, bool notifyListeners)
    {
        if (DEBUG_MODE)
            Console.WriteLine($"analogWrite({pin}, {value});");
        _ = pin switch
        {
            >= 14 and <= 21 => AnalogPin.DoNotUse[pin].Value = value,
            _ => throw new Exception("Pin number is out of range")
        };
        if (notifyListeners) AnalogUpdate?.Invoke(AnalogPin.DoNotUse[pin], new EventArgs());
    }

    protected float analogRead(int pin)
    {
        if (DEBUG_MODE)
            Console.WriteLine($"analogRead({pin});");
        return _ = pin switch
        {
            >= 14 and <= 21 => AnalogPin.DoNotUse[pin].Value,
            _ => throw new Exception("Pin number is out of range")
        };
    }

    protected void delay(int milliseconds)
    {
        if (DEBUG_MODE)
            Console.WriteLine($"delay({milliseconds});");
        Thread.Sleep(milliseconds);
    }


    private int prevVal = 0;
    protected void shiftOut(int dataPin, int clockPin, int bitOrder, int val)
    {
        var tVal = val;
        for (int i = 0; i < 8; i++)
        {
            if (bitOrder == LSBFIRST)
            {
                digitalWrite(dataPin, val & 1, false);
                val >>= 1;
            }
            else
            {
                digitalWrite(dataPin, (val & 128) != 0, false);
                val <<= 1;
            }

            digitalWrite(clockPin, HIGH, false);
            digitalWrite(clockPin, LOW, false);
        }
        DataShiftedOut?.Invoke(null, tVal);
    }
}

public static class IntExtensions
{
    public static string ToHighLow(this int value)
    {
        return value switch
        {
            0 => "LOW",
            1 => "HIGH",
            _ => "N/D"
        };
    }

    public static string ToHighLow(this bool value)
    {
        return value ? "HIGH" : "LOW";
    }
}