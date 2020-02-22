using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAI : MonoBehaviour
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void Initialize()
	{

	}

    public virtual void SendBoardInformations(int[] mainBoard, int opponentCount)
	{

	}

    public virtual void BeginThinking()
    {

    }

    public virtual int FetchThinkingAnswer()
    {
        return 0;
    }

    public virtual int FetchThinkingCount()
    {
        return 0;
    }
}
