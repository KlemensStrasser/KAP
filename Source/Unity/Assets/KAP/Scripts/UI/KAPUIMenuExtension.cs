using UnityEditor;
using UnityEngine;
public class KAPUIMenuExtension : MonoBehaviour
{

    [MenuItem("GameObject/KAP/KAPUI/KAPUIManager", false, 10)]
    static void CreateKAPUIManager(MenuCommand menuCommand)
    {
        GameObject managerGameObject = Resources.Load<GameObject>("Prefabs/UI/KAPUIManager");
        managerGameObject = Instantiate<GameObject>(managerGameObject);
        managerGameObject.name = "KAPUIManager";

        GameObjectUtility.SetParentAndAlign(managerGameObject, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(managerGameObject, "Create " + managerGameObject.name);
        Selection.activeObject = managerGameObject;
    }
}
