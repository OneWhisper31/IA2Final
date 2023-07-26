using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class TimeSlicingProcessor
{
    public const float PreferedFramesPerSecond = 1f / 120f;

    public IEnumerator Operation(List<Action> process)
    {
        Stopwatch myStopWatch = new Stopwatch();
        myStopWatch.Start();

        int frames = 0;

        for (int i = 0; i < process.Count; i++)
        {
            process[i].Invoke();

            if (PreferedFramesPerSecond <= myStopWatch.ElapsedMilliseconds)
            {
                yield return new WaitForEndOfFrame();
                frames += 1;
                myStopWatch.Restart();
            }
        }

        UnityEngine.Debug.LogWarning("Tardó " + frames + " frames en completar la acción");
    }


}