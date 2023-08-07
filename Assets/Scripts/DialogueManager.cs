using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Json")] 
    [SerializeField] private TextAsset jsonAsset;

    [Header("UI Components")] 

    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI speakerWords;
    [SerializeField] private List<GameObject> choices;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private GameObject choicesGroup;
    [SerializeField] private float choicesSpacing;

    private Story _story;
    private bool _choosingInterval;
    private bool _isChoosing;

    private void SetChildrenActive(GameObject gameObject, bool active)
    {
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            child.SetActive(active);
        }
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _story = new Story(jsonAsset.text);
        SetChildrenActive(gameObject, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetButtonDown("Submit")) return;

        if (_story.canContinue)
        {
            SetChildrenActive(gameObject, true);
            _story.Continue();
            speakerName.text = "ABC";
            speakerWords.text = _story.currentText;
            if (_story.currentChoices.Count == 0) return;
            PaintButtons();

            return;
        }
        SetChildrenActive(gameObject, false);
    }

    private void ClearAllChoices()
    {
        foreach (var choice in choices)
        {
            Destroy(choice);
        }
        choices.RemoveRange(0, choices.Count);
    }

    private void PaintButtons()
    {
        var index = 0;
        foreach (var currentChoice in _story.currentChoices)
        {
            var choice = Instantiate(choicePrefab, choicesGroup.transform);
            choice.transform.position = new Vector2(0, index * choicesSpacing);
            choice.GetComponentInChildren<TextMeshProUGUI>().text = currentChoice.text;
            var index1 = index;
            choice.GetComponent<Button>().onClick.AddListener(() =>
            {
                _story.ChooseChoiceIndex(index1);
                _story.Continue();
                _story.Continue();
                speakerName.text = "ABC";
                speakerWords.text = _story.currentText;
                ClearAllChoices();
            });
            choices.Add(choice);
            index++;
        }
    }
}
