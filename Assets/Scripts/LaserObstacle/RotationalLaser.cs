using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalLaser : MonoBehaviour, ILaserType
{
    public Transform _RotationCenter;
    public float _Offset;
    public float _RotationalSpeed;
    public Vector3 _RotationAxis;
    // Start is called before the first frame update
    void Start()
    {
        InitializeLaserPosition();
    }

    // Update is called once per frame
    void Update()
    {
        LaserMovement();
    }

    public void LaserMovement()
    {
        this.transform.RotateAround(_RotationCenter.position, _RotationAxis, _RotationalSpeed * Time.deltaTime);
    }

    public void InitializeLaserPosition()
    {
        this.transform.position = _RotationCenter.transform.position;
        this.transform.position += new Vector3(_Offset, 0, 0);
    }
}

public interface ILaserType
{
    public void LaserMovement();
    
}
