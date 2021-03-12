using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private Transform _focusObject;

    // Screenshake stuff
    private float _screenshakeOffset;
    private Vector3 _neutralPosition;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        EventManager<Transform>.AddListener(EventType.SetCameraFocus, SetFocus);
    }

    private void Start()
    {
        _neutralPosition = transform.localPosition;
    }

    private void SetFocus(Transform focusObject)
    {
        StartCoroutine(ChangeFocus(focusObject));
    }

    private IEnumerator ChangeFocus(Transform focusObject)
    {
        yield return new WaitForSeconds(0.2f);
        _focusObject = focusObject;
    }

    public void ShakeScreen(float intensity)
    {
        _screenshakeOffset = intensity;
    }

    private void Update()
    {
        HandleFocus();
        HandleScreenshake();
    }

    private void HandleFocus()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, -10);
        Vector3 objectPos = new Vector3(_focusObject.position.x, _focusObject.position.y, -10);
        if (pos != objectPos)
        {
            transform.localPosition = Vector3.Lerp(pos, objectPos, 0.25f);
            _neutralPosition = transform.localPosition;
        }
    }

    private void HandleScreenshake()
    {
        if (_screenshakeOffset > 0)
        {
            float direction = Random.Range(-1, 1);
            transform.localPosition = _neutralPosition + new Vector3(_screenshakeOffset * Mathf.Cos(direction), _screenshakeOffset * Mathf.Sin(direction));
            
            _screenshakeOffset *= 0.8f;
            if (_screenshakeOffset < 0.1f)
            {
                transform.localPosition = _neutralPosition;
                _screenshakeOffset = 0;
            }
        }
    }
}