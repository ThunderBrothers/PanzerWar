﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class Utility_CreateDefaultVehicleAssets : EditorWindow {
    string vehicleToCreateStr = "";

    [MenuItem("Tools/Create Default VehicleAssets")]
    static void Init(){
        EditorWindow.GetWindow(typeof(Utility_CreateDefaultVehicleAssets));
    }
    void OnGUI() {
        vehicleToCreateStr = GUILayout.TextField(vehicleToCreateStr);

        if(GUILayout.Button("Create" + vehicleToCreateStr))
        {
            CreateAssets(vehicleToCreateStr);
        }

    }
    void CreateAssets(string vehicleName){
        string rootPath = "Assets/Res/Vehicles/Ground/Data/Vehicle/{0}";

        if (!Directory.Exists(string.Format(rootPath, vehicleName))) {
            Directory.CreateDirectory(string.Format(rootPath, vehicleName));
        }
        else{
            return;
        }

        rootPath += "/{0}_{1}.asset";

        GameObject wheelCollider = new GameObject(string.Format("{0}_WheelCollider", vehicleName));

        WheelCollider wC = wheelCollider.AddComponent<WheelCollider>();
        wheelCollider.AddComponent(typeof(Rigidbody)); // Visual Debug

        GameObject wheelColliderPrefab = PrefabUtility.CreatePrefab(string.Format("Assets/Res/Vehicles/Ground/Data/Vehicle/{0}/{1}.prefab", vehicleName, wheelCollider.name),wheelCollider);

        DestroyImmediate(wheelCollider);

        VehicleHitBox vehilceHitBox = new ScriptableObjectClassManager<VehicleHitBox>().Create(string.Format(rootPath, vehicleName, "VehicleHitBox"));

        VehicleTextData  vehicleTextData = new ScriptableObjectClassManager<VehicleTextData>().Create(string.Format(rootPath, vehicleName, "VehicleTextData"));

        VehicleData vehicleData = new ScriptableObjectClassManager<VehicleData>().Create(string.Format(rootPath, vehicleName, "VehicleData"));

        VehicleEngineSoundData vehicleEngineSoundData = new ScriptableObjectClassManager<VehicleEngineSoundData>().Create(string.Format(rootPath, vehicleName, "VehicleEngineSoundData"));


        vehicleData.vehicleTextData = vehicleTextData;

        vehicleData.modelData.HitBox = vehilceHitBox;

        vehicleTextData.AssetName = vehicleName;

        vehicleTextData.PTCParameter.TankWheelCollider = wheelColliderPrefab;
        vehicleTextData.PTCParameter.vehicleEngineSoundData = vehicleEngineSoundData;
    }
}

public class ScriptableObjectClassManager<T> where T:ScriptableObject
{
    public T Create(string path){
        T asset = ScriptableObject.CreateInstance<T>();

        Debug.Log(asset.GetType());

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

        return asset;
    }
}