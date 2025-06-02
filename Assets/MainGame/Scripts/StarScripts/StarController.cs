using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarController : MonoBehaviour, IInteract {

    UIManager _uiManager;
    string starName;
    public float gravitationCost;

    [SerializeField] ParticleSystem particles;
    [SerializeField] Renderer starBody;
    [SerializeField] Canvas starUi;
    [SerializeField] TextMeshProUGUI starNameDisplay;
    [SerializeField] Transform planetsParent;
    [SerializeField] public Image backDrop;

    Camera cam;

    void Awake() {
        cam = Camera.main;
    }

    void Update() {
        starUi.transform.forward = cam.transform.forward;
    }

    private void Start() {
        _uiManager = UIManager.Instance;

        float randomScaleValue = Random.Range(_uiManager.minStarSize, _uiManager.maxStarSize) / 100f;
        transform.localScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue);
        
        particles.transform.localScale *= (randomScaleValue / 0.2f);
        gravitationCost = randomScaleValue * 30;

        starBody.material.SetColor("_Color1", RandRGBColour());
        starBody.material.SetColor("_Color2", RandRGBColour());

        starUi.transform.position = transform.position + new Vector3(0, 3, 0);
        starUi.transform.forward = transform.position - Camera.main.transform.position;
        starName = StarPetNames.names[Random.Range(0, StarPetNames.names.Length)] + " - " + Random.Range(0, 1000);
        starNameDisplay.text = starName;

        transform.name = starName;

        #region Planets
        for (int j = 0; j < UnityEngine.Random.Range(1, 10); j++) {
            Transform tempCentre = Instantiate(StarGeneration.instance.PlanetCentre).transform;
            Transform tempPlanet = Instantiate(StarGeneration.instance.planetList[UnityEngine.Random.Range(0, StarGeneration.instance.planetList.Count)]).transform;
            tempPlanet.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            tempPlanet.position = new Vector3(UnityEngine.Random.Range(2, 10), 0, 0);
            tempPlanet.SetParent(tempCentre.transform);
            tempCentre.SetParent(planetsParent);
            tempCentre.position = transform.position;
            tempCentre.localEulerAngles = new Vector3(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180));
        }
        #endregion
    }

    UnityEngine.Color RandRGBColour() {
        return new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
    }

    public void ChangeColour(Color colour) { backDrop.color = colour; }

    public void Interact() {
        print("Interacted STAR = " + starName);
    }
}