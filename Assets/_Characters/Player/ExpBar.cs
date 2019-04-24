using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(Image))]
    public class ExpBar : MonoBehaviour
    {
        [SerializeField] Image expBarFill;
        [SerializeField] Text expText;
        Character player;

        // Use this for initialization
        void Start()
        {
            player = FindObjectOfType<Character>();
        }

        // Update is called once per frame
        void Update()
        {
            expBarFill.fillAmount = player.GetExpAsDecimal();
            expText.text = player.GetExp() + " / " + player.GetExpToLevel() + "|" + player.GetExpAsPercentage() + "%";
        }
    }
}
