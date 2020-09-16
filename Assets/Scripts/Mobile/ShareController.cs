using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class ShareController : MonoBehaviour
{
    public string SMS_BODY_MESSAGE;
    //public string[] ARRAY_OF_RECIPIENTS;

    // Start is called before the first frame update
    public void ShareViaSMS()
    {
        if (!NPBinding.Sharing.IsMessagingServiceAvailable())
        {
            Debug.Log("Messaging service not available on this device.");
            return;
        }

        MessageShareComposer _composer = new MessageShareComposer();
        _composer.Body = SMS_BODY_MESSAGE;
        //_composer.ToRecipients = ARRAY_OF_RECIPIENTS;

        NPBinding.Sharing.ShowView(_composer, FinishedSharing);
    }

    void FinishedSharing(eShareResult _result)
    {
        Debug.Log("Finished sharing");
        Debug.Log("Share Result = " + _result);
    }
}
