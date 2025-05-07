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
namespace  PlanetaryThrustControll {

/*
 * Do not change this declaration because this is the game requirement.
 */
public sealed class Program : MyGridProgram {

    /*
     * Must be same as the namespace. Will be used for automatic script export.
     * The code inside this region is the ingame script.
     */
    #region  PlanetaryThrustControll

    /*
     * The constructor, called only once every session and always before any 
     * other method is called. Use it to initialize your script. 
     *    
     * The constructor is optional and can be removed if not needed.
     *
     * It's recommended to set RuntimeInfo.UpdateFrequency here, which will 
     * allow your script to run itself without a timer block.
     */

     List<IMyGyro> myGyros;
     List<IMyMotorStator> myRotors;
     List<IMyMotorStator > myHinges;
     List<IMyThrust> myThrusters;
     IMyControlPanel myControlPanel;
     IMyShipController myShipController;

     void GetGroupBlocks<T>(string groupName, List<T> listOut) where T : class, IMyTerminalBlock
     {
        listOut.Clear();
        var grp = GridTerminalSystem.GetBlockGroupWithName(groupName);
        if (grp != null)
            grp.GetBlocksOfType(listOut);
     }
     

    public Program() 
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;

        myShipController = GridTerminalSystem.GetBlockWithName("Main Seat") as IMyShipController;

        myGyros = new List<IMyGyro>();
        myThrusters = new List<IMyThrust>();
        myHinges = new List<IMyMotorStator>();
        myRotors = new List<IMyMotorStator>();

        GetGroupBlocks("Gyros", myGyros);
        GetGroupBlocks("Atmospheric Thrusters", myThrusters);
        GetGroupBlocks("Thruster Hinges", myHinges);
        GetGroupBlocks("Thruster Rotors", myRotors);    

        // Check number of components
        Echo($"Gyros: {myGyros.Count}");
        Echo($"Atmospheric Thrusters: {myThrusters.Count}");
        Echo($"Thruster Rotors: {myRotors.Count}");    
        Echo($"Thruster Hinges: {myHinges.Count}");    
    }


    public void Save() {}

    /*
     * The main entry point of the script, invoked every time one of the 
     * programmable block's Run actions are invoked, or the script updates 
     * itself. The updateSource argument describes where the update came from.
     * 
     * The method itself is required, but the arguments above can be removed 
     * if not needed.
     */
    public void Main(string argument, UpdateType updateSource) {}

    #endregion //  PlanetaryThrustControll
}}