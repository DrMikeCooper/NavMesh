using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {

    NavMeshAgent nv;
    MeshRenderer mr;
    public Material linkMaterial;
    public Material noLinkMaterial;
    PointSequence currentPoints = null;
    int currentIndex = 0;

    // Use this for initialization
    void Start () {
        nv = GetComponent<NavMeshAgent>();
        mr = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (nv.currentOffMeshLinkData.offMeshLink != null)
        {
            mr.material = linkMaterial;
            if (currentPoints == null)
            {
                currentPoints = nv.currentOffMeshLinkData.offMeshLink.endTransform.GetComponent<PointSequence>();
                currentIndex = 0;
            }
        }
        else
        {
            mr.material = noLinkMaterial;
        }

        // move along the list of transforms if we have some
        if (currentPoints != null)
        {
            // at the end, we want to return to the NavMesh
            Transform from = currentPoints.allPoints[currentIndex];
            Transform target = currentPoints.allPoints[currentIndex+1];

            Vector3 pos = transform.position;
            pos = Vector3.MoveTowards(pos, target.position, 0.1f);
            if ((pos - target.position).magnitude < 0.05f)
            {
                if (currentIndex >= currentPoints.allPoints.Length - 2)
                {
                    currentPoints = null;
                    nv.CompleteOffMeshLink();
                }
                else
                {
                    currentIndex++;
                }
            }

            transform.position = pos;

        }

        bool touch = false;
        Vector3 mousepos = new Vector3(0, 0, 0);

#if MOBILE_INPUT
        if (Input.touchCount > 0)
        {
            touch = true;
            mousepos = Input.GetTouch(0).position;
        }
#else
        if (Input.GetMouseButton(0))
        {
            touch = true;
            mousepos = Input.mousePosition;
        }
#endif
        if (!touch)
            return;

        Ray ray = Camera.main.ScreenPointToRay(mousepos);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo))
        {
            nv.SetDestination(hitInfo.point);
        }


    }
}
