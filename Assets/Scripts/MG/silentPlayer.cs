using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.AI;

public class silentPlayer : MonoBehaviour
{
    private CharacterController characterController;
    public NavMeshSurface nms;
    public NavMeshAgent nmsAgent;

    [SerializeField][Range(5f, 10f)]
    private float movespeed = 5f;
    [SerializeField]
    [Range(50f, 200f)]
    private float rotatespeed = 50f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        nms.BuildNavMesh();
    }

    private void Update()
    {
        float axisV = Input.GetAxis("Vertical");
        float axisH = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * axisV * movespeed;
        characterController.SimpleMove(move);

        transform.Rotate(0, axisH * rotatespeed * Time.deltaTime,0);
    }

}
