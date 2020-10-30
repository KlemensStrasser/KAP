using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class UA11YUIMenuExtension : MonoBehaviour
{
    [MenuItem("GameObject/KAP/KAPUI/KAPUIManager", false, 10)]
    static void CreateKAPUIManager(MenuCommand menuCommand)
    {
        GameObject managerGameObject = Resources.Load<GameObject>("Prefabs/UI/UA11YUIManager");
        managerGameObject = Instantiate<GameObject>(managerGameObject);
        managerGameObject.name = "KAPUIManager";

        GameObjectUtility.SetParentAndAlign(managerGameObject, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(managerGameObject, "Create " + managerGameObject.name);
        Selection.activeObject = managerGameObject;
    }
}

#endif