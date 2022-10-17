using System;
using TMPro;
using UnityEngine;

namespace UI
{
  [RequireComponent(typeof(TMP_Text))]
  public class VersionDisplay : MonoBehaviour
  {
    private void Awake()
    {
      var text = GetComponent<TMP_Text>();
      text.text = Application.version;
    }
  }
}
