using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraInitialiser : MonoBehaviour {
    
    // We need to set the depth mode of the camera so we can fade-out to darkness
	void Start ()
    {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
    }
}
