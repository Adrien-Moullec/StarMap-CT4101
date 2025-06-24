using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarMaps;

public class StarController : MonoBehaviour, IInteract, IPool {

    UIManager _uiManager;
    public string starName {
        get { return transform.name; }
        set { transform.name = value; starNameDisplay.text = value; }
    }
        
    public float gravitationCost;

    [SerializeField] ParticleSystem particles;
    [SerializeField] ParticleSystem.MainModule particlesMain;
    [SerializeField] Canvas starUi;
    [SerializeField] TextMeshProUGUI starNameDisplay;
    [SerializeField] Transform planetsParent;
    [SerializeField] public Image backDrop;
    [SerializeField] Material sourceMat;

    Camera cam;
    MeshRenderer currentMat;
    MaterialPropertyBlock pb;

    void Awake() {
        cam = Camera.main;
        currentMat = GetComponent<MeshRenderer>();
        pb = new MaterialPropertyBlock();
        particlesMain = particles.main;
    }

    void Update() {
        starUi.transform.forward = cam.transform.forward;
    }

    public void OnPooled() {
        _uiManager = UIManager.Instance;
        ChangeParticleColor(Color.white);

        float randomScaleValue = Random.Range(_uiManager.minStarSize, _uiManager.maxStarSize) / 20f;
        transform.localScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue);
        particles.transform.localScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue) * 0.65f;
        
        gravitationCost = randomScaleValue * 30;

        pb.SetColor("_Color1", Random.ColorHSV());
        pb.SetColor("_Color2", Random.ColorHSV());
        currentMat.SetPropertyBlock(pb);

        starUi.transform.forward = transform.position - Camera.main.transform.position;

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

    public void ChangeParticleColor(Color color) => particlesMain.startColor = color;

    Vector3 RandomVec3(float range) {
        return new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }

    public void ChangeColour(Color colour) { backDrop.color = colour; }

    public void Interact() {
        StartCoroutine(PathFinder.instance.SelectDestination(this));
    }
}