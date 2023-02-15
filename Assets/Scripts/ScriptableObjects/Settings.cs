using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Objects/Globals/Settings")]
public class Settings : ScriptableSingleton<Settings>
{
    [Header("General")]
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
    [ConditionalHide("DoWaterMovement")]
    [Tooltip("Controlles of waterSources give of water")]
    public bool doWaterSources;
    [Tooltip("Controlles of weather is a thing")]
    public bool doWeather;
    [ConditionalHide("DoWeather")]
    [Tooltip("Controlles of weather automaticly changes")]
    public bool doWeatherCycle;
    [ConditionalHide("DoWaterMovement", ConditionalSourceField2 = "DoWeather")]
    [Tooltip("Controlles of weather effects water height")]
    public bool wheaterEffectsWater;

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
