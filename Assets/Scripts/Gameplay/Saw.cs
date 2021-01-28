using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof( Projectile ) )]
public class Saw : MonoBehaviour
{
    public static Saw Instance;

    // position, direction, speed
    public UnityEvent<Vector3, Vector3, float> SawFiredEvent = new UnityEvent<Vector3, Vector3, float>();

    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] float MinDragDistance = 1.0f;
    [SerializeField] float MaxDragDistance = 5.0f; // this is just the max for graphical purposes
    [SerializeField] SpriteRenderer DirectionArrow;
    [SerializeField] Sprite DirectionArrowSprite;
    [SerializeField] Sprite DirectionArrowGreyedSprite;
    [SerializeField] GameObject DirectionArrowPivot;
    [SerializeField] float MinimumAngleDegrees = 15;
    [SerializeField] private Animator animator;

    private bool on_left_side = true;
    private bool dragging = false;
    private Vector3 drag_start_position;
    private Vector3 drag_last_position;
    private Color drag_arrow_color = new Color( 1, 1, 1, 0 );

    private bool Moving { get { return move_direction != Vector3.zero; } }
    private Vector3 move_direction = Vector3.zero;
    private Projectile proj;

    private float cover_in_mud_duration = -1.0f;
    private float cur_move_speed_multiplier = 1.0f;

    public Animator SawmageddonFX;
    public Animator AnomalyFX;
    public Animator TyphoonFX;


    private void Start()
    {
        if( Instance != null )
            Debug.LogError( "There appear to me two main saws?!?" );
        Instance = this;

        DirectionArrow.sprite = DirectionArrowSprite;
        DirectionArrow.gameObject.SetActive( false );
        proj = GetComponent<Projectile>();
        proj.ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
        proj.SetTimeScaleFilter( GameplayManager.TimeScale.UI ); // don't pay attention to temporal anomoly time scale
    }

    private void Update()
    {
        SawmageddonFX.SetFloat("Duration", SawmageddonAbility.AnimatorDuration);
        AnomalyFX.SetFloat("Duration", TemporalAnomalyAbility.AnimatorDuration);
        TyphoonFX.SetFloat("Duration", TyphoonAbility.AnimatorDuration);

        if ( dragging )
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

        if( cover_in_mud_duration != -1.0f )
        {
            cover_in_mud_duration -= Time.deltaTime * GameplayManager.GamePlayTimeScale;
            if( cover_in_mud_duration <= 0.0f )
                EndCoverInMud();
        }
    }


    private void OnProjectileHitWall( ProjectileHitInfo hit_info )
    {
        if( hit_info.wall == ProjectileHitInfo.Wall.Bottom ||
            hit_info.wall == ProjectileHitInfo.Wall.Top )
        {
            proj.SetWallHitBehavior( Projectile.WallHitBehavior.Bounce );
        }
        else if( hit_info.wall == ProjectileHitInfo.Wall.Left ||
            hit_info.wall == ProjectileHitInfo.Wall.Right )
        {
            proj.SetWallHitBehavior( Projectile.WallHitBehavior.Attach );
            move_direction = Vector3.zero;
            if( dragging )
                DirectionArrow.sprite = DirectionArrowSprite;
        }
    }
    private void OnTriggerEnter2D( Collider2D col )
    {
        if( col.tag == "Enemy" )
            col.gameObject.GetComponent<Enemy>().Hit( move_direction, true );
        else if( col.tag == "Mudball" && !Moving )
            col.gameObject.GetComponent<MudSlingerProjectile>().HitSaw( this );
        else if( col.tag == "AbilityDrop" )
        {
            AbilityDrop drop = col.gameObject.GetComponent<AbilityDrop>();
            if( !drop.JustSpawned )
                drop.AddAbilityCharge();
        }
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
        DirectionArrow.gameObject.SetActive( false );
        if( !Moving )
        {
            on_left_side = !on_left_side;
            move_direction = ( drag_last_position - drag_start_position ).normalized;
            proj.SetProjectileSpeed( MoveSpeed * cur_move_speed_multiplier );
            proj.StartMoveInDirection( move_direction );
            SawFiredEvent.Invoke( transform.position, move_direction, MoveSpeed );
        }
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

        DirectionArrow.gameObject.SetActive( true );
        DirectionArrow.sprite = Moving ? DirectionArrowGreyedSprite : DirectionArrowSprite;
        drag_arrow_color.a = 0.0f;
        DirectionArrow.GetComponent<Renderer>().material.SetColor( "_Color", drag_arrow_color );
    }

    public void CoverInMud( float duration, float move_speed_mult )
    {
        if( cover_in_mud_duration != -1.0f )
            cover_in_mud_duration = Mathf.Max( duration, cover_in_mud_duration );
        else
            cover_in_mud_duration = duration;
        animator.SetBool( "Muddy", true );
        cur_move_speed_multiplier = move_speed_mult;
        proj.SetProjectileSpeed( MoveSpeed * cur_move_speed_multiplier );
    }

    private void EndCoverInMud()
    {
        cover_in_mud_duration = -1.0f;
        animator.SetBool( "Muddy", false );
        cur_move_speed_multiplier = 1.0f;
        proj.SetProjectileSpeed( MoveSpeed * cur_move_speed_multiplier );
    }
}
