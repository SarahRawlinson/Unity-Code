using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class ActiveModel : MonoBehaviour
    {
        [SerializeField] bool CompletelyRandom = false;
        [SerializeField] string species = "Human";
        [SerializeField] ModelAttributes.Ages.Age age = ModelAttributes.Ages.Age.Adult;
        private CharacterModelMaker maker;
        private List<ModelAttributes.ActiveModel> activeModels = new List<ModelAttributes.ActiveModel>();
        void Start()
        {
            //Debug.Log($"{transform.parent.gameObject.name} run random models");
            maker = transform.GetComponent<CharacterModelMaker>();
            maker.DeactivateAll();
            if (CompletelyRandom) maker.TurnOnRandom();
            else maker.TurnOnRandomAdultHuman(age, species);
        }
        

    }
}
