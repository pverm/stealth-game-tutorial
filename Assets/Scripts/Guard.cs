using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;
    public float speed = 4;
    public float delay = .3f;
    public float turnSpeed = 90;
    public Light spotlight;
    public float viewDistance;
    public LayerMask obstacleMask;
    public float timeToBeSpotted = 1f;
    public static event Action OnPlayerSpotted;

    private float viewAngle;
    private Transform player;
    private Color spotLightColor;
    private float spotMeter;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        spotLightColor = spotlight.color;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];


        for (int i=0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
        }

        StartCoroutine(FollowPath(waypoints));
    }

    IEnumerator FollowPath(Vector3[] waypoints) {
        int i = 0;
        Vector3 nextWaypoint = transform.position;
        transform.LookAt(nextWaypoint);

        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, speed * Time.deltaTime);
            if (transform.position == nextWaypoint) {
                i = (i + 1) % waypoints.Length;
                nextWaypoint = waypoints[i];
                yield return new WaitForSeconds(delay);
                yield return StartCoroutine(TurnTo(nextWaypoint));
            }
            yield return null;
        }
        
    }

    IEnumerator TurnTo(Vector3 turnTarget) {
        Vector3 direction = (turnTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }


    private void OnDrawGizmos() {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder) {
            Gizmos.DrawSphere(waypoint.position, .5f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    bool canSeePlayer() {
        if (Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f) {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Linecast(transform.position, player.position, obstacleMask)) {
                    return true;
                }
            }
        }
        return false;
    }

    void Update() {
        if (canSeePlayer()) {
            spotMeter += Time.deltaTime;
        } else {
            spotMeter -= Time.deltaTime;
        }
        spotMeter = Mathf.Clamp(spotMeter, 0, timeToBeSpotted);
        spotlight.color = Color.Lerp(spotLightColor, Color.red, spotMeter / timeToBeSpotted);

        if (spotMeter == timeToBeSpotted) {
            if (OnPlayerSpotted != null) {
                OnPlayerSpotted();
            }
        }
    }
}
