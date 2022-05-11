namespace Sterownik_Mijanek___tester;

internal class SterownikMijanekCode2 : ArduinoCode
{
    internal bool dummybool = false;
    internal int detected;
    internal int det_1, det_2, det_3, detNext, detNext2;
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

        SETPIN(SW1_1, HIGH);
        SETPIN(SW1_2, HIGH);
        SETPIN(SW1_3, HIGH);
    }

    public const int junctionLockTime = 5;

    #region define pins and data
    public const int Data             = 2;
    public const int RCLK             = 3;
    public const int SRCLK            = 4;
    public const int Det_Next         = 5;
    public const int Det_Next_2       = 6;
    public const int Det_1            = 7;      // Low means detected
    public const int Det_2            = 8;      // Low means detected
    public const int Det_3            = 9;      // Low means detected
    public const int SW1_1            = 10;     // High means on
    public const int SW1_2            = 11;     // High means on
    public const int SW1_3            = 12;     // High means on

    public const int DetOut           = 0;     //Detector output: High means detected (outside arduino signal is inverted)
    public const int Junction1L       = 1; //Junction 1 Left
    public const int Junction1R       = 2; //Junction 1 Right
    public const int Junction2L       = 3; //Junction 2 Left
    public const int Junction2R       = 4; //Junction 2 Right
    public const int Track1           = 5;     //Track 1
    public const int Track2           = 6;     //Track 2
    public const int Track3           = 7;     //Track 3

    public const int Track1Button     = A0;
    public const int Track2Button     = A1;
    public const int Track3Button     = A2;
    public const int Junction1LButton = A3;
    public const int Junction1RButton = A4;
    public const int Junction2LButton = A5;
    public const int Junction2RButton = A6;
    #endregion

    internal int detLook = Det_Next;

    internal int CurrentOutput = 0;

    internal bool track1Enable = false;
    internal bool track2Enable = false;
    internal bool track3Enable = false;

    internal byte actualSelectedTrack = 1;
    internal byte j1Lock, j2Lock;


    internal Status status = WAITING_FOR_CLEAR_TRACK;
    internal enum Status { WAITING_FOR_CLEAR_TRACK = 0, WAITING_FOR_TRAIN = 1, TRAIN_DEPARTED = 2, TRAIN_DEPARTING = 3, ERROR = -1 }
    public const Status WAITING_FOR_CLEAR_TRACK = Status.WAITING_FOR_CLEAR_TRACK;
    public const Status WAITING_FOR_TRAIN = Status.WAITING_FOR_TRAIN;
    public const Status TRAIN_DEPARTED = Status.TRAIN_DEPARTED;
    public const Status TRAIN_DEPARTING = Status.TRAIN_DEPARTING;
    public const Status ERROR = Status.ERROR;

    protected override void setup()
    {
        pinMode(Data, OUTPUT);
        pinMode(RCLK, OUTPUT);
        pinMode(SRCLK, OUTPUT);
        pinMode(Det_Next, INPUT);
        pinMode(Det_Next_2, INPUT);
        pinMode(Det_1, INPUT);
        pinMode(Det_2, INPUT);
        pinMode(Det_3, INPUT);
        pinMode(SW1_1, INPUT);
        pinMode(SW1_2, INPUT);
        pinMode(SW1_3, INPUT);
        pinMode(A0, INPUT);
        pinMode(A1, INPUT);
        pinMode(A2, INPUT);
        pinMode(A3, INPUT);
        pinMode(A4, INPUT);
        pinMode(A5, INPUT);
        pinMode(A6, INPUT);

        digitalWrite(Det_Next, HIGH);
        digitalWrite(Det_Next_2, HIGH);
        digitalWrite(Det_1, HIGH);
        digitalWrite(Det_2, HIGH);
        digitalWrite(Det_3, HIGH);

        ClearOutput();
    }

    protected override void loop()
    {
        DebugRead();
        checkSW();
        checkPulpit();
        updateJunctions();

        if (status == WAITING_FOR_CLEAR_TRACK)
        {
            if (digitalRead(detLook) == HIGH && digitalRead(Det_Next) == HIGH)
            {
                departTrain();
            }
        }
        if (status == TRAIN_DEPARTED)
        {
            if (digitalRead(Det_Next) == LOW && detectActualTrack() == HIGH)
            {
                if (switchTrack())
                {
                    clearSignals();
                    status = WAITING_FOR_TRAIN;
                }
            }
        }
        if (status == WAITING_FOR_TRAIN)
        {
            copyDet();
            if (detectActualTrack() == LOW)
            {
                selectTrack();
                status = WAITING_FOR_CLEAR_TRACK;
            }
        }

        delay(500);
    }

    void DebugRead()
    {
        det_1 = digitalRead(Det_1); //DELETE IN FINAL
        det_2 = digitalRead(Det_2);
        det_3 = digitalRead(Det_3);
        detNext = digitalRead(Det_Next);
        detNext2 = digitalRead(Det_Next_2);
    }

    void checkSW()
    {
        track1Enable = digitalRead(SW1_1) == HIGH;
        track2Enable = digitalRead(SW1_2) == HIGH;
        track3Enable = digitalRead(SW1_3) == HIGH;
    }

    void checkPulpit()
    {
        if (digitalRead(Track1Button) == LOW) // Track1 button
            SetOutput(Track1, HIGH);

        if (digitalRead(Track2Button) == LOW) // Track2 button
            SetOutput(Track2, HIGH);

        if (digitalRead(Track3Button) == LOW) // Track3 button
            SetOutput(Track3, HIGH);


        if (digitalRead(Junction1LButton) == LOW && j1Lock == 0) // Junction 1 L button
            setJunctions(1, -1);

        if (digitalRead(Junction1RButton) == LOW && j1Lock == 0) // Junction 1 R button
            setJunctions(2, -1);

        if (digitalRead(Junction2LButton) == LOW && j2Lock == 0) // Junction 2 L button
            setJunctions(-1, 1);

        if (analogRead(Junction2RButton) < 125 && j2Lock == 0) // Junction 2 R button
            setJunctions(-1, 2);
    }

    void updateJunctions()
    {
        if (j1Lock > 0) // if junction locked - subtract time;
            j1Lock--;
        else
            j1Lock = 0;

        if (j2Lock > 0) // if junction locked - subtract time;
            j2Lock--;
        else
            j2Lock = 0;
    }

    void setJunctions(int junction1, int junction2)
    {
        switch (junction1)
        {
            case 0: // Release Junction
                SetOutput(Junction1L, LOW);
                SetOutput(Junction1R, LOW);
                j1Lock = 0; // reset Lock
                break;
            case 1: // Set Junction to LEFT
                SetOutput(Junction1L, HIGH);
                SetOutput(Junction1R, LOW);
                j1Lock = junctionLockTime; // Set junction lock for `junctionLockTime` cicles
                break;
            case 2: // Set Junction to RIGHT
                SetOutput(Junction1L, LOW);
                SetOutput(Junction1R, HIGH);
                j1Lock = junctionLockTime; // Set junction lock for `junctionLockTime` cicles
                break;
            default:
                break;
        }

        switch (junction2)
        {
            case 0: // Release Junction
                SetOutput(Junction2L, LOW);
                SetOutput(Junction2R, LOW);
                j2Lock = 0; // reset Lock
                break;
            case 1: // Set Junction to LEFT
                SetOutput(Junction2L, HIGH);
                SetOutput(Junction2R, LOW);
                j2Lock = junctionLockTime; // Set junction lock for `junctionLockTime` cicles
                break;
            case 2: // Set Junction to RIGHT
                SetOutput(Junction2L, LOW);
                SetOutput(Junction2R, HIGH);
                j2Lock = junctionLockTime; // Set junction lock for `junctionLockTime` cicles
                break;
            default:
                break;
        }
    }

    void selectTrack()
    {
        byte tNumber = selectNextTrack();
        if (tNumber == 0)
        {
            actualSelectedTrack = 0;
            return;
        }
        actualSelectedTrack = tNumber;
    }

    bool switchTrack()
    {
        if (j1Lock > 0 || j2Lock > 0)
            return false;
        switch (actualSelectedTrack)
        {
            case 1:
                if (!track1Enable)
                    return false;
                setJunctions(1, 1);
                break;
            case 2:
                if (!track2Enable)
                    return false;
                setJunctions(2, 1);
                break;
            case 3:
                if (!track3Enable)
                    return false;
                setJunctions(2, 2);
                break;
        }
        return true;
    }

    byte selectNextTrack(byte t)
    {
        t = (byte) (t + 1);
        if (t > 3)
            t = 1;
        if (t == 1 && !track1Enable)
            return selectNextTrack(t);
        if (t == 2 && !track2Enable)
            return selectNextTrack(t);
        if (t == 3 && !track3Enable)
            return selectNextTrack(t);
        return t;
    }

    byte selectNextTrack()
    {
        if (!track1Enable && !track2Enable && !track3Enable)
            return 0;

        return selectNextTrack(actualSelectedTrack);
    }

    void clearSignals()
    {
        SetOutput(Track1, LOW);
        SetOutput(Track2, LOW);
        SetOutput(Track3, LOW);
    }

    void copyDet()
    {
        switch (actualSelectedTrack)
        {
            case 1:
                SetOutput(DetOut, digitalRead(Det_1)); //replace with !
                break;
            case 2:
                SetOutput(DetOut, digitalRead(Det_2));
                break;
            case 3:
                SetOutput(DetOut, digitalRead(Det_3));
                break;
        }
    }

    void departTrain()
    {
        switch (actualSelectedTrack)
        {
            case 1: SetOutput(Track1, HIGH); status = TRAIN_DEPARTED; break;
            case 2: SetOutput(Track2, HIGH); status = TRAIN_DEPARTED; break;
            case 3: SetOutput(Track3, HIGH); status = TRAIN_DEPARTED; break;
        }
    }

    int detectActualTrack()
    {
        switch (actualSelectedTrack)
        {
            case 1: return digitalRead(Det_1);
            case 2: return digitalRead(Det_2);
            case 3: return digitalRead(Det_3);
        }
        return 0;
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
}
