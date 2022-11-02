using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaneponPanel : MonoBehaviour
{
    //メインのMeshRenderer
    [SerializeField] private MeshRenderer _meshRenderer = null;

    public MeshRenderer meshRenderer { get { return _meshRenderer; } set { _meshRenderer = value; } }

    //パネルの状態
    private PaneponSystem.PanelState _state = PaneponSystem.PanelState.Stable;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
