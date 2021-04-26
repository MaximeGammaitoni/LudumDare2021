using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]

public class LaserBehaviour : MonoBehaviour
{
    // Inspector
    public Material _LaserMaterial;
    public ILaserType _LaserType;
    public LayerMask _WallLayerMask;
    public UnityEvent _OnPlayerHit;
    public float OriginSize = 30;

    // Variables
    private LineRenderer _laserRenderer;
    

    private void Start()
    {
        _laserRenderer = GetComponent<LineRenderer>();
        if (_laserRenderer == null)
        {
            gameObject.AddComponent<LineRenderer>();
            _laserRenderer = GetComponent<LineRenderer>();

        }
        if (_LaserMaterial != null)
        {
            _laserRenderer.material = _LaserMaterial;
        }
        else
        {
            Debug.Log("LaserMaterial is null on " + transform.gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        GenerateRaycasts();
    }

    private void GenerateRaycasts()
    {

        int mask = _WallLayerMask.value;

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, Mathf.Infinity, mask))
        {
            GenerateLaser(new Vector3(0, 0, Vector3.Distance(transform.position, hit.point)));
            CheckPlayerHit(hit);
        }
        else
        {
            GenerateLaser(new Vector3(0, 0, OriginSize));
        }
        
    }

    private void GenerateLaser(Vector3 position)
    {
        //_laserRenderer.SetPosition(0, transform.position);
        _laserRenderer.SetPosition(1, position);
    }

    private RaycastHit CheckPlayerHit(RaycastHit hit)
    {
        if (hit.transform.gameObject.CompareTag("Player"))
        {
            GameManager.singleton.PlayerEvents.PlayerIsDead();
        }

        return hit;
    }
}
