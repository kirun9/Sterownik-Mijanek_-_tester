namespace Sterownik_Mijanek___tester;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class SterownikMijanekCodeOld : ArduinoCode
{
    public event EventHandler<int> DataShiftedOut;

    protected override void PreInit()
    {
        SETPIN(A0, HIGH);
        SETPIN(A1, HIGH);
        SETPIN(A2, HIGH);
        SETPIN(A3, HIGH);
        SETPIN(A4, HIGH);
        SETPIN(A5, HIGH);
        SETPIN(A6, HIGH);
        SETPIN(A7, HIGH);
    }

    internal enum Status { WAITING_FOR_CLEAR_TRACK = 0, WAITING_FOR_TRAIN = 1, TRAIN_DEPARTED = 2, ERROR = -1 }
    public const Status WAITING_FOR_CLEAR_TRACK = Status.WAITING_FOR_CLEAR_TRACK;
    public const Status WAITING_FOR_TRAIN = Status.WAITING_FOR_TRAIN;
    public const Status TRAIN_DEPARTED = Status.TRAIN_DEPARTED;
    public const Status ERROR = Status.ERROR;

    public const int MSBFIRST = 1;
    public const int LSBFIRST = 0;

    public const int junctionSwitchTime = 1000;

    public const int Data = 2;
    public const int RCLK = 3;
    public const int SRCLK = 4;
    public const int Det_Next = 5;
    public const int Det_Next_2 = 6;
    public const int Det_1 = 7;      // Low means detected
    public const int Det_2 = 8;      // Low means detected
    public const int Det_3 = 9;      // Low means detected
    public const int SW1_1 = 10;     // High means on
    public const int SW1_2 = 11;     // High means on
    public const int SW1_3 = 12;     // High means on

    public const int DetOut = 0;     //Detector output: Low means detected
    public const int Junction1L = 1; //Junction 1 Left
    public const int Junction1R = 2; //Junction 1 Right
    public const int Junction2L = 3; //Junction 2 Left
    public const int Junction2R = 4; //Junction 2 Right
    public const int Track1 = 5;     //Track 1
    public const int Track2 = 6;     //Track 2
    public const int Track3 = 7;     //Track 3

    public const int Junction1LButton = A3;
    public const int Junction1RButton = A4;
    public const int Junction2LButton = A5;
    public const int Junction2RButton = A6;



    internal int detLook = Det_Next;

    internal int CurrentOutput;
    internal bool track1Enable = false;
    internal bool track2Enable = false;
    internal bool track3Enable = false;
    internal byte actualSelectedTrack = 1;
    internal Status status = WAITING_FOR_CLEAR_TRACK;

    protected override void setup()
    {
        pinMode(Data, OUTPUT);
        pinMode(RCLK, OUTPUT);
        pinMode(SRCLK, OUTPUT);
        pinMode(Det_Next, INPUT);
        pinMode(Det_Next_2, INPUT);
        digitalWrite(Det_Next, HIGH);
        digitalWrite(Det_Next_2, HIGH);
        pinMode(Det_1, INPUT);
        pinMode(Det_2, INPUT);
        pinMode(Det_3, INPUT);
        pinMode(SW1_1, INPUT);
        pinMode(SW1_2, INPUT);
        pinMode(SW1_3, INPUT);
        pinMode(A1, INPUT);
        pinMode(A2, INPUT);
        pinMode(A3, INPUT);
        pinMode(A4, INPUT);
        pinMode(A5, INPUT);
        pinMode(A6, INPUT);
        pinMode(A7, INPUT);

        ClearOutput();
        checkState();
    }

    protected override void loop()
    {
        checkSwitches();
        checkPulpit();
        if (status == WAITING_FOR_CLEAR_TRACK)
        {
            if (digitalRead(detLook) == HIGH && digitalRead(Det_Next) == HIGH)
            {
                departTrain(actualSelectedTrack);
                return;
            }
        }
        if (status == TRAIN_DEPARTED)
        {
            if (digitalRead(Det_Next) == LOW)
            {
                status = WAITING_FOR_TRAIN;
                stopTrain();
                return;
            }
        }
        if (status == WAITING_FOR_TRAIN)
        {
            if (checkTrack() == HIGH)
            {
                selectNextTrack();
                status = WAITING_FOR_CLEAR_TRACK;
            }
        }
        copyDetection();
        delay(100);
    }

    void checkState()
    {
        byte s = 0;
        if (track1Enable)
        {
            if (digitalRead(Det_1) == LOW)
            {
                s += 4;
            }
        }
        if (track2Enable)
        {
            if (digitalRead(Det_2) == LOW)
            {
                s += 2;
            }
        }
        if (track3Enable)
        {
            if (digitalRead(Det_3) == LOW)
            {
                s += 1;
            }
        }
        if (((s >> 0) & 1) == 1 || !track1Enable)
        {
            if (((s >> 1) & 1) == 1 || !track2Enable)
            {
                if (((s >> 2) & 1) == 1 || !track3Enable)
                {
                    status = WAITING_FOR_CLEAR_TRACK;
                }
                else
                {
                    status = WAITING_FOR_TRAIN;
                    selectTrack(3);
                }
            }
            else
            {
                status = WAITING_FOR_TRAIN;
                selectTrack(2);
            }
        }
        else
        {
            status = WAITING_FOR_TRAIN;
            selectTrack(1);
        }
    }

    void selectTrack(byte track)
    {
        actualSelectedTrack = track;
        if (track == 1)
        {
            SetJunction(0, 0);
        }
        else if (track == 2)
        {
            SetJunction(1, 0);
        }
        else if (track == 3)
        {
            SetJunction(1, 1);
        }
    }

    void ClearJunction()
    {
        SetOutput(Junction1L, LOW);
        SetOutput(Junction1R, LOW);
        SetOutput(Junction2L, LOW);
        SetOutput(Junction2R, LOW);
    }

    void SetJunction(int junction1, int junction2)
    {
        if (junction1 == 0)
        {
            SetOutput(Junction1R, LOW);
            SetOutput(Junction1L, HIGH);
        }
        else if (junction1 == 1)
        {
            SetOutput(Junction1L, LOW);
            SetOutput(Junction1R, HIGH);
        }
        if (junction2 == 0)
        {
            SetOutput(Junction2R, LOW);
            SetOutput(Junction2L, HIGH);
        }
        else if (junction2 == 1)
        {
            SetOutput(Junction2L, LOW);
            SetOutput(Junction2R, HIGH);
        }
        __InternalUpdateListeners();
        delay(1000);
        ClearJunction();
    }

    void departTrain(int track)
    {
        if (track == 1)
        {
            SetOutput(Track1, HIGH);
        }
        else if (track == 2)
        {
            SetOutput(Track2, HIGH);
        }
        else if (track == 3)
        {
            SetOutput(Track3, HIGH);
        }
        status = TRAIN_DEPARTED;
    }

    void stopTrain()
    {
        SetOutput(Track1, LOW);
        SetOutput(Track2, LOW);
        SetOutput(Track3, LOW);
    }

    void selectNextTrack()
    {
        if (!track1Enable && !track2Enable && !track3Enable)
        {
            status = ERROR;
            actualSelectedTrack = 0;
            return;
        }
        actualSelectedTrack++;
        if (actualSelectedTrack > 3)
        {
            actualSelectedTrack = 1;
        }
        if (actualSelectedTrack == 1 && !track1Enable)
        {
            selectNextTrack();
        }
        if (actualSelectedTrack == 2 && !track2Enable)
        {
            selectNextTrack();
        }
        if (actualSelectedTrack == 3 && !track3Enable)
        {
            selectNextTrack();
        }
        selectTrack(actualSelectedTrack);
    }

    void checkSwitches()
    {
        track1Enable = track2Enable = track3Enable = true;
    }

    int checkTrack()
    {
        if (actualSelectedTrack == 1)
        {
            return digitalRead(Det_1);
        }
        else if (actualSelectedTrack == 2)
        {
            return digitalRead(Det_2);
        }
        else if (actualSelectedTrack == 3)
        {
            return digitalRead(Det_3);
        }
        else
        {
            return LOW;
        }
    }

    void checkPulpit()
    {
        if (digitalRead(A0) == LOW)
        {
            departTrain(1);
        }
        if (digitalRead(A1) == LOW)
        {
            departTrain(2);
        }
        if (digitalRead(A2) == LOW)
        {
            departTrain(3);
        }
        if (digitalRead(Junction1LButton) == LOW)
        {
            SetJunction(0, -1);
        }
        if (digitalRead(Junction1RButton) == LOW)
        {
            SetJunction(1, -1);
        }
        if (digitalRead(Junction2LButton) == LOW)
        {
            SetJunction(-1, 0);
        }
        if (analogRead(Junction2RButton) < 125)
        {
            SetJunction(-1, 1);
        }
    }

    void copyDetection()
    {
        switch (actualSelectedTrack)
        {
            case 1:
                SetOutput(DetOut, digitalRead(Det_1));
                break;
            case 2:
                SetOutput(DetOut, digitalRead(Det_2));
                break;
            case 3:
                SetOutput(DetOut, digitalRead(Det_3));
                break;
        }
    }

    void ClearOutput()
    {
        digitalWrite(RCLK, LOW);
        shiftOut(Data, SRCLK, MSBFIRST, 0);
        digitalWrite(RCLK, HIGH);
        digitalWrite(RCLK, LOW);
        CurrentOutput = 0;
    }

    void SetOutput(byte o, int state)
    {
        if (state == HIGH)
        {
            CurrentOutput |= (1 << o);
        }
        else
        {
            CurrentOutput &= ~(1 << o);
        }
        digitalWrite(RCLK, LOW);
        shiftOut(Data, SRCLK, MSBFIRST, CurrentOutput);
        digitalWrite(RCLK, HIGH);
    }

    #region TimerOne - external script
    private int prevVal = 0;
    void shiftOut(int dataPin, int clockPin, int bitOrder, int val)
    {
        if (val == prevVal)
            return;
        int i;

        for (i = 0; i < 8; i++)
        {
            if (bitOrder == LSBFIRST)
            {
                digitalWrite(dataPin, val & 1);
                val >>= 1;
            }
            else
            {
                digitalWrite(dataPin, (val & 128) != 0);
                val <<= 1;
            }

            digitalWrite(clockPin, HIGH);
            digitalWrite(clockPin, LOW);
            DataShiftedOut?.Invoke(null, val);
        }
        prevVal = val;
    }
    #endregion
}
