using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    #region Play UI
    [Header("Main Star Focus")]
    [SerializeField] public TextMeshProUGUI _startStarTextUI;
    [SerializeField] public TextMeshProUGUI _endStarTextUI;
    [SerializeField] public TextMeshProUGUI _starPathTextUI;

    const string empty = "---";
    #endregion

    #region Audio
    [Space]
    [Header("Audio")]
    [SerializeField] public AudioSource starSelectAudio;
    [SerializeField] public AudioSource starDeselectAudio;
    #endregion

    #region Sliders and Values
    public int spawnRange { get { return (int)sliderSpawnRange.value; } }
    public int spawnCount { get { return (int)sliderSpawnCount.value; } }
    public int leapDistance { get { return (int)sliderLeapDistance.value; } }
    public int minStarSize { get { return (int)sliderMinStarSize.value; } }
    public int maxStarSize { get { return (int)sliderMaxStarSize.value; } }

    [Space]
    [Header("Sliders")]
    [SerializeField] public Slider sliderSpawnRange;
    [SerializeField] public Slider sliderSpawnCount;
    [SerializeField] public Slider sliderLeapDistance;
    [SerializeField] public Slider sliderMinStarSize;
    [SerializeField] public Slider sliderMaxStarSize;

    [Space]
    [Header("Text References")]
    [SerializeField] TextMeshProUGUI textSpawnRange;
    [SerializeField] TextMeshProUGUI textSpawnCount;
    [SerializeField] TextMeshProUGUI textLeapDistance;
    [SerializeField] TextMeshProUGUI textMinStarSize;
    [SerializeField] TextMeshProUGUI textMaxStarSize;
    #endregion
    
    //Text & Image
    [SerializeField] TextMeshProUGUI loadingStarText;
    [SerializeField] RawImage loadingStarImage;

    //Bools
    [HideInInspector] public bool continueLoad;

    //Colors
    Color imageColor;
    Color textColor;    

    private void Awake() {
        Instance = this;

        #region Setting slider values
        sliderSpawnRange.value = 60;
        sliderSpawnCount.value = 50;
        sliderLeapDistance.value = 40;
        sliderMinStarSize.value = 10;
        sliderMaxStarSize.value = 50;

        textSpawnRange.text = spawnRange.ToString();
        textSpawnCount.text = spawnCount.ToString();
        textLeapDistance.text = leapDistance.ToString();
        textMinStarSize.text = minStarSize.ToString();
        textMaxStarSize.text = maxStarSize.ToString();

        sliderSpawnRange.onValueChanged.AddListener(delegate { textSpawnRange.text = spawnRange.ToString(); });
        sliderSpawnCount.onValueChanged.AddListener(delegate { textSpawnCount.text = spawnCount.ToString(); });
        sliderLeapDistance.onValueChanged.AddListener(delegate { textLeapDistance.text = leapDistance.ToString(); });
        sliderMinStarSize.onValueChanged.AddListener(delegate { textMinStarSize.text = minStarSize.ToString(); });
        sliderMaxStarSize.onValueChanged.AddListener(delegate { textMaxStarSize.text = maxStarSize.ToString(); });
        #endregion

        #region Setting Play UI
        _starPathTextUI.text = empty;
        _endStarTextUI.text = empty;
        _starPathTextUI.text = "";

        textColor = loadingStarText.color; textColor.a = 0; loadingStarText.color = textColor;
        imageColor = loadingStarImage.color; imageColor.a = 0; loadingStarImage.color = imageColor;
        #endregion
    }
    
    //Make the loading star flash by changing alpha value
    public IEnumerator LoadingStarFlash(string message) {
        float alphaValue;
        float time = 0;

        continueLoad = true;
        loadingStarText.text = message;

        while (continueLoad) {
            time += Time.deltaTime * 5; time = time > 4 ? 0 : time;
            alphaValue = Mathf.Abs(Mathf.Sin(time));
            imageColor.a = alphaValue; textColor.a = alphaValue;
            loadingStarImage.color = imageColor; loadingStarText.color = textColor;

            yield return null;
        }
        imageColor.a = 0; textColor.a = 0;
        loadingStarImage.color = imageColor; loadingStarText.color = textColor;
    }

    public void UpdatePathList(bool foundPath) {

        if (!foundPath) {
            _starPathTextUI.text = "Failed to find path";
            _starPathTextUI.color = Color.red;
            return;
        }

        _starPathTextUI.color = Color.white;
        _starPathTextUI.text = StarGeneration.instance.finalStarPath[StarGeneration.instance.finalStarPath.Count - 1].name;
        for (int i = StarGeneration.instance.finalStarPath.Count - 2; i >= 0; i--) {
            _starPathTextUI.text = _starPathTextUI.text + "\n" + StarGeneration.instance.finalStarPath[i].name;
        }
    }
}
