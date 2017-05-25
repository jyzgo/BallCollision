using UnityEngine;
using System.Collections;
using System;

public class BGResize : MonoBehaviour,ICtrlAble {

    SpriteRenderer sr;
    public Transform ballPos;
    public Transform startPos;
    public Transform endPos;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

	// Use this for initialization
	void Start () {
        Resize();
        LevelMgr.current.CtrlListeners += SetCtrlAble;

    }
    void Resize()
    {


        transform.localScale = new Vector3(1, 1, 1);

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;


        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 xWidth = transform.localScale;
        xWidth.x = worldScreenWidth / width;
        transform.localScale = xWidth;
        //transform.localScale.x = worldScreenWidth / width;
        Vector3 yHeight = transform.localScale;
        yHeight.y = worldScreenHeight / height;
        transform.localScale = yHeight;
        //transform.localScale.y = worldScreenHeight / height;

    }

    public void SetBg(Sprite sp)
    {
        sr.sprite = sp;
        Resize();
    }

    Vector3 touchPos = Vector3.zero;
    Vector3 dragPos = Vector3.zero;
    private void OnMouseDown()
    {

        if (_ctrlAble)
        {
            var v3 = Input.mousePosition;
            v3.z = 10f;
            touchPos = Camera.main.ScreenToWorldPoint(v3);
            startPos.position = touchPos;
            arrow.transform.position = ballPos.position;
            //var inst = Instantiate<GameObject>(ballPrefab);
            //var sc = inst.GetComponent<ball>();
            //var nSp = v3 - startPos.position;
            //sc.speed = nSp.normalized * 0.2f;
            //inst.transform.position = startPos.position;
        }

    }

    private void OnMouseDrag()
    {
        if (_ctrlAble)
        {
            var v3 = Input.mousePosition;
            v3.z = 10f;
            dragPos = Camera.main.ScreenToWorldPoint(v3);

            endPos.position = dragPos;

            if (dragPos.y > touchPos.y || Vector3.Distance(dragPos,touchPos) <= MIN_DIS)
            {
                arrow.SetActive(false);
            }
            else
            {
                
                arrow.SetActive(true);
                arrow.transform.position = ballPos.position;
                Vector3 vectorToTarget = dragPos - startPos.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

                arrow.transform.rotation = q;

                

                var distance = Vector3.Distance(dragPos, touchPos);
                float scale = Mathf.Clamp(distance / DIS, 1f, 4f);
                arrow.transform.localScale = new Vector3(scale,scale);
            }


        }
    }

    const float DIS = 0.5f;
    const float MIN_DIS = 0.4f;
    private void OnMouseUp()
    {
        arrow.SetActive(false);

        if(_ctrlAble)
        {
            if (dragPos.y < touchPos.y)
            {
                if (Vector3.Distance(dragPos, touchPos) > MIN_DIS)
                {
                    LevelMgr.current.Fire(  touchPos-dragPos);
                }
            }
        }
    }

    bool _ctrlAble = false;
    public void SetCtrlAble(bool b)
    {
        _ctrlAble = b;
    }

    public GameObject arrow;

}
