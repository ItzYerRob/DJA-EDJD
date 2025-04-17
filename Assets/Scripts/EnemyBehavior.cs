using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        agent = GetComponent<NavMeshAgent>();

        //Ensure the agent is on a valid NavMesh surface
        if (!IsAgentOnNavMesh(agent))
        {
            Debug.LogWarning("Agent is not on a valid NavMesh surface!", this);
            agent.enabled = false; //Disable agent to prevent errors
        }
    }

    void Update()
    {
        if (player != null && agent.enabled && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    //Check if the agent is on a valid NavMesh position
    private bool IsAgentOnNavMesh(NavMeshAgent agent)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(agent.transform.position, out hit, 1.0f, NavMesh.AllAreas);
    }
}
