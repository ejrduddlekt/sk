using UnityEngine;
using System;

public enum SelectionStage { None, Wafer, StackMapLayer, Chip, NoInkMap }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isDragModeActive = false;

    public event Action<SelectionStage> OnStageChanged;

    private SelectionStage _currentStage = SelectionStage.None;
    public SelectionStage CurrentStage
    {
        get => _currentStage;
        set
        {
            if (_currentStage == value) return;
            _currentStage = value;
            OnStageChanged?.Invoke(_currentStage);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        _currentStage = SelectionStage.StackMapLayer;
    }
}
