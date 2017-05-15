using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VRButtonBehavior : MonoBehaviour, IVirtualButtonEventHandler
{
    private void Start()
    {
        GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
    }

    public void OnButtonPressed(VirtualButtonAbstractBehaviour vb)
    {
        VRInputManager.SetIsControllerButtonPressed(true);
    }

    public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
    {
        VRInputManager.SetIsControllerButtonPressed(false);
    }
}
