using UnityEditor;
using UnityEngine;

public class SOHandler : MonoBehaviour
{
    // file at the end of the path must have correct extension. For example if we are Instantiating
    // (or CreateInstancing for SOs) animation, that extension is .anim 
    // below directory must be created before create SO instance
    private string _cardAssetPath = "Assets/SO/Cards/";

    private void Start()
    {
        SOInstance();
    }

    // Step 1 - Create or reload the assets that store each Card object.
    private void SOInstance()
    {
        // Card magentaCard = new Card(); => This method of creating SO instance is wrong!
        // 'ScriptableObject.CreateInstance' method instead of 'new Card()'

        // DIKKAT! AssetDatabase buildde cagrilamiyor

        Card _cardFields;
        _cardAssetPath = "Assets/SO/Cards/CardFields.asset";
        _cardFields = AssetDatabase.LoadAssetAtPath<Card>(_cardAssetPath);
        if (_cardFields == null)
        {
            // Create and save ScriptableObject because it doesn't exist yet
            _cardFields = ScriptableObject.CreateInstance<Card>();
            _cardFields.CardColors.Add(Color.magenta);
            _cardFields.CardColors.Add(Color.green);
            _cardFields.CardColors.Add(Color.blue);
            _cardFields.CardColors.Add(Color.cyan);
            AssetDatabase.CreateAsset(_cardFields, _cardAssetPath);
        }

        /*
        LevelData cardData;
        cardAssetPath = "Assets/SO/Cards/CardData.asset";
        cardData = AssetDatabase.LoadAssetAtPath<LevelData>(cardAssetPath);
        if(cardData == null)
        {
            cardData = ScriptableObject.CreateInstance<LevelData>();

            AssetDatabase.CreateAsset(cardData, cardAssetPath);
        }
        */
    }

    // Step 2 - Create some example cards in the current scene...
}
