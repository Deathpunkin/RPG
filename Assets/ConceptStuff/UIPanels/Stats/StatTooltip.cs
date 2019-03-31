using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;
using System;

public class StatTooltip : MonoBehaviour
{
    [SerializeField] Text StatNameText;
    [SerializeField] Text StatModifiersLabelText;
    [SerializeField] Text StatModifiersText;

    private StringBuilder sb = new StringBuilder();

    public void ShowToolTip(CharacterStat stat, string statname)
    {
        StatNameText.text = GetStatTipText(stat, statname);
        StatModifiersText.text = GetStatModifiersText(stat);

        gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        gameObject.transform.position = Input.mousePosition;
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    private string GetStatTipText(CharacterStat stat, string statname)
    {
        sb.Length = 0;
        sb.Append(statname);
        sb.Append(" ");
        sb.Append(stat.Value);

        if (stat.Value != stat.BaseValue)
        {
            sb.Append(" (");
            sb.Append(stat.BaseValue);

            if (stat.Value > stat.BaseValue)
            {
                sb.Append("+");
            }
            sb.Append(System.Math.Round(stat.Value - stat.BaseValue, 4));
            sb.Append(")");
        }

        return sb.ToString();
    }

    private string GetStatModifiersText(CharacterStat stat)
    {
        sb.Length = 0;

        foreach (StatModifier mod in stat.StatModifiers)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if(mod.Value > 0)
            {
                sb.Append("+");
            }

            if (mod.Type == StatModType.Flat)
            {
                sb.Append(mod.Value);
            }
            else
            {
                sb.Append(mod.Value * 100);
                sb.Append("%");
            }
            EquippableItem item = mod.Source as EquippableItem;

            if (item != null)
            {
                sb.Append(" ");
                sb.Append(item.ItemName);
            }
            else
            {
                Debug.LogError("Modifier is not an EquippableItem!");
            }
        }

        return sb.ToString();
    }
}