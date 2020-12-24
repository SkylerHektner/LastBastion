using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] float MinDragDistance = 1.0f;
    [SerializeField] float MaxDragDistance = 5.0f; // this is just the max for graphical purposes
    [SerializeField] GameObject DirectionArrow;
    [SerializeField] GameObject DirectionArrowPivot;
    [SerializeField] float MinimumAngleDegrees = 15;

    private bool on_left_side = true;
    private bool dragging = false;
    private Vector3 drag_start_position;
    private Vector3 drag_last_position;
    private Color drag_arrow_color = new Color( 1, 1, 1, 0 );

    private Vector3 move_direction = Vector3.zero;

    private void Start()
    {
        DirectionArrow.SetActive( false );
    }

    private void Update()
    {
        if( dragging )
        {
            bool still_dragging;

#if PC || UNITY_EDITOR
            still_dragging = Input.GetMouseButton( 0 );
#endif
#if MOBILE && !UNITY_EDITOR
            still_dragging = Input.touchCount > 0;
#endif
            if( still_dragging )
            {
                Vector3 touch_postition;
#if PC || UNITY_EDITOR
                touch_postition = Input.mousePosition;
#endif
#if MOBILE && !UNITY_EDITOR
                touch_postition = Input.GetTouch( 0 ).position;
#endif
                drag_last_position = Camera.main.ScreenToWorldPoint( touch_postition );

                UpdateDragArrowGraphics();
            }
            else
            {
                DragEnded();
            }


        }

        if( move_direction != Vector3.zero )
        {
            // hit top or bottom? Bounce
            Vector3 new_pos = transform.position + move_direction * MoveSpeed * Time.deltaTime * GameplayManager.GamePlayTimeScale;
            if( new_pos.y < Rails.Instance.Bottom )
            {
                new_pos.y += ( Rails.Instance.Bottom - new_pos.y ) * 2;
                move_direction.y = -move_direction.y;
            }
            if( new_pos.y > Rails.Instance.Top )
            {
                new_pos.y -= ( new_pos.y - Rails.Instance.Top ) * 2;
                move_direction.y = -move_direction.y;
            }

            // Hit left or right? Stop
            if( new_pos.x > Rails.Instance.Right )
            {
                new_pos.x = Rails.Instance.Right;
                move_direction = Vector3.zero;
            }
            if( new_pos.x < Rails.Instance.Left )
            {
                new_pos.x = Rails.Instance.Left;
                move_direction = Vector3.zero;
            }

            transform.position = new_pos;
        }
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        col.gameObject.GetComponent<Enemy>().Hit( move_direction );
    }

    private void UpdateDragArrowGraphics()
    {
        VerifyDragLastPosition();
        Vector3 delta_vec = ( drag_last_position - drag_start_position );
        float drag_dist = Mathf.Min( delta_vec.magnitude, MaxDragDistance );

        if( drag_dist > MinDragDistance )
        {
            drag_arrow_color.a = Mathf.Min( drag_dist / ( MaxDragDistance - MinDragDistance ), 1.0f );
            float theta = Mathf.Atan2( delta_vec.y, delta_vec.x ) * Mathf.Rad2Deg;
            DirectionArrowPivot.transform.rotation = Quaternion.Euler( 0, 0, theta );

            DirectionArrow.transform.localPosition = new Vector3( drag_dist * 0.2f + 1.0f, 0, 0 );
        }
        else
            drag_arrow_color.a = 0.0f;

        DirectionArrow.GetComponent<Renderer>().material.SetColor( "_Color", drag_arrow_color );
    }

    private void DragEnded()
    {
        VerifyDragLastPosition();
        dragging = false;
        on_left_side = !on_left_side;
        DirectionArrow.SetActive( false );
        if( move_direction == Vector3.zero )
            move_direction = ( drag_last_position - drag_start_position ).normalized;
    }

    // corrects the drag last position, if needed, to ensure we don't go past our minimum angle
    private void VerifyDragLastPosition()
    {
        Vector3 delta_vec = ( drag_last_position - drag_start_position );
        if( delta_vec.x < 0.0f && on_left_side ) delta_vec.x = 0.0f;
        if( delta_vec.x > 0.0f && !on_left_side ) delta_vec.x = 0.0f;
        float necessary_x = Mathf.Tan( MinimumAngleDegrees * Mathf.Deg2Rad ) * Mathf.Abs( delta_vec.y );
        if( necessary_x > Mathf.Abs( delta_vec.x ) )
        {
            delta_vec.x = ( on_left_side ? 1.0f : -1.0f ) * necessary_x;
            drag_last_position = drag_start_position + delta_vec;
        }
    }

    public void DragStarted()
    {
        if( dragging )
        {
            Debug.LogWarning( "Somehow a drag has started even though we're in a drag???!?!?!?!?!???!?!!?!??" );
            return;
        }

        Vector3 touch_postition = Vector3.zero;
#if PC || UNITY_EDITOR
        touch_postition = Input.mousePosition;
#endif
#if MOBILE && !UNITY_EDITOR
        touch_postition = Input.GetTouch( 0 ).position;
#endif
        drag_start_position = Camera.main.ScreenToWorldPoint( touch_postition );
        dragging = true;

        DirectionArrow.SetActive( true );
        drag_arrow_color.a = 0.0f;
        DirectionArrow.GetComponent<Renderer>().material.SetColor( "_Color", drag_arrow_color );
    }
}
