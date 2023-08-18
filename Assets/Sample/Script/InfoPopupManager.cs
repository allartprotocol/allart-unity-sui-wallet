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

    public Sprite warningSprite;
    public Sprite errorSprite;
    public Sprite infoSprite;

    public List<NotificationPopup> notifQueue = new List<NotificationPopup>();
    public Transform underlay;

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

        if (notifQueue.Count > 0 && notifQueue[notifQueue.Count - 1].titleText.text == message)
        {
            return;
        }

        GameObject notif = Instantiate(notifPrefab, contentHolder);

        switch (type)
        {
            case InfoType.Info:
                notif.GetComponent<NotificationPopup>().SetInfo(infoColor, message, infoSprite);
                break;
            case InfoType.Warning:
                notif.GetComponent<NotificationPopup>().SetInfo(warningColor, message, warningSprite);
                break;
            case InfoType.Error:
                notif.GetComponent<NotificationPopup>().SetInfo(errorColor, message, errorSprite);
                break;
            default:
                break;
        }

        notifQueue.Add(notif.GetComponent<NotificationPopup>());

        if(notifQueue.Count > 3)
        {
            Destroy(notifQueue[0].gameObject);
            notifQueue.Remove(notifQueue[0]);
        }
        underlay.gameObject.SetActive(true);
    }

    public void ClearNotif()
    {
        foreach (var notif in notifQueue)
        {
            Destroy(notif.gameObject);
        }
        notifQueue.Clear();
    }

    public void RemoveNotif(NotificationPopup notif)
    {
        notifQueue.Remove(notif);
        if(notifQueue.Count == 0)
        {
            underlay.gameObject.SetActive(false);
        }
    }
}
