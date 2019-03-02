using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_Input : MonoBehaviour {


    public class fps_InputAxis
    {
        public KeyCode positive;
        public KeyCode negative;
    }

	public Dictionary<string,ulong> buttons = new Dictionary<string,ulong>();
    public Dictionary<string, fps_InputAxis> axis = new Dictionary<string, fps_InputAxis>();
    public List<string> unityAxis = new List<string>();
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device deviceRight = null;
    private SteamVR_Controller.Device deviceLeft = null;

    void Awake(){
		trackedObject = GetComponent<SteamVR_TrackedObject> ();

	}

    void Start()
    {
        SetupDefaults();
    }


    void FixedUpdate()
    {
        var deviceIndexRight = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
		deviceRight = SteamVR_Controller.Input (deviceIndexRight);
        var deviceIndexLeft = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        deviceLeft = SteamVR_Controller.Input(deviceIndexLeft);
    }

    private void SetupDefaults(string type = "")
    {
        if (type == "" || type == "button")
        {
            if (buttons.Count == 0)
            {
                //AddButton("Fire", KeyCode.Mouse0);
                //AddButton("Reload", KeyCode.R);
                //AddButton("Jump", KeyCode.Space);
                //AddButton("Crouch", KeyCode.C);
                //AddButton("Sprint", KeyCode.LeftShift);
				AddButton("Fire", SteamVR_Controller.ButtonMask.Trigger);
				AddButton("Reload", SteamVR_Controller.ButtonMask.Grip);
				AddButton("Jump", SteamVR_Controller.ButtonMask.Trigger);
				AddButton("Crouch", SteamVR_Controller.ButtonMask.Touchpad);
				AddButton("Sprint", SteamVR_Controller.ButtonMask.Touchpad);
            }
        }

        if (type == "" || type == "Axis")
        {
            if (axis.Count == 0)
            {
                AddAxis("Horizontal", KeyCode.W, KeyCode.S);
                AddAxis("Vertical", KeyCode.A, KeyCode.D);

            }
        }

        if (type == "" || type == "UnityAxis")
        {
            if (unityAxis.Count == 0)
            {
                AddUnityAxis("Mouse X");
                AddUnityAxis("Mouse Y");
                AddUnityAxis("Horizontal");
                AddUnityAxis("Vertical");
            }
        }
    }

	private void AddButton(string n, ulong k)
    {
        if (buttons.ContainsKey(n))
        {
            buttons[n] = k;
        }
        else
        {
            buttons.Add(n, k);
        }
    }

    private void AddAxis(string n,KeyCode pk,KeyCode nk)
    {
        if (axis.ContainsKey(n))
        {
            axis[n] = new fps_InputAxis() { positive = pk, negative = nk };
        }
        else
        {
            axis.Add(n, new fps_InputAxis() { positive = pk, negative = nk });
        }
    }

    private void AddUnityAxis(string n)
    {
        if (!unityAxis.Contains(n))
        {
            unityAxis.Add(n);
        }
    }

    public bool GetButton(string button)
    {
        if (buttons.ContainsKey(button))
        {
			return deviceRight.GetPress(buttons[button]);
        }
        return false;
    }

    public bool GetButtonDown(string button)
    {
        if (buttons.ContainsKey(button))
        {
			return deviceLeft.GetPressDown(buttons[button]);
        }
        return false;
    }

    public float GetAxis(string axis)
    {
        if (this.unityAxis.Contains(axis))
        {
            return Input.GetAxis(axis);
        }
        else
        {
            return 0;
        }
    }

    public float GetAxisRaw(string axis)
    {
        if (this.axis.ContainsKey(axis))
        {
            float val = 0;
            if (Input.GetKey(this.axis[axis].positive))
            {
                return 1;
            }
            if (Input.GetKey(this.axis[axis].negative))
            {
                return -1;
            }
            return val;
        }
        else if(unityAxis.Contains(axis))
        {
            return Input.GetAxisRaw(axis);
        }
        else
        {
            return 0;
        }
    }
}
