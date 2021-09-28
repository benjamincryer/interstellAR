public class UnitSelector : Selector<Unit>
{
    public override void PrintInformation()
    {
        //Print out unit information
        if (selectedText)
        {
            selectedText.text = "Selected Units";
            foreach (Unit unit in SelectedObjects)
            {
                selectedText.text +=
                    "\nUnit Name: " + SelectedObject.UnitName +
                    "\nHealth: " + SelectedObject.Health +
                    "\nDamage: " + SelectedObject.Damage +
                    "\nSpeed: " + SelectedObject.Speed +
                    "\n --------------------";
            }
        }
    }
}