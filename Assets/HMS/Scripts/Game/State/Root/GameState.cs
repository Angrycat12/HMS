using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
     public List<WorldData> Worlds { get; set; }
     public List<ResourceData> Resources { get; set; }
}