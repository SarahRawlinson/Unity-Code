using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Attributes
{
    public class ModelAttributes
    {
        [System.Serializable]
        public class Models
        {
            public enum BodyPart
            {
                WholeBody,
                Head,
                Hair,
                Face,
                FacePlate,
                FaceRight,
                FaceLeft,
                EyeBrows,
                Eyes,
                EarRight,
                EarLeft,
                Nose,
                Cheak,
                Freckles,
                Mouth,
                Beard,
                Mustache,
                BeardMustach,
                Chin,
                Goatee,
                NeckDown,
                Neck,
                Chest,
                ArmRight,
                ArmLeft,
                HandRight,
                HandLeft,
                HipRight,
                HipLeft,
                Butt,
                LegRight,
                LegLeft,
                FootRight,
                FootLeft,
            }
            [SerializeField] bool attachment;
            [SerializeField] BodyPart bodyPart;
            [SerializeField] GameObject[] Model;
            public bool IsAttachment()
            {
                return attachment;
            }
            public BodyPart GetBodyPart()
            {
                return bodyPart;
            }
            public GameObject[] GetModelGameObjects()
            {
                return Model;
            }
        };
        [System.Serializable]
        public class Gender
        {
            public enum Sex { Any, Female, Male };
            [SerializeField] Sex sex;
            [SerializeField] Models[] models;
            public Sex GetGender()
            {
                return sex;
            }
            public Models[] GetModels()
            {
                return models;
            }
        };
        [System.Serializable]
        public class Ages
        {
            public enum Age { Adult, Kid };
            [SerializeField] Age age;
            [SerializeField] Gender[] sex;
            public Age GetAge()
            {
                return age;
            }
            public Gender[] GetGenders()
            {
                return sex;
            }
        };
        [System.Serializable]
        public class Species
        {
            [SerializeField] string species;
            [SerializeField] Ages[] age;
            public string GetSpecies()
            {
                return species;
            }
            public Ages[] GetAges()
            {
                return age;
            }
        };
        [System.Serializable]
        public class Materials
        {
            [SerializeField] List<Material> materials;
            public string name;
            [SerializeField] Models.BodyPart[] bodyparts;
            public List<Material> GetMaterials()
            {
                return materials;
            }
            public Models.BodyPart[] GetBodyParts()
            {

                return bodyparts;
            }
            public bool PartInBodyPart(Models.BodyPart part)
            {
                return bodyparts.Contains(part);
            }
            public bool PartsInBodyPart(Models.BodyPart[] parts)
            {
                bool partsIn = false;
                foreach (Models.BodyPart part in parts)
                {
                    if (PartInBodyPart(part)) partsIn = true;
                }
                return partsIn;
            }
        };
        [System.Serializable]
        public class ActiveModel
        {
            public GameObject gObject;
            public Models models;
            public int modelInt;
            public Material material;
            public int materialInt;
            public Materials materials;
            public List<Material> materialsList;
            public GameObject[] gameObjects;
            public bool hasMaterial;
            public ActiveModel(GameObject modelObject, Models modelCollection, 
                Material activeMaterial, Materials materialCollection, 
                int modelListInt, int materialListInt, List<Material> listMaterials,
                GameObject[] gObjects, bool gameObjectHasMaterial)
            {
                gObject = modelObject;
                models = modelCollection;
                material = activeMaterial;
                materials = materialCollection;
                modelInt = modelListInt;
                materialInt = materialListInt;
                materialsList = listMaterials;
                gameObjects = gObjects;
                hasMaterial = gameObjectHasMaterial;
            }
        }
    }
}
