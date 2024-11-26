using UnityEngine;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour
{
    public GameObject instructionPanel;

    void Start()
    {
        // Make sure the instruction panel is visible at the start
        instructionPanel.SetActive(true);
    }

    void Update()
    {
        // Check if Enter is pressed to hide the instruction panel
        if (Input.GetKeyDown(KeyCode.Return))
        {
            instructionPanel.SetActive(false);
        }

        // Check if Escape is pressed to exit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}