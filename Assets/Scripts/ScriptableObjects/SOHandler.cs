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

        Card cardFields;
        cardAssetPath = "Assets/SO/Cards/CardFields.asset";
        cardFields = AssetDatabase.LoadAssetAtPath<Card>(cardAssetPath);
        if (cardFields == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            cardFields = ScriptableObject.CreateInstance<Card>();
            cardFields.CardColors.Add(Color.magenta);
            cardFields.CardColors.Add(Color.green);
            cardFields.CardColors.Add(Color.blue);
            cardFields.CardColors.Add(Color.cyan);
            AssetDatabase.CreateAsset(cardFields, cardAssetPath);
        }

        LevelData cardData;
        cardAssetPath = "Assets/SO/Cards/CardData.asset";
        cardData = AssetDatabase.LoadAssetAtPath<LevelData>(cardAssetPath);
        if(cardData == null)
        {
            cardData = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(cardData, cardAssetPath);
        }
    }

    // Step 2 - Create some example cards in the current scene...
}
