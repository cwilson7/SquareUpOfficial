using System.Collections;
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
    private VideoClip demo;
    [SerializeField] private TMP_Text header;

    GameObject DisplayedCharacter;

    [SerializeField] private GameObject LockedPanel, UnlockedPanel;

    public void Setup()
    {
        GrabComponents();
        CharacterSetup();
        SetUpAbilityDemo();
        SetPanelLockedInfo();
    }

    public void SetPanelLockedInfo()
    {
        CharacterInfo info = (CharacterInfo)ProgressionSystem.Instance.Characters[Character.name];
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
        charInfo = (CharacterInfo)ProgressionSystem.Instance.Characters[Character.name];
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
}
