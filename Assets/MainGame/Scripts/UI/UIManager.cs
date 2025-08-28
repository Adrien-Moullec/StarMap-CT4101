using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

    public static UIManager Instance;
    public static List<StarSliderValues> allSliders = new List<StarSliderValues>();

    #region Play UI
    [Header("Star Info")]
    [SerializeField] public TextMeshProUGUI _startStarTextUI;
    [SerializeField] public TextMeshProUGUI _endStarTextUI;
    [SerializeField] public TextMeshProUGUI _starPathTextUI;

    public const string empty = "---";
    #endregion

    #region Audio
    [Space]
    [Header("Audio")]
    [SerializeField] public AudioSource starSelectAudio;
    [SerializeField] public AudioSource starDeselectAudio;
    #endregion

    [Space]
    [Header("Slider Values")]
    [Space]
    
    [SerializeField] public StarSliderValues spawnRange;
    [SerializeField] public StarSliderValues spawnCount;
    [SerializeField] public StarSliderValues leapDistance;
    [SerializeField] public StarSliderValues minStarSize;
    [SerializeField] public StarSliderValues maxStarSize;
    [SerializeField] public StarSliderValues evilRegionRange;
    [SerializeField] public StarSliderValues evilRegionMult;

    [Space]
    [Header("Dropdown options")]
    [SerializeField] public TMP_Dropdown pathfindingOptions;
    [SerializeField] public Toggle quickFindToggle;
    [SerializeField] public TMP_Dropdown optimizationOptions;

    [Space]
    [Header("Buttons")]
    [SerializeField] Button defaultValuesButton;
    [SerializeField] Button resetStarsButton;
    [SerializeField] Button exitButton;

    //Text & Image
    [Space]
    [Header("Loading star")]
    [SerializeField] TextMeshProUGUI loadingStarText;
    [SerializeField] RawImage loadingStarImage;
    [SerializeField] Slider loadingSlider;

    //Colors
    Color imageColor;
    Color textColor;

    private void Awake() {
        Instance = this;
        loadingStarImage.gameObject.SetActive(false);

        #region Setting slider values
        spawnRange.Awaken();
        spawnCount.Awaken();
        leapDistance.Awaken();
        minStarSize.Awaken();
        maxStarSize.Awaken();
        evilRegionRange.Awaken();
        evilRegionMult.Awaken();
        #endregion

        #region Setting Play UI
        _startStarTextUI.text = empty;
        _endStarTextUI.text = empty;
        _starPathTextUI.text = empty;

        textColor = loadingStarText.color; textColor.a = 0; loadingStarText.color = textColor;
        imageColor = loadingStarImage.color; imageColor.a = 0; loadingStarImage.color = imageColor;
        #endregion

        #region Button Setup
        defaultValuesButton.onClick.AddListener(delegate {
            Instance.starSelectAudio.Play(); 
            foreach (StarSliderValues s in allSliders) s.ResetSliderValues(); 
        });
        exitButton.onClick.AddListener(() => Application.Quit());
        #endregion
    }

    //Make the loading star flash by changing alpha value
    public IEnumerator LoadingStarFlash(string message) {
        float alphaValue;
        float time = 0;
        loadingStarText.text = message;
        loadingStarImage.gameObject.SetActive(true);

        while (true) {
            time += Time.deltaTime; time = time > (4*Mathf.PI) ? 0 : time;
            alphaValue = Mathf.Abs(Mathf.Sin(time));
            print(alphaValue);
            SetLoadStarOpacity(alphaValue);

            yield return null;
        }
    }
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! SET Interface for star load specifically???????????????
    public void SetLoadStarOpacity(float alpha) {
        imageColor.a = alpha; textColor.a = alpha;
        loadingStarImage.color = imageColor; loadingStarText.color = textColor;
    }
    public void DisableStar() {
        loadingStarImage.gameObject.SetActive(false);
        imageColor.a = 0; textColor.a = 0;
        loadingStarImage.color = imageColor; loadingStarText.color = textColor;
    }

    public void SetLoadStarBar(int num) {
        loadingSlider.value = num;
        //if (alpha == 0 || alpha == 1)  
    }
    public void SetLoadStarBarMax(int num) {
        loadingSlider.maxValue = num;
    }

    public void UpdatePathList(bool foundPath) {

        if (!foundPath) {
            _starPathTextUI.text = "Failed to find path";
            _starPathTextUI.color = Color.red;
            return;
        }

        _starPathTextUI.color = Color.white;
        if (StarGeneration.instance.finalStarPath.Count > 1) {
            _starPathTextUI.text = StarGeneration.instance.finalStarPath[StarGeneration.instance.finalStarPath.Count - 1].name;
            for (int i = StarGeneration.instance.finalStarPath.Count - 2; i >= 0; i--) {
                _starPathTextUI.text = _starPathTextUI.text + "\n" + StarGeneration.instance.finalStarPath[i].name;
            }
        } else {
            _starPathTextUI.text = "Failed to find path";
            _starPathTextUI.color = Color.red;
            return;
        }
    }

    public void ResetStars() {
        _starPathTextUI.text = "";
        _starPathTextUI.color = Color.black;
    }
}