using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyboardShortcuts : MonoBehaviour
{
    [Serializable]
    private class Shortcut
    {
        public Key key;
        public Button button;
    }

    [SerializeField] private Shortcut[] shortcuts;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        foreach (Shortcut shortcut in shortcuts)
        {
            if (!keyboard[shortcut.key].wasPressedThisFrame)
                continue;

            if (shortcut.button.interactable && shortcut.button.gameObject.activeInHierarchy)
                shortcut.button.onClick.Invoke();
        }
    }
}
