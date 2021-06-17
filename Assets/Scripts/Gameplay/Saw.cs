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
    public bool Moving { get { return MoveDirection != Vector3.zero; } }
    public float AdjustedMoveSpeed { get { return MoveSpeed * covered_in_mud_movespeed_multiplier * on_fire_movespeed_multiplier; } }
    public Vector3 MoveDirection { get { return proj.GetMoveDirection(); } }

    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] float MinDragDistance = 1.0f;
    [SerializeField] float MaxDragDistance = 5.0f; // this is just the max for graphical purposes
    [SerializeField] SpriteRenderer DirectionArrow;
    [SerializeField] Sprite DirectionArrowSprite;
    [SerializeField] Sprite DirectionArrowGreyedSprite;
    [SerializeField] GameObject DirectionArrowPivot;
    [SerializeField] float MinimumAngleDegrees = 15;
    [SerializeField] private Animator animator;
    [SerializeField] float SawCorrectionTime = 3.0f; // if saw hasn't hit a wall in this time a correction mechanism resets its position

    private bool on_left_side { get { return transform.position.x < 0.0f; } }
    private bool dragging = false;
    private Vector3 drag_start_position;
    private Vector3 drag_last_position;
    private Color drag_arrow_color = new Color( 1, 1, 1, 0 );

    private Projectile proj;

    public bool OnFire { get { return on_fire_duration > 0.0f; } }
    private float on_fire_duration = -1.0f;
    private int on_fire_extra_damage = 0;
    private float on_fire_movespeed_multiplier = 1.0f;

    private float cover_in_mud_duration = -1.0f;
    private float covered_in_mud_movespeed_multiplier = 1.0f;

    private float correction_timer = 0.0f;

    public Animator SawmageddonFX;
    public Animator AnomalyFX;
    public Animator TyphoonFX;

    public void SetMoveDirection( Vector3 move_direction, float speed )
    {
        proj.SetProjectileSpeed( speed );
        proj.StartMoveInDirection( move_direction );
    }

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
        if( Moving )
        {
            correction_timer += Time.deltaTime;
            if( correction_timer > SawCorrectionTime )
            {
                correction_timer = 0.0f;
                Vector3 rails_center_left = new Vector3( Rails.Instance.Left, Rails.Instance.Bottom + Rails.Instance.Top / 2, 0.0f );
                SetMoveDirection( ( rails_center_left - transform.position ).normalized, AdjustedMoveSpeed );
            }
        }

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
                && PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UnlockFlags.TyphoonFlameSaw, GameplayManager.Instance.Survival ) )
            {
                TyphoonAbility.ActiveTyphoon.SetSawOnFire( this );
            }
        }
        else if( hit_info.wall == ProjectileHitInfo.Wall.Left ||
            hit_info.wall == ProjectileHitInfo.Wall.Right )
        {
            proj.SetWallHitBehavior( Projectile.WallHitBehavior.Attach );
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
            en.Hit( MoveDirection,
                true,
                OnFire ? DamageSource.FlamingSaw : DamageSource.Saw,
                out died,
                out dodged,
                1 + ( OnFire ? on_fire_extra_damage : 0 ) );

            if( died )
                SawKilledEnemyEvent.Invoke( pos );
            if( !dodged )
                SawHitEnemyEvent.Invoke( id );

            if( !died && en.Bouncy && AnomalyAbility.ActiveAnomaly == null /*anomaly allows the saw to bypass bounce*/ )
            {
                Vector2 new_move_direction = MathUtility.ReflectVector( MoveDirection, transform.position - en.transform.position );
                // some safety to make sure our new move direction isn't too up/down
                if( new_move_direction.x == 0.0f )
                    new_move_direction.x = 0.01f;
                float theta = Mathf.Atan( new_move_direction.y / new_move_direction.x );
                float delta = ( Mathf.PI * 0.5f ) - Mathf.Abs( theta );
                if( delta < MinimumAngleDegrees * Mathf.Deg2Rad )
                {
                    new_move_direction.x = 0;
                    new_move_direction = MathUtility.RotateVector2D( new_move_direction, MinimumAngleDegrees * Mathf.Deg2Rad * -Mathf.Sign( theta ) );
                }

                SetMoveDirection( new_move_direction, AdjustedMoveSpeed );
            }
        }
        else if( col.tag == "Mudball" && !Moving )
            col.gameObject.GetComponent<MudSlingerProjectile>().HitSaw( this );
        else if( col.tag == "AbilityDrop" )
        {
            AbilityDrop drop = col.gameObject.GetComponent<AbilityDrop>();
            if( !drop.JustSpawned )
                drop.UseAbility();
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
            SetMoveDirection( ( drag_last_position - drag_start_position ).normalized, AdjustedMoveSpeed );
            SawFiredEvent.Invoke( transform.position, MoveDirection, MoveSpeed );
            correction_timer = 0.0f;
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
        proj.SetProjectileSpeed( AdjustedMoveSpeed );
    }

    private void EndCoverInMud()
    {
        cover_in_mud_duration = -1.0f;
        animator.SetBool( "Muddy", false );
        covered_in_mud_movespeed_multiplier = 1.0f;
        proj.SetProjectileSpeed( AdjustedMoveSpeed );
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
            proj.SetProjectileSpeed( AdjustedMoveSpeed );
        }
    }

    private void EndOnFire()
    {
        on_fire_duration = -1.0f;
        on_fire_extra_damage = 0;
        on_fire_movespeed_multiplier = 1.0f;
    }
}
