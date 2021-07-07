using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MyDefaultTrackableEventHandler : DefaultTrackableEventHandler
{
    
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        transform.SetParent(null, true);
    }
}

