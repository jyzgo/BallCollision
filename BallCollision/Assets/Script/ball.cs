using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {
    void Awake()
    { }
    public Vector3 speed = new Vector3(0, 0.1f, 0);
	// Use this for initialization
	void Start () {
        maxX = LevelMgr.current.maxX;
        maxY = LevelMgr.current.maxY;

        minX = LevelMgr.current.minX;
        minY = LevelMgr.current.minY;

        DontGoThroughStart();
       // StartCoroutine(move());
	}

    public float maxX;
    public float minX;
    public float maxY;
    public float minY;




    public void BeBlock(Cell cell)
    {
        LevelMgr.current.PlayPop();
        var off = transform.position - cell.transform.position;
        float x = Mathf.Abs(off.x);
        float y = Mathf.Abs(off.y);

        var small = Mathf.Min(x, y);
        var big = Mathf.Max(x, y);
        if (small / big > 0.85f)
        {
            speed = off.normalized * LevelMgr.BASE_SPEED;
        }
        else
        {

            if (x > y)
            {
                if (off.x > 0)
                {

                    speed = new Vector3(Mathf.Abs(speed.x), speed.y, speed.z);
                }
                else
                {
                    speed = new Vector3(Mathf.Abs(speed.x) * -1, speed.y, speed.z);
                }

            }
            else //if (x < y)
            {
                if (off.y > 0)
                {
                    speed = new Vector3(speed.x, Mathf.Abs(speed.y), speed.z);
                }
                else
                {
                    speed = new Vector3(speed.x, Mathf.Abs(speed.y) * -1, speed.z);
                }

            }
        }
        transform.position += speed * 0.5f;
        //else
        //{
        //    speed = new Vector3(speed.x * -1, speed.y * -1, speed.z);
        //}

        SpeedDown();
        cell.OnBallHit();
    }



    #region DontGothrough

    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector2 previousPosition;
    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;

    //initialize values 
    void DontGoThroughStart()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        previousPosition = myRigidbody.position;
        minimumExtent = 0.1f; //Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;

    }



    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        Vector2 movementThisStep = myRigidbody.position - previousPosition;
        float movementSqrMagnitude = speed.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            //RaycastHit2D hitInfo;

            //check for obstructions we might have missed 

            var hitInfo = Physics2D.Raycast(previousPosition, speed, movementSqrMagnitude, layerMask.value);
            if (hitInfo.collider != null)
            {



                //  if (hitInfo.collider.isTrigger)

                hitInfo.collider.SendMessage("OnTriggerEnter2D", myCollider,SendMessageOptions.DontRequireReceiver);

                //if (!hitInfo.collider.isTrigger)
                //    myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;

            }
        }

        previousPosition = myRigidbody.position;


        var p = transform.position;
        if (p.x >= maxX)
        {
            speed.x = Mathf.Abs(speed.x) * -1;
            SpeedDown();
        }
        else if (p.x <= minX)
        {
            speed.x = Mathf.Abs(speed.x);
            SpeedDown();
        }

        if (p.y >= maxY)
        {
            speed.y = Mathf.Abs(speed.y) * -1;
        }
        else if (p.y <= minY)
        {

            LevelMgr.current.TouchGround(this);
           // LevelMgr.current.Retrive(this);
            speed.y = Mathf.Abs(speed.y);
        }

        transform.position += speed;





    }

    void SpeedDown()
    {
        speed += Vector3.down * 0.0001f;
        speed = speed.normalized * LevelMgr.BASE_SPEED;
    }

    #endregion
}
