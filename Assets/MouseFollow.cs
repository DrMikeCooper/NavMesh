using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {

    NavMeshAgent nv;
    MeshRenderer mr;
    public Material linkMaterial;
    public Material noLinkMaterial;
    PointSequence currentPoints = null;
    int currentIndex = 0;
    bool backwards;

    public AudioClip clip;

    public string clip2Name;
    private AudioClip clip2;

    Vector3? targetPoint = null;
    AudioSource source;

    // Use this for initialization
    void Start () {
        nv = GetComponent<NavMeshAgent>();
        mr = GetComponent<MeshRenderer>();
        clip2 = new AudioClip();
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (nv.currentOffMeshLinkData.offMeshLink != null)
        {
            mr.material = linkMaterial;
            if (currentPoints == null)
            {
                Vector3 pos = nv.currentOffMeshLinkData.startPos;
                currentPoints = nv.currentOffMeshLinkData.offMeshLink.endTransform.GetComponent<PointSequence>();
                backwards = (pos != currentPoints.allPoints[0].position);
                currentIndex = backwards ? currentPoints.allPoints.Length - 1 : 0;
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
            Transform target = currentPoints.allPoints[backwards ? currentIndex-1 : currentIndex+1];

            Vector3 pos = transform.position;
            pos = Vector3.MoveTowards(pos, target.position, 0.1f);
            if ((pos - target.position).magnitude < 0.05f)
            {
                if (!backwards)
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
                else
                {
                    if (currentIndex <= 1)
                    {
                        currentPoints = null;
                        nv.CompleteOffMeshLink();
                    }
                    else
                    {
                        currentIndex--;
                    }
                }
            }

            transform.position = pos;

        }

        //if (targetPoint != null && !source.isPlaying)
        //{
        //    nv.SetDestination(targetPoint.Value);
        //    targetPoint = null;
        //}

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
            source.clip = Resources.Load<AudioClip>(clip2Name);
            source.Play();
            //targetPoint = hitInfo.point;
            nv.SetDestination(hitInfo.point);
        }
    }
}
