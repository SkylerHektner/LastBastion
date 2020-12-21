using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] Rail RightRail;
    [SerializeField] Rail LeftRail;

    private bool onLeftSide = true;
    private Vector3 target_position;
    bool moving = false;

    private void Start()
    {
        target_position = transform.position;
    }

    private void Update()
    {
        if( moving )
        {
            if((transform.position - target_position).magnitude < MoveSpeed * Time.deltaTime)
            {
                transform.position = target_position;
                moving = false;
            }
            else
                transform.position = transform.position + ( target_position - transform.position ).normalized * MoveSpeed * Time.deltaTime;
        }
    }

    public void GoToSide()
    {
        Vector3 touch_postition = Vector3.zero;
#if PC
        touch_postition = Input.mousePosition;
#endif
#if MOBILE
        touch_postition = Input.GetTouch( 0 ).position;
#endif

        Vector3 world_pos = Camera.main.ScreenToWorldPoint( touch_postition );
        world_pos.z = 0;

        Rail new_rail = onLeftSide ? RightRail : LeftRail;
        if( world_pos.y < new_rail.Bottom )
            world_pos.y = new_rail.Bottom;
        else if( world_pos.y > new_rail.Top )
            world_pos.y = new_rail.Top;
        world_pos.x = new_rail.RailCenter;

        target_position = world_pos;
        moving = true;
        onLeftSide = !onLeftSide;
    }
}
