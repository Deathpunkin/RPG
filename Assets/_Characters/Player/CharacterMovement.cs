using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    //[RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        ThirdPersonCharacter character;   // A reference to the ThirdPersonCharacter on the object
        //AICharacterControl aiCharacterControl = null;
        GameObject walkTarget;
        [SerializeField] float stoppingDistance = 0.2f;
        Vector3 movePoint;
        NavMeshAgent agent;

        // TODO solve fight between serialize and const
        [SerializeField] const int walkableLayerNumber = 8;
        [SerializeField] const int enemyLayerNumber = 9;

        bool isInDirectMode = true;
        private bool Jump;

        void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            character = GetComponent<ThirdPersonCharacter>();
            //aiCharacterControl = GetComponent<AICharacterControl>();
            walkTarget = new GameObject("walkTarget");

            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.stoppingDistance = stoppingDistance;

            cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            //cameraRaycaster.onMouseOverLootable += OnMouseOverLootable; //TODO Fix when Loot works
        }

        void Update()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, true, true);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                agent.SetDestination(enemy.transform.position);
                character.Move(enemy.transform.position, false, false);
                movePoint = transform.position;
            }
        }
        void OnMouseOverLootable(Vector3 lootable)
        {
            return;
        }
        void OnMouseOverWalkable(Vector3 destination)
        {
            //TODO figure out why destination is mouse cursor
            movePoint = destination;
            if (Input.GetMouseButton(0) && destination.magnitude > stoppingDistance)
            {
                agent.SetDestination(destination);
            }
            if(destination.magnitude <= stoppingDistance)
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        //Jump
        private void FixedUpdate()
        {
            if (!Jump)
            {
                Jump = Input.GetButtonDown("Jump");
            }
        }

        // TODO make this get called again
        void ProcessDirectMovement()
        {
            print("Direct Movement");

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // calculate camera relative direction to move:
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

            character.Move(movement, false, Jump);
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, movePoint);
        }

    }
}