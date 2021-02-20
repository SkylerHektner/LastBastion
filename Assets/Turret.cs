using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    public float RotationMax;
    public float RotationMin;
    bool EnemySighted;
    public Transform MaxSightRange;
    public GameObject PivotPoint;
    public float FireRate;

    private void FixedUpdate()
    {
        PivotPoint.GetComponent<Animator>().SetFloat("FireRate", FireRate); // animation speed for firing anim
        RaycastingEnemy();
        if (EnemySighted)
        {
            // enemy sighted, fire!
            PivotPoint.GetComponent<Animator>().SetBool("Detected", true);
        }
        else
        {
            // search for enemies
            PivotPoint.GetComponent<Animator>().SetBool("Detected", false);
        }
    }

    public void SearchForEnemy()
    {
        // rotate between max and min rotation angles while not doing anything
    }

    public void FireBullet() // called by pivot animator
    {
        // instantiate projectile prefab
        Debug.Log("Pew Pew Pew");
    }

    void RaycastingEnemy()
    {
        Debug.DrawLine(PivotPoint.transform.position, MaxSightRange.position, Color.green);  // during playtime, projects a line from a start point to and end point
        EnemySighted = Physics2D.Linecast(PivotPoint.transform.position, MaxSightRange.position, 1 << LayerMask.NameToLayer("Enemy")); // returns true if line touches an enemy (1<<8)
    }
}
