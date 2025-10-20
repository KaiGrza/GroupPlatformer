using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    public Rect rect;
    public Vector2 velocity;
    public RaycastHit2D ground;
    public float moveSpeed;
    public bool BulletMode;
    bool reserveDash;
    public Vector3 bulletTarget;
    float bte = 0f;
    public GameObject defaultRender;
    public GameObject DashRender;
    public ParticleSystem onHitWallEffect;
    void Start()
    {
        
    }
    public static float CastInsideRect(Vector3 dir,Rect rect)
    {
        return Mathf.Min(Mathf.Abs(dir.normalized.x == 0 ? 100 : rect.size.x * 0.5f / dir.normalized.x), Mathf.Abs(dir.normalized.y == 0 ? 100 : rect.size.y * 0.5f / dir.normalized.y));
    }
    public static Vector2 TurnCardinal(Vector2 dir)
    {
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Vector2.right * Mathf.Sign(dir.x): Vector2.up * Mathf.Sign(dir.y);
    }
    private void FixedUpdate()
    {
        if (BulletMode)
        {
            Vector3 dir = Vector3.ClampMagnitude(bulletTarget - transform.position, Time.deltaTime * 100f);
            transform.position += dir;
            if ((transform.position - bulletTarget).magnitude < 0.2f)
            {
                bte += Time.deltaTime;
                if(DashRender.activeInHierarchy)
                {
                    defaultRender.SetActive(true);
                    DashRender.SetActive(false);
                    onHitWallEffect.transform.position = bulletTarget;
                    onHitWallEffect.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                    onHitWallEffect.Play();
                }
            }
            if (bte > .2f)
            {
                BulletMode = false;
                bte = 0f;
                transform.position = bulletTarget;
                reserveDash = false;
            }
            return;
        }
        ground = velocity.y > 0 ? new RaycastHit2D() : Physics2D.Raycast(transform.position, -Vector2.up, rect.height * .5f + .05f, (1 << 0) + (1 << 6));
        rect.center = transform.position;
        foreach (Collider2D col in Physics2D.OverlapAreaAll(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMax)))
        {
            Vector2 dir = col.ClosestPoint(transform.position) - (Vector2)transform.position;
            transform.position += (Vector3)dir.normalized * Mathf.Min(0, dir.magnitude - CastInsideRect(dir,rect));

            if (!ground && Vector2.Dot(dir, Vector2.up) < -.5f)
                ground = Physics2D.Raycast(transform.position, dir, dir.magnitude + .05f, (1 << 0) +(1 << 6));
            if (Vector2.Dot(dir, velocity) > 0)
            {
                dir = Vector2.Perpendicular(TurnCardinal(- dir)).normalized;
                dir = dir * Vector2.Dot(velocity, dir);
                velocity = dir;// + (dir-velocity)*.1;
            }
        }
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
    void Update()
    {
        if (BulletMode) return;
        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(!ground)
        {
            velocity += Physics2D.gravity * Time.deltaTime;
            velocity += Vector2.right * moveDir.x * Time.deltaTime * 10f;
            velocity.x*= Mathf.Pow(0.3f, Time.deltaTime);

        }
        else
        {
            if(!Input.GetKey(KeyCode.Space))
            velocity += Vector2.right * moveDir.x * Time.deltaTime * moveSpeed;
            velocity *= Mathf.Pow(0.00001f, Time.deltaTime);
            velocity.y = 0;
            reserveDash = true;

        }
        RaycastHit2D hit;
        if (reserveDash &&Input.GetKeyUp(KeyCode.Space) && moveDir.magnitude != 0 && (hit = Physics2D.Raycast(transform.position, TurnCardinal(moveDir), 100, 1 << 0)))
        {
            BulletMode = true;
            defaultRender.SetActive(false);
            DashRender.SetActive(true);
            bulletTarget = hit.point + hit.normal * CastInsideRect(hit.normal,rect);
            velocity = Vector2.zero;
            reserveDash = false;
            ground = new RaycastHit2D();
        }
    }
}
