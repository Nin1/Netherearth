using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** NOTE: Currently only works with canvas UI Scale Mode "Scale With Screen Size" in Expand or Shrink match mode */
public class UICursor : MonoBehaviour {

    private RectTransform m_transform;
    public CanvasScaler m_canvasScaler;
    
    void Start () {
        m_transform = GetComponent<RectTransform>();

        if (m_canvasScaler == null)
        {
            m_canvasScaler = GetComponentInParent<CanvasScaler>();
        }
    }
	
	void Update () {
        float mousePosX;
        float mousePosY;

        SoftwareMouse.GetMousePos(out mousePosX, out mousePosY);

        // Center around 0,0
        mousePosX -= Screen.width / 2;
        mousePosY -= Screen.height / 2;

        Vector2 canvasSpace = SoftwareMouse.ScreenToCanvas(new Vector2(mousePosX, mousePosY));
        
        m_transform.localPosition = new Vector3(canvasSpace.x, canvasSpace.y);
	}
}
