using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class ModelDetails : MonoBehaviour
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
        public enum ClothingTypeTags
        {
            Accessary,
            Adventure,
            Armor,
            Augmented,
            Ballerina,
            Beard,
            BeathingAssistance,
            Belt,
            BodyPart,
            Cardboard,
            Casual,
            Cheerleader,
            Cloths,
            Container,
            Cowboy,
            Crew,
            Crown,
            Cryo,
            Cyber,
            Demon,
            Doctor,
            Dress,
            Eastern,
            Elf,
            Exercises,
            Explorer,
            EyeBrows,
            Eyes,
            EyeWear,
            Face,
            FaceHair,
            Fantasy,
            Farmer,
            Female,
            Footballer,
            Garbage,
            Geisha,
            Ghost,
            Gloves,
            Hair,
            Hat,
            HeadDress,
            HeadHair,
            Hijab,
            Holiday,
            Hoodie,
            Hunter,
            Jeans,
            Jewellry,
            Jungle,
            Junker,
            Karate,
            Knight,
            Magician,
            Maid,
            Male,
            Mask,
            Medical,
            Monk,
            Mouth,
            Mummy,
            Muscle,
            Mustash,
            Necklace,
            Ninja,
            Normal,
            Onesie,
            Pants,
            Peasant,
            Pirate,
            PlaidShirt,
            PlanePilot,
            Police,
            Poor,
            Prince,
            PufferVest,
            Punk,
            Pyjamas,
            Raincoat,
            Rich,
            Ring,
            Robber,
            Robot,
            Samurai,
            Scarf,
            School,
            SciFi,
            Scout,
            ShipCaptin,
            ShipCrew,
            Shoe,
            Shorts,
            SideBurns,
            Skater,
            Skirt,
            Smart,
            SnowJacket,
            Spacesuit,
            Suit,
            Summer,
            Superhero,
            Survivor,
            Sweater,
            Swimwear,
            Tail,
            Tiara,
            Tracksuit,
            Trucker,
            Tshirt,
            Underwear,
            Uniform,
            Viking,
            VR,
            Warrior,
            Watch,
            Wetsuit,
            Winter,
            Witch,
            Wizard,
        }
        public enum BodySize
        {
            Any,
            Small,
            Large,
        }
        [SerializeField] bool attachment;
        [SerializeField] BodyPart bodyPart;
        [SerializeField] bool isSpecial;
        [SerializeField] bool hasHair;
        [SerializeField] bool hasSkin;
        [SerializeField] bool isFacePlate;
        [SerializeField] Material[] usableMaterials;
        [SerializeField] BodyPart[] bodyPartConflicts;
        [SerializeField] ClothingTypeTags[] tags;

        public bool IsAttachment()
        {
            return attachment;
        }
        public BodyPart GetBodyPart()
        {
            return bodyPart;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
