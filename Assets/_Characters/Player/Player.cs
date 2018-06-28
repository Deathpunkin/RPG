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


        public float maxHealthPoints = 100f;

        public float currentHealthPoints;
        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;
        public float timeSinceLastDamaged;
        float regenHealthDelay = 5f;
        float regenHealthspeed = 1f;
        float lastDamaged;

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
            cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
            currentHealthPoints = maxHealthPoints;
            PutWeaponInMainHand();
            PutWeaponInOffHand();
            SetupRuntimeAnimator();
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;

            //TODO Get OFFHAND and Block working
            animatorOverrideController["DEFAULT MAINHAND ATTACK"] = mainHandWeapon.GetMainHandAttackAnimClip();
            //animatorOverrideController["DEFAULT OFFHAND ATTACK"] = offHandWeapon.GetOffHandAttackAnimClip();
            //animatorOverrideController["DEFAULT MAINHAND BLOCK"] = mainHandWeapon.GetMainHandBlockAnimClip();
            animatorOverrideController["DEFAULT OFFHAND BLOCK"] = offHandWeapon.GetOffHandBlockAnimClip();

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
                StartCoroutine("regenHealth");
            }
            if (currentHealthPoints == maxHealthPoints & (Time.time - timeSinceLastDamaged) <= regenHealthDelay)
            {
                //CancelInvoke();
                StopCoroutine("regenHealth");
                print("Done Regen!");
            }
        }

        IEnumerator regenHealth()
        {
            currentHealthPoints = currentHealthPoints + regenHealthspeed;
            //        regenHealthSpeed = regenHealthSpeed +1 ;
            print("Still Regen!");
            yield return new WaitForSeconds(1);
        }


        void OnMouseClick(RaycastHit raycastHit, int layerHit)
        {
            
            if (layerHit == enemyLayer)
            {
                var enemy = raycastHit.collider.gameObject;

                // Check enemy is in range
                if(IsTargetInRange(enemy))
                {
                    AttackTarget(enemy);
                }
            }
        }


        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTaret = (target.transform.position - transform.position).magnitude;
            return distanceToTaret <= mainHandWeapon.GetMaxAttackRange();
        }

        private void AttackTarget(GameObject target)
        {
            var enemyComponent = target.GetComponent<Enemy>();
            if (Time.time - lastHitTime > mainHandWeapon.GetAttackSpeed())
            {
                transform.LookAt(target.transform);
                animator.SetTrigger("Attack");
                //mainHandWeapon.GetWeaponHitSound();
                //mainHandWeapon.GetWeaponAudioSouce().Play();
                enemyComponent.TakeDamage(mainHandWeapon.GetDamagePerHit());
                lastHitTime = Time.time;
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