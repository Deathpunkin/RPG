using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters //TODO consider making Core
{   

    public class Energy : MonoBehaviour
    {
        Player player;
        [SerializeField] Image energyOrb = null;
        public float maxEnergyPoints = 100f;
        [SerializeField] float energyCost = 10;
        [SerializeField] float regenPerSecond = 1f;
        public float currentEnergyPoints;
        float energyTextLocation = 315;

        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyDisplay();
        }

        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergy();
                UpdateEnergyDisplay();
                if (maxEnergyPoints >= 100 && maxEnergyPoints < 1000)
                {
                    energyTextLocation = 315;
                }
                else if(maxEnergyPoints >= 1000 && maxEnergyPoints < 10000)
                {
                    energyTextLocation = 315 + 5;
                }
            }
        }

        private void AddEnergy()
        {
            var pointsToAdd = regenPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyDisplay();
        }

        private void UpdateEnergyDisplay()
        {
            energyOrb.fillAmount = energyAsPercent();
        }

        float energyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}