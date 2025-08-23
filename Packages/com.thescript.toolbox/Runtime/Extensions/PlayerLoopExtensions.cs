using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class PlayerLoopExtensions
    {
        /// <summary>
        /// Inserts the new system as the first child of the target system
        /// </summary>
        public static void InsertFirst(this ref PlayerLoopSystem system, PlayerLoopSystem newSystem, bool allowDuplicate = false, bool logModifications = false)
        {
            using var subSystems = ListPool.Get(system.subSystemList);
            if (!allowDuplicate && subSystems.Any(s => s.type == newSystem.type))
            {
                return;
            }

            subSystems.Insert(0, newSystem);
            system.subSystemList = subSystems.ToArray();

            if (logModifications)
            {
                Debug.Log($"[Mtl.Toolbox] Injecting '{newSystem.type.Name}' subsystem as first of '{system.type.Name}'");
            }
        }

        /// <summary>
        /// Inserts the new system as the last child of the target system
        /// </summary>
        public static void InsertLast(this ref PlayerLoopSystem system, PlayerLoopSystem newSystem, bool allowDuplicate = false, bool logModifications = false)
        {
            using var subSystems = ListPool.Get(system.subSystemList);
            if (!allowDuplicate && subSystems.Any(s => s.type == newSystem.type))
            {
                return;
            }

            subSystems.Add(newSystem);
            system.subSystemList = subSystems.ToArray();

            if (logModifications)
            {
                Debug.Log($"[Mtl.Toolbox] Injecting '{newSystem.type.Name}' subsystem as last of '{system.type.Name}'");
            }
        }

        /// <summary>
        /// Inserts the new system before target system type (this operation is recursive on the target system)
        /// </summary>
        public static bool InsertBefore(this ref PlayerLoopSystem system, Type beforeType, PlayerLoopSystem newSystem, bool allowDuplicate = false, bool logModifications = false)
        {
            var currentSubSystems = system.subSystemList;
            for (int i = 0, iMax = currentSubSystems.Length; i < iMax; ++i)
            {
                var subsystem = currentSubSystems[i];
                if (subsystem.type == beforeType)
                {
                    using var subSystems = ListPool.Get(currentSubSystems);
                    if (!allowDuplicate && subSystems.Any(s => s.type == newSystem.type))
                    {
                        return false;
                    }

                    subSystems.Insert(i, newSystem);
                    system.subSystemList = subSystems.ToArray();

                    if (logModifications)
                    {
                        Debug.Log(system.type == null
                            ? $"[Mtl.Toolbox] Injecting '{newSystem.type.Name}' subsystem after '{beforeType.Name}' in Root System List"
                            : $"[Mtl.Toolbox] Injecting '{newSystem.type.Name}' subsystem after '{beforeType.Name}' in '{system.type.Name}'");
                    }

                    return true;
                }

                if (InsertBefore(ref subsystem, beforeType, newSystem, logModifications))
                {
                    currentSubSystems[i] = subsystem;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Inserts the new system after target system type (this operation is recursive on the target system)
        /// </summary>
        public static bool InsertAfter(this ref PlayerLoopSystem system, Type afterType, PlayerLoopSystem newSystem, bool allowDuplicate = false, bool logModifications = false)
        {
            var currentSubSystems = system.subSystemList;
            if (currentSubSystems == null)
            {
                return false;
            }

            for (int i = 0, iMax = currentSubSystems.Length; i < iMax; ++i)
            {
                var subsystem = currentSubSystems[i];
                if (subsystem.type == afterType)
                {
                    using var subSystems = ListPool.Get(currentSubSystems);
                    if (!allowDuplicate && subSystems.Any(s => s.type == newSystem.type))
                    {
                        return false;
                    }

                    subSystems.Insert(i + 1, newSystem);
                    system.subSystemList = subSystems.ToArray();

                    if (logModifications)
                    {
                        Debug.Log(system.type == null
                            ? $"[Mtl.Toolbox] Injecting '{newSystem.type.Name}' subsystem after '{afterType.Name}' in Root System List"
                            : $"[Mtl.Toolbox] Injecting '{newSystem.type.Name}' subsystem after '{afterType.Name}' in '{system.type.Name}'");
                    }

                    return true;
                }

                if (InsertAfter(ref subsystem, afterType, newSystem, logModifications))
                {
                    currentSubSystems[i] = subsystem;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Outputs the full system hierarchy to the Debug console 
        /// </summary>
        /// <param name="system">The system to log</param>
        /// <param name="message">An optional message to prepend to the output</param>
        public static void LogSystem(this in PlayerLoopSystem system, string message = null)
        {
            var builder = new StringBuilder();
            RecursiveLog(in system, builder, 0);
            Debug.Log(string.IsNullOrEmpty(message) ? $"Player Loop:{builder}" : $"{message}{builder}");
        }

        /// <summary>
        /// Internal recursive loop to build the output log
        /// </summary>
        private static void RecursiveLog(this in PlayerLoopSystem system, StringBuilder builder, int depth)
        {
            builder.AppendLine($"{new string('\t', depth)}{system.type}");
            if (!(system.subSystemList?.Length > 0))
            {
                return;
            }

            for (int i = 0, iMax = system.subSystemList.Length; i < iMax; ++i)
            {
                RecursiveLog(in system.subSystemList[i], builder, depth + 1);
            }
        }
    }
}