using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public EventPage eventPage;
    
    public TMPro.TextMeshProUGUI packageId;
    public TMPro.TextMeshProUGUI transactionModule;
    public TMPro.TextMeshProUGUI sender;
    public TMPro.TextMeshProUGUI type;

    public void InitializeObject(EventPage eventPage)
    {
        
        this.eventPage = eventPage;
        packageId.text = eventPage.packageId;
        transactionModule.text = eventPage.transactionModule;
        type.text = eventPage.type;
    }

}
