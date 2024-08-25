using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class YieldInstructionCache
{
    //public static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private static readonly Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float second)
    {
        WaitForSeconds wfs;

        if(!waitForSeconds.TryGetValue(second, out wfs))  
        {
            waitForSeconds.Add(second, wfs = new WaitForSeconds(second));
        }
        return wfs;
    }
}
