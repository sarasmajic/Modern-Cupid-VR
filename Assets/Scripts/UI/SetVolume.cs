using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolume : MonoBehaviour
{
    public void ChangeVolume(float sliderValue) {
        AudioListener.volume = sliderValue;
    }
}
