using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    //[RequireComponent(typeof(AICharacterControl))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float moveThreshold = 1f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float jumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float gravityMultiplier = 2f;
        [SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] float animationSpeedMultiplier = 1f;
        [SerializeField] float groundCheckDistance = 0.1f;
        bool m_IsGrounded; //TODO move all "IsGrounded" here.
        Animator animator;
        Rigidbody rigidBody;
        bool isGrounded;
        float origGroundCheckDistance;
        const float k_Half = 0.5f;
        float turnAmount;
        float forwardAmount;
        Vector3 groundNormal;
        float capsuleHeight;
        Vector3 capsuleCenter;
        CapsuleCollider capsule;
        public bool crouching;
        Player player;

        //AICharacterControl aiCharacterControl = null;
        GameObject walkTarget;
        [SerializeField] float stoppingDistance = 0.2f;
        [SerializeField] float moveSpeedMultiplier = 0.7f;
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
            //aiCharacterControl = GetComponent<AICharacterControl>();
            walkTarget = new GameObject("walkTarget");
            animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            capsuleHeight = capsule.height;
            capsuleCenter = capsule.center;
            player = FindObjectOfType<Player>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            origGroundCheckDistance = groundCheckDistance;

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
                Move(agent.desiredVelocity, true, true);
            }
            else
            {
                ProcessDirectMovement();
                Move(Vector3.zero, false, false);
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                agent.SetDestination(enemy.transform.position);
                Move(enemy.transform.position, false, false);
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
                Move(Vector3.zero, false, false);
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



        // TODO make this work with navmesh/click to move
        void ProcessDirectMovement()
        {
            print("Direct Movement");

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // calculate camera relative direction to move:
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

            Move(movement, false, Jump);
        }

        public void Move(Vector3 movement, bool crouch, bool jump)
        {
            if (!player.isDead)
            {
                SetForwardAndTurn(movement, crouch, jump);
            }
            else
            {
                return;
            }
        }

        private void SetForwardAndTurn(Vector3 movement, bool crouch, bool jump)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.

            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }
            movement = transform.InverseTransformDirection(movement);
            CheckGroundStatus();
            movement = Vector3.ProjectOnPlane(movement, groundNormal);
            turnAmount = Mathf.Atan2(movement.x, movement.z);
            forwardAmount = movement.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (isGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            // send input and other state parameters to the animator
            UpdateAnimator(movement);
        }

        void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", crouching);
            animator.SetBool("OnGround", isGrounded);
            if (!isGrounded)
            {
                animator.SetFloat("Jump", rigidBody.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * forwardAmount;
            if (isGrounded)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (isGrounded && move.magnitude > 0)
            {
                animator.speed = animationSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                animator.speed = 1;
            }
        }


        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigidBody.AddForce(extraGravityForce);

            groundCheckDistance = rigidBody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                // jump!
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
                isGrounded = false;
                animator.applyRootMotion = false;
                groundCheckDistance = 0.1f;
            }
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                velocity.y = rigidBody.velocity.y;
                rigidBody.velocity = velocity;
            }
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, movePoint);
        }
        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                groundNormal = hitInfo.normal;
                isGrounded = true;
                animator.applyRootMotion = true;
            }
            else
            {
                isGrounded = false;
                groundNormal = Vector3.up;
                animator.applyRootMotion = false;
            }
        }
    }
}