
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private EnemyPath enemyPath;

    [Header("Movement")]
    [SerializeField] private float speed = 1.5f;

    private int currentWaypointIndex = 0;

    private void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (enemyPath == null || enemyPath.waypoints == null ||
            enemyPath.waypoints.Length == 0)
        {
            return;
        }

        if (currentWaypointIndex >= enemyPath.waypoints.Length)
        {
            return;
        }

        Transform targetWaypoint =
            enemyPath.waypoints[currentWaypointIndex];

        if (targetWaypoint == null)
        {
            currentWaypointIndex++;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            speed * Time.deltaTime
        );

        float distance = Vector2.Distance(
            transform.position,
            targetWaypoint.position
        );

        if (distance < 0.05f)
        {
            currentWaypointIndex++;
        }
    }
}
