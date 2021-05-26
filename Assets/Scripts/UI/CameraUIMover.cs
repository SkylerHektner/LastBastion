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
    public Transform SurvivalZone;

    public float CameraSpeed;   // default is .8f

    public float MoveDelay;
    public bool Delayed;


    private void FixedUpdate()
    {
        if (Delayed == false) // delays camera movement
        {
            Camera.transform.position = Vector3.MoveTowards(Camera.transform.position, CurrentDestination.position, CameraSpeed); // constantly move the camera to the "Current Destination"
        }
        else
        {
            MoveDelay -= Time.smoothDeltaTime;
            if (MoveDelay <= 0)
            {
                Delayed = false;
            }
        }

    }


    // updates the taret position for the camera to move towards
    [ContextMenu("LoadLevels")]
    public void LoadLevels()
    {
        CurrentDestination = LevelsZone;
    }

    public void LoadLevelShortcut() // triggers when returning from a campaign level
    {
        CurrentDestination = LevelsZone;
        Camera.transform.position = CurrentDestination.position;
    }

    public void ShowUpgradeTree(Transform UpgradeLocation)
    {
        CurrentDestination = UpgradeLocation;
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

    public void LoadSurvival()
    {
        CurrentDestination = SurvivalZone;
        CameraSpeed = 3f;
        MoveDelay = 1f;
        Delayed = true;
    }

    public void LoadSurvivalShortcut() // triggers when returning from survival
    {
        CurrentDestination = SurvivalZone;
        Camera.transform.position = CurrentDestination.position;
    }

    public void ExitSurvivalArea()
    {
        CurrentDestination = MainZone;
        CameraSpeed = 3f;
    }
}
