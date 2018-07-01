using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{   
    public class Player : MonoBehaviour, IDamageable
    {

        [SerializeField] int enemyLayer = 9;
        //Gear Slots Setup Here
        public Weapon mainHandWeapon;
        public Weapon offHandWeapon;
        public float level = 1f;

        public float maxHealthPoints = 100f;

        public float currentHealthPoints;
        CameraRaycaster cameraRaycaster;
        [SerializeField] float lastAttackTime = 0f;
        public float timeSinceLastDamaged; //TODO remove public after debugging done
        float regenHealthDelay = 5f;
        float regenHealthspeed = 1f;

        float damage;
        [SerializeField] float critChance = 10f;
        [SerializeField] float critDamage;
        [SerializeField] float critMultiplyer = 1.5f; // 1.5 extra dmg
        [SerializeField] float highestDamage;
        [SerializeField] float highestCrit; 

        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        void Start()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            currentHealthPoints = maxHealthPoints;

            PutWeaponInMainHand();
            if(!offHandWeapon)
            {
                return;
            }
            PutWeaponInOffHand();
            SetupRuntimeAnimator();
            DamageTextController.Initialize();
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;

            //TODO Get OFFHAND and Block working
            animatorOverrideController["DEFAULT MAINHAND ATTACK"] = mainHandWeapon.GetMainHandAttackAnimClip();
            //animatorOverrideController["DEFAULT OFFHAND ATTACK"] = offHandWeapon.GetOffHandAttackAnimClip();
            //animatorOverrideController["DEFAULT MAINHAND BLOCK"] = mainHandWeapon.GetMainHandBlockAnimClip();
            //animatorOverrideController["DEFAULT OFFHAND BLOCK"] = offHandWeapon.GetOffHandBlockAnimClip();

        }
        //Weapon/Hand setup
        //MainHand
        private void PutWeaponInMainHand()
        {
            var weaponMainHandPrefab = mainHandWeapon.GetWeaponPrefab();
            GameObject mainHand = RequestMainHand();
            var weaponMainHand = Instantiate(weaponMainHandPrefab, mainHand.transform);
            weaponMainHand.transform.localPosition = mainHandWeapon.gripMainHandTransform.localPosition;
            weaponMainHand.transform.localRotation = mainHandWeapon.gripMainHandTransform.localRotation;
        }
            //OffHand
        public void PutWeaponInOffHand()
        {
                var weaponOffHandPrefab = offHandWeapon.GetWeaponPrefab();
                GameObject offHand = RequestOffHand();
                var weaponOffHand = Instantiate(weaponOffHandPrefab, offHand.transform);
                weaponOffHand.transform.localPosition = offHandWeapon.gripOffHandTransform.localPosition;
                weaponOffHand.transform.localRotation = offHandWeapon.gripOffHandTransform.localRotation;
        }

        private GameObject RequestMainHand()
        {
            var mainHands = GetComponentsInChildren<MainHand>();
            int numberOfMainHands = mainHands.Length;
            Assert.AreNotEqual(numberOfMainHands, 0, "No main hand found on player, please add one");
            Assert.IsFalse(numberOfMainHands > 1, "Multiple MainHand scripts on player, please remove one");
            return mainHands[0].gameObject;
        }
        private GameObject RequestOffHand()
        {
            var OffHands = GetComponentsInChildren<OffHand>();
            int numberOfOffHands = OffHands.Length;
            Assert.AreNotEqual(numberOfOffHands, 0, "No main hand found on player, please add one");
            Assert.IsFalse(numberOfOffHands > 1, "Multiple MainHand scripts on player, please remove one");
            return OffHands[0].gameObject;
        }


        void Update()
        {
            if ((Time.time - timeSinceLastDamaged) >= regenHealthDelay && currentHealthPoints != maxHealthPoints)
            {
                print("regenerating!");
                StartCoroutine(regenHealth());
            }
            if (currentHealthPoints == maxHealthPoints & (Time.time - timeSinceLastDamaged) <= regenHealthDelay)
            {
                //CancelInvoke();
                StopCoroutine(regenHealth());
            }
            //Damage Math
            damage = Mathf.Round(UnityEngine.Random.Range(mainHandWeapon.GetMinDamagePerHit(), mainHandWeapon.GetMaxDamagePerHit()));
            critDamage = Mathf.Round(damage * critMultiplyer);
        }

        IEnumerator regenHealth()
        {
            currentHealthPoints = currentHealthPoints + regenHealthspeed;
            //        regenHealthSpeed = regenHealthSpeed +1 ;
            print("Still Regen!");
            yield return new WaitForSeconds(1);
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
            else
            {
                if(Input.GetMouseButtonDown(0))
                {
                    DamageTextController.CreateFloatingOutOfRangeText("Target out of reach.", enemy.transform);
                    //StartCoroutine(moveIntoRange());
                }
            }
        }

        IEnumerator moveIntoRange()
        {
            print("Make me Move to Target");
            yield return new WaitForSeconds(0);
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= mainHandWeapon.GetMaxAttackRange();
        }

        private void AttackTarget(Enemy enemy)
        {
            if (Time.time - lastAttackTime > mainHandWeapon.GetAttackSpeed())
            {
                transform.LookAt(enemy.transform);
                animator.SetTrigger("Attack");
                //mainHandWeapon.GetWeaponHitSound();
                //mainHandWeapon.GetWeaponAudioSouce().Play
                if (UnityEngine.Random.Range(1.0f, 100.0f) < critChance && UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
                {
                    enemy.TakeDamage(critDamage);
                    print("CRIT! Dealt " + critDamage);
                    if (critDamage >= highestCrit && enemy.level >= level)
                    {
                        highestCrit = critDamage;
                    }
                    DamageTextController.CreateFloatingCritDamageText(critDamage.ToString(), enemy.transform);

                }
                else if (UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
                    {
                        if (damage >= highestDamage && enemy.level >= level)
                        {
                            highestDamage = damage;
                            Debug.Log(enemy.transform);
                        }
                        DamageTextController.CreateFloatingDamageText(damage.ToString(), enemy.transform);
                        enemy.TakeDamage(damage);
                        print("Dealt " + damage);
                    print("Target position - " + enemy.transform.position.ToString());
                    }
                else
                {
                    print("DODGED!");
                    DamageTextController.CreateFloatingDodgeText("Dodged!", enemy.transform);

                }
                lastAttackTime = Time.time;
            }
        }
        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            timeSinceLastDamaged = Time.time;
        }

        //TODO Uncomment this after fixing exp system.
        public void OnGUI()
        {
            string currentHealth = currentHealthPoints.ToString();
            string maxHealth = maxHealthPoints.ToString();
            //    string lvl = level.ToString();
            //    string exp = experiencePoints.ToString();
            //    string expToLevel = experienceToNextLevel.ToString();
            //    string lastdamaged = lastDamaged.ToString();
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 55, 100, 20), currentHealth + "/" + maxHealth);
            //    GUI.Label(new Rect(Screen.width / 3, Screen.height - 30, 100, 20), lvl);
            //    GUI.Label(new Rect(Screen.width / 2, Screen.height - 30, 100, 20), exp + "/" + expToLevel);
            //    GUI.Label(new Rect(Screen.width - 80, Screen.height - 20, 100, 20), "Last hit " + (Time.time - timeSinceLastDamaged) + "s ago");

        }
    }
}