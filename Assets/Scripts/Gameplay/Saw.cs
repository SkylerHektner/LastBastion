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
    public UnityEvent<Vector3> SawKilledEnemyEvent = new UnityEvent<Vector3>();
    public UnityEvent<long> SawHitEnemyEvent = new UnityEvent<long>();
    public UnityEvent SawMuddiedEvent = new UnityEvent();

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

    public bool OnFire { get { return on_fire_duration > 0.0f; } }
    private float on_fire_duration = -1.0f;
    private int on_fire_extra_damage = 0;
    private float on_fire_movespeed_multiplier = 1.0f;

    private float cover_in_mud_duration = -1.0f;
    private float covered_in_mud_movespeed_multiplier = 1.0f;

    public Animator SawmageddonFX;
    public Animator AnomalyFX;
    public Animator TyphoonFX;


    private void Start()
    {
#if UNITY_EDITOR
        if( Instance != null )
            Debug.LogError( "There appear to me two main saws?!?" );
#endif
        Instance = this;

        DirectionArrow.sprite = DirectionArrowSprite;
        DirectionArrow.gameObject.SetActive( false );
        proj = GetComponent<Projectile>();
        proj.ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
    }

    private void Update()
    {
        if( OnFire )
        {
            on_fire_duration -= Time.deltaTime * GameplayManager.TimeScale;
            if( on_fire_duration <= 0.0f )
                EndOnFire();
            TyphoonFX.SetFloat( "Duration", on_fire_duration );
        }
        AnomalyFX.SetFloat( "Duration", AnomalyAbility.AnimatorDuration );
        SawmageddonFX.SetFloat( "Duration", SawmageddonAbility.AnimatorDuration );

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

        if( cover_in_mud_duration != -1.0f )
        {
            cover_in_mud_duration -= Time.deltaTime * GameplayManager.TimeScale;
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

            // light saw on fire if flame saw upgrade and typhoon active
            if( hit_info.wall == ProjectileHitInfo.Wall.Bottom 
                && TyphoonAbility.ActiveTyphoon != null 
                && PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.TyphoonFlameSaw ) )
            {
                TyphoonAbility.ActiveTyphoon.SetSawOnFire( this );
            }
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
        {
            Enemy en = col.gameObject.GetComponent<Enemy>();
            Debug.Assert( en != null );
            Vector3 pos = en.transform.position;

            bool died;
            bool dodged;
            long id = en.EnemyID;
            en.Hit( move_direction, true, out died, out dodged, 1 + ( OnFire ? on_fire_extra_damage : 0 ) );

            if( died )
                SawKilledEnemyEvent.Invoke( pos );
            if( !dodged )
                SawHitEnemyEvent.Invoke( id );
        }
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
            proj.SetProjectileSpeed( MoveSpeed * covered_in_mud_movespeed_multiplier * on_fire_movespeed_multiplier );
            proj.StartMoveInDirection( move_direction );
            SawFiredEvent.Invoke( transform.position, move_direction, MoveSpeed );
        }
    }

    // corrects the drag last position, if needed, to ensure we don't go past our minimum angle
    private void VerifyDragLastPosition()
    {
        Vector3 delta_vec = ( drag_last_position - drag_start_position );
        if( delta_vec.x < 0.0f && on_left_side )
            delta_vec.x = 0.0f;
        if( delta_vec.x > 0.0f && !on_left_side )
            delta_vec.x = 0.0f;
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
#if UNITY_EDITOR
            Debug.LogWarning( "Somehow a drag has started even though we're in a drag???!?!?!?!?!???!?!!?!??" );
#endif
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

    public void TryCoverInMud( float duration, float move_speed_mult )
    {
        if( OnFire ) // flaming saws don't get covered in mud
            return;

        SawMuddiedEvent.Invoke();

        if( cover_in_mud_duration != -1.0f )
            cover_in_mud_duration = Mathf.Max( duration, cover_in_mud_duration );
        else
            cover_in_mud_duration = duration;
        animator.SetBool( "Muddy", true );
        covered_in_mud_movespeed_multiplier = move_speed_mult;
        proj.SetProjectileSpeed( MoveSpeed * covered_in_mud_movespeed_multiplier );
    }

    private void EndCoverInMud()
    {
        cover_in_mud_duration = -1.0f;
        animator.SetBool( "Muddy", false );
        covered_in_mud_movespeed_multiplier = 1.0f;
        proj.SetProjectileSpeed( MoveSpeed * covered_in_mud_movespeed_multiplier );
    }

    public void SetOnFire( float duration, int extra_damage, float move_speed_multiplier )
    {
        if( cover_in_mud_duration > 0.0f )
            EndCoverInMud(); // clear cover in mud status effect if we have it

        // if already on fire keep effect with longest duration
        if( on_fire_duration < duration )
        {
            on_fire_duration = duration;
            on_fire_extra_damage = extra_damage;
            on_fire_movespeed_multiplier = move_speed_multiplier;

            // set new movespeed on proj
            proj.SetProjectileSpeed( MoveSpeed * covered_in_mud_movespeed_multiplier * on_fire_movespeed_multiplier );
        }
    }

    private void EndOnFire()
    {
        on_fire_duration = -1.0f;
        on_fire_extra_damage = 0;
        on_fire_movespeed_multiplier = 1.0f;
    }
}
