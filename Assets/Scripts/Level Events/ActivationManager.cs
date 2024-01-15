using System.Collections.Generic;
using UnityEngine;

namespace Level_Events
{
    [System.Serializable]
    public class ObjectMapping
    {
        public ActivationManager.ObjectIdentifier identifier;
        public GameObject gameObject;
    }

    public class ActivationManager : MonoBehaviour
    {
        public enum ObjectIdentifier
        {
            LabHallway,
            LabTpTriggerObject,
            BotanicTpTriggerObject,
            DarkHallway,
            SaveRoom,
            BotanicHallway,
            Containment,
            CoreActivation,
            SecondFloor,
            SecondFloorWalls,
            SecondFloorPath,
            ContainmentWall,
            PowerCoreWall,
            PowerCorePath,
            DownstairWalls,
            PowerCoreShortcut,
            PathBehindStatue,
            UpstairBlock,
        }
        
        public static ActivationManager Instance { get; private set; }
        
        [SerializeField]
        private List<ObjectMapping> objectsToManage = new List<ObjectMapping>();
        private Dictionary<ObjectIdentifier, GameObject> objectDict = new Dictionary<ObjectIdentifier, GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDictionary();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDictionary()
        {
            foreach (ObjectMapping mapping in objectsToManage)
            {
                objectDict[mapping.identifier] = mapping.gameObject;
            }
        }

        public void ActivateObject(ObjectIdentifier identifier)
        {
            if (objectDict.TryGetValue(identifier, out GameObject obj))
            {
                obj.SetActive(true);
            }
        }

        public void DeactivateObject(ObjectIdentifier identifier)
        {
            if (objectDict.TryGetValue(identifier, out GameObject obj))
            {
                obj.SetActive(false);
            }
        }
    }
}