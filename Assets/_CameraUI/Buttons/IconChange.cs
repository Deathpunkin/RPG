using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.CameraUI
{
    public class IconChange : MonoBehaviour
    {
        //TODO Change skill icon when skill in slot changes
        Image skill1;
        //Player player;
        SpecialAbility[] abilities;
        Sprite abilityIcon;
        // Use this for initialization
        void Start()
        {
            skill1 = GetComponent<Image>();
            abilities = FindObjectOfType<Player>().GetComponent<SpecialAbility[]>();
            //abilityarray = player.GetComponent<SpecialAbility[]>();
            abilities[0].GetSkillIcon();
        }

        // Update is called once per frame
        void Update()
        {
            skill1.sprite = abilityIcon;
            Debug.Log(abilityIcon != null);
        }
    }
}