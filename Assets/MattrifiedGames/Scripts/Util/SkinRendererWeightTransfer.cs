using UnityEngine;

namespace MattrifiedGames.Utility
{
    [ExecuteInEditMode(), RequireComponent(typeof(SkinnedMeshRenderer))]
    public class SkinRendererWeightTransfer : MonoBehaviour
    {
        [SerializeField()]
        bool initialized = false;

        [SerializeField()]
        SkinnedMeshRenderer smr;

        public Transform newSkeletonRoot;

        [SerializeField()]
        string originalRoot = "";

        [SerializeField()]
        string[] originalBoneNames = null;

        void Awake()
        {
            initialized = false;
        }

        public void Update()
        {
            if (!initialized)
            {
                if (!smr)
                {
                    smr = GetComponent<SkinnedMeshRenderer>();
                    smr.enabled = false;
                    if (!smr)
                        return;
                }

                if (originalBoneNames == null || originalBoneNames.Length == 0)
                {
                    originalRoot = smr.rootBone.name;
                    originalBoneNames = new string[smr.bones.Length];
                    for (int i = 0; i < smr.bones.Length; i++)
                        originalBoneNames[i] = smr.bones[i].name;
                }

                if (newSkeletonRoot == null)
                    return;

                // Reassign bones
                if (newSkeletonRoot)
                {
                    Transform newRoot;
                    if (!StaticHelpers.FindChildByName(newSkeletonRoot.root, originalRoot, out newRoot))
                    {
                        Debug.LogError("No bone found for:  " + originalRoot);
                        return;
                    }

                        Transform[] newBones = new Transform[originalBoneNames.Length];
                    for (int i = 0; i < originalBoneNames.Length; i++)
                    {
                        if (!StaticHelpers.FindChildByName(newSkeletonRoot.root, originalBoneNames[i], out newBones[i]))
                        {
                            Debug.LogError("No bone found for:  " + originalBoneNames[i]);
                            return;
                        }
                    }

                    smr.bones = newBones;
                    smr.rootBone = newRoot;

                    initialized = true;
                    enabled = false;
                    
                }
            }
        }

        public static Transform TransferBone(Transform oBoneOld, Transform oBoneNewRoot)
        {      // Returns a transform at the same relative path of a bone (provided by SkinnedMeshRenderer.bones[i]) but rooted at 'oBoneNewRoot'.  Assumes an identically-structured bone tree between the objects (Running this function over all bones of an object  will make it move along the object at 'oBoneNewRoot')
            string sBonePath = oBoneOld.name;
            Transform oNodeIterator = oBoneOld.parent;
            while (oNodeIterator.parent != null)
            {                                  // Iterate up the parent chain to construct the relative path all the way to just before the top-level object
                sBonePath = oNodeIterator.name + "/" + sBonePath;
                oNodeIterator = oNodeIterator.parent;
            }
            Transform oBoneNew = oBoneNewRoot.Find(sBonePath);
            if (oBoneNew == null)
                Debug.LogError("*Err: TransferBone could not transfer bone '" + sBonePath + "' to new root '" + oBoneNewRoot + "'");
            return oBoneNew;
        }

        public static void TransferBones(ref SkinnedMeshRenderer oSkinMeshRend, Transform oBoneNewRoot)
        {
            Transform[] aBones = oSkinMeshRend.bones;
            for (int nBone = 0; nBone < oSkinMeshRend.bones.Length; nBone++)                                // Iterate through all bones of this skinned mesh to remap them to our body's bones.  (This will make clothing skinned mesh 'move along' with body)
                aBones[nBone] = TransferBone(aBones[nBone], oBoneNewRoot);
            oSkinMeshRend.bones = aBones;
            oSkinMeshRend.rootBone = TransferBone(oSkinMeshRend.rootBone, oBoneNewRoot);    // Also remap the root bone similarly
        }
    }
}
