using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraInitialiser : MonoBehaviour {
    
	void Start ()
    {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
    }
}
