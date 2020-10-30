using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class UA11YSonarMenuExtension : MonoBehaviour
{
    [MenuItem("GameObject/KAP/UA11YSonar/UA11YSonarManager", false, 10)]
    static void CreateUA11YSonarManager(MenuCommand menuCommand)
    {
        GameObject managerGameObject = Resources.Load<GameObject>("Prefabs/Sonar/UA11YSonarManager");
        managerGameObject = Instantiate<GameObject>(managerGameObject);
        managerGameObject.name = "UA11YSonarManager";

        GameObjectUtility.SetParentAndAlign(managerGameObject, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(managerGameObject, "Create " + managerGameObject.name);
        Selection.activeObject = managerGameObject;
    }
}

#endif