using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters //TODO consider making Core
{   

    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float energyCost = 10;
        [SerializeField] float energyRegenSpeed = 0.5f;
        CameraRaycaster cameraRaycaster;
        
        public float currentEnergyPoints;
        // Use this for initialization
        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyNumBar1Observers += ProcessNumBar1;
        }

        void ProcessNumBar1(RaycastHit raycastHit, int layerHit)
        {
            print("Numbar 1!");
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints - energyCost, 0, maxEnergyPoints);
        }

        // Update is called once per frame
        void Update()
        {
            float xValue = -(energyAsPercent() / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        IEnumerator energyRegen()
        {
            currentEnergyPoints = currentEnergyPoints + energyRegenSpeed;
            yield return new WaitForSeconds(1);
        }

        float energyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }

        public void OnGUI()
        {
            string currentEnergy = currentEnergyPoints.ToString();
            string maxEnergy = maxEnergyPoints.ToString();
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 90, 100, 20), currentEnergy + "/" + maxEnergy);

        }
    }
}