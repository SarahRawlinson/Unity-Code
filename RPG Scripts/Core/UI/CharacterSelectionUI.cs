using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.UI;
using RPG.Movement;
using System.Linq;

namespace RPG.Core.UI
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        [SerializeField] Dropdown genderDropdown;
        [SerializeField] Dropdown speciesDropdown;
        [SerializeField] Dropdown ageDropdown;
        private CharacterModelMaker maker;
        private List<ModelAttributes.Models> curentOptionsModels = new List<ModelAttributes.Models>();
        private List<ModelAttributes.ActiveModel> activeModels = new List<ModelAttributes.ActiveModel>();
        [SerializeField] Text selectedBodyPart;
        private int activeModelInt = 0;
        [SerializeField] Text selectedModel;
        //private int selectedModelInt = 0;
        [SerializeField] Text selectedMaterial;
        private int selectedMaterialInt = 0;

        // Start is called before the first frame update
        void Start()
        {
            maker = FindObjectOfType<CharacterModelMaker>();
            DropdownAllToAny();
            SetUpDropDowns(maker.Species);
            ActivateRandomModel();            
        }

        public void NextObject(int a)
        {
            //Debug.Log("Button Pressed");
            activeModels = maker.NextModelObject(activeModelInt, a);
            SetSelectedText(activeModelInt);
        }

        public void NextMaterial(int a)
        {
            //Debug.Log("Button Pressed");
            List<Material> materials = activeModels[activeModelInt].materials.GetMaterials();
            if ((selectedMaterialInt + a) < 0)
            {
                //Debug.Log("Material 0");
                selectedMaterialInt = materials.Count - 1;
            }
            else if(materials.Count <= (selectedMaterialInt + a))
            {
                selectedMaterialInt = 0;
            }
            else
            {
                //Debug.Log("Material " + (selectedMaterialInt + a));
                selectedMaterialInt = selectedMaterialInt + a;
            }

            activeModels = maker.NextMaterial(activeModelInt, selectedMaterialInt);
            SetSelectedText(activeModelInt);
        }
        public void NextModel(int a)
        {
            //Debug.Log("Button Pressed");
            if (activeModels.Count <= (activeModelInt + a))
            {
                //Debug.Log("Model 0");
                activeModelInt = 0;
            }
            else if ((activeModelInt + a) < 0)
            {
                activeModelInt = activeModels.Count - 1;
            }
            else
            {
                //Debug.Log("Model " + (activeModelInt + a));
                activeModelInt = activeModelInt + a;
            }
            
            //Debug.Log($"next model in {activeModelInt}");
            SetSelectedText(activeModelInt);
        }

        private void SetSelectedText(int ModelInt)
        {
            //Debug.Log($"Active model int {activeModels.Count()}, total Models {activeModels.Count()}");
            ModelAttributes.ActiveModel model = activeModels[ModelInt];
            selectedBodyPart.text = model.models.GetBodyPart().ToString();
            try
            {
                if (model.gObject != null) selectedModel.text = model.gObject.name;
            }
            catch
            {
                selectedModel.text = "None";
            }
            try
            {
                if (model.material != null) selectedMaterial.text = model.material.name;
            }
            catch
            {
                selectedMaterial.text = "None";
            }
        }        

        public void ActivateRandomDance()
        {
            maker.transform.GetComponent<CharacterEditMovements>().ActivateRandomDance();
        }

        public void StopDance()
        {
            maker.transform.GetComponent<CharacterEditMovements>().StopDance();
        }

        public void ActivateRandomModel()
        {
            DropdownAllToAny();
            ActivateSelected();
        }

        private ModelAttributes.Species WorkOutSpeciesFromDropDown()
        {
            ModelAttributes.Species speciesType;
            speciesType = maker.RandomSpecies();
            if (speciesDropdown.captionText.text != "Any")
            {
                foreach (ModelAttributes.Species type in maker.Species)
                {
                    if (type.GetSpecies() == speciesDropdown.captionText.text)
                    {
                        speciesType = type;
                        break;
                    }
                }
            }
            return speciesType;
        }

        public void ActivateSelected()
        {
            maker.DeactivateAll();
            List<ModelAttributes.Models> modelsList = GetModelAttributes();
            curentOptionsModels = modelsList;
            ModelAttributes.Materials[] materials = maker.Materials;
            activeModels = maker.TurnOnRandom(modelsList, materials);
            genderDropdown.captionText.text = maker.GenderString;
            ageDropdown.captionText.text = maker.AgeString;
            speciesDropdown.captionText.text = maker.SpeciesString;
            SetSelectedText(activeModelInt);
        }

        private void DropdownAllToAny()
        {
            genderDropdown.captionText.text = "Any";
            ageDropdown.captionText.text = "Any";
            speciesDropdown.captionText.text = "Any";
        }

        private void SetUpDropDowns(ModelAttributes.Species[] species)
        {
            List<string> speciesList = new List<string>();
            List<string> AgeList = new List<string>();
            List<string> genderList = new List<string>();
            string checkString;
            foreach (ModelAttributes.Species sType in species)
            {
                checkString = sType.GetSpecies();
                if (!speciesList.Contains(checkString)) speciesList.Add(checkString);
                foreach (ModelAttributes.Ages age in sType.GetAges())
                {
                    checkString = age.GetAge().ToString();
                    if (!AgeList.Contains(checkString)) AgeList.Add(checkString);
                    foreach (ModelAttributes.Gender sex in age.GetGenders())
                    {
                        checkString = sex.GetGender().ToString();
                        if (!genderList.Contains(checkString)) genderList.Add(checkString);
                    }
                }
            }
            AddToDropDown(genderList, genderDropdown);
            AddToDropDown(AgeList, ageDropdown);
            AddToDropDown(speciesList, speciesDropdown);
        }

        private static void AddToDropDown(List<string> list, Dropdown dropdown)
        {
            //Clear the old options of the Dropdown menu
            dropdown.ClearOptions();
            Dropdown.OptionData startMessage = new Dropdown.OptionData();
            startMessage.text = "Any";
            dropdown.options.Add(startMessage);
            dropdown.captionText.text = "Any";
            foreach (string s in list)
            {
                Dropdown.OptionData message = new Dropdown.OptionData();
                message.text = s;
                //Debug.Log(s);
                dropdown.options.Add(message);
            }
        }

        public List<ModelAttributes.Models> GetModelAttributes()
        {
            ModelAttributes.Species speciesType = WorkOutSpeciesFromDropDown();
            maker.SpeciesString = speciesType.GetSpecies();
            ModelAttributes.Ages AgeType = WorkOutAgeFromDropDown(speciesType);
            maker.AgeString = AgeType.GetAge().ToString();
            ModelAttributes.Gender gender = WorkOutGenderFromDropDown(AgeType);
            maker.GenderString = gender.GetGender().ToString();
            List<ModelAttributes.Models> modelsList = maker.GetModels(speciesType.GetSpecies(), AgeType.GetAge(), gender.GetGender(), CharacterModelMaker.AllBodyPartsList());
            return modelsList;
        }

        private ModelAttributes.Ages WorkOutAgeFromDropDown(ModelAttributes.Species species)
        {
            ModelAttributes.Ages AgeType;
            AgeType = CharacterModelMaker.GetRandomAge(species);
            if (ageDropdown.captionText.text != "Any")
            {
                foreach (ModelAttributes.Ages age in species.GetAges())
                {
                    if (age.GetAge().ToString() == ageDropdown.captionText.text)
                    {
                        AgeType = age;
                        break;
                    }
                }
            }
            return AgeType;
        }

        private ModelAttributes.Gender WorkOutGenderFromDropDown(ModelAttributes.Ages ages)
        {
            ModelAttributes.Gender genderType;
            genderType = CharacterModelMaker.GetRandomGender(ages);
            if (genderDropdown.captionText.text != "Any")
            {
                foreach (ModelAttributes.Gender gender in ages.GetGenders())
                {
                    if (gender.GetGender().ToString() == genderDropdown.captionText.text)
                    {
                        genderType = gender;
                        break;
                    }
                }
            }
            return genderType;
        }
    }
}
