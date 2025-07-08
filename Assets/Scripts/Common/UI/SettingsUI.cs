using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : UIBase
{
    [SerializeField] private TMP_Dropdown ResDropDown;
    [SerializeField] private Toggle FullScreenToggle;

    private Resolution[] AllResolutions;
    private List<Resolution> SelectedResolutionList = new List<Resolution>();

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

    public override void SetInfo(UIBaseData uiData)
    {
        base.SetInfo(uiData);

        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + "x" + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
        }

        ResDropDown.AddOptions(resolutionStringList);

        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            var selectedResolution = userSettingsData.ResolutionIndex;
            ResDropDown.value = selectedResolution;
            FullScreenToggle.isOn = userSettingsData.FullScreen;
            Screen.SetResolution(SelectedResolutionList[selectedResolution].width,
                                SelectedResolutionList[selectedResolution].height,
                                userSettingsData.FullScreen);
            MusicSlider.value = userSettingsData.Music_Volume;
            SFXSlider.value = userSettingsData.SFX_Volume;
        }
    }

    public void SetResolution()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            userSettingsData.ResolutionIndex = ResDropDown.value;
            userSettingsData.SaveData();
            var selectedResolution = userSettingsData.ResolutionIndex;
            Screen.SetResolution(SelectedResolutionList[selectedResolution].width,
                                SelectedResolutionList[selectedResolution].height,
                                userSettingsData.FullScreen);
        }
    }

    public void ChangeFullScreen()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            userSettingsData.FullScreen = FullScreenToggle.isOn;
            userSettingsData.SaveData();
            Screen.fullScreen = userSettingsData.FullScreen;
        }
    }

    public void SetMusicVolume()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            userSettingsData.Music_Volume = MusicSlider.value;
            userSettingsData.SaveData();
            //AudioManager.Instance.SetVolume(userSettingsData);
        }
    }

    public void SetSFXVolume()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            userSettingsData.SFX_Volume = SFXSlider.value;
            userSettingsData.SaveData();
            //AudioManager.Instance.SetVolume(userSettingsData);
        }
    }
}
