using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameSceneGasStation : LevelGameScene
{
    // Override scene type for the gas station
    public override Type SceneType => Type.GasStation;
    
    // Additional gas station specific functionality can be added here
    
    protected new void Start()
    {
        // Call base implementation for common level setup
        base.Start();
        
        // Add any gas station specific initialization here
        Debug.Log("Gas Station scene initialized");
    }
}