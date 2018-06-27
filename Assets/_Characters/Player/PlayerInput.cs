using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters //consider changing to core
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerInput : MonoBehaviour
    {

        Player player;
        ThirdPersonCharacter thirdPersonCharacter = null;
        [SerializeField] float damageCaused = 10;


        //KeyBinds
        public KeyCode Crouch = KeyCode.Z;
        public KeyCode SelfDamage = KeyCode.H;
        public KeyCode Exp = KeyCode.K;

        // Use this for initialization
        void Start()
        {
            player = FindObjectOfType<Player>();
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        }
        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }


        // Update is called once per frame
        void Update()
        {
            ////crouching
            //if (Input.GetKey(Crouch))
            //{
            //    thirdPersonCharacter.m_Crouching = true;
            //}
            //else
            //{
            //    thirdPersonCharacter.m_Crouching = false;
            //}

            //if (thirdPersonCharacter.m_Crouching)
            //{
            //    print("Crouching!");
            //}

            if (Input.GetKeyDown(SelfDamage))
            {
                Component damageableComponent = player;


                (damageableComponent as IDamageable).TakeDamage(damageCaused);
                player.timeSinceLastDamaged = Time.time;
                print("Dealt 10dmg to Self!");
                if (player.currentHealthPoints <= 0)
                {
                    Destroy(gameObject);
                }
            }
            //TODO uncomment after fix EXP
            //if (Input.GetKeyDown(Exp))
            //{
            //   player.experiencePoints += 12;
            //}
        }
    }
}