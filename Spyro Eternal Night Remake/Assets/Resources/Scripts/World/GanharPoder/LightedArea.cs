using UnityEngine;

public class LightedArea : MonoBehaviour
{
    public GameObject[] torches;
    public GameObject door;
    public GameObject[] platforms;

    void Update()
    {
        CheckAllTorchesActive();

        if (CheckAllTorchesActive())
        {
            door.SetActive(false);

            foreach (GameObject platform in platforms)
            {
                platform.SetActive(true);
            }
        }
    }

    bool CheckAllTorchesActive()
    {
        foreach (GameObject torch in torches)
        {
            if (torch != null && !torch.GetComponent<PedestalFogo>().ativo)
            {
                return false;
            }
        }

        return true;
    }
}
