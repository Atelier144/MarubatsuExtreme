﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAIWeak : BoardAI
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void BeginThinking()
    {
        Debug.Log("WEAK THINKING");
    }
}