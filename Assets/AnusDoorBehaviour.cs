using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnusDoorBehaviour : MonoBehaviour
{
    private Animator _anusAnimator;
    public bool _isOpened;


    private void Start()
    {
        _anusAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isOpened)
        {
            _anusAnimator.SetFloat("Blend", 1f, 0.2f, Time.deltaTime); 
        }
        else
        {
            _anusAnimator.SetFloat("Blend", 0f, 0.2f, Time.deltaTime);
        }
    }

    public void IsOpened()
    {
        _isOpened = true;
    }

    public void IsClosed()
    {
        _isOpened = false;
    }

    
}
