using UnityEngine;

public class EncryptedWWWForm
{
    public readonly WWWForm form = new();

    public void AddField(string key, string value)
    {
        if (SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY.Trim() != string.Empty && SensitiveInfo.SERVER_SEND_TRANSFER_KEY.Trim() != string.Empty)
            form.AddField(SensitiveInfo.Encrypt(key, SensitiveInfo.SERVER_SEND_TRANSFER_KEY), SensitiveInfo.Encrypt(value, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
    }
}