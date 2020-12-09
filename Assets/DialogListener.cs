using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DialogListener
{
    void onSuccess();

    void onCancel();
}
