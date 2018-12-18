using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters //consider changing to core
{
    public class PlayerInput : MonoBehaviour
    {
        Player player;
        CharacterMovement character;
        [SerializeField] float damageCaused = 10;
        Canvas HUD;

        [SerializeField] GameObject inventoryGameObject;
        [SerializeField] GameObject characterPanelGameObject;


        //KeyBinds
        public KeyCode Crouch = KeyCode.Z;
        public KeyCode SelfDamage = KeyCode.L;
        public KeyCode Exp = KeyCode.K;
        public KeyCode SkillBar1 = KeyCode.Alpha1;
        public KeyCode Revive = KeyCode.J;
        public KeyCode CommandButton = KeyCode.LeftControl;
        public KeyCode hideHud = KeyCode.H;
        public KeyCode Inventory = KeyCode.I;
        public KeyCode CharacterPanel = KeyCode.C;

        public bool HUDHidden;

        // Use this for initialization
        void Start()
        {
            player = FindObjectOfType<Player>();
            HUD = FindObjectOfType<Canvas>();
            
        }
        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(Inventory))
            {
                toggleInventory();
            }

            if (Input.GetKeyDown(CharacterPanel))
            {
                toggleCharacterPanel();
            }


            if (Input.GetKey(CommandButton))
            {
                print("ctrl pressed");
                if (Input.GetKeyDown(hideHud))
                {
                    print("HideHud?");
                    HideHud();
                }
            }

            ////crouching
            if (Input.GetKey(Crouch))
            {
                character.crouching = true;
            }
            else
            {
                character.crouching = false;
            }

            if (character.crouching)
            {
                print("Crouching!");
            }

            if (Input.GetKeyDown(SelfDamage))
            {
                Component damageableComponent = player;


                (damageableComponent as IDamageable).TakeDamage(damageCaused);
                player.timeSinceLastDamaged = Time.time;
                print("Dealt 10dmg to Self!");
                if (player.currentHealthPoints <= 0)
                {
                    StartCoroutine(player.TriggerDeath());
                }
            }
            if(Input.GetKeyDown(Revive))
            {
                player.Respawn();
            }
            //TODO uncomment after fix EXP
            //if (Input.GetKeyDown(Exp))
            //{
            //   player.experiencePoints += 12;
            //}
        }

        private void HideHud()
        {
            if (!HUDHidden)
            {
                HUDHidden = true;
                HUD.enabled = !enabled;
            }
            else if(HUDHidden)
            {
                HUDHidden = false;
                HUD.enabled = enabled;
            }
        }

        public void toggleInventory()
        {
            inventoryGameObject.SetActive(!inventoryGameObject.activeSelf);
        }

        public void toggleCharacterPanel()
        {
            characterPanelGameObject.SetActive(!characterPanelGameObject.activeSelf);
        }
    }
}