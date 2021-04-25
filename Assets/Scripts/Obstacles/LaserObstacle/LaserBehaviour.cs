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
        if(Physics.Raycast(this.transform.position, this.transform.forward,out hit, Mathf.Infinity, mask))
        {
            CheckPlayerHit(hit);
            GenerateLaser(hit);
        }
    }

    private void GenerateLaser(RaycastHit hit)
    {
        _laserRenderer.SetPosition(0, transform.position);
        _laserRenderer.SetPosition(1, hit.point);
    }

    private RaycastHit CheckPlayerHit(RaycastHit hit)
    {
        if (hit.transform.gameObject.CompareTag("Player"))
        {
            _OnPlayerHit?.Invoke();
        }

        return hit;
    }
}
