using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    public List<SectionManager> sections;
    public CameraFollow cameraFollow;
    private int _currentIndex = 0;

    private void Start() { 
        StartSection(_currentIndex);
    }

    public void StartSection(int sectionIndex) {
        sections[sectionIndex].Initialize(this);
    }

    public void MoveToNextSection() {
        _currentIndex++;
        if (_currentIndex >= sections.Count) {
            // 클리어 이후 처리 추가
        }
        else {
            StartSection(_currentIndex);
        }
    }
}
