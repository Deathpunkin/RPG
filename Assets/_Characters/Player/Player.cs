using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{   
    public class Player : MonoBehaviour, IDamageable
    { 
        //TODO sort and clean all this
        CameraRaycaster cameraRaycaster;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;
        AudioSource audioSource;
        [SerializeField] AudioClip[] hurtSounds;
        [SerializeField] AudioClip[] deathSounds;

        // Temporarily serialized for debugging
        [SerializeField] SpecialAbility[] abilities;

        //Gear Slots Setup Here
        public Weapon mainHandWeapon;
        public Weapon offHandWeapon;

        //Stat Setup
        public float level = 1f;
        public float experiencePoints;
        public float experienceToNextLevel;

        public float maxHealthPoints = 100f;
        public float currentHealthPoints;
        public float respawnHealth;
        float regenHealthDelay = 5.5f;
        float baseRegenHealthSpeed = 0.5f;
        public bool isDead = false;
        public Button respawnButton;
        float respawnInvuln = 5f;
        public float respawnInvulnTimer;

        [SerializeField] float regenHealthSpeed;
        [SerializeField] float critChance = 10f;
        [SerializeField] float critDamage;
        [SerializeField] float critMultiplyer = 1.5f; // 150% extra dmg

        [SerializeField] float damage; //damage to deal
        [SerializeField] float lastAttackTime = 0f;
        public float timeSinceLastDamaged; //TODO remove public after debugging 
        [SerializeField] float highestDamage;
        [SerializeField] float highestCrit; 


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
            regenHealthSpeed = baseRegenHealthSpeed;
            audioSource = GetComponent<AudioSource>();
            PutWeaponInMainHand();
            PutWeaponInOffHand();
            SetupRuntimeAnimator();
            DamageTextController.Initialize();
            abilities[0].AttachComponent(gameObject);
            Button respawn = respawnButton.GetComponent<Button>();
            respawnHealth = Mathf.Round(currentHealthPoints / 3);
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
            animatorOverrideController["DEFAULT DEATH"] = mainHandWeapon.GetDeathAnimClip();
            animatorOverrideController["DEFAULT REVIVE"] = mainHandWeapon.GetReviveAnimClip();

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
            if ((Time.time - timeSinceLastDamaged) >= regenHealthDelay && currentHealthPoints != maxHealthPoints && !isDead)
            {
                print("regenerating!");
                StartCoroutine(regenHealth());
            }
            if (currentHealthPoints == maxHealthPoints && (Time.time - timeSinceLastDamaged) <= regenHealthDelay)
            {
                //CancelInvoke();
                StopCoroutine(regenHealth());
                regenHealthSpeed = baseRegenHealthSpeed;
            }
            //Damage Math
            damage = Mathf.Round(UnityEngine.Random.Range(mainHandWeapon.GetMinDamagePerHit(), mainHandWeapon.GetMaxDamagePerHit()));
            critDamage = Mathf.Round(damage * critMultiplyer);
            if(isDead)
            {
                animator.SetTrigger("Dead");
                StopCoroutine(regenHealth());
                this.GetComponent<ThirdPersonUserControl>().enabled = false;
            }
            else
            {
                animator.ResetTrigger("Dead");
                this.GetComponent<ThirdPersonUserControl>().enabled = true;
            }
           respawnInvulnTimer = Mathf.Clamp(respawnInvuln - Time.time, 0, respawnInvuln);
    }

    IEnumerator regenHealth()
        {
            currentHealthPoints = currentHealthPoints + regenHealthSpeed;
            //regenHealthSpeed = regenHealthSpeed + 0.5f;
            print("Still Regen!");
            yield return new WaitForSeconds(1);
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UseAbility(0, enemy);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UseAbility(1, enemy);
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

        private void UseAbility(int abilityIndex, Enemy enemy)
        {
            var energyComponent = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if(energyComponent.IsEnergyAvailable(energyCost))
            {
                energyComponent.ConsumeEnergy(energyCost);
                if(UnityEngine.Random.Range(1.0f, 100.0f) < critChance && UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
                {
                    var abilityParams = new AbilityUseParams(enemy, critDamage);
                    abilities[abilityIndex].Use(abilityParams);
                }
                else if (UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
                {
                    var abilityParams = new AbilityUseParams(enemy, damage);
                    abilities[abilityIndex].Use(abilityParams);
                }
            }
            else
            {
                DamageTextController.CreateFloatingNotEnoughEnergyText("Not Enough Energy.", transform);
            }

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
                        DamageTextController.CreateFloatingHighestCritDamageText(critDamage.ToString(), enemy.transform);

                    }
                    if(critDamage <= highestCrit && enemy.level <= level)
                    {
                       DamageTextController.CreateFloatingCritDamageText(critDamage.ToString(), enemy.transform);
                    }

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
            if(respawnInvulnTimer != 0)
            {
                damage = 0;
            }
            bool playerDies = (currentHealthPoints - damage <= 0); //Must ask before dealing damage
            ReduceHealth(damage);
            timeSinceLastDamaged = Time.time;
            //Trigger Death
            if (playerDies)
            {
                StartCoroutine(TriggerDeath());
            }
        }

        public IEnumerator TriggerDeath()
        {
            var energyComponent = GetComponent<Energy>();
            //Kill
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            isDead = true;
            //Drain Energy
            respawnButton.onClick.AddListener(Respawn);
            //TODO get button to Respawn, or wait for revive working
            yield return new WaitForSecondsRealtime(audioSource.clip.length);

        }

        public void Respawn()
        {
            isDead = false;
            currentHealthPoints = respawnHealth;
            timeSinceLastDamaged = Time.time;
            respawnInvuln =+ 5f;
        }
        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            audioSource.clip = hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)];
            audioSource.Play();
        }

        //TODO Uncomment this after fixing exp system.
        public void OnGUI()
        {
            string currentHealth = currentHealthPoints.ToString("F0");
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