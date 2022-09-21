using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneBackButton : MonoBehaviour
{
    private Button button;
    [HideInInspector] public bool isDisabled;

    [SerializeField] private PhoneBackButton disableWhileActive;

    private void Start() {
        button = GetComponent<Button>();
    }

    private void OnEnable() {
        if(disableWhileActive != null){
            disableWhileActive.isDisabled = true;
        }
    }

    private void OnDisable() {
        if(disableWhileActive != null){
            disableWhileActive.isDisabled = false;           
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDisabled && Input.GetKeyDown(KeyCode.Escape)) {
            button.onClick.Invoke();
        }
    }
}
