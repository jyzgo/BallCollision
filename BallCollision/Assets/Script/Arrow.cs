using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public GameObject ballPoint;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 16; i++)
        {
            var g = Instantiate<GameObject>(ballPoint);
            g.transform.SetParent(transform);
            g.transform.localPosition = new Vector3(-0.1f * (i+1), 0, 0);
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
