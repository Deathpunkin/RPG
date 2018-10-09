using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        Animator animator;
        ParticleSystem lootableParticle;
        public float level = 1;
        public float maxHealthPoints = 100f;
        float currentHealthPoints;
        float regenHealthspeed = 1f;

        [SerializeField] float chaseRadius = 6f;

        [SerializeField] float attackRadius = 4f;
        public float damagePerShot = 9f;
        public float attackSpeed = 0.5f;
        [SerializeField] float attackSpeedVariation = 0.1f;
        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        bool isAttacking = false;
        AICharacterControl aiCharacterControl = null;
        Player player = null;
        float damageTaken;
        public float dodgechance = 10f;

        [SerializeField] bool isProp = false;
        public bool lootable = false;
        GameObject enemyUI;
        Collider collider;
        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            damageTaken = damage;
            //DamageTextController.CreateFloatingDamageText(damageTaken.ToString(), gameObject.transform);

            if (currentHealthPoints <= 0)
            {
                if (isProp)
                {
                    return;
                }
                enemyUI.SetActive(false);
                collider.enabled = false;
                if (lootable)
                {
                    CancelInvoke("SpawnProjectile");
                    aiCharacterControl.SetTarget(transform);
                    animator.SetTrigger("Dead");
                    lootableParticle.Play();
                    gameObject.tag = "Lootable";
                    gameObject.layer = LayerMask.NameToLayer("Lootable");
                }
                else
                {
                    CancelInvoke("SpawnProjectile");
                    aiCharacterControl.SetTarget(transform);
                    animator.SetTrigger("Dead");
                    Destroy(gameObject, 10f); // Adjust time Body stays around.
                }
               
            }
        }

        IEnumerator awardExp()
        {
            print("Gained Exp!");
            //TODO Uncomment when fixed Exp
            // players.experiencePoints = players.experiencePoints + experienceAwarded;
            yield return null;
        }
        IEnumerator regenHealth()
        {
            currentHealthPoints = currentHealthPoints + regenHealthspeed;
            //        regenHealthSpeed = regenHealthSpeed +1 ;
            yield return new WaitForSeconds(1);
        }

        void Start()
        {
            player = FindObjectOfType<Player>();
            animator = GetComponent<Animator>();
            lootableParticle = GetComponentInChildren<ParticleSystem>();
            enemyUI = this.gameObject.transform.GetChild(0).gameObject;
            collider = GetComponent<Collider>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            currentHealthPoints = maxHealthPoints;
            DamageTextController.Initialize();
        }

        void Update()
        {
            Transform chasestop;
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius && !isAttacking && currentHealthPoints > 0)
            {
                isAttacking = true;
                float randomizedAttackSpeed = Random.Range(attackSpeed - attackSpeedVariation, attackSpeed + attackSpeedVariation);
                aiCharacterControl.SetTarget(transform);
                InvokeRepeating("SpawnProjectile", 0f, randomizedAttackSpeed); // TODO switch to Coroutine
            }

            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }

            if (distanceToPlayer <= chaseRadius && distanceToPlayer >= attackRadius && currentHealthPoints > 0)
            {
                aiCharacterControl.SetTarget(player.transform);
            }
            else
            {
                aiCharacterControl.SetTarget(transform);
            }
            if (player.healthAsPercentage <= Mathf.Epsilon)
            {
                StopAllCoroutines();
            }
            if (isProp)
            {
                if (currentHealthPoints < maxHealthPoints)
                {
                    StartCoroutine("regenHealth", 5f);
                }
            }
            if (Random.Range(0, 500) == 20)
            {
                lootable = true;
                print("Loot Me!");
            }
        }


        void SpawnProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetDamage(damagePerShot);
            projectileComponent.SetShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.projectileSpeed;
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // Draw chase sphere 
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }

}