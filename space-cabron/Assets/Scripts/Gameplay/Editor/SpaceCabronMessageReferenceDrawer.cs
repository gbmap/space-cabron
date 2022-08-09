using System.Collections;
using System.Collections.Generic;
using SpaceCabron.Gameplay;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpaceCabronMessageReference))]
public class SpaceCabronMessageReferenceDrawer : MessageReferenceDrawer<SpaceCabron.Messages.MsgGameOver>
{
}
