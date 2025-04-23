using System;

// Space Engineers DLLs
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using VRageMath;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;

/*
 * Must be unique per each script project.
 * Prevents collisions of multiple `class Program` declarations.
 * Will be used to detect the ingame script region, whose name is the same.
 */
namespace SolarTracker {

/*
 * Do not change this declaration because this is the game requirement.
 */
public sealed class Program : MyGridProgram {

    /*
     * Must be same as the namespace. Will be used for automatic script export.
     * The code inside this region is the ingame script.
     */
    #region SolarTracker

IMyMotorStator azimuthRotor;
IMyMotorStator elevationHinge;
IMySolarPanel elevationPanel;
IMySolarPanel azimuthPanel;
IMyBlockGroup solarHingesGroup;
IMyBlockGroup solarRotorsGroup;
List<IMyMotorStator> mySolarHinges;
List<IMyMotorStator> mySolarRotors;
float lastAzimuthOutput = 0f;
float lastElevationOutput = 0f;
// Tracking directions
int azimuthDirection = 1;
int elevationDirection = 1;

// Settings
const float rpm = 0.05f;
const float stepDelaySeconds = 0.5f; // How often to check output and change direction
double lastStepTime = 0;

public Program()
{
    azimuthRotor = GridTerminalSystem.GetBlockWithName("Azimuth Rotor") as IMyMotorStator;
    elevationHinge = GridTerminalSystem.GetBlockWithName("Elevation Hinge") as IMyMotorStator;
    azimuthPanel = GridTerminalSystem.GetBlockWithName("Azimuth Panel") as IMySolarPanel;
    elevationPanel = GridTerminalSystem.GetBlockWithName("Elevation Panel") as IMySolarPanel;
    solarHingesGroup = GridTerminalSystem.GetBlockGroupWithName("Solar Hinges") as IMyBlockGroup;
    solarRotorsGroup = GridTerminalSystem.GetBlockGroupWithName("Solar Rotors") as IMyBlockGroup;

    if (azimuthRotor == null || elevationHinge == null || azimuthPanel == null || elevationPanel == null)
    {
        Echo("Missing one or more components!");
        return;
    }

    if (solarHingesGroup != null && solarRotorsGroup != null){
        mySolarHinges = new List<IMyMotorStator>();
        solarHingesGroup.GetBlocksOfType(mySolarHinges);
        mySolarRotors = new List<IMyMotorStator>();
        solarRotorsGroup.GetBlocksOfType(mySolarRotors);
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    Echo("Initialized dual-axis solar tracker");
}

public void Main(string argument, UpdateType updateSource)
{
    if (azimuthRotor == null || elevationHinge == null || azimuthPanel == null || elevationPanel == null)
    {
        Echo("Missing components");
        return;
    } 

    double now = Runtime.TimeSinceLastRun.TotalSeconds + lastStepTime;
    float azimuthOutput = azimuthPanel.MaxOutput;
    float elevationOutput = elevationPanel.MaxOutput;

    if (elevationOutput == 0 && azimuthOutput == 0)
    {
        Echo("No Solar Output Returning To Home Position.");

        ResetToZero(azimuthRotor);
        ResetToZero(elevationHinge);

        foreach (var hinge in mySolarHinges)
        {
            ResetToZero(hinge);
        }

        foreach (var rotor in mySolarRotors)
        {
            ResetToZero(rotor);
        }

    }
    else if (now >= stepDelaySeconds)
    {

        // Reverse azimuth direction if output decreased
        if (azimuthOutput < lastAzimuthOutput)
        {
            azimuthDirection *= -1;
        } 
        lastAzimuthOutput = azimuthOutput;      
        // Reverse elevation direction if output decreased
        if (elevationOutput < lastElevationOutput)
        {
            elevationDirection *= -1;
        } 
        lastElevationOutput = elevationOutput;
        // Apply rotation
        azimuthRotor.TargetVelocityRPM = azimuthDirection * rpm;
        elevationHinge.TargetVelocityRPM = elevationDirection * rpm;

        //Apply rotation to the rest of the grid
        foreach(var hinge in mySolarHinges){
            hinge.TargetVelocityRPM = elevationDirection * rpm;
        }
        foreach(var rotor in mySolarRotors){
            rotor.TargetVelocityRPM = azimuthDirection * rpm;
        }

        lastStepTime = 0;
    }
    else
    {
        lastStepTime += Runtime.TimeSinceLastRun.TotalSeconds;
    }


    Echo($"Azimuth Output: {azimuthPanel.MaxOutput * 1000} kW");
    Echo($"Elevation Output: {elevationPanel.MaxOutput * 1000} kW");
    Echo($"Az Dir: {azimuthDirection}, Elv Dir: {elevationDirection}");

}
void ResetToZero(IMyMotorStator rotor, float speedRpm = 0.3f, float toleranceDeg = 1f)
{
    float angleDeg = rotor.Angle * (180f / (float)Math.PI);
    float direction = -Math.Sign(angleDeg); // if angle is positive, go negative, and vice versa

    if (!(Math.Abs(angleDeg) < toleranceDeg))
    {
        rotor.TargetVelocityRPM = direction * speedRpm;
    }
    else
    {
        rotor.TargetVelocityRPM = 0f; // Stop if movement 
    }
}


    #endregion // SolarTracker
}}