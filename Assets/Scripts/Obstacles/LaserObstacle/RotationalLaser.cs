using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalLaser : BaseObstacle, ILaserType
{
    public Transform _RotationCenter;
    //public float _Offset;
    public float _Range;
    public float _RotationalSpeed;
    public float originalRotation;
    public Vector3 _RotationAxis;
    //public Vector3 _OffsetAxis;
    // Start is called before the first frame update
    void Start()
    {
        InitializeLaserPosition();
        originalRotation = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        LaserMovement();
    }

    public void LaserMovement()
    {
        float angle = Mathf.Sin(Time.time * _RotationalSpeed) * _Range + originalRotation;
        transform.rotation = Quaternion.identity;
        //transform.rotation =  Quaternion.AngleAxis(angle * Mathf.Deg2Rad, _RotationAxis) * transform.rotation;
        transform.Rotate(_RotationAxis, angle);
        //this.transform.RotateAround(_RotationCenter.position, _RotationAxis, _RotationalSpeed * Time.deltaTime);
    }

    public void InitializeLaserPosition()
    {
        if (_RotationCenter != null)
        {
            this.transform.position = _RotationCenter.transform.position;
        }
        else
        {
            Debug.LogWarning("RotationCenter missing for " + this.transform.gameObject.name);
        }
        //this.transform.position += _Offset * _OffsetAxis;
    }
}

public interface ILaserType
{
    public void LaserMovement();
    
}
