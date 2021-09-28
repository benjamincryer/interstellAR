using UnityEngine;
using UnityEngine.UI;

public class FormationSelector : Selector<Formation>
{
    public override void PrintInformation()
    {
        foreach (GameObject textObj in GameObject.FindGameObjectsWithTag("FormationText"))
        {
            Destroy(textObj);
        }
        //RemoveUnselectedFormations();
        int offset = 0;
        foreach (Formation form in SelectedObjects)
        {
            CreateFormationTextObject(form, offset);
            offset++;
        }
    }

    private void RemoveUnselectedFormations()
    {
        bool flag = true;
        foreach (GameObject textObj in GameObject.FindGameObjectsWithTag("FormationText"))
        {
            foreach (Formation form in SelectedObjects)
            {
                if (CheckFormationTextIsFormation(form, textObj))
                    flag = false; break;
            }
            if (flag)
                Destroy(textObj);
        }
    }

    private void CreateFormationTextObject(Formation form, int offset)
    {
        if(form != null)
        {
            //Create new text object and set values
            GameObject newText = Instantiate(GameObject.Find("GameController").GetComponent<ResourceFinder>().selectedFormationTextPrefab, GameObject.FindGameObjectWithTag("FormationTexts").transform);
            newText.transform.localPosition = new Vector2(
                newText.transform.localPosition.x + (newText.GetComponent<RectTransform>().rect.width) * offset,
                newText.transform.localPosition.y);
            Text[] texts = newText.GetComponentsInChildren<Text>();
            texts[0].text = form.name;
            texts[1].text = "Health: " + form.Health.ToString();
            texts[2].text = "Attack: " + form.Attack.ToString();
            texts[3].text = "Speed: " + form.AverageSpeed.ToString();
        }

    }

    //Checks a formation if it is equivalent to a formation text object
    private bool CheckFormationTextIsFormation(Formation form, GameObject textObject)
    {
        Text[] texts = textObject.GetComponentsInChildren<Text>();
        if (form.name == texts[0].text
            && "Health: " + form.Health.ToString() == texts[1].text
            && "Attack: " + form.Attack.ToString() == texts[2].text
            && "Speed: " + form.AverageSpeed.ToString() == texts[3].text
            )
        {
            return true;
        }
        return false;
    }

    //Checks a formation to see if its presently loaded as a formation text
    private int CheckFormationAlreadyInTextForm(Formation form)
    {
        int offset = 0;
        GameObject[] selectedTexts = GameObject.FindGameObjectsWithTag("FormationText");
        foreach (GameObject textObject in selectedTexts)
        {
            if (CheckFormationTextIsFormation(form, textObject))
            {
                return -1;
            }
            offset++;
        }
        return offset;
    }
}