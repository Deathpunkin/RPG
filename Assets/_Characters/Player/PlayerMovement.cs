using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {
        ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
        CameraRaycaster cameraRaycaster = null;
        AICharacterControl aiCharacterControl = null;
        GameObject walkTarget = null;
        [SerializeField] float walkStopDistance = 0.2f;
        Vector3 movePoint;

        // TODO solve fight between serialize and const
        [SerializeField] const int walkableLayerNumber = 8;
        [SerializeField] const int enemyLayerNumber = 9;

        bool isInDirectMode = true;
        private bool Jump;

        void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            walkTarget = new GameObject("walkTarget");

            cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                aiCharacterControl.SetTarget(enemy.transform);
                thirdPersonCharacter.Move(enemy.transform.position, false, false);
                movePoint = transform.position;
            }
        }

        void OnMouseOverWalkable(Vector3 destination)
        {
            //TODO figure out why destination is mouse cursor
            movePoint = destination;
            if (Input.GetMouseButton(0) && destination.magnitude > walkStopDistance)
            {
                //walkTarget.transform.position = destination;
               // thirdPersonCharacter.Move(destination, false, false);
                aiCharacterControl.SetTarget(walkTarget.transform);
            }
            if(destination.magnitude <= walkStopDistance)
            {
                thirdPersonCharacter.Move(Vector3.zero, false, false);
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

            thirdPersonCharacter.Move(movement, false, Jump);
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, movePoint);
        }

    }
}