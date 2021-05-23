using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] protected Vector3 MoveDirection = Vector3.zero;
    [SerializeField] protected WallHitBehavior CurWallHitBehavior;
    public GameObject DestroyEffect;

    public UnityEvent<ProjectileHitInfo> ProjectileHitWallEvent = new UnityEvent<ProjectileHitInfo>();

    public enum WallHitBehavior
    {
        Bounce,
        Attach,
        Destroy,
        DoNothing, // danger zone https://www.youtube.com/watch?v=siwpn14IE7E
    }

    public void SetWallHitBehavior( WallHitBehavior behavior )
    {
        CurWallHitBehavior = behavior;
    }
    public void SetProjectileSpeed( float speed )
    {
        MoveSpeed = speed;
    }

    public void StartMoveInDirection( Vector3 move_direction )
    {
        this.MoveDirection = move_direction.normalized;
    }
    public Vector3 GetMoveDirection()
    {
        return this.MoveDirection;
    }

    protected virtual void Start()
    {
        // nothing for now
    }

    protected virtual void Update()
    {
        if( MoveDirection != Vector3.zero )
        {
            Vector3 new_pos = transform.position + MoveDirection * MoveSpeed * Time.deltaTime * GameplayManager.TimeScale;

            bool hit_wall = false;
            // hit bottom
            if( new_pos.y < Rails.Instance.Bottom )
            {
                ProjectileHitWallEvent.Invoke( new ProjectileHitInfo( ProjectileHitInfo.Wall.Bottom, new_pos ) );
                hit_wall = true;
                if( CurWallHitBehavior == WallHitBehavior.Attach )
                {
                    new_pos.y = Rails.Instance.Top;
                    MoveDirection = Vector3.zero;
                }
                else if( CurWallHitBehavior == WallHitBehavior.Bounce )
                {
                    new_pos.y += ( Rails.Instance.Bottom - new_pos.y ) * 2;
                    MoveDirection.y = -MoveDirection.y;
                }
            }

            // hit top
            if( new_pos.y > Rails.Instance.Top )
            {
                ProjectileHitWallEvent.Invoke( new ProjectileHitInfo( ProjectileHitInfo.Wall.Top, new_pos ) );
                hit_wall = true;
                if( CurWallHitBehavior == WallHitBehavior.Attach )
                {
                    new_pos.y = Rails.Instance.Top;
                    MoveDirection = Vector3.zero;
                }
                else if( CurWallHitBehavior == WallHitBehavior.Bounce )
                {
                    new_pos.y -= ( new_pos.y - Rails.Instance.Top ) * 2;
                    MoveDirection.y = -MoveDirection.y;
                }
            }

            // hit right
            if( new_pos.x > Rails.Instance.Right )
            {
                ProjectileHitWallEvent.Invoke( new ProjectileHitInfo( ProjectileHitInfo.Wall.Right, new_pos ) );
                hit_wall = true;
                if( CurWallHitBehavior == WallHitBehavior.Attach )
                {
                    new_pos.x = Rails.Instance.Right;
                    MoveDirection = Vector3.zero;
                }
                else if( CurWallHitBehavior == WallHitBehavior.Bounce )
                {
                    new_pos.x -= ( new_pos.x - Rails.Instance.Right ) * 2;
                    MoveDirection.x = -MoveDirection.x;
                }
            }

            // hit left
            if( new_pos.x < Rails.Instance.Left )
            {
                ProjectileHitWallEvent.Invoke( new ProjectileHitInfo( ProjectileHitInfo.Wall.Left, new_pos ) );
                hit_wall = true;
                if( CurWallHitBehavior == WallHitBehavior.Attach )
                {
                    new_pos.x = Rails.Instance.Left;
                    MoveDirection = Vector3.zero;
                }
                else if( CurWallHitBehavior == WallHitBehavior.Bounce )
                {
                    new_pos.x += ( Rails.Instance.Left - new_pos.x ) * 2;
                    MoveDirection.x = -MoveDirection.x;
                }
            }

            transform.position = new_pos;

            if( hit_wall && CurWallHitBehavior == WallHitBehavior.Destroy )
                DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        if( DestroyEffect != null )
            Instantiate( DestroyEffect ).transform.position = transform.position;
        Destroy( gameObject );
    }
}

public class ProjectileHitInfo
{
    public enum Wall
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public Wall wall;
    public Vector3 point;

    public ProjectileHitInfo( Wall _wall, Vector3 _point )
    {
        wall = _wall;
        point = _point;
    }
}