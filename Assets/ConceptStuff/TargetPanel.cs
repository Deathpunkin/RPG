using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

public class TargetPanel : MonoBehaviour {

    [SerializeField] Character player;
    [SerializeField] Text targetName;
    [SerializeField] Text targetLevel;
    [SerializeField] Text _player;
    [SerializeField] float warningLevelDifMin;
    [SerializeField] float warningLevelDifMax;
    [SerializeField] Color warning;
    [SerializeField] int dangerLevelDif;
    [SerializeField] Color danger;

	// Use this for initialization
	void Start () {
		if (player == null)
        {
           player = FindObjectOfType<Character>();
        }

	}
	
	// Update is called once per frame
	void Update () {
        if (player.GetTarget() != null)
        {
            if(player.GetTargetLevel() > player.level + dangerLevelDif)
            {
                targetName.color = danger;
            }
            if(player.GetTargetLevel() < player.level + warningLevelDifMax && player.GetTargetLevel() > player.level + warningLevelDifMin)
            {
                targetName.color = warning;
            }
            if(player.GetTargetLevel() <= player.level + 2)
            {
                targetName.color = Color.white;
            }
            targetName.text = player.GetTarget().gameObject.name;
            targetLevel.text = player.GetTargetLevel().ToString();
        }
        _player.text = player.gameObject.name.ToString();
	}
}
