using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpectatorMovement : NetworkBehaviour
{
    public Transform eyes;
    public GameObject characterModel;

    public float sens = 5;
    public float speed = 5;
    private Quaternion charTargetRot;
    private Quaternion camTargetRot;
    [SerializeField]
    private float _zoomSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        charTargetRot = gameObject.transform.localRotation;
        camTargetRot = eyes.localRotation;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        float yRot = Input.GetAxis(Keybinds.MouseY) * sens;
        float xRot = Input.GetAxis(Keybinds.MouseX) * sens;
        charTargetRot *= Quaternion.Euler(0f, xRot, 0f);
        camTargetRot *= Quaternion.Euler(-yRot, 0f, 0f);

        camTargetRot.x = Mathf.Clamp(camTargetRot.x, -.7f, .7f);
        camTargetRot.w = Mathf.Clamp(camTargetRot.w, .6f, .7f);
        gameObject.transform.localRotation = charTargetRot;
        eyes.transform.localRotation = camTargetRot;

        transform.Translate(Input.GetAxis("Vertical")*speed*Time.deltaTime*eyes.forward, Space.World);
        transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime * transform.right, Space.World);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(scroll * _zoomSpeed * eyes.forward, Space.World);
    }

    public void OnEnable()
    {
        if (hasAuthority)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //eyes.gameObject.SetActive(true);
            foreach (Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            characterModel.SetActive(false);
            for (int i = 0; i < eyes.childCount; i++)
            {
                eyes.GetChild(i).gameObject.SetActive(true);
            }
            
        }

    }
    public void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = true;
        }
        characterModel.SetActive(true);
        //eyes.gameObject.SetActive(false);
        for (int i = 0; i < eyes.childCount; i++)
        {
            eyes.GetChild(i).gameObject.SetActive(false);
        }
        
    }
}
