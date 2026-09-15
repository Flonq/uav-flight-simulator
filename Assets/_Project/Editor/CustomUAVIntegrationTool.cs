using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MertKaan.UAVSimulator.Aircraft;
using MertKaan.UAVSimulator.InputSystem;
using MertKaan.UAVSimulator.UI.Debugging;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MertKaan.UAVSimulator.Editor
{
    internal static class CustomUAVIntegrationTool
    {
        private const string MenuPath = "Tools/UAV Simulator/Custom UAV/Build or Update Visual Prefab";

        private const string ModelPath =
            "Assets/_Project/Art/Aircraft/CustomUAV/UAV_Custom.fbx";

        private const string BaseColorPath =
            "Assets/_Project/Art/Aircraft/CustomUAV/Textures/UAV_BaseColor.png";

        private const string MetallicSmoothnessPath =
            "Assets/_Project/Art/Aircraft/CustomUAV/Textures/UAV_MetallicSmoothness.png";

        private const string NormalPath =
            "Assets/_Project/Art/Aircraft/CustomUAV/Textures/UAV_Normal.png";

        private const string MaterialPath =
            "Assets/_Project/Art/Materials/M_CustomUAV_URP.mat";

        private const string PrefabPath =
            "Assets/_Project/Prefabs/Aircraft/PF_CustomUAVVisual.prefab";

        private const string PhysicsTemplatePath =
            "Assets/_Project/Prefabs/Aircraft/PF_AircraftPrototype.prefab";

        private const string AircraftPrefabPath =
            "Assets/_Project/Prefabs/Aircraft/PF_CustomUAVAircraftPrototype.prefab";

        private const string FlightTestScenePath =
            "Assets/_Project/Scenes/FlightTest.unity";

        [MenuItem(MenuPath)]
        private static void BuildOrUpdate()
        {
            GameObject visualPivot = null;

            try
            {
                ConfigureTexture(BaseColorPath, TextureImporterType.Default, true);
                ConfigureTexture(MetallicSmoothnessPath, TextureImporterType.Default, false);
                ConfigureTexture(NormalPath, TextureImporterType.NormalMap, false);
                ConfigureModel();

                Material material = BuildOrUpdateMaterial();
                GameObject modelAsset = RequireAsset<GameObject>(ModelPath);
                GameObject modelInstance = PrefabUtility.InstantiatePrefab(modelAsset) as GameObject;

                if (modelInstance == null)
                {
                    throw new InvalidOperationException($"Model prefab could not be instantiated: {ModelPath}");
                }

                visualPivot = new GameObject("VisualPivot");
                modelInstance.name = "UAV_Custom";
                modelInstance.transform.SetParent(visualPivot.transform, false);
                ResetLocalTransform(modelInstance.transform);
                AssignMaterial(modelInstance, material);

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(visualPivot, PrefabPath, out bool success);
                if (!success || prefab == null)
                {
                    throw new InvalidOperationException($"Visual prefab could not be saved: {PrefabPath}");
                }

                AssetDatabase.SaveAssets();
                LogValidation(visualPivot);
                Selection.activeObject = prefab;
                EditorGUIUtility.PingObject(prefab);

                Debug.Log(
                    $"Custom UAV integration assets are ready. Material: {MaterialPath} | Prefab: {PrefabPath}");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Custom UAV Integration Failed",
                    exception.Message,
                    "Close");
            }
            finally
            {
                if (visualPivot != null)
                {
                    UnityEngine.Object.DestroyImmediate(visualPivot);
                }
            }
        }

        [MenuItem("Tools/UAV Simulator/Custom UAV/Build or Update Aircraft Prototype")]
        private static void BuildOrUpdateAircraftPrototype()
        {
            GameObject aircraftRoot = null;

            try
            {
                GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AircraftPrefabPath);
                if (existingPrefab != null)
                {
                    ValidateExistingAircraftPrototype(existingPrefab);
                    LogAircraftPrototypeValidation(existingPrefab);
                    Selection.activeObject = existingPrefab;
                    EditorGUIUtility.PingObject(existingPrefab);

                    Debug.Log(
                        $"Custom UAV aircraft prototype already exists and was preserved: {AircraftPrefabPath}. " +
                        "Rebuilding was skipped so manually tuned colliders and added components remain unchanged.");
                    return;
                }

                GameObject visualPrefab = RequireAsset<GameObject>(PrefabPath);
                GameObject physicsTemplate = RequireAsset<GameObject>(PhysicsTemplatePath);
                Rigidbody templateRigidbody = physicsTemplate.GetComponent<Rigidbody>();

                if (templateRigidbody == null)
                {
                    throw new InvalidOperationException(
                        $"Physics template has no Rigidbody: {PhysicsTemplatePath}");
                }

                aircraftRoot = new GameObject("AircraftRoot");
                Rigidbody rigidbody = aircraftRoot.AddComponent<Rigidbody>();
                EditorUtility.CopySerialized(templateRigidbody, rigidbody);

                GameObject visualPivot = PrefabUtility.InstantiatePrefab(visualPrefab) as GameObject;
                if (visualPivot == null)
                {
                    throw new InvalidOperationException($"Visual prefab could not be instantiated: {PrefabPath}");
                }

                visualPivot.name = "VisualPivot";
                visualPivot.transform.SetParent(aircraftRoot.transform, false);
                ResetLocalTransform(visualPivot.transform);

                CreatePrototypeColliders(aircraftRoot, visualPivot);
                ConfigureRuntimeComponents(aircraftRoot);

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
                    aircraftRoot,
                    AircraftPrefabPath,
                    out bool success);

                if (!success || prefab == null)
                {
                    throw new InvalidOperationException(
                        $"Aircraft prototype could not be saved: {AircraftPrefabPath}");
                }

                AssetDatabase.SaveAssets();
                LogAircraftPrototypeValidation(aircraftRoot);
                Selection.activeObject = prefab;
                EditorGUIUtility.PingObject(prefab);

                Debug.Log(
                    $"Custom UAV aircraft prototype is ready: {AircraftPrefabPath}. " +
                    "The existing aircraft prefab and scenes were not modified.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Custom UAV Aircraft Prototype Failed",
                    exception.Message,
                    "Close");
            }
            finally
            {
                if (aircraftRoot != null)
                {
                    UnityEngine.Object.DestroyImmediate(aircraftRoot);
                }
            }
        }

        private static void ValidateExistingAircraftPrototype(GameObject aircraftRoot)
        {
            Rigidbody rootRigidbody = aircraftRoot.GetComponent<Rigidbody>();
            if (rootRigidbody == null)
            {
                throw new InvalidOperationException(
                    $"Existing custom aircraft prototype has no root Rigidbody: {AircraftPrefabPath}");
            }

            Rigidbody[] rigidbodies = aircraftRoot.GetComponentsInChildren<Rigidbody>(true);
            if (rigidbodies.Length != 1 || rigidbodies[0] != rootRigidbody)
            {
                throw new InvalidOperationException(
                    "Existing custom aircraft prototype must have exactly one Rigidbody on its root.");
            }

            Transform visualPivot = aircraftRoot.transform.Find("VisualPivot");
            if (visualPivot == null || visualPivot.parent != aircraftRoot.transform)
            {
                throw new InvalidOperationException(
                    "Existing custom aircraft prototype must contain a direct VisualPivot child.");
            }

            Collider[] colliders = aircraftRoot.GetComponentsInChildren<Collider>(true);
            if (colliders.Length == 0)
            {
                throw new InvalidOperationException(
                    "Existing custom aircraft prototype has no colliders to preserve.");
            }

            ValidateRuntimeComponents(aircraftRoot);
        }

        private static void ConfigureRuntimeComponents(GameObject aircraftRoot)
        {
            AircraftInputReader input = aircraftRoot.AddComponent<AircraftInputReader>();
            AircraftControlSurfaceAnimator animator = aircraftRoot.AddComponent<AircraftControlSurfaceAnimator>();
            SerializedObject serialized = new SerializedObject(animator);
            serialized.FindProperty("_inputReader").objectReferenceValue = input;
            string[] fields = { "_leftAileron", "_rightAileron", "_leftRuddervator", "_rightRuddervator" };
            string[] names = { "Aileron_Left", "Aileron_Right", "Ruddervator_Left", "Ruddervator_Right" };
            for (int i = 0; i < fields.Length; i++)
            {
                serialized.FindProperty(fields[i]).objectReferenceValue =
                    FindDescendant(aircraftRoot.transform.Find("VisualPivot"), names[i]);
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            ValidateRuntimeComponents(aircraftRoot);
        }

        private static void ValidateRuntimeComponents(GameObject aircraftRoot)
        {
            AircraftInputReader input = aircraftRoot.GetComponent<AircraftInputReader>();
            AircraftControlSurfaceAnimator animator = aircraftRoot.GetComponent<AircraftControlSurfaceAnimator>();
            if (input == null || animator == null ||
                aircraftRoot.GetComponentsInChildren<AircraftInputReader>(true).Length != 1 ||
                aircraftRoot.GetComponentsInChildren<AircraftControlSurfaceAnimator>(true).Length != 1)
            {
                throw new InvalidOperationException("Custom aircraft prefab requires one root Input Reader and Animator.");
            }

            SerializedObject serialized = new SerializedObject(animator);
            if (serialized.FindProperty("_inputReader").objectReferenceValue != input)
            {
                throw new InvalidOperationException("Animator must reference its prefab's root Input Reader.");
            }
            Transform visualPivot = aircraftRoot.transform.Find("VisualPivot");
            string[] fields = { "_leftAileron", "_rightAileron", "_leftRuddervator", "_rightRuddervator" };
            string[] names = { "Aileron_Left", "Aileron_Right", "Ruddervator_Left", "Ruddervator_Right" };
            for (int i = 0; i < fields.Length; i++)
            {
                Transform surface = serialized.FindProperty(fields[i]).objectReferenceValue as Transform;
                if (surface == null || visualPivot == null || !surface.IsChildOf(visualPivot) || surface.name != names[i])
                {
                    throw new InvalidOperationException($"Animator has an invalid prefab reference: {fields[i]}");
                }
            }
        }

        [MenuItem("Tools/UAV Simulator/Custom UAV/Stage Aircraft in Clean FlightTest")]
        private static void StageAircraftInFlightTest()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.path != FlightTestScenePath)
            {
                EditorUtility.DisplayDialog(
                    "Open FlightTest",
                    $"Open {FlightTestScenePath} before staging the custom aircraft.",
                    "Close");
                return;
            }

            if (scene.isDirty)
            {
                EditorUtility.DisplayDialog(
                    "FlightTest Has Unsaved Changes",
                    "Undo, save, or discard the current scene changes before staging. " +
                    "This guard prevents test placement from mixing with unrelated work.",
                    "Close");
                return;
            }

            GameObject physicsTemplate = RequireAsset<GameObject>(PhysicsTemplatePath);
            GameObject customAircraftPrefab = RequireAsset<GameObject>(AircraftPrefabPath);
            ValidateExistingAircraftPrototype(customAircraftPrefab);
            GameObject[] candidates = scene.GetRootGameObjects()
                .Where(root =>
                    root.activeSelf &&
                    PrefabUtility.GetCorrespondingObjectFromSource(root) == physicsTemplate)
                .ToArray();

            if (candidates.Length != 1)
            {
                EditorUtility.DisplayDialog(
                    "Active AircraftRoot Is Ambiguous",
                    $"Expected one active {PhysicsTemplatePath} instance, found {candidates.Length}. " +
                    "No scene changes were made.",
                    "Close");
                return;
            }

            GameObject oldAircraft = candidates[0];
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Stage Custom UAV Aircraft");

            GameObject customAircraft = PrefabUtility.InstantiatePrefab(customAircraftPrefab, scene) as GameObject;
            if (customAircraft == null)
            {
                throw new InvalidOperationException(
                    $"Custom aircraft could not be instantiated: {AircraftPrefabPath}");
            }

            Undo.RegisterCreatedObjectUndo(customAircraft, "Create Custom UAV Aircraft");
            Undo.RegisterCompleteObjectUndo(oldAircraft, "Disable Meshy Aircraft Backup");

            oldAircraft.name = "AircraftRoot_Meshy_Backup";
            oldAircraft.SetActive(false);

            customAircraft.name = "AircraftRoot";
            customAircraft.transform.SetPositionAndRotation(
                new Vector3(oldAircraft.transform.position.x, 0f, oldAircraft.transform.position.z),
                oldAircraft.transform.rotation);
            customAircraft.transform.localScale = Vector3.one;

            AircraftInputReader oldInput = oldAircraft.GetComponent<AircraftInputReader>();
            AircraftInputReader newInput = customAircraft.GetComponent<AircraftInputReader>();
            foreach (InputDebugPanel panel in scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<InputDebugPanel>(true)))
            {
                SerializedObject serializedPanel = new SerializedObject(panel);
                SerializedProperty inputReference = serializedPanel.FindProperty("_inputReader");
                if (oldInput != null && inputReference.objectReferenceValue == oldInput)
                {
                    Undo.RecordObject(panel, "Rebind Aircraft Input Debug Panel");
                    inputReference.objectReferenceValue = newInput;
                    serializedPanel.ApplyModifiedProperties();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(panel);
                }
            }

            AlignLowestVisualPointToGround(customAircraft, 0f);
            Undo.CollapseUndoOperations(undoGroup);

            Selection.activeGameObject = customAircraft;
            SceneView.lastActiveSceneView?.FrameSelected();
            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log(
                "Custom UAV staged in FlightTest without saving the scene. " +
                "The former active aircraft is inactive as AircraftRoot_Meshy_Backup. " +
                "Use Ctrl+Z to revert the entire staging operation.");
        }

        private static void ConfigureModel()
        {
            ModelImporter importer = AssetImporter.GetAtPath(ModelPath) as ModelImporter;
            if (importer == null)
            {
                throw new FileNotFoundException("Custom UAV FBX importer was not found.", ModelPath);
            }

            importer.globalScale = 1f;
            importer.useFileScale = true;
            importer.bakeAxisConversion = true;
            importer.importAnimation = false;
            importer.animationType = ModelImporterAnimationType.None;
            importer.importNormals = ModelImporterNormals.Import;
            importer.importTangents = ModelImporterTangents.CalculateMikk;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.preserveHierarchy = true;
            importer.SaveAndReimport();
        }

        private static void ConfigureTexture(
            string path,
            TextureImporterType textureType,
            bool sRgb)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new FileNotFoundException("Custom UAV texture importer was not found.", path);
            }

            importer.textureType = textureType;
            importer.sRGBTexture = sRgb;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = false;
            importer.maxTextureSize = 4096;
            importer.mipmapEnabled = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.anisoLevel = 8;
            importer.SaveAndReimport();
        }

        private static Material BuildOrUpdateMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new InvalidOperationException("Universal Render Pipeline/Lit shader was not found.");
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                material = new Material(shader)
                {
                    name = "M_CustomUAV_URP",
                    enableInstancing = true
                };
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            else
            {
                material.shader = shader;
                material.enableInstancing = true;
            }

            Texture2D baseColor = RequireAsset<Texture2D>(BaseColorPath);
            Texture2D metallicSmoothness = RequireAsset<Texture2D>(MetallicSmoothnessPath);
            Texture2D normal = RequireAsset<Texture2D>(NormalPath);

            material.SetColor("_BaseColor", Color.white);
            material.SetTexture("_BaseMap", baseColor);
            material.SetTexture("_MetallicGlossMap", metallicSmoothness);
            material.SetFloat("_Metallic", 1f);
            material.SetFloat("_Smoothness", 1f);
            material.SetFloat("_SmoothnessTextureChannel", 0f);
            material.EnableKeyword("_METALLICSPECGLOSSMAP");
            material.SetTexture("_BumpMap", normal);
            material.SetFloat("_BumpScale", 1f);
            material.EnableKeyword("_NORMALMAP");

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void AssignMaterial(GameObject root, Material material)
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                Material[] slots = new Material[renderer.sharedMaterials.Length];
                Array.Fill(slots, material);
                renderer.sharedMaterials = slots;
            }
        }

        private static void CreatePrototypeColliders(GameObject aircraftRoot, GameObject visualPivot)
        {
            Bounds fuselage = GetRendererBoundsInRootSpace(
                RequireRenderer(visualPivot, "Fuselage"),
                aircraftRoot.transform);
            Bounds wing = GetRendererBoundsInRootSpace(
                RequireRenderer(visualPivot, "Wing_Main"),
                aircraftRoot.transform);
            Bounds mainTires = GetRendererBoundsInRootSpace(
                RequireRenderer(visualPivot, "MainGear_Tire_Pair"),
                aircraftRoot.transform);
            Bounds noseTire = GetRendererBoundsInRootSpace(
                RequireRenderer(visualPivot, "NoseGear_Tire"),
                aircraftRoot.transform);

            CreateBodyCollider(aircraftRoot.transform, fuselage);
            CreateWingColliders(aircraftRoot.transform, wing);
            CreateMainWheelColliders(aircraftRoot.transform, mainTires);
            CreateWheelCollider(
                aircraftRoot.transform,
                "NoseWheelCollider",
                noseTire.center,
                Mathf.Min(noseTire.size.y, noseTire.size.z) * 0.5f);
        }

        private static void CreateBodyCollider(Transform parent, Bounds bounds)
        {
            GameObject colliderObject = new GameObject("BodyCollider");
            colliderObject.transform.SetParent(parent, false);
            CapsuleCollider collider = colliderObject.AddComponent<CapsuleCollider>();
            collider.direction = 2;
            collider.center = bounds.center;
            collider.radius = Mathf.Max(bounds.size.x, bounds.size.y) * 0.5f;
            collider.height = Mathf.Max(bounds.size.z, collider.radius * 2f);
        }

        private static void CreateWingColliders(Transform parent, Bounds bounds)
        {
            float halfWidth = bounds.size.x * 0.5f;
            Vector3 colliderSize = new Vector3(
                halfWidth,
                Mathf.Max(bounds.size.y, 0.12f),
                bounds.size.z);

            CreateBoxCollider(
                parent,
                "LeftWingCollider",
                new Vector3(bounds.center.x - halfWidth * 0.5f, bounds.center.y, bounds.center.z),
                colliderSize);
            CreateBoxCollider(
                parent,
                "RightWingCollider",
                new Vector3(bounds.center.x + halfWidth * 0.5f, bounds.center.y, bounds.center.z),
                colliderSize);
        }

        private static void CreateMainWheelColliders(Transform parent, Bounds bounds)
        {
            float radius = Mathf.Min(bounds.size.y, bounds.size.z) * 0.5f;
            float centerOffset = Mathf.Max(0f, bounds.extents.x - radius);

            CreateWheelCollider(
                parent,
                "LeftMainWheelCollider",
                new Vector3(bounds.center.x - centerOffset, bounds.center.y, bounds.center.z),
                radius);
            CreateWheelCollider(
                parent,
                "RightMainWheelCollider",
                new Vector3(bounds.center.x + centerOffset, bounds.center.y, bounds.center.z),
                radius);
        }

        private static void CreateBoxCollider(
            Transform parent,
            string name,
            Vector3 center,
            Vector3 size)
        {
            GameObject colliderObject = new GameObject(name);
            colliderObject.transform.SetParent(parent, false);
            BoxCollider collider = colliderObject.AddComponent<BoxCollider>();
            collider.center = center;
            collider.size = size;
        }

        private static void CreateWheelCollider(
            Transform parent,
            string name,
            Vector3 center,
            float radius)
        {
            GameObject colliderObject = new GameObject(name);
            colliderObject.transform.SetParent(parent, false);
            SphereCollider collider = colliderObject.AddComponent<SphereCollider>();
            collider.center = center;
            collider.radius = radius;
        }

        private static Renderer RequireRenderer(GameObject root, string name)
        {
            Renderer renderer = root.GetComponentsInChildren<Renderer>(true)
                .SingleOrDefault(candidate => candidate.name == name);

            if (renderer == null)
            {
                throw new InvalidOperationException($"Required renderer was not found: {name}");
            }

            return renderer;
        }

        private static Bounds GetRendererBoundsInRootSpace(Renderer renderer, Transform root)
        {
            Bounds worldBounds = renderer.bounds;
            Vector3 min = worldBounds.min;
            Vector3 max = worldBounds.max;
            Vector3[] corners =
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, max.y, max.z)
            };

            Bounds localBounds = new Bounds(root.InverseTransformPoint(corners[0]), Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
            {
                localBounds.Encapsulate(root.InverseTransformPoint(corners[i]));
            }

            return localBounds;
        }

        private static void AlignLowestVisualPointToGround(GameObject aircraftRoot, float groundY)
        {
            Renderer[] renderers = aircraftRoot.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = CalculateBounds(renderers);
            aircraftRoot.transform.position += Vector3.up * (groundY - bounds.min.y);
        }

        private static void LogAircraftPrototypeValidation(GameObject aircraftRoot)
        {
            Transform visualPivot = aircraftRoot.transform.Find("VisualPivot");
            Rigidbody rigidbody = aircraftRoot.GetComponent<Rigidbody>();
            Collider[] colliders = aircraftRoot.GetComponentsInChildren<Collider>(true);
            Renderer[] renderers = aircraftRoot.GetComponentsInChildren<Renderer>(true);
            Bounds bounds = CalculateBounds(renderers);

            Debug.Log(
                $"Custom UAV aircraft validation | Direct VisualPivot: {visualPivot != null} | " +
                $"Rigidbody: {rigidbody != null} | Colliders: {colliders.Length} | " +
                $"Visual center: {bounds.center} | Visual size: {bounds.size}");
        }

        private static void LogValidation(GameObject visualPivot)
        {
            Renderer[] renderers = visualPivot.GetComponentsInChildren<Renderer>(true);
            MeshFilter[] filters = visualPivot.GetComponentsInChildren<MeshFilter>(true);
            var meshes = new HashSet<Mesh>();
            long triangleCount = 0;

            foreach (MeshFilter filter in filters)
            {
                if (filter.sharedMesh == null || !meshes.Add(filter.sharedMesh))
                {
                    continue;
                }

                for (int subMesh = 0; subMesh < filter.sharedMesh.subMeshCount; subMesh++)
                {
                    triangleCount += (long)filter.sharedMesh.GetIndexCount(subMesh) / 3L;
                }
            }

            Transform rotorPivot = FindDescendant(visualPivot.transform, "Rotor_Pivot");
            Bounds bounds = CalculateBounds(renderers);

            Debug.Log(
                $"Custom UAV validation | Renderers: {renderers.Length} | Unique meshes: {meshes.Count} | " +
                $"Triangles: {triangleCount:N0} | Bounds center: {bounds.center} | Bounds size: {bounds.size} | " +
                $"Rotor_Pivot: {(rotorPivot != null ? "found" : "MISSING")}");
        }

        private static Bounds CalculateBounds(IReadOnlyList<Renderer> renderers)
        {
            if (renderers.Count == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Count; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private static Transform FindDescendant(Transform root, string name)
        {
            foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
            {
                if (candidate.name == name)
                {
                    return candidate;
                }
            }

            return null;
        }

        private static T RequireAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new FileNotFoundException($"Required asset was not found: {path}", path);
            }

            return asset;
        }

        private static void ResetLocalTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}
