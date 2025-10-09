using MattrifiedGames.EventSO;
using MattrifiedGames.SVData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MGInputManager :  MonoBehaviour
{
    static List<Keyboard> mainKeyboards;
    
    static int keyboardCount;

    public MattrifiedGames.SVData.SaveLoadScriptableObject saveLoadData;

    public StringScriptableValue[] playerInputSaveData;

    #region USER-DEFINED
    // USER-DTODO:  DEFINE THE NUMBER OF PLAYERS beforehand.
    public const int PLAYER_MAX = 4;

    // DEFINE THE NUMBER OF PLAYERS IN THE INPUT LIST
    public static List<MGInput> playerInputs = new List<MGInput>() { null, null, null, null };
    #endregion

    public static MGInputManager Instance { get; private set; }

    public IntEventSO OnInputAssigned;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        mainKeyboards = new List<Keyboard>();
        foreach (var d in InputSystem.devices)
        {
            if (d is Keyboard)
            {
                mainKeyboards.Add((Keyboard)d);
            }
        }
        keyboardCount = mainKeyboards.Count;

        InputSystem.onDeviceChange += InputSystem_onDeviceChange;
    }

    private void InputSystem_onDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        if (arg2 == InputDeviceChange.Disconnected)
        {
            if (playerInputs.Exists(x => x != null && x.device == arg1))
            {
                Debug.Log("DEVICE DISCONNECTED!!!");
            }
        }
    }

    public InputDevice GetDevice(int index)
    {
        if (index > PLAYER_MAX)
            return null;

        return playerInputs[index] == null ? null : playerInputs[index].device;
    }

    public void ClearInput(int index)
    {
        playerInputs[index] = null;
        OnInputAssigned.Raise(index);
    }
    public MGDetectState DetectInput(int index, bool clear)
    {
        // This means the device is already assigned...
        if (playerInputs[index] != null && !clear)
        {
            return MGDetectState.AlreadyDefined;
        }

        playerInputs[index] = null;

        foreach (var d in InputSystem.devices)
        {
            if (playerInputs.Exists(x => x != null && x.device == d))
                continue;

            if (d is Keyboard)
            {
                Keyboard keyboard = d as Keyboard;
                if (keyboard.anyKey.wasPressedThisFrame)
                {
                    playerInputs[index] = new MGKeyboardInput();
                    playerInputs[index].SupplyDevice(d);
                    saveLoadData.Load();
                    playerInputs[index].LoadIDs(playerInputSaveData[index]);

                    OnInputAssigned?.Raise(index);

                    return MGDetectState.KeyboardDetected;
                }
            }
            else if (d is Gamepad)
            {
                Gamepad gamePad = d as Gamepad;
                if (gamePad.buttonEast.wasPressedThisFrame ||
                    gamePad.buttonNorth.wasPressedThisFrame ||
                    gamePad.buttonSouth.wasPressedThisFrame ||
                    gamePad.buttonWest.wasPressedThisFrame)
                {
                    playerInputs[index] = new MGGamepadInput();
                    playerInputs[index].SupplyDevice(d);
                    saveLoadData.Load();
                    playerInputs[index].LoadIDs(playerInputSaveData[index]);

                    OnInputAssigned?.Raise(index);

                    return MGDetectState.GamepadDetected;
                }
            }
            else if (d is Joystick)
            {
                Joystick j = d as Joystick;
                Debug.Log("Joystick detected:  " + d.description);
                
                // TODO:  Make it so this can be recognized.
                OnInputAssigned.Raise(index);

                return MGDetectState.JoystickDetected;
            }
        }
        return MGDetectState.Undetected;
    }
}

public enum MGDetectState : byte
{
    Undetected = 0,
    Detected = 1,
    KeyboardDetected = 1 | 2,
    GamepadDetected = 1 | 4,
    JoystickDetected = 1 | 8,
    AlreadyDefined = 128,
}

public enum MGInputState : byte
{
    None = 0,
    Pressed = 1,
    Down =2,
}

[System.Serializable()]
public class MGInputInfo
{
    public Key[] key;
    public UnityEngine.InputSystem.LowLevel.GamepadButton[] gpButton;
    public string internalID;

    public MGInputInfo(string ID, Key[] key, UnityEngine.InputSystem.LowLevel.GamepadButton[] gpButton)
    {
        this.internalID = ID;
        this.key = key;
        this.gpButton = gpButton;
    }

    public MGInputInfo(string ID, Key key, UnityEngine.InputSystem.LowLevel.GamepadButton gpButton)
    {
        this.internalID = ID;
        this.key = new Key[] { key };
        this.gpButton = new UnityEngine.InputSystem.LowLevel.GamepadButton[] { gpButton };
    }

    public MGInputState GetState(Keyboard keyboard)
    {
        for (int i = 0, l = key.Length; i < l; i++)
        {
            var value = keyboard[key[i]];
            if (value.wasPressedThisFrame)
                return MGInputState.Pressed;
            else if (value.isPressed)
                return MGInputState.Down;
        }

        return MGInputState.None;
    }

    public float GetValue(Keyboard keyboard)
    {
        float v = 0f;
        for (int i = 0, l = key.Length; i < l; i++)
        {
            var value = keyboard[key[i]];
            v = Mathf.Max(value.ReadValue());
        }
        return v;
    }

    public float GetValue(Gamepad gamepad)
    {
        float v = 0f;
        for (int i = 0, l = gpButton.Length; i < l; i++)
        {
            var value = gamepad[gpButton[i]];
            v = Mathf.Max(value.ReadValue());
        }
        return v;
    }

    public MGInputState GetState(Gamepad gamepad)
    {
        for (int i = 0, l = gpButton.Length; i < l; i++)
        {
            var value = gamepad[gpButton[i]];
            if (value.wasPressedThisFrame)
                return MGInputState.Pressed;
            else if (value.isPressed)
                return MGInputState.Down;
        }
        return MGInputState.None;
    }

    public override string ToString()
    {
        return string.Format("{0}|{1}", key.ToString(), gpButton.ToString());
    }
}

#region DEFINE USER INPUTS
// CHANGE THESE PER GAME
[System.Serializable()]
public class MGInputSet
{
    public MGInputInfo leftInput;
    public MGInputInfo rightInput;
    public MGInputInfo upInput;
    public MGInputInfo downInput;

    public MGInputInfo lightPunchInput;
    public MGInputInfo lightKickInput;

    public MGInputInfo mediumPunchInput;
    public MGInputInfo mediumKickInput;

    public MGInputInfo heavyPunchInput;
    public MGInputInfo heavyKickInput;

    private ReadOnlyCollection<MGInputInfo> _buttonList;
    private ReadOnlyCollection<MGInputInfo> _directionAndButtonList;

    public bool TryAndFind(string internalID, out MGInputInfo result)
    {
        result = null;
        foreach (var i in DirectionAndButtonList)
            if (i.internalID == internalID)
            {
                result = i;
                return true;
            }
        return false;
    }
    public ReadOnlyCollection<MGInputInfo> ButtonList
    {
        get
        {
            if (_buttonList == null)
            {
                var l = new List<MGInputInfo>()
                {
                    lightPunchInput,
                    lightKickInput,
                    mediumPunchInput,
                    mediumKickInput,
                    heavyPunchInput,
                    heavyKickInput,
                };
                _buttonList = l.AsReadOnly();
            }
            return _buttonList;
        }

    }

    public ReadOnlyCollection<MGInputInfo> DirectionAndButtonList
    {
        get
        {
            if (_directionAndButtonList == null)
            {
                var kList = new List<MGInputInfo>()
                {
                    leftInput,
                    rightInput,
                    upInput,
                    downInput,
                    lightPunchInput,
                    lightKickInput,
                    mediumPunchInput,
                    mediumKickInput,
                    heavyPunchInput,
                    heavyKickInput,
                };
                _directionAndButtonList = kList.AsReadOnly();
            }
            return _directionAndButtonList;
        }
    }
}
#endregion

public abstract class MGInput
{
    public InputDevice device;

    public MGInputSet mainInputSet;

    #region USER-DEFINED -- Define different inputs
    public MGInputInfo confirmInput = new MGInputInfo(
        "Confirm",
        new Key[]
        {
            Key.Enter
        },
        new UnityEngine.InputSystem.LowLevel.GamepadButton[]
        {
             UnityEngine.InputSystem.LowLevel.GamepadButton.South,
              UnityEngine.InputSystem.LowLevel.GamepadButton.Start,
        }
    );

    public MGInputInfo cancelInput = new MGInputInfo(
        "Cancel",
        new Key[]
        {
                Key.Backspace, Key.Escape,
        },
        new UnityEngine.InputSystem.LowLevel.GamepadButton[]
        {
                 UnityEngine.InputSystem.LowLevel.GamepadButton.East,
                  UnityEngine.InputSystem.LowLevel.GamepadButton.Select,
        }
    );

    public MGInputInfo resetTraining = new MGInputInfo
        (
        "Reset Training",
        new Key[] { Key.Backspace },
        new GamepadButton[]
        {
            GamepadButton.Select,
            GamepadButton.RightStick
        }

        );

    public void SetDefaultIDs()
    {
        mainInputSet = new MGInputSet()
        {
            downInput = new MGInputInfo("Down", Key.S, UnityEngine.InputSystem.LowLevel.GamepadButton.DpadDown),
            upInput = new MGInputInfo("Up", Key.W, UnityEngine.InputSystem.LowLevel.GamepadButton.DpadUp),
            leftInput = new MGInputInfo("Left", Key.A, UnityEngine.InputSystem.LowLevel.GamepadButton.DpadLeft),
            rightInput = new MGInputInfo("Right", Key.D, UnityEngine.InputSystem.LowLevel.GamepadButton.DpadRight),

            lightPunchInput = new MGInputInfo("LP", Key.U, UnityEngine.InputSystem.LowLevel.GamepadButton.West),
            lightKickInput = new MGInputInfo("LK", Key.J, UnityEngine.InputSystem.LowLevel.GamepadButton.South),

            mediumPunchInput = new MGInputInfo("MP", Key.I, GamepadButton.North),
            mediumKickInput = new MGInputInfo("MK", Key.K, GamepadButton.East),

            heavyPunchInput = new MGInputInfo("HP", new Key[] { Key.O }, 
                new GamepadButton[] { GamepadButton.LeftShoulder, GamepadButton.RightShoulder }),
            heavyKickInput = new MGInputInfo("HK", new Key[] { Key.L }, 
                new GamepadButton[] { GamepadButton.LeftTrigger, GamepadButton.RightTrigger }),
        };
    }
    #endregion

    public void LoadIDs(StringScriptableValue jsonString)
    {
        string s = jsonString.Value;
        Debug.Log("LOADED ID:  " + s);
        if (!string.IsNullOrEmpty(s))
        {
            mainInputSet = JsonUtility.FromJson<MGInputSet>(s);
        }
        else
        {
            SetDefaultIDs();
            jsonString.Value = SaveIDs();
            Debug.Log("Saved value:  " + jsonString.Value);
        }
    }

    public string SaveIDs()
    {
        return JsonUtility.ToJson(mainInputSet);
    }
    public abstract Vector2 GetDirectionalInput();
    public abstract void SupplyDevice(InputDevice device);

    public void ClearKeys()
    {
        var t = mainInputSet.DirectionAndButtonList;
        foreach (var tt in t)
            tt.key = new Key[0];
    }

    public void SetNewKey(string configID, Key btnToSwap)
    {
        var tempList = mainInputSet.ButtonList;
        for (int i = 0; i < tempList.Count; i++)
        {
            for (int j = 0; j < tempList[i].key.Length; j++)
            {
                if (tempList[i].key[j] == btnToSwap)
                {
                    // Does the swap
                    List<Key> gp = new List<Key>(tempList[i].key);
                    gp.Remove(btnToSwap);

                    tempList[i].key = gp.ToArray();

                    gp.Clear();

                    gp = null;

                    break;
                }
            }
        }

        if (mainInputSet.TryAndFind(configID, out MGInputInfo input))
        {
            List<Key> updatedKeys = new List<Key>(input.key);
            updatedKeys.Add(btnToSwap);

            input.key = updatedKeys.ToArray();

            updatedKeys.Clear();
            updatedKeys = null;
        }
    }

    public void SetNewGamepadButton(string configID, GamepadButton btnToSwap)
    {
        int swapIndex = -1;
        var tempList = mainInputSet.ButtonList;
        for (int i = 0; i < tempList.Count; i++)
        {
            for (int j = 0; j < tempList[i].gpButton.Length; j++)
            {
                if (tempList[i].gpButton[j] == btnToSwap)
                {
                    // Does the swap
                    List<UnityEngine.InputSystem.LowLevel.GamepadButton> gp = new List<GamepadButton>(tempList[i].gpButton);
                    gp.Remove(btnToSwap);

                    tempList[i].gpButton = gp.ToArray();

                    gp.Clear();

                    gp = null;

                    swapIndex = i;

                    break;
                }
            }
        }

        if (mainInputSet.TryAndFind(configID, out MGInputInfo input))
        {
            List<UnityEngine.InputSystem.LowLevel.GamepadButton> gp = new List<GamepadButton>(input.gpButton);
            gp.Add(btnToSwap);

            input.gpButton = gp.ToArray();

            gp.Clear();
            gp = null;
        }
    }

    public abstract MGInputState TestStaticInput(Key key, GamepadButton button);

    public abstract MGInputState TestInput(MGInputInfo inputInfo);
}

public class MGGamepadInput : MGInput
{
    public Gamepad gamepad;
    public override Vector2 GetDirectionalInput()
    {
        var dpad = gamepad.dpad.ReadValue();
        var ls = gamepad.leftStick.ReadValue();

        var avg = dpad + ls;

        return avg;
    }

    public MGInputState GetButtonState(GamepadButton button)
    {
        if (gamepad[button].wasPressedThisFrame)
            return MGInputState.Pressed;
        else if (gamepad[button].isPressed)
            return MGInputState.Down;
        else
            return MGInputState.None;
    }
    public override void SupplyDevice(InputDevice device)
    {
        this.device = device;
        if (device is Gamepad)
        {
            gamepad = (Gamepad)device;
        }
        else
        {
            Debug.LogError(device + " is not gamepad.");
        }
    }

    public override MGInputState TestStaticInput(Key key, GamepadButton button)
    {
        if (gamepad[button].wasPressedThisFrame)
            return MGInputState.Pressed;
        else if (gamepad[button].isPressed)
            return MGInputState.Down;
        else
            return MGInputState.None;
    }

    public override MGInputState TestInput(MGInputInfo inputInfo)
    {
        if (inputInfo == null)
            return MGInputState.None;

        MGInputState result = MGInputState.None;
        for (int i = 0; i < inputInfo.gpButton.Length; i++)
        {
            if (gamepad[inputInfo.gpButton[i]].wasPressedThisFrame)
                result = MGInputState.Pressed;
            else if (gamepad[inputInfo.gpButton[i]].isPressed)
                return MGInputState.Down;
        }

        return result;
    }
}

public class MGKeyboardInput : MGInput
{
    public Keyboard keyboard;

    public override void SupplyDevice(InputDevice device)
    {
        this.device = device;

        if (device is Keyboard)
        {
            keyboard = (Keyboard)device;
        }
        else
        {
            Debug.LogError(device + " is not keyboard.");
        }
    }

    public override Vector2 GetDirectionalInput()
    {
        Vector2 result = Vector2.zero;

        if (mainInputSet.leftInput.GetState(keyboard) != MGInputState.None)
        {
            result.x = -1;
        }
        else if (mainInputSet.rightInput.GetState(keyboard) != MGInputState.None)
        {
            result.x = 1;
        }

        if (mainInputSet.upInput.GetState(keyboard) != MGInputState.None)
        {
            result.y = 1;
        }
        else if (mainInputSet.downInput.GetState(keyboard) != MGInputState.None)
        {
            result.y = -1;
        }

        return result;
    }

    public override MGInputState TestStaticInput(Key key, GamepadButton button)
    {
        if (keyboard[key].wasPressedThisFrame)
            return MGInputState.Pressed;
        else if (keyboard[key].isPressed)
            return MGInputState.Down;
        else
            return MGInputState.None;
    }

    public override MGInputState TestInput(MGInputInfo inputInfo)
    {
        if (inputInfo == null)
            return MGInputState.None;

        MGInputState result = MGInputState.None;
        for (int i = 0; i < inputInfo.key.Length; i++)
        {
            if (keyboard[inputInfo.key[i]].wasPressedThisFrame)
                result = MGInputState.Pressed;
            else if (keyboard[inputInfo.key[i]].isPressed)
                return MGInputState.Down;
        }

        return result;
    }
}