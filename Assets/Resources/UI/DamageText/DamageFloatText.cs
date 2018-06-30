using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFloatText : MonoBehaviour {
    public Animator animator;
    private Text damageText;

   void OnEnable()
    {
        Debug.Log("FloatTextStart");
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        damageText = animator.GetComponent<Text>();
    }

    public void SetText(string text)
    {
        animator.GetComponent<Text>().text = text;
    }
}
