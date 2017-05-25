using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    public SpriteRenderer sp;
    public Text num;
    public void Init(int hp)
    {
        _hp = hp;
        UpdateHp();
    }

    int _hp = 0;

    Color or = new Color(0.627f, 0, 0.309f);
    void UpdateHp()
    {
        float g = 1f - (float)_hp / 15f;
        
        sp.color = new Color(or.r, g, or.b);
        if (_hp <= 0)
        {
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
