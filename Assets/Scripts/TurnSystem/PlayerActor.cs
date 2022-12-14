using UnityEngine;
using TurnSystem;
using System.Collections;
using CardNameSpace;

[DisallowMultipleComponent]
public class PlayerActor : MonoBehaviour, ITurnActor
{
    public GameObject Actor { get; set; }
    public ActorState ActorState { get; set; }
    [SerializeField] private TilemapReader tilemapReader;
    private Dice<int>[] dices = new Dice<int>[2];

    private Navigation navigation;
    [SerializeField] private HighlightTile highlightTilePrefab;
    private HighlightTile[] highlightTiles;

    [SerializeField] private DeckHandler deckHandler;

    public IEnumerator ActionCoroutine()
    {
        ActorState = ActorState.Start;

        yield return CardAction();

        //yield return MoveAction();

        ActorState = ActorState.End;
    }

    private IEnumerator CardAction()
    {
        // hand card show
        deckHandler.Show();

        // card click and use
        yield return deckHandler.WaitForClickCard();

        // card effect and animation
        Debug.Log("Card using....!");
        //yield return new WaitForSeconds(2);
        // end
        deckHandler.Hide();
        yield return null;
    }

    private IEnumerator MoveAction()
    {
        var centerPoint = tilemapReader.ChangeWorldToLocalPosition(transform.position);
        Vector3Int[] directions = new Vector3Int[4] { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down };

        for (int i = 0; i < highlightTiles.Length; i++)
        {
            var nearbyCoordinate = centerPoint + directions[i];
            if (tilemapReader.HasTile(nearbyCoordinate))
            {
                highlightTiles[i].transform.position = tilemapReader.ChangeLocalToWorldPosition(nearbyCoordinate);
                highlightTiles[i].Show();
            }
            
        }

        yield return navigation.WaitForClickDestination();

        var destination = navigation.Destination;

        yield return navigation.GoDestination(end: destination, target: transform);

        for (int i = 0; i < highlightTiles.Length; i++)
        {
            highlightTiles[i].Hide();
        }
    }

    private void Awake()
    {
        Actor = this.gameObject;
        navigation = new Navigation(tilemapReader);

        dices[0] = new Dice<int>(new int[6] { 1, 2, 3, 4, 5, 6 });
        dices[1] = new Dice<int>(new int[6] { 1, 2, 3, 4, 5, 6 });
        var movePoint = dices[0].GetRandomValue() + dices[1].GetRandomValue();

        highlightTiles = new HighlightTile[4];

        for(int i=0; i<highlightTiles.Length; i++)
        {
            highlightTiles[i] = Instantiate(highlightTilePrefab.gameObject).GetComponent<HighlightTile>();
            highlightTiles[i].clickEvent = navigation.CreateDestination;
        }
        
    }
}

