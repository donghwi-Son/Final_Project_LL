using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsUI : UIBase
{
    [SerializeField] private GameObject[] Tabs;
    [SerializeField] private RectTransform[] TabButtons;
    [SerializeField] private float InactiveButtonPosX = -275f;
    [SerializeField] private float ActiveButtonPosX = -300f;

    [SerializeField] private TMP_Dropdown LanguageDropDown;

    [SerializeField] private TMP_Dropdown ResDropDown;
    [SerializeField] private Toggle FullScreenToggle;

    private Resolution[] AllResolutions;
    private List<Resolution> SelectedResolutionList = new List<Resolution>();

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;

    public override void SetInfo(UIBaseData uiData)
    {
        base.SetInfo(uiData);

        List<string> languageOptions = new List<string>();
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            languageOptions.Add(locale.Identifier.Code);
        }
        LanguageDropDown.AddOptions(languageOptions);

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
            var selectedLanguage = userSettingsData.Language;
            LanguageDropDown.value = (int)selectedLanguage;
            var selectedResolution = userSettingsData.ResolutionIndex;
            ResDropDown.value = selectedResolution;
            FullScreenToggle.isOn = userSettingsData.FullScreen;
            Screen.SetResolution(SelectedResolutionList[selectedResolution].width,
                                SelectedResolutionList[selectedResolution].height,
                                userSettingsData.FullScreen);
            MusicSlider.value = userSettingsData.Music_Volume;
            SFXSlider.value = userSettingsData.SFX_Volume;
        }

        SwitchTab(0); // Default to the first tab
    }

    public void SwitchTab(int _tabId)
    {
        foreach(GameObject go in Tabs)
        {
            go.SetActive(false);
        }
        Tabs[_tabId].SetActive(true);

        foreach(RectTransform tabButton in TabButtons)
        {
            tabButton.anchoredPosition = new Vector2(InactiveButtonPosX, tabButton.anchoredPosition.y);
        }
        TabButtons[_tabId].anchoredPosition = new Vector2(ActiveButtonPosX, TabButtons[_tabId].anchoredPosition.y);
    }

    public void SetLanguage()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            userSettingsData.Language = (LocalizationLanguage)LanguageDropDown.value;
            userSettingsData.SaveData();
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)userSettingsData.Language];
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
            AudioManager.Instance.SetMusicVolume(userSettingsData.Music_Volume);
        }
    }

    public void SetSFXVolume()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            userSettingsData.SFX_Volume = SFXSlider.value;
            userSettingsData.SaveData();
            AudioManager.Instance.SetSFXVolume(userSettingsData.SFX_Volume);
        }
    }
}
