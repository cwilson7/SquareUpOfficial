﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    private RawImage image;
    private VideoPlayer videoPlayer;
    [HideInInspector] public GameObject Character;
    [HideInInspector] public CharacterInfo charInfo;
    public GameObject CosmeticPanel;
    private VideoClip demo;
    [SerializeField] private TMP_Text header;
    public GameObject MainShopPnl;

    GameObject DisplayedCharacter;
    Vector3 DisplayedCharacterOffset = new Vector3(1f, 0, 0);

    [SerializeField] private GameObject LockedPanel, UnlockedPanel;

    public void Setup()
    {
        GrabComponents();
        CharacterSetup();
        SetUpAbilityDemo();
        SetPanelLockedInfo();
        SetUpCosmeticPanel();
    }

    private void SetUpCosmeticPanel()
    {
        CosmeticPanel.GetComponent<CosmeticPanel>().charInfo = charInfo;
        CosmeticPanel.GetComponent<CosmeticPanel>().DisplayedCharacter = DisplayedCharacter;
        CosmeticPanel.GetComponent<CosmeticPanel>().Initialize();
    }

    public void SetPanelLockedInfo()
    {
        CharacterInfo info = (CharacterInfo)ProgressionSystem.Instance.Characters[Character.GetComponent<AvatarCharacteristics>().info.characterName];
        if (info.status == Status.Unlocked)
        {
            UnlockedPanel.SetActive(true);
            LockedPanel.SetActive(false);
        }
        else
        {
            LockedPanel.SetActive(true);
            UnlockedPanel.SetActive(false);
        }
    }

    void GrabComponents()
    {
        image = GetComponentInChildren<RawImage>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
    }

    void CharacterSetup()
    {
        //given character from shop controller
        charInfo = (CharacterInfo)ProgressionSystem.Instance.Characters[Character.GetComponent<AvatarCharacteristics>().info.characterName];
        header.text = charInfo.characterName;
        DisplayedCharacter = Instantiate(Character, new Vector3(Camera.main.transform.position.x + 1.5f, Camera.main.transform.position.y - 2, Camera.main.transform.position.z  + 5), Quaternion.Euler(0, 180, 0));
        //sr.sprite = Character model as sprite
        //turn model into sprite and set sprite to this
    }

    void SetUpAbilityDemo()
    {
        //do this better
        //RenderTexture videoTexture = Resources.Load<RenderTexture>("Images/RenderTexures");
        //demo = Character.GetComponent<AvatarCharacteristics>().myDemo;
        //videoPlayer.clip = demo;
        //image.texture = videoTexture;
        //videoPlayer.targetTexture = videoTexture;

    }

    public void DontDisplay()
    {
        videoPlayer.Stop();
        DisplayedCharacter.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Display()
    {
        gameObject.SetActive(true);
        videoPlayer.Play();
        DisplayedCharacter.SetActive(true);
    }

    public void ReturnToMainPanel()
    {
        ShopController.Instance.mainPanel.SetActive(true);
        DontDisplay();
    }

    public void OpenCosmeticsPanel()
    {
        CosmeticPanel.SetActive(true);
        MainShopPnl.SetActive(false);
        DisplayedCharacter.transform.position += DisplayedCharacterOffset;
    }

    public void CloseCosmeticsPanel()
    {
        CosmeticPanel.SetActive(false);
        MainShopPnl.SetActive(true);
        DisplayedCharacter.transform.position -= DisplayedCharacterOffset;
    }
}
