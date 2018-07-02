using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters //TODO consider making Core
{   

    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float energyCost = 10;
        [SerializeField] float regenPerSecond = 1f;
        public float currentEnergyPoints;

        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
        }

        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergy();
                UpdateEnergyBar();
            }
            //if (currentEnergyPoints < maxEnergyPoints)
            //{
            //    StartCoroutine(energyRegen());
            //}
            //if (currentEnergyPoints == maxEnergyPoints)
            //{
            //    StopCoroutine(energyRegen());
            //}
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
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            float xValue = -(energyAsPercent() / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        //TODO fix bar not updating proper with regen
        //IEnumerator energyRegen()
        //{
        //    print("regen energy!");
        //    currentEnergyPoints = currentEnergyPoints + energyRegenSpeed;
        //    yield return new WaitForSeconds(1);
        //}

        float energyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }

        public void OnGUI()
        {
            //TODO fix strange energy numbers showing up on bar
            string currentEnergy = currentEnergyPoints.ToString("F0");
            string maxEnergy = maxEnergyPoints.ToString("F0");
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 90, 100, 20), currentEnergy + "/" + maxEnergy);

        }
    }
}