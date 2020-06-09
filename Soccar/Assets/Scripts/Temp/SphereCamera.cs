using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCamera : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    private Vector3 _relativeDistance;

    // Start is called before the first frame update
    void Start()
    {
        _relativeDistance = new Vector3(0, 2.0f, -2.0f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _gameObject.transform.position + _relativeDistance;
        transform.rotation = Quaternion.LookRotation((_gameObject.transform.position - transform.position).normalized);
    }
}
