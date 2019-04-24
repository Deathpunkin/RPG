using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

public class SkillBar : MonoBehaviour {

    Player _player;
    [SerializeField] Character player;
    [SerializeField] SkillSlot[] skillSlots;
    [SerializeField] Image[] skillIcons;

    // Use this for initialization
    void Start () {
        _player = FindObjectOfType<Player>();
        player = _player.GetComponent<Character>();        
	}

    private void OnValidate()
    {
        skillSlots = GetComponentsInChildren<SkillSlot>();
        skillIcons = new Image[skillSlots.Length];

    }

    // Update is called once per frame
    void Update () {
        for (int i = 0; i < skillIcons.Length; i++)
        {
            skillIcons[i] = player.GetAbilityIcon(i);
        }
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].SetIcon(skillIcons[i]);
        }
	}
}
