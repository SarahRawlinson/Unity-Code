using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Saving;



namespace RPG.Attributes
{
    public class CharacterModelMaker : MonoBehaviour, ISaveable
    {
        [SerializeField] Vector3 scaleChangeKid, positionChangeKid;
        [SerializeField] Vector3 scaleChangeAdult, positionChangeAdult;
        [SerializeField] GameObject[] normalScaleGameObjects;

        [SerializeField] GameObject nullObject;
        [SerializeField] ModelAttributes.Species[] species;
        [SerializeField] ModelAttributes.Materials[] materials;
        
        
        private List<ModelAttributes.ActiveModel> activeModels = new List<ModelAttributes.ActiveModel>();

        private string ageString;
        private string genderString;
        private string speciesString;
        private enum saveDataVar { Models, Materials}

        public ModelAttributes.Species[] Species { get => species; }
        public ModelAttributes.Materials[] Materials { get => materials; }
        public string AgeString { get => ageString; set => ageString = value; }
        public string GenderString { get => genderString; set => genderString = value; }
        public string SpeciesString { get => speciesString; set => speciesString = value; }
        

        public void DeactivateAll()
        {
            foreach (Transform pos in transform)
            {
                try
                {
                    //if (typeof(SkinnedMeshRenderer).IsAssignableFrom(pos.gameObject.GetType())) 
                    if (pos.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        pos.gameObject.SetActive(false);
                    }
                    //else Debug.Log($"{pos.gameObject.name} does'nt have SkinnedMeshRenderer");
                }
                catch (Exception e)
                {
                    Debug.Log($"issue checking for skinned mesh renderer {e.Source}");
                }
            }
        }

        public ModelAttributes.Species FindSpecies(string speciesString)
        {
            foreach (ModelAttributes.Species speciesItem in species)
            {
                if (speciesItem.GetSpecies() == speciesString)
                {
                    return speciesItem;
                }
            }
            return RandomSpecies();
        }

        public ModelAttributes.Species RandomSpecies()
        {
            return species[UnityEngine.Random.Range((int)0, species.Length)];
        }

        public ModelAttributes.Ages FindAge(ModelAttributes.Species speciesType, ModelAttributes.Ages.Age desiredAge)
        {
            ModelAttributes.Ages[] ages = speciesType.GetAges();
            foreach (ModelAttributes.Ages age in ages)
            {
                if (age.GetAge() == desiredAge)
                {
                    return age;
                }
            }
            return GetRandomAge(speciesType);
        }

        public static ModelAttributes.Gender GetRandomGender(ModelAttributes.Ages AgeType)
        {
            ModelAttributes.Gender[] sexs = AgeType.GetGenders();
            ModelAttributes.Gender sexType = sexs[UnityEngine.Random.Range((int)0, sexs.Length)];
            return sexType;
        }

        public static ModelAttributes.Ages GetRandomAge(ModelAttributes.Species speciesType)
        {
            ModelAttributes.Ages[] ages = speciesType.GetAges();
            ModelAttributes.Ages AgeType = ages[UnityEngine.Random.Range((int)0, ages.Length)];
            return AgeType;
        }

        public static List<ModelAttributes.Models.BodyPart> AllBodyPartsList()
        {
            List<ModelAttributes.Models.BodyPart> bodyParts = new List<ModelAttributes.Models.BodyPart>();
            foreach (int i in System.Enum.GetValues(typeof(ModelAttributes.Models.BodyPart))) bodyParts.Add((ModelAttributes.Models.BodyPart)i);
            return bodyParts;
        }
            
        

        

        public List<ModelAttributes.Models> GetModels(string speciesType, ModelAttributes.Ages.Age ageType, ModelAttributes.Gender.Sex sexType, List<ModelAttributes.Models.BodyPart> bodyParts)
        {
            List<ModelAttributes.Models> modelsList = new List<ModelAttributes.Models>();
            foreach (ModelAttributes.Species type in species)
            {
                if (type.GetSpecies() != speciesType) continue;
                List<ModelAttributes.Ages> ages = GetAgeFromSpecies(ageType, type);
                if (ageType == ModelAttributes.Ages.Age.Kid)
                {
                    transform.localScale = scaleChangeKid;                    
                    foreach (GameObject g in normalScaleGameObjects)
                    {
                        g.transform.localScale = g.transform.localScale * (scaleChangeAdult.x / scaleChangeKid.x);
                    }
                }
                else
                {
                    transform.localScale = scaleChangeAdult;
                }
                
                foreach (ModelAttributes.Ages age in ages)
                {
                    List<ModelAttributes.Gender> genders = CheckSexInAge(sexType, age);
                    foreach (ModelAttributes.Gender gender in genders) modelsList.AddRange(GetModelsFromGender(gender, bodyParts));
                }
            }
            return modelsList;
        }

        public List<ModelAttributes.ActiveModel> NextModelObject(int models, int nextModelInt)
        {
            ModelAttributes.ActiveModel modelList = activeModels[models];
            modelList.gObject.SetActive(false);
            ModelAttributes.Models model = modelList.models;
            if ((modelList.modelInt + nextModelInt) < GetGameObjectsFromModels(model).Length & (modelList.modelInt + nextModelInt) >= 0)
            {
                modelList.modelInt = modelList.modelInt + nextModelInt;                
            }
            else
            {
                modelList.modelInt = 0;
            }
            modelList.gObject = GetGameObjectsFromModels(model)[modelList.modelInt];
            int materialInt = 0;
            FindMaterial(ref materialInt, modelList.materials.GetMaterials(), modelList.gObject);
            modelList.materialInt = materialInt;
            modelList.gObject.SetActive(true);
            return activeModels;
        }

        public List<ModelAttributes.ActiveModel> NextMaterial(int models, int materialInt)
        {
            ModelAttributes.ActiveModel modelList = activeModels[models];
            modelList.materialInt = materialInt;
            modelList.material = modelList.materials.GetMaterials()[materialInt];
            SetMaterial(materialInt, modelList.materials.GetMaterials(), modelList.gObject);

            return activeModels;
        }

        public List<ModelAttributes.ActiveModel> TurnOnRandomAdultHuman(ModelAttributes.Ages.Age ageType, string speciesType)
        {
            ModelAttributes.Species type = FindSpecies(speciesType);
            ModelAttributes.Ages age = FindAge(type, ageType);
            return TurnOnRandom(GetModels(speciesType, ageType, GetRandomGender(age).GetGender(), AllBodyPartsList()), materials);
        }

        public List<ModelAttributes.ActiveModel> TurnOnRandom()
        {
            ModelAttributes.Species type = RandomSpecies();
            ModelAttributes.Ages age = GetRandomAge(type);
            return TurnOnRandom(GetModels(type.GetSpecies(), age.GetAge(), GetRandomGender(age).GetGender(), AllBodyPartsList()), materials);
        }

        public List<ModelAttributes.ActiveModel> TurnOnRandom(List<ModelAttributes.Models> modelsList, ModelAttributes.Materials[] materials, int gObjectint = -1, int materialInt = -1)
        {
            
            List<ModelAttributes.ActiveModel> modelList = new List<ModelAttributes.ActiveModel>();
            foreach (ModelAttributes.Models models in modelsList)
            {
                List<Material> materialList = new List<Material>();
                bool hasMaterial;
                ModelAttributes.Materials materialColection;
                hasMaterial = false;
                materialColection = new ModelAttributes.Materials();
                foreach (ModelAttributes.Materials materialsPart in materials)
                {

                    if (materialsPart.PartInBodyPart(models.GetBodyPart()))
                    {
                        materialList.AddRange(materialsPart.GetMaterials());
                        hasMaterial = true;
                        materialColection = materialsPart;

                    }

                }
                //if (!addBodyPart)
                //{
                //    modelList.Add(new ModelAttributes.ActiveModel(null, models, null, materialColection, -1, -1));
                //    continue;
                //}
                GameObject[] gObjects = GetGameObjectsFromModels(models);
                GameObject gObject = null;
                System.Random rng = new System.Random();
                if (models.IsAttachment() && rng.Next(-1, 10) > 0)
                {
                    gObjectint = gObjects.Length - 1;
                }
                //Debug.Log($"Gameobject past int {gObjectint}");
                if (gObjectint <= -1 ^ gObjectint >= gObjects.Length)
                {
                    //Debug.Log($"Gameobject Random Range 0 - {gObjects.Length}");
                    gObjectint = UnityEngine.Random.Range((int)0, gObjects.Length);
                }
                //Debug.Log($"Gameobject int {gObjectint}");
                Material material = SetGameObjectAndMaterial(gObjectint, ref materialInt, materialList, hasMaterial, gObjects, ref gObject);

                modelList.Add(new ModelAttributes.ActiveModel(gObject, models, material, materialColection, 
                    gObjectint, materialInt, materialList, gObjects, hasMaterial));
            }
            activeModels = modelList;
            return modelList;
        }

        private static Material SetGameObjectAndMaterial(int gObjectint, ref int materialInt, List<Material> materialList, bool hasMaterial, GameObject[] gObjects, ref GameObject gObject)
        {
            Material material = null;
            try
            {
                gObject = gObjects[gObjectint];
                gObject.SetActive(true);
                material = gObject.GetComponent<Material>();
                if (hasMaterial)
                {
                    material = FindMaterial(ref materialInt, materialList, gObject);

                }
            }
            catch (Exception e)
            {
                Debug.Log(e);

            }

            return material;
        }

        private GameObject[] GetGameObjectsFromModels(ModelAttributes.Models models)
        {
            GameObject[] gObjects = models.GetModelGameObjects();
            if (models.IsAttachment())
            {
                //Debug.Log(gObjects.Length);
                GameObject[] gameObjects = new GameObject[gObjects.Length + 1];
                for (int i = 0; i < gObjects.Length; i++) gameObjects[i] = gObjects[i];
                try
                {
                    gameObjects[gObjects.Length] = nullObject;
                    //Debug.Log("NullObject Added");
                }
                catch
                {
                    Debug.Log($"{gObjects.Length} too big for array size: {gameObjects.Length}");
                }
                gObjects = gameObjects;
                //Debug.Log(gObjects.Length);
            }
            return gObjects;
        }

        private static Material FindMaterial(ref int materialInt, List<Material> materialList, GameObject gObject)
        {
            Material material;
            if (materialInt <= -1 ^ materialInt >= materialList.Count)
            {
                materialInt = UnityEngine.Random.Range((int)0, materialList.Count);
            }
            material = SetMaterial(materialInt, materialList, gObject);
            return material;
        }

        public static Material SetMaterial(int materialInt, List<Material> materialList, GameObject gObject)
        {
            Material material = materialList[materialInt];
            gObject.GetComponent<Renderer>().material = material;
            return material;
        }

        //private static void GetMaterialCollection(ModelAttributes.Materials[] materials, ModelAttributes.Models models, List<Material> materialList, out bool hasMaterial, out ModelAttributes.Materials materialColection)
        //{
            
        //}

        public static List<ModelAttributes.Ages> GetAgeFromSpecies(ModelAttributes.Ages.Age ageType, ModelAttributes.Species type)
        {
            List<ModelAttributes.Ages> ages = new List<ModelAttributes.Ages>();
            foreach (ModelAttributes.Ages age in type.GetAges())
            {
                if (age.GetAge() != ageType) continue;
                ages.Add(age);
            }
            return ages;
        }

        public static List<ModelAttributes.Gender> CheckSexInAge(ModelAttributes.Gender.Sex sexType, ModelAttributes.Ages age)
        {
            List<ModelAttributes.Gender> genders = new List<ModelAttributes.Gender>();
            foreach (ModelAttributes.Gender gender in age.GetGenders())
            {
                if (gender.GetGender() != sexType) continue;
                genders.Add(gender);
            }
            return genders;
        }

        public static List<ModelAttributes.Models> GetModelsFromGender(ModelAttributes.Gender sex, List<ModelAttributes.Models.BodyPart> bodyParts)
        {
            List<ModelAttributes.Models> modelList = new List<ModelAttributes.Models>();
            foreach (ModelAttributes.Models models in sex.GetModels())
            {
                if (CheckBodyPart(bodyParts, models))
                {
                    modelList.Add(models);
                }
            }
            return modelList;
        }

        public static bool CheckBodyPart(List<ModelAttributes.Models.BodyPart> bodyParts, ModelAttributes.Models models)
        {
            bool checkBodyPart = false;
            foreach (ModelAttributes.Models.BodyPart part in bodyParts)
            {
                if (part == models.GetBodyPart()) checkBodyPart = true;
            }
            return checkBodyPart;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public object CaptureState()
        {

            //Dictionary<saveDataVar, object> data = new Dictionary<saveDataVar, object>
            //{
            //    [saveDataVar.KillXP] = XP,
            //    [saveDataVar.KillCount] = killCount,
            //    [saveDataVar.NextLevel] = nextLevelXP
            //};
            return null;
        }

        public void RestoreState(object state)
        {
            //loaded = false;
            //Dictionary<saveDataVar, object> data = (Dictionary<saveDataVar, object>)state;
            //XP = (float)data[saveDataVar.KillXP];
            //killCount = (long)data[saveDataVar.KillCount];
            //nextLevelXP = (float)data[saveDataVar.NextLevel];
            ////while (IfLevelUp())
            ////{
            ////}
            //loaded = true;
        }
    }
}
