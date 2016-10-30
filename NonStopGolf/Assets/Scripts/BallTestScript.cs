using UnityEngine;
using System.Collections;

public class BallTestScript : MonoBehaviour
{
    public GameObject hole;
    public BoardCreator.TileType onTileType;
    public Vector2 previousPosition;

    Transform sprite;
    Vector2 dir;
    Vector2 holePosition;

    Rigidbody2D rbody;
    RaycastHit2D hit;

    LineRenderer line;

    public float jumpPowerCap;
    float initialVelocity;
    float height;
    float velocity;
    float power;
    float accuracy;
    float spriteGravity = -1f;

    bool isAirborne = true;
    bool bounced = false;
    bool inHole = false;
    bool isMoving = false;
    bool isColliding = false;
    bool treeHit = false;
    bool checkedForTrees = false;

    void Start()
    {
        Physics2D.gravity = new Vector2(0f, 0f);
        hole = GameObject.Find("Hole");
        sprite = transform.FindChild("BallSprite");
        dir = hole.transform.position - transform.position;


        line = GetComponent<LineRenderer>();
        rbody = GetComponent<Rigidbody2D>();
    }

    public void Hit()
    {
        previousPosition = transform.position;
        bounced = false;
        height = 0;
        initialVelocity = 0;

        if (!treeHit)
        {
            dir = hole.transform.position - transform.position;
        }

        //dir.Normalize();        

        power = GameManager.instance.inputManager.GetPower();
        accuracy = GameManager.instance.inputManager.GetAccuracy();

        rbody.AddForce(power * dir.normalized, ForceMode2D.Impulse);

        if (power > jumpPowerCap)
        {
            initialVelocity = power / 6;
            velocity = initialVelocity;
            height = 0.1f;
            isAirborne = true;
        }

        StartCoroutine(Arc());

        checkedForTrees = false;
        treeHit = false;
    }

    void FixedUpdate()
    {
        if (!isColliding)
        {
            onTileType = GameManager.instance.boardCreator.GetTileTypeFromPosition(transform.position);
        }

        SetFriction();

        if (!inHole && (rbody.velocity.x >= -0.05 && rbody.velocity.x <= 0.05 && rbody.velocity.y >= -0.05 && rbody.velocity.y <= 0.05) && !GameManager.instance.inputManager.inputActive)
        {
            GameManager.instance.inputManager.StartPower();
        }

        isMoving = (Mathf.Abs(rbody.velocity.x) >= 0.05 || Mathf.Abs(rbody.velocity.y) >= 0.05);

        if (!isMoving)
        {
            DrawLineToHole();
            line.enabled = true;

            if (!checkedForTrees)
            { 
                CheckForTrees();
            }
        }

        else
        {
            line.enabled = false;
        }

        if (isAirborne)
        {
            velocity += spriteGravity * 3 * Time.deltaTime;
            height += velocity * 3 * Time.deltaTime;

            sprite.localPosition = new Vector3(0, height, 0);
            sprite.localScale = new Vector3(height + 1, height + 1, 1);
        }

        if (height <= Mathf.Epsilon)
        {
            isAirborne = false;
            sprite.localPosition = Vector2.zero;

            if (onTileType == BoardCreator.TileType.outOfBounds || onTileType == BoardCreator.TileType.water)
            {
                rbody.velocity = Vector2.zero;
                transform.position = previousPosition;
                height = 0;
                velocity = 0;
                isAirborne = false;
                Physics2D.gravity = Vector3.zero;
            }

            if (!bounced)
            {
                Bounce();
            }
        }

        if (!isAirborne && Mathf.Abs(rbody.velocity.x) < 2f && Mathf.Abs(rbody.velocity.y) < 2f && Vector2.Distance(hole.transform.position, transform.position) < 0.1f)
        {
            BallInTheHole();
        }
    }

    void Bounce()
    {
        if (initialVelocity > 0)
        {
            if (onTileType == BoardCreator.TileType.fairway || onTileType == BoardCreator.TileType.rough)
            {
                velocity = initialVelocity / 2;

                height = 0.1f;
                isAirborne = true;
                bounced = true;
            }

            else
            {
                rbody.velocity = Vector2.zero;
                height = 0;
                velocity = 0;
                isAirborne = false;
                Physics2D.gravity = Vector3.zero;
            }
        }
    }

    void BallInTheHole()
    {
        transform.position = hole.transform.position;
        inHole = true;
        GameManager.instance.BallHole();
    }

    void SetFriction()
    {
        if (height > Mathf.Epsilon)
        {
            rbody.drag = 1;
        }

        else
        {
            switch (onTileType)
            {
                case (BoardCreator.TileType.fairway):
                    rbody.drag = 1;
                    break;
                case (BoardCreator.TileType.rough):
                    rbody.drag = 2;
                    break;
                case (BoardCreator.TileType.sand):
                    rbody.drag = 4;
                    break;
                default:
                    rbody.drag = 1;
                    break;
            }
        }
    }

    public void Reset()
    {
        inHole = false;
        //transform.position = new Vector3(-5f, -3f, 0);
    }

    void DrawLineToHole()
    {
        line.SetPosition(0, transform.position);
        if (!treeHit && !checkedForTrees)
        {
            dir = hole.transform.position - transform.position;
        }

        //line.SetPosition(1, Vector2.LerpUnclamped(transform.position, hole.transform.position, GameManager.instance.inputManager.powerSlider.maxValue / Vector2.Distance(transform.position, hole.transform.position)));
        line.SetPosition(1, Vector2.LerpUnclamped(transform.position,(Vector2) transform.position + dir , GameManager.instance.inputManager.powerSlider.maxValue / Vector2.Distance(transform.position, hole.transform.position)));

    }

    void CheckForTrees()
    {
        hit = Physics2D.Raycast(transform.position, dir, power);
        if (hit.collider != null && !hit.transform.name.Equals("Hole") )
        {
            treeHit = true;
            PointAwayFromTree();
            checkedForTrees = true;
        }
    }

    IEnumerator Arc()
    {
        yield return new WaitForSeconds(0.2f);
        Physics2D.gravity = accuracy * 50 * Perpendicular(dir.normalized) / 10;
        yield return new WaitForSeconds(0.6f);
        Physics2D.gravity = Vector2.zero;
    }

    public static Vector2 Perpendicular(Vector2 original)
    {
        float x = original.x;
        float y = original.y;

        y = -y;

        return new Vector2(y, x);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        isColliding = true;
        Debug.Log("iscolliding");
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }

    public void PointAwayFromTree ()
    {
        Vector2 newDir = new Vector2();
        newDir = hole.transform.position - transform.position;
        //newDir.Normalize();
        //newDir *= GameManager.instance.inputManager.powerSlider.maxValue;

        while (hit.collider != null)
        {
            hit = Physics2D.Raycast(transform.position, newDir, GameManager.instance.inputManager.powerSlider.maxValue);
            newDir.x++;
        }

        if (GameManager.instance.boardCreator.IsFairway((Vector2)transform.position + newDir.normalized * GameManager.instance.inputManager.powerSlider.maxValue))
        {
            dir = newDir;
            return;
        }

        else if (!GameManager.instance.boardCreator.IsOutOfBounds((Vector2)transform.position + newDir.normalized * GameManager.instance.inputManager.powerSlider.maxValue))
        {
            dir = newDir;
        }

        else
        {
            while (hit.collider != null)
            {
                hit = Physics2D.Raycast(transform.position, newDir, GameManager.instance.inputManager.powerSlider.maxValue);
                newDir.x--;
            }

            if (GameManager.instance.boardCreator.IsFairway((Vector2)transform.position + newDir.normalized * GameManager.instance.inputManager.powerSlider.maxValue))
            {
                dir = newDir;
                return;
            }

            else
            {
                dir = newDir;
            }
        }
    }
}
