using System.Collections.Generic;
using UnityEngine;

public class BubblesManager : MonoBehaviour {
    public static BubblesManager instance { private set; get; }

    [SerializeField] private Bubble bubblePrefab; // Prefab for the bubble
    [SerializeField] private float divisionSizeFactor = 0.5f; // Size factor for divided bubbles
    //[SerializeField] private float mergeSizeFactor = 2f; // Size factor for merged bubbles
    [SerializeField] private List<Bubble> bubbles; // List of active bubbles

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one BubbleManager at once time!");
        }
        else
        {
            instance = this;
        }
    }

    public void DivideBubble(Bubble bubble)
    {
        if (!bubbles.Contains(bubble)) return;
        Vector2 originalVelocity = bubble.GetVelocity();

        // Calculate new size
        Vector3 originalPosition = bubble.transform.position;
        float originalSize = bubble.transform.localScale.x;
        float newSize = originalSize * divisionSizeFactor;

        if (newSize < 0.3f) return; // Prevent bubbles from becoming too small

        // Create two new bubbles
        Vector3 offset = new Vector3(newSize / 2 + 0.05f, newSize / 2 + 0.05f, 0);
        CreateBubbleWithVelocity(originalPosition + offset, newSize, originalVelocity + new Vector2(1f, 0.5f));
        CreateBubbleWithVelocity(originalPosition - offset, newSize, originalVelocity + new Vector2(-1f, -0.5f));

        // Destroy the original bubble
        DestroyBubbleFromList(bubble);
    }


    private Bubble CreateBubble(Vector3 position, float size)
    {
        Bubble newBubble = Instantiate(bubblePrefab, position, Quaternion.identity);
        newBubble.transform.localScale = new Vector3(size, size, 1);
        bubbles.Add(newBubble);
        return newBubble;
    }
    private void CreateBubbleWithVelocity(Vector3 position, float size, Vector2 velocity)
    {
        Bubble newBubble = CreateBubble(position, size);
        newBubble.SetVelocity(velocity);
    }

    public void OnBubbleCollide(Bubble firstBubble, Bubble secondBubble)
    {
        if (firstBubble && secondBubble && bubbles.Contains(firstBubble) && bubbles.Contains(secondBubble))
        {
            UniteBubbles(firstBubble, secondBubble);
        }
    }

    private void UniteBubbles(Bubble firstBubble, Bubble secondBubble)
    {

        // Calculate new size
        float size1 = firstBubble.transform.localScale.x;
        float size2 = secondBubble.transform.localScale.x;

        //Calculate new velocity
        float mass1 = size1 * size1; // Assuming mass is proportional to area
        float mass2 = size2 * size2;
        float newSize = Mathf.Sqrt(size1 * size1 + size2 * size2);
        Vector2 combinedVelocity = (firstBubble.GetVelocity() * mass1 + secondBubble.GetVelocity() * mass2) / (mass1 + mass2);

        // Create a new united bubble
        Vector3 averagePosition = (firstBubble.transform.position + secondBubble.transform.position) / 2;
        CreateBubbleWithVelocity(averagePosition, newSize, combinedVelocity);

        // Destroy the original bubbles
        DestroyBubbleFromList(firstBubble);
        DestroyBubbleFromList(secondBubble);
    }

    private void DestroyBubbleFromList(Bubble bubble)
    {
        bubbles.Remove(bubble);
        bubble.DestroyMyself();
    }

    public List<Bubble> GetBubbleList() { return bubbles; }
}
