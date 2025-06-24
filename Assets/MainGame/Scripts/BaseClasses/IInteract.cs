
namespace StarMaps {
    using System.Collections.Generic;
    using System.Diagnostics;
    using Unity.VisualScripting.FullSerializer;
    using UnityEngine;

    public interface IInteract {
        void Interact();
    }
    public interface IPool {
        void OnPooled();
    }
    public interface IPath {
        void DisplayPath();
        void ResetPaths();
    }
}