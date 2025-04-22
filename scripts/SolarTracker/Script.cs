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
IMyMotorStator elevationRotor;
IMySolarPanel solarPanel;

float lastOutput = 0f;

// Tracking directions
int azimuthDirection = 1;
int elevationDirection = 1;

// Settings
const float rpm = 0.3f;
const float stepDelaySeconds = 1.0f; // How often to check output and change direction
double lastStepTime = 0;

public Program()
{
    azimuthRotor = GridTerminalSystem.GetBlockWithName("Azimuth Rotor") as IMyMotorStator;
    elevationRotor = GridTerminalSystem.GetBlockWithName("Elevation Rotor") as IMyMotorStator;
    solarPanel = GridTerminalSystem.GetBlockWithName("Tracked Panel") as IMySolarPanel;

    if (azimuthRotor == null || elevationRotor == null || solarPanel == null)
    {
        Echo("Missing one or more components!");
        return;
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    Echo("Initialized dual-axis solar tracker");
}

public void Main(string argument, UpdateType updateSource)
{
    if (azimuthRotor == null || elevationRotor == null || solarPanel == null)
    {
        Echo("Missing blocks");
        return;
    }

    double now = Runtime.TimeSinceLastRun.TotalSeconds + lastStepTime;

    if (now >= stepDelaySeconds)
    {
        float currentOutput = solarPanel.CurrentOutput;

        // Reverse direction if output decreased
        if (currentOutput < lastOutput)
        {
            azimuthDirection *= -1;
            elevationDirection *= -1;
        }

        lastOutput = currentOutput;
        lastStepTime = 0;
    }
    else
    {
        lastStepTime += Runtime.TimeSinceLastRun.TotalSeconds;
    }

    // Apply rotation
    azimuthRotor.TargetVelocityRPM = azimuthDirection * rpm;
    elevationRotor.TargetVelocityRPM = elevationDirection * rpm;

    Echo($"Output: {solarPanel.CurrentOutput:F2} kW");
    Echo($"Az Dir: {azimuthDirection}, Elv Dir: {elevationDirection}");
}



    #endregion // SolarTracker
}}