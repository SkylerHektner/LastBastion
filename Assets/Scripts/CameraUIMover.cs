using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIMover : MonoBehaviour
{
    public GameObject Camera;
    public Transform CurrentDestination;

    public Transform LevelsZone;
    public Transform MainZone;
    public Transform UpgradesZone;

    public float CameraSpeed;
   


    private void FixedUpdate()
    {
        Camera.transform.position = Vector3.MoveTowards(Camera.transform.position, CurrentDestination.position, CameraSpeed); // constantly move the camera to the "Current Destination"
    }

    // updates the taret position for the camera to move towards
    [ContextMenu("LoadLevels")]
    public void LoadLevels()
    {
        CurrentDestination = LevelsZone;
    }

    [ContextMenu("LoadMainMenu")]
    public void LoadMainMenu()
    {
        CurrentDestination = MainZone;
    }

    public void LoadUpgradesMenu()
    {
        CurrentDestination = UpgradesZone;
    }
}
