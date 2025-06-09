using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarController : MonoBehaviour, IInteract, IPool {

    UIManager _uiManager;
    string starName;
    public float gravitationCost;

    [SerializeField] ParticleSystem particles;
    [SerializeField] Canvas starUi;
    [SerializeField] TextMeshProUGUI starNameDisplay;
    [SerializeField] Transform planetsParent;
    [SerializeField] public Image backDrop;
    [SerializeField] Material sourceMat;

    Camera cam;
    Material currentMat;

    void Awake() {
        cam = Camera.main;
    }

    void Update() {
        starUi.transform.forward = cam.transform.forward;
    }

    public void OnStarSpawn() {
        _uiManager = UIManager.Instance;

        float randomScaleValue = Random.Range(_uiManager.minStarSize, _uiManager.maxStarSize) / 20f;
        transform.localScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue);
        print(randomScaleValue);
        
        particles.transform.localScale *= (randomScaleValue / 0.2f);
        gravitationCost = randomScaleValue * 30;

        currentMat.SetColor("_Color1", RandRGBColour());
        currentMat.SetColor("_Color2", RandRGBColour());

        starUi.transform.position = transform.position + new Vector3(0, 3, 0);
        starUi.transform.forward = transform.position - Camera.main.transform.position;
        starName = StarPetNames.names[Random.Range(0, StarPetNames.names.Length)] + "-" + Random.Range(0, 1000);
        starNameDisplay.text = starName;

        transform.name = starName;
        transform.position = RandomVec3(UIManager.Instance.spawnRange);

        #region Planets
        for (int j = 0; j < Random.Range(1, 10); j++) {
            Transform tempCentre = Instantiate(StarGeneration.instance.PlanetCentre).transform;
            Transform tempPlanet = Instantiate(StarGeneration.instance.planetList[UnityEngine.Random.Range(0, StarGeneration.instance.planetList.Count)]).transform;
            tempPlanet.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            tempPlanet.position = new Vector3(Random.Range(2, 10), 0, 0);
            tempPlanet.SetParent(tempCentre.transform);
            tempCentre.SetParent(planetsParent);
            tempCentre.position = transform.position;
            tempCentre.localEulerAngles = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
        }
        #endregion
    }

    Color RandRGBColour() {
        return new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
    }

    Vector3 RandomVec3(float range) {
        return new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }

    public void ChangeColour(Color colour) { backDrop.color = colour; }

    public void Interact() {
        print("Interacted STAR = " + starName);
    }

    public void OnPooled() {
        currentMat = new Material(sourceMat);
        MaterialPropertyBlock pb = new MaterialPropertyBlock();

    }
}