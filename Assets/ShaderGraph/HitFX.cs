using _SCRIPTS;
using UnityEngine;

public class HitFX : MonoBehaviour {

    private Material material;
    private Color materialTintColor;
    private DefenceHandler defence;
    public Renderer renderer;

   private float tintFadeSpeed = 6f;
    [SerializeField]private Color tintColor = Color.red;
    private float duration = .1f;

    private void Start()
    {
        defence = GetComponent<DefenceHandler>();
        defence.OnDamaged += Damaged;
        materialTintColor = new Color();
        SetMaterial(renderer.material);
    }

    private void Damaged(Vector3 arg1, float damage, Vector3 arg3)
    {

        SetTintColor(tintColor);
        SHAKER.ShakeCamera(transform.position, damage);
        HITSTUN.StartStun(duration);
    }

    private void Update() {
        if (materialTintColor.a > 0) {
            materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
            material.SetColor("_Tint", materialTintColor);
        }
    }

    public void SetMaterial(Material material) {
        this.material = material;
    }

    public void SetTintColor(Color color) {
        materialTintColor = color;
        material.SetColor("_Tint", materialTintColor);
    }

    public void SetTintFadeSpeed(float tintFadeSpeed) {
        this.tintFadeSpeed = tintFadeSpeed;
    }

}
