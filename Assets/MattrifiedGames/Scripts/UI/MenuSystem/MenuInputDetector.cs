#if USING_INCONTROL
using InControl;
using MattrifiedGames.Audio;
using MattrifiedGames.SVData;
using MattrifiedGames.UI;
using UnityEngine;
using UnityEngine.Events;

public class MenuInputDetector : MonoBehaviour
{
    [SerializeField()]
    InControlDefinitionScriptableValue inputDict;

    [SerializeField()]
    InControlDefinitionScriptableValue[] otherInputValues;

    [SerializeField()]
    public AudioClipScriptableObject onPressSFX;

    public UnityEvent OnInputAssigned;

    public LoadingScreenBase loadingScreenRef;

    public string sceneName;

    public bool Locked { get; set; }
    public UnityStringEvent OnUnknownDeviceDetected;

    private void Start()
    {
        if (inputDict.AssociatedController.Value != null)
        {
            Debug.LogWarning("Input is already assigned.");
            OnInputAssigned.Invoke();
            enabled = false;
        }
    }

    public void OpenAndClearValue()
    {
        if (inputDict.AssociatedController.Value != null)
        {
            inputDict.AssociatedController.Value.Destroy();
            inputDict.AssociatedController.Value = null;
        }

        for (int i = 0; i < otherInputValues.Length; i++)
        {
            if (otherInputValues[i].AssociatedController.Value != null)
            {
                otherInputValues[i].AssociatedController.Value.Destroy();
                otherInputValues[i].AssociatedController.Value = null;
            }
        }

        enabled = true;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            enabled = false;
            loadingScreenRef.LoadScene(sceneName);
            return;
        }

        if (Locked)
        {
            return;
        }

        if (AssignDevice(inputDict, otherInputValues, out InputDevice unknownDetected))
        {
            OnInputAssigned.Invoke();
            if (onPressSFX)
                onPressSFX.PlayFromPool();
            enabled = false;
        }
        else if (unknownDetected != null)
        {
            // TODO:  Show pop-up for unknown item detected
            OnUnknownDeviceDetected.Invoke(unknownDetected.Meta);
        }
    }
    
    public static bool AssignDevice(InControlDefinitionScriptableValue inputValue, InControlDefinitionScriptableValue[] otherInputValues, out InputDevice unknownDetected)
    {
        unknownDetected = null;
        var devices = InputManager.Devices;
        for (int i = 0; i < devices.Count; i++)
        {
            bool isDevicedUsed = false;
            for (int j = 0; j < otherInputValues.Length; j++)
            {
                var v = otherInputValues[j].Value;

                if (v == null)
                    continue;

                if (otherInputValues[j].AssociatedController.Value != null &&
                    otherInputValues[j].AssociatedController.Value.Device == devices[i])
                {
                    isDevicedUsed = true;
                    break;
                }
            }

            if (isDevicedUsed)
                continue;

            if (devices[i].AnyButtonWasPressed)
            {
                UnknownInputStringValuePairSet unknownSet = null;
                if (devices[i].IsUnknown)
                {
                    Debug.Log("Unknown device detected.");
                    // searches for unknown device data...
                    string meta = devices[i].Meta;
                    //UnknownDevice
                    unknownSet = UnknownDeviceConfigBehaviour.LoadData(meta);

                    if (unknownSet == null)
                    {
                        unknownDetected = devices[i];
                        return false;
                    }
                }

                inputValue.AssociatedController.Value = inputValue.Value.CreatePlayerActionSet(devices[i]);
                return true;
            }
        }

        // We do this check to see if the keyboard is a viable option.
        // May need to put within a platform check.
        for (int j = 0; j < otherInputValues.Length; j++)
        {
            var v = otherInputValues[j].Value;

            if (v == null)
                continue;

            if (otherInputValues[j].AssociatedController.Value != null &&
                otherInputValues[j].AssociatedController.Value.Device == null)
                return false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputValue.AssociatedController.Value = inputValue.Value.CreatePlayerActionSet(null);
            return true;
        }



        return false;
    }

    public static bool AssignDevice(InControlDefinitionScriptableValue inputValue, InControlDefinedPlayerActionSetScriptableValue otherInputValue)
    {
        var devices = InputManager.Devices;
        for (int i = 0; i < devices.Count; i++)
        {
            bool isDevicedUsed = false;

            if (otherInputValue.Value == null)
                continue;
            else if (otherInputValue.Value.Device == devices[i])
                isDevicedUsed = true;

            if (isDevicedUsed)
                continue;

            if (devices[i].AnyButtonWasPressed)
            {
                inputValue.AssociatedController.Value = inputValue.Value.CreatePlayerActionSet(devices[i]);
                return true;
            }
        }

        if (otherInputValue.Value != null && otherInputValue.Value.Device == null)
            return false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputValue.AssociatedController.Value = inputValue.Value.CreatePlayerActionSet(null);
            return true;
        }

        return false;
    }
}
#endif