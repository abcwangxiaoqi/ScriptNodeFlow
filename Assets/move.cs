using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class move : MonoBehaviour {

    public float angle = 0;

	// Use this for initialization
	void Start () {

        Vector2 dir = new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad));

        Debug.LogError(">>" + dir);

        GetComponent<RectTransform>().anchoredPosition = dir * 50;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
