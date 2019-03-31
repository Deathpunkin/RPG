using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemSlotText;
    [SerializeField] Text ItemStatsText;

    private StringBuilder sb = new StringBuilder();

    public void ShowToolTip(EquippableItem item)
    {
        ItemNameText.text = item.ItemName;
        ItemSlotText.text = item.EquipmentType.ToString();

        sb.Length = 0;
        AddStat(item.StrengthBonus, "Strength");
        AddStat(item.AgilityBonus, "Agility");
        AddStat(item.IntelligenceBonus, "Intelligence");
        AddStat(item.VitalityBonus, "Vitality");

        AddStat(item.StrengthPercentBonus / 100, "Strength", isPercent: true);
        AddStat(item.AgilityPercentBonus / 100, "Agility", isPercent: true);
        AddStat(item.IntelligencePercentBonus / 100, "Intelligence", isPercent: true);
        AddStat(item.VitalityPercentBonus / 100, "Vitality", isPercent: true);

        ItemStatsText.text = sb.ToString();

        gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        gameObject.transform.position = Input.mousePosition;
    }
    private void Update()
    {
        if(isActiveAndEnabled)
        {
            gameObject.transform.SetAsLastSibling();
        }
    }
    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    private void AddStat(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if (value > 0)
            {
                sb.Append("+");
            }
            if (isPercent)
            {
                sb.Append(value * 100);
                sb.Append("% ");
            }
            else
            {
                sb.Append(value);
                sb.Append(" ");
            }
            sb.Append(statName);
        }
    }
}
