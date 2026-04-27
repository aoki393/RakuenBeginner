using UnityEngine;
using PLAYERTWO.PlatformerProject;
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    public WaypointManager waypoints{get;protected set;}
    void Awake(){
        tag=GameTags.Platform;
        waypoints=GetComponent<WaypointManager>();
    }

    void LateUpdate()
    {
        var position = transform.position;
        var target = waypoints.Current.position;
        position = Vector3.MoveTowards(position, target, speed * Time.deltaTime);
        transform.position = position;

        if (Vector3.Distance(position, target) < 0.1f)
        {
            waypoints.Next();
        }
    }
}
