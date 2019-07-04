using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class KAPSonarMenuExtension : MonoBehaviour
{
    [MenuItem("GameObject/KAP/KAPSonar/KAPSonarManager", false, 10)]
    static void CreateKAPSonarManager(MenuCommand menuCommand)
    {
        GameObject managerGameObject = Resources.Load<GameObject>("Prefabs/Sonar/KAPSonarManager");
        managerGameObject = Instantiate<GameObject>(managerGameObject);
        managerGameObject.name = "KAPSonarManager";

        GameObjectUtility.SetParentAndAlign(managerGameObject, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(managerGameObject, "Create " + managerGameObject.name);
        Selection.activeObject = managerGameObject;
    }
}

#endif