using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class NameArgs : Args
{
    public string name;
}

public class EnterName : MonoBehaviour
{
    #region inspector
    TMP_InputField _nameField = null;

    Button _confirmButton = null;
    #endregion

    #region public members

    public const string PLAYERPREFS_NAME_KEY = "PLAYERPREFS_NAME_KEY";

    public UnityAction<Args> OnNameEntered;

    #endregion

    #region private methods

    void OnEnable()
    {
        string prefName = PlayerPrefs.GetString(PLAYERPREFS_NAME_KEY, null);
        if (!string.IsNullOrEmpty(prefName))
        {
            _nameField.text = prefName;
        }
    }

    #endregion

    #region public methods

    public void OnNameFieldUpdated(string name)
    {
        _confirmButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void Confirm()
    {
        string name = _nameField.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        PlayerPrefs.SetString(PLAYERPREFS_NAME_KEY, name);
        EventsManager.TriggerEvent(nameof(OnNameEntered), new NameArgs() {
            name = name
        });
    }

    #endregion
}