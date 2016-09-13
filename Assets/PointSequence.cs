using UnityEngine;
using System.Collections;

public class PointSequence : MonoBehaviour {

    public Transform[] points;

    [HideInInspector]
    public Transform[] allPoints;

	// Use this for initialization
	void Start () {
        OffMeshLink link = GetComponent<OffMeshLink>();
        int num = points.Length;
        allPoints = new Transform[num + 2];
        allPoints[0] = link.startTransform;
        for (int i=0; i<num; i++)
            allPoints[i+1] = points[i];
        allPoints[num + 1] = link.endTransform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
