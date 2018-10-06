//2016 Spyblood Games

using UnityEngine;
using System.Collections;

[System.Serializable]
public class DayColors
{
    public Color skyColor;
    public Color equatorColor;
    public Color horizonColor;
}

public class DayAndNightControl : MonoBehaviour
{
    public bool StartDay; //start game as day time
    public GameObject StarDome;
    public GameObject moonState;
    public GameObject moon;
    public DayColors dawnColors;
    public DayColors dayColors;
    public DayColors nightColors;
    public int currentDay = 0; //day 8287... still stuck in this grass prison... no esacape... no freedom...
    public Light directionalLight; //the directional light in the scene we're going to work with
    public float SecondsInAFullDay = 120f; //in realtime, this is about two minutes by default. (every 1 minute/60 seconds is day in game)
    public float currentTime = 0; //at default when you press play, it will be nightTime. (0 = night, 1 = day)
    [Range(0, 24)]
    public float hour = 0;
    public float minute = 0;
    public double second = 0;
    public bool aM;
    public bool pM;
    public string aMOrPM = null;
    public System.DateTime startTime;
    public System.DateTime lastTime;
    public System.TimeSpan elapsedTime;
    [HideInInspector]
    public float timeMultiplier = 1f; //how fast the day goes by regardless of the secondsInAFullDay var. lower values will make the days go by longer, while higher values make it go faster. This may be useful if you're siumulating seasons where daylight and night times are altered.
    public bool showUI;
    float lightIntensity; //static variable to see what the current light's insensity is in the inspector
    Material starMat;

    Camera targetCam;

    // Use this for initialization
    void Start()
    {
        foreach (Camera c in GameObject.FindObjectsOfType<Camera>())
        {
            if (c.isActiveAndEnabled)
            {
                targetCam = c;
            }
        }
        lightIntensity = directionalLight.intensity; //what's the current intensity of the light
        starMat = StarDome.GetComponentInChildren<MeshRenderer>().material;
        if (StartDay)
        {
            hour = 7f; //start at Morning
            starMat.color = new Color(1f, 1f, 1f, 0f);
            aMOrPM = "AM";
        }
        startTime = System.DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLight();
        currentTime += ((Time.deltaTime / SecondsInAFullDay) * timeMultiplier) / 0.60f;
        if (hour >= 24)
        {
            hour = 0;//once we hit "midnight"; any time after that sunrise will begin.
            currentDay++; //make the day counter go up
        }
        lastTime = System.DateTime.Now;
        elapsedTime = lastTime - startTime;
        second = System.Math.Truncate(elapsedTime.TotalSeconds);
        if (second == 60)
        {
            minute++;
            startTime = System.DateTime.Now;
        }
        if (minute == 60)
        {
            hour++;
            minute = 0;
        }
        if (hour == 12 && minute == 0 && aM)
        {
            aM = false;
            pM = true;
            aMOrPM = "PM";
        }
        else if (hour == 0 && minute == 0 && pM)
        {
            pM = false;
            aM = true;
            aMOrPM = "AM";
        }
    }

    void UpdateLight()
    {
        StarDome.transform.Rotate(new Vector3(0, 0.25f * Time.deltaTime, 0));
        moon.transform.LookAt(targetCam.transform);
        directionalLight.transform.localRotation = Quaternion.Euler((hour * 15f) - 90, 170, 0);
        moonState.transform.localRotation = Quaternion.Euler((hour * 15f) - 100, 170, 0);
        //^^ we rotate the sun 360 degrees around the x axis, or one full rotation times the current time variable. we subtract 90 from this to make it go up
        //in increments of 0.25.

        //the 170 is where the sun will sit on the horizon line. if it were at 180, or completely flat, it would be hard to see. Tweak this value to what you find comfortable.

        float intensityMultiplier = 1;
        //TODO sun dissapears at 8AM?
        if (hour <= 6f || hour >= 19f)
        {
            intensityMultiplier = 0.5f; //when the sun is below the horizon, or setting, the intensity needs to be 0 or else it'll look weird
            starMat.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, Time.deltaTime));
        }
        else if (hour <= 18f)
        {
            intensityMultiplier = Mathf.Clamp01((hour - 0.23f) * (1 / 0.02f));
            starMat.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, Time.deltaTime));
        }
        else if (hour <= 19f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((hour - 0.73f) * (1 / 0.02f)));
        }


        //change env colors to add mood

        if (hour <= 5f)
        {
            RenderSettings.ambientSkyColor = nightColors.skyColor;
            RenderSettings.ambientEquatorColor = nightColors.equatorColor;
            RenderSettings.ambientGroundColor = nightColors.horizonColor;
        }
        if (hour > 5f && hour < 8f)
        {
            RenderSettings.ambientSkyColor = dawnColors.skyColor;
            RenderSettings.ambientEquatorColor = dawnColors.equatorColor;
            RenderSettings.ambientGroundColor = dawnColors.horizonColor;
        }
        if (hour > 8f && hour < 17f)
        {
            RenderSettings.ambientSkyColor = dayColors.skyColor;
            RenderSettings.ambientEquatorColor = dayColors.equatorColor;
            RenderSettings.ambientGroundColor = dayColors.horizonColor;
        }
        if (hour > 17f)
        {
            RenderSettings.ambientSkyColor = dayColors.skyColor;
            RenderSettings.ambientEquatorColor = dayColors.equatorColor;
            RenderSettings.ambientGroundColor = dayColors.horizonColor;
        }

        directionalLight.intensity = lightIntensity * intensityMultiplier;
    }

    public string TimeOfDay()
    {
        string dayState = "";
        if (hour > 0f && hour < 1f)
        {
            dayState = "Midnight";
        }
        if (hour > 1f && hour < 4f)
        {
            dayState = "Small Hours";
        }
        if (hour < 4f && hour > 6f)
        {
            dayState = "Dawn";
        }
        if (hour > 6f && hour < 7f)
        {
            dayState = "Sunrise";
        }
        if (hour > 7f && hour < 12f)
        {
            dayState = "Morning";
        }
        if (hour > 12f && hour < 13f)
        {
            dayState = "Noon";
        }
        if (hour > 13f && hour < 17f)
        {
            dayState = "Afternoon";
        }
        if (hour > 17f && hour < 18f)
        {
            dayState = "Sunset";
        }
        if (hour > 18f && hour < 20f)
        {
            dayState = "Dusk";
        }
        if (hour > 20f && hour < 24f)
        {
            dayState = "Night";
        }
        return dayState;
    }

    void OnGUI()
    {
        //debug GUI on screen visuals
        if (showUI)
        {
            GUILayout.Box("Day: " + currentDay);
            GUILayout.Box(TimeOfDay());
            GUILayout.Box(string.Format("{00:00}", hour) + ":" + string.Format("{00:00}", minute) + ":" + string.Format("{00:00}", second) + aMOrPM);
            GUILayout.VerticalSlider(hour, 0f, 24f);
        }
    }
}
