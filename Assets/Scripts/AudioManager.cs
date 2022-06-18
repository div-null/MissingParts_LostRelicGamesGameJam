using System.Collections;
using System.Collections.Generic;
using Infrastructure;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private Queue<float> queue;
    private ICoroutineRunner _runner;

    public void MakeJoinNoise()
    {
        if (queue.Count == 0)
        {
            queue.Enqueue(0);
                //_runner.StartCoroutine(PlayJoinNoise);
        }
        else
        {
            var delay = queue.Peek()+0.1f;
            queue.Enqueue(delay);
            
        }
    }

    /*
    IEnumerator PlayJoinNoise()
    {
        while (queue.TryPeek(out var delay))
        {
            
        }
    }
    */
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
