using UnityEditor;
using UnityEngine;

public class SOHandler : MonoBehaviour
{
    // file at the end of the path must have correct extension. For example if we are Instantiating
    // (or CreateInstancing for SOs) animation, that extension is .anim 
    // below directory must be created before create SO instance
    string cardAssetPath = "Assets/SO/Cards/";

    void Start()
    {
        SOInstance();
    }

    // Step 1 - Create or reload the assets that store each Card object.
    private void SOInstance()
    {
        // Card magentaCard = new Card(); => This method of creating SO instance is wrong!
        // 'ScriptableObject.CreateInstance' method instead of 'new Card()'
        /*
        Card magentaCard;
        cardAssetPath = "Assets/SO/Cards/MagentaCard.asset";
        magentaCard = AssetDatabase.LoadAssetAtPath<Card>(cardAssetPath);
        if (magentaCard == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            magentaCard = ScriptableObject.CreateInstance<Card>();
            magentaCard.cardColor = Color.magenta;
            AssetDatabase.CreateAsset(magentaCard, cardAssetPath);
        }

        Card greenCard;
        cardAssetPath = "Assets/SO/Cards/GreenCard.asset";
        greenCard = AssetDatabase.LoadAssetAtPath<Card>(cardAssetPath);
        if (greenCard == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            greenCard = ScriptableObject.CreateInstance<Card>();
            greenCard.cardColor = Color.green;
            AssetDatabase.CreateAsset(greenCard, cardAssetPath);
        }

        Card blueCard;
        cardAssetPath = "Assets/SO/Cards/BlueCard.asset";
        blueCard = AssetDatabase.LoadAssetAtPath<Card>(cardAssetPath);
        if (blueCard == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            blueCard = ScriptableObject.CreateInstance<Card>();
            blueCard.cardColor = Color.blue;
            AssetDatabase.CreateAsset(blueCard, cardAssetPath);
        }

        Card CyanCard;
        cardAssetPath = "Assets/SO/Cards/CyanCard.asset";
        CyanCard = AssetDatabase.LoadAssetAtPath<Card>(cardAssetPath);
        if (CyanCard == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            CyanCard = ScriptableObject.CreateInstance<Card>();
            CyanCard.cardColor = Color.cyan;
            AssetDatabase.CreateAsset(CyanCard, cardAssetPath);
        }
        */

        Card cardFields;
        cardAssetPath = "Assets/SO/Cards/CardFields.asset";
        cardFields = AssetDatabase.LoadAssetAtPath<Card>(cardAssetPath);
        if (cardFields == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            cardFields = ScriptableObject.CreateInstance<Card>();
            cardFields.cardColors.Add(Color.magenta);
            cardFields.cardColors.Add(Color.green);
            cardFields.cardColors.Add(Color.blue);
            cardFields.cardColors.Add(Color.cyan);
            AssetDatabase.CreateAsset(cardFields, cardAssetPath);
        }
    }

    // Step 2 - Create some example cards in the current scene...
}
