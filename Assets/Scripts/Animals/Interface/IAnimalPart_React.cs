using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimalPart_React
{
    List<AnimalType> ReactSources { get; }
    AnimalFinishAction_Base ReactAction { get; }
    Animal_AI_Base TheAnimal { get; }
}
