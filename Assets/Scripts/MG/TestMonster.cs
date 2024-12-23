using UnityEngine;
using UnityEngine.AI;

public class TestMonster : MonoBehaviour
{
    private NavMeshAgent agent;
    public LayerMask isTarget;

    [SerializeField]
    private float Sqhere = 20f;
}
