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

        float randomScaleValue = Random.Range(_uiManager.minStarSize.Value, _uiManager.maxStarSize.Value) / 20f;
        transform.localScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue);
        particles.transform.localScale = new Vector3(randomScaleValue, randomScaleValue, randomScaleValue) * 0.65f;
        
        gravitationCost = randomScaleValue * 30;

        pb.SetColor("_Color1", Random.ColorHSV());
        pb.SetColor("_Color2", Random.ColorHSV());
        currentMat.SetPropertyBlock(pb);

        starUi.transform.forward = transform.position - Camera.main.transform.position;

        transform.name = starName;
        transform.position = RandomVec3(UIManager.Instance.spawnRange.Value);

        ChangeColour(Color.black);
    }

    public void ChangeParticleColor(Color color) => particlesMain.startColor = color;
    public void ChangeParticleColor() => particlesMain.startColor = Random.ColorHSV();

    Vector3 RandomVec3(float range) {
        return new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
    }

    public void ChangeColour(Color colour) { backDrop.color = colour; }

    public void Interact() {
        PathFinder.instance.SelectDestination(this);
    }
}