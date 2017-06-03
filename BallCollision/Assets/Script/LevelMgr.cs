using MTUnity.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using System;
using UnityEngine.UI;

public interface ICtrlAble
{
    void SetCtrlAble(bool b);
}

[DefaultExecutionOrder(-1)]
public class LevelMgr : MonoBehaviour {
    public enum LevelState
    {
        Menu,
        Playing,
        Fire,
        CellMoving,
        Win,
        Lose,
        ResetMenu

    }


    public Text Score;

    int currentScore = 0;
    int maxScore = 0;
    int increaseBall = 0;
    public void AddCurrentScore()
    {
        currentScore++;
        UpdateScore();
    }

    public Text BallText;
    public void IncreseBallNum(BallItem t)
    {
        Retrive(t);

        increaseBall++;
    }

    public void UpdateMoves()
    {
        Score.text = moveTimes.ToString();
    }
    
    public void UpdateScore()
    {
        //Score.text = moveTimes.ToString();// currentScore + "/" + maxScore; 
    }
   public Action<bool> CtrlListeners;

    public GameObject PlayMenu;
    public static LevelMgr current;
    public GameObject ballPrefab;
    public GameObject ballPos;

    public GameObject parPrefab;

    public ComponentPool<ParticleSystem> _parPool;



    public void Retrive(Cell cell)
    {
        cell.gameObject.SetActive(false);
        cell.transform.parent = null;
        _unusedCell.Add(cell);
        _usedCell.Remove(cell);
    }

    public void Retrive(ball b)
    {
        b.gameObject.SetActive(false);
        b.transform.parent = null;
        _unusedBall.Add(b);
        _usedBall.Remove(b);
    }

    public GameObject CellPrefab;
    public Dictionary<int, Cell> _cellDict = new Dictionary<int, Cell>();

    HashSet<ball> _unusedBall = new HashSet<ball>();
    HashSet<ball> _usedBall = new HashSet<ball>();

    StateMachine<LevelState> fsm;
    private void Awake()
    {
        current = this;
        AdMgr.RegisterAllAd();
        AdMgr.ShowDownAdmobBanner();
        Application.targetFrameRate = 60; 
        Vector3 max = new Vector3(Screen.width, Screen.height, 10f);
        Vector3 min = new Vector3(0, 0, 10f);

        max = Camera.main.ScreenToWorldPoint(max);
        min = Camera.main.ScreenToWorldPoint(min);

        maxX = 3.5f * CELL_SIDE;// max.x -r;
        maxY = max.y - r;

        minX = -3.5f * CELL_SIDE; // min.x + r;
        minY = min.y;//+ r;
        for (int i = 0; i < CELL_MAX_NUM; i++)
        {
            var gb = Instantiate<GameObject>(CellPrefab);
            gb.transform.position = new Vector3(100, 100, 0);
            gb.SetActive(false);
           
            var cell = gb.GetComponent<Cell>();
            _unusedCell.Add(cell);
        }

        for(int i = 0; i < BALL_MAX_NUM;i ++)
        {
            var gb = Instantiate<GameObject>(ballPrefab);
            var ball = gb.GetComponent<ball>();
            Retrive(ball);

        }

        for (int i = 0; i < BALL_ITEM_NUM; i++)
        {
            var gb = Instantiate<GameObject>(BallItemPrefab);
            var item = gb.GetComponent<BallItem>();
            Retrive(item);
        }

        _parPool = new ComponentPool<ParticleSystem>(0, parPrefab);

    }

    public void ShowPar(Vector3 pos)
    {
       var par =  _parPool.GenGameObject();

        par.transform.position = pos;
        StartCoroutine(Retrive(par));
        
    }

    public AudioSource _pop;

    public void PlayPop()
    {
        _pop.Play();
    }

    IEnumerator Retrive(ParticleSystem sys)
    {
        yield return new WaitForSeconds(0.3f);
        var em = sys.emission;
        em.enabled= false;
        Destroy(sys.gameObject,1f);
    }


    const int CELL_MAX_NUM = 20;
    const int BALL_MAX_NUM = 5;
    const int BALL_ITEM_NUM = 5;

    #region BallItem

    public GameObject BallItemPrefab;
    HashSet<BallItem> _unusedBallItem = new HashSet<BallItem>();
    HashSet<BallItem> _usedBallItem = new HashSet<BallItem>();

    public void Retrive(BallItem it)
    {
        it.gameObject.StopAllActions();
        it.gameObject.SetActive(false);
        it.transform.parent = null;
        _unusedBallItem.Add(it);
        _usedBallItem.Remove(it);
    }

    public BallItem GetUnusedBallItem()
    {
        BallItem item = null;
        if (_unusedBallItem.Count > 0)
        {
            var ce = _unusedBallItem.GetEnumerator();
            while (ce.MoveNext())
            {
                item = ce.Current;
                break;
            }
        }
        else
        {
            var gb = Instantiate<GameObject>(BallItemPrefab);
            item = gb.GetComponent<BallItem>();
        }
        _unusedBallItem.Remove(item);
        _usedBallItem.Add(item);
        item.gameObject.SetActive(true);
        item.StartAction();
        return item;
    }


    #endregion BallItem

    const float r = 0.2f;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;
    // Use this for initialization

    HashSet<Cell> _unusedCell = new HashSet<Cell>();
    HashSet<Cell> _usedCell = new HashSet<Cell>();
    void Start () {
        fsm = StateMachine<LevelState>.Initialize(this, LevelState.Menu);
        
	}

    public Transform startPos;
    private void ResetGame()
    {
        moveTimes = 0;
        startPos.position = new Vector3(0, startPos.position.y, 0);
        var cellEn = _cellDict.GetEnumerator();
        while (cellEn.MoveNext())
        {
            cellEn.Current.Value.gameObject.SetActive(false);
            _usedCell.Remove(cellEn.Current.Value);
            _unusedCell.Add(cellEn.Current.Value);
        }
        
        var itemEn = _usedBallItem.GetEnumerator();
        while(itemEn.MoveNext())
        {
            itemEn.Current.gameObject.SetActive(false);
            _unusedBallItem.Add(itemEn.Current);
            
        }
        _usedBallItem.Clear();
            
        
        _cellDict.Clear();
        currentScore = 0;
        maxScore = 0;
        ballcount = 1;
        moveDownConf = 1;
    }

    void Menu_Enter()
    {
        PlayMenu.SetActive(true);
        ResetGame();
        if (AdMgr.IsAdmobInterstitialReady())
        {
            AdMgr.ShowAdmobInterstitial();
        }

    }



    void Menu_Exit()
    {
        AdMgr.PreloadAdmobInterstitial();
        PlayMenu.SetActive(false);
        CellRoot.position = Vector3.zero;


        StartGame();

        UpdateScore();


    }

    public void PlayBtn_Click()
    {

        fsm.ChangeState(LevelState.Playing);
    }

    int ballcount = 1;
    public void Fire(Vector3 dir)
    {
        arrDir = dir;
        fsm.ChangeState(LevelState.Fire);

    }
    Vector3 arrDir = Vector3.zero;
    void Fire_Enter()
    {
        var n = arrDir.normalized;
        touchNum = 0;
        increaseBall = 0;
        StartCoroutine(DoFire(n));
    }
    int touchNum = 0;

    void Fire_Exit()
    {
        ballcount += increaseBall;
        UpdateBallCount();
    }

    void UpdateBallCount()
    {
        BallText.text = "x" + ballcount;
    }

    public Text lvText;

    const int MAX_LV = 6;

    int _genRow = 0;
    void GenCells()
    {

        for (int i = 0; i < 7; i++)
        {
            int hp = MTRandom.GetRandomInt(-1 - _genRow / 26 * 7, 3 + _genRow  * 7/ 5);
            if (hp != 0)
            {
                int r = MTRandom.GetRandomInt(1, 10);
                if (r % 2 != 0)
                {
                    GameObject gb = null;
                    if (hp > 0)
                    {
                        var cell = GetUnusedCell();
                        gb = cell.gameObject;


                        maxScore += hp;
                        cell.Init(hp);
                        if (!_cellDict.ContainsKey(gb.GetInstanceID()))
                        {
                            _cellDict.Add(gb.GetInstanceID(), cell);
                        }
                    }
                    else
                    {
                        var ballItem = GetUnusedBallItem();
                        gb = ballItem.gameObject;
                    }
                    gb.transform.SetParent(CellRoot);
                    gb.transform.localPosition = new Vector3(-3f * CELL_SIDE  + CELL_SIDE * i, 0f + (_genRow + 3) * CELL_SIDE, 0);
                    gb.SetActive(true);

                }

            }
        }

       
        

        _genRow++;
    }
    void StartGame()
    {
        _genRow = -2;
        for (int i = 0; i < 5; i++)
        {
            GenCells();
        }


        
        UpdateBallCount();

    }

    int moveDownConf = 0;
    int moveTimes = 0;
    void initLvConf(string[] conf)
    {
        ballcount = Convert.ToInt32(conf[0]);
        int xIndex = Convert.ToInt32(conf[1]);
        if (conf[2] == "")
        {
            moveDownConf = 0;
        }
        else
        {
            moveDownConf = Convert.ToInt32(conf[2]);
        }

        float startX = -2.5f * CELL_SIDE; //minX + 0.2f + CELL_SIDE * (xIndex % 7);
        var curB = ballPos.transform.position;
        ballPos.transform.position = new Vector3(startX, curB.y, curB.z);

    }



    public void TouchGround(ball b)
    {
        if (touchNum == 0)
        {
            var last = ballPos.transform.position;
            ballPos.transform.position = new Vector3(b.transform.position.x, last.y, last.z);
            Retrive(b);

        }
        else
        {
            b.RunActions(new MTMoveTo(0.2f, ballPos.transform.position, true), new MTCallFunc(() => Retrive(b)));
        }
        touchNum++;
        if (touchNum >= ballcount)
        {
                fsm.ChangeState(LevelState.CellMoving);

        }
    }

    IEnumerator DoFire(Vector3 dir)
    {
        var curB = ballPos.transform.position;
        for (int i = 0; i < ballcount; i++)
        {
            var ball = GetUnusedBall();
            ball.transform.position = curB;
            ball.speed = dir * BASE_SPEED;
            yield return new WaitForSeconds(FIRE_INTERVAL);
        }
        
    }
    public const float BASE_SPEED = 0.1f;
    const float FIRE_INTERVAL = 0.1f;
    Cell GetUnusedCell()
    {
        Cell curCell = null;
        if (_unusedCell.Count > 0)
        {
            var ce = _unusedCell.GetEnumerator();
            while (ce.MoveNext())
            {
                curCell = ce.Current;
                break;
            }
        }
        else
        {
            var gb = Instantiate<GameObject>(CellPrefab);
            curCell = gb.GetComponent<Cell>();
        }
        _unusedCell.Remove(curCell);
        _usedCell.Add(curCell);
        return curCell;
        
    }


    ball GetUnusedBall()
    {
        ball b = null;
        if (_unusedBall.Count > 0)
        {
            var ce = _unusedBall.GetEnumerator();
            while (ce.MoveNext())
            {
                b = ce.Current;
                break;
            }
        }
        else
        {
            var gb = Instantiate<GameObject>(ballPrefab);
            b = gb.GetComponent<ball>();
        }
        _unusedBall.Remove(b);
        _usedBall.Add(b);
        b.gameObject.SetActive(true);
        return b;
    }

    void Playing_Enter()
    {


        if (CtrlListeners != null)
        {
            CtrlListeners(true);
        }
        BallText.gameObject.SetActive(true); 


    }

    const float CELL_SIDE = 0.78f;

    void Playing_Exit()
    {
        if (CtrlListeners != null)
        {
            CtrlListeners(false);
        }
        BallText.gameObject.SetActive(false);
    }
	
    

    public Transform CellRoot;

    #region CellMoving

    const float MOVE_CELL_TIME = 0.4f;
    IEnumerator CellMoving_Enter()
    {
        GenCells();
        moveTimes++;
        UpdateMoves();


        CellRoot.gameObject.RunAction(new MTMoveBy(MOVE_CELL_TIME * moveDownConf, new Vector3(0, -CELL_SIDE * moveDownConf, 0)));
        yield return new WaitForSeconds(MOVE_CELL_TIME * moveDownConf);
        CheckSucess();
        
    }

    public Transform DeadZone;
    void CheckSucess()
    {

        if (_usedCell.Count == 0)
        {
            fsm.ChangeState(LevelState.Win);
            return;
        }

        float deadY = DeadZone.position.y;
        var en = _usedCell.GetEnumerator();
        while(en.MoveNext())
        {
            var lowY = en.Current.transform.position.y;
            if (lowY < deadY)
            {
                fsm.ChangeState(LevelState.Lose);
                return;
            }
        }
        fsm.ChangeState(LevelState.Playing);
    }

    #endregion CellMoving

    #region Win
    public GameObject WinMenu;
    void Win_Enter()
    {
        WinMenu.SetActive(true);
    }

    void Win_Exit()
    {
        WinMenu.SetActive(false);
    }

    #endregion Win;


    public void ToMenu()
    {
        fsm.ChangeState(LevelState.Menu);
    }
    #region Lose
    public GameObject LoseMenu;
    void Lose_Enter()
    {
        LoseMenu.SetActive(true);
    }

    void Lose_Exit()
    {
        LoseMenu.SetActive(false);
    }
    #endregion Lose
}
