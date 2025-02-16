using System;
using System.Collections.Generic;

[Serializable] 
public class TutorialSettings
{
    public List<int> CompletedTutorials;
    public bool TutorialEnabled;

    public TutorialSettings(List<int> completedTutorials, bool tutorialEnabled)
    {
        CompletedTutorials = completedTutorials;
        TutorialEnabled = tutorialEnabled;
    }
}
