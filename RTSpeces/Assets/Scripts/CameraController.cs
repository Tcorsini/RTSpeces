using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_speed = 0.5f;
    public float m_rotateSpeed = 1f;

    public float m_orthographicWheelScale = 1f;
    public float m_PerspectiveWheelscale = 7f;

    public float m_minFieldOfView = 10f;
    public float m_maxFieldOfView = 90f;

    public float m_minSize = 1f;
    public float m_maxSize = 10f;

    public Camera m_camera;

    //valeur (proportionnelle a l'ecran a partir de 
    //laquelle la souris est consideree sur le bord
    public float m_mouseLimitBorder = 0.01f;
    public float m_mouseDefilling = 0.3f;

    //permet de detecter si les touches fleches ont ete 
    //presse pendant la rotation, utile pour eviter les
    //decallages
    //private bool isRotatedHorizontal = false;
    //private bool isRotatedVertical = false;

    private void Awake()
    {
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    //fonction principale de mouvement, traduit les input
    private void Move()
    {
        //pour zoomer
        float wheelDelta = -Input.mouseScrollDelta.y;
        ZoomCamera(wheelDelta);

        //pour faire tourner la camera
        if (Input.GetKey(Parameters.rotateKey))
        {
            float offset = Input.GetAxis("Horizontal");
            RotateCamera(offset);
        }
        else //pour bouger la camera
        {

            float HOffset = Input.GetAxis("Horizontal") * m_speed;
            float VOffset = Input.GetAxis("Vertical") * m_speed;

            Vector2 mousePos = GetMouseBorderValue();
            MoveCamera(HOffset + mousePos.x, VOffset + mousePos.y);
        }
    }

    private void ZoomCamera(float offset)
    {
        //set the scale of the camera
        if (offset != 0f)
        {
            if (m_camera.orthographic)
            {
                offset *= m_orthographicWheelScale;
                float view = m_camera.orthographicSize;
                view += offset;
                view = Mathf.Max(Mathf.Min(view, m_maxSize), m_minSize);
                m_camera.orthographicSize = view;
            }
            else
            {
                offset *= m_PerspectiveWheelscale;
                float view = m_camera.fieldOfView;
                view += offset;
                view = Mathf.Max(Mathf.Min(view, m_maxFieldOfView), m_minFieldOfView);
                m_camera.fieldOfView = view;
            }
        }
    }

    //pour faire pivoter la camera
    private void RotateCamera(float offset)
    {
        transform.Rotate(0, 0, offset * m_rotateSpeed);
    }

    //pour deplacer en x,y la camera
    private void MoveCamera(float horizontalOffset, float verticalOffset)
    {
        if (horizontalOffset != 0f || verticalOffset != 0f)
        {

            float angle = transform.localEulerAngles.z * Mathf.Deg2Rad;

            float deltaX = horizontalOffset * Mathf.Cos(angle) - verticalOffset * Mathf.Sin(angle);
            float deltaY = horizontalOffset * Mathf.Sin(angle) + verticalOffset * Mathf.Cos(angle);

            // Translates the camera to a new position, at a given speed.
            Vector2 newPos = new Vector2(transform.position.x + deltaX,
                                         transform.position.y + deltaY);


            this.transform.position = new Vector3(newPos.x,
                                                  newPos.y,
                                                  this.transform.position.z);
        }
    }

    //renvoie une valeur pour faire defiler l ecran selon la position de
    //la souris
    public Vector2 GetMouseBorderValue()
    {
        //Camera[] allCameras = Object.FindObjectsOfType(typeof(Camera)) as Camera[];
        Vector2 res = new Vector2(0, 0);
        Vector3 point = m_camera.ScreenToViewportPoint(Input.mousePosition);
        if (point.x >= 0 && point.x <= 1 && point.y >= 0 && point.y <= 1)
        {
            if (point.x <= m_mouseLimitBorder)
            {
                res.x = -m_mouseDefilling;
            }
            if (point.x >= 1 - m_mouseLimitBorder)
            {
                res.x = m_mouseDefilling;
            }
            if (point.y <= m_mouseLimitBorder)
            {
                res.y = -m_mouseDefilling;
            }
            if (point.y >= 1 - m_mouseLimitBorder)
            {
                res.y = m_mouseDefilling;
            }
        }
        return res;
    }
}
