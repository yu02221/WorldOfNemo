using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    public GameObject OptionsMenu;

    public Text masterText;
    public Text musicText;
    public Text blockText;
    public Text friendlyText;
    public Text hostileText;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider blockSlider;
    public Slider friendlySlider;
    public Slider hostileSlider;

    public AudioSource clickButtonSound;
    public AudioMixer mixer;

    public DayAndNight dn;

    public void SetSound()
    {
      
        masterText.text = ($" Master Volume : ") + Mathf.RoundToInt(masterSlider.value * 100) + "%";
        mixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        musicText.text = ($" Music : ") + Mathf.RoundToInt(musicSlider.value * 100) + "%";
        mixer.SetFloat("Bgm", Mathf.Log10(musicSlider.value) * 20);
        blockText.text = ($" Block : ") + Mathf.RoundToInt(blockSlider.value * 100) + "%";
        mixer.SetFloat("Block", Mathf.Log10(blockSlider.value) * 20);
        friendlyText.text = ($" Friendly Creatures : ") + Mathf.RoundToInt(friendlySlider.value * 100) + "%";
        mixer.SetFloat("Friendly", Mathf.Log10(friendlySlider.value) * 20);
        hostileText.text = ($" Hostile Creatures : ") + Mathf.RoundToInt(hostileSlider.value * 100) + "%";
        mixer.SetFloat("Hostile", Mathf.Log10(hostileSlider.value) * 20);
    }

    public void StartGame()
    {
        clickButtonSound.Play();
        StartCoroutine(LoadingSceneDelay());
    }

    public void QuitGame()
    {
        clickButtonSound.Play();
        Application.Quit(); //게임나가기
    }
    
    public void OnClickOptions()
    {
        clickButtonSound.Play();
        OptionsMenu.SetActive(true);  //옵션들어가기
        GetOption();
        SetSound();
    }

    public void OptionBack()
    {
        clickButtonSound.Play();
        SaveOption();
        OptionsMenu.SetActive(false);  //옵션나가기
    }

    public void SaveOption()
    {
        PlayerPrefs.SetFloat("Master Volume", masterSlider.value);
        
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        
        PlayerPrefs.SetFloat("Block", blockSlider.value);
        
        PlayerPrefs.SetFloat("Friendly Creatures", friendlySlider.value);
        
        PlayerPrefs.SetFloat("Hostile Creatures", hostileSlider.value);
        
    }

    public void GetOption()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Master Volume");
        musicSlider.value = PlayerPrefs.GetFloat("Music");
        blockSlider.value = PlayerPrefs.GetFloat("Block");
        friendlySlider.value = PlayerPrefs.GetFloat("Friendly Creatures");
        hostileSlider.value = PlayerPrefs.GetFloat("Hostile Creatures");
    }

    IEnumerator LoadingSceneDelay()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Loading"); //게임 들어가기
    }

    public void AccelTime()
    {
        dn.timeSpeed = 100;
    }

    public void NormalTime()
    {
        dn.timeSpeed = 1;
    }
}
