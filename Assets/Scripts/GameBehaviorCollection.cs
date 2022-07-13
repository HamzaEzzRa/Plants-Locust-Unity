using System.Collections.Generic;

[System.Serializable]
public class GameBehaviorCollection
{
    private List<GameBehavior> behaviors = new List<GameBehavior>();

    public void Add(GameBehavior behavior) { behaviors.Add(behavior); }

    public void GameUpdate()
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].GameUpdate())
            {
                int lastIndex = behaviors.Count - 1;
                behaviors[i] = behaviors[lastIndex];
                behaviors.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }
}