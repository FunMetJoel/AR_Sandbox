using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Objects/Globals/Settings")]
public class Settings : SingletonScriptableObject<Settings>
{
    [Header("General")]

    public int OPS;

    [Tooltip("If the land should change")]
    public bool doLandChanges = true;

    [Header("Display")]
    [Tooltip("Material of land")]
    public Material landMaterial;
    [Tooltip("Material of water")]
    public Material waterMaterial;

    [Header("Water")]
    [Tooltip("Controlles if water moves")]
    public bool doWaterMovement;
    [ConditionalHide("doWaterMovement")]
    [Tooltip("Controlles of waterSources give of water")]
    public bool doWaterSources;
    [Tooltip("Controlles of weather is a thing")]
    public bool doWeather;
    [ConditionalHide("doWeather")]
    [Tooltip("Controlles of weather automaticly changes")]
    public bool doWeatherCycle;
    [ConditionalHide("doWaterMovement", ConditionalSourceField2 = "doWeather")]
    [Tooltip("Controlles of weather effects water height")]
    public bool wheaterEffectsWater;

    public bool doIceForming;
    public bool doGroundHumidity;
    public bool doGroundHumidityMovement;
    public bool doAirHumidity;


    [Header("UpdateSettings")]
    [InspectorButton("SettingChange")]
    public bool SettingsChangedUpdate;
    public UnityEvent SettingsChanged;
    public void SettingChange()
    {
        SettingsChanged.Invoke();
        Debug.Log("Updated Steeings");
    }
}
