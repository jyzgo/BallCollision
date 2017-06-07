using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    readonly Color[] colors = {
        new Color(0.96f, 0.71f, 0.18f),
        new Color(0.52f,0.77f,0.31f),
        new Color(0.64f,0.56f,0.32f),
        new Color(0.89f,0.07f,0.37f),
        new Color(0.76f,0.13f,0.53f),
        new Color(0.09f,0.45f,0.74f),
        new Color(0f,0.6f,0.55f),
        new Color(0.88f,0.32f,0.12f),
        new Color(0.24f,0.15f,0.14f),
        new Color(0.1f,0.14f,0.49f),
        new Color(0.72f,0.11f,0.11f) };

     int[] LV_ARR = new int []{
         5,
        10,
        15,
        30,
        50,
        100,
        150,
        200,
        300,
        400
        };

    Color GetCurrentColor()
    {

        return colors[GetCurrentColorIndex()];
    }

    int GetCurrentColorIndex()
    {
        for (int i = 0; i < LV_ARR.Length; i++)
        {
            if (_hp < LV_ARR[i])
            {
                return i;
            }
        }
        return colors.Length - 1;
    }

    Color GetPreColor()
    {
        int index = GetCurrentColorIndex()-1;
        if (index > 0)
        {
            return colors[index];
        }
        return colors[0];

    }
    void InitColor()
    {
        sp.color = GetCurrentColor();
    }
    Vector3 _offsetColor;
    void UpdateColor()
    {
        if (_hp > LV_ARR[0]) {

            var currentColor = GetCurrentColor();
            var preColor = GetPreColor();
            int currentIndex =  GetCurrentColorIndex();
            int indexHp = LV_ARR[currentIndex];
            int preHp = LV_ARR[currentIndex - 1];
            float per = (float)(_hp - preHp) / (float)(indexHp - preHp);

            sp.color = Color.Lerp(currentColor, preColor,per);
        }

        
    }
    public SpriteRenderer sp;
    public Text num;
    public void Init(int hp)
    {
        _hp = hp;
        InitColor();
        UpdateHp();
    }

    int _hp = 0;

    Color or = new Color(0.627f, 0, 0.309f);
    void UpdateHp()
    {
        //UpdateColor();
        if (_hp <= 0)
        {
            LevelMgr.current.ShowPar(transform.position);
            LevelMgr.current.Retrive(this);

        }
        else
        {

            num.text = _hp.ToString();
        }

    }



    int instID = 0;
    private void OnDestroy()
    {

        // LevelMgr.current._cellDict.Remove(instID);
    }

    public void OnBallHit()
    {
        _hp--;
        LevelMgr.current.AddCurrentScore();
        UpdateHp();
        UpdateColor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        var ball = collision.GetComponent<ball>();
        if (ball != null)
        {

            ball.BeBlock(this);
            // mo = false;


        }
    }


}
