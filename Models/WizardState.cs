namespace HitsterCardGenerator.Models;

public class WizardState
{
    public Step CurrentStep { get; private set; }
    public HashSet<Step> CompletedSteps { get; private set; }

    public WizardState()
    {
        CurrentStep = Step.ImportCsv;
        CompletedSteps = new HashSet<Step>();
    }

    public void AdvanceToNextStep()
    {
        // Mark current step as completed
        CompletedSteps.Add(CurrentStep);

        // Move to next step
        var nextStepValue = (int)CurrentStep + 1;
        if (nextStepValue <= (int)Step.ExportPdf)
        {
            CurrentStep = (Step)nextStepValue;
        }
    }

    public bool IsStepCompleted(Step step)
    {
        return CompletedSteps.Contains(step);
    }

    public bool IsCurrentStep(Step step)
    {
        return CurrentStep == step;
    }
}
