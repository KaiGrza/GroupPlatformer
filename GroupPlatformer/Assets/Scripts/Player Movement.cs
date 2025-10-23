using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement main;
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
    public Animator animator;
    public SpriteRenderer sr;
    Vector3 lastGrounded = Vector3.zero;
    public bool respawnAtLastGround = true;
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f,1f,0.3f,0.1f);
        Gizmos.DrawCube(transform.position, new Vector3(rect.width, rect.height, 1));
        Gizmos.color = new Color(1f, .9f, 0f, 0.1f);
        Gizmos.DrawSphere(lastGrounded, 0.1f);
    }
    private void Awake()
    {
        main = this;
        lastGrounded = transform.position;
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
        transform.position += (Vector3)velocity * Time.deltaTime;
        ground = velocity.y > 0 ? new RaycastHit2D() : Physics2D.Raycast(transform.position, -Vector2.up, rect.height * .5f + .05f, (1 << 0) + (1 << 6));
        rect.center = transform.position;
        Vector2[] searchLoc = new Vector2[] { new Vector2(rect.center.x, rect.yMin), new Vector2(rect.xMax, rect.center.y), new Vector2(rect.center.x, rect.yMax), new Vector2(rect.xMin, rect.center.y) };
        foreach (Collider2D col in Physics2D.OverlapAreaAll(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMax)))
        {
            float dist = 2f;
            Vector2 dir = Vector2.zero;
            for (int l = 0; l < 4; l++)
            {
                dir = col.ClosestPoint(searchLoc[l]) - (Vector2)transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude+0.05f, (1 << 0) + (1 << 6));
                dir = dir.normalized * hit.distance;
                Debug.DrawLine(transform.position,hit.point, Color.yellow);
                dist = dir.magnitude - CastInsideRect(dir, rect);
                transform.position += (Vector3)dir.normalized * Mathf.Min(0, dist);

                if (!ground && Vector2.Dot(hit.normal, Vector2.up) > .5f)
                    ground = Physics2D.Raycast(transform.position, dir, dir.magnitude + .05f, (1 << 0) + (1 << 6));
                if (Vector2.Dot(hit.normal, velocity) <0 && dist < 0f)
                {
                    dir = Vector2.Perpendicular(hit.normal);
                    dir = dir * Vector2.Dot(velocity, dir);
                    velocity = dir;// + (dir-velocity)*.1;
                }

                if (dist < 0 && col.gameObject.tag == "Hazard")
                    Respawn();
            }
        }
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
            animator.SetFloat("walk", 0);

        }
        else
        {
            if(ground.collider.gameObject.tag=="Hazard")
            {
                Respawn();
            }else
            {
                if (!Input.GetKey(KeyCode.Space))
                    velocity += Vector2.right * moveDir.x * Time.deltaTime * moveSpeed;
                velocity *= Mathf.Pow(0.00001f, Time.deltaTime);
                velocity.y = 0;
                reserveDash = true;

                animator.SetFloat("walk", Mathf.Abs(velocity.x));
                if (respawnAtLastGround&& Physics2D.Raycast(transform.position - Vector3.right * rect.width*.5f, -Vector2.up, rect.height * .5f + .1f, (1 << 0) + (1 << 6)) && Physics2D.Raycast(transform.position + Vector3.right *rect.width * .5f, -Vector2.up, rect.height * .5f + .1f, (1 << 0) + (1 << 6)))
                    lastGrounded = transform.position;
            }
        }
        if (moveDir.x != 0) sr.flipX = moveDir.x < 0;
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
    public void Respawn()
    {
        transform.position = lastGrounded;
        velocity = Vector2.zero;
        BulletMode = false;
        reserveDash = false;
        defaultRender.SetActive(true);
        DashRender.SetActive(false);
    }
}
