﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpDataProcessor.CommandLine
{
    /// <summary>
    /// Parsers commands given to this data processor.
    /// </summary>
    public static class CommandParser
    {
        /// <summary>
        /// Contains the switches for reading xml.
        /// </summary>
        private static readonly string[] ReadXmlSwitches = new string[] { "-rx", "--read-xml" };

        /// <summary>
        /// Contains the switches for writing xml.
        /// </summary>
        private static readonly string[] WriteXmlSwitches = new string[] { "-wx", "--write-xml" };

        /// <summary>
        /// Contains the switches for reading pbf.
        /// </summary>
        private static readonly string[] ReadPBFSwitches = new string[] { "-rp", "--read-pbf" };

        /// <summary>
        /// Contains the switches for sorting.
        /// </summary>
        private static readonly string[] SortSwitches = new string[] { "-so", "--sort" };

        /// <summary>
        /// Contains the switches for  bounding box.
        /// </summary>
        private static readonly string[] BoundingBoxSwitches = new string[] { "-bb", "--bounding-box" };

        /// <summary>
        /// Returns true if the given string can be a switch.
        /// </summary>
        /// <param name="switchCommand"></param>
        /// <returns></returns>
        private static bool IsSwitch(string switchCommand)
        {
            // make sure no caps mess things up.
            switchCommand = switchCommand.ToLower();
            switchCommand = switchCommand.Trim();

            // test read-xml.
            if (ReadXmlSwitches.Contains(switchCommand))
            { // commmand can be a read-xml.
                return true;
            }

            // test write-xml.
            if (WriteXmlSwitches.Contains(switchCommand))
            { // command can be a write-xml.
                return true;
            }

            // test read-pbf.
            if (ReadPBFSwitches.Contains(switchCommand))
            { // commmand can be a read-xml.
                return true;
            }

            // test sorting.
            if (SortSwitches.Contains(switchCommand))
            { // commmand can be a read-xml.
                return true;
            }

            // test bounding-box.
            if (BoundingBoxSwitches.Contains(switchCommand))
            { // commmand can be a read-xml.
                return true;
            }

            return false; // no switch found!
        }

        /// <summary>
        /// Parses the command line arguments into a sorted list of commands.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Command[] ParseCommands(string[] args)
        {
            // initialize the list of commands.
            var commands = new List<Command>();

            // start parsing arguments one-by-one.
            int idx = 0;
            while (idx < args.Length)
            { // check the next argument for a switch.
                if (!CommandParser.IsSwitch(args[idx]))
                {
                    throw new CommandLineParserException(args[idx], "Invalid switch!");
                }

                // parse the command.
                Command command;
                int eatenArguments = CommandParser.ParseCommand(args, idx, out command);

                // increase idx.
                idx = idx + eatenArguments;

                // add resulting command.
                commands.Add(command);
            }

            return commands.ToArray();
        }

        /// <summary>
        /// Parses one commands and returns the number of eaten arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static int ParseCommand(string[] args, int idx, out Command command)
        {
            int eatenArguments = 1; // eat the first argument.
            string switchCommand = args[idx]; // get the switch command.

            // make sure no caps mess things up.
            switchCommand = switchCommand.ToLower();
            switchCommand = switchCommand.Trim();

            // test read-xml.
            if (ReadXmlSwitches.Contains(switchCommand))
            { // commmand can be a read-xml.
                eatenArguments = eatenArguments +
                                 CommandReadXml.Parse(args, idx + 1, out command);
                return eatenArguments;
            }

            // test write-xml.
            if (WriteXmlSwitches.Contains(switchCommand))
            { // command can be a write-xml.
                eatenArguments = eatenArguments +
                                 CommandWriteXml.Parse(args, idx + 1, out command);
                return eatenArguments;
            }

            // test read-pbf.
            if (ReadPBFSwitches.Contains(switchCommand))
            { // command can be a read-pbf.
                eatenArguments = eatenArguments +
                                 CommandReadPBF.Parse(args, idx + 1, out command);
                return eatenArguments;
            }

            // test sorting.
            if (SortSwitches.Contains(switchCommand))
            { // command can be a sorting.
                eatenArguments = eatenArguments +
                                 CommandFilterSort.Parse(args, idx + 1, out command);
                return eatenArguments;
            }

            // test bounding-box.
            if (BoundingBoxSwitches.Contains(switchCommand))
            { // command can be a sorting.
                eatenArguments = eatenArguments +
                                 CommandFilterBoundingBox.Parse(args, idx + 1, out command);
                return eatenArguments;
            }

            throw new CommandLineParserException(args[idx], "Switch not found!");
        }

        /// <summary>
        /// Remove quotes from strings if they occur at exactly the beginning and end.
        /// </summary>
        /// <param name="stringToParse"></param>
        /// <returns></returns>
        public static string RemoveQuotes(string stringToParse)
        {
            if (string.IsNullOrEmpty(stringToParse))
            {
                return stringToParse;
            }

            if (stringToParse.Length < 2)
            {
                return stringToParse;
            }

            if (stringToParse[0] == '"' && stringToParse[stringToParse.Length - 1] == '"')
            {
                return stringToParse.Substring(1, stringToParse.Length - 2);
            }

            if (stringToParse[0] == '\'' && stringToParse[stringToParse.Length - 1] == '\'')
            {
                return stringToParse.Substring(1, stringToParse.Length - 2);
            }

            return stringToParse;
        }
    }
}
