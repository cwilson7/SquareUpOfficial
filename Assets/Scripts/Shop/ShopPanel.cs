using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    private RawImage image;
    private VideoPlayer videoPlayer;
    [HideInInspector] public GameObject Character;
    public Transform charLocation;
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
        CharacterInfo info = ProgressionSystem.CharacterData(Character.GetComponent<AvatarCharacteristics>().info);
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
        charInfo = ProgressionSystem.CharacterData(Character.GetComponent<AvatarCharacteristics>().info);
        header.text = charInfo.characterName;
        DisplayedCharacter = Instantiate(Character, charLocation);
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
        //DisplayedCharacter.SetActive(false);
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
    }

    public void CloseCosmeticsPanel()
    {
        CosmeticPanel.SetActive(false);
        MainShopPnl.SetActive(true);
    }
}
