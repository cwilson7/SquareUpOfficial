using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;

public class CloudSaveHandler : MonoBehaviour
{
    [SerializeField] static string progressSystemKey;
    static string stringValueOnCloud;

    // Start is called before the first frame update
    void Awake()
    {
        NPBinding.CloudServices.Initialise();
    }

    private void OnEnable()
    {
        CloudServices.KeyValueStoreDidInitialiseEvent += OnKeyValueStoreInitialised;
    }

    private void OnDisable()
    {
        CloudServices.KeyValueStoreDidInitialiseEvent -= OnKeyValueStoreInitialised;
    }

    private void OnKeyValueStoreInitialised(bool _success)
    {
        if (_success)
        {
            // Get String
            stringValueOnCloud = NPBinding.CloudServices.GetString(progressSystemKey);
        }
    }

    public static string PlayerInformation()
    {
        return stringValueOnCloud;
    }

    public static void SaveStringProgressSystem(string info)
    {
        NPBinding.CloudServices.SetString(progressSystemKey, info);
    }
}
