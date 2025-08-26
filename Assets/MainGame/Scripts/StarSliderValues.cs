
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class StarSliderValues : MonoBehaviour {
    [HideInInspector] public int Value {
        get {
            if (toggle == null)
                return (int)slider.value;
            else
                return toggle.isOn ? (int)slider.value : 0;
        }
        set {
            slider.value = value;
            textDisplay.text = value.ToString();
        }
    }
    [Header("Default Values")]
    [SerializeField] public int DefaultValue = 10;
    [SerializeField] public bool DefaultToggle = true;
    [Space]
    [Header("UI Elements")]
    [SerializeField] public Slider slider;
    [SerializeField] public TextMeshProUGUI textDisplay;
    [SerializeField] public Toggle toggle;

    public void Awaken() {
        slider.onValueChanged.AddListener(delegate { Value = (int)slider.value; });
        if (toggle != null) toggle.onValueChanged.AddListener(delegate { Value = (int)slider.value; });
        UIManager.allSliders.Add(this);
        ResetSliderValues();
    }

    public void ResetSliderValues() {
        if (toggle != null) toggle.isOn = DefaultToggle;

        Value = DefaultValue;
    }

    private void OnValidate() {
        slider = GetComponent<Slider>();
    }
}