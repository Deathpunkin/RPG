using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

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

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        

        public void AdjustHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            damageTaken = damage;
            //DamageTextController.CreateFloatingDamageText(damageTaken.ToString(), gameObject.transform);

            if (currentHealthPoints <= 0)
            {
                if(isProp)
                {
                    return;
                }
                Destroy(gameObject, 0.1f);
                StartCoroutine("awardExp");

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
            print("Enemy Regen!");
            yield return new WaitForSeconds(1);
        }

        void Start()
        {
            player = FindObjectOfType<Player>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            currentHealthPoints = maxHealthPoints;
            DamageTextController.Initialize();
        }

        void Update()
        {
            Transform chasestop;
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius && !isAttacking)
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

            if (distanceToPlayer <= chaseRadius && distanceToPlayer >= attackRadius)
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

        }


        void SpawnProjectile()
        {
            print("EnemyFire!");
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