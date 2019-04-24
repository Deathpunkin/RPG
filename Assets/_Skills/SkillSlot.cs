using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

public class SkillSlot : MonoBehaviour
{

    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0);
    [SerializeField] Image image;

    protected virtual void OnValidate()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    public void SetIcon(Image _icon)
    {
        image.sprite = _icon.sprite;
        image.color = normalColor;
    }
       
}

    
//    {
//        get { return _item; }
//        set
//        {
//            _item = value;
//            if (_item == null)
//            {
//                image.color = disabledColor;
//            }
//            else
//            {
//                image.sprite = _item.Icon;
//                image.color = normalColor;
//            }
//        }
//    }
//}
