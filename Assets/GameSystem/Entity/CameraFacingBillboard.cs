using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFacingBillboard : MonoBehaviour {

    Camera m_camera;

    public bool m_reverseDirection= false;

	// Use this for initialization
	void Awake () {
        SetMainCamera();
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        SetMainCamera();
        FaceTowardsCamera();
	}

    void FaceTowardsCamera()
    {
        Vector3 target;
        if (m_reverseDirection)
        {
            target = transform.position + m_camera.transform.rotation * -Vector3.forward;
        }
        else
        {
            target = transform.position + m_camera.transform.rotation * Vector3.forward;
        }
        Vector3 up = m_camera.transform.rotation * Vector3.up;
        transform.LookAt(target, up);
    }

    void SetMainCamera()
    {
        if (!m_camera || !m_camera.CompareTag("MainCamera"))
        {
            m_camera = Camera.main;
        }
    }
}
