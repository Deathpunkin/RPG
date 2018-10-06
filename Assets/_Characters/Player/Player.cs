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
        [SerializeField] AbilityConfig[] abilities;

        //Gear Slots Setup Here
        public Weapon mainHandWeapon;
        public Weapon offHandWeapon;

        //Stat Setup
        public float level = 1f;
        public float experiencePoints;
        public float experienceToNextLevel;

        public float maxHealthPoints = 100f;
        public float currentHealthPoints;
        float prevHealth;
        public float respawnHealth;
        public float regenHealthDelay = 5.5f;
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
        Enemy enemy = null;


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
            prevHealth = currentHealthPoints;
            regenHealthSpeed = baseRegenHealthSpeed;
            audioSource = GetComponent<AudioSource>();
            PutWeaponInMainHand();
            PutWeaponInOffHand();
            SetupRuntimeAnimator();
            DamageTextController.Initialize();
            AttachInitialAbilities();
            Button respawn = respawnButton.GetComponent<Button>();
            respawnHealth = Mathf.Round(currentHealthPoints / 3);
        }

        void Update()
        {
            if (healthAsPercentage > Mathf.Epsilon)
            {
                if ((Time.time - timeSinceLastDamaged) >= regenHealthDelay && currentHealthPoints != maxHealthPoints && !isDead)
                {
                    print("regenerating!");
                    StartCoroutine(regenHealth());
                }
                else if (currentHealthPoints == maxHealthPoints && (Time.time - timeSinceLastDamaged) <= regenHealthDelay)
                {
                    //CancelInvoke();
                    StopCoroutine(regenHealth());
                    print("Done Regen!");
                    regenHealthSpeed = baseRegenHealthSpeed;
                }
                //Damage Math
                damage = Mathf.Round(UnityEngine.Random.Range(mainHandWeapon.GetMinDamagePerHit(), mainHandWeapon.GetMaxDamagePerHit()));
                critDamage = Mathf.Round(damage * critMultiplyer);
                if (isDead)
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
                //TODO get respawn invuln working
                //respawnInvulnTimer = Mathf.Clamp(respawnInvuln - Time.time, 0, respawnInvuln);
               ScanForAbilityKeyDown();
            }
        }
        //TODO uncomment when targeting for skills works
        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 0; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    UseAbility(keyIndex);
                }
            }
        }
        private void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachComponent(gameObject);
            }
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
            Assert.AreNotEqual(numberOfOffHands, 0, "No off hand found on player, please add one");
            Assert.IsFalse(numberOfOffHands > 1, "Multiple Off Hand scripts on player, please remove one");
            return OffHands[0].gameObject;
        }




    IEnumerator regenHealth()
        {
            currentHealthPoints = currentHealthPoints + regenHealthSpeed;
            //regenHealthSpeed = regenHealthSpeed + 0.5f;
            print("Still Regen!");
            yield return new WaitForSeconds(1);
        }

        void OnMouseOverEnemy(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                transform.LookAt(enemy.transform);
                AttackTarget();
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
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

        private void UseAbility(int abilityIndex)
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

        private void AttackTarget()
        {
            if (Time.time - lastAttackTime > mainHandWeapon.GetAttackSpeed())
            {
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
        public void TakeDamage(float damage) //TODO change back to damage and duplicate for healing
        {
            if(respawnInvulnTimer != 0)
            {
                damage = 0;
            }
            ReduceHealth(damage);
            //Trigger Death
            if (currentHealthPoints <= 0)
            {
                StartCoroutine(TriggerDeath());
            }
        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
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
            if(currentHealthPoints < prevHealth)
            {
                timeSinceLastDamaged = Time.time;
                print("Damaged!");
                audioSource.clip = hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)];
                audioSource.Play();
            }
            prevHealth = currentHealthPoints;
        }

        //TODO Uncomment this after fixing exp system.
        //public void OnGUI()
        //{
        //    string currentHealth = currentHealthPoints.ToString("F0");
        //    string maxHealth = maxHealthPoints.ToString();
        //    //    string lvl = level.ToString();
        //    //    string exp = experiencePoints.ToString();
        //    //    string expToLevel = experienceToNextLevel.ToString();
        //    //    string lastdamaged = lastDamaged.ToString();
        //    GUI.Label(new Rect(Screen.width - 545, Screen.height - 77, 100, 20), currentHealth + "/" + maxHealth);
        //    //    GUI.Label(new Rect(Screen.width / 3, Screen.height - 30, 100, 20), lvl);
        //    //    GUI.Label(new Rect(Screen.width / 2, Screen.height - 30, 100, 20), exp + "/" + expToLevel);
        //    //    GUI.Label(new Rect(Screen.width - 80, Screen.height - 20, 100, 20), "Last hit " + (Time.time - timeSinceLastDamaged) + "s ago");

        //}
    }
}