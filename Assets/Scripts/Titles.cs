using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titles : MonoBehaviour {

    MainManager mainManager;

    Animator animator;
	// Use this for initialization
	void Start () {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTrigger(string s)
    {
        animator.SetTrigger(s);
    }

    public void OnAnimationEnd()
    {
        mainManager.SetActivePanelTitle();
        animator.SetTrigger("Idle");
    }

    public void OnAnimationEnd2()
    {
        mainManager.MoveToMain();
    }
}
