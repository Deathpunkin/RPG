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
        [SerializeField] int level;
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float currentHealthPoints;
        [SerializeField] int baseExpReward = 100;
        int expReward;
        [SerializeField] bool isDead = false;
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
        //AICharacterControl aiCharacterControl = null;
        Character player = null;
        float damageTaken;
        public float dodgechance = 10f;

        [SerializeField] bool isProp = false;
        public bool lootable = false;
        GameObject lootMark;
        GameObject enemyUI;
        Rigidbody enemyRigidbody;
        Collider enemyCollider;
        bool givenExp = false; //remove after death is fixed

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
                if (givenExp == false)
                {
                    if (level - player.GetLevel() >= 10)
                    {
                        expReward = baseExpReward * 10;
                        givenExp = true;
                    }
                    else
                    {
                        if (level >= player.GetLevel())
                        {
                            expReward = baseExpReward * (level - player.GetLevel());
                            givenExp = true;
                        }
                        if (level <= player.GetLevel())
                        {
                            expReward = ((baseExpReward / player.GetLevel() - level));
                            givenExp = true;
                        }
                    }
                }
                else
                {
                    return;
                }
                if (isProp)
                {
                    return;
                }
                isDead = true;
                enemyUI.SetActive(false);
                enemyRigidbody.isKinematic = true;
                enemyCollider.enabled = false;
                player.GiveExp(expReward);
                if (lootable)
                {
                    CancelInvoke("SpawnProjectile");
                    //aiCharacterControl.SetTarget(transform);
                    animator.SetTrigger("Dead");
                    lootableParticle.Play();
                    gameObject.tag = "Lootable";
                    gameObject.layer = LayerMask.NameToLayer("Lootable");
                }
                else
                {
                    CancelInvoke("SpawnProjectile");
                    //aiCharacterControl.SetTarget(transform);
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

        public readonly static HashSet<Enemy> Pool = new HashSet<Enemy>();

        private void OnEnable()
        {
            Enemy.Pool.Add(this);
        }

        private void OnDisable()
        {
            Enemy.Pool.Remove(this);
        }

        public static Enemy FindClosestEnemy(Vector3 pos)
        {
            Enemy result = null;
            float dist = float.PositiveInfinity;
            var e = Enemy.Pool.GetEnumerator();
            while (e.MoveNext())
            {
                float d = (e.Current.transform.position - pos).sqrMagnitude;
                if (d < dist)
                {
                    result = e.Current;
                    dist = d;
                }
            }
            return result;
        }


        void Start()
        {
            player = FindObjectOfType<Character>();
            animator = GetComponent<Animator>();
            lootableParticle = GetComponentInChildren<ParticleSystem>();
            enemyUI = this.gameObject.transform.GetChild(0).gameObject;
            //aiCharacterControl = GetComponent<AICharacterControl>();
            currentHealthPoints = maxHealthPoints;
            DamageTextController.Initialize();
            lootMark = this.transform.Find("LootMark").gameObject;
            enemyRigidbody = GetComponent<Rigidbody>();
            enemyCollider = GetComponent<Collider>();
        }

        void Update()
        {
            Transform chasestop;
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            //if (distanceToPlayer <= attackRadius && !isAttacking && currentHealthPoints > 0)
            //{
            //    isAttacking = true;
            //    float randomizedAttackSpeed = Random.Range(attackSpeed - attackSpeedVariation, attackSpeed + attackSpeedVariation);
            //    //aiCharacterControl.SetTarget(transform);
            //    InvokeRepeating("SpawnProjectile", 0f, randomizedAttackSpeed); // TODO switch to Coroutine
            //}

            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }

            if (distanceToPlayer <= chaseRadius && distanceToPlayer >= attackRadius && currentHealthPoints > 0)
            {
                //aiCharacterControl.SetTarget(player.transform);
            }
            else
            {
                //aiCharacterControl.SetTarget(transform);
            }
            if (player.GetHealthAsPercentage() <= Mathf.Epsilon)
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
            if (Random.Range(0, 100) == 20)
            {
                lootable = true;
                lootMark.SetActive(true);
                print(this.name + ": Loot Me!");
            }
            else if (Random.Range(0, 50) == Random.Range(0, 50))
            {
                lootable = false;
                lootMark.SetActive(false);
            }
            Debug.Log("EnemyCount = " + Enemy.Pool.Count);
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer == 10)
            {
                Debug.Log(this.name + "  Entered Aggro Bubble!");
                isAttacking = true;
                float randomizedAttackSpeed = Random.Range(attackSpeed - attackSpeedVariation, attackSpeed + attackSpeedVariation);
                //aiCharacterControl.SetTarget(transform);
                InvokeRepeating("SpawnProjectile", 0f, randomizedAttackSpeed); // TODO switch to Coroutine
            }
        }

        void TriggerAttack()
        {
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

        public float GetLevel()
        {
            return level;
        }

        public float GetMaxHealth()
        {
            return maxHealthPoints;
        }

        public float GetCurrentHealth()
        {
            return currentHealthPoints;
        }

        public bool IsDead()
        {
            return isDead;
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