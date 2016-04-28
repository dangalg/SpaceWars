using UnityEngine;
using System.Collections;

public interface ITarget
{
	System.Action targetDestroyed { get; set; }
}
