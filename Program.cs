using Spectre.Console;
using HitsterCardGenerator.UI;
using HitsterCardGenerator.Models;

// Create wizard state
var wizardState = new WizardState();

// Create step menu
var stepsPanel = StepMenu.Render(wizardState);

// Create content panel (placeholder for now)
var contentPanel = new Panel("Welcome to Hitster Card Generator!\n\nThis wizard will guide you through creating custom Hitster cards.")
    .Header("Content")
    .Border(BoxBorder.Rounded);

// Render the layout
AppLayout.Render(stepsPanel, contentPanel);
