using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPopupManager : MonoBehaviour
{

    public enum InfoType
    {
        Info,
        Warning,
        Error
    }

    public Transform contentHolder;
    public GameObject notifPrefab;

    public static InfoPopupManager instance;

    public Color warningColor;
    public Color errorColor;
    public Color infoColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddNotif(InfoType type, string message)
    {
        GameObject notif = Instantiate(notifPrefab, contentHolder);

        switch (type)
        {
            case InfoType.Info:
                notif.GetComponent<NotificationPopup>().SetInfo(infoColor, message);
                break;
            case InfoType.Warning:
                notif.GetComponent<NotificationPopup>().SetInfo(warningColor, message);
                break;
            case InfoType.Error:
                notif.GetComponent<NotificationPopup>().SetInfo(errorColor, message);
                break;
            default:
                break;
        }
    }
}
