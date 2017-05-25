using MTUnity.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallItem : MonoBehaviour {

	// Use this for initialization
	void Start () {

        StartAction();
	}

    public void StartAction()
    {
        gameObject.StopAllActions();
        gameObject.RunAction(new MTRepeatForever(new MTSequence(new MTScaleTo(0.6f, 0.3f), new MTScaleTo(0.6f, 0.5f))));
    }
	


    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelMgr.current.IncreseBallNum(this);
    }
}
